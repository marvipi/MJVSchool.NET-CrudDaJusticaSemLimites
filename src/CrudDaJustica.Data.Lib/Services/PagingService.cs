namespace CrudDaJustica.Data.Lib.Services;

/// <summary>
/// Represents a service that does data paging in a repository.
/// </summary>
public class PagingService
{
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
    public int RowsPerPage { get; private set; }

    /// <summary>
    /// Produces a range of pages in the range [FIRST_PAGE, LastPage]
    /// </summary>
    public IEnumerable<int> PageRange => Enumerable.Range(FIRST_PAGE, LastPage);

    /// <summary>
    /// Validates and stores the page and rows.
    /// </summary>
    /// <param name="page"> The page to validate. </param>
    /// <param name="rows"> The amount of rows per data page. </param>
    /// <param name="repositorySize"> The size of the repository to page. </param>
    /// <returns></returns>
    public (int validPage, int validRows) Validate(int page, int rows, int repositorySize)
    {
        var validRows = rows < MIN_ROWS_PER_PAGE
            ? MIN_ROWS_PER_PAGE
            : rows > MAX_ROWS_PER_PAGE
            ? MAX_ROWS_PER_PAGE
            : rows;
        RowsPerPage = validRows;

        CalculateLastPage(RowsPerPage, repositorySize);

        var validPage = page < FIRST_PAGE
            ? FIRST_PAGE
            : page > LastPage
            ? LastPage
            : page;
        CurrentPage = validPage;

        return (validPage, validRows);
    }

    // Summary: Calculates the last page of the repository based on its size and the amount of rows per data page.
    private void CalculateLastPage(int rowsPerPage, int repositorySize)
    {
        var numPagesRequired = repositorySize / (double)rowsPerPage;
        var lastPage = (int)Math.Ceiling(numPagesRequired);
        LastPage = lastPage < FIRST_PAGE
            ? FIRST_PAGE
            : lastPage;
    }
}
