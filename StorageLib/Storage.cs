using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ford.SaveSystem
{
    public class Storage
    {
        private string _pathSave;
        private readonly string horsesFileName = "horses.json";

        public Storage()
        {
            _pathSave = Path.Combine(Environment.CurrentDirectory, "Saves");

            if (!Directory.Exists(_pathSave))
            {
                Directory.CreateDirectory(_pathSave);
            }
        }

        public Storage(string pathSave)
        {
            _pathSave = pathSave;

            if (!Directory.Exists(_pathSave))
            {
                Directory.CreateDirectory(_pathSave);
            }
        }

        #region Horse CRUD
        // переписать метод с использованием частичного считывани€, а не всего файла.
        public HorseLocalSaveData? GetHorse(string id)
        {
            string pathHorses = Path.Combine(_pathSave, horsesFileName);

            if (!FileIsExists(pathHorses))
            {
                return null;
            }

            StreamReader sr = new(pathHorses);
            HorseLocalSaveData? horseData = null;

            using (JsonTextReader reader = new(sr))
            {
                reader.SupportMultipleContent = true;
                var serializer = new JsonSerializer();
                while (reader.Read())
                {
                    horseData = serializer.Deserialize<HorseLocalSaveData>(reader);

                    if (horseData is not null)
                    {
                        if (horseData.Id == id)
                        {
                            break;
                        }
                    }
                }
            }

            sr.Dispose();
            return horseData;
        }

        public IEnumerable<HorseLocalSaveData>? GetHorses()
        {
            string pathHorses = Path.Combine(_pathSave, horsesFileName);

            if (!FileIsExists(pathHorses))
            {
                return null;
            }

            string json = "";

            using (StreamReader sr = new StreamReader(pathHorses))
            {
                json = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<ArraySerializable<HorseLocalSaveData>>(json).Items;
        }

        // надо помимо добавлени€ проверить уже наличие этого объекта в файле.
        // а как быстро пробежатьс€ по файлу, чтобы не выт€гивать целый объект и не провер€ть его -- вопрос
        // также по хорошему не перезаписывать весь файл, а добавить в него json строку.
        public HorseLocalSaveData? CreateHorse(CreationHorseData horseData)
        {
            if (string.IsNullOrEmpty(horseData.Id))
            {
                horseData.Id = Guid.NewGuid().ToString();
            }

            var horses = GetHorses();
            List<HorseLocalSaveData> horseList = new();

            if (horses is not null)
            {
                horseList = horses.ToList();
            }

            HorseLocalSaveData addHorse = new()
            {
                Id = horseData.Id,
                Name = horseData.Name,
                Description = horseData.Description,
                BirthDate = horseData.BirthDate,
                Sex = horseData.Sex,
                City = horseData.City,
                Region = horseData.Region,
                Country = horseData.Country,
                DateCreation = DateTime.Now,
                LastUpdate = DateTime.Now,
                Saves = horseData.Saves is null ? null : horseData.Saves
            };

            horseList.Add(addHorse);
            RewriteHorseFile(horseList, Path.Combine(_pathSave, horsesFileName));
            return addHorse;
        }

        public HorseLocalSaveData? UpdateHorse(CreationHorseData horseData)
        {
            var horses = GetHorses();
            List<HorseLocalSaveData> horseList = new();

            if (horses is null)
            {
                return null;
            }

            horseList = horses.ToList();
            int existHorseIndex = horseList.FindIndex(h => h.Id == horseData.Id);

            if (existHorseIndex < 0)
            {
                return null;
            }

            var existHorse = horseList[existHorseIndex];
            horseList.RemoveAt(existHorseIndex);

            //
            existHorse.Name = horseData.Name;
            existHorse.Description = horseData.Description;
            existHorse.BirthDate = horseData.BirthDate;
            existHorse.Sex = horseData.Sex;
            existHorse.City = horseData.City;
            existHorse.Region = horseData.Region;
            existHorse.Country = horseData.Country;
            existHorse.LastUpdate = DateTime.Now;
            //

            horseList.Insert(existHorseIndex, existHorse);

            RewriteHorseFile(horseList, Path.Combine(_pathSave, horsesFileName));

            return existHorse;
        }

        public bool DeleteHorse(string id)
        {
            var horses = GetHorses();

            if (horses == null)
            {
                return false;
            }

            var horseList = horses.ToList();
            var findHorse = horseList.FirstOrDefault(h => h.Id == id);
            bool result = horseList.Remove(findHorse);

            if (!result)
            {
                return false;
            }

            RewriteHorseFile(horseList, Path.Combine(_pathSave, horsesFileName));

            return true;
        }
        #endregion

        private bool FileIsExists(string path) => File.Exists(path);

        private void RewriteHorseFile(List<HorseLocalSaveData> horses, string path)
        {
            using StreamWriter sw = new(path);
            using JsonWriter jsonWriter = new JsonTextWriter(sw);
            JsonSerializer.CreateDefault().Serialize(jsonWriter, new ArraySerializable<HorseLocalSaveData>(horses));
        }
    }
}
