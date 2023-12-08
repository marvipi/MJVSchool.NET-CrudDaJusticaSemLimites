using CrudDaJustica.Data.Lib.Models;
using CrudDaJustica.Data.Lib.Services;

namespace CrudDaJustica.Data.Lib.Repositories;

/// <summary>
/// Represents a repository that stores data in memory.
/// </summary>
/// <remarks>
/// All data will be lost when the system is shutdown.
/// </remarks>
public class VirtualHeroRepository : HeroRepository
{
    private int Size => LastFilledIndex(heroes) + 1;

    // Summary: All heroes registered in this repository.
    private HeroEntity[] heroes;

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualHeroRepository"/> class.
    /// </summary>
    /// <param name="pagingService"> The service responsible for paging data repositories. </param>
    /// <param name="initialSize"> The amount space to reserve for registering new heroes. </param>
    public VirtualHeroRepository(PagingService pagingService, uint initialSize) : base(pagingService)
    {
        heroes = new HeroEntity[initialSize];
    }

    public override bool Register(HeroEntity newHero)
    {
        if (Size == heroes.Length)
        {
            Array.Resize(ref heroes, heroes.Length * 2);
        }
        var firstEmptyIndex = LastFilledIndex(heroes) + 1;
        heroes[firstEmptyIndex] = newHero;
        return true;
    }

    public override IEnumerable<HeroEntity> Get(int page, int rows)
    {
        (var validPage, var validRows) = pagingService.Validate(page, rows, Size);

        var skip = (validPage - 1) * validRows;
        var take = validPage * validRows;
        var heroesPage = heroes[skip..take];

        var amountNonNull = LastFilledIndex(heroesPage) + 1;
        var nonNullHeroes = heroesPage[..amountNonNull];
        return nonNullHeroes;
    }

    public override HeroEntity? Get(Guid id)
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

    public override bool Update(Guid id, HeroEntity updatedHero)
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

    public override bool Delete(Guid id)
    {
        var indexToDelete = -1;

        foreach (var i in Enumerable.Range(0, Size))
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
