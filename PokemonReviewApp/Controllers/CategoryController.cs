using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var categories = mapper.Map<List<CategoryDto>>(categoryRepository.GetCategories());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if(!categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var category = mapper.Map<CategoryDto>(categoryRepository.GetCategory(categoryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(category);
        }

        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type =typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByCategory(int categoryId)
        {
            if(!categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var pokemons = mapper.Map<List<PokemonDto>>(categoryRepository.GetPokemonsByCategory(categoryId));

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemons);
        }

        [HttpPost("/createCategory")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto createCategory)
        {
            if(createCategory == null)
            {
                return BadRequest(ModelState);
            }

            var category = categoryRepository.GetCategories()
                           .Where(c => c.Name.Trim().ToUpper() == createCategory.Name.TrimEnd().ToUpper())
                           .FirstOrDefault();

            if(category != null)
            {
                ModelState.AddModelError("", "Category Already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryMap = mapper.Map<Category>(createCategory);

            if (!categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500,ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updatedCategory)
        {
            if(updatedCategory == null)
            {
                return BadRequest(ModelState);
            }

            if(categoryId != updatedCategory.Id)
            {
                return BadRequest(ModelState);
            }

            if(!categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryMap = mapper.Map<Category>(updatedCategory);

            if(!categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something Went Wrong !");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Updated");
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if(!categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var categoryToDelete = categoryRepository.GetCategory(categoryId);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Something Went Wrong !");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Deleted !");
        }
    }
}
