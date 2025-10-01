using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProductAPI.Application.DTOs
{
    public class ProductPatchDto
    {
        [MaxLength(50)]
        public string? Name { get; set; }
        [MaxLength(150)]
        public string? Description { get; set; }
        [Precision(7, 2)]
        public decimal? Price { get; set; }
    }
}
