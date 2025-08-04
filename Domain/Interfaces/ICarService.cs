using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.DTOs;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces
{
    public interface ICarService
    {
        List<Car> GetAllCars(int? page = 1, string? name = null, string? brand = null);
        Car? GetCarById(int id);
        void Add (Car car);
        void Update (Car car);
        void Remove (Car car);
    }
}