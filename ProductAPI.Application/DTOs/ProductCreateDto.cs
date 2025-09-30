using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProductAPI.Application.DTOs
{
    public class ProductCreateDto
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
