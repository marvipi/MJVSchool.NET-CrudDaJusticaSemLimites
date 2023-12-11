using System.ComponentModel.DataAnnotations;

namespace CrudDaJustica.HttpDto.Lib.Models;

/// <summary>
/// Initializes a new instance of the <see cref="HeroPostRequest"/> record.
/// </summary>
/// <param name="Alias"> The alias of the new hero. </param>
/// <param name="Debut"> The date of the new hero's first apparition. </param>
/// <param name="FirstName"> The first name of the new hero. </param>
/// <param name="LastName"> The last name of the new hero. </param>
public record HeroPostRequest(
    [Required(AllowEmptyStrings = false, ErrorMessage = "Alias is required")] string Alias,
    [Required(ErrorMessage = "Debut is required")] DateOnly Debut,
    [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required")] string FirstName,
    [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")] string LastName);