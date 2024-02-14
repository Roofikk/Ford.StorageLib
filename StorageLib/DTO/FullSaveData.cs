using Ford.SaveSystem.Data;
using System.Collections.Generic;

namespace Ford.SaveSystem.Dto
{
    public class FullSaveData : CreateSaveDto
    {
        public string FileName { get; set; } = null!;
        public ICollection<BoneData> Bones { get; set; } = null!;
    }
}
