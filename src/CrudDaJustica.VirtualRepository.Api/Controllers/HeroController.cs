using CrudDaJustica.ApiCommon.Lib.Controllers;
using CrudDaJustica.Data.Lib.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CrudDaJustica.VirtualRepository.Api.Controllers;

/// <summary>
/// Represents an API controller that deals with hero information.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class HeroController : HeroApiController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HeroController"/> class.
    /// </summary>
    /// <param name="logger"> A service that logs requests and responses. </param>
    /// <param name="heroRepository"> A data repository that stores hero information. </param>
    public HeroController(ILogger<HeroController> logger, HeroRepository heroRepository) : base(logger, heroRepository) { }
}
