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
  public class AdminService : IAdminService
  {
    private readonly ApplicationDbContext _dbContext;
    public AdminService(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }
        public Admin? Login(LoginDTO loginDTO)
        {
            var adm = _dbContext.Admins.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
            return adm;
        }
  }
}