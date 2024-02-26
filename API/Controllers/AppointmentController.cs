﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.AppointmentService;
using System.Runtime.InteropServices;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _repository;
        public AppointmentController(IAppointmentRepository repository)
        {
            _repository = repository;
            
        }

        [HttpGet]

        public async Task<ActionResult<ICollection<Appointment>>> GetAllAppointments()
        {
            var appointments = await _repository.GetAll();
            return Ok(appointments);
        }

        [HttpPost]
        public async Task AddAppointment(Appointment appointment)
        {
              await _repository.AddAppointment(appointment);
        }
        [HttpGet("{id}",Name ="GetAppointment")]

        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment=await _repository.GetAppointment(id);
            return Ok(appointment);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            var targetAppointment = await _repository.GetAppointment(id);
            if(targetAppointment is null)
            {
                return NotFound();
            }

            _repository.DeleteAppointment(id);
            return NoContent();
        }

        [HttpGet("doctor/{doctorId}",Name ="GetDoctorAppointments")]
        public async Task<ActionResult<ICollection<Appointment>>> GetDoctorAppointments(int doctorId)
        {
            var doctorAppointments = await _repository.GetDoctorAppointments(doctorId);
            return Ok(doctorAppointments);
        }
        [HttpGet("doctor/{doctorId}/day/{date}")]
        public async Task<ActionResult<ICollection<Appointment>>> GetDoctorAppointmentsByDate(int doctorId,DateTime date)
        {
            var doctorDayAppointments=await _repository.GetDoctorAppointmentsByDate(doctorId,date);
            return Ok(doctorDayAppointments);
        }


        [HttpDelete("doctor/{doctorId}/day/{date}")]
        public async Task<ActionResult> DeleteDoctorAllDayAppointments(int doctorId,DateTime date)
        {
            var targetAppointments = await _repository.GetDoctorAppointmentsByDate(doctorId, date);
            if (targetAppointments is null)
            {
                return NotFound();
            }

            _repository.DeleteAllDoctorDayAppointments(doctorId, date);
            return NoContent();

        }

        [HttpGet("doctors")]
        public async Task<ActionResult<ICollection<User>>> GetDoctors()
        {
            var doctors=_repository.GetDoctors();
            return Ok(doctors);
        }


        
    }
}