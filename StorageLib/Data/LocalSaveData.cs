using System;

namespace Ford.SaveSystem
{
    public class LocalSaveData
    {
        public string Id { get; set; } = null!;
        public string Header { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate{ get; set; }
        public string PathFileSave { get; set; } = null!;
    }
}
