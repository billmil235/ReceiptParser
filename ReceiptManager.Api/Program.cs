using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.AI;
using ReceiptManager.Api;
using ReceiptManager.Api.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAntiforgery();
builder.Services.AddChatClient(new OllamaChatClient("http://192.168.1.118:11434/", "qwen2.5vl:7b"));
builder.RegisterServices();

builder.Services.Configure<JsonOptions>(opts => opts.SerializerOptions.IncludeFields = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.RegisterReceiptReaderEndpoints();

app.Run();