﻿using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AdminServices
{
    public interface IAnalyticsService
    {
        Task<object> GetMaleFemalePatientsCountAllDays();
        Task<object> GetTotalAmount();
        Task<object> GetAvailableCount();
        Task<object> GetTotalDrugUsage();
    }
}
