using Se7ety.Api.Exceptions;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Services.Implementations;

public sealed class FileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private const long MaxFileSizeInBytes = 5 * 1024 * 1024;

    public async Task<string> SaveProfileImageAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Profile image cannot be empty.");
        }

        if (file.Length > MaxFileSizeInBytes)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Profile image size cannot exceed 5 MB.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Profile image must be jpg, jpeg, png, or webp.");
        }

        if (!string.IsNullOrWhiteSpace(file.ContentType) &&
            !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Uploaded file must be an image.");
        }

        var webRootPath = environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(environment.ContentRootPath, "wwwroot");
        }

        var uploadDirectory = Path.Combine(webRootPath, "uploads", "profiles");
        Directory.CreateDirectory(uploadDirectory);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var destinationPath = Path.Combine(uploadDirectory, fileName);

        await using var stream = File.Create(destinationPath);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/profiles/{fileName}";
    }
}
