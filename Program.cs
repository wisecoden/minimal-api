var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/login", (MinimalApi.DTOs.loginDTO loginDTO) =>
{
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Password == "password")
        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
});
app.Run();
