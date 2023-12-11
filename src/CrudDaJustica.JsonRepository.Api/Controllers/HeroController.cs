using CrudDaJustica.Data.Lib.Models;
using CrudDaJustica.Data.Lib.Repositories;
using CrudDaJustica.Data.Lib.Services;
using CrudDaJustica.HttpDto.Lib.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrudDaJustica.JsonRepository.Api.Controllers;

/// <summary>
/// Represents an API controller that deals with hero information.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class HeroController : ControllerBase
{
	private readonly ILogger<HeroController> logger;
	private readonly HeroRepository heroRepository;

	/// <summary>
	/// Initializes a new instance of the <see cref="HeroController"/> class.
	/// </summary>
	/// <param name="logger"> A service that logs requests and responses. </param>
	/// <param name="heroRepository"> A data repository that stores hero information. </param>
	public HeroController(ILogger<HeroController> logger, HeroRepository heroRepository)
	{
		this.logger = logger;
		this.heroRepository = heroRepository;
	}

	/// <summary>
	/// Produces all heroes registered in a given page of an <see cref="HeroRepository"/>.
	/// </summary>
	/// <param name="page"> The page where the heroes are registered. </param>
	/// <param name="rows"> The amount of heroes to fetch. </param>
	/// <returns> A <see cref="HeroGetPagedResponse"/>. </returns>
	[HttpGet]
	public IActionResult GetPage(
		[FromQuery] int page = PagingService.FIRST_PAGE,
		[FromQuery] int rows = PagingService.MIN_ROWS_PER_PAGE)
	{
		logger.LogInformation("{timestamp}: getting a page of heroes", DateTime.Now);

		var heroes = heroRepository
			.Get(page, rows)
			.Select(he => new HeroGetResponse(he.Id, he.Alias, he.Debut, he.FirstName, he.LastName));

		return Ok(new HeroGetPagedResponse(heroes, heroRepository.PageRange, heroRepository.CurrentPage, heroRepository.RowsPerPage));
	}

	/// <summary>
	/// Searches for a hero in an <see cref="HeroRepository"/>.
	/// </summary>
	/// <param name="id"> The unique identifier of the hero to get. </param>
	/// <returns> 
	///     A <see cref="HeroGetResponse"/> that contains information about the hero.
	///     Or <see cref="NotFoundResult"/>, if the given id doesn't match any heroes in the repository.
	/// </returns>
	[HttpGet]
	[Route("{id}")]
	public IActionResult Get(Guid id)
	{
		logger.LogInformation("{timestamp}: getting a hero from the repository", DateTime.Now);

		var hero = heroRepository.Get(id);

		if (hero is not null)
		{
			logger.LogInformation("{timestamp}: hero successfully fetched from the repository", DateTime.Now);
			return Ok(new HeroGetResponse(hero.Id, hero.Alias, hero.Debut, hero.FirstName, hero.LastName));
		}
		else
		{
			logger.LogWarning("{timestamp}: hero was not registered in the repository", DateTime.Now);
			return NotFound();
		}
	}

	/// <summary>
	/// Registers a new hero in the repository.
	/// </summary>
	/// <param name="request"> Information about the new hero. </param>
	/// <returns> 
	///     A <see cref="CreatedAtActionResult"/> that points to where the new hero was created. 
	///     Or <see cref="BadRequestResult"/>, if the request contains invalid data.
	/// </returns>
	[HttpPost]
	public IActionResult Create([FromBody] HeroPostRequest request)
	{
		logger.LogInformation("{timestamp}: Attempting to create a new hero", DateTime.Now);

		bool success;
		Guid newHeroId = Guid.NewGuid();
		if (success = ModelState.IsValid)
		{
			var newHero = new HeroEntity(newHeroId,
										 request.Alias,
										 request.Debut,
										 request.FirstName,
										 request.LastName);

			success = heroRepository.Register(newHero);
		}

		if (success)
		{
			logger.LogInformation("{timestamp}: Hero successfully created", DateTime.Now);
			return CreatedAtAction(nameof(Get), new { id = newHeroId }, null);
		}
		else
		{
			logger.LogWarning("{timestamp}: Failed to create a new hero", DateTime.Now);
			return BadRequest();
		}
	}

	/// <summary>
	/// Updates a hero.
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
	public IActionResult Update(Guid id, [FromBody] HeroPutRequest request)
	{
		logger.LogInformation("{timestamp}: Attempting to update hero", DateTime.Now);

		if (!ModelState.IsValid)
		{
			logger.LogWarning("{timestamp}: Failed to update hero due to invalid request", DateTime.Now);
			return BadRequest();
		}

		var updatedInformation = new HeroEntity(id,
												request.Alias,
												request.Debut,
												request.FirstName,
												request.LastName);
		var success = heroRepository.Update(updatedInformation);

		if (success)
		{
			logger.LogInformation("{timestamp}: Hero successfully updated", DateTime.Now);
			return NoContent();
		}
		else
		{
			logger.LogWarning("{timestamp}: Failed to update hero due to non-registered id", DateTime.Now);
			return NotFound();
		}
	}

	/// <summary>
	/// Deletes a hero from the repository.
	/// </summary>
	/// <param name="id"> The unique identifier of the hero to delete. </param>
	/// <returns> 
	///     <see cref="NoContentResult"/> if the hero was successfully deleted.
	///     Or <see cref="NotFoundResult"/>, if the id is not registered in the repository.
	/// </returns>
	[HttpDelete]
	[Route("{id}")]
	public IActionResult Delete(Guid id)
	{
		var success = heroRepository.Delete(id);

		if (success)
		{
			logger.LogInformation("{timestamp}: Hero successfully deleted", DateTime.Now);
			return NoContent();
		}
		else
		{
			logger.LogWarning("{timestamp}: Failed to delete hero", DateTime.Now);
			return NotFound();
		}
	}
}
