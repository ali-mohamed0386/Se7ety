using Microsoft.AspNetCore.Identity;
using Se7ety.Api.Domain.Entities;
using Se7ety.Api.DTOs.Settings;
using Se7ety.Api.Exceptions;
using Se7ety.Api.Repositories.Interfaces;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Services.Implementations;

public sealed class SettingsService(
    IUserRepository users,
    ICurrentUserService currentUser,
    IPasswordHasher<User> passwordHasher) : ISettingsService
{
    public async Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByIdAsync(currentUser.UserId, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "User was not found.");

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Current password is incorrect.");
        }

        user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
        user.UpdatedAtUtc = DateTime.UtcNow;
        users.Update(user);
        await users.SaveChangesAsync(cancellationToken);
    }
}
