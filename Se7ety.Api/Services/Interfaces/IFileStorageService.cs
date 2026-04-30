namespace Se7ety.Api.Services.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveProfileImageAsync(IFormFile file, CancellationToken cancellationToken = default);
}
