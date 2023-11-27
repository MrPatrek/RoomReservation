using Entities.Models;

namespace Contracts
{
    public interface IImageRepository
    {
        IEnumerable<Image> GetAllImages();
        Image GetImageById(Guid imageId);
        Image GetImageWithDetails(Guid imageId);
        IEnumerable<Image> ImagesForRoom(Guid roomId);
        void CreateImage(Image image);
        void DeleteImage(Image image);
    }
}
