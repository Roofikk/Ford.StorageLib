using System;

namespace Ford.SaveSystem.Dto
{
    public class UpdateSaveDto
    {
        public string Id { get; set; } = null!;
        public string Header { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
    }
}
