using AlgebraWebshop2024.Data;
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

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
