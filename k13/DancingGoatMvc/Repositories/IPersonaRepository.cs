using System.Collections.Generic;

using CMS.Personas;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Provides operations for personas.
    /// </summary>
    public interface IPersonaRepository : IRepository
    {
        /// <summary>
        /// Returns an enumerable collection of all personas.
        /// </summary>
        IEnumerable<PersonaInfo> GetAll();
    }
}
