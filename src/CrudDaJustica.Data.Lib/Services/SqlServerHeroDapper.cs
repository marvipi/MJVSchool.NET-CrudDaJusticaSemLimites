using CrudDaJustica.Data.Lib.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CrudDaJustica.Data.Lib.Services;

/// <summary>
/// Represents a service that accesses a SQL Server database through Dapper.
/// </summary>
public class SqlServerHeroDapper : SqlServerHeroDal
{
    public override int Size => sqlConnection.ExecuteScalar<int>(COUNT);

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerHeroDapper"/> class.
    /// </summary>
    /// <param name="connectionString"> A connection string of a SQL Server database. </param>
    public SqlServerHeroDapper(string connectionString)
    {
        sqlConnection = new SqlConnection(connectionString);

        sqlConnection.Open();
        sqlConnection.Close();

        SqlMapper.AddTypeHandler(new SqlMapperDateOnlyTypeHandler());
    }

    public override bool Register(HeroEntity newHero)
    {
        var rowsAffected = sqlConnection.Execute(INSERT,
            new
            {
                id = newHero.Id,
                alias = newHero.Alias,
                debut = newHero.Debut,
                firstName = newHero.FirstName,
                lastName = newHero.LastName
            });
        return rowsAffected > 0;
    }

    public override IEnumerable<HeroEntity> Get(int page, int rows) => sqlConnection.Query<HeroEntity>(GET_PAGED, new { page, rows });

    public override HeroEntity? Get(Guid id) => sqlConnection.QueryFirst<HeroEntity?>(GET, new { id });

    public override bool Update(HeroEntity updatedHero)
    {
        var rowsAffected = sqlConnection.Execute(UPDATE,
            new
            {
                id = updatedHero.Id,
                alias = updatedHero.Alias,
                debut = updatedHero.Debut,
                firstName = updatedHero.FirstName,
                lastName = updatedHero.LastName
            });
        return rowsAffected > 0;
    }

    public override bool Delete(Guid id)
    {
        var rowsAffected = sqlConnection.Execute(DELETE, new { id });
        return rowsAffected > 0;
    }
}
