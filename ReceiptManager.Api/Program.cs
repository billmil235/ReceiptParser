using Microsoft.AspNetCore.Http.Json;
using ReceiptManager.Api;
using ReceiptManager.Api.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAntiforgery();
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