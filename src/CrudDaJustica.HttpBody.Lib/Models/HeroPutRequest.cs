using System.ComponentModel.DataAnnotations;

namespace CrudDaJustica.HttpDto.Lib.Models;

/// <summary>
/// Initializes a new instance of the <see cref="HeroPutRequest"/> record.
/// </summary>
/// <param name="Alias"> The new secret identity of a hero. </param>
/// <param name="Debut"> The new debut date of a hero. </param>
/// <param name="FirstName"> The new first name of a hero. </param>
/// <param name="LastName"> The new last name of a hero. </param>
public record HeroPutRequest(
	[Required(AllowEmptyStrings = false, ErrorMessage = "Alias is required")] string Alias,
	[Required(ErrorMessage = "Debut is required")] DateOnly Debut,
	[Required(AllowEmptyStrings = false, ErrorMessage = "First name is required")] string FirstName,
	[Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")] string LastName);