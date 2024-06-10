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

namespace AlgebraWebshop2024.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Order
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Order.ToListAsync();
            foreach (var order in orders)
            {
                order.OrderItems=_context.OrderItem.Where(oi=>oi.OrderId==order.Id).ToList();
            }
            ViewBag.Users = _context.Users.ToList();
            return View(orders);
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderItems=_context.OrderItem.Where(oi=>oi.OrderId==order.Id).ToList();
            foreach (var item in order.OrderItems)
            {
                Product p = _context.Product.Where(p => p.Id == item.ProductId).FirstOrDefault();
                if(p==null) continue;
                item.ProductTitle = p.Title;
                item.ProductUnit = p.Unit;
            }
            ViewBag.User=_context.Users.FirstOrDefault(u=>u.Id==order.UserId);

            return View(order);
        }

        // GET: Admin/Order/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Admin/Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,DateCreated,DiscountAmmount,Message,UserId,IsPaid,PaymentMethod,Status,BillingFirstName,BillingLastName,BillingEmailAddress,BillingPhone,BillingAddress,BillingCity,BillingZip,BillingCountry,ShippingFirstName,ShippingLastName,ShippingEmailAddress,ShippingPhone,ShippingAddress,ShippingCity,ShippingZip,ShippingCountry")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(order);
        //}

        // GET: Admin/Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Admin/Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateCreated,DiscountAmmount,Message,UserId,IsPaid,PaymentMethod,Status,BillingFirstName,BillingLastName,BillingEmailAddress,BillingPhone,BillingAddress,BillingCity,BillingZip,BillingCountry,ShippingFirstName,ShippingLastName,ShippingEmailAddress,ShippingPhone,ShippingAddress,ShippingCity,ShippingZip,ShippingCountry")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Admin/Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderItems = _context.OrderItem.Where(oi => oi.OrderId == order.Id).ToList();
            foreach (var item in order.OrderItems)
            {
                Product p = _context.Product.Where(p => p.Id == item.ProductId).FirstOrDefault();
                if (p == null) continue;
                item.ProductTitle = p.Title;
                item.ProductUnit = p.Unit;
            }
            ViewBag.User = _context.Users.FirstOrDefault(u => u.Id == order.UserId);

            return View(order);
        }

        // POST: Admin/Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                var orderItems=_context.OrderItem.Where(oi => oi.OrderId == order.Id).ToList();
                foreach(var orderItem in orderItems)
                {
                    var product= _context.Product.Where(p => p.Id == orderItem.ProductId).FirstOrDefault();
                    if (product!=null)
                    {
                        product.Quantity += orderItem.Quantity;
                        _context.Update(product);
                    }
                }
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Order/Cancel/5
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderItems = _context.OrderItem.Where(oi => oi.OrderId == order.Id).ToList();
            foreach (var item in order.OrderItems)
            {
                Product p = _context.Product.Where(p => p.Id == item.ProductId).FirstOrDefault();
                if (p == null) continue;
                item.ProductTitle = p.Title;
                item.ProductUnit = p.Unit;
            }
            ViewBag.User = _context.Users.FirstOrDefault(u => u.Id == order.UserId);

            return View(order);
        }


        // POST: Admin/Order/Cancel/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            //TODO: implement cancel order
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
