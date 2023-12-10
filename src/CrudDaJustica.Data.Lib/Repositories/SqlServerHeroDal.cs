using CrudDaJustica.Data.Lib.Models;

namespace CrudDaJustica.Data.Lib.Repositories;

/// <summary>
/// Represents the behavior shared by all SQL Server data access services.
/// </summary>
public abstract class SqlServerHeroDal
{
    protected const string COUNT =
        @"SELECT COUNT(*) as HeroRepositorySize
          FROM HeroInformation;";

    protected const string INSERT =
        @"EXECUTE InsertHero @Id = @id,
                             @Alias = @alias, 
                             @Debut = @debut, 
                             @FirstName = @firstName, 
                             @LastName = @lastName;";

    protected const string GET_PAGED =
        @"SELECT Id, Alias, Debut, FirstName, LastName
          FROM HeroInformation
          ORDER BY Alias
          OFFSET (@page - 1) * @rows ROWS
          FETCH NEXT @rows ROWS ONLY;";

    protected const string GET =
        @"SELECT Id, Alias, Debut, FirstName, LastName
          FROM HeroInformation
          WHERE Id = @id;";

    protected const string UPDATE =
        @"EXECUTE UpdateHero @Id = @id,
                             @Alias = @alias,
                             @Debut = @debut,
                             @FirstName = @firstName,
                             @LastName = @lastName;";

    protected const string DELETE = @"EXECUTE DeleteHero @Id = @id;";

    /// <summary>
    /// The amount of rows contained in the hero table of the database.
    /// </summary>
    public abstract int Size { get; }

    /// <summary>
    /// Registers a new hero in the database.
    /// </summary>
    /// <param name="newHero"> The hero to register. </param>
    public abstract bool Register(HeroEntity newHero);

    /// <summary>
    /// Retrieves information about all heroes registered in a given page.
    /// </summary>
    /// <param name="page"> The page where information will be retrieved. </param>
    /// <param name="rows"> The amount of data to get. </param>
    /// <returns> A collection of all heroes registered in the page. </returns>
    public abstract IEnumerable<HeroEntity> Get(int page, int rows);

    /// <summary>
    /// Retrieves information about a hero.
    /// </summary>
    /// <param name="id"> The id of the hero to get. </param>
    /// <returns> Information about a hero whose id matches the given one, or null, if no matching ids are found. </returns>
    public abstract HeroEntity? Get(Guid id);

    /// <summary>
    /// Updates the information about a registered hero.
    /// </summary>
    /// <param name="updatedHero"> A <see cref="HeroEntity"/> containing update to date information about the hero. </param>
    public abstract bool Update(HeroEntity updatedHero);

    /// <summary>
    /// Deletes a hero from the database.
    /// </summary>
    /// <param name="id"> The id of the hero to delete. </param>
    public abstract bool Delete(Guid id);
}
