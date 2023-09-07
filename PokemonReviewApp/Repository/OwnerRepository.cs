using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DataContext context;

        public OwnerRepository(DataContext context)
        {
            this.context = context;
        }


        public Owner GetOwner(int ownerId)
        {
            return context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnerByPokemon(int pokemonId)
        {
            return context.PokemonOwners.Where(po => po.Pokemon.Id == pokemonId).Select(o => o.Owner).ToList();
        }

        public ICollection<Owner> GetOwners()
        {
            return context.Owners.ToList();
        }

        public ICollection<Pokemon> GetPokemonsByOwner(int ownerId)
        {
            return context.PokemonOwners.Where(po => po.Owner.Id == ownerId).Select(p => p.Pokemon).ToList();
        }

        public bool OwnerExists(int ownerId)
        {
            return context.Owners.Any(o => o.Id == ownerId);
        }
        public bool CreateOwner(Owner owner)
        {
            context.Add(owner);
            return Save();
        }
        public bool UpdateOwner(Owner owner)
        {
            context.Update(owner);
            return Save();
        }
        public bool DeleteOwner(Owner owner)
        {
            context.Remove(owner);
            return Save();
        }

        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
