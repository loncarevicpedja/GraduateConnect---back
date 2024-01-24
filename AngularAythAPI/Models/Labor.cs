using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularAythAPI.Models
{
    public class Labor
    {
        public int Id { get; set; }
        public Commission Commission { get; set; }
        [ForeignKey("Commission")]
        public int? CommissionId { get; set; }
        public int? ProfesorId { get; set; }
        [ForeignKey("ProfesorId")]
        public User Profesor { get; set; }
        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public User Student { get; set; }
        public string? Name { get; set; }
        public string? KeyWords { get; set; }
        public string? Description { get; set; }
        public DateTime? DateOfSubmission { get; set; }
        public DateTime? DateOfDefense { get; set; }
        public int? Rate { get; set; }
        public string? Status { get; set; }
    }
}
