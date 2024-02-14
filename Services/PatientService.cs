﻿using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class PatientService : IPatientService
    {
        private readonly IRepository<Patient> _repository;
        private readonly ApplicationDbContext _dbContext;

        public PatientService(IRepository<Patient> repository)
        {
            _repository = repository;
        }

        public async Task AddPatient(Patient patient)
        {
           await _repository.Add(patient);
        }

        public void DeletePatient(int id)
        {
            _repository.Delete(id);
        }

        public async Task<Patient> GetPatient(int id)
        {
            return await _repository.Get(id);
        }

        public async Task<List<Patient>> GetAllPatients()
        {
            return await _repository.GetAll();
        }

        public async Task<int> UpdatePatient(Patient patient)
        {
            return await _repository.Update(patient);
        }


    }
}
