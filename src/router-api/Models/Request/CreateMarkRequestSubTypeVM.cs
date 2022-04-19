namespace RouterApi.Models.Request
{
    public class CreateMarkRequestSubTypeVM
    {
        public int? OldMarkRating { get; set; }

        public bool OldMarkPresence { get; set; }

        public int? NewMarkRating { get; set; }

        public bool NewMarkPresence { get; set; }
    }
}
