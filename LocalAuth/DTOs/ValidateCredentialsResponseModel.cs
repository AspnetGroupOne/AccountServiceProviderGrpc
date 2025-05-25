namespace IdentityGrpc.DTOs;

public class ValidateCredentialsResponseModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? UserId { get; set; }
}