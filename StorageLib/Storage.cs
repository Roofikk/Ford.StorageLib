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

        private List<HorseData> _horses;

        public Storage()
        {
            _horses = new();
            _storagePath = Path.Combine(Environment.CurrentDirectory, "storage");
            _savesPath = Path.Combine(_storagePath, "saves");

            if (!Directory.Exists(_savesPath))
            {
                Directory.CreateDirectory(_savesPath);
            }
        }

        public Storage(string storagePath)
        {
            _horses = new();
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
            if (_horses.Count > 0)
            {
                return _horses;
            }

            string pathHorses = Path.Combine(_storagePath, _horsesFileName);

            if (!File.Exists(pathHorses))
            {
                return null;
            }

            ICollection<HorseData>? horseData = GetSerializableArrayFromFile<HorseData>(pathHorses);
            using (StreamReader sr = new(pathHorses))
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

            _horses = horseData.ToList();
            return _horses;
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
            var query = findHorse.Saves.GroupBy(s => s.SaveFileName).Select(q => new { FileName = q.Key, Ids = q.Select(id => id.Id)});

            foreach (var path in query)
            {
                DeleteSaves(Path.Combine(_savesPath, path.FileName), path.Ids.ToArray());
            }

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

            return GetSerializableArrayFromFile<SaveBonesData>(savePath);
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

            string fileName = GetSaveFileName();

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

            string pathSave = Path.Combine(_savesPath, fileName);
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

        public bool DeleteSave(string horseId, string saveId)
        {
            var horses = GetHorses();

            if (horses is null)
            {
                return false;
            }

            var savesInfo = horses.FirstOrDefault(h => h.Id == horseId).Saves;
            var saveInfo = savesInfo.FirstOrDefault(s => s.Id == saveId);
            var saves = GetSaves(saveInfo.SaveFileName);

            if (saves is null)
            {
                return false;
            }

            var saveData = saves.FirstOrDefault(s => s.SaveId == saveId);

            saves.Remove(saveData);
            RewriteSaveBonesFile(saveInfo.SaveFileName, saves);

            savesInfo!.Remove(saveInfo);
            RewriteHorseFile(horses);

            return true;
        }

        private void DeleteSaves(string pathSave, string[] saveIds)
        {
            var saves = GetSerializableArrayFromFile<SaveBonesData>(pathSave).ToList() ?? throw new Exception($"Saves not serialized is {pathSave} file");

            saves.RemoveAll(s => saveIds.Contains(s.SaveId));
            RewriteSaveBonesFile(pathSave, saves);
        }
        #endregion

        private string GetSaveFileName()
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

                FileInfo saveFile = new FileInfo(Path.Combine(_savesPath, fileName));
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

        private ICollection<T>? GetSerializableArrayFromFile<T>(string path)
        {
            ICollection<T>? collection = null;

            using (StreamReader sr = new(path))
            using (JsonTextReader reader = new(sr))
            {
                reader.SupportMultipleContent = true;
                var serializer = new JsonSerializer();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        collection = serializer.Deserialize<ArraySerializable<T>>(reader)?.Items;
                    }
                }
            }

            return collection;
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
            if (saves.Count() == 0)
            {
                File.Delete(path);
                return;
            }

            using StreamWriter sw = new(path);
            using JsonWriter jsonWriter = new JsonTextWriter(sw);
            JsonSerializer.CreateDefault().Serialize(jsonWriter, new ArraySerializable<SaveBonesData>(saves));
        }
    }
}
