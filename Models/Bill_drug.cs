using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class Bill_drug
    {
        public int Id { get; set; }    
        public int DrugID { get; set; }
        public int PrescriptionID { get; set; }
        public int Amount { get; set; }
    }
}