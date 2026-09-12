using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RoomImagesController(IRoomImageService roomImageService,
                                  ILogger<RoomImagesController> logger) : ControllerBase
{
    [HttpPost("{roomId:int}/upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImages(int roomId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received request to upload {Count} image(s) for room {RoomId}", files?.Count ?? 0, roomId);

        if (roomId <= 0)
        {
            logger.LogWarning("Invalid room id received in UploadImages: {RoomId}", roomId);
            return BadRequest("Room id is invalid.");
        }

        if (files is null || files.Count == 0)
        {
            logger.LogWarning("No files provided in room image upload request for room {RoomId}", roomId);
            return BadRequest("At least one image is required.");
        }

        List<RoomImageUploadFile> mappedFiles = new(files.Count);

        foreach (IFormFile file in files)
        {
            await using MemoryStream memoryStream = new();
            await file.CopyToAsync(memoryStream, cancellationToken);
            mappedFiles.Add(new RoomImageUploadFile(file.FileName, memoryStream.ToArray()));
        }

        try
        {
            IReadOnlyCollection<RoomPictures> images = await roomImageService.UploadImagesAsync(roomId, mappedFiles, cancellationToken);
            logger.LogInformation("Successfully uploaded {Count} images for room {RoomId}", images.Count, roomId);
            return Ok(images);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid room image upload request for room {RoomId}", roomId);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Room not found for image upload. RoomId: {RoomId}", roomId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error uploading images for room {RoomId}", roomId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while uploading room images.");
        }
    }

    [HttpGet("{roomId:int}")]
    public async Task<IActionResult> GetRoomImages(int roomId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received request to get images for room {RoomId}", roomId);

        if (roomId <= 0)
        {
            logger.LogWarning("Invalid room id received in GetRoomImages: {RoomId}", roomId);
            return BadRequest("Room id is invalid.");
        }

        try
        {
            IReadOnlyCollection<RoomPictures> images = await roomImageService.GetRoomImagesAsync(roomId, cancellationToken);
            logger.LogInformation("Successfully retrieved {Count} image(s) for room {RoomId}", images.Count, roomId);
            return Ok(images);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Room not found while retrieving images. RoomId: {RoomId}", roomId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error retrieving images for room {RoomId}", roomId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving room images.");
        }
    }

    [HttpDelete("{roomId:int}/{imageId:int}")]
    public async Task<IActionResult> DeleteRoomImage(int roomId, int imageId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received request to delete image {ImageId} for room {RoomId}", imageId, roomId);

        if (roomId <= 0 || imageId <= 0)
        {
            logger.LogWarning("Invalid parameters received in DeleteRoomImage: RoomId: {RoomId}, ImageId: {ImageId}", roomId, imageId);
            return BadRequest("Room id and image id must be greater than zero.");
        }

        try
        {
            bool deleted = await roomImageService.DeleteImageAsync(roomId, imageId, cancellationToken);
            if (!deleted)
            {
                logger.LogWarning("Room image {ImageId} for room {RoomId} was not found to delete", imageId, roomId);
                return NotFound("Image not found.");
            }

            logger.LogInformation("Successfully deleted room image {ImageId} for room {RoomId}", imageId, roomId);
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error deleting image {ImageId} for room {RoomId}", imageId, roomId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting room image.");
        }
    }
}
