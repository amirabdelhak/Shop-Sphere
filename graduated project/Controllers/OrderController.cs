using graduated_project.Models;
using graduated_project.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace graduated_project.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly ShopSpheredbcontext context;

        public OrderController(IOrderRepository orderRepository, ShopSpheredbcontext context)
        {
            this.orderRepository = orderRepository;
            this.context = context;
        }

        private Dictionary<int, int> GetCart()
        {
            var cartCookie = Request.Cookies["cart"];
            if (string.IsNullOrEmpty(cartCookie))
            {
                return new Dictionary<int, int>();
            }
            return JsonSerializer.Deserialize<Dictionary<int, int>>(cartCookie) ?? new Dictionary<int, int>();
        }

        public IActionResult GetAll()
        {
            var orders = orderRepository.GetAll();
            return View(orders);
        }

        public IActionResult Get(int id)
        {
            var order = orderRepository.GetByid(id);
            return View(order);
        }

        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Add()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                return RedirectToAction("ViewCart", "Cart");
            }

            var productIds = cart.Keys.ToList();
            var products = context.Products
                             .Where(p => productIds.Contains(p.Id))
                             .ToList();

            if (products == null || !products.Any())
            {
                return RedirectToAction("ViewCart", "Cart");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var user = await context.Users.FindAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            foreach (var product in products)
            {
                var requestedQty = cart.GetValueOrDefault(product.Id, 1);
                if (requestedQty > product.Quantity)
                {
                    TempData["Error"] = $"Requested quantity of {product.Name} ({requestedQty}) exceeds available stock ({product.Quantity})";
                    return RedirectToAction("ViewCart", "Cart");
                }
            }

            decimal totalPrice = products.Sum(p => p.Priceafterdiscount * cart.GetValueOrDefault(p.Id, 1));

            var order = new graduated_project.Models.Order
            {
                OrderDate = DateTime.Now,
                ShippingAddress = user.Address,
                TotalPrice = totalPrice,
                UserId = userId
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            foreach (var product in products)
            {
                var quantity = cart.GetValueOrDefault(product.Id, 1);
                
                var productOrder = new ProductOrder
                {
                    ProductId = product.Id,
                    OrderId = order.Id,
                    Quantity = quantity
                };
                context.ProductOrders.Add(productOrder);

                product.Quantity -= quantity;
            }

            await context.SaveChangesAsync();

            Response.Cookies.Delete("cart");

            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await context.Orders
                                .Include(o => o.ProductOrders)
                                .ThenInclude(po => po.Product)
                                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var order = orderRepository.GetByid(id);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, graduated_project.Models.Order order)
        {
            if (ModelState.IsValid)
            {
                orderRepository.Update(id, order);
                return RedirectToAction(nameof(GetAll));
            }
            return View(order);
        }

        public IActionResult Delete(int id)
        {
            orderRepository.Delete(id);
            return RedirectToAction(nameof(GetAll));
        }
    }
}
