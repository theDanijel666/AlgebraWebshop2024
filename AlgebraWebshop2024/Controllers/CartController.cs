using Microsoft.AspNetCore.Mvc;
using AlgebraWebshop2024.Data;
using AlgebraWebshop2024.Models;

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
            return View();
        }
    }
}
