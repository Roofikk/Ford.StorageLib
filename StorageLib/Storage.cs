using Ford.SaveSystem.Data;
using Ford.SaveSystem.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Ford.SaveSystem
{
    public class Storage
    {
        private string _storagePath;
        private string _savesPath;
        private readonly string _horsesFileName = "horses.json";
        private readonly string _storageSettingsFileName = "storageSettings.json";

        public Storage()
        {
            _storagePath = Path.Combine(Environment.CurrentDirectory, "storage");
            _savesPath = Path.Combine(_storagePath, "saves");

            if (!Directory.Exists(_savesPath))
            {
                Directory.CreateDirectory(_savesPath);
            }
        }

        public Storage(string storagePath)
        {
            _storagePath = storagePath;
            _savesPath = Path.Combine(_storagePath, "saves");

            if (!Directory.Exists(_savesPath))
            {
                Directory.CreateDirectory(_savesPath);
            }
        }

        #region Horse CRUD
        public ICollection<HorseData>? GetHorses()
        {
            string pathHorses = Path.Combine(_storagePath, _horsesFileName);

            if (!File.Exists(pathHorses))
            {
                return null;
            }

            ICollection<HorseData>? horseData = Array.Empty<HorseData>();

            using (FileStream fs = File.Open(pathHorses, FileMode.Open))
            using (StreamReader sr = new(fs))
            using (JsonTextReader reader = new(sr))
            {
                reader.SupportMultipleContent = true;
                var serializer = new JsonSerializer();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        horseData = serializer.Deserialize<ArraySerializable<HorseData>>(reader)?.Items;
                    }
                }
            }

            return horseData;
        }

        public HorseData? GetHorse(string id)
        {
            var horses = GetHorses();

            if (horses == null)
            {
                return null;
            }

            var findHorse = horses.FirstOrDefault(h => h.Id == id);
            return findHorse;
        }

        public HorseData? CreateHorse(CreationHorseData horseData)
        {
            ICollection<HorseData>? horses = GetHorses();
            horses ??= new Collection<HorseData>();

            if (string.IsNullOrEmpty(horseData.Id))
            {
                horseData.Id = Guid.NewGuid().ToString();
            }
            else
            {
                var horseExist = horses.FirstOrDefault(h => h.Id == horseData.Id);

                if (horseExist is not null)
                {
                    return null;
                }
            }

            HorseData addHorse = new()
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

            horses.Add(addHorse);
            RewriteHorseFile(horses);
            return addHorse;
        }

        public HorseData? UpdateHorse(CreationHorseData horseData)
        {
            var horses = GetHorses();

            if (horses is null)
            {
                return null;
            }

            HorseData? existHorse = horses.FirstOrDefault(h => h.Id == horseData.Id);

            if (existHorse is null)
            {
                return null;
            }

            //
            existHorse.Name = horseData.Name;
            existHorse.Description = horseData.Description;
            existHorse.BirthDate = horseData.BirthDate;
            existHorse.Sex = horseData.Sex;
            existHorse.City = horseData.City;
            existHorse.Region = horseData.Region;
            existHorse.Country = horseData.Country;
            existHorse.LastUpdate = DateTime.Now;

            if (horseData.Saves is not null)
            {
                existHorse.Saves = horseData.Saves;
            }
            //

            RewriteHorseFile(horses);
            return existHorse;
        }

        // не забыть удалить сохранения лошади, поскольку они не нужны.
        public bool DeleteHorse(string id)
        {
            var horses = GetHorses();

            if (horses == null)
            {
                return false;
            }

            var findHorse = horses.FirstOrDefault(h => h.Id == id);

            bool result = horses.Remove(findHorse);

            if (!result)
            {
                return false;
            }

            

            RewriteHorseFile(horses);
            return true;
        }
        #endregion

        #region Save CRUD
        public ICollection<SaveBonesData>? GetSaves(string fileName)
        {
            string savePath = Path.Combine(_savesPath, fileName);

            if (!File.Exists(savePath))
            {
                return null;
            }

            ICollection<SaveBonesData>? saves = null;

            using (StreamReader sr = new(savePath))
            using (JsonTextReader reader = new(sr))
            {
                reader.SupportMultipleContent = true;
                var serializer = new JsonSerializer();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        saves = serializer.Deserialize<ArraySerializable<SaveBonesData>>(reader)?.Items;
                    }
                }
            }

            return saves;
        }

        public SaveBonesData? GetSave(string fileName, string saveId)
        {
            List<SaveBonesData> saves = GetSaves(fileName).ToList();

            if (saves is null)
            {
                return null;
            }

            return saves.FirstOrDefault(s => s.SaveId == saveId);
        }

        public SaveBonesData? GetSave(string saveId)
        {
            SaveData saveData = GetSaveInfo(saveId);
            return GetSave(saveData.SaveFileName, saveId);
        }

        public SaveData GetSaveInfo(string saveId)
        {
            SaveData? save = null;

            //var query = (from h in GetHorses()
            //            from s in h.Saves
            //            where s.Id == saveId
            //            select s).First(s => s.Id == saveId);

            save = GetHorses()
                .SelectMany(h => h.Saves)
                .FirstOrDefault(p => p.Id == saveId);

            return save;
        }

        public SaveData? CreateSave(string horseId, CreateSaveDto saveData)
        {
            var horses = GetHorses();

            if (horses is null)
            {
                return null;
            }

            HorseData? existingHorse = horses.FirstOrDefault(h => h.Id == horseId);

            if (existingHorse is null)
            {
                return null;
            }

            string fileName = GetPathSaveFile();

            saveData.Id ??= Guid.NewGuid().ToString();
            existingHorse.Saves ??= new Collection<SaveData>();

            SaveData save = new()
            {
                Id = saveData.Id,
                Header = saveData.Header,
                Description = saveData.Description,
                Date = saveData.Date,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now,
                SaveFileName = fileName
            };

            existingHorse.Saves.Add(save);
            RewriteHorseFile(horses);

            string pathSave = Path.Combine(_storagePath, fileName);
            SaveBonesData saveBones = new()
            {
                SaveId = saveData.Id,
                Bones = saveData.Bones
            };

            var saves = GetSaves(pathSave);

            if (saves is null)
            {
                saves = [saveBones];
            }
            else
            {
                saves.Add(saveBones);
            }

            RewriteSaveBonesFile(pathSave, saves);
            return save;
        }

        public SaveData? UpdateSave(UpdateSaveDto saveData)
        {
            var horses = GetHorses();

            if (horses is null)
            {
                return null;
            }

            var save = horses.SelectMany(h => h.Saves).FirstOrDefault(s => s.Id == saveData.Id);

            if (save is null)
            {
                return null;
            }

            save.Header = saveData.Header;
            save.Description = saveData.Description;
            save.Date = saveData.Date;
            save.LastUpdate = DateTime.Now;

            RewriteHorseFile(horses);
            return save;
        }

        public void DeleteSave(string saveId)
        {

        }

        public void DeleteSaves(string horseId)
        {

        }

        private void DeleteCascadeSaves(string horseId)
        {

        }
        #endregion

        private string GetPathSaveFile()
        {
            string pathSettings = Path.Combine(_storagePath, _storageSettingsFileName);
            string fileName = Guid.NewGuid().ToString() + ".json";

            if (File.Exists(pathSettings))
            {
                using StreamReader sr = new StreamReader(pathSettings);
                using JsonReader reader = new JsonTextReader(sr);

                while (reader.Read())
                {
                    reader.SupportMultipleContent = true;
                    var serializer = new JsonSerializer();
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        var settings = serializer.Deserialize<StorageSettingsData>(reader) ?? throw new Exception("File not exists");
                        fileName = settings.LastSaveFileName;
                    }
                }

                FileInfo saveFile = new FileInfo(Path.Combine(_storagePath, fileName));
                float fileSizeMb = saveFile.Length / (1024f * 1024f);

                if (saveFile.Length < 10f)
                {
                    return fileName;
                }
            }

            using StreamWriter sw = new StreamWriter(pathSettings);
            using JsonWriter jsonWriter = new JsonTextWriter(sw);
            JsonSerializer.CreateDefault().Serialize(jsonWriter, new StorageSettingsData()
            {
                LastSaveFileName = fileName
            });

            return fileName;
        }

        private void RewriteHorseFile(ICollection<HorseData> horses)
        {
            string path = Path.Combine(_storagePath, _horsesFileName);
            using StreamWriter sw = new(path);
            using JsonWriter jsonWriter = new JsonTextWriter(sw);
            JsonSerializer.CreateDefault().Serialize(jsonWriter, new ArraySerializable<HorseData>(horses));
        }

        private void RewriteSaveBonesFile(string path, ICollection<SaveBonesData> saves)
        {
            using StreamWriter sw = new(path);
            using JsonWriter jsonWriter = new JsonTextWriter(sw);
            JsonSerializer.CreateDefault().Serialize(jsonWriter, new ArraySerializable<SaveBonesData>(saves));
        }
    }
}
