using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.Entities
{
    public class Admin
    {   [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = default!;
        [Required]
        [StringLength(255)]
        public string Email { get; set; } = default!;
        [StringLength(50)]
        public string Password { get; set; } = default!;
        [StringLength(10)]
         public string Profile { get; set; } = default!;
    }
}