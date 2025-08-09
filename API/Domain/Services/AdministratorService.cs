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
  public class AdministratorService : IAdministratorService
  {
    private readonly ApplicationDbContext _dbContext;
    public AdministratorService(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

    public Administrator Add(Administrator administrator)
    {
      _dbContext.Administrators.Add(administrator);
      _dbContext.SaveChanges();
      return administrator;
    }

    public Administrator? GetAdministratorById(int id)
    {
      return _dbContext.Administrators.Where(a => a.Id == id).FirstOrDefault();
    }

    public List<Administrator> GetAllAdministrators(int? page)
    {
      var query = _dbContext.Administrators.AsQueryable();

      int pageSize = 10; // Define the number of items per page

      if (page != null)
      {
         query = query.Skip(((int)page - 1) * pageSize).Take(pageSize);  
      }
         
      return query.ToList();
    }

    public Administrator? Login(LoginDTO loginDTO)
        {
            var adm = _dbContext.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
            return adm;
        }
  }
}