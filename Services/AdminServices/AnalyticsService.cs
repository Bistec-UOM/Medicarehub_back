﻿using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.AdminServices
{

    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _dbcontext;
        public AnalyticsService(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<object> GetMaleFemalePatientsCountAllDays()
        {
            var Prescriptions = await _dbcontext.prescriptions
                .Include(ap => ap.Appointment)
                .ThenInclude(a => a.Patient)
                .ToListAsync();

            var countsByDay = new List<object>();

            // Extract distinct dates
            var distinctDates = Prescriptions.Select(p => p.DateTime.Date).Distinct();
            foreach (var date in distinctDates) 
            {
                var child_male = Prescriptions
                   .Where(p => p.DateTime.Date == date)
                   .Select(p => p.Appointment)
                   .Count(a => a?.Patient?.Gender == "Male" && CalculateAge(a.Patient.DOB) < 18);
                var child_female = Prescriptions
                    .Where(p => p.DateTime.Date == date)
                    .Select(p => p.Appointment)
                    .Count(a => a?.Patient?.Gender == "Female" && CalculateAge(a.Patient.DOB) < 18);

                var adult_male = Prescriptions
                    .Where(p => p.DateTime.Date == date)
                    .Select(p => p.Appointment)
                    .Count(a => a?.Patient?.Gender == "Male" && CalculateAge(a.Patient.DOB) >= 18 && CalculateAge(a.Patient.DOB) < 45);
                var adult_female = Prescriptions
                    .Where(p => p.DateTime.Date == date)
                    .Select(p => p.Appointment)
                    .Count(a => a?.Patient?.Gender == "Female" && CalculateAge(a.Patient.DOB) >= 18 && CalculateAge(a.Patient.DOB) < 45);

                var old_male = Prescriptions
                    .Where(p => p.DateTime.Date == date)
                    .Select(p => p.Appointment)
                    .Count(a => a?.Patient?.Gender == "Male" && CalculateAge(a.Patient.DOB) > 45);
                var old_female = Prescriptions
                    .Where(p => p.DateTime.Date == date)
                    .Select(p => p.Appointment)
                    .Count(a => a?.Patient?.Gender == "Female" && CalculateAge(a.Patient.DOB) > 45);


                //remove this after completion of this part
                var Name = Prescriptions
                    .Where(p => p.DateTime.Date == date)
                    .Select(p => p.Appointment)
                    .Select(p => p.Patient)
                    .Select(p => new
                    {
                        name = p.Name,
                        dob = p.DOB
                    });
                countsByDay.Add(new {
                    Date=date,
                    child_male=child_male, 
                    child_female=child_female, 
                    adult_male = adult_male, 
                    adult_female = adult_female, 
                    old_male = old_male, 
                    old_female = old_female, 
                    Name =Name
                });
            }

            return countsByDay;
        }
        private int CalculateAge(DateTime? dob)
        {   //hasvalue use for not nullable values :- datetime?
            if (dob.HasValue)
            {
                DateTime now = DateTime.Today;
                int age = now.Year - dob.Value.Year;

                // If the current date is before the birth date of this year's birthday, decrement the age
                // This adjustment is necessary because if the birthday hasn't occurred yet this year,
                // the age should be one less than the difference in years between birth and current date
                if (now < dob.Value.AddYears(age))
                {
                    age--;
                }
                return age;
            }
            return 0; // Return 0 if date of birth is not valid
        }

    }

}
