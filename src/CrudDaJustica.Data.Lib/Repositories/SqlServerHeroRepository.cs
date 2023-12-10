using CrudDaJustica.Data.Lib.Models;
using CrudDaJustica.Data.Lib.Services;

namespace CrudDaJustica.Data.Lib.Repositories;

/// <summary>
/// Represents a SQL Server database that stores information about heroes.
/// </summary>
public class SqlServerHeroRepository : HeroRepository
{
    private readonly SqlServerHeroDal sqlServerHeroDal;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerHeroRepository"/> class.
    /// </summary>
    /// <param name="pagingService"> Service responsible for paging the database. </param>
    /// <param name="sqlServerHeroDal"> Service responsible for accessing the database. </param>
    public SqlServerHeroRepository(PagingService pagingService, SqlServerHeroDal sqlServerHeroDal) : base(pagingService)
    {
        this.sqlServerHeroDal = sqlServerHeroDal;
    }

    public override bool Register(HeroEntity newHero) => sqlServerHeroDal.Register(newHero);

    public override IEnumerable<HeroEntity> Get(int page, int rows)
    {
        (var validPage, var validRows) = pagingService.Validate(page, rows, sqlServerHeroDal.Size);
        return sqlServerHeroDal.Get(validPage, validRows);
    }

    public override HeroEntity? Get(Guid id) => sqlServerHeroDal.Get(id);

    public override bool Update(HeroEntity updatedHero) => sqlServerHeroDal.Update(updatedHero);

    public override bool Delete(Guid id) => sqlServerHeroDal.Delete(id);

}
