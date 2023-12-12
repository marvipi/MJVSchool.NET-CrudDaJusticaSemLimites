using CrudDaJustica.HttpDto.Lib.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrudDaJustica.WebApi.Controllers;

/// <summary>
/// Represents an API controller that deals with hero information.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class HeroController : ControllerBase
{
    private readonly IHttpClientFactory httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeroController"/> class.
    /// </summary>
    /// <param name="httpClientFactory"> Service used to instantiate the <see cref="HttpClient"/> class. </param>
    public HeroController(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Produces all heroes registered in a given page of the SQL Server Database.
    /// </summary>
    /// <param name="page"> The page where the heroes are registered. </param>
    /// <param name="rows"> The amount of heroes to fetch. </param>
    /// <returns> A <see cref="HeroGetPagedResponse"/>. </returns>
    [HttpGet]
    public async Task<IActionResult> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int rows = 10)
    {
        var sqlServerRepoService = httpClientFactory.CreateClient("SqlServerRepository");
        var requestUri = new Uri(sqlServerRepoService.BaseAddress.AbsoluteUri + $"?page={page}&rows={rows}");
        var response = await sqlServerRepoService.GetFromJsonAsync<HeroGetPagedResponse>(requestUri);
        return Ok(response);
    }

    /// <summary>
    /// Searches for a hero in the SQL Server database.
    /// </summary>
    /// <param name="id"> The unique identifier of the hero to get. </param>
    /// <returns> 
    ///     <see cref="NotFoundResult"/> if the given id doesn't match any heroes in the database,
    ///     or a <see cref="HeroGetResponse"/> that contains information about the hero.
    /// </returns>
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var sqlServerRepoService = httpClientFactory.CreateClient("SqlServerRepository");
        var requrestUri = new Uri(sqlServerRepoService.BaseAddress.AbsoluteUri + $"/{id}");
        var response = await sqlServerRepoService.GetFromJsonAsync<HeroGetResponse>(requrestUri);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    /// <summary>
    /// Registers a new hero in the Json file and in the SQL Server database.
    /// </summary>
    /// <param name="request"> Information about the new hero. </param>
    /// <returns> 
    ///     <see cref="BadRequestResult"/> if the request contains invalid data,
    ///     or a <see cref="CreatedAtActionResult"/> that points to where the new hero was created. 
    /// </returns>
    /// <remarks>
    /// The hero will have a different id in each repository.
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HeroPostRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var jsonRepoService = httpClientFactory.CreateClient("JsonRepository");
        var sqlServerRepoService = httpClientFactory.CreateClient("SqlServerRepository");

        await jsonRepoService.PostAsJsonAsync(jsonRepoService.BaseAddress, request);
        var response = await sqlServerRepoService.PostAsJsonAsync(sqlServerRepoService.BaseAddress, request);

        return Created(response.Headers.Location, null);

    }

    /// <summary>
    /// Updates a hero in SQL Server database.
    /// </summary>
    /// <param name="id"> The id of the hero to update. </param>
    /// <param name="request"> New information about the hero. </param>
    /// <returns> 
    ///     Produces <see cref="BadRequestResult"/> if request isn't valid,
    ///     <see cref="NoContentResult"/> if the update is successful,
    ///     or <see cref="NotFoundResult"/> if the id is not registered in the repository.
    /// </returns>
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] HeroPutRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var sqlServerRepoService = httpClientFactory.CreateClient("SqlServerRepository");
        var response = await sqlServerRepoService.PutAsJsonAsync(sqlServerRepoService.BaseAddress + $"/{id}", request);

        if (response.IsSuccessStatusCode)
        {
            return NoContent();
        }

        return NotFound();
    }

    /// <summary>
    /// Deletes a hero from the SQL Server database.
    /// </summary>
    /// <param name="id"> The unique identifier of the hero to delete. </param>
    /// <returns> 
    ///     <see cref="NoContentResult"/> if the hero was successfully deleted,
    ///     Or <see cref="NotFoundResult"/> if the id is not registered in the repository.
    /// </returns>
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var sqlServerRepoService = httpClientFactory.CreateClient("SqlServerRepository");
        var response = await sqlServerRepoService.DeleteAsync(sqlServerRepoService.BaseAddress + $"/{id}");

        if (response.IsSuccessStatusCode)
        {
            return NoContent();
        }

        return NotFound();
    }
}
