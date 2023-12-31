﻿namespace CrudDaJustica.Cli.App.Models;

/// <summary>
/// Contains information about a hero that can be displayed in the user interface.
/// </summary>
public class HeroViewModel
{
    /// <summary>
    /// A unique identifier that distinguishes a hero from all others.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The name of a hero's secret identity.
    /// </summary>
    public string Alias { get; init; }

    /// <summary>
    /// A date when a hero was first seen.
    /// </summary>
    public DateOnly Debut { get; init; }

    /// <summary>
    /// The first name of the person behind the secret identity.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// The last name of the person behind the secret identity.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeroViewModel"/> struct.
    /// </summary>
    /// <param name="id"> A unique identifier that distinguishes a hero from all others. </param>
    /// <param name="alias"> The name of a hero's secret identity. </param>
    /// <param name="debut"> A date when a hero was first seen. </param>
    /// <param name="firstName"> The first name of the person behind the secret identity. </param>
    /// <param name="lastName"> The last name of the person behind the secret identity. </param>
    public HeroViewModel(Guid id, string alias, DateOnly debut, string firstName, string lastName)
    {
        Id = id;
        Alias = alias;
        Debut = debut;
        FirstName = firstName;
        LastName = lastName;
    }

    public override string ToString()
    {
        var fullName = string.Format("{0} {1}", FirstName, LastName);
        return string.Format("{0}  -  {1}  -  {2}", Alias, fullName, Debut);
    }
}