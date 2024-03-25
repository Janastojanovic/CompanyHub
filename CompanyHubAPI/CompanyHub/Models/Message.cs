using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyHub.Models
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }=string.Empty;
        public DateTime Time { get; set; }=DateTime.Now;
        public int ChatId { get; set; } = 0;
        public string SenderId { get; set; }= string.Empty;
    }
}
