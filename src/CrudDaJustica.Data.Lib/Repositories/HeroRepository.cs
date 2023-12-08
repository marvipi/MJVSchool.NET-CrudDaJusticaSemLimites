using CrudDaJustica.Data.Lib.Models;
using CrudDaJustica.Data.Lib.Services;

namespace CrudDaJustica.Data.Lib.Repositories;

/// <summary>
/// Represents a repository that stores information about heroes.
/// </summary>
public abstract class HeroRepository
{
    protected readonly PagingService pagingService;

    /// <summary>
    /// The page of data to retrieve.
    /// </summary>
    public int CurrentPage => pagingService.CurrentPage;

    /// <summary>
    /// The amount of data retrieved per <see cref="GetHeroes(int, int)"/>.
    /// </summary>
    public int RowsPerPage => pagingService.RowsPerPage;

    /// <summary>
    /// A sequence of all pages in a hero repository.
    /// </summary>
    public IEnumerable<int> PageRange => pagingService.PageRange;

    protected HeroRepository(PagingService pagingService)
    {
        this.pagingService = pagingService;
    }

    /// <summary>
    /// Registers a new hero in this repository.
    /// </summary>
    /// <param name="newHero"> The hero to register. </param>
    public abstract bool RegisterHero(HeroEntity newHero);

    /// <summary>
    /// Retrieves information about all heroes registered in a given page.
    /// </summary>
    /// <param name="page"> The page where information will be retrieved. </param>
    /// <param name="rows"> The amount of data to get. </param>
    /// <returns> A collection of all heroes registered in the page. </returns>
    public abstract IEnumerable<HeroEntity> GetHeroes(int page, int rows);

    /// <summary>
    /// Retrieves information about a hero.
    /// </summary>
    /// <param name="id"> The id of the hero to get. </param>
    /// <returns> Information about a hero whose id matches the given one, or null, if no matching ids are found. </returns>
    public abstract HeroEntity? GetHero(Guid id);

    /// <summary>
    /// Updates the information about a registered hero.
    /// </summary>
    /// <param name="id"> The id of the hero to update. </param>
    /// <param name="updatedHero"> A <see cref="HeroEntity"/> containing update to date information about the hero. </param>
    public abstract bool UpdateHero(Guid id, HeroEntity updatedHero);

    /// <summary>
    /// Deletes a hero from the repository.
    /// </summary>
    /// <param name="id"> The id of the hero to delete. </param>
    public abstract bool DeleteHero(Guid id);
}
