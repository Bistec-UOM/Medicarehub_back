﻿using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AppointmentService
{
    public class AppointmentService 
    {

        private readonly ApplicationDbContext _dbcontext;
        private readonly IRepository<Appointment> _appointment;
        private readonly IRepository<Patient> _patient;
        private readonly IRepository<User> _doctor;
        private readonly IRepository<Unable_Date> _unable_date;
        
        public AppointmentService(ApplicationDbContext dbcontext,IRepository<Appointment> appointment,IRepository<Patient> patient,IRepository<User> doctor,IRepository<Unable_Date> unableDate)
        {
            _dbcontext = dbcontext;
            _appointment = appointment;
            _patient = patient;
            _doctor = doctor;
            _unable_date = unableDate;
        }     
        public async Task AddAppointment(Appointment appointment)
        { 
              await _appointment.Add(appointment);    //adding an appointment using IRepository add function
              var patientId = appointment.PatientId;   // sending an email for the patient
              var patient = await GetPatient(patientId);
              string emailSubject = "Confirmation: Your Appointment with Medicare Hub";
              string userName = patient?.FullName;
              string emailMessage = "Dear " + patient.Name + ",\n" + "We're thrilled to confirm your appointment with Medicare Hub scheduled for " + appointment.DateTime;

              EmailSender emailSernder = new EmailSender();
              await emailSernder.SendMail(emailSubject, patient.Email, userName, emailMessage);  
        }
        public async Task<List<Appointment>> CancelAllAppointments(int doctorId, DateTime date)  //services function for cancelling the all appointments by a doctor
        {
            var appointments = await GetDoctorAppointmentsByDate(doctorId, date);
            List<Appointment> updatedResults = new List<Appointment>();

            foreach (Appointment app in appointments)  //filter new appointmetns and add them to the updatedResults list
            {
                if (app.Status == "new") // check the status of an appointment.because cancel is allowed only for new appointments
                {
                    var upAppointment = await UpdateOnlyOneAppointmentUsingId(app.Id);  //change the status of a new appointment
                    updatedResults.Add(upAppointment);
                }
            }
            return updatedResults;
        }
        public async Task<List<Appointment>> DeleteAllDoctorDayAppointments(int doctorId, DateTime date)  //service function for real delete  of new  appointments from table
        {
            var targetAppointments = await GetDoctorAppointmentsByDate(doctorId, date);

            if (targetAppointments != null && targetAppointments.Any())
            {
                // Filter appointments with status "new"
                var appointmentsToDelete = targetAppointments.Where(appointment => appointment.Status == "new").ToList();

                if (appointmentsToDelete.Any())
                {
                    _dbcontext.appointments.RemoveRange(appointmentsToDelete);  //remove the appointmentsToDelete list from table
                    _dbcontext.SaveChanges();
                   
                }
                return appointmentsToDelete;

            }
            else
            {
                return new List<Appointment>();
            }
        }

        public async Task<Patient> GetPatient(int id)
        {
            var patient = _patient.Get(id);
            return await patient;
        }

        public async Task<Appointment> DeleteAppointment(int id)
        {
            var appointmentk=await GetAppointment(id);
            if (appointmentk != null)
            {
               await _appointment.Delete(id);
                return appointmentk;

            }
            else
            {
                throw new ArgumentException($"Appointment with id {id} not found.");
            }
        }

        public async Task<List<Appointment>> GetAll()
        {

            return await _appointment.GetAll();

            
        }
        public async Task<Appointment> GetAppointment(int id)
        {
            var appointment = _appointment.Get(id);
            return await appointment;
        }
        public async Task<List<Appointment>> GetDoctorAppointments(int doctorId)   //getting all the appointments of a specific doctor
        {   
            var doctorAppointments =  _dbcontext.appointments.Where(a => a.DoctorId == doctorId);
            return doctorAppointments.ToList();
        }
        public async Task<List<Appointment>> GetDoctorAppointmentsByDate(int doctorId, DateTime date)   //getting all the appointments of a specific doctor of a specific date
        {
            var doctorDayAppointments =  _dbcontext.appointments.Where(a => a.DoctorId == doctorId && a.DateTime.Date == date);
            return doctorDayAppointments.ToList();   
        }
        public async Task<List<User>> GetDoctors()
        {
            var doctors =  _dbcontext.users.Where(d => d.Role == "Doctor");
            return  doctors.ToList();      
        }
        public async Task<List<Patient>> GetPatients()
        {
           
            return  await _patient.GetAll();
        }
        public async Task RegisterPatient(Patient patient)
        {
            await _patient.Add(patient);  //adding the patient to the table
            string emailSubject = "Your Medicare Hub Membership: A Warm Welcome Awaits!";
            string userName = patient.FullName;
            string emailMessage = "Dear " + patient.Name +"\n"+ "Thank you for choosing Medicare Hub. We're honored to be part of your healthcare journey. Please reach out if you need anything.";
            EmailSender emailSernder= new EmailSender();  //sending email after succeful registration of a patient
            await emailSernder.SendMail(emailSubject,patient.Email,userName,emailMessage);
        }
       public async Task<Appointment> UpdateAppointment(int id, Appointment appointment)  //just update appointment
        {   
            var oldAppointment = await GetAppointment(id);
             
            if (oldAppointment != null)
            {
                oldAppointment.DateTime = appointment.DateTime;    // Update properties of the existing appointment
                oldAppointment.Status = appointment.Status;
                oldAppointment.PatientId = appointment.PatientId;
                oldAppointment.DoctorId = appointment.DoctorId;
                oldAppointment.RecepId = appointment.RecepId;
                 try
                 {
                      await _dbcontext.SaveChangesAsync();
                      return oldAppointment;
                 }
                 catch (Exception ex)
                 {
                        
                        throw new Exception("Error occured during the updation");
                 }
            }
            else
            {
                throw new ArgumentException($"Appointment with id {id} not found.");  //throw an exception if the appointment does not exist
            }
        }
        public async Task<Appointment> UpdateAppointmentStatus(int id, Appointment appointment)  //update  appointment
        {
            var oldAppointment = await GetAppointment(id);
            if (oldAppointment != null)
            {
                // Update properties of the existing appointment
                oldAppointment.DateTime = appointment.DateTime;
                oldAppointment.Status = appointment.Status;
                oldAppointment.PatientId = appointment.PatientId;
                oldAppointment.DoctorId = appointment.DoctorId;
                oldAppointment.RecepId = appointment.RecepId;

                try
                {
                    await _dbcontext.SaveChangesAsync();
                    return oldAppointment;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                  
                    throw new Exception("Error occured during the updation");
                }
            }
            else
            {
               
                throw new ArgumentException($"Appointment with id {id} not found.");
            }

        }
        public async Task<Appointment> UpdateOnlyOneAppointmentUsingId(int id)  //update only the appointment status from new to cancelled
        {
            var oldAppointment = await GetAppointment(id);


            if (oldAppointment != null)
            {
                // Update properties of the existing appointment


                oldAppointment.DateTime = oldAppointment.DateTime;
                oldAppointment.Status = "cancelled";
                oldAppointment.PatientId = oldAppointment.PatientId;
                oldAppointment.DoctorId = oldAppointment.DoctorId;
                oldAppointment.RecepId = oldAppointment.RecepId;

                try
                {
                    var targetPatient=await  GetPatient(oldAppointment.PatientId);

                    var targetEmail = targetPatient.Email;
                    var targetDay = oldAppointment.DateTime.Date;
                    var targetTime = oldAppointment.DateTime;
                    string emailSubject = "Appointment Update: Cancellation Notification"; //sending the cancel notification mail
                    string userName = targetPatient.FullName;
                    string emailMessage = "Dear " + targetPatient.Name + ",\n" + " We regret to inform you that your scheduled appointment with Medicare Hub on " + targetTime + " has been cancelled. We apologize for any inconvenience this may cause you.";
                    EmailSender emailSernder = new EmailSender();
                    await emailSernder.SendMail(emailSubject, targetEmail, userName, emailMessage);

                    await _dbcontext.SaveChangesAsync();
                    return oldAppointment;
                }
                catch (Exception ex)
                {
                    
                    throw new Exception("Error occured during the updation");
                }
            }
            else
            {

                throw new ArgumentException($"Appointment with id {id} not found.");
            }

        }
        public async Task<List<Appointment>> GetAppointmentCountOfDays(int doctorId, int monthId)  //get monthly appointment count of a doctor for progress bar
        {
            var targetAppointment=_dbcontext.appointments.Where(a=>a.DoctorId==doctorId && (a.DateTime.Month)-1==monthId).ToList();
            return targetAppointment;
        }
        public async Task AddUnableDate(Unable_Date uDate)
        {
            await _unable_date.Add(uDate);
        }
        public async Task<List<Unable_Date>> getUnableDates(int doctorId)
        {
            var uDates = _dbcontext.unable_Dates.Where(u => u.doctorId == doctorId);
            return  uDates.ToList();
        }
    }
}
