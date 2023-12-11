namespace CrudDaJustica.HttpDto.Lib.Models;

/// <summary>
/// Initializes a new instance of the <see cref="HeroGetResponse"/> record.
/// </summary>
/// <param name="Id"> A unique identifier that distingues a hero from all others. </param>
/// <param name="Alias"> The secret identity of a hero. </param>
/// <param name="Debut"> The date of a hero's first apparition. </param>
/// <param name="FirstName"> The hero's first name. </param>
/// <param name="LastName"> The hero's last name. </param>
public record HeroGetResponse(Guid Id, string Alias, DateOnly Debut, string FirstName, string LastName);
