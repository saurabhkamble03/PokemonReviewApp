using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext context;

        public ReviewerRepository(DataContext context)
        {
            this.context = context;
        }


        public Reviewer GetReviewer(int reviewerId)
        {
            return context.Reviewers.Where(r => r.Id == reviewerId).FirstOrDefault();
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return context.Reviewers.ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return context.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
        }

        public bool ReviewerExists(int reviewerId)
        {
            return context.Reviewers.Any(r => r.Id == reviewerId);
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            context.Add(reviewer);
            return Save();
        }
        public bool UpdateReviewer(Reviewer reviewer)
        {
            context.Update(reviewer);
            return Save();
        }
        public bool DeleteReviewer(Reviewer reviewer)
        {
            context.Remove(reviewer);
            return Save();
        }
        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
