using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pb304PetShop.DataContext;
using Pb304PetShop.DataContext.Entities;
using Pb304PetShop.Models;

namespace Pb304PetShop.Controllers
{
    public class WishlistController : Controller
    {
        private readonly AppDbContext _dbContext;
        private const string WishlistCookieKey = "Wishlist";

        public WishlistController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RemoveFromWishlist(int id)
        {
            var product = _dbContext.Products.Find(id);

            if (product == null)
            {
                return BadRequest();
            }

            var wishlist = GetWishlist();
            var wishlistItem = wishlist.Find(x => x.ProductId == id);
            if (wishlistItem != null)
            {
                wishlist.Remove(wishlistItem);
            }
            var wishlistJson = JsonConvert.SerializeObject(wishlist);
            Response.Cookies.Append(WishlistCookieKey, wishlistJson, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddHours(1),
                Path = "/",
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });

            return Json(new { success = true, message = "Added to wishlist" });

        }
        [HttpPost]
        public IActionResult AddToWishlist(int id)
        {
            var product = _dbContext.Products.Find(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            var wishlist = GetWishlist();
            var existingItem = wishlist.FirstOrDefault(x => x.ProductId == id);
            if (existingItem == null)
            {
                wishlist.Add(new WishlistItemCookieModel { ProductId = id, Quantity = product.Quantity });
            }
            else
            {
                existingItem.Quantity = product.Quantity;
            }

            var wishlistJson = JsonConvert.SerializeObject(wishlist);
            Response.Cookies.Append(WishlistCookieKey, wishlistJson, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddHours(1),
                Path = "/",
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });

            return Json(new { success = true, message = "Added to wishlist" });
        }




        private List<WishlistItemCookieModel> GetWishlist()
        {
            var wishlist = Request.Cookies[WishlistCookieKey];

            if (string.IsNullOrEmpty(wishlist))
            {
                return new List<WishlistItemCookieModel>();
            }

            var wishlistItems = JsonConvert.DeserializeObject<List<WishlistItemCookieModel>>(wishlist);

            if (wishlistItems == null)
            {
                return new List<WishlistItemCookieModel>();
            }

            return wishlistItems;
        }

        public async Task<IActionResult> Wantlist()
        {
            

            var wishlistItems = GetWishlist();

            var wantlist = new WantlistViewModel();
            var wantlistItemList = new List<WantlistItemViewModel>();

            foreach (var item in wishlistItems ?? [])
            {
                var product = await _dbContext.Products.FindAsync(item.ProductId);

                if (product == null) continue;

                wantlistItemList.Add(new WantlistItemViewModel
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Description = product.Name,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    ImageUrl = product.CoverImageUrl
                });
            }

            wantlist.Items = wantlistItemList;
            wantlist.Quantity = wantlistItemList.Count;
            wantlist.Total = wantlistItemList.Sum(x => x.Price);
            wantlist.WishlistProductIds = wishlistItems.Select(x => x.ProductId).ToList();

            return View(wantlist);
        }


        [HttpPost]
        public IActionResult UpdateWishlist([FromBody] UpdateWishlistModel updatewishlistModel)
        {
            var wishlist = GetWishlist();
            var wishlistItem = wishlist.Find(x => x.ProductId == updatewishlistModel.ProductId);
            if (wishlistItem != null)
            {
                if (updatewishlistModel.Quantity <= 0)
                {
                    wishlist.Remove(wishlistItem);
                }
                else
                {
                    wishlistItem.Quantity = updatewishlistModel.Quantity;
                }
            }
            var wishlistJson = JsonConvert.SerializeObject(wishlist);
            Response.Cookies.Append(WishlistCookieKey, wishlistJson, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddHours(1)
            });

            var wantlist = new WantlistViewModel();
            var wantlistItemList = new List<WantlistItemViewModel>();

            foreach (var item in wishlist ?? [])
            {
                var product = _dbContext.Products.Find(item.ProductId);

                if (product == null) continue;

                wantlistItemList.Add(new WantlistItemViewModel
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Description = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    ImageUrl = product.CoverImageUrl
                });
            }

            wantlist.Items = wantlistItemList;
            wantlist.Quantity = wantlistItemList.Sum(x => x.Quantity);
            wantlist.Total = wantlistItemList.Sum(x => x.Quantity * x.Price);

            return PartialView("_WantlistPartialView", wantlist);
        }
    }

    public class UpdateWishlistModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    } 
}
