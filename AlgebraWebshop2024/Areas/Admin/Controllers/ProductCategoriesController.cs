using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlgebraWebshop2024.Data;
using AlgebraWebshop2024.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;

namespace AlgebraWebshop2024.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductCategories
        public async Task<IActionResult> Index(int productId)
        {
            if (productId <= 0)
            {
                return NotFound();
            }
            var product = await _context.Product.FindAsync(productId);
            if (product==null)
            {
                return NotFound();
            }
            List<ProductCategory> list= await (
                    from pc in _context.ProductCategory
                    where pc.ProductId == productId
                    select new ProductCategory()
                    {
                        Id=pc.Id,
                        ProductId=pc.ProductId,
                        CategoryId=pc.CategoryId,
                        ProductTitle= _context.Product.Where(p=>p.Id==pc.ProductId).FirstOrDefault().Title,
                        CategoryTitle= _context.Category.Where(c=>c.Id==pc.CategoryId).FirstOrDefault().Title

                    }).ToListAsync();
            ViewBag.ProductId=productId;
            return View(list);
        }

        // GET: Admin/ProductCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            productCategory.ProductTitle = _context.Product.Where(p => p.Id == productCategory.ProductId).First().Title;
            productCategory.CategoryTitle = _context.Category.Where(c => c.Id == productCategory.CategoryId).First().Title;

            return View(productCategory);
        }

        // GET: Admin/ProductCategories/Create
        public IActionResult Create(int productId)
        {
            if (productId <= 0)
            {
                return NotFound();
            }
            var product = _context.Product.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.ProductId = productId;
            var cat=new SelectList(_context.Category, "Id", "Title");
            //TODO: remove already set catetogories
            ViewBag.Categories = cat;
            ProductCategory pc=new ProductCategory() { 
                ProductId = productId,
                ProductTitle = product.Title
            };


            return View(pc);
        }

        // POST: Admin/ProductCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,CategoryId")] ProductCategory productCategory)
        {
            ModelState.Remove("ProductTitle");
            ModelState.Remove("CategoryTitle");
            if (ModelState.IsValid)
            {
                _context.Add(productCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),new { productId = productCategory.ProductId });
            }
            return View(productCategory);
        }

        // GET: Admin/ProductCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategory.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }
            var cat = new SelectList(_context.Category, "Id", "Title",productCategory.CategoryId);
            ViewBag.Categories = cat;
            productCategory.ProductTitle=_context.Product.Where(p => p.Id == productCategory.ProductId).First().Title;

            return View(productCategory);
        }

        // POST: Admin/ProductCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,CategoryId")] ProductCategory productCategory)
        {
            if (id != productCategory.Id)
            {
                return NotFound();
            }

            ModelState.Remove("ProductTitle");
            ModelState.Remove("CategoryTitle");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCategoryExists(productCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { productId = productCategory.ProductId });
            }
            return View(productCategory);
        }

        // GET: Admin/ProductCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            productCategory.ProductTitle = _context.Product.Where(p => p.Id == productCategory.ProductId).First().Title;
            productCategory.CategoryTitle = _context.Category.Where(c => c.Id == productCategory.CategoryId).First().Title;

            return View(productCategory);
        }

        // POST: Admin/ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productCategory = await _context.ProductCategory.FindAsync(id);
            int productId=0;
            if (productCategory != null)
            {
                productId = productCategory.ProductId;
                _context.ProductCategory.Remove(productCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index),new { productId = productId});
        }

        private bool ProductCategoryExists(int id)
        {
            return _context.ProductCategory.Any(e => e.Id == id);
        }
    }
}
