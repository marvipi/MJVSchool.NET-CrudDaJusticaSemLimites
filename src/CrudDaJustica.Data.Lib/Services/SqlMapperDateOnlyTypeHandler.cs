using Dapper;
using System.Data;

namespace CrudDaJustica.Data.Lib.Services;

/// <summary>
/// Represents a type handler that converts between DateOnly and DateTime when querying a SQL Server database with Dapper.
/// </summary>
/// <remarks>
/// By default, Dapper doesn't support DateOnly, so we must add support ourselves.
/// </remarks>
internal class SqlMapperDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    /// <summary>
    /// Converts a value to a DateOnly.
    /// </summary>
    /// <param name="value"> The value to convert. </param>
    /// <returns> A DateOnly parsed from another object. </returns>
    public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);

    /// <summary>
    /// Converts a DateOnly to a DateTime then assigns it to a database parameter.
    /// </summary>
    /// <param name="parameter"> A database parameter that expects a DateOnly or a DateTime. </param>
    /// <param name="value"> A DateOnly to convert. </param>
    public override void SetValue(IDbDataParameter parameter, DateOnly value) => parameter.Value = value.ToDateTime(TimeOnly.MinValue);
}
