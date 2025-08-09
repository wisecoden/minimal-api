using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ViewModels;
using MinimalApi.Domain.Services;
using MinimalApi.DTOs;
using MinimalApi.Infra.Db;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
    }

    private string key = "";
    public IConfiguration Configuration { get;set; } = default!;

    public void Configureservices(IServiceCollection services)
    {
        services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {   ValidateLifetime = true, 
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
        };
    });

    services.AddAuthorization();

    services.AddScoped<IAdministratorService, AdministratorService>();
    services.AddScoped<IVehicleService, VehicleService>();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Insira o token JWT aqui: "
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme{
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
    });
    });



    services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseMySql(
            Configuration.GetConnectionString("MySql"),
            ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
        );
    });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

      
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        //app.UseCors();
        

        app.UseEndpoints(endpoints => {

        #region Home Page
        endpoints.MapGet("/", () => Results.Json(new HomeViewModel())).AllowAnonymous().WithTags("Home");
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
                new Claim(ClaimTypes.Role, administrator.Profile),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );
            

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        endpoints.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
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
        }).AllowAnonymous().WithTags("Admins");

        endpoints.MapGet("/admins", ([FromQuery] int? page, IAdministratorService administratorService) =>
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
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin"})
        .WithTags("Admins");

        endpoints.MapGet("/admins/{id}", ([FromRoute]int id, IAdministratorService administratorService) =>
        {
            var administrator = administratorService.GetAdministratorById(id);
            if (administrator == null) return Results.NotFound("No admins found");

            return Results.Ok(new AdministratorViewModel
                {
                    Id = administrator.Id,
                    Email = administrator.Email,
                    Profile =  administrator.Profile
                    
                });
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin"})
        .WithTags("Admins");

        endpoints.MapPost("/admins", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
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
                Profile = administrator.Profile

            });
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
        .WithTags("Admins");

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

        endpoints.MapPost("/Vehicles", ([FromBody] VehicleDTO VehicleDTO, IVehicleService VehicleService) =>
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
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin, Editor"})
        .WithTags("Vehicles");

        endpoints.MapGet("/Vehicles", ([FromQuery] int? page, IVehicleService VehicleService) =>
        {
            var Vehicles = VehicleService.GetAllVehicles(page);
            return Results.Ok(Vehicles);
        }).RequireAuthorization().WithTags("Vehicles");

        endpoints.MapGet("/Vehicles/{id}", ([FromRoute] int id, IVehicleService VehicleService) =>
        {
            var Vehicle = VehicleService.GetVehicleById(id);
            if (Vehicle == null) return Results.NotFound("Vehicle not found");

            return Results.Ok(Vehicle);
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin, Editor"})
        .WithTags("Vehicles");

        endpoints.MapPut("/Vehicles/{id}", ([FromBody] VehicleDTO VehicleDTO, [FromRoute] int id, IVehicleService VehicleService) =>
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
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin"})
        .WithTags("Vehicles");

        endpoints.MapDelete("/Vehicles/{id}", ([FromRoute] int id, IVehicleService VehicleService) =>
        {
            var Vehicle = VehicleService.GetVehicleById(id);
            if (Vehicle == null) return Results.NotFound("Vehicle not found");

            VehicleService.Remove(Vehicle);
            return Results.NoContent();
        })
        .RequireAuthorization()
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
        .WithTags("Vehicles");

        #endregion
        });
    }
}