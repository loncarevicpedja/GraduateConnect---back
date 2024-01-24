using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularAythAPI.Models
{
    public class Theme
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Field { get; set; }
        public bool Status { get; set; }
        public int? ProfesorId { get; set; }

        [ForeignKey("ProfesorId")]
        public  User Profesor { get; set; }

        public int? StudentId { get; set; }

        [ForeignKey("StudentId")]
        public User Student { get; set; }
    }

}
