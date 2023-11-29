using Entities.Models;

namespace Contracts
{
    public interface IImageRepository
    {
        Task<IEnumerable<Image>> GetAllImagesAsync();
        Task<Image> GetImageByIdAsync(Guid imageId);
        Task<Image> GetImageWithDetailsAsync(Guid imageId);
        Task<IEnumerable<Image>> GetImagesForRoomAsync(Guid roomId);
        void CreateImage(Image image);
        void DeleteImage(Image image);
    }
}
