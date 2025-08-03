using Microsoft.EntityFrameworkCore;
using MinimalApi.infra.Db;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

app.MapGet("/", () => "Hello World!");
app.MapPost("/login", (MinimalApi.DTOs.loginDTO loginDTO) =>
{
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Password == "password")
        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
});
app.Run();
