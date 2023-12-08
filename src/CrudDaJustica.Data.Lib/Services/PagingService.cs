using CrudDaJustica.Data.Lib.Repositories;

namespace CrudDaJustica.Data.Lib.Services;

/// <summary>
/// Represents a service that does data paging in a repository.
/// </summary>
public class PagingService
{
    private int rowsPerPage;

    // Summary: The hero repository being paged by this service.
    private readonly IHeroRepository heroRepository;

    /// <summary>
    /// The first page of data in the repository.
    /// </summary>
    public const int FIRST_PAGE = 1;

    /// <summary>
    /// The minimum amount of rows that a data page can contain.
    /// </summary>
    public const int MIN_ROWS_PER_PAGE = 10;

    /// <summary>
    /// The maximum amount of rows that a data page can contain.
    /// </summary>
    public const int MAX_ROWS_PER_PAGE = 100;

    /// <summary>
    /// The current page of the repository.
    /// </summary>
    public int CurrentPage { get; private set; }

    /// <summary>
    /// The last page of data in the repository.
    /// </summary>
    public int LastPage { get; private set; }

    /// <summary>
    /// The amount of rows contained in each page of data.
    /// </summary>
    public int RowsPerPage
    {
        get => rowsPerPage;
        set
        {
            rowsPerPage = value < MIN_ROWS_PER_PAGE
                ? MIN_ROWS_PER_PAGE
                : value > MAX_ROWS_PER_PAGE
                ? MAX_ROWS_PER_PAGE
                : value;
        }
    }

    /// <summary>
    /// Gets the <see cref="CurrentPage"/> and the <see cref="RowsPerPage"/> as a <see cref="Services.DataPage"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="Services.DataPage"/> that can be used to retrieve data from the repository.
    /// </returns>
    public DataPage DataPage => new(CurrentPage, RowsPerPage);

    /// <summary>
    /// Produces a range of pages in the range [FIRST_PAGE, LastPage]
    /// </summary>
    public IEnumerable<int> PageRange => Enumerable.Range(FIRST_PAGE, LastPage);

    /// <summary>
    /// Initializes a new instance of the <see cref="PagingService"/> class.
    /// </summary>
    /// <param name="heroRepository"> The hero repository that will be paged by this service. </param>
    /// <param name="rowsPerPage"> The amount of rows read per page. </param>
    public PagingService(IHeroRepository heroRepository, int rowsPerPage)
    {
        this.heroRepository = heroRepository;
        RowsPerPage = rowsPerPage;
        CurrentPage = FIRST_PAGE;
        CalculateLastPage();
    }

    /// <summary>
    /// Jumps to a data page.
    /// </summary>
    /// <param name="number"> The number of the page to jump to. </param>
    /// <remarks> Always stays within range of <see cref="FIRST_PAGE"/> and <see cref="LastPage"/>. </remarks>
    public void JumpToPage(int number)
    {
        CalculateLastPage();
        CurrentPage = number < FIRST_PAGE
            ? FIRST_PAGE
            : number > LastPage
            ? LastPage
            : number;
    }

    // Summary: Calculates the last page of the repository based on its size.
    private void CalculateLastPage()
    {
        var numPagesRequired = heroRepository.RepositorySize / (double)RowsPerPage;
        var lastPage = (int) Math.Ceiling(numPagesRequired);
        LastPage = lastPage < FIRST_PAGE 
            ? FIRST_PAGE 
            : lastPage;
    }
}
