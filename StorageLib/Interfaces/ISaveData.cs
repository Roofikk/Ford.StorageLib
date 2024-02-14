using System;

namespace Ford.SaveSystem.Interfaces
{
    internal interface ISaveData
    {
        public string Id { get; set; }
        public string Header { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
