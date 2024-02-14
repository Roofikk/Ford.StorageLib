using Ford.SaveSystem.Data;
using System;
using System.Collections.Generic;

namespace Ford.SaveSystem.Dto
{
    public class CreateSaveDto
    {
        public string Id { get; set; } = null!;
        public string Header { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? Date { get; set; }

        public ICollection<BoneData> Bones { get; set; } = null!;
    }
}
