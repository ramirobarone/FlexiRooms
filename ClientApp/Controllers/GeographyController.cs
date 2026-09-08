using Application.Models;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ClientApp.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class GeographyController(FlexiRoomsContext context, IMemoryCache memoryCache, ILogger<GeographyController> logger) : ControllerBase
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(8);
    private const string ProvincesCacheKey = "geography_provinces_all";

    [HttpGet(nameof(GetProvinces))]
    public async Task<IActionResult> GetProvinces()
    {
        if (memoryCache.TryGetValue(ProvincesCacheKey, out IEnumerable<Province>? cachedProvinces))
        {
            return Ok(cachedProvinces);
        }

        List<Province> provinces = await context.Provincies
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new Province
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        memoryCache.Set(ProvincesCacheKey, provinces, CacheDuration);
        logger.LogInformation("Cached {Count} provinces for {CacheDuration}", provinces.Count, CacheDuration);

        return provinces.Count == 0 ? NoContent() : Ok(provinces);
    }

    [HttpGet(nameof(GetCitiesByProvince))]
    public async Task<IActionResult> GetCitiesByProvince(int provinceId)
    {
        if (provinceId <= 0)
        {
            return BadRequest();
        }

        string cacheKey = $"geography_cities_province_{provinceId}";
        if (memoryCache.TryGetValue(cacheKey, out IEnumerable<City>? cachedCities))
        {
            return Ok(cachedCities);
        }

        List<City> cities = await context.Cities
            .AsNoTracking()
            .Where(x => EF.Property<int?>(x, "ProvinceId") == provinceId)
            .OrderBy(x => x.Name)
            .Select(x => new City
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        memoryCache.Set(cacheKey, cities, CacheDuration);
        logger.LogInformation("Cached {Count} cities for province {ProvinceId} for {CacheDuration}", cities.Count, provinceId, CacheDuration);

        return cities.Count == 0 ? NoContent() : Ok(cities);
    }
}
