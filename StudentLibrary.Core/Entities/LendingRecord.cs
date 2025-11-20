using System;

namespace StudentLibrary.Core.Entities
{
    // Опционально: если захотите фиксировать даты выдачи/возврата
    public class LendingRecord
    {
        public Guid DocumentId { get; set; }
        public Guid UserId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}
