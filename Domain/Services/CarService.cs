using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.infra.Db;

namespace minimal_api.Domain.Services
{
  public class CarService : ICarService
  {
    private readonly ApplicationDbContext _dbContext;
    public CarService(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

    public void Add(Car car)
    {
        _dbContext.Cars.Add(car);
        _dbContext.SaveChanges();
    }

    public List<Car> GetAllCars(int? page = 1, string? name = null, string? brand = null)
    {
      var query = _dbContext.Cars.AsQueryable();
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

    public Car? GetCarById(int id)
    {
      return _dbContext.Cars.Where(c => c.Id == id).FirstOrDefault();
    }

    public void Remove(Car car)
    {
       _dbContext.Cars.Remove(car);
       _dbContext.SaveChanges();
    }

    public void Update(Car car)
    {
        _dbContext.Cars.Update(car);
        _dbContext.SaveChanges();
    }
  }
}