using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebshop2024.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        [Column(TypeName = "decimal(9, 2)")]
        public decimal Quantity { get; set; } = 0;

        [Column(TypeName = "decimal(12, 4)")]
        public decimal Price { get; set; } = 0;

        [Column(TypeName = "decimal(9, 2)")]
        public decimal Discount { get; set; } = 0;

        [Column(TypeName = "decimal(9, 2)")]
        public decimal VAT { get; set; } = 0;

        public decimal ItemTotalPrice() { 
            return Price*(1-Discount/100)*(1+VAT/100)*Quantity; 
        }

        [NotMapped]
        public string ProductTitle { get; set; }

        [NotMapped]
        public string ProductUnit { get; set; }
    }
}
