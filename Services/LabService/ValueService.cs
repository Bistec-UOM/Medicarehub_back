﻿using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Models;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.LabService
{
    public class ValueService
    {
        private readonly IRepository<LabReport> _rep;
        private readonly ApplicationDbContext _cntx;
        public ValueService(IRepository<LabReport> rep,ApplicationDbContext contxt)
        {
            _rep = rep;
            _cntx = contxt;
        }


        async public Task<IEnumerable<Object>> RequestList()
        {
            var data = await _cntx.patients
               .Where(p => p.Appointment.Any(a => a.Prescription != null)) // Filter out patients without appointments or prescriptions
               .SelectMany(p => p.Appointment.Where(a => a.Prescription != null).Select(a => new
               {
                   Name = p.FullName,

                   PrescriptionId = a.Prescription.Id
               }))
               .ToListAsync();
            return data;
             
        }
        async public Task AcceptSample(int id)
        {
            var tmp= await _rep.Get(id);
            tmp.Status = "accepted";
            await _rep.Update(tmp);
        }

        async public Task<IEnumerable<LabReport>> AcceptedSamplesList()
        {
            return await _rep.GetByProp("Status", "accepted");
        }
    }
}
