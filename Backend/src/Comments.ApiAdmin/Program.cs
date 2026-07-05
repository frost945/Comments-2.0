
using Comments.Api;
using Comments.Application;
using Comments.Infrastructure;
using Comments.Infrastructure.Persistence.Extensions;
using Comments.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder();

builder.Services.AddCommentsDbContext(builder.Configuration);

builder.Services.Configure<StorageOptions>(
    builder.Configuration.GetSection("Storage"));

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddApi();

//только для регистрации сервисов, которые используют IDistributedCache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "CommentsApiAdmin:";
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();