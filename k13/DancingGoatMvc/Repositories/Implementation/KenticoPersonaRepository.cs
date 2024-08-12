using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Personas;

using DancingGoat.Infrastructure;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Provides operations for personas.
    /// </summary>
    public class KenticoPersonaRepository : IPersonaRepository
    {
        private readonly IPersonaInfoProvider personaInfoProvider;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoPersonaRepository"/> class.
        /// </summary>
        /// <param name="personaInfoProvider">Provider for <see cref="PersonaInfo"/> management.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="personaInfoProvider"/> is null.</exception>
        public KenticoPersonaRepository(IPersonaInfoProvider personaInfoProvider)
        {
            this.personaInfoProvider = personaInfoProvider ?? throw new ArgumentNullException(nameof(personaInfoProvider));
        }


        /// <summary>
        /// Returns an enumerable collection of all personas.
        /// </summary>
        public IEnumerable<PersonaInfo> GetAll()
        {
            return RepositoryCacheHelper.CacheObjects(() =>
            {
                return personaInfoProvider.Get();
            }, $"{nameof(KenticoPersonaRepository)}|{nameof(GetAll)}");
        }
    }
}