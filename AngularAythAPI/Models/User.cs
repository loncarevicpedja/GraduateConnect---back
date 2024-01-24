using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularAythAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string  FirstName{ get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public List<Theme>? Themes { get; set; }
        public User Profesor { get; set; }
        public int? ProfesorId { get; set; }
        public Theme StudentThemes { get; set; }
        public int? StudentThemesId { get; set; }
        public Labor StudentsLabor { get; set; }
        public int? StudentsLaborId { get; set; }
        public List<Commission>? Commissions { get; set; }

        public int CommissionId { get;  set; }
    }
}
