using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pb304PetShop.DataContext;
using Pb304PetShop.Models;

namespace Pb304PetShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ShopController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ProductCount = await _dbContext.Products.CountAsync();
            var products = await _dbContext.Products.Take(3).ToListAsync();
            var wishlist = Request.Cookies["Wishlist"];
            List<int> wishlistProductIds = new List<int>();

            if (!string.IsNullOrEmpty(wishlist))
            {
                var wishlistItems = JsonConvert.DeserializeObject<List<WishlistItemCookieModel>>(wishlist);
                wishlistProductIds = wishlistItems?.Select(w => w.ProductId).ToList() ?? new List<int>();
            }
            var model = new HomeViewModel
            {
                Products = products,
                WishlistProductIds = wishlistProductIds
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Partial([FromBody]RequestModel requestmodel)
        {
            var products = await _dbContext.Products.Skip(requestmodel.StartFrom).Take(3).ToListAsync();
            var wishlist = Request.Cookies["Wishlist"];
            List<int> wishlistProductIds = new List<int>();

            if (!string.IsNullOrEmpty(wishlist))
            {
                var wishlistItems = JsonConvert.DeserializeObject<List<WishlistItemCookieModel>>(wishlist);
                wishlistProductIds = wishlistItems?.Select(w => w.ProductId).ToList() ?? new List<int>();
            }
            var model = new HomeViewModel
            {
                Products = products,
                WishlistProductIds = wishlistProductIds
            };
            return PartialView("_ProductPartial", model);
        }
    }
    public class RequestModel
    {
        public int StartFrom { get; set; }
    }
}
