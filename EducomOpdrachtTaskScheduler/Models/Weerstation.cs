namespace EducomOpdrachtTaskScheduler.Models
{
    public class Weerstation
    {
        public long Id { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }

        public Weerstation(long id, string region, string name)
        {
            this.Id = id;
            this.Region = region;
            this.Name = name;
        }
    }
}
