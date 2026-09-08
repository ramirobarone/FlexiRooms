using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HotelImagesController(IHotelImageService hotelImageService,
                                   ILogger<HotelImagesController> logger) : ControllerBase
{
    [HttpPost("{hotelId:int}/upload")]
    public async Task<IActionResult> UploadImages(int hotelId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken)
    {
        if (hotelId <= 0)
            return BadRequest("Hotel id is invalid.");

        if (files.Count == 0)
            return BadRequest("At least one image is required.");

        List<HotelImageUploadFile> mappedFiles = new(files.Count);

        foreach (IFormFile file in files)
        {
            await using MemoryStream memoryStream = new();
            await file.CopyToAsync(memoryStream, cancellationToken);

            mappedFiles.Add(new HotelImageUploadFile(file.FileName, memoryStream.ToArray()));
        }

        try
        {
            IReadOnlyCollection<HotelPictureDto> images = await hotelImageService.UploadImagesAsync(hotelId, mappedFiles, cancellationToken);
            return Ok(images);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid image upload request for hotel {HotelId}", hotelId);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Hotel not found for image upload. HotelId: {HotelId}", hotelId);
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{hotelId:int}")]
    public async Task<IActionResult> GetHotelImages(int hotelId, CancellationToken cancellationToken)
    {
        if (hotelId <= 0)
            return BadRequest("Hotel id is invalid.");

        try
        {
            IReadOnlyCollection<HotelPictureDto> images = await hotelImageService.GetHotelImagesAsync(hotelId, cancellationToken);
            return Ok(images);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Hotel not found while retrieving images. HotelId: {HotelId}", hotelId);
            return NotFound(ex.Message);
        }
    }
}
