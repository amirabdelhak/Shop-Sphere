using graduated_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using Stripe;
using System.Text.Json;

namespace graduated_project.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopSpheredbcontext _context;
        private readonly IConfiguration _configuration;

        public CartController(ShopSpheredbcontext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

        private void SaveCart(Dictionary<int, int> cart)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7)
            };
            Response.Cookies.Append("cart", JsonSerializer.Serialize(cart), options);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCart();

            if (cart.ContainsKey(productId))
            {
                cart[productId] += quantity;
            }
            else
            {
                cart[productId] = quantity;
            }

            if (cart[productId] > product.Quantity)
            {
                cart[productId] = product.Quantity;
                TempData["Warning"] = $"Only {product.Quantity} of {product.Name} available";
            }

            SaveCart(cart);
            return RedirectToAction("GetProducts", "Products");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCart();

            if (quantity <= 0)
            {
                cart.Remove(productId);
            }
            else
            {
                if (quantity > product.Quantity)
                {
                    quantity = product.Quantity;
                    TempData["Warning"] = $"Only {product.Quantity} of {product.Name} available";
                }
                cart[productId] = quantity;
            }

            SaveCart(cart);
            return RedirectToAction("ViewCart");
        }

        public IActionResult ViewCart()
        {
            var cart = GetCart();
            var productIds = cart.Keys.ToList();

            var products = _context.Products
                            .Where(p => productIds.Contains(p.Id))
                            .ToList();

            ViewBag.CartQuantities = cart;
            ViewBag.TotalPrice = products.Sum(p => p.Priceafterdiscount * cart.GetValueOrDefault(p.Id, 1));

            return View(products);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.Remove(productId);
            SaveCart(cart);
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public IActionResult EmptyCart()
        {
            Response.Cookies.Delete("cart");
            return RedirectToAction("ViewCart");
        }

        public async Task<IActionResult> Checkout()
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var cart = GetCart();
            if (!cart.Any())
            {
                return RedirectToAction("ViewCart", "Cart");
            }

            var productIds = cart.Keys.ToList();
            var products = _context.Products
                            .Where(p => productIds.Contains(p.Id))
                            .ToList();

            if (products == null || !products.Any())
            {
                return RedirectToAction("ViewCart", "Cart");
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

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var product in products)
            {
                var quantity = cart.GetValueOrDefault(product.Id, 1);
                var lineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(product.Priceafterdiscount * 100),
                        Currency = "EGP",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = product.Name,
                        },
                    },
                    Quantity = quantity,
                };

                lineItems.Add(lineItem);
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:44321/Order/Add/",
                CancelUrl = "https://localhost:44321/Cart/ViewCart/",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }
    }
}
