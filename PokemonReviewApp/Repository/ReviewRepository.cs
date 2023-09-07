﻿using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext context;

        public ReviewRepository(DataContext context)
        {
            this.context = context;
        }


        public Review GetReview(int reviewId)
        {
            return context.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return context.Reviews.ToList();
        }

        public ICollection<Review> GetReviewsOfAPokemon(int pokeId)
        {
            return context.Reviews.Where(r => r.Pokemon.Id == pokeId).ToList();
        }

        public bool ReviewExists(int reviewId)
        {
            return context.Reviews.Any(r => r.Id == reviewId);
        }
        public bool CreateReview(Review review)
        {
            context.Add(review);
            return Save();
        }
        public bool UpdateReview(Review review)
        {
            context.Update(review);
            return Save();
        }
        public bool DeleteReviews(List<Review> reviews)
        {
            context.RemoveRange(reviews);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            context.Remove(review);
            return Save();
        }

        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
