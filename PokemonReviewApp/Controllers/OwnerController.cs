using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository ownerRepository;
        private readonly ICountryRepository countryRepository;
        private readonly IMapper mapper;

        public OwnerController(IOwnerRepository ownerRepository,ICountryRepository countryRepository ,IMapper mapper)
        {
            this.ownerRepository = ownerRepository;
            this.countryRepository = countryRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var owners = mapper.Map<List<OwnerDto>>(ownerRepository.GetOwners());

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int ownerId)
        {
            if(!ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var owner = mapper.Map<OwnerDto>(ownerRepository.GetOwner(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }    

            return Ok(owner);
        }

        [HttpGet("/pokemon/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var pokemons = mapper.Map<List<PokemonDto>>(ownerRepository.GetPokemonsByOwner(ownerId));

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemons);
        }

        [HttpPost("/createOwner")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromQuery] int countryId,[FromBody] OwnerDto createOwner)
        {
            if(createOwner == null)
            {
                return BadRequest(ModelState);
            }

            var owner = ownerRepository.GetOwners()
                        .Where(l => l.LastName.Trim().ToUpper() == createOwner.LastName.TrimEnd().ToUpper())
                        .FirstOrDefault();

            if(owner != null)
            {
                ModelState.AddModelError("","Owner Already Exists");
                return StatusCode(422,ModelState);
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ownerMap = mapper.Map<Owner>(createOwner);
            ownerMap.Country = countryRepository.GetCountry(countryId);

            if(!ownerRepository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something Went Wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Created");
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updatedOwner)
        {
            if (updatedOwner == null)
            {
                return BadRequest(ModelState);
            }

            if (ownerId != updatedOwner.Id)
            {
                return BadRequest(ModelState);
            }

            if (!ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var ownerMap = mapper.Map<Owner>(updatedOwner);

            if(!ownerRepository.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something Went Wrong !");
                return StatusCode(500,ModelState);
            }

            return Ok("Successfully Updated !");
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner(int ownerId)
        {
            if(!ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var ownerToBeDeleted = ownerRepository.GetOwner(ownerId);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!ownerRepository.DeleteOwner(ownerToBeDeleted))
            {
                ModelState.AddModelError("", "Something Went Wrong !");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Deleted");
        }
    }
}
