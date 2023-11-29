using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using RoomReservationServer.FileUploader;
using RoomReservationServer.Interfaces;

namespace RoomReservationServer.Controllers
{
    [Route("api/image"), Authorize]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        private IFileService _fileService;
        private ISharedController _sharedController;

        public ImageController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper, IFileService fileService, ISharedController sharedController)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _fileService = fileService;
            _sharedController = sharedController;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MultipartFormData]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadImage()
        {
            try
            {
                Stream fileStream = HttpContext.Request.Body;
                string contentType = Request.ContentType;

                var fileCount = 0;
                long totalSizeInBytes = 0;

                var boundary = _fileService.GetBoundary(MediaTypeHeaderValue.Parse(contentType));
                var multipartReader = new MultipartReader(boundary, fileStream);
                var section = await multipartReader.ReadNextSectionAsync();

                Guid roomId = Guid.Empty;           // this should never be empty, if passed to a SaveFileAsync() method below !

                if (section != null)
                {
                    string roomIdStr = await section.ReadAsStringAsync();
                    if (roomIdStr != null)
                    {
                        if (Guid.TryParse(roomIdStr, out roomId))
                        {



                            // here we start the DB stuff:

                            var room = await _repository.Room.GetRoomByIdAsync(roomId);

                            if (room is null)
                            {
                                _logger.LogError($"Room with id: {roomId}, hasn't been found in db.");
                                return NotFound();
                            }

                            // If it reaches here (i.e., it did not return NotFound()), then it means our room was found. Because of this, just proceed until further notice.



                        }
                        else
                        {
                            return BadRequest("Your string is not convertable to Guid.");
                        }
                    }
                    else
                    {
                        return BadRequest("You did not specify room ID.");
                    }

                    section = await multipartReader.ReadNextSectionAsync();
                }
                else
                {
                    return BadRequest("You did not send anything.");
                }

                var filePaths = new List<string>();
                var notUploadedFiles = new List<string>();

                while (section != null)
                {
                    var fileSection = section.AsFileSection();
                    if (fileSection != null)
                    {
                        totalSizeInBytes += await _fileService.SaveFileAsync(fileSection, filePaths, notUploadedFiles, roomId);
                        fileCount++;
                    }

                    section = await multipartReader.ReadNextSectionAsync();
                }





                var fileUploadSummary = new FileUploadSummary
                {
                    TotalFilesUploaded = fileCount,
                    TotalSizeUploaded = _fileService.ConvertSizeToString(totalSizeInBytes),
                    FilePaths = filePaths,
                    NotUploadedFiles = notUploadedFiles
                };

                if (fileCount == 0)
                    return BadRequest("You did not upload files.");

                return CreatedAtAction(nameof(UploadImage), fileUploadSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UploadImage action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            try
            {
                var images = await _repository.Image.GetAllImagesAsync();
                if (!images.Any())
                {
                    _logger.LogInfo($"No images to return from the database.");
                    return NoContent();
                }

                _logger.LogInfo($"Returned all images from database.");

                var imagesResult = _mapper.Map<IEnumerable<ImageDto>>(images);
                return Ok(imagesResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllImages action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            try
            {
                var image = await _repository.Image.GetImageByIdAsync(id);
                if (image == null)
                {
                    _logger.LogError($"Image with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                if (System.IO.File.Exists(image.Path))
                {
                    System.IO.File.Delete(image.Path);
                }
                else
                {
                    _logger.LogError($"Image with id: {id}, HAS been found in the DB, but not in the resources... No changes were applied.");
                    return NotFound($"Image with id: {id}, HAS been found in the DB, but not in the resources... No changes were applied.");
                }

                _repository.Image.DeleteImage(image);
                await _repository.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteImages action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("room-id-{roomId}")]
        public async Task<IActionResult> DeleteImagesForRoom(Guid roomId)
        {
            try
            {
                return await _sharedController.DeleteImagesForRoomAsync(roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteImages action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
