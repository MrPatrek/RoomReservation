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

        public async Task<IEnumerable<Image>> GetAllImagesAsync()
        {
            return await FindAll()
                .OrderBy(image => image.RoomId)
                .ToListAsync();
        }

        public async Task<Image> GetImageByIdAsync(Guid imageId)
        {
            return await FindByCondition(image => image.Id.Equals(imageId))
                .FirstOrDefaultAsync();
        }

        public async Task<Image> GetImageWithDetailsAsync(Guid imageId)
        {
            return await FindByCondition(image => image.Id.Equals(imageId))
                .Include(image => image.Room)
                .FirstOrDefaultAsync();
        }

        // this one is used by DeleteRoom action menthod
        public async Task<IEnumerable<Image>> GetImagesForRoomAsync(Guid roomId)
        {
            return await FindByCondition(image => image.RoomId.Equals(roomId))
                .ToListAsync();
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
