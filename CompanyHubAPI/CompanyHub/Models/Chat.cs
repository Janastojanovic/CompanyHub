using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyHub.Models
{
    [Table("Chats")]
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
    }
}
