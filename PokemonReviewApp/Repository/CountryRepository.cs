using AutoMapper;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public CountryRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public bool CountryExists(int countryId)
        {
            return context.Countries.Any(e => e.Id == countryId);
        }


        public ICollection<Country> GetCountries()
        {
            return context.Countries.ToList();
        }

        public Country GetCountry(int countryId)
        {
            return context.Countries.Where(c => c.Id == countryId).FirstOrDefault();
        }

        public Country GetCountryByOwner(int ownerId)
        {
            return context.Owners.Where(o => o.Id == ownerId).Select(c => c.Country).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnersFromACountry(int countryId)
        {
            return context.Owners.Where(c => c.Country.Id == countryId).ToList();
        }
        public bool CreateCountry(Country country)
        {
            context.Add(country);
            return Save();
        }
        public bool UpdateCountry(Country country)
        {
            context.Update(country);
            return Save();
        }
        public bool DeleteCountry(Country country)
        {
            context.Remove(country);
            return Save();
        }

        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
