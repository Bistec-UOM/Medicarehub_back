using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    public class Record
    {
        public int Id { get; set; }
        public int LabReportId { get; set; }
        public int ReportFieldId { get; set; }
        public float Result { get; set; }
        public string? Status { get; set; }
    }
}
