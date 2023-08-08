using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace CityInfo.API.Controllers;

[ApiController]
[Authorize]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest/")]
public class PointOfInterestController : ControllerBase
{
    private readonly ILogger<PointOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly IMapper _mapper;
    private readonly ICityInfoRepository _cityInfoRepository;

    public PointOfInterestController(ILogger<PointOfInterestController> logger,
    IMailService mailService,
    ICityInfoRepository cityInfoRepository,
    IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper
             ?? throw new ArgumentNullException(nameof(mapper));

    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
    {   
        // var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
 
        var cityExist = await _cityInfoRepository.CityExistAsync(cityId);

        if (!cityExist)
        {
            _logger.LogInformation($"City with {cityId} not found");
            return NotFound("City not found");
        }

        var pointsOfInterestEntity = await _cityInfoRepository
            .GetPointsOfInterestAsync(cityId);

        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestEntity));

    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(
        int cityId, int pointOfInterestId)
    {
        if (!await _cityInfoRepository.CityExistAsync(cityId))
        {
            return NotFound("City not found");
        }

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestAsync(cityId, pointOfInterestId);

        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterestEntity));
    }

    [HttpPost()]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
    int cityId,
    [FromBody] PointOfInterestCreationDto pointOfInterest)
    {
        if (!await _cityInfoRepository.CityExistAsync(cityId))
        {
            return NotFound("City not found");
        }
        var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

        await _cityInfoRepository.AddPointOfInterestForCityAsync(
            cityId, finalPointOfInterest);

        await _cityInfoRepository.SaveChangesAsync();
        var createdPointOfInterest = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);
        return CreatedAtRoute("GetPointOfInterest",
            new
            {
                cityId,
                pointOfInterestId = createdPointOfInterest.Id
            },
            createdPointOfInterest);
    }

    [HttpPut("{pointOfInterestId}")]
    public async Task<ActionResult<PointOfInterestDto>> UpdatePointOfInterest(
    int cityId,
    int pointOfInterestId,
    [FromBody] PointOfInterestUpdateDto pointOfInterest)
    {
        if (!await _cityInfoRepository.CityExistAsync(cityId))
        {
            return NotFound("City not found");
        }

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestAsync(cityId, pointOfInterestId);

        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }

        _mapper.Map(pointOfInterest, pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{pointOfInterestId}")]
    public async Task<ActionResult<PointOfInterestDto>> PartialUpdatePointOfInterest(
    int cityId,
    int pointOfInterestId,
    JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
    {
        if (!await _cityInfoRepository.CityExistAsync(cityId))
        {
            return NotFound("City not found");
        }

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestAsync(cityId, pointOfInterestId);

        if (pointOfInterestEntity == null)
        {
            return NotFound("Point of interest not found in");
        }

        var pointOfInterestToPatch = _mapper.Map<PointOfInterestUpdateDto>(
            pointOfInterestEntity);

        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(pointOfInterestToPatch))
        {
            return BadRequest(ModelState);

        }

        _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{pointOfInterestId}")]
    public async Task<ActionResult> DeletaPointOfInterest(
        int cityId, int pointOfInterestId)
    {

       if (!await _cityInfoRepository.CityExistAsync(cityId))
        {
            return NotFound("City not found");
        }

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestAsync(cityId, pointOfInterestId);

        if (pointOfInterestEntity == null)
        {
            return NotFound("Point of interest not found in");
        }

        _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();

        _mailService.Send("Point of interest deleted",
                    $" Point of interest {pointOfInterestEntity.Id} has been deleted");
        return NoContent();
    }
}