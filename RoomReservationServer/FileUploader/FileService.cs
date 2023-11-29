using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using RoomReservationServer.Interfaces;

namespace RoomReservationServer.FileUploader
{
    public class FileService : IFileService
    {
        private readonly string UploadsSubDirectory;

        private readonly IEnumerable<string> allowedExtensions = new List<string> { ".png", ".jpg" };

        private readonly IRepositoryWrapper _repository;
        private readonly IConfiguration _configuration;

        public FileService(IRepositoryWrapper repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;

            UploadsSubDirectory = @$"{_configuration.GetValue<string>("ResourcesDir")}/Images";
        }

        //public async Task<FileUploadSummary> UploadFileAsync(Stream fileStream, string contentType)
        //{


        //    return 
        //}

        public async Task<long> SaveFileAsync(FileMultipartSection fileSection, IList<string> filePaths, IList<string> notUploadedFiles, Guid roomId)
        {
            var extension = Path.GetExtension(fileSection.FileName);
            if (!allowedExtensions.Contains(extension))
            {
                notUploadedFiles.Add(fileSection.FileName);
                return 0;
            }

            Directory.CreateDirectory(UploadsSubDirectory);



            // (001) start

            // OLD:
            //var filePath = Path.Combine(UploadsSubDirectory, fileSection?.FileName);

            Guid imageId;
            string filePath;
            do
            {
                imageId = Guid.NewGuid();
                filePath = Path.Combine(UploadsSubDirectory, imageId.ToString() + extension);       // '+' and not ',' since otherwise you will get '/.jpg' in the end of the file path
            }
            while (File.Exists(filePath));

            // (001) end





            await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024);
            await fileSection.FileStream?.CopyToAsync(stream);

            filePaths.Add(filePath);



            // (002) now, when file exists for sure, save it to the DB:

            var imageEntity = new Image
            {
                Id = imageId,
                Path = filePath,
                RoomId = roomId
            };

            _repository.Image.CreateImage(imageEntity);
            await _repository.SaveAsync();

            // (002) end



            return fileSection.FileStream.Length;
        }

        public string ConvertSizeToString(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            return fileSize switch
            {
                _ when fileSize < kilobyte => "Less then 1KB",
                _ when fileSize < megabyte =>
                    $"{Math.Round(fileSize / kilobyte, fileSize < 10 * kilobyte ? 2 : 1, MidpointRounding.AwayFromZero):##,###.##}KB",
                _ when fileSize < gigabyte =>
                    $"{Math.Round(fileSize / megabyte, fileSize < 10 * megabyte ? 2 : 1, MidpointRounding.AwayFromZero):##,###.##}MB",
                _ when fileSize >= gigabyte =>
                    $"{Math.Round(fileSize / gigabyte, fileSize < 10 * gigabyte ? 2 : 1, MidpointRounding.AwayFromZero):##,###.##}GB",
                _ => "n/a"
            };
        }

        public string GetBoundary(MediaTypeHeaderValue contentType)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            return boundary;
        }
    }
}
