using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using uniqilo.DataAcces;
using uniqilo.ViewModel.Common;
using uniqilo.ViewModel.Product;
using uniqilo.ViewModel.Slider;

namespace uniqilo.Controllers
{
    public class HomeController(AppDbContext _context) : Controller
    {
         
 
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new HomeVM();

            vm.Sliders = await _context.Sliders
                .Where(x => !x.IsDeleted)
                .Select(x => new SliderItemVM
                {
                    ImageUrl = x.ImageUrl,
                    Link = x.Link,
                    Title = x.Title,
                    Subtitle = x.Description
                }).ToListAsync();

            vm.Products = await _context.Products
                .Where(x => !x.IsDeleted)
                .Select(x => new ProductItemVM
                {
                    Discount = x.Discount,
                    Id = x.Id,
                    ImageUrl = x.CoverImage,
                    IsInStock = x.Quantity > 0,
                    Name = x.ProductName,
                    Price = x.SellPrice
                }).ToListAsync();

            return View(vm);
        }

    }
}
