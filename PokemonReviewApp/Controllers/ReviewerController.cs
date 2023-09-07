using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository reviewerRepository;
        private readonly IMapper mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            this.reviewerRepository = reviewerRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        public IActionResult GetReviewers()
        {
            var reviewers = mapper.Map<List<ReviewerDto>>(reviewerRepository.GetReviewers());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviewers);
        }


        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if(!reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }

            var reviewer = mapper.Map<ReviewerDto>(reviewerRepository.GetReviewer(reviewerId));

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviewer);
        }

        [HttpGet("/reviews/{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if(!reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }

            var reviews = mapper.Map<ReviewDto>(reviewerRepository.GetReviewsByReviewer(reviewerId));

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviews);
        }
        [HttpPost("/createReviewer/")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto createReviewer)
        {
            if(createReviewer == null)
            {
                return BadRequest(ModelState);
            }

            var reviewer = reviewerRepository.GetReviewers()
                           .Where(r => r.LastName.Trim().ToUpper() == createReviewer.LastName.TrimEnd().ToUpper())
                           .FirstOrDefault();

            if(reviewer != null)
            {
                ModelState.AddModelError("", "Reviewer Already Exists !");
                return StatusCode(422,ModelState);
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewerMap = mapper.Map<Reviewer>(createReviewer);

            if(!reviewerRepository.CreateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Something Went Wrong !");
                return StatusCode(500,ModelState);
            }

            return Ok("Successfully Created");

        }

        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updatedReviewer)
        {
            if(updatedReviewer == null)
            {
                return BadRequest(ModelState);
            }

            if(reviewerId != updatedReviewer.Id)
            {
                return BadRequest();
            }

            if(!reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewerMap = mapper.Map<Reviewer>(updatedReviewer);

            if(!reviewerRepository.UpdateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Something Went Wrong");
                return StatusCode(500, ModelState);
            }

            return Ok("Updated Successfully");

        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if(!reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }

            var reviewerToBeDeleted = reviewerRepository.GetReviewer(reviewerId);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!reviewerRepository.DeleteReviewer(reviewerToBeDeleted))
            {
                ModelState.AddModelError("", "Something Went Wrong !");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully Deleted");
        }
    }
}
