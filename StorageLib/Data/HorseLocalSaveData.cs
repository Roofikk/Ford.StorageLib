using System;
using System.Collections.Generic;

namespace Ford.SaveSystem
{
    public class HorseLocalSaveData
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Sex { get; set; } = null!;
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime LastUpdate { get; set; }

        public ICollection<LocalSaveData>? Saves { get; set; }
    }
}
