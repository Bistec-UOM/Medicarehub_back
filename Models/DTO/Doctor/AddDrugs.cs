﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO.Doctor
{
    public class AddDrugs
    {
        public int Id { get; set; } //appoinment id
        public List<Drugs> Drugs { get; set; }
        public List<Labs> Labs { get; set; }
        public string? Description { get; set; }
    }
}
