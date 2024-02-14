using System.Collections.Generic;

namespace Ford.SaveSystem.Data
{
    public class SaveBonesData
    {
        public string SaveId { get; set; } = null!;
        public ICollection<BoneData> Bones { get; set; } = null!;
    }
}
