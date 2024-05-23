using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebshop2024.Models
{
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public int CategoryId { get; set; }

        [NotMapped]
        public string ProductTitle { get; set; }

        [NotMapped]
        public string CategoryTitle { get; set; }
    }
}
