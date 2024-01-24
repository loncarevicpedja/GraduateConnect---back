using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularAythAPI.Models
{
    public class Commission
    {
        [Key]
        public int Id { get; set; }

        public List<User> CommissionMembers { get; set; }
    }
}
