using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository reviewRepository;
        private readonly IReviewerRepository reviewerRepository;
        private readonly IPokemonRepository pokemonRepository;
        private readonly IMapper mapper;

        public ReviewController(IReviewRepository reviewRepository
            ,IReviewerRepository reviewerRepository
            ,IPokemonRepository pokemonRepository
            ,IMapper mapper)
        {
            this.reviewRepository = reviewRepository;
            this.reviewerRepository = reviewerRepository;
            this.pokemonRepository = pokemonRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var reviews = mapper.Map<List<ReviewDto>>(reviewRepository.GetReviews());

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if(!reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var review = mapper.Map<ReviewDto>(reviewRepository.GetReview(reviewId));

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(review);
        }

        [HttpGet("reviews/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsOfAPokemon(int pokeId)
        {
            var reviews = mapper.Map<List<ReviewDto>>(reviewRepository.GetReviewsOfAPokemon(pokeId));

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviews);
        }

        [HttpPost("/createReview/")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId ,[FromBody] ReviewDto createReview)
        {
            if(createReview == null)
            {
                return BadRequest(ModelState);
            }

            var review = reviewRepository.GetReviews()
                         .Where(r => r.Title.Trim().ToUpper() == createReview.Title.TrimEnd().ToUpper())
                         .FirstOrDefault();

            if(review != null)
            {
                ModelState.AddModelError("", "Review already exists");
                return StatusCode(422,ModelState);
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewMap = mapper.Map<Review>(createReview);

            reviewMap.Pokemon = pokemonRepository.GetPokemon(pokeId);

            reviewMap.Reviewer = reviewerRepository.GetReviewer(reviewerId);

            if(!reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something Went Wrong!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Created");

        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updatedReview)
        {
            if(updatedReview == null)
            {
                return BadRequest(ModelState);
            }

            if(reviewId != updatedReview.Id)
            {
                return BadRequest(ModelState);
            }

            if(!reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewMap = mapper.Map<Review>(updatedReview);

            if (!reviewRepository.UpdateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something Went Wrong !");
                return StatusCode(500,ModelState);
            }

            return Ok("Successfully Updated !");
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewId)
        {
            if(!reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var reviewToBeDeleted = reviewRepository.GetReview(reviewId);

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            if(!reviewRepository.DeleteReview(reviewToBeDeleted))
            {
                ModelState.AddModelError("", "Something Went Wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Deleted");
        }

    }
}
