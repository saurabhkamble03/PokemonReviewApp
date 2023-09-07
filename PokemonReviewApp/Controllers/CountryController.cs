using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository countryRepository;
        private readonly IMapper mapper;

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            this.countryRepository = countryRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            var countries = mapper.Map<List<CountryDto>>(countryRepository.GetCountries());

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {
            if(!countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }

            var country = mapper.Map<CountryDto>(countryRepository.GetCountry(countryId));

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(country);
        }

        [HttpGet("/country/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountryByOwner(int ownerId)
        {
            if(!countryRepository.CountryExists(ownerId))
            {
                return NotFound();
            }

            var country = mapper.Map<CountryDto>(countryRepository.GetCountryByOwner(ownerId));

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(country);
        }

        [HttpPost("/createCountry")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDto createCountry)
        {
            if(createCountry == null)
            {
                return BadRequest(ModelState);
            }

            var country = countryRepository.GetCountries()
                          .Where(c => c.Name.Trim().ToUpper() == createCountry.Name.TrimEnd().ToUpper())
                          .FirstOrDefault();

            if(country != null)
            {
                ModelState.AddModelError("","Country Already Exists");
                return StatusCode(422, ModelState);
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countryMap = mapper.Map<Country>(createCountry);

            if(!countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something Went Wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Created");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto updatedCountry)
        {
            if(updatedCountry == null)
            {
                return BadRequest(ModelState);
            }

            if(countryId != updatedCountry.Id)
            {
                return BadRequest(ModelState);
            }

            if(!countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countryMap = mapper.Map<Country>(updatedCountry);

            if(!countryRepository.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", "Something Went Wrong !");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Updated !");
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCountry(int countryId)
        {
            if(!countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }

            var countryToBeDeleted = countryRepository.GetCountry(countryId);

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            if(!countryRepository.DeleteCountry(countryToBeDeleted))
            {
                ModelState.AddModelError("", "Something Went Wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Deleted");

        }

    }
}
