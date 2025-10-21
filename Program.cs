using Microsoft.OpenApi.Models;
using ProfileApi.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<StringRepository>();
builder.Services.AddSingleton<StringAnalyzerService>();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod());

app.UseAuthorization();
app.MapControllers();

app.Run();
