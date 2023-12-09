using CrudDaJustica.Cli.App.Models;
using CrudDaJustica.Data.Lib.Models;
using CrudDaJustica.Data.Lib.Repositories;
using System.Globalization;

namespace CrudDaJustica.Cli.App.Controllers;

/// <summary>
/// Represents a controller that manages communication between the hero repository and the user interface.
/// </summary>
public class HeroController
{
    private readonly HeroRepository heroRepository;

    /// <summary>
    /// The current data page of the repository.
    /// </summary>
    public int CurrentPage => heroRepository.CurrentPage;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeroController"/> class.
    /// </summary>
    /// <param name="heroRepository"> Repository that stores information about the heroes. </param>
    public HeroController(HeroRepository heroRepository)
    {
        this.heroRepository = heroRepository;
    }

    /// <summary>
    /// Assures that data read from a form is valid.
    /// </summary>
    /// <param name="heroFormModel"> The hero data read from a form. </param>
    /// <returns> A collection of all problems encountered in the data. Or an empty collection, if no problems where found. </returns>
    public IEnumerable<string> Validate(HeroFormModel heroFormModel)
    {
        var validationProblems = new List<string>();

        var expectedFormat = CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern;
        var validDebut = DateOnly.TryParseExact(heroFormModel.Debut,
            expectedFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var _);

        if (!validDebut)
        {
            var debutProblem = string.Format("Invalid debut date. Expected format {0}, given {1}", expectedFormat, heroFormModel.Debut);
            validationProblems.Add(debutProblem);
        }

        if (string.IsNullOrWhiteSpace(heroFormModel.Alias))
        {
            validationProblems.Add("Alias is required");
        }

        if (string.IsNullOrWhiteSpace(heroFormModel.FirstName))
        {
            validationProblems.Add("First name is required");
        }

        if (string.IsNullOrWhiteSpace(heroFormModel.LastName))
        {
            validationProblems.Add("Last name is required");
        }

        return validationProblems;
    }

    /// <summary>
    /// Creates a new hero in the repository>.
    /// </summary>
    /// <param name="heroFormModel"> A form model that contains data about the new hero. </param>
    public void Create(HeroFormModel heroFormModel)
    {
        var validDate = DateOnly.ParseExact(heroFormModel.Debut!,
            CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern,
            CultureInfo.InvariantCulture);

        heroRepository.Register(new(
            Guid.NewGuid(),
            heroFormModel.Alias!, validDate,
            heroFormModel.FirstName!,
            heroFormModel.LastName!));
    }

    /// <summary>
    /// Lists all heroes registered in the current page of the repository.
    /// </summary>
    /// <returns> 
    /// A collection of all heroes registered in the current page of the repository. 
    /// </returns>
    public IEnumerable<HeroViewModel> List()
    {
        var heroes = heroRepository.Get(heroRepository.CurrentPage, heroRepository.RowsPerPage);

        var heroViewModels = heroes
            .Select(hero => new HeroViewModel(hero.Id, hero.Alias, hero.Debut, hero.FirstName, hero.LastName))
            .ToList();

        return heroViewModels;
    }

    /// <summary>
    /// Updates the information about a hero.
    /// </summary>
    /// <param name="id"> The id of the hero to update. </param>
    /// <param name="heroFormModel"> The updated information about a hero. </param>
    public void Update(Guid id, HeroFormModel heroFormModel)
    {
        var validDate = DateOnly.ParseExact(heroFormModel.Debut!,
            CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern,
            CultureInfo.InvariantCulture);

        heroRepository.Update(new HeroEntity(id,
                                             heroFormModel.Alias!,
                                             validDate,
                                             heroFormModel.FirstName!,
                                             heroFormModel.LastName!));
    }

    /// <summary>
    /// Removes a hero from the repository.
    /// </summary>
    /// <param name="id"> The id of the hero to delete. </param>
    public void Delete(Guid id) => heroRepository.Delete(id);

    /// <summary>
    /// Moves to the next page of the repository, up to the last page.
    /// </summary>
    public void NextPage() => heroRepository.Get(heroRepository.CurrentPage + 1, heroRepository.RowsPerPage);

    /// <summary>
    /// Returns to the previous page of the repository, down to the first page.
    /// </summary>
    public void PreviousPage() => heroRepository.Get(heroRepository.CurrentPage - 1, heroRepository.RowsPerPage);
}