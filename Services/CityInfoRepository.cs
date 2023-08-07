using Microsoft.EntityFrameworkCore;
using CityInfo.API.DbContexts;
using CityInfo.API.Entities;

namespace CityInfo.API.Services;

public class CityInfoRepository : ICityInfoRepository
{
    private readonly CityInfoContext _context;
    public CityInfoRepository(CityInfoContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<IEnumerable<City>> GetCitiesAsync(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return await GetCitiesAsync();
        }
        name = name.Trim();
        return await _context.Cities
            .Where(c => c.Name == name)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }


    public async Task<City?> GetCityAsync(
        int cityId,
        bool includePointsOfInterest)
    {
        if (includePointsOfInterest)
        {
            return await _context.Cities.Include(c => c.PointsOfInterest)
                .Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }
        return await _context.Cities
            .Where(c => c.Id == cityId).FirstOrDefaultAsync();
    }

    public async Task<PointOfInterest?> GetPointOfInterestAsync(
        int cityId,
        int PointOfInterestId)
    {
        return await _context.PointOfInterest
            .Where(p => p.CityId == cityId && p.Id == PointOfInterestId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CityExistAsync(int cityId)
    {
        return await _context.Cities.AnyAsync(c => c.Id == cityId);
    }


    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId)
    {
        return await _context.PointOfInterest
            .Where(p => p.CityId == cityId)
            .ToListAsync();
    }

    public async Task AddPointOfInterestForCityAsync(int cityId,
        PointOfInterest pointOfInterest)
    {
        var city = await GetCityAsync(cityId, false);
        city?.PointsOfInterest.Add(pointOfInterest);

    }

    public void DeletePointOfInterest(PointOfInterest pointOfInterest)
    {
        _context.PointOfInterest.Remove(pointOfInterest);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() >= 0);
    }
}
