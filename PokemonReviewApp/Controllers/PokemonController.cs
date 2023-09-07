using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository pokemonRepository;
        private readonly IReviewRepository reviewRepository;
        private readonly IMapper mapper;

        public PokemonController(IPokemonRepository pokemonRepository
            , IReviewRepository reviewRepository
            ,IMapper mapper)
        {
            this.pokemonRepository = pokemonRepository;
            this.reviewRepository = reviewRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200,Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = mapper.Map<List<PokemonDto>>(pokemonRepository.GetPokemons());
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId) 
        {
            if (!pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            var pokemon = mapper.Map<PokemonDto>(pokemonRepository.GetPokemon(pokeId));

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(pokemon);
        }

        //[HttpGet("{name}")]
        //[ProducesResponseType(200, Type = typeof(Pokemon))]
        //[ProducesResponseType(400)]
        //public IActionResult GetPokemon(string name)
        //{
        //    if (!pokemonRepository.PokemonExists(name))
        //    {
        //        return NotFound();
        //    }

        //    var pokemon = pokemonRepository.GetPokemon(name);

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }

        //    return Ok(pokemon);
        //}

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if (!pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            var rating = pokemonRepository.GetPokemonRating(pokeId);

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(rating);
        }

        [HttpPost("/createPokemon")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto createPokemon)
        {
            if(createPokemon == null)
            {
                return BadRequest(ModelState);
            }

            var pokemons = pokemonRepository.GetPokemons()
                           .Where(c => c.Name.Trim().ToUpper() == createPokemon.Name.TrimEnd().ToUpper())
                           .FirstOrDefault();

            if(pokemons != null)
            {
                ModelState.AddModelError("", "Pokemon already exists");
                return StatusCode(422,ModelState);
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pokemonMap = mapper.Map<Pokemon>(createPokemon);

            if(!pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap))
            {
                ModelState.AddModelError("", "Something Went Wrong!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Created!");

        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokeId,[FromQuery] int ownerId, [FromQuery] int categoryId ,[FromBody] PokemonDto updatedPokemon)
        {
            if(updatedPokemon == null)
            {
                return BadRequest(ModelState);
            }

            if(pokeId != updatedPokemon.Id)
            {
                return BadRequest(ModelState);
            }

            if(!pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var pokemonMap = mapper.Map<Pokemon>(updatedPokemon);

            if(!pokemonRepository.UpdatePokemon(ownerId,categoryId,pokemonMap))
            {
                ModelState.AddModelError("","Something Went Wrong !");
                return StatusCode(500,ModelState);
            }

            return Ok("Successfully Updated !");
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult DeletePokemon(int pokeId)
        {
            if(!pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            var reviewsToBeDeleted = reviewRepository.GetReviewsOfAPokemon(pokeId);

            var pokemonToBeDeleted = pokemonRepository.GetPokemon(pokeId);

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            if(!reviewRepository.DeleteReviews(reviewsToBeDeleted.ToList()))
            {
                ModelState.AddModelError("", "Reviews Could Not Be Deleted");
                return StatusCode(500, ModelState);
            }

            if(!pokemonRepository.DeletePokemon(pokemonToBeDeleted))
            {
                ModelState.AddModelError("", "Something Went Wrong !");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Deleted");
        }

    }
}
