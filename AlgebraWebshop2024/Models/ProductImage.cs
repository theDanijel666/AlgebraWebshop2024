using System.ComponentModel.DataAnnotations;

namespace AlgebraWebshop2024.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool MainImage { get; set; }=false;
    }
}
