using Grpc.Core;
using IdentityGrpc.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityGrpc.Services;

public class AccountService(UserManager<IdentityUser> userManager)
{
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public async Task<CreateAccountResponseModel> CreateAccount(CreateAccountRequestModel request)
    {
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password!);

        var response = new CreateAccountResponseModel
        {
            Success = result.Succeeded,
            Message = result.Succeeded 
                ? "Account was Created successfully." 
                : string.Join(", ", result.Errors.Select(e => e.Description))
        };

        if (result.Succeeded)
            response.UserId = user.Id;

        return response;
    }

    public async Task<ValidateCredentialsResponseModel> ValidateCredentials(ValidateCredentialsRequestModel request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new ValidateCredentialsResponseModel 
            { 
                Success = false,
                Message =  "Email and password must be provided"
            };
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null) 
        {
            return new ValidateCredentialsResponseModel
            {
                Success = false,
                Message = "Invalid Credentials"
            };
        }

        var isValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isValid) 
        {
            return new ValidateCredentialsResponseModel
            {
                Success = false,
                Message = "Invalid Credentials"
            };
        }

        return new ValidateCredentialsResponseModel
        {
            Success = true,
            Message = "Login Successful",
            UserId = user.Id
        };



    }










    public async Task<GetAccountsResponse> GetAccounts(GetAccountsRequest request)
    {
        var users = await _userManager.Users.ToListAsync();

        var response = new GetAccountsResponse
        {
            Success = true,
            Message = users.Count >= 1
                ? "Accounts retrieved successfully."
                : "No accounts found"
        };

        foreach (var user in users)
        {
            response.Accounts.Add(new Account
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber
            });
        }

        return response;
    }

    public async Task<GetAccountByIdResponse> GetAccountById(GetAccountByIdRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return new GetAccountByIdResponse
            {
                Success = false,
                Message = "No account found."
            };
        }

        var account = new Account
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        return new GetAccountByIdResponse
        {
            Success = true,
            Account = account,
            Message = "Account found"
        };

    }

    public async Task<UpdatePhoneNumberResponse> UpdatePhoneNumber(UpdatePhoneNumberRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return new UpdatePhoneNumberResponse
            {
                Success = false,
                Message = "No account found."
            };
        }

        if (!string.Equals(user.PhoneNumber, request.PhoneNumber, StringComparison.OrdinalIgnoreCase))
        {
            user.PhoneNumber = request.PhoneNumber;
        }

        var result = await _userManager.UpdateAsync(user);

        return new UpdatePhoneNumberResponse
        {
            Success = true,
            Message = result.Succeeded
                ? "Phone number was updated successfully"
                : string.Join(", ", result.Errors.Select(e => e.Description))
        };
    }

    public async Task<DeleteAccountByIdResponse> DeleteAccountById(DeleteAccountByIdRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return new DeleteAccountByIdResponse
            {
                Success = false,
                Message = "No account was found"
            };
        }

        var result = await _userManager.DeleteAsync(user);

        return new DeleteAccountByIdResponse
        {
            Success = true,
            Message = result.Succeeded
                ? "Account has been deleted"
                : string.Join(", ", result.Errors.Select(e => e.Description))
        }; 
    }

    public async Task<ConfirmAccountResponse> ConfirmAccount(ConfirmAccountRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return new ConfirmAccountResponse
            {
                Success = false,
                Message = "No account found"
            };
        }

        if (await _userManager.IsEmailConfirmedAsync(user))
        {
            return new ConfirmAccountResponse
            {
                Success = true,
                Message = "Account already confirmed."
            };
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        return new ConfirmAccountResponse
        {
            Success = true,
            Message = result.Succeeded
                ? "Email confirmed successfully"
                : string.Join(", ", result.Errors.Select(e => e.Description)),
        };
    }

    public async Task<ConfirmEmailChangeResponse> ConfirmEmailChange(ConfirmEmailChangeRequest request)
    {
        var users = await _userManager.FindByEmailAsync(request.UserId);

        if (users == null)
        {
            return new ConfirmEmailChangeResponse
            {
                Success = false,
                Message = "User not found"
            };
        }

        var result = await _userManager.ChangeEmailAsync(users, request.NewEmail, request.Token);

        return new ConfirmEmailChangeResponse
        {
            Success = result.Succeeded,
            Message = result.Succeeded
                ? "Email has been updated successfully"
                : string.Join(", ", result.Errors.Select(e => e.Description))
        };
    }

    public async Task<UpdateEmailResponse> UpdateEmail(UpdateEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return new UpdateEmailResponse
            {
                Success = false,
                Message = "User not found"
            };
        }

        if (string.Equals(user.Email, request.NewEmail, StringComparison.OrdinalIgnoreCase))
        {
            return new UpdateEmailResponse
            {
                Success = true,
                Message = "Email is already up to date.",
                Token = string.Empty
            };
        }

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);

        return new UpdateEmailResponse
        {
            Success = true,
            Message = "Token Generated for email change.",
            Token = token
        };
    }

    public async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return new ResetPasswordResponse
            {
                Success = false,
                Message = "User not found."
            };
        }
        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        return new ResetPasswordResponse
        {
            Success = result.Succeeded,
            Message = result.Succeeded
                ? "Password has been reset"
                : string.Join(", ", result.Errors.Select(e => e.Description))
        };
    }

    public async Task<GenerateTokenResponse> GeneratePasswordResetToken(GenerateTokenRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return new GenerateTokenResponse
            {
                Success = false,
                Token = string.Empty,
                Message = "User not found."
                
            };
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return new GenerateTokenResponse
        {
            Success = true,
            Token = token,
            Message = "Password reset token generated"
        };
    }

    public async Task<GenerateTokenResponse> GenerateEmailConfirmationToken(GenerateTokenRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return new GenerateTokenResponse
            {
                Success = false,
                Token = string.Empty,
                Message = "User not found."
            };
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        return new GenerateTokenResponse
        {
            Success = true,
            Token = token,
            Message = "Email confirmationn token generated"
        };
    }
}