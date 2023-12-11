namespace CrudDaJustica.HttpDto.Lib.Models;

/// <summary>
/// Initializes a new instance of the <see cref="HeroGetPagedResponse"/> record.
/// </summary>
/// <param name="Heroes"> A collection of hero information. </param>
/// <param name="PageRange"> A range of integers that represent all pages in a hero repository. </param>
/// <param name="Page"> The page where the <see cref="Heroes"/> are registered. </param>
/// <param name="Rows"> The amount of <see cref="Heroes"/> registered in the <see cref="Page"/>. </param>
public record HeroGetPagedResponse(IEnumerable<HeroGetResponse> Heroes, IEnumerable<int> PageRange, int Page, int Rows);
