using MinimalApi.Domain.Enums;

namespace MinimalApi.DTOs;

public class AdministratorDTO
{
  public string Email { get; set; } = default!;
  public string Password { get; set; } = default!;
  public Profile? Profile { get; set; } = default!;
}