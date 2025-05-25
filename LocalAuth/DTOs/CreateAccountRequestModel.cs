using System.ComponentModel.DataAnnotations;

namespace IdentityGrpc.DTOs;

public class CreateAccountRequestModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}

