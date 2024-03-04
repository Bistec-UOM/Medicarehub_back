using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class LabReport
    {

        public int Id { get; set; }
        public int PrescriptionID { get; set; }
        public DateTime DateTime { get; set; }
        public int TestId { get; set; }
        public string Status { get; set; } = null!;
        public int LbAstID { get; set; }
    }
}