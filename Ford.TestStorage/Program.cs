// See https://aka.ms/new-console-template for more information
using Ford.SaveSystem;
using Ford.SaveSystem.Data;
using Ford.StorageTest;
using Newtonsoft.Json;
using System.Diagnostics;

Storage storage = new();

HorseData? horse = storage.CreateHorse(new()
{
    Id = Guid.NewGuid().ToString(),
    Name = "Ford",
    BirthDate = DateTime.Now,
    Description = "Description",
    City = "There",
    Sex = "Male"
});

for (int i = 0; i < 10; i++)
{
    storage.CreateSave(horse!.Id, new()
    {
        Id = Guid.NewGuid().ToString(),
        Header = $"Horse name - {horse!.Name}, save - {i}",
        Description = "Description",
        Bones = [],
        Date = DateTime.Now
    });
}

HorseData? horse1 = storage.CreateHorse(new()
{
    Id = Guid.NewGuid().ToString(),
    Name = "Ford2",
    BirthDate = DateTime.Now,
    Description = "Description2",
    City = "There2",
    Sex = "Female"
});

for (int i = 0; i < 10; i++)
{
    storage.CreateSave(horse1!.Id, new()
    {
        Id = Guid.NewGuid().ToString(),
        Header = $"Horse name - {horse1!.Name}, save - {i}",
        Description = "Description",
        Bones = [],
        Date = DateTime.Now
    });
}

storage.DeleteHorse(horse!.Id);

static void Create500Horse()
{
    Console.WriteLine("Begining create 500 horses and 500 saves");
    HorseTest ht = new();
    ht.CreateHorses(500);
    Console.WriteLine("Finish");
}

static void GetHorseTest(string id)
{
    string path = Path.Combine(Environment.CurrentDirectory, "Saves");
    Storage storage = new Storage(path);
    var horse = storage.GetHorse(id);

    if (horse is null)
    {
        Console.WriteLine("Horse not found");
    }
    else
    {
        Console.WriteLine(JsonConvert.SerializeObject(horse, Formatting.Indented));
    }
}

static void UpdateHorseTest()
{
    string path = Path.Combine(Environment.CurrentDirectory, "Saves");
    Storage storage = new(path);
    Stopwatch stopwatch = Stopwatch.StartNew();

    var createHorse = storage.CreateHorse(new()
    {
        Name = "Rename horse",
        Description = "Rename description",
        Sex = "Male",
        City = "New City",
        Region = "New Region",
        Country = "Russia",
        BirthDate = DateTime.Now
    });
    stopwatch.Stop();

    if (createHorse is null)
    {
        Console.WriteLine("Create horse failed");
    }
    else
    {
        Console.WriteLine("Create horse success");
    }

    Console.WriteLine($"Creation horse time spent: {stopwatch.Elapsed.TotalMilliseconds} ms");
}

static void CreateHorseTest()
{
    string path = Path.Combine(Environment.CurrentDirectory, "Saves");
    Storage storage = new(path);
    Stopwatch stopwatch = Stopwatch.StartNew();

    stopwatch.Start();
    var updateHorse = storage.UpdateHorse(new()
    {
        Id = "6cc7956f-8b5c-4f68-a8b7-631b12abe19c",
        Name = "Rename horse",
        Description = "Rename description",
        Sex = "Male",
        City = "New City",
        Region = "New Region",
        Country = "Russia",
        BirthDate = DateTime.Now
    });
    stopwatch.Stop();

    if (updateHorse is null)
    {
        Console.WriteLine("Update horse failed");
    }
    else
    {
        Console.WriteLine("Update horse success");
    }

    Console.WriteLine($"Updating horse time spent: {stopwatch.Elapsed.TotalMilliseconds} ms");
}

static bool DeleteHorseTest(string id)
{
    string path = Path.Combine(Environment.CurrentDirectory, "Saves");
    Storage storage = new Storage(path);

    Stopwatch stopwatch = Stopwatch.StartNew();

    bool result = storage.DeleteHorse(id);
    stopwatch.Stop();

    Console.WriteLine($"Time spent: {stopwatch.Elapsed.TotalMilliseconds} ms");
    
    if (result)
    {
        Console.WriteLine($"Remove success.");
    }
    else
    {
        Console.WriteLine($"Horse (id: {id}) not found. Remove failed");
    }

    return result;
}