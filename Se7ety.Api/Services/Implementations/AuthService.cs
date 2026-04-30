using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Se7ety.Api.Domain.Entities;
using Se7ety.Api.Domain.Enums;
using Se7ety.Api.DTOs.Auth;
using Se7ety.Api.Exceptions;
using Se7ety.Api.Repositories.Interfaces;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Services.Implementations;

public sealed class AuthService(
    IUserRepository users,
    IPasswordHasher<User> passwordHasher,
    IJwtTokenService jwtTokenService) : IAuthService
{
    public Task<AuthResponse> RegisterPatientAsync(AuthRequest request, CancellationToken cancellationToken = default)
    {
        return RegisterAsync(request, UserRole.Patient, cancellationToken);
    }

    public Task<AuthResponse> RegisterDoctorAsync(AuthRequest request, CancellationToken cancellationToken = default)
    {
        return RegisterAsync(request, UserRole.Doctor, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(AuthRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var user = await users.Query()
            .Include(current => current.PatientProfile)
            .Include(current => current.DoctorProfile)
            .FirstOrDefaultAsync(current => current.Email == email, cancellationToken);

        if (user is null)
        {
            throw new ApiException(StatusCodes.Status401Unauthorized, "Invalid email or password.");
        }

        var passwordResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new ApiException(StatusCodes.Status401Unauthorized, "Invalid email or password.");
        }

        return CreateResponse(user);
    }

    private async Task<AuthResponse> RegisterAsync(AuthRequest request, UserRole role, CancellationToken cancellationToken)
    {
        var email = NormalizeEmail(request.Email);
        if (await users.EmailExistsAsync(email, cancellationToken))
        {
            throw new ApiException(StatusCodes.Status409Conflict, "Email is already registered.");
        }

        var user = new User
        {
            Email = email,
            Role = role
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        if (role == UserRole.Patient)
        {
            user.PatientProfile = new PatientProfile();
        }
        else
        {
            user.DoctorProfile = new DoctorProfile();
        }

        await users.AddAsync(user, cancellationToken);
        await users.SaveChangesAsync(cancellationToken);

        return CreateResponse(user);
    }

    private AuthResponse CreateResponse(User user)
    {
        var token = jwtTokenService.CreateToken(user);

        return new AuthResponse
        {
            Token = token.Token,
            ExpiresAtUtc = token.ExpiresAtUtc,
            UserId = user.Id,
            ProfileId = user.Role == UserRole.Patient ? user.PatientProfile?.Id : user.DoctorProfile?.Id,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
