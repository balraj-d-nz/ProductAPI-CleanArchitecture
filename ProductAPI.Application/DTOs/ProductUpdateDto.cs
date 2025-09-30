using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProductAPI.Application.DTOs
{
    public class ProductUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(150)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Precision(7,2)]
        public decimal Price { get; set; }
    }
}
