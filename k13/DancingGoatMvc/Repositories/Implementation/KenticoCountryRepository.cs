using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Globalization;

using DancingGoat.Infrastructure;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of countries and states.
    /// </summary>
    public class KenticoCountryRepository : ICountryRepository
    {
        private readonly ICountryInfoProvider countryInfoProvider;
        private readonly IStateInfoProvider stateInfoProvider;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoCountryRepository"/> class.
        /// </summary>
        /// <param name="countryInfoProvider">Provider for <see cref="CountryInfo"/> management.</param>
        /// <param name="stateInfoProvider">Provider for <see cref="StateInfo"/> management.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="countryInfoProvider"/> or <paramref name="stateInfoProvider"/> is null.</exception>
        public KenticoCountryRepository(ICountryInfoProvider countryInfoProvider, IStateInfoProvider stateInfoProvider)
        {
            this.countryInfoProvider = countryInfoProvider ?? throw new ArgumentNullException(nameof(countryInfoProvider));
            this.stateInfoProvider = stateInfoProvider ?? throw new ArgumentNullException(nameof(stateInfoProvider));
        }


        /// <summary>
        /// Returns all available countries.
        /// </summary>
        /// <returns>Collection of all available countries</returns>
        public IEnumerable<CountryInfo> GetAllCountries()
        {
            return RepositoryCacheHelper.CacheObjects(() =>
            {
                return countryInfoProvider.Get();
            }, $"{nameof(KenticoCountryRepository)}|{nameof(GetAllCountries)}");
        }


        /// <summary>
        /// Returns the country with the specified ID.
        /// </summary>
        /// <param name="countryId">The identifier of the country.</param>
        /// <returns>The country with the specified ID, if found; otherwise, null.</returns>
        public CountryInfo GetCountry(int countryId)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                return countryInfoProvider.Get(countryId);
            }, $"{nameof(KenticoCountryRepository)}|{nameof(GetCountry)}|{countryId}");
        }


        /// <summary>
        /// Returns the country with the specified code name.
        /// </summary>
        /// <param name="countryName">The code name of the country.</param>
        /// <returns>The country with the specified code name, if found; otherwise, null.</returns>
        public CountryInfo GetCountry(string countryName)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                return countryInfoProvider.Get(countryName);
            }, $"{nameof(KenticoCountryRepository)}|{nameof(GetCountry)}|{countryName}");
        }

        
        /// <summary>
        /// Returns all states in country with given ID.
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Collection of all states in county.</returns>
        public IEnumerable<StateInfo> GetCountryStates(int countryId)
        {
            return RepositoryCacheHelper.CacheObjects(() =>
            {
                return stateInfoProvider.Get().WhereEquals("CountryID", countryId);
            }, $"{nameof(KenticoCountryRepository)}|{nameof(GetCountryStates)}|{countryId}");
        }


        /// <summary>
        /// Returns the state with the specified code name.
        /// </summary>
        /// <param name="stateName">The code name of the state.</param>
        /// <returns>The state with the specified code name, if found; otherwise, null.</returns>
        public StateInfo GetState(string stateName)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                return stateInfoProvider.Get(stateName);
            }, $"{nameof(KenticoCountryRepository)}|{nameof(GetState)}|{stateName}");
        }


        /// <summary>
        /// Returns the state with the specified ID.
        /// </summary>
        /// <param name="stateId">The identifier of the state.</param>
        /// <returns>The state with the specified ID, if found; otherwise, null.</returns>
        public StateInfo GetState(int stateId)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                return stateInfoProvider.Get(stateId);
            }, $"{nameof(KenticoCountryRepository)}|{nameof(GetState)}|{stateId}");
        }
    }
}