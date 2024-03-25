namespace CompanyHub.Models
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime Deadline { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public int ProcentageCompleted { get; set; } = 0;
        //public List<Task> Tasks { get; set; }
    }
}
