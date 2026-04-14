using Comments.Api.Middleware;
using Comments.Application.Interfaces;
using Comments.Application.Services;
using Comments.Infrastructure.Data;
using Comments.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;

var builder = WebApplication.CreateBuilder();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()

      // Audit logs  (file only)
      .WriteTo.Logger(audit => audit
          .MinimumLevel.Information() // Audit logs are always Information level
          .Filter.ByIncludingOnly(Matching.WithProperty("AuditUser"))
          .WriteTo.Async(a => a.File(
              "logs/audit-user-.txt",
              rollingInterval: RollingInterval.Day,
              buffered: false)) //for production, to set buffered: true
                                //for dev, to set buffered: false, to display logs immediately
      )

    // Technical logs (console and file), excluding audit
    // Log level is taken from configuration file appsettings.json
    .WriteTo.Logger(tech => tech
        .Filter.ByExcluding(Matching.WithProperty("AuditUser"))
        .Filter.ByExcluding(logEvent => // to avoid duplication of logs
            logEvent.MessageTemplate.Text.Contains("An unhandled exception has occurred"))
        .WriteTo.Async(a => a.File(
            "logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            buffered: true))
        .WriteTo.Console()
    )

    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<CommentsDbContext>(options => 
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }
    );
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<TextFileService>();
builder.Services.AddSingleton<UploadFolders>();

const string frontendCors = "frontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCors, policy =>
    {
        policy
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors(frontendCors);

app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;

    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    headers["Cross-Origin-Opener-Policy"] = "same-origin";
    headers["Cross-Origin-Resource-Policy"] = "cross-origin";

    await next();
});

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project_Comments v1"));

    // In development, remove CSP to allow Swagger UI to function properly
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Remove("Content-Security-Policy");
        await next();
    });
}

app.UseUploads();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CommentsDbContext>();

    // For development purposes only: reset database
    //  db.Database.EnsureDeleted();
    // db.Database.EnsureCreated();

    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
        db.Database.Migrate();

    Console.WriteLine("DB Connection: " + db.Database.CanConnect());
}

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" })).WithTags("Health");

app.Run();
