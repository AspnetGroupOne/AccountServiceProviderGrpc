using System.ComponentModel.DataAnnotations;

namespace IdentityGrpc.DTOs;

public class ValidateCredentialsRequestModel
{
    public string? Email { get; set; }

    public string? Password { get; set; }
}
