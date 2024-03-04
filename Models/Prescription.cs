using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int AppointmentID { get; set; }
        public float Total { get; set; }
        public int CashierId { get; set; }
        public User? Cashier { get; set; }



    }
}