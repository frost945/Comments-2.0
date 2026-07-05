using Comments.Application;
using Comments.Infrastructure;
using Comments.Infrastructure.Persistence.Extensions;
using Comments.Api.Mappers;

var builder = WebApplication.CreateBuilder();

builder.Services.AddCommentsDbContext(builder.Configuration);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<CommentResponseMapper>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();