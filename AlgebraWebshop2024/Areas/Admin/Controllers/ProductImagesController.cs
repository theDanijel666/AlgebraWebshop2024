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
    public class ProductImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductImages
        public async Task<IActionResult> Index(int productId)
        {
            if (productId <= 0)
            {
                return NotFound();
            }
            var product = await _context.Product.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }
            var images= await _context.ProductImage.Where(i => i.ProductId == productId).ToListAsync();
            ViewBag.product = product;
            return View(images);
        }

        // GET: Admin/ProductImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // GET: Admin/ProductImages/Create
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
            ViewBag.product = product;
            ProductImage pi = new ProductImage()
            {
                ProductId=productId
            };
            return View(pi);
        }

        // POST: Admin/ProductImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,ImageUrl,Name,Description,MainImage")] ProductImage productImage,
            IFormFile UploadImage)
        {
            if (UploadImage == null)
            {
                ModelState.AddModelError("ImageUrl", "Image is mandatory!");
            }
            var fileextension = System.IO.Path.GetExtension(UploadImage.FileName);
            if (fileextension != ".jpg" && fileextension != ".jpeg" && fileextension != ".png")
            {
                ModelState.AddModelError("ImageUrl", "Only jpg, jpeg and png files are allowed!");
            }
            if (ModelState.IsValid)
            {
                // Save image to wwwroot/images
                var fileName = Guid.NewGuid().ToString() + fileextension;
                var path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Create))
                {
                    await UploadImage.CopyToAsync(stream);
                }
                // primjer rezultata C:\work\AlgebraWebshop2024\wwwroot\Images\123456.jpg spremanja datoteke
                // format koji nam treba za imageUrl /Images/123456.jpg
                productImage.ImageUrl = "/Images/" + fileName;
                _context.Add(productImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { productId = productImage.ProductId });
            }

            var product = _context.Product.Find(productImage.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.product = product;
            return View(productImage);
        }

        // GET: Admin/ProductImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImage.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }
            return View(productImage);
        }

        // POST: Admin/ProductImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,ImageUrl,Name,Description,MainImage")] ProductImage productImage,
            IFormFile UploadImage)
        {
            if (id != productImage.Id)
            {
                return NotFound();
            }
            ModelState.Remove("UploadImage");
            if (ModelState.IsValid)
            {
                try
                {
                    string oldFile = "";
                    //check if new image is uploaded, if so, replace the old one
                    if (UploadImage != null)
                    {
                        var fileextension = System.IO.Path.GetExtension(UploadImage.FileName);
                        var fileName = Guid.NewGuid().ToString() + fileextension;
                        var path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
                        using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Create))
                        {
                            await UploadImage.CopyToAsync(stream);
                        }
                        oldFile= Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/Images",
                            productImage.ImageUrl.Replace("/Images/", ""));
                        productImage.ImageUrl = "/Images/" + fileName;
                    }
                    _context.Update(productImage);
                    await _context.SaveChangesAsync();
                    if (oldFile != "")
                    {
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductImageExists(productImage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { productId = productImage.ProductId });
            }
            return View(productImage);
        }

        // GET: Admin/ProductImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // POST: Admin/ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productImage = await _context.ProductImage.FindAsync(id);
            int productId = 0;
            if (productImage != null)
            {
                productId = productImage.ProductId;
                var filename=productImage.ImageUrl.Replace("/Images/", "");
                var path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot/Images", filename);
                _context.ProductImage.Remove(productImage);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index),new {productId= productId});
        }

        private bool ProductImageExists(int id)
        {
            return _context.ProductImage.Any(e => e.Id == id);
        }
    }
}
