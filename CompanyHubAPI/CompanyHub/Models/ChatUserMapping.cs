using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyHub.Models
{
    [Table("ChatUserMapping")]
    public class ChatUserMapping
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }

        public string UserName { get; set; }
        public string ChatId { get; set; }

    }
}
