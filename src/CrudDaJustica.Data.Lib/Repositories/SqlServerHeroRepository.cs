using CrudDaJustica.Data.Lib.Models;
using CrudDaJustica.Data.Lib.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CrudDaJustica.Data.Lib.Repositories;

/// <summary>
/// Represents a SQL Server database that stores information about heroes.
/// </summary>
public class SqlServerHeroRepository : HeroRepository
{
    private int Size
    {
        get
        {
            countCommand.Connection.Open();
            var heroRepositorySize = (int)countCommand.ExecuteScalar();
            countCommand.Connection.Close();
            return heroRepositorySize;
        }
    }

    private readonly SqlConnection sqlConnection;

    // Summary: Counts how many heroes are registered in the database.
    // Remarks: Used to calculate the repository size.
    private readonly SqlCommand countCommand;

    private const string COUNT =
        @"SELECT COUNT(*) as HeroRepositorySize
          FROM HeroInformation;";

    private const string INSERT =
        @"EXECUTE InsertHero @Alias = @alias, 
                             @Debut = @debut, 
                             @FirstName = @firstName, 
                             @LastName = @lastName;";

    private const string GET_PAGED =
        @"SELECT Id, Alias, Debut, FirstName, LastName
          FROM HeroInformation
          ORDER BY Alias
          OFFSET (@page - 1) * @rows ROWS
          FETCH NEXT @rows ROWS ONLY;";

    private const string GET =
        @"SELECT Id, Alias, Debut, FirstName, LastName
          FROM HeroInformation
          WHERE Id = @id;";

    private const string UPDATE =
        @"EXECUTE UpdateHero @Id = @id,
                             @Alias = @alias,
                             @Debut = @debut,
                             @FirstName = @firstName,
                             @LastName = @lastName;";

    private const string DELETE = @"EXECUTE DeleteHero @Id = @id;";

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerHeroRepository"/> class.
    /// </summary>
    /// <param name="pagingService"> Service responsible for paging the database. </param>
    /// <param name="connectionString"> A connection string for a SQL Server database. </param>
    public SqlServerHeroRepository(PagingService pagingService, string connectionString) : base(pagingService)
    {
        sqlConnection = new(connectionString);
        countCommand = new(COUNT, sqlConnection);

        sqlConnection.Open();
        sqlConnection.Close();
    }

    public override bool Register(HeroEntity newHero)
    {
        var createHeroCommand = new SqlCommand(INSERT, sqlConnection);
        createHeroCommand.Parameters.AddRange(new SqlParameter[]
        {
            new("id", newHero.Id),
            new("alias", newHero.Alias),
            new("debut", newHero.Debut),
            new("firstName", newHero.FirstName),
            new("lastName", newHero.LastName),
        });

        createHeroCommand.Connection.Open();
        var amountOfHeroesCreated = createHeroCommand.ExecuteNonQuery();
        createHeroCommand.Connection.Close();

        return amountOfHeroesCreated > 0;
    }

    public override IEnumerable<HeroEntity> Get(int page, int rows)
    {
        (var validPage, var validRows) = pagingService.Validate(page, rows, Size);

        var getHeroesCommand = new SqlCommand(GET_PAGED, sqlConnection);
        getHeroesCommand.Parameters.AddRange(new SqlParameter[]
        {
            new("page", validPage),
            new("rows", validRows),
        });

        getHeroesCommand.Connection.Open();

        var heroes = new List<HeroEntity>();
        using (var sqlReader = getHeroesCommand.ExecuteReader(CommandBehavior.CloseConnection))
        {
            while (sqlReader.Read())
            {
                heroes.Add(ReadHeroData(sqlReader));
            }
        }

        return heroes;
    }

    public override HeroEntity? Get(Guid id)
    {
        var getHeroCommand = new SqlCommand(GET, sqlConnection);
        getHeroCommand.Parameters.AddWithValue("id", id);

        getHeroCommand.Connection.Open();

        HeroEntity? hero = null;
        using (var sqlReader = getHeroCommand.ExecuteReader(CommandBehavior.CloseConnection))
        {
            if (sqlReader.Read())
            {
                hero = ReadHeroData(sqlReader);
            }
        }

        return hero;
    }

    // Summary: Reads hero data from a SqlDataReader then uses it to initialize a new HeroEntity.
    // Remarks: Assumes hero data cannot be null.
    private static HeroEntity ReadHeroData(SqlDataReader sqlReader)
    {
        var idFromQuery = sqlReader.GetGuid(0);
        var alias = sqlReader.GetString(1);
        var debut = DateOnly.FromDateTime(sqlReader.GetDateTime(2));
        var firstName = sqlReader.GetString(3);
        var lastName = sqlReader.GetString(4);

        return new HeroEntity { Id = idFromQuery, Alias = alias, Debut = debut, FirstName = firstName, LastName = lastName };
    }

    public override bool Update(Guid id, HeroEntity updatedHero)
    {
        var updateHeroCommand = new SqlCommand(UPDATE, sqlConnection);
        updateHeroCommand.Parameters.AddRange(new SqlParameter[]
        {
            new("id", id),
            new("alias", updatedHero.Alias),
            new("debut", updatedHero.Debut),
            new("firstName", updatedHero.FirstName),
            new("lastName", updatedHero.LastName),
        });

        updateHeroCommand.Connection.Open();
        var rowsUpdated = updateHeroCommand.ExecuteNonQuery();
        updateHeroCommand.Connection.Close();

        return rowsUpdated > 0;
    }

    public override bool Delete(Guid id)
    {
        var deleteHeroCommand = new SqlCommand(DELETE, sqlConnection);
        deleteHeroCommand.Parameters.AddWithValue("id", id);

        deleteHeroCommand.Connection.Open();
        var rowsDeleted = deleteHeroCommand.ExecuteNonQuery();
        deleteHeroCommand.Connection.Close();

        return rowsDeleted > 0;
    }

}
