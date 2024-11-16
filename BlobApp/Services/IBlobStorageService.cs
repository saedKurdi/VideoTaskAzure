namespace BlobApp.Services;
public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream stream,string fileName);
    Task<Stream> DownloadAsync(string fileName);
    Task<List<string>> ListFilesAsync();
}
