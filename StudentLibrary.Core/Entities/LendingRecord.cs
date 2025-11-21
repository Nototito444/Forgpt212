using System;

namespace StudentLibrary.Core.Entities
{
    public class LendingRecord
    {
        public Guid DocumentId { get; set; }
        public Guid UserId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}
