using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.Services;
using minimal_api.Domain.ViewModels;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.ViewModels;
using MinimalApi.DTOs;
using MinimalApi.infra.Db;

#region Builder Configuration
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
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

app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
}).WithTags("Admins");

app.MapGet("/admins", ([FromQuery] int? page, IAdministratorService administratorService) =>
{
    var admins = new List<AdministratorViewModel>();

    var administrators = administratorService.GetAllAdministrators(page);

    foreach (var adm in administrators)
    {
        admins.Add(new AdministratorViewModel
        {
            Id = adm.Id,
            Email = adm.Email,
            Profile =  adm.Profile
            
        });
    };

    if (admins == null || !admins.Any())
        return Results.NotFound("No admins found");

    return Results.Ok(admins);
}).WithTags("Admins");

app.MapGet("/admins/{id}", ([FromRoute]int id, IAdministratorService administratorService) =>
{
    var administrator = administratorService.GetAdministratorById(id);
    if (administrator == null) return Results.NotFound("No admins found");

    return Results.Ok(new AdministratorViewModel
        {
            Id = administrator.Id,
            Email = administrator.Email,
            Profile =  administrator.Profile
            
        });
}).WithTags("Admins");

app.MapPost("/admins", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
{
    var validations = new ValidationErrosViewModel
    {
        Messages = new List<string>()
    };
    if (string.IsNullOrEmpty(administratorDTO.Email))
        validations.Messages.Add("Email is required");

    if (string.IsNullOrEmpty(administratorDTO.Password))
        validations.Messages.Add("Password is required");
    if (administratorDTO.Profile == null)
        validations.Messages.Add("Profile is required");



    if (validations.Messages.Count() > 0)
        return Results.BadRequest(validations);

    var administrator = new Administrator
    {
        Email = administratorDTO.Email,
        Password = administratorDTO.Password,
        Profile = administratorDTO.Profile.ToString() ?? Profile.Editor.ToString()
    };
   
    administratorService.Add(administrator);

    
    return Results.Created($"/admin/{administrator.Id}", new AdministratorViewModel
        {
            Id = administrator.Id,
            Email = administrator.Email,
            Profile =  administrator.Profile
            
        });
}).WithTags("Admins");
#endregion
#region vehicles

ValidationErrosViewModel validationDTO(VehicleDTO VehicleDTO)
{
    var validations = new ValidationErrosViewModel
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(VehicleDTO.Name))
        validations.Messages.Add("Name is required");

    if (string.IsNullOrEmpty(VehicleDTO.Brand))
        validations.Messages.Add("Brand is required");

    var yearLimit = 1950;
    if (VehicleDTO.Year < yearLimit)
        validations.Messages.Add("Vehicle year must be between 1950 and 2025.");
    

    return validations;
}

app.MapPost("/Vehicles", ([FromBody] VehicleDTO VehicleDTO, IVehicleService VehicleService) =>
{
    var validations = validationDTO(VehicleDTO);
    if (validations.Messages.Count() > 0)
        return Results.BadRequest(validations);

    var Vehicle = new Vehicle
    {
        Name = VehicleDTO.Name,
        Brand = VehicleDTO.Brand,
        Year = VehicleDTO.Year
    };
    VehicleService.Add(Vehicle);

    return Results.Created($"/Vehicle/{Vehicle.Id}", Vehicle);
}).WithTags("Vehicles");

app.MapGet("/Vehicles", ([FromQuery] int? page, IVehicleService VehicleService) =>
{
    var Vehicles = VehicleService.GetAllVehicles(page);
    return Results.Ok(Vehicles);
}).WithTags("Vehicles");

app.MapGet("/Vehicles/{id}", ([FromRoute]int id, IVehicleService VehicleService) =>
{
    var Vehicle = VehicleService.GetVehicleById(id);
    if (Vehicle == null) return Results.NotFound("Vehicle not found");

    return Results.Ok(Vehicle);
}).WithTags("Vehicles");

app.MapPut("/Vehicles/{id}", ([FromBody] VehicleDTO VehicleDTO, [FromRoute] int id, IVehicleService VehicleService) =>
{
    var Vehicle = VehicleService.GetVehicleById(id);
    if (Vehicle == null)
        return Results.NotFound("Vehicle not found");

    var validations = validationDTO(VehicleDTO);
    if (validations.Messages.Count() > 0)
        return Results.BadRequest(validations);


    Vehicle.Name = VehicleDTO.Name;
    Vehicle.Brand = VehicleDTO.Brand;
    Vehicle.Year = VehicleDTO.Year;
    
   

    VehicleService.Update(Vehicle);
    return Results.Ok(Vehicle);
}).WithTags("Vehicles");

app.MapDelete("/Vehicles/{id}", ([FromRoute] int id, IVehicleService VehicleService) =>
{
    var Vehicle = VehicleService.GetVehicleById(id);
    if (Vehicle == null) return Results.NotFound("Vehicle not found");

    VehicleService.Remove(Vehicle);
    return Results.NoContent();
}).WithTags("Vehicles");
#endregion


#region App
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
#endregion