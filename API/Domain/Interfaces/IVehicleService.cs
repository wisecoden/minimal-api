using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.DTOs;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces
{
    public interface IVehicleService
    {
        List<Vehicle> GetAllVehicles(int? page = 1, string? name = null, string? brand = null);
        Vehicle? GetVehicleById(int id);
        void Add (Vehicle vehicle);
        void Update (Vehicle Vehicle);
        void Remove (Vehicle vehicle);
    }
}