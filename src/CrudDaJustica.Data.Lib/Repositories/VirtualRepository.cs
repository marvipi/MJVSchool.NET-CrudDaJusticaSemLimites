using CrudDaJustica.Data.Lib.Models;
using CrudDaJustica.Data.Lib.Services;

namespace CrudDaJustica.Data.Lib.Repositories;

/// <summary>
/// Represents a repository that stores data in memory.
/// </summary>
/// <remarks>
/// All data will be lost when the system is shutdown.
/// </remarks>
public class VirtualRepository : IHeroRepository
{
    public int CurrentPage { get => pagingService.CurrentPage; }
    public int RowsPerPage { get => pagingService.RowsPerPage; }
    public IEnumerable<int> PageRange { get => pagingService.PageRange; }


    private readonly PagingService pagingService;
    private int RepositorySize => LastFilledIndex(heroes) + 1;


    // Summary: All heroes registered in this repository.
    private HeroEntity[] heroes;

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualRepository"/> class.
    /// </summary>
    /// <param name="pagingService"> The service responsible for paging data repositories. </param>
    /// <param name="initialSize"> The amount space to reserve for registering new heroes. </param>
    public VirtualRepository(PagingService pagingService, uint initialSize)
    {
        heroes = new HeroEntity[initialSize];
        this.pagingService = pagingService;
    }

    public bool RegisterHero(HeroEntity newHero)
    {
        if (RepositorySize == heroes.Length)
        {
            Array.Resize(ref heroes, heroes.Length * 2);
        }
        var firstEmptyIndex = LastFilledIndex(heroes) + 1;
        heroes[firstEmptyIndex] = newHero;
        return true;
    }

    public IEnumerable<HeroEntity> GetHeroes(int page, int rows)
    {
        (var validPage, var validRows) = pagingService.Validate(page, rows, RepositorySize);

        var skip = (validPage - 1) * validRows;
        var take = validPage * validRows;
        var heroesPage = heroes[skip..take];

        var amountNonNull = LastFilledIndex(heroesPage) + 1;
        var nonNullHeroes = heroesPage[..amountNonNull];
        return nonNullHeroes;
    }

    public HeroEntity? GetHero(Guid id)
    {
        foreach (var hero in heroes)
        {
            if (hero.Id == id)
            {
                return hero;
            }
        }
        return null;
    }

    public bool UpdateHero(Guid id, HeroEntity updatedHero)
    {
        var index = 0;

        foreach (var hero in heroes[..])
        {
            if (hero.Id == id)
            {
                heroes[index] = new HeroEntity()
                {
                    Id = id,
                    Alias = updatedHero.Alias,
                    Debut = updatedHero.Debut,
                    FirstName = updatedHero.FirstName,
                    LastName = updatedHero.LastName,
                };
                return true;
            }
            index++;
        }

        return false;
    }

    public bool DeleteHero(Guid id)
    {
        var indexToDelete = -1;

        foreach (var i in Enumerable.Range(0, RepositorySize))
        {
            if (heroes[i].Id == id)
            {
                indexToDelete = i;
                break;
            }

        }

        if (indexToDelete < 0)
        {
            return false;
        }

        foreach (var i in Enumerable.Range(indexToDelete, heroes.Length - 2))
        {
            heroes[i] = heroes[i + 1];
        }

        return true;
    }

    // Summary: Produces the last filled index in an array of T.
    private static int LastFilledIndex<T>(T[] array)
    {
        var firstEmptyIndex = Array.IndexOf(array, null);
        return int.IsNegative(firstEmptyIndex)
            ? array.Length - 1
            : firstEmptyIndex - 1;
    }

}
