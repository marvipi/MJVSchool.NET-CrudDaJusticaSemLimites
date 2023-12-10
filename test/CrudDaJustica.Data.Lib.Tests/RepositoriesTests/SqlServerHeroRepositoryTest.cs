using CrudDaJustica.Data.Lib.Models;
using CrudDaJustica.Data.Lib.Repositories;
using CrudDaJustica.Data.Lib.Services;
using Moq;

namespace CrudDaJustica.Data.Lib.Test.RepositoriesTests;

[TestFixture]
internal class SqlServerHeroRepositoryTest
{
    [Test]
    public void Register_CallsSqlServerHeroDal()
    {
        var pagingService = new PagingService();
        var sqlServerHeroDal = new Mock<SqlServerHeroDal>();
        var newHero = new HeroEntity(Guid.NewGuid(), "Johnny Quick", new(1941, 9, 1), "Johnny", "Chambers");

        sqlServerHeroDal
            .Setup(x => x.Register(newHero))
            .Returns(true);
        var heroRepo = new SqlServerHeroRepository(pagingService, sqlServerHeroDal.Object);
        var success = heroRepo.Register(newHero);

        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            sqlServerHeroDal.Verify(x => x.Register(newHero), Times.Once);
        });
    }

    [Test]
    public void Get_ById_CallsSqlServerHeroDal()
    {
        var pagingService = new PagingService();
        var sqlServerHeroDal = new Mock<SqlServerHeroDal>();
        var newHeroId = Guid.NewGuid();
        var newHero = new HeroEntity(newHeroId, "Batwoman", new(1956, 7, 1), "Kathy", "Kane");

        sqlServerHeroDal
            .Setup(x => x.Get(newHeroId))
            .Returns(newHero);
        var heroRepo = new SqlServerHeroRepository(pagingService, sqlServerHeroDal.Object);
        var getResult = heroRepo.Get(newHeroId);

        Assert.Multiple(() =>
        {
            Assert.That(getResult, Is.EqualTo(newHero));
            sqlServerHeroDal.Verify(x => x.Get(newHeroId), Times.Once);
        });
    }

    [Test]
    public void Get_Paged_CallsSqlServerHeroDal()
    {
        var pagingService = new PagingService();
        var sqlServerHeroDal = new Mock<SqlServerHeroDal>();
        var validPage = PagingService.FIRST_PAGE;
        var validRows = PagingService.MIN_ROWS_PER_PAGE;

        sqlServerHeroDal
            .Setup(x => x.Get(validPage, validRows))
            .Returns(new List<HeroEntity>());
        var heroRepo = new SqlServerHeroRepository(pagingService, sqlServerHeroDal.Object);
        var getResult = heroRepo.Get(validPage, validRows);

        Assert.Multiple(() =>
        {
            Assert.That(getResult, Is.Empty);
            sqlServerHeroDal.Verify(x => x.Get(validPage, validRows), Times.Once);
        });
    }

    [Test]
    public void Update_CallsSqlServerHeroDal()
    {
        var pagingService = new PagingService();
        var sqlServerHeroDal = new Mock<SqlServerHeroDal>();
        var updatedHero = new HeroEntity(Guid.NewGuid(), "Captain Comet", new(1951, 7, 1), "Adam", "Blake");

        sqlServerHeroDal
            .Setup(x => x.Update(updatedHero))
            .Returns(true);
        var heroRepo = new SqlServerHeroRepository(pagingService, sqlServerHeroDal.Object);
        var success = heroRepo.Update(updatedHero);

        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            sqlServerHeroDal.Verify(x => x.Update(updatedHero), Times.Once);
        });
    }

    [Test]
    public void Delete_CallsSqlServerHeroDal()
    {
        var pagingService = new PagingService();
        var sqlServerHeroDal = new Mock<SqlServerHeroDal>();
        var heroToDeleteId = Guid.NewGuid();

        sqlServerHeroDal
            .Setup(x => x.Delete(heroToDeleteId))
            .Returns(true);
        var heroRepo = new SqlServerHeroRepository(pagingService, sqlServerHeroDal.Object);
        var success = heroRepo.Delete(heroToDeleteId);

        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            sqlServerHeroDal.Verify(x => x.Delete(heroToDeleteId), Times.Once);
        });
    }
}
