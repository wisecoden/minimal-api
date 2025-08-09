using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace  MinimalApi.Domain.Entities
{
    public class Vehicle
    {   [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = default!;
        [StringLength(100)]
        [Required]
        public string Brand { get; set; } = default!;
        [StringLength(10)]
        [Required]
        public int Year { get; set; } = default!;
    }
}