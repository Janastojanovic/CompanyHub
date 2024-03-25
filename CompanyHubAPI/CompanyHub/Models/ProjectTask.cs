namespace CompanyHub.Models
{
    public class ProjectTask
    {
        public string Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }=DateTime.Now;
        public DateTime Deadline { get; set; }=DateTime.Now;
        public bool Completed { get; set; } = false;
        public int Procentage { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string ProjectId { get; set; }= string.Empty;
    }
}
