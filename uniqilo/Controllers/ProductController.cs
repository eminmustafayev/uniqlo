using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using uniqilo.DataAcces;
using uniqilo.Models;
using uniqilo.ViewModel.Comment;

namespace uniqilo.Controllers
{
    public class ProductController(AppDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return BadRequest();
            var data = await _context.Products
                .Where(x => x.Id == id)
                .Include(x => x.Images)
                .Include(x=>x.Comments)
                .ThenInclude(c=>c.User)
                .FirstOrDefaultAsync();
            if(data is null) return NotFound();
            return View(data);
        }
        public async Task<IActionResult> Comment(int productId, CommentVM vm)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

            var data = await _context.Comments.Where(x => x.UserId == userId && x.ProductId == productId).FirstOrDefaultAsync();


            Comment comment = new Comment
            {
                Content = vm.Content,
                ProductId = productId,
                UserId = userId,
            };

            await _context.Comments.AddAsync(comment);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { Id = productId });
        }
    }
}

