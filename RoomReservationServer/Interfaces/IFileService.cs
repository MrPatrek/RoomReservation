using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using RoomReservationServer.FileUploader;

namespace RoomReservationServer.Interfaces
{
    public interface IFileService
    {
        //Task<FileUploadSummary> UploadFileAsync(Stream fileStream, string contentType);
        Task<long> SaveFileAsync(FileMultipartSection fileSection, IList<string> filePaths, IList<string> notUploadedFiles, Guid roomId);
        string ConvertSizeToString(long bytes);
        string GetBoundary(MediaTypeHeaderValue contentType);
    }
}
