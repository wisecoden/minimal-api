using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.ViewModels
{
    public class ValidationErrosViewModel
    {   
        public List<string> Messages { get; set; }
    }
    
}