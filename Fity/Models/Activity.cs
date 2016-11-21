namespace Fity.Models
{
    public class Activity
    {
        public string Sport { get; set; }

        public string Id { get; set; }

        public string Notes { get; set; }

        public Lap Lap { get; set; }
    }
}
