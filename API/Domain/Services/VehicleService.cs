using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infra.Db;

namespace  MinimalApi.Domain.Services
{
  public class VehicleService : IVehicleService
  {
    private readonly ApplicationDbContext _dbContext;
    public VehicleService(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

    public void Add(Vehicle vehicle)
    {
        _dbContext.vehicles.Add(vehicle);
        _dbContext.SaveChanges();
    }

    public List<Vehicle> GetAllVehicles(int? page = 1, string? name = null, string? brand = null)
    {
      var query = _dbContext.vehicles.AsQueryable();
      if (!string.IsNullOrEmpty(name))
      {
        query = query.Where(c => EF.Functions.Like(c.Name.ToLower(), $"%{name.ToLower()}%"));
      }
      if (!string.IsNullOrEmpty(brand))
      {
        query = query.Where(c => EF.Functions.Like(c.Brand.ToLower(), $"%{brand.ToLower()}%"));
      }

      int pageSize = 10; // Define the number of items per page

      if (page != null)
      {
         query = query.Skip(((int)page - 1) * pageSize).Take(pageSize);  
      }
         
      return query.ToList();
    }

    public Vehicle? GetVehicleById(int id)
    {
      return _dbContext.vehicles.Where(c => c.Id == id).FirstOrDefault();
    }

    public void Remove(Vehicle vehicle)
    {
       _dbContext.vehicles.Remove(vehicle);
       _dbContext.SaveChanges();
    }

    public void Update(Vehicle vehicle)
    {
        _dbContext.vehicles.Update(vehicle);
        _dbContext.SaveChanges();
    }
  }
}