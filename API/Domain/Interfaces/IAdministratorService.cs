using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.DTOs;
using  MinimalApi.Domain.Entities;

namespace  MinimalApi.Domain.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDTO loginDTO);
        Administrator Add(Administrator Administrator);
        List<Administrator> GetAllAdministrators(int? page);
        Administrator? GetAdministratorById(int id);
    }
}