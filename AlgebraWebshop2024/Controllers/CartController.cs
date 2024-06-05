using Microsoft.AspNetCore.Mvc;
using AlgebraWebshop2024.Data;
using AlgebraWebshop2024.Models;
using AlgebraWebshop2024.Extensions;

namespace AlgebraWebshop2024.Controllers
{
    public class CartController : Controller
    {
        public const string SessionKeyName = "_cart";

        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) ?? new List<CartItem>();

            decimal sum = 0;
            ViewBag.TotalPrice = cart.Sum(item => item.getTotal())+sum;
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            List<CartItem> cart=HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) ?? new List<CartItem>();

            bool new_item = true;
            foreach (var item in cart)
            {
                if (item.Product.Id == productId)
                {
                    item.Quantity++;
                    new_item = false;
                }
            }

            if (new_item)
            {
                CartItem new_product = new CartItem()
                {
                    Product = _context.Product.Find(productId),
                    Quantity = 1
                };
                cart.Add(new_product);
            }

            HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int productId)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) ?? new List<CartItem>();

            CartItem itemToRemove = cart.Find(item => item.Product.Id == productId);
            cart.Remove(itemToRemove);

            HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, decimal quantity)
        {
            if(quantity< 0)
            {
                return RedirectToAction(nameof(Index));
            }

            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) ?? new List<CartItem>();

            CartItem itemToUpdate = cart.Find(item => item.Product.Id == productId);
            itemToUpdate.Quantity = quantity;
            if(quantity == 0)
            {
                cart.Remove(itemToUpdate);
            }

            HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);
            return RedirectToAction(nameof(Index));
        }
    }
}
