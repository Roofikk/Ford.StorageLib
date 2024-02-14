using Ford.SaveSystem.Interfaces;
using System;

namespace Ford.SaveSystem.Data
{
    public class SaveData : ISaveData
    {
        public string Id { get; set; } = null!;
        public string Header { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public string SaveFileName { get; set; } = null!;
    }
}
