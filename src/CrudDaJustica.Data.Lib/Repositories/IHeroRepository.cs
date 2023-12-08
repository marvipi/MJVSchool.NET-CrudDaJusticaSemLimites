using CrudDaJustica.Data.Lib.Models;

namespace CrudDaJustica.Data.Lib.Repositories;

/// <summary>
/// Represents a repository that stores information about heroes.
/// </summary>
public interface IHeroRepository
{
    /// <summary>
    /// The page of data to retrieve.
    /// </summary>
    public int CurrentPage { get; }

    /// <summary>
    /// The amount of data retrieved per <see cref="GetHeroes(int, int)"/>.
    /// </summary>
    public int RowsPerPage { get; }

    /// <summary>
    /// A sequence of all pages in a hero repository.
    /// </summary>
    public IEnumerable<int> PageRange { get; }

    /// <summary>
    /// Registers a new hero in this repository.
    /// </summary>
    /// <param name="newHero"> The hero to register. </param>
    public bool RegisterHero(HeroEntity newHero);

    /// <summary>
    /// Retrieves information about all heroes registered in a given page.
    /// </summary>
    /// <param name="page"> The page where information will be retrieved. </param>
    /// <param name="rows"> The amount of data to get. </param>
    /// <returns> A collection of all heroes registered in the page. </returns>
    public IEnumerable<HeroEntity> GetHeroes(int page, int rows);

    /// <summary>
    /// Retrieves information about a hero.
    /// </summary>
    /// <param name="id"> The id of the hero to get. </param>
    /// <returns> Information about a hero whose id matches the given one, or null, if no matching ids are found. </returns>
    public HeroEntity? GetHero(Guid id);

    /// <summary>
    /// Updates the information about a registered hero.
    /// </summary>
    /// <param name="id"> The id of the hero to update. </param>
    /// <param name="updatedHero"> A <see cref="HeroEntity"/> containing update to date information about the hero. </param>
    public bool UpdateHero(Guid id, HeroEntity updatedHero);

    /// <summary>
    /// Deletes a hero from the repository.
    /// </summary>
    /// <param name="id"> The id of the hero to delete. </param>
    public bool DeleteHero(Guid id);
}
