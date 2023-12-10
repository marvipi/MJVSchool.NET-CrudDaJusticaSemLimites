using CrudDaJustica.Data.Lib.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CrudDaJustica.Data.Lib.Services;

/// <summary>
/// Represents a data access layer that uses ADO.NET to connect to a SQL Server database.
/// </summary>
public class SqlServerHeroAdo : SqlServerHeroDal
{
    // Summary: Counts how many heroes are registered in the database.
    // Remarks: Used to calculate the repository size.
    private readonly SqlCommand countCommand;

    public override int Size
    {
        get
        {
            countCommand.Connection.Open();
            var heroRepositorySize = (int)countCommand.ExecuteScalar();
            countCommand.Connection.Close();
            return heroRepositorySize;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerHeroAdo"/> class.
    /// </summary>
    /// <param name="connectionString"></param>
    public SqlServerHeroAdo(string connectionString)
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
        var getHeroesCommand = new SqlCommand(GET_PAGED, sqlConnection);
        getHeroesCommand.Parameters.AddRange(new SqlParameter[]
        {
            new("page", page),
            new("rows", rows),
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

        return new HeroEntity(idFromQuery, alias, debut, firstName, lastName);
    }

    public override bool Update(HeroEntity updatedHero)
    {
        var updateHeroCommand = new SqlCommand(UPDATE, sqlConnection);
        updateHeroCommand.Parameters.AddRange(new SqlParameter[]
        {
            new("id", updatedHero.Id),
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
