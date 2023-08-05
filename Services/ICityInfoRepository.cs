using CityInfo.API.Entities;

namespace CityInfo.API.Services;

public interface ICityInfoRepository
{
    Task<IEnumerable<City>> GetCitiesAsync();
    Task<bool> CityExistAsync(int cityId);
    Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId);
    Task<PointOfInterest?> GetPointOfInterestAsync(int cityId,
        int PointOfInterestId);
    Task AddPointOfInterestForCityAsync(int cityId,
        PointOfInterest pointOfInterest);

    void DeletePointOfInterest(PointOfInterest pointOfInterest);
    Task<bool> SaveChangesAsync();
    
}