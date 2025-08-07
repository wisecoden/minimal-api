using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "password";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))
    );
});


var app = builder.Build();

#endregion

#region Home Page
app.MapGet("/", () => Results.Json(new HomeViewModel())).WithTags("Home");
#endregion

#region Admins
string GenerateToken(Administrator administrator)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim("Email", administrator.Email),
        new Claim("Profile", administrator.Profile),
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );
    

    return new JwtSecurityTokenHandler().WriteToken(token);
}


app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{   
    var admin = administratorService.Login(loginDTO);
    if (admin != null)
    {
        string token = GenerateToken(admin);
        return Results.Ok(new LoggedAdministratorViewModel
        {
            Email = admin.Email,
            Profile = admin.Profile,
            Token = token
        });
    }
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
}).RequireAuthorization().WithTags("Admins");

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
}).RequireAuthorization().WithTags("Admins");

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
}).RequireAuthorization().WithTags("Admins");
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
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/Vehicles", ([FromQuery] int? page, IVehicleService VehicleService) =>
{
    var Vehicles = VehicleService.GetAllVehicles(page);
    return Results.Ok(Vehicles);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/Vehicles/{id}", ([FromRoute]int id, IVehicleService VehicleService) =>
{
    var Vehicle = VehicleService.GetVehicleById(id);
    if (Vehicle == null) return Results.NotFound("Vehicle not found");

    return Results.Ok(Vehicle);
}).RequireAuthorization().WithTags("Vehicles");

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
}).RequireAuthorization().WithTags("Vehicles");

app.MapDelete("/Vehicles/{id}", ([FromRoute] int id, IVehicleService VehicleService) =>
{
    var Vehicle = VehicleService.GetVehicleById(id);
    if (Vehicle == null) return Results.NotFound("Vehicle not found");

    VehicleService.Remove(Vehicle);
    return Results.NoContent();
}).RequireAuthorization().WithTags("Vehicles");
#endregion


#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion