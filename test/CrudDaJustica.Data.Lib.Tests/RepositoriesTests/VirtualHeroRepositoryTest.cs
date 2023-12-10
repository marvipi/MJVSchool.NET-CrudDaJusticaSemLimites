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
        var heroRepo = InitializeHeroRepository(PagingService.MIN_ROWS_PER_PAGE);

        var nonExistentId = Guid.NewGuid();

        var returnedHero = heroRepo.Get(nonExistentId);
        Assert.That(returnedHero, Is.Null);
    }

    [TestCase(1, 10, (uint)10, 10)]
    [TestCase(2, 15, (uint)30, 15)]
    [TestCase(1, 100, (uint)25, 25)]
    public void Get_PageIsNotEmpty_ReturnsACollectionOfHeroEntity(int page, int rows, uint initialSize, int expectedCount)
    {
        var heroRepo = InitializeHeroRepository(initialSize);

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

    [TestCase(1, (uint) 10)]
    [TestCase(6, (uint) 11)]
    [TestCase(12, (uint) 12)]
    public void Update_IdIsRegistered_UpdatesTheHeroWithAMatchingId(int element, uint initialSize)
    {
        var heroRepo = InitializeHeroRepository(initialSize);
        var heroToUpdateId = heroRepo
            .Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE)
            .ElementAt(element - 1).Id;

        var updatedHero = new HeroEntity(heroToUpdateId, "Superman", new(1938, 6, 1), "Clark", "Kent");
        var success = heroRepo.Update(updatedHero);

        var heroInRepo = heroRepo.Get(heroToUpdateId);
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(heroInRepo, Is.EqualTo(updatedHero));
        });
    }

    [TestCase(1, (uint)3)]
    [TestCase(2, (uint)3)]
    [TestCase(3, (uint)3)]
    public void Update_IdIsRegistered_DoesntChangeAnyOtherHero(int element, uint initialSize)
    {
        var heroRepo = InitializeHeroRepository(initialSize);
        var getHeroes = () => heroRepo.Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE);
        var heroToUpdate = getHeroes.Invoke().ElementAt(element - 1);
        var otherHeroesBeforeUpdate = getHeroes
            .Invoke()
            .Where(hero => hero.Id != heroToUpdate.Id);

        var updatedHero = new HeroEntity(heroToUpdate.Id, "Wonder Woman", new(1, 1, 1), "Diana", "of Themyscira");
        heroRepo.Update(updatedHero);

        var otherHeroesAfterUpdate = getHeroes
            .Invoke()
            .Where(hero => hero.Id != updatedHero.Id);
        Assert.That(otherHeroesAfterUpdate, Is.EquivalentTo(otherHeroesBeforeUpdate));
    }

    [Test]
    public void Update_IdIsntRegistered_DoesntChangeAnyHero()
    {
        var heroRepo = InitializeHeroRepository(PagingService.MIN_ROWS_PER_PAGE);
        var getHeroes = () => heroRepo.Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE);
        var heroesBeforeUpdate = getHeroes.Invoke();

        var nonExistentId = Guid.NewGuid();
        var updatedHero = new HeroEntity(nonExistentId, "Doesn't matter", new(1, 1, 1), "Doesn't matter", "Doesn't matter");
        var success = heroRepo.Update(updatedHero);

        var heroesAfterUpdate = getHeroes.Invoke();
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(heroesAfterUpdate, Is.EquivalentTo(heroesBeforeUpdate));
        });
    }

    [TestCase(1, (uint)4)]
    [TestCase(5, (uint)10)]
    [TestCase(6, (uint)6)]
    public void Delete_IdIsRegistered_DeletesHeroWithAMatchingId(int element, uint initialSize)
    {
        var heroRepo = InitializeHeroRepository(initialSize);
        var heroToDeleteId = heroRepo
            .Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE)
            .ElementAt(element - 1)
            .Id;

        var success = heroRepo.Delete(heroToDeleteId);

        var getReturn = heroRepo.Get(heroToDeleteId);
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(getReturn, Is.Null);
        });
    }

    [TestCase(1, (uint)3)]
    [TestCase(2, (uint)6)]
    [TestCase(7, (uint)7)]
    public void Delete_IdIsRegistered_DoesntDeleteAnyOtherHero(int element, uint initialSize)
    {
        var heroRepo = InitializeHeroRepository(initialSize);
        var countHeroes = () => heroRepo.Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE);
        var heroToDeleteId = countHeroes
            .Invoke()
            .ElementAt(element - 1).Id;
        var otherHeroesBeforeDeletion = countHeroes
            .Invoke()
            .Where(hero => hero.Id != heroToDeleteId);

        heroRepo.Delete(heroToDeleteId);

        var otherHeroesAfterDeletion = countHeroes
            .Invoke()
            .Where(hero => hero.Id != heroToDeleteId);
        Assert.That(otherHeroesAfterDeletion, Is.EquivalentTo(otherHeroesBeforeDeletion));
    }

    [Test]
    public void Delete_IdIsntRegistered_DoesntDeleteAnyHero()
    {
        const int INITIAL_SIZE = 25;
        var heroRepo = InitializeHeroRepository(INITIAL_SIZE);
        var nonExistentId = Guid.NewGuid();

        var success = heroRepo.Delete(nonExistentId);

        var heroCountAfterDeletion = heroRepo
            .Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE)
            .Count();
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(heroCountAfterDeletion, Is.EqualTo(INITIAL_SIZE));
        });
    }

    private static VirtualHeroRepository InitializeHeroRepository(uint initialSize)
    {
        var pagingService = new PagingService();
        var heroRepo = new VirtualHeroRepository(pagingService, initialSize);
        HeroEntity newHero;

        DateOnly debut = DateOnly.FromDateTime(DateTime.Now);
        for (int index = 0; index < initialSize; index++)
        {
            newHero = new(Guid.NewGuid(),
                          "Alias " + index,
                          debut.AddDays(index),
                          "First name " + index,
                          "Last name " + index);
            heroRepo.Register(newHero);
        }

        return heroRepo;
    }
}
