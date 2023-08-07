using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Models;
using CityInfo.API.Services;
using System.Text.Json;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;
    const int MAX_CITIES_PAGE_SIZE = 20;

    public CitiesController(ICityInfoRepository cityInfoRepository,
        IMapper mapper)
    {
        _cityInfoRepository = cityInfoRepository
            ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper 
             ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities(
        [FromQuery] string? name,
        [FromQuery] string? searchQuery,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {

        if (pageSize > MAX_CITIES_PAGE_SIZE) pageSize = MAX_CITIES_PAGE_SIZE;

        var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(
            name, searchQuery, pageSize, pageNumber);

        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntities));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCity(
        int id, bool includePointsOfInterest = false)
    {
        var cityEntity = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

        if (cityEntity == null)
        {
            return NotFound();
        }

        if (includePointsOfInterest){
            return Ok(_mapper.Map<CityDto>(cityEntity));
        }

        return Ok(_mapper.Map<CityWithoutPointOfInterestDto>(cityEntity));
    }
}
