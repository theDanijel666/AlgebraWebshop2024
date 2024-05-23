using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebshop2024.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(300,MinimumLength = 2)]
        public string Title { get; set; }

        [ForeignKey("CategoryId")]
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
