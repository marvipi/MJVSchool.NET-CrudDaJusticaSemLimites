using CrudDaJustica.Data.Lib.Models;
using CrudDaJustica.Data.Lib.Services;
using System.Text.Json;

namespace CrudDaJustica.Data.Lib.Repositories;

/// <summary>
/// Represents a repository that stores information in a JSON file.
/// </summary>
public class JsonHeroRepository : HeroRepository
{
    public int Size { get; private set; }


    // Summary: The path of the json file where hero data is stored.
    private readonly string heroDataFilePath;

    // Summary: The path of the directory where the hero data file is stored.
    private readonly string heroDataDirPath;

    // Summary: A temporary file used to update or delete heroes from the repository.
    private string HeroDataTempFilePath => Path.Combine(heroDataDirPath, "heroTemp.json");

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonHeroRepository"/> class.
    /// </summary>
    /// <param name="pagingService"> Service responsible for paging the json file. </param>
    /// <param name="heroDataFilePath"> The absolute path where the hero data file is or will be stored. </param>
    /// <exception cref="ArgumentException"></exception>
    public JsonHeroRepository(PagingService pagingService, string heroDataFilePath) : base(pagingService)
    {
        var dirPath = Path.GetDirectoryName(heroDataFilePath);
        if (string.IsNullOrEmpty(dirPath))
        {
            var errorMsg = string.Format("{0} cannot be a root directory nor null. Given path: {1}",
                nameof(heroDataDirPath),
                heroDataDirPath);

            throw new ArgumentException(errorMsg);
        }

        heroDataDirPath = dirPath;
        if (!Directory.Exists(heroDataDirPath))
        {
            var heroDataDir = new DirectoryInfo(heroDataDirPath);
            heroDataDir.Create();
        }

        this.heroDataFilePath = heroDataFilePath;
        if (!File.Exists(this.heroDataFilePath))
        {
            var heroDataFile = new FileInfo(this.heroDataFilePath);
            heroDataFile
                .CreateText()
                .Close();
        }

        Size = File.ReadLines(this.heroDataFilePath).Count();
    }

    public override bool Register(HeroEntity newHero)
    {
        var heroAsJson = JsonSerializer.Serialize(newHero);
        using (var streamWriter = File.AppendText(heroDataFilePath))
        {
            streamWriter.WriteLine(heroAsJson);
        }
        Size++;
        return true;
    }

    public override IEnumerable<HeroEntity> Get(int page, int rows)
    {
        (var validPage, var validRows) = pagingService.Validate(page, rows, Size);

        return File.ReadLines(heroDataFilePath)
            .Skip((validPage - 1) * validRows)
            .Take(validPage * validRows)
            .Select(line => JsonSerializer.Deserialize<HeroEntity>(line))
            .Cast<HeroEntity>()
            .ToList();
    }

    public override HeroEntity? Get(Guid id) => File.ReadLines(heroDataFilePath)
                                                .Select(line => JsonSerializer.Deserialize<HeroEntity>(line))
                                                .FirstOrDefault(he => he?.Id == id, null);

    public override bool Update(Guid id, HeroEntity updatedHero) => OverwriteData(id, updatedHero);

    public override bool Delete(Guid id)
    {
        var success = OverwriteData(id);

        if (success)
        {
            Size--;
        }

        return success;
    }

    // Summary: Deletes or updates a hero whose id matches then given one.
    //			Copies all data from the current data file into a temporary one, altering it as necessary.
    //          Replaces the old data file with the temporary one, once the operation has completed.
    //
    // Remarks: If updatedInformation is null then the hero will be deleted from the file,
    //          otherwise they will be overwritten with updatedInformation.
    //          All other rows in the repository will be left untouched.
    private bool OverwriteData(Guid id, HeroEntity? updatedInformation = null)
    {
        var currentLine = 0;
        var dataRow = string.Empty;
        var fileChanged = false;

        using (var streamReader = new StreamReader(heroDataFilePath))
        {
            using var streamWriter = new StreamWriter(HeroDataTempFilePath);
            while ((dataRow = streamReader.ReadLine()) is not null)
            {
                var currentHero = JsonSerializer.Deserialize<HeroEntity>(dataRow);

                if (currentHero?.Id != id)
                {
                    streamWriter.WriteLine(dataRow);
                }
                else if (updatedInformation is null)
                {
                    fileChanged = true; // Delete
                }
                else if (updatedInformation is not null)
                {
                    var updatedHero = new HeroEntity(
                        id,
                        updatedInformation.Alias,
                        updatedInformation.Debut,
                        updatedInformation.FirstName,
                        updatedInformation.LastName);

                    var updatedHeroAsJson = JsonSerializer.Serialize(updatedHero);
                    streamWriter.WriteLine(updatedHeroAsJson); // Update

                    fileChanged = true;
                }

                currentLine++;
            }
        }

        File.Move(HeroDataTempFilePath, heroDataFilePath, true);
        return fileChanged;
    }
}