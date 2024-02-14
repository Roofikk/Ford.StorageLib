using Ford.SaveSystem.Data;
using System;
using System.Collections.Generic;

namespace Ford.SaveSystem.Dto
{
    public class CreationHorseData
    {
        public string? Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Sex { get; set; } = null!;
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public ICollection<SaveData>? Saves { get; set; }
    }
}
