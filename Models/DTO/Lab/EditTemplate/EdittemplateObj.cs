﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO.Lab.EditTemplate
{
    public class EdittemplateObj
    {
        public int TestId { get; set; }
        public List<EdittemplateField>? Fields { get; set; }
    }
}
