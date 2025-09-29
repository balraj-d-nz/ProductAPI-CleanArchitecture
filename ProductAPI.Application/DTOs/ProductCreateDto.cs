namespace ProductAPI.Application.DTOs
{
    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
