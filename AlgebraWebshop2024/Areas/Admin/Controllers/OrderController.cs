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
            var dbusers=_context.Users.ToList();
            dbusers.Insert(0,new ApplicationUser{Id=null,UserName="Anonymus User"});
            SelectList users = new SelectList(dbusers, "Id", "UserName");
            ViewBag.UsersList = users;

            order.OrderItems = _context.OrderItem.Where(oi => oi.OrderId == order.Id).ToList();
            foreach (var item in order.OrderItems)
            {
                Product p = _context.Product.Where(p => p.Id == item.ProductId).FirstOrDefault();
                item.ProductTitle = p.Title;
                item.ProductUnit = p.Unit;
            }

            ViewBag.Products = new SelectList(_context.Product.ToList(), "Id", "Title");

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
            ModelState.Remove("OrderItems");
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

        public IActionResult RemoveOrderItem(int orderItemId)
        {
            var orderItem = _context.OrderItem.FirstOrDefault(oi => oi.Id == orderItemId);
            if (orderItem == null) return NotFound();
            int orderId = orderItem.OrderId;

            Product product = _context.Product.FirstOrDefault(p => p.Id == orderItem.ProductId);
            product.Quantity += orderItem.Quantity;
            _context.Update(product);
            _context.OrderItem.Remove(orderItem);
            _context.SaveChanges();

            return RedirectToAction("Edit", new { id = orderId });
        }

        [HttpPost]
        public IActionResult AddOrderItem(int orderId,int productId)
        {
            Product product = _context.Product.FirstOrDefault(p => p.Id == productId);
            OrderItem orderItem = new OrderItem() 
            {
                OrderId=orderId,
                ProductId=productId,
                Quantity = 1,
                Price = product.Price,
                Discount = product.Discount,
                VAT = product.VAT
            };
            _context.OrderItem.Add(orderItem);
            product.Quantity -= 1;
            _context.Update(product);
            _context.SaveChanges();
            return RedirectToAction("Edit", new { id = orderId });
        }

        [HttpPost]
        public IActionResult UpdateOrderItem(int Id,int OrderId,string Quantity,string Price,string Discount,string VAT)
        {
            var update_oi=_context.OrderItem.Where(oi=>oi.Id==Id).FirstOrDefault();
            if (update_oi==null) return NotFound();
            decimal new_quantity=decimal.Parse(Quantity.Replace(".", ","));
            if(update_oi.Quantity!=new_quantity)
            {
                Product product = _context.Product.FirstOrDefault(p => p.Id == update_oi.ProductId);
                product.Quantity -= (new_quantity - update_oi.Quantity);
                _context.Update(product);
                update_oi.Quantity=new_quantity;
            }
            update_oi.Price=decimal.Parse(Price.Replace(".", ","));
            update_oi.Discount= decimal.Parse(Discount.Replace(".", ","));
            update_oi.VAT=decimal.Parse(VAT.Replace(".", ","));
            _context.Update(update_oi);
            _context.SaveChanges();
            return RedirectToAction("Edit", new { id = OrderId });
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
            Order cancel_order= new Order();
            cancel_order.DateCreated=DateTime.Now;
            cancel_order.DiscountAmmount=-order.DiscountAmmount;
            cancel_order.Message="Order is canceled! \n"+order.Message;
            cancel_order.UserId=order.UserId;
            cancel_order.IsPaid=order.IsPaid;
            cancel_order.PaymentMethod=order.PaymentMethod;
            cancel_order.Status="Canceled Order "+order.Id;
            order.Status = "Canceled";
            cancel_order.BillingFirstName=order.BillingFirstName;
            cancel_order.BillingLastName=order.BillingLastName;
            cancel_order.BillingEmailAddress=order.BillingEmailAddress;
            cancel_order.BillingPhone=order.BillingPhone;
            cancel_order.BillingAddress=order.BillingAddress;
            cancel_order.BillingCity=order.BillingCity;
            cancel_order.BillingZip=order.BillingZip;
            cancel_order.BillingCountry=order.BillingCountry;
            cancel_order.ShippingFirstName=order.ShippingFirstName;
            cancel_order.ShippingLastName=order.ShippingLastName;
            cancel_order.ShippingEmailAddress=order.ShippingEmailAddress;
            cancel_order.ShippingPhone=order.ShippingPhone;
            cancel_order.ShippingAddress=order.ShippingAddress;
            cancel_order.ShippingCity=order.ShippingCity;
            cancel_order.ShippingZip=order.ShippingZip;
            cancel_order.ShippingCountry=order.ShippingCountry;

            _context.Update(order);
            _context.Add(cancel_order);
            await _context.SaveChangesAsync();

            var orderItems=_context.OrderItem.Where(oi => oi.OrderId == order.Id).ToList();
            foreach(var item in orderItems)
            {
                OrderItem cancel_item=new OrderItem();
                cancel_item.OrderId=cancel_order.Id;
                cancel_item.ProductId=item.ProductId;
                cancel_item.Quantity=-item.Quantity;
                Product product= _context.Product.Where(p => p.Id == item.ProductId).FirstOrDefault();
                product.Quantity += item.Quantity;
                cancel_item.Price=item.Price;
                cancel_item.Discount=item.Discount;
                cancel_item.VAT=item.VAT;
                _context.Update(product);
                _context.Add(cancel_item);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }

        [AllowAnonymous]
        public IActionResult MyOrders()
        {
            var user=_context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user==null) return RedirectToAction("Index","Home",new { area="" });
            var orders=_context.Order.Where(o=>o.UserId==user.Id).ToList();
            foreach (var order in orders)
            {
                order.OrderItems = _context.OrderItem.Where(oi => oi.OrderId == order.Id).ToList();
            }
            return View(orders);
        }

        [AllowAnonymous]
        public IActionResult MyOrderDetails(int? id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null) return RedirectToAction("Index", "Home", new { area = "" });
            ViewBag.User = user;
            if (id == null)
            {
                return NotFound();
            }

            var order = _context.Order.FirstOrDefault(m => m.Id == id);
            if (order == null)
            {
                return RedirectToAction("MyOrders");
            }
            if (order.UserId != user.Id)
            {
                return RedirectToAction("MyOrders");
            }

            order.OrderItems = _context.OrderItem.Where(oi => oi.OrderId == order.Id).ToList();
            foreach (var item in order.OrderItems)
            {
                Product p = _context.Product.Where(p => p.Id == item.ProductId).FirstOrDefault();
                if (p == null) continue;
                item.ProductTitle = p.Title;
                item.ProductUnit = p.Unit;
            }
            

            return View("Details",order);
        }
    }
}
