using Comments.Application;
using Comments.Infrastructure;
using Comments.Infrastructure.Logging;
using Comments.Infrastructure.Persistence.Extensions;
using Comments.Infrastructure.Storage;
using Comments.Api.Middleware;
using Comments.Api;
using StackExchange.Redis;
using Serilog;


var builder = WebApplication.CreateBuilder();

builder.ConfigureSerilog();
builder.Host.UseSerilog();

builder.Services.AddCommentsDbContext(builder.Configuration);

builder.Services.Configure<StorageOptions>(
    builder.Configuration.GetSection("Storage"));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddSwaggerGen();

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

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = new ConfigurationOptions
    {
        EndPoints = { "localhost:6379" },
        AbortOnConnectFail = false,
        ConnectRetry = 0,
        ConnectTimeout = 200,
        SyncTimeout = 200,
        AsyncTimeout = 200
    };
    options.InstanceName = "CommentsApp:";
});

builder.Services.AddApplication();

builder.Services.AddInfrastructure();

builder.Services.AddApi();

var app = builder.Build();

await app.ApplyMigrationsAsync();

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment() || !app.Environment.IsEnvironment("Docker"))
{
    app.UseHsts();
}

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
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project_Comments v2"));

    // In development, remove CSP to allow Swagger UI to function properly
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Remove("Content-Security-Policy");
        await next();
    });
}

app.UseUploads();

app.MapControllers();

app.MapGet("/ping", () => Results.Ok(new { status = "pong" }));

app.Run();
