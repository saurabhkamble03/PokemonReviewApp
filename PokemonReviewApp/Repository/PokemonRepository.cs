using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext context;
        
        public PokemonRepository(DataContext context)
        {
            this.context = context;
        }


        public Pokemon GetPokemon(int id)
        {
            return context.Pokemon.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon GetPokemon(string name)
        {
            return context.Pokemon.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int pokeId)
        {
            var review = context.Reviews.Where(p => p.Pokemon.Id == pokeId);

            if(review.Count() <= 0)
            {
                return 0;
            }

            return ((decimal)review.Sum(r => r.Rating) / review.Count());
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return context.Pokemon.OrderBy(p => p.Id).ToList();
        }

        public bool PokemonExists(int pokeId)
        {
            return context.Pokemon.Any(p => p.Id == pokeId);
        }

        public bool PokemonExists(string name)
        {
            return context.Pokemon.Any(p => p.Name == name);
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = context.Owners.Where(a => a.Id == ownerId).FirstOrDefault();
            var category = context.Categories.Where(a => a.Id == categoryId).FirstOrDefault();

            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon
            };

            context.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon
            };

            context.Add(pokemonCategory);

            context.Add(pokemon);

            return Save();

        }
        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            context.Update(pokemon);
            return Save();
        }
        public bool DeletePokemon(Pokemon pokemon)
        {
            context.Remove(pokemon);
            return Save();
        }

        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
