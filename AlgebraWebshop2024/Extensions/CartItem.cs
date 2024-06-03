using AlgebraWebshop2024.Models;

namespace AlgebraWebshop2024.Extensions
{
    public class CartItem
    {
        public Product Product { get; set; }
        public decimal Quantity { get; set; }

        public decimal getTotal()
        {

           return Product.TotalPrice() * Quantity;
        }
    }
}
