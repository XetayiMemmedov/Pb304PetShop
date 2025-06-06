using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pb304PetShop.DataContext;
using Pb304PetShop.Models;

namespace Pb304PetShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IActionResult> Index()
        {
            var sliders = await _dbContext.Sliders.ToListAsync();
            var products = await _dbContext.Products.ToListAsync();
            var categories = await _dbContext.Categories.ToListAsync();

            var wishlist = Request.Cookies["Wishlist"];
            List<int> wishlistProductIds = new List<int>();

            if (!string.IsNullOrEmpty(wishlist))
            {
                var wishlistItems = JsonConvert.DeserializeObject<List<WishlistItemCookieModel>>(wishlist);
                wishlistProductIds = wishlistItems?.Select(w => w.ProductId).ToList() ?? new List<int>();
            }

            var model = new HomeViewModel
            {
                Sliders = sliders,
                Products = products,
                Categories = categories,
                WishlistProductIds = wishlistProductIds
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreProducts(int skip = 6, int take = 6)
        {
            var products = await _dbContext.Products
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var wishlist = Request.Cookies["Wishlist"];
            List<int> wishlistProductIds = new();

            if (!string.IsNullOrEmpty(wishlist))
            {
                var wishlistItems = JsonConvert.DeserializeObject<List<WishlistItemCookieModel>>(wishlist);
                wishlistProductIds = wishlistItems?.Select(w => w.ProductId).ToList() ?? new();
            }

            var model = new HomeViewModel
            {
                Products = products,
                WishlistProductIds = wishlistProductIds
            };

            return PartialView("_ProductPartialView", model);
        }



        public IActionResult Test()
        {
            Response.Cookies.Append("Test", "Pb304-New",new CookieOptions { Expires = DateTimeOffset.Now.AddHours(1)});

            HttpContext.Session.SetString("Test-Session", "Pb304-Session");

            return NoContent();
        }

        public IActionResult Get()
        {
            var test = Request.Cookies["Test"];
            var testSession = HttpContext.Session.GetString("Test-Session");
            return Json(new { test, testSession });
        }
    }
}
