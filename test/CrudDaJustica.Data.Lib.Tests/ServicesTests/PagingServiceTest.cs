using CrudDaJustica.Data.Lib.Services;

namespace CrudDaJustica.Data.Lib.Test.ServicesTests;

[TestFixture]
internal class PagingServiceTest
{
    [Test]
    public void Validate_ValidPageAndRows_ReturnsSamePageAndRows()
    {
        var pagingService = new PagingService();
        const int EXPECTED_PAGE = 2;
        const int EXPECTED_ROWS = 10;
        const int REPOSITORY_SIZE = 30;

        (var validPage, var validRows) = pagingService.Validate(EXPECTED_PAGE, EXPECTED_ROWS, REPOSITORY_SIZE);

        Assert.Multiple(() =>
        {
            Assert.That(validPage, Is.EqualTo(EXPECTED_PAGE));
            Assert.That(validRows, Is.EqualTo(EXPECTED_ROWS));
        });
    }

    [Test]
    public void Validate_PageIsLessThanFirstPage_ReturnsFirstPage()
    {
        var pagingService = new PagingService();
        const int NOT_BEING_TESTED = 5;

        (var validPage, var _) = pagingService.Validate(PagingService.FIRST_PAGE - 1, PagingService.MIN_ROWS_PER_PAGE, NOT_BEING_TESTED);

        Assert.That(validPage, Is.EqualTo(PagingService.FIRST_PAGE));
    }

    [Test]
    public void Validate_PageIsGreaterThanLastPage_ReturnsLastPage()
    {
        var pagingService = new PagingService();
        const int EXPECTED_LAST_PAGE = 5;
        const int REPOSITORY_SIZE = PagingService.MIN_ROWS_PER_PAGE * EXPECTED_LAST_PAGE;

        (var validPage, var _) = pagingService.Validate(EXPECTED_LAST_PAGE + 1, PagingService.MIN_ROWS_PER_PAGE, REPOSITORY_SIZE);

        Assert.That(validPage, Is.EqualTo(pagingService.LastPage));
    }

    [Test]
    public void Validate_RowsIsLessThanMinRowsPerPage_ReturnsMinRowsPerPage()
    {
        var pagingService = new PagingService();
        const int NOT_BEING_TESTED = 1;

        (var _, var validRows) = pagingService.Validate(NOT_BEING_TESTED, PagingService.MIN_ROWS_PER_PAGE - 1, NOT_BEING_TESTED);

        Assert.That(validRows, Is.EqualTo(PagingService.MIN_ROWS_PER_PAGE));
    }

    [Test]
    public void Validate_RowsIsGreaterThanMaxRowsPerPage_ReturnsMaxRowsPerPage()
    {
        var pagingService = new PagingService();
        const int NOT_BEING_TESTED = 1;

        (var _, var validRows) = pagingService.Validate(NOT_BEING_TESTED, PagingService.MAX_ROWS_PER_PAGE + 1, NOT_BEING_TESTED);

        Assert.That(validRows, Is.EqualTo(PagingService.MAX_ROWS_PER_PAGE));
    }
}
