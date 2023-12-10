using CrudDaJustica.Data.Lib.Models;
using CrudDaJustica.Data.Lib.Repositories;
using CrudDaJustica.Data.Lib.Services;

namespace CrudDaJustica.Data.Lib.Test.RepositoriesTests;

[TestFixture]
internal class JsonHeroRepositoryTest
{
    private static string HeroDataDirPath => Path.Combine(TestContext.CurrentContext.WorkDirectory, "CRUD da Justica");
    private static string HeroDataFilePath => Path.Combine(HeroDataDirPath, "heroData.json");

    [Test]
    public void RegisterAndGet_EmptyRepo_AddsAndRetrievesHero()
    {
        try
        {
            var heroRepo = InitializeHeroRepo(0);

            var newHeroId = Guid.NewGuid();
            var newHero = new HeroEntity(newHeroId, "Green Lantern", new(1959, 10, 1), "Harold", "Jordan");
            var success = heroRepo.Register(newHero);
            var getResult = heroRepo.Get(newHeroId);

            Assert.Multiple(() =>
            {
                Assert.That(success, Is.True);
                Assert.That(getResult, Is.EqualTo(newHero));
            });
        }
        finally
        {
            DeleteTestDirectory();
        }
    }

    [Test]
    public void Get_IdIsntRegistered_ReturnsNull()
    {
        try
        {
            var heroRepo = InitializeHeroRepo(0);

            var nonExistentId = Guid.NewGuid();
            var getResult = heroRepo.Get(nonExistentId);

            Assert.That(getResult, Is.Null);
        }
        finally
        {
            DeleteTestDirectory();
        }
    }

    [TestCase(1, 5, (uint)5, 5)]
    [TestCase(2, 20, (uint)30, 10)]
    [TestCase(3, 50, (uint)150, 50)]
    public void Get_PageIsNotEmpty_ReturnsACollectionOfHeroEntity(int page, int rows, uint amount, int expectedCount)
    {
        try
        {
            var heroRepo = InitializeHeroRepo(amount);

            var heroesInPage = heroRepo.Get(page, rows);

            Assert.That(heroesInPage.Count, Is.EqualTo(expectedCount));
        }
        finally
        {
            DeleteTestDirectory();
        }
    }

    [Test]
    public void Get_PageIsEmpty_ReturnsAnEmptyCollectionOfHeroEntity()
    {
        try
        {
            var heroRepo = InitializeHeroRepo(0);

            var heroesInPage = heroRepo.Get(page: 1, rows: 1);

            Assert.That(heroesInPage, Is.Empty);
        }
        finally
        {
            DeleteTestDirectory();
        }
    }

