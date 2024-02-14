using Ford.SaveSystem;
using Ford.SaveSystem.Data;
using System.Collections.ObjectModel;

namespace Ford.StorageTest
{
    public class HorseTest
    {
        private Storage _storage;
        public HorseTest()
        {
            _storage = new Storage();
        }

        public ICollection<HorseData> CreateHorses(int count = 10)
        {
            Collection<HorseData> horses = new();
            DateTime now = DateTime.Now;

            for (int i = 0; i < count; i++)
            {
                var horse = _storage.CreateHorse(new()
                {
                    Name = $"horse - {i}",
                    Description = $"Horse description - {i}",
                    BirthDate = now,
                    Sex = "Male",
                    City = $"City - {i}",
                    Region = $"Region - {i}",
                    Country = "Russia",
                    Saves = CreateSaves(count)
                });

                if (horse is not null)
                {
                    horses.Add(horse);
                }
            }

            return horses;
        }

        public ICollection<SaveData> CreateSaves(int count = 10)
        {
            Collection<SaveData> saves = new();
            DateTime now = DateTime.Now;

            for (int i = 0; i < count; i++)
            {
                saves.Add(new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Header = $"Horse's save - {i}",
                    Description = $"Save description - {i}",
                    CreationDate = now,
                    LastUpdate = now,
                    SaveFileName = string.Empty,
                });
            }

            return saves;
        }
    }
}
