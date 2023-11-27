using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ImageRepository : RepositoryBase<Image>, IImageRepository
    {
        public ImageRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Image> GetAllImages()
        {
            return FindAll()
                .OrderBy(image => image.RoomId)
                .ToList();
        }

        public Image GetImageById(Guid imageId)
        {
            return FindByCondition(image => image.Id.Equals(imageId))
                .FirstOrDefault();
        }

        public Image GetImageWithDetails(Guid imageId)
        {
            return FindByCondition(image => image.Id.Equals(imageId))
                .Include(image => image.Room)
                .FirstOrDefault();
        }

        // this one is used by DeleteRoom action menthod
        public IEnumerable<Image> ImagesForRoom(Guid roomId)
        {
            return FindByCondition(image => image.RoomId.Equals(roomId)).ToList();
        }

        public void CreateImage(Image image)
        {
            Create(image);
        }

        public void DeleteImage(Image image)
        {
            Delete(image);
        }


    }
}
