// See https://aka.ms/new-console-template for more information
using Ford.SaveSystem;
using Ford.StorageTest;
using System.Diagnostics;

string path = Path.Combine(Environment.CurrentDirectory, "Saves");
HorseTest ht = new();
Storage storage = new Storage(path);

Console.WriteLine("Start getting horses");
Stopwatch stopwatch = Stopwatch.StartNew();

ht.CreateHorses(1);
storage.UpdateHorse(new()
{
    Id = "f59b3982-890a-4e78-9ad4-ba3990911c3e",
    Name = "Re",
});

stopwatch.Stop();
Console.WriteLine($"Time spent: {stopwatch.Elapsed.TotalMilliseconds} ms");
stopwatch.Reset();

Console.ReadKey();