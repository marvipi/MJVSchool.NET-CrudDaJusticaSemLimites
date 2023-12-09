namespace CrudDaJustica.Data.Lib.Models;

/// <summary>
/// Represents a hero from the DC Universe.
/// </summary>
[Serializable]
public class HeroEntity
{
    /// <summary>
    /// A unique global identifier that distinguishes this hero from all others.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The name of a hero's secret identity.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// The date when a hero was first seen.
    /// </summary>
    public DateOnly Debut { get; set; }

    /// <summary>
    /// The first name of the person behind the secret identity.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// The last name of the person behind the secret identity.
    /// </summary>
    public string LastName { get; set; }

    private HeroEntity() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeroEntity"/> class.
    /// </summary>
    /// <param name="id"> A unique global identifier that distinguishes this hero from all others. </param>
    /// <param name="alias"> The name of a hero's secret identity. </param>
    /// <param name="debut"> A date when a hero was first seen. </param>
    /// <param name="firstName"> The first name of the person behind the secret identity. </param>
    /// <param name="lastName"> The last name of the person behind the secret identity. </param>
    public HeroEntity(Guid id, string alias, DateOnly debut, string firstName, string lastName)
    {
        Id = id;
        Alias = alias;
        Debut = debut;
        FirstName = firstName;
        LastName = lastName;
    }

    public override bool Equals(object? obj)
    {
        return obj is HeroEntity entity &&
               Id.Equals(entity.Id) &&
               Alias == entity.Alias &&
               Debut.Equals(entity.Debut) &&
               FirstName == entity.FirstName &&
               LastName == entity.LastName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Alias, Debut, FirstName, LastName);
    }
}
