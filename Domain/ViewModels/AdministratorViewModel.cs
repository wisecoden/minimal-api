using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Domain.Enums;

namespace minimal_api.Domain.ViewModels
{
    public record AdministratorViewModel
    {   
        public int Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Profile { get; set; } = default!;

    }
    
}