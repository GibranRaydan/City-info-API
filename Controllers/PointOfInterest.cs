using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using CityInfo.API.Models;
using CityInfo.API.Services;
namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities/{cityId}/pointsofinterest/")]
public class PointOfInterestController : ControllerBase
{
    private readonly ILogger<PointOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly CitiesDataStore _citiesDataStore;
    public PointOfInterestController(ILogger<PointOfInterestController> logger,
        IMailService mailService,
        CitiesDataStore citiesDataStore)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        try
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                _logger.LogInformation($"City Id: {cityId} was not found");
                return NotFound();
            }
            return Ok(city.PointsOfInterest);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"City Id: {cityId} was not found", e);
            return StatusCode(500, "A problem happend.");
        }

    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(
        int cityId, int pointOfInterestId)
    {
        var city = _citiesDataStore.Cities
            .FirstOrDefault(city => city.Id == cityId);

        if (city == null)
        {
            return NotFound("City not found");
        }

        var pointOfInterest = city.PointsOfInterest
            .FirstOrDefault(point => point.Id == pointOfInterestId);

        if (pointOfInterest == null)
        {
            return NotFound($"Point of interest not found in {city.Name}");
        }

        return Ok(pointOfInterest);
    }

    [HttpPost()]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(
    int cityId,
    [FromBody] PointOfInterestCreationDto pointOfInterest)
    {
        var city = _citiesDataStore.Cities
            .FirstOrDefault(city => city.Id == cityId);

        if (city == null)
        {
            return NotFound("City not found");
        }
        var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(
            c => c.PointsOfInterest).Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };

        city.PointsOfInterest.Add(finalPointOfInterest);

        return CreatedAtRoute("GetPointOfInterest",
            new
            {
                cityId = cityId,
                pointOfInterestId = finalPointOfInterest.Id
            },
            finalPointOfInterest);
    }


    [HttpPut("{pointOfInterestId}")]
    public ActionResult<PointOfInterestDto> UpdatePointOfInterest(
    int cityId,
    int pointOfInterestId,
    [FromBody] PointOfInterestCreationDto pointOfInterest)
    {
        var city = _citiesDataStore.Cities
            .FirstOrDefault(city => city.Id == cityId);

        if (city == null)
        {
            return NotFound("City not found");
        }

        var previousPointOfInterest = city.PointsOfInterest
            .FirstOrDefault(point => point.Id == pointOfInterestId);

        if (previousPointOfInterest == null)
        {
            return NotFound($"Point of interest not found in {city.Name}");
        }

        previousPointOfInterest.Name = pointOfInterest.Name;
        previousPointOfInterest.Description = pointOfInterest.Description;

        return Ok(previousPointOfInterest);
    }

    [HttpPatch("{pointOfInterestId}")]
    public ActionResult<PointOfInterestDto> PartialUpdatePointOfInterest(
    int cityId,
    int pointOfInterestId,
    JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
    {
        var city = _citiesDataStore.Cities
            .FirstOrDefault(city => city.Id == cityId);

        if (city == null)
        {
            return NotFound("City not found");
        }

        var previousPointOfInterest = city.PointsOfInterest
            .FirstOrDefault(point => point.Id == pointOfInterestId);

        if (previousPointOfInterest == null)
        {
            return NotFound($"Point of interest not found in {city.Name}");
        }

        var pointOfInterestToPatch =
            new PointOfInterestUpdateDto()
            {
                Name = previousPointOfInterest.Name,
                Description = previousPointOfInterest.Description
            };

        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(pointOfInterestToPatch))
        {
            return BadRequest();

        }

        previousPointOfInterest.Name = pointOfInterestToPatch.Name;
        previousPointOfInterest.Description = pointOfInterestToPatch.Description;

        return Ok(patchDocument);

    }

    [HttpDelete("{pointOfInterestId}")]
    public ActionResult DeletaPointOfInterest(
        int cityId, int pointOfInterestId)
    {

        var city = _citiesDataStore.Cities
            .FirstOrDefault(city => city.Id == cityId);

        if (city == null)
        {
            return NotFound("City not found");
        }

        var previousPointOfInterest = city.PointsOfInterest
            .FirstOrDefault(point => point.Id == pointOfInterestId);

        if (previousPointOfInterest == null)
        {
            return NotFound($"Point of interest not found in {city.Name}");
        }

        city.PointsOfInterest.Remove(previousPointOfInterest);
        _mailService.Send("Point of interest deleted",
                    $" Point of interest {previousPointOfInterest.Id} has been deleted");
        return NoContent();
    }
}