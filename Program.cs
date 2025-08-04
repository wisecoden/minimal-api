using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.Services;
using MinimalApi.Domain.ViewModels;
using MinimalApi.DTOs;
using MinimalApi.infra.Db;

#region Builder Configuration
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});


var app = builder.Build();

#endregion

#region Home Page
app.MapGet("/", () => Results.Json(new HomeViewModel())).WithTags("Home");
#endregion

#region Admins

app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    if (adminService.Login(loginDTO) != null)
        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
}).WithTags("Admins");
#endregion
#region Cars

app.MapPost("/cars", ([FromBody] CarDTO carDTO, ICarService carService) =>
{
    var car = new Car
    {
        Name = carDTO.Name,
        Brand = carDTO.Brand,
        Age = carDTO.Age
    };
    carService.Add(car);

    return Results.Created($"/car/{car.Id}", car);
}).WithTags("Cars");

app.MapGet("/cars", ([FromQuery] int? page, ICarService carService) =>
{
    var cars = carService.GetAllCars(page);
    return Results.Ok(cars);
}).WithTags("Cars");

app.MapGet("/cars/{id}", ([FromRoute]int id, ICarService carService) =>
{
    var car = carService.GetCarById(id);

    if (car == null) return Results.NotFound("Car not found");

    return Results.Ok(car);
}).WithTags("Cars");

app.MapPut("/cars/{id}", ([FromBody] CarDTO carDTO, [FromRoute] int id, ICarService carService) =>
{
    var car = carService.GetCarById(id);
    if (car == null) return Results.NotFound("Car not found");

    car.Name = carDTO.Name;
    car.Brand = carDTO.Brand;
    car.Age = carDTO.Age;

    carService.Update(car);
    return Results.Ok(car);
}).WithTags("Cars");

app.MapDelete("/cars/{id}", ([FromRoute] int id, ICarService carService) =>
{
    var car = carService.GetCarById(id);
    if (car == null) return Results.NotFound("Car not found");

    carService.Remove(car);
    return Results.NoContent();
}).WithTags("Cars");
#endregion


#region App
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
#endregion