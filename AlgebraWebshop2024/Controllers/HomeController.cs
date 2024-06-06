using AlgebraWebshop2024.Data;
using AlgebraWebshop2024.Extensions;
using AlgebraWebshop2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace AlgebraWebshop2024.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public const string SessionKeyName = "_cart";

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string? message)
        {
            ViewBag.Message = message;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Product(int? categoryId)
        {
            List<Product> products=_context.Product.ToList();

            if (categoryId != null)
            {
                products = (
                    from product in products
                    join pro_cat in _context.ProductCategory on product.Id equals pro_cat.ProductId
                    where pro_cat.CategoryId == categoryId
                    select product
                    ).ToList();
            }

            products.ForEach(product=>product.ProductImages=_context.ProductImage.Where(
                    i => i.ProductId == product.Id).ToList());

            ViewBag.Categories = _context.Category.Select(c =>
            new SelectListItem
            {
                Value=c.Id.ToString(),
                Text=c.Title
            }).ToList();

            return View(products);
        }

        public IActionResult Order(List<string> errors)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) ?? new List<CartItem>();

            if (cart.Count == 0)
            {
                return RedirectToAction(nameof(Product));
            }

            foreach(var item in cart)
            {
                var prod=_context.Product.Find(item.Product.Id);
                if (prod.Quantity < item.Quantity)
                {
                    errors.Add("Product " + prod.Title + " not available in desired quantity! " +
                        "Max avaliable quantity is "+prod.Quantity.ToString());
                }
            }

            ViewBag.TotalPrice = cart.Sum(item => item.getTotal());
            ViewBag.Errors = errors;
            return View(cart);
        }

        [HttpPost]
        public IActionResult CreateOrder(Order order,string ShippingSameAsBilling)
        {
            var errors = new List<string>();

            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) ?? new List<CartItem>();
            if (cart.Count == 0)
            {
                return RedirectToAction(nameof(Product));
            }

            foreach(var item in cart)
            {
               var prod=_context.Product.Find(item.Product.Id);
                if (prod.Quantity>prod.Quantity)
                {
                    ModelState.AddModelError("produdct" + prod.Id.ToString(), "Product " + prod.Title + " no available in desired quantity!");
                }
            }

            if (ShippingSameAsBilling=="on")
            {
                order.ShippingFirstName= order.BillingFirstName;
                ModelState.Remove("ShippingFirstName");
                order.ShippingLastName = order.BillingLastName;
                ModelState.Remove("ShippingLastName");
                order.ShippingEmailAddress = order.BillingEmailAddress;
                ModelState.Remove("ShippingEmailAddress");
                order.ShippingPhone = order.BillingPhone;
                ModelState.Remove("ShippingPhone");
                order.ShippingAddress = order.BillingAddress;
                ModelState.Remove("ShippingAddress");
                order.ShippingCity = order.BillingCity;
                ModelState.Remove("ShippingCity");
                order.ShippingZip = order.BillingZip;
                ModelState.Remove("ShippingZip");
                order.ShippingCountry = order.BillingCountry;
                ModelState.Remove("ShippingCountry");
            }

            order.DateCreated = DateTime.Now;
            order.IsPaid = false;
            
            if(User.Identity.IsAuthenticated)
            {
                order.UserId = _context.Users.Where(u=>u.UserName==User.Identity.Name).First().Id;
            }
            else
            {
                order.UserId = null;
            }
            ModelState.Remove("UserId");
            ModelState.Remove("Message");
            if(order.Message==null)
            {
                order.Message = "";
            }
            ModelState.Remove("OrderItems");
            ModelState.Remove("ShippingSameAsBilling");
            
            if (ModelState.IsValid)
            {
                _context.Order.Add(order);
                _context.SaveChanges();

                int orderId = order.Id;

                foreach (var item in cart)
                {
                    OrderItem order_item = new OrderItem()
                    {
                        OrderId = orderId,
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity,
                        Price=item.Product.Price,
                        Discount=item.Product.Discount,
                        VAT=item.Product.VAT
                    };
                    _context.OrderItem.Add(order_item);
                    var prod=_context.Product.Find(item.Product.Id);
                    prod.Quantity-=item.Quantity;
                    _context.Product.Update(prod);
                    _context.SaveChanges();
                }

                HttpContext.Session.SetObjectAsJson(SessionKeyName, "");

                return RedirectToAction(nameof(Index), new { message = "Thank you for ordering :)" });
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
            }

            return RedirectToAction(nameof(Order),new {errors=errors});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
