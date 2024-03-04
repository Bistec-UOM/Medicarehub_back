using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string? Status { get; set; } 
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int RecepId { get; set; }


    }
}