using CrudDaJustica.Data.Lib.Models;
using CrudDaJustica.Data.Lib.Repositories;
using CrudDaJustica.Data.Lib.Services;

namespace CrudDaJustica.Data.Lib.Test.RepositoriesTests;

[TestFixture]
internal class VirtualHeroRepositoryTest
{
    [Test]
    public void RegisterAndGet_EmptyRepo_AddsAndRetrievesHero()
    {
        var pagingService = new PagingService();
        var heroRepo = new VirtualHeroRepository(pagingService, initialSize: 0);

        var newHeroId = Guid.NewGuid();
        var newHero = new HeroEntity(newHeroId, "Specter", new(1940, 2, 1), "Jim", "Corrigan");
        var success = heroRepo.Register(newHero);
        var heroInRepo = heroRepo.Get(newHeroId);

        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(heroInRepo, Is.EqualTo(newHero));
        });
    }

    [Test]
    public void Get_IdIsntRegistered_ReturnsNull()
    {
        var pagingService = new PagingService();
        var heroRepo = new VirtualHeroRepository(pagingService, PagingService.MIN_ROWS_PER_PAGE);
        var newHero = new HeroEntity(Guid.NewGuid(), "Doesn't matter", new(1, 1, 1), "Doesn't matter", "Doesn't matter");
        heroRepo.Register(newHero);

        var nonExistentId = Guid.NewGuid();
        var returnedHero = heroRepo.Get(nonExistentId);

        Assert.That(returnedHero, Is.Null);
    }

    [TestCase(1, 10, (uint)10, 10)]
    [TestCase(2, 15, (uint)30, 15)]
    [TestCase(1, 100, (uint)25, 25)]
    public void Get_PageIsNotEmpty_ReturnsACollectionOfHeroEntity(int page, int rows, uint initialSize, int expectedCount)
    {
        var pagingService = new PagingService();
        var heroRepo = InitializeHeroRepository(pagingService, initialSize);

        var heroesInPage = heroRepo.Get(page, rows);

        Assert.That(heroesInPage.Count(), Is.EqualTo(expectedCount));
    }

    [Test]
    public void Get_PageIsEmpty_ReturnsAnEmptyCollectionOfHeroEntity()
    {
        var pagingService = new PagingService();
        var heroRepo = new VirtualHeroRepository(pagingService, PagingService.MIN_ROWS_PER_PAGE);

        var heroesInPage = heroRepo.Get(1, 10);

        Assert.That(heroesInPage, Is.Empty);
    }

    [Test]
    public void Update_IdIsRegistered_UpdatesTheHeroWithAMatchingId()
    {
        var pagingService = new PagingService();
        var heroRepo = new VirtualHeroRepository(pagingService, PagingService.MIN_ROWS_PER_PAGE);

        var heroToUpdateId = Guid.NewGuid();
        var heroToUpdate = new HeroEntity(heroToUpdateId, "Supperguy", new(1844, 5, 8), "Clerk", "Kurt");
        heroRepo.Register(heroToUpdate);
        var updatedHero = new HeroEntity(heroToUpdateId, "Superman", new(1938, 6, 1), "Clark", "Kent");
        var success = heroRepo.Update(updatedHero);

        var heroInRepo = heroRepo.Get(heroToUpdateId);
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(heroInRepo, Is.EqualTo(updatedHero));
        });
    }

    [Test]
    public void Update_IdIsRegistered_DoesntChangeAnyOtherHero()
    {
        var pagingService = new PagingService();
        var heroRepo = InitializeHeroRepository(pagingService, PagingService.MIN_ROWS_PER_PAGE);
        var getHeroes = () => heroRepo.Get(pagingService.CurrentPage, pagingService.RowsPerPage);
        var heroesBeforeUpdate = getHeroes.Invoke();

        var heroToUpdate = heroesBeforeUpdate.Last();
        var updatedHero = new HeroEntity(heroToUpdate.Id, "Wonder Woman", new(1, 1, 1), "Diana", "of Themyscira");
        heroRepo.Update(updatedHero);

        var otherHeroesBeforeUpdate = heroesBeforeUpdate
            .Where(hero => hero.Id != heroToUpdate.Id);
        var otherHeroesAfterUpdate = getHeroes
            .Invoke()
            .Where(hero => hero.Id != updatedHero.Id);
        Assert.That(otherHeroesAfterUpdate, Is.EquivalentTo(otherHeroesBeforeUpdate));
    }

    [Test]
    public void Update_IdIsntRegistered_DoesntChangeAnyHero()
    {
        var pagingService = new PagingService();
        var heroRepo = new VirtualHeroRepository(pagingService, PagingService.MIN_ROWS_PER_PAGE);

        var nonExistentId = Guid.NewGuid();
        var updatedHero = new HeroEntity(nonExistentId, "Doesn't matter", new(1, 1, 1), "Doesn't matter", "Doesn't matter");
        var success = heroRepo.Update(updatedHero);

        var getResult = heroRepo.Get(nonExistentId);
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(getResult, Is.Null);
        });
    }

    [Test]
    public void Delete_IdIsRegistered_DeletesHeroWithAMatchingId()
    {
        var pagingService = new PagingService();
        var heroRepo = new VirtualHeroRepository(pagingService, PagingService.MIN_ROWS_PER_PAGE);

        var heroToDeleteId = Guid.NewGuid();
        var heroToDelete = new HeroEntity(heroToDeleteId, "Batman", new(1939, 5, 1), "Bruce", "Wayne");
        heroRepo.Register(heroToDelete);
        var success = heroRepo.Delete(heroToDeleteId);

        var getReturn = heroRepo.Get(heroToDeleteId);
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(getReturn, Is.Null);
        });
    }

    [Test]
    public void Delete_IdIsRegistered_DoesntDeleteAnyOtherHero()
    {
        var pagingService = new PagingService();
        var heroRepo = new VirtualHeroRepository(pagingService, PagingService.MIN_ROWS_PER_PAGE);
        var getHeroes = () => heroRepo.Get(pagingService.CurrentPage, pagingService.RowsPerPage);
        var otherHeroesBeforeDeletion = getHeroes.Invoke();

        var heroToDeleteId = Guid.NewGuid();
        var heroToDelete = new HeroEntity(heroToDeleteId, "Zatara", new(1938, 6, 1), "Giovanni", "Zatara");
        heroRepo.Register(heroToDelete);
        heroRepo.Delete(heroToDeleteId);

        otherHeroesBeforeDeletion = otherHeroesBeforeDeletion
            .Where(hero => hero.Id != heroToDeleteId);
        var otherHeroesAfterDeletion = getHeroes
            .Invoke()
            .Where(hero => hero.Id != heroToDeleteId);
        Assert.That(otherHeroesAfterDeletion, Is.EquivalentTo(otherHeroesBeforeDeletion));
    }

    [Test]
    public void Delete_IdIsntRegistered_DoesntDeleteAnyHero()
    {
        var pagingService = new PagingService();
        var heroRepo = InitializeHeroRepository(pagingService, PagingService.MIN_ROWS_PER_PAGE);
        var heroCount = () => heroRepo
            .Get(pagingService.CurrentPage, pagingService.RowsPerPage)
            .Count();

        var heroCountBeforeDeletion = heroCount.Invoke();
        var nonExistentId = Guid.Empty;
        var success = heroRepo.Delete(nonExistentId);
        var heroCountAfterDeletion = heroCount.Invoke();

        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(heroCountAfterDeletion, Is.EqualTo(heroCountBeforeDeletion));
        });
    }

    private static VirtualHeroRepository InitializeHeroRepository(PagingService pagingService, uint initialSize)
    {
        var heroRepo = new VirtualHeroRepository(pagingService, initialSize);
        HeroEntity newHero;

        DateOnly debut = DateOnly.FromDateTime(DateTime.Now);
        for (int index = 0; index < initialSize; index++)
        {
            newHero = new(Guid.NewGuid(),
                          "Alias " + index,
                          debut.AddDays(1),
                          "First name " + index,
                          "Last name " + index);
            heroRepo.Register(newHero);
        }

        return heroRepo;
    }
}