    [TestCase((uint)1, (uint)10)]
    [TestCase((uint)5, (uint)25)]
    [TestCase((uint)20, (uint)20)]
    public void Update_IdIsRegistered_UpdatesTheHeroWithAMatchingId(uint line, uint amount)
    {
        try
        {
            var heroRepo = InitializeHeroRepo(amount);
            var heroToUpdateId = heroRepo
                .Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE)
                .ElementAt((int)line - 1)
                .Id;

            var updatedHero = new HeroEntity(heroToUpdateId, "Martian Manhunter", new(1955, 11, 1), "J'onn", "J'onzz");
            var success = heroRepo.Update(updatedHero);

            var getResult = heroRepo.Get(heroToUpdateId);
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.True);
                Assert.That(getResult, Is.EqualTo(updatedHero));
            });
        }
        finally
        {
            DeleteTestDirectory();
        }
    }

    [TestCase((uint)1, (uint)3)]
    [TestCase((uint)2, (uint)4)]
    [TestCase((uint)5, (uint)5)]
    public void Update_IdIsRegistered_DoesntChangeAnyOtherHero(uint line, uint amount)
    {
        try
        {
            var heroRepo = InitializeHeroRepo(amount);
            var getHeroes = () => heroRepo.Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE);
            var heroToUpdate = getHeroes.Invoke().ElementAt((int)line - 1);
            var otherHeroesBeforeUpdate = getHeroes
                .Invoke()
                .Where(hero => hero.Id != heroToUpdate.Id);

            var updatedHero = new HeroEntity(heroToUpdate.Id, "Ultra-Man", new(1939, 11, 1), "Gary", "Concord");
            heroRepo.Update(updatedHero);

            var otherHeroesAfterUpdate = getHeroes
                .Invoke()
                .Where(hero => hero.Id != heroToUpdate.Id);
            Assert.That(otherHeroesAfterUpdate, Is.EquivalentTo(otherHeroesBeforeUpdate));
        }
        finally
        {
            DeleteTestDirectory();
        }
    }

    [Test]
    public void Update_IdIsntRegistered_DoesntChangeAnyHero()
    {
        try
        {
            var heroRepo = InitializeHeroRepo(amount: 20);
            var getHeroes = () => heroRepo.Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE);
            var heroesBeforeUpdate = getHeroes.Invoke();

            var unregisteredHero = new HeroEntity(Guid.NewGuid(), "Doctor Occult", new(1935, 10, 1), "Richard", "Occult");
            var success = heroRepo.Update(unregisteredHero);

            var heroesAfterUpdate = getHeroes.Invoke();
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.False);
                Assert.That(heroesBeforeUpdate, Is.EquivalentTo(heroesAfterUpdate));
            });
        }
        finally
        {
            DeleteTestDirectory();
        }
    }

    [TestCase((uint)1, (uint)5)]
    [TestCase((uint)2, (uint)3)]
    [TestCase((uint)10, (uint)10)]
    public void Delete_IdIsRegistered_DeletesHeroWithAMatchingId(uint line, uint amount)
    {
        try
        {
            var heroRepo = InitializeHeroRepo(amount);
            var heroToDeleteId = heroRepo
                .Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE)
                .ElementAt((int)line - 1)
                .Id;

            var success = heroRepo.Delete(heroToDeleteId);

            var getResult = heroRepo.Get(heroToDeleteId);
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.True);
                Assert.That(getResult, Is.Null);
            });
        }
        finally
        {
            DeleteTestDirectory();
        }
    }

    [TestCase((uint)1, (uint)2)]
    [TestCase((uint)5, (uint)11)]
    [TestCase((uint)27, (uint)27)]
    public void Delete_IdIsRegistered_DoesntDeleteAnyOtherHero(uint line, uint amount)
    {
        try
        {
            var heroRepo = InitializeHeroRepo(amount);
            var getHeroes = () => heroRepo.Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE);
            var heroToDeleteId = getHeroes
                .Invoke()
                .ElementAt((int)line - 1).Id;
            var otherHeroesBeforeDeletion = getHeroes
                .Invoke()
                .Where(hero => hero.Id != heroToDeleteId);

            heroRepo.Delete(heroToDeleteId);

            var otherHeroesAfterDeletion = getHeroes
                .Invoke()
                .Where(hero => hero.Id != heroToDeleteId);
            Assert.That(otherHeroesAfterDeletion, Is.EquivalentTo(otherHeroesBeforeDeletion));
        }
        finally
        {
            DeleteTestDirectory();
        }
    }

    [Test]
    public void Delete_IdIsntRegistered_DoesntDeleteAnyHero()
    {
        try
        {
            const int EXPECTED_COUNT = 20;
            var heroRepo = InitializeHeroRepo(EXPECTED_COUNT);
            var nonExistentId = Guid.NewGuid();

            var success = heroRepo.Delete(nonExistentId);

            var countAfterDeletion = heroRepo
                .Get(PagingService.FIRST_PAGE, PagingService.MAX_ROWS_PER_PAGE)
                .Count();
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.False);
                Assert.That(countAfterDeletion, Is.EqualTo(EXPECTED_COUNT));
            });
        }
        finally
        {
            DeleteTestDirectory();
        }
    }

    private JsonHeroRepository InitializeHeroRepo(uint amount)
    {
        var pagingService = new PagingService();
        var heroRepo = new JsonHeroRepository(pagingService, HeroDataFilePath);

        HeroEntity newHero;
        DateOnly debut = DateOnly.FromDateTime(DateTime.Now);

        for (int line = 0; line < amount; line++)
        {
            newHero = new HeroEntity(Guid.NewGuid(),
                "Alias " + line,
                debut.AddDays(line),
                "First name " + line,
                "Last name " + line);

            heroRepo.Register(newHero);
        }

        return heroRepo;
    }

    private void DeleteTestDirectory() => Directory.Delete(HeroDataDirPath, true);
}
