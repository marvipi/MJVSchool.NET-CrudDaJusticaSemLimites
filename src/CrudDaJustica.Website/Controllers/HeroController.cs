using CrudDaJustica.HttpDto.Lib.Models;
using CrudDaJustica.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CrudDaJustica.Website.Controllers;

/// <summary>
/// Represents a controller that handles information about heroes.
/// </summary>
public class HeroController : Controller
{
    private readonly ILogger<HeroController> logger;
    private readonly IHttpClientFactory httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeroController"/> class.
    /// </summary>
    /// <param name="logger"> A service responsible for logging the behavior of the website. </param>
    /// <param name="httpClientFactory"> Service responsible for instantiating http clients. </param>
    public HeroController(ILogger<HeroController> logger, IHttpClientFactory httpClientFactory)
    {
        this.logger = logger;
        this.httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Lists all heroes in a given data page.
    /// </summary>
    /// <returns> A view containing a list of heroes. </returns>
    public async Task<IActionResult> Index(int page = 1, int rows = 10)
    {
        logger.LogInformation("{timestamp}: displaying a list of heroes", DateTime.Now);
        var httpClient = httpClientFactory.CreateClient("HeroApi");

        (var heroes, var pageRange, var validPage, var validRows) = await httpClient.GetFromJsonAsync<HeroGetPagedResponse>($"?page={page}&rows={rows}");
        var heroViewModels = heroes.Select(hgr => new HeroViewModel(hgr.Id, hgr.Alias, hgr.Debut, hgr.FirstName, hgr.LastName));

        var heroListModel = new HeroListModel(heroViewModels, pageRange, validPage);

        // Used to display this page when another view redirects to here.
        TempData["Rows"] = validRows;
        TempData["Page"] = validPage;

        return View(heroListModel);
    }

    /// <summary>
    /// Opens the hero creation form.
    /// </summary>
    /// <returns> A view that allows you to create new heroes. </returns>
    [HttpGet]
    public IActionResult Create()
    {
        var emptyForm = new HeroFormModel(string.Empty, DateOnly.FromDateTime(DateTime.Today), string.Empty, string.Empty);
        return View(emptyForm);
    }

    /// <summary>
    /// Creates and registers a new hero.
    /// </summary>
    /// <param name="heroFormModel"> Hero data read from a form. </param>
    /// <returns> The hero creation form. </returns>
    [HttpPost]
    public async Task<IActionResult> Create(HeroFormModel heroFormModel)
    {
        logger.LogInformation("{timestamp}: Attempting to create a new hero", DateTime.Now);

        bool success;
        if (success = ModelState.IsValid)
        {
            var httpClient = httpClientFactory.CreateClient("HeroApi");
            var postRequest = new HeroPostRequest(Guid.NewGuid(),
                                                  heroFormModel.Alias,
                                                  heroFormModel.Debut,
                                                  heroFormModel.FirstName,
                                                  heroFormModel.LastName);
            var response = await httpClient.PostAsJsonAsync(httpClient.BaseAddress, postRequest);

            success = response.IsSuccessStatusCode;
            ViewBag.Success = success;
        }

        if (success)
        {
            logger.LogInformation("{timestamp}: Hero successfully created", DateTime.Now);
        }
        else
        {
            logger.LogWarning("{timestamp}: Failed to create a new hero", DateTime.Now);
        }

        return Create();
    }
    /// <summary>
    /// Opens the hero update form.
    /// </summary>
    /// <param name="id"> The id of the hero to update. </param>
    /// <returns> The hero update form pre-filled with information. </returns>
    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var httpClient = httpClientFactory.CreateClient("HeroApi");
        var requestUrl = GetRequestUrl(id, httpClient);
        var response = await httpClient.GetFromJsonAsync<HeroViewModel>(requestUrl);

        if (response is null)
        {
            logger.LogWarning("{timestamp}: there was an attempt at updating a non-registered hero", DateTime.Now);
            return RedirectToAction(nameof(Index));
        }

        var heroFormModel = new HeroFormModel(response.Alias, response.Debut, response.FirstName, response.LastName);

        return View((heroFormModel, id));
    }


    /// <summary>
    /// Updates the information registered about a hero.
    /// </summary>
    /// <param name="id"> The id of the hero to update. </param>
    /// <param name="heroFormModel"> New information about the hero. </param>
    /// <returns> The hero update form. </returns>
    [HttpPost]
    public async Task<IActionResult> Update(Guid id, HeroFormModel heroFormModel)
    {
        logger.LogInformation("{timestamp}: Attempting to update hero", DateTime.Now);

        bool success;
        if (success = ModelState.IsValid)
        {
            var httpClient = httpClientFactory.CreateClient("HeroApi");
            var requestUrl = GetRequestUrl(id, httpClient);
            var response = await httpClient.PutAsJsonAsync(requestUrl, heroFormModel);

            success = response.IsSuccessStatusCode;
        }

        if (success)
        {
            ViewBag.Success = success;
            logger.LogInformation("{timestamp}: Hero successfully updated", DateTime.Now);
        }
        else
        {
            logger.LogWarning("{timestamp}: Failed to update hero", DateTime.Now);
        }

        return View((heroFormModel, id));
    }


    /// <summary>
    /// Deletes a hero from the repository.
    /// </summary>
    /// <param name="id"> The unique identifier of the hero to delete. </param>
    /// <returns> A redirection to the last visited index page. </returns>
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        logger.LogInformation("{timestamp}: Attempting to delete hero", DateTime.Now);

        var httpClient = httpClientFactory.CreateClient("HeroApi");
        string requestUrl = GetRequestUrl(id, httpClient);
        var response = await httpClient.DeleteAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("{timestamp}: Hero successfully deleted", DateTime.Now);
        }
        else
        {
            logger.LogWarning("{timestamp}: Failed to delete hero", DateTime.Now);
        }

        return RedirectToLastVisitedIndexPage();
    }

    // Summary: Appends an id to the end of an HttpClient's base address.
    // Returns: An URL in the format: domain/id
    private static string GetRequestUrl(Guid id, HttpClient httpClient) => $"{httpClient.BaseAddress}/{id}";

    /// <summary>
    /// Redirects to the page of the Index view last visited by the user.
    /// </summary>
    /// <returns> A <see cref="RedirectToActionResult"/> that points to last visited page of the index. </returns>
    public IActionResult RedirectToLastVisitedIndexPage()
    {
        return RedirectToIndexPage(Convert.ToInt32(TempData["Page"]));
    }

    /// <summary>
    /// Redirects to a page of the Index view, displaying the same amount of rows as before.
    /// </summary>
    /// <param name="page"> The page to redirect to. </param>
    /// <returns> A <see cref="RedirectToActionResult"/> that points to a specific page of the Index. </returns>
    public IActionResult RedirectToIndexPage(int page)
    {
        return RedirectToAction(nameof(Index), new { page, rows = Convert.ToInt32(TempData["Rows"]) });
    }

    /// <summary>
    /// Changes the amount of rows being displayed in the Index.
    /// </summary>
    /// <param name="rows"> The amount of rows to display from now on. </param>
    /// <returns> A <see cref="RedirectToActionResult"/> that points to last visited page of the index. </returns>
    public IActionResult UpdateRows(int rows)
    {
        TempData["Rows"] = rows;
        return RedirectToLastVisitedIndexPage();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
