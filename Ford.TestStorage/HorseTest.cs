using Ford.SaveSystem;
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

        public ICollection<HorseLocalSaveData> CreateHorses(int count = 10)
        {
            Collection<HorseLocalSaveData> horses = new();
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

        public ICollection<LocalSaveData> CreateSaves(int count = 10)
        {
            Collection<LocalSaveData> saves = new();
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
                    PathFileSave = string.Empty,
                });
            }

            return saves;
        }
    }
}
