using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebshop2024.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Unit { get; set; }

        [Column(TypeName ="decimal(9, 2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(12, 4)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(9, 2)")]
        public decimal Discount { get; set; } = 0;

        [Column(TypeName = "decimal(9, 2)")]
        public decimal VAT { get; set; } = 0;

        public decimal TotalPrice()
        {
           return Price * (1 - Discount / 100) * (1 + VAT / 100);
        }

        [ForeignKey("ProductId")]
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }

        [ForeignKey("ProductId")]
        public virtual ICollection<ProductImage> ProductImages { get; set; }

        [ForeignKey("ProductId")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
