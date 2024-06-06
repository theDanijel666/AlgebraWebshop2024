using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebshop2024.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }=DateTime.Now;

        [Column(TypeName = "decimal(12, 4)")]
        public decimal DiscountAmmount { get; set; } = 0;

        [StringLength(200,ErrorMessage ="Message too long! Limi to 200 characters")]
        public string? Message { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("OrderId")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public decimal TotalPrice() {
            decimal total = 0;
            foreach (var item in OrderItems)
            {
                total += item.ItemTotalPrice();
            }
            return total-DiscountAmmount;
        }

        public bool IsPaid { get; set; } = false;
        public string PaymentMethod { get; set; } = "Bank transfer";
        //public string PaymentMethodType { get; set; }
        //public string PaymentMethodToken { get; set; }
        //public string PaymentMethodStatus { get; set; }
        //public string PaymentMethodMessage { get; set; }

        //billing info
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name too long! Limit to 100 characters")]
        public string BillingFirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name too long! Limit to 100 characters")]
        public string BillingLastName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [StringLength(100, ErrorMessage = "Email address too long! Limit to 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string BillingEmailAddress { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(40, ErrorMessage = "Phone number too long! Limit to 40 characters")]
        public string BillingPhone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address too long! Limit to 200 characters")]
        public string BillingAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City too long! Limit to 100 characters")]
        public string BillingCity { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        [StringLength(10, ErrorMessage = "Zip code too long! Limit to 10 characters")]
        public string BillingZip { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100, ErrorMessage = "Country too long! Limit to 100 characters")]
        public string BillingCountry { get; set; }


        //shipping info
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name too long! Limit to 100 characters")]
        public string ShippingFirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name too long! Limit to 100 characters")]
        public string ShippingLastName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [StringLength(100, ErrorMessage = "Email address too long! Limit to 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string ShippingEmailAddress { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(40, ErrorMessage = "Phone number too long! Limit to 40 characters")]
        public string ShippingPhone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address too long! Limit to 200 characters")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City too long! Limit to 100 characters")]
        public string ShippingCity { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        [StringLength(10, ErrorMessage = "Zip code too long! Limit to 10 characters")]
        public string ShippingZip { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100, ErrorMessage = "Country too long! Limit to 100 characters")]
        public string ShippingCountry { get; set; }

    }
}
