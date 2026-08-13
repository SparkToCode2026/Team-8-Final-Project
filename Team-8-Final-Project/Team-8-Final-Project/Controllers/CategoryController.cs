using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;
namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("Category")]
    public class CategoryController : ControllerBase
    {
        private ProjectContext context;
        public CategoryController(ProjectContext _context)
        {
            context = _context;
        }

        // Add a new category
        [HttpPost("AddCategory")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult AddCategory(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();

            return Ok(category);
        }

        // Update an existing category (full update)
        [HttpPut("UpdateCategory")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult UpdateCategory(int id, Category category)
        {
            Category existingCategory = context.Categories.FirstOrDefault(c => c.CategoryId == id);

            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.CategoryDescription = category.CategoryDescription;

            context.SaveChanges();

            return Ok(existingCategory);
        }

        // Update only the description of a category
        [HttpPatch("UpdateCategoryDescription")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult UpdateCategoryDescription(int id, string categoryDescription)
        {
            Category existingCategory = context.Categories.FirstOrDefault(c => c.CategoryId == id);

            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }

            existingCategory.CategoryDescription = categoryDescription;

            context.SaveChanges();

            return Ok(existingCategory);
        }

        // Delete a category
        [HttpDelete("DeleteCategory")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult DeleteCategory(int id)
        {
            Category existingCategory = context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }
            context.Categories.Remove(existingCategory);
            context.SaveChanges();

            return Ok("Category with ID " + id + " has been deleted.");
        }

        // Get all categories, including the books in each one
        [HttpGet("GetAllCategories")]
        [Authorize]
        public IActionResult GetAllCategories()
        {
            List<Category> categories = context.Categories.Include(c => c.Books).ToList();

            return Ok(categories);
        }

        // Get a category by its Id
        [HttpGet("GetCategoryById")]
        [Authorize]
        public IActionResult GetCategoryById(int id)
        {
            Category category = context.Categories.FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            return Ok(category);
        }

        // Filter categories by name (partial match search)
        [HttpGet("FilterCategoriesByName")]
        [Authorize]
        public IActionResult FilterCategoriesByName(string name)
        {
            List<Category> categories = context.Categories.Where(c => c.CategoryName.Contains(name)).ToList();

            if (categories.Count == 0)
            {
                return NotFound("No categories found.");
            }

            return Ok(categories);
        }

        // Count how many books are in each category
        [HttpGet("GetBookCountByCategory")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult GetBookCountByCategory()
        {
            var bookCounts = context.Categories.Select(c => new { CategoryName = c.CategoryName, TotalBooks = c.Books.Count() }).ToList();

            return Ok(bookCounts);
        }
    }
}
