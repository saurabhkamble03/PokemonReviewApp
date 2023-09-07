using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext context;

        public CategoryRepository(DataContext context)
        {
            this.context = context;
        }

        public bool CategoryExists(int id)
        {
            return context.Categories.Any(c => c.Id == id);
        }


        public ICollection<Category> GetCategories()
        {
            return context.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return context.Categories.Where(c => c.Id == id).FirstOrDefault();
        }

        public ICollection<Pokemon> GetPokemonsByCategory(int categoryId)
        {
            return context.PokemonCategories.Where(pc => pc.CategoryId == categoryId).Select(c => c.Pokemon).ToList();
        }
        public bool CreateCategory(Category category)
        {
            context.Add(category);
            return Save();
        }
        public bool UpdateCategory(Category category)
        {
            context.Update(category);
            return Save();
        }
        public bool DeleteCategory(Category category)
        {
            context.Remove(category);
            return Save();
        }

        public bool Save()
        {
            var saved = context.SaveChanges();

            return saved > 0 ? true : false;
        }

    }
}
