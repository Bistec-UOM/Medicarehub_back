﻿using Castle.Core.Smtp;
using DataAccessLayer;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Models;
using Moq;
using Services.AppointmentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingProject.MockData;
using IEmailSender = Services.AppointmentService.IEmailSender;

namespace TestingProject.System.Services
{
    public class AppointmentServiceTest:IDisposable
    {
        private readonly ApplicationDbContext _dbContext;

        public AppointmentServiceTest() {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _dbContext.Database.EnsureCreated();


        }

        //----getAll()---
        [Fact]
        public async Task getAll_ReturnsAllAppointments()
        {
            //Arrange
            var mockAppointmentRepository = new Mock<IRepository<Appointment>>();
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            var mockDoctorRepository = new Mock<IRepository<User>>();
            var mockUnableDateRepository = new Mock<IRepository<Unable_Date>>();
            var mockNotificationRepository = new Mock<IRepository<Notification>>();

            _dbContext.appointments.AddRange(AppointmentMockData.getAppointments());
            _dbContext.SaveChanges();
            



            mockAppointmentRepository.Setup(repo => repo.GetAll()).ReturnsAsync(AppointmentMockData.getAppointments());
            var sut = new AppointmentService(_dbContext, mockAppointmentRepository.Object, mockPatientRepository.Object, mockDoctorRepository.Object, mockUnableDateRepository.Object, mockNotificationRepository.Object);


            //Act

            var result=await sut.GetAll();

            //Assert
            Assert.Equal(AppointmentMockData.getAppointments().Count, result.Count);
            Assert.Equal(AppointmentMockData.getAppointments()[0].Id, result[0].Id);
            Assert.Equal(AppointmentMockData.getAppointments()[1].Id, result[1].Id);

        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
        //----getDoctors()---
        [Fact]
        public async Task getDoctors_ReturnsAllDoctors()
        {
            //Arrange
            var mockAppointmentRepository = new Mock<IRepository<Appointment>>();
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            var mockDoctorRepository = new Mock<IRepository<User>>();
            var mockUnableDateRepository = new Mock<IRepository<Unable_Date>>();
            var mockNotificationRepository = new Mock<IRepository<Notification>>();

            _dbContext.users.AddRange(AppointmentMockData.getUsers());
            _dbContext.SaveChangesAsync();

            var sut=new AppointmentService(_dbContext,mockAppointmentRepository.Object,mockPatientRepository.Object, mockDoctorRepository.Object,mockUnableDateRepository.Object,mockNotificationRepository.Object);

            //Act

            var result=await sut.GetDoctors();


            //Assert

            Assert.Equal(AppointmentMockData.getUsers().Count-1,result.Count);
            Assert.Equal(AppointmentMockData.getUsers()[0].Id, result[0].Id);
            Assert.Equal(AppointmentMockData.getUsers()[1].Id, result[1].Id);


        }
        //-----AddAppointment(Appointment app)---
        [Fact]
        public async Task AddAppointment_ShouldAddAppointment()
        {

            //Arrange
            var mockAppointmentRepository = new Mock<IRepository<Appointment>>();
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            var mockDoctorRepository = new Mock<IRepository<User>>();
            var mockUnableDateRepository = new Mock<IRepository<Unable_Date>>();
            var mockNotificationRepository = new Mock<IRepository<Notification>>();

           

            
            _dbContext.users.AddRange(AppointmentMockData.getUsers());
            _dbContext.doctors.AddRange(AppointmentMockData.getDoctUserDoctors());
            _dbContext.patients.AddRange(AppointmentMockData.GetPatients());
            _dbContext.appointments.AddRange(AppointmentMockData.getAppointments());
            _dbContext.SaveChangesAsync();

            var appointmentToAdd = AppointmentMockData.AddNewAppointment();

            mockAppointmentRepository.Setup(repo => repo.Add(It.IsAny<Appointment>()))
                   .Returns((Appointment appointment) =>
                   {
                       _dbContext.appointments.Add(appointment);
                       return Task.CompletedTask;
                   });



            var mockEmailSender = new Mock<IEmailSender>();
            mockEmailSender.Setup(sender => sender.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(Task.CompletedTask);
            var sut = new AppointmentService(_dbContext, mockAppointmentRepository.Object, mockPatientRepository.Object, mockDoctorRepository.Object, mockUnableDateRepository.Object, mockNotificationRepository.Object);
            
            //Act

            var result = await sut.AddAppointment(AppointmentMockData.AddNewAppointment());
            _dbContext.SaveChangesAsync();


            //Assert
            result.Should().Be(0);
            mockAppointmentRepository.Verify(repo=>repo.Add(It.IsAny<Appointment>()),Times.Once);
            _dbContext.appointments.Count().Should().Be(5);
           

        }
        //----GetPatient(int id)----
        [Fact]
        public async Task GetPatient_SholudReturnPatient()
        {
            //Arrange
            var mockAppointmentRepository = new Mock<IRepository<Appointment>>();
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            var mockDoctorRepository = new Mock<IRepository<User>>();
            var mockUnableDateRepository = new Mock<IRepository<Unable_Date>>();
            var mockNotificationRepository = new Mock<IRepository<Notification>>();


            _dbContext.patients.AddRange(AppointmentMockData.GetPatients());
            _dbContext.SaveChangesAsync();
            mockPatientRepository.Setup(repo => repo.Get(AppointmentMockData.GetPatient().Id)).ReturnsAsync(AppointmentMockData.GetPatient());
            var sut = new AppointmentService(_dbContext, mockAppointmentRepository.Object, mockPatientRepository.Object, mockDoctorRepository.Object, mockUnableDateRepository.Object, mockNotificationRepository.Object);


            //Act
            var result = await sut.GetPatient(AppointmentMockData.GetPatient().Id);


            //Assert

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(AppointmentMockData.GetPatient());
            mockPatientRepository.Verify(repo => repo.Get(AppointmentMockData.GetPatient().Id), Times.Once);


        }
        //----getAppointment(int id)----
        [Fact]
        public async Task GetAppointment_SholudReturnAppointment()
        {
            //Arrange
            var mockAppointmentRepository = new Mock<IRepository<Appointment>>();
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            var mockDoctorRepository = new Mock<IRepository<User>>();
            var mockUnableDateRepository = new Mock<IRepository<Unable_Date>>();
            var mockNotificationRepository = new Mock<IRepository<Notification>>();

            _dbContext.appointments.AddRange(AppointmentMockData.getAppointments());
            _dbContext.SaveChangesAsync();
            mockAppointmentRepository.Setup(repo => repo.Get(AppointmentMockData.getAppointments()[0].Id)).ReturnsAsync(AppointmentMockData.getAppointments()[0]);
            var sut = new AppointmentService(_dbContext, mockAppointmentRepository.Object, mockPatientRepository.Object, mockDoctorRepository.Object, mockUnableDateRepository.Object, mockNotificationRepository.Object);

            //Act
            var result = await sut.GetAppointment(AppointmentMockData.getAppointments()[0].Id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(AppointmentMockData.getAppointments()[0]);
            mockAppointmentRepository.Verify(repo => repo.Get(AppointmentMockData.getAppointments()[0].Id), Times.Once);


        }
        //----GetDoctorAppointments(int id)------
        [Fact]
        public async Task getDoctorAppointments_shouldReturnDoctorAppointment()
        {
            //Arrange
            var mockAppointmentRepository = new Mock<IRepository<Appointment>>();
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            var mockDoctorRepository = new Mock<IRepository<User>>();
            var mockUnableDateRepository = new Mock<IRepository<Unable_Date>>();
            var mockNotificationRepository = new Mock<IRepository<Notification>>();

            _dbContext.appointments.AddRange(AppointmentMockData.getDoctor1Appointment());
            _dbContext.SaveChangesAsync();

            var sut = new AppointmentService(_dbContext, mockAppointmentRepository.Object, mockPatientRepository.Object, mockDoctorRepository.Object, mockUnableDateRepository.Object, mockNotificationRepository.Object);

            //Act

            var result = await sut.GetDoctorAppointments(1);


            //Assert

            Assert.Equal(AppointmentMockData.getDoctor1Appointment().Count, result.Count);
            Assert.Equal(AppointmentMockData.getDoctor1Appointment()[0].Id, result[0].Id);
            Assert.Equal(AppointmentMockData.getDoctor1Appointment()[1].Id, result[1].Id);


        }

        //---getPatients()---
        [Fact]
        public async Task getPatients_ReturnsAllPatients()
        {
            //Arrange
            var mockAppointmentRepository = new Mock<IRepository<Appointment>>();
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            var mockDoctorRepository = new Mock<IRepository<User>>();
            var mockUnableDateRepository = new Mock<IRepository<Unable_Date>>();
            var mockNotificationRepository = new Mock<IRepository<Notification>>();

            _dbContext.patients.AddRange(AppointmentMockData.GetPatients());
            _dbContext.SaveChangesAsync();
            mockPatientRepository.Setup(repo => repo.GetAll()).ReturnsAsync(AppointmentMockData.GetPatients());

            var sut = new AppointmentService(_dbContext, mockAppointmentRepository.Object, mockPatientRepository.Object, mockDoctorRepository.Object, mockUnableDateRepository.Object, mockNotificationRepository.Object);

            //Act

            var result = await sut.GetPatients();


            //Assert

            Assert.Equal(AppointmentMockData.GetPatients().Count, result.Count);
            Assert.Equal(AppointmentMockData.GetPatients()[0].Id, result[0].Id);
            Assert.Equal(AppointmentMockData.GetPatients()[1].Id, result[1].Id);


        }

        //----RegisterPatient(Patient patient)
        [Fact]
        public async Task RegisterPatient_shouldCallRepoAdd()
        {
            //Arrange
            var mockAppointmentRepository = new Mock<IRepository<Appointment>>();
            var mockPatientRepository = new Mock<IRepository<Patient>>();
            var mockDoctorRepository = new Mock<IRepository<User>>();
            var mockUnableDateRepository = new Mock<IRepository<Unable_Date>>();
            var mockNotificationRepository = new Mock<IRepository<Notification>>();


            _dbContext.users.AddRange(AppointmentMockData.getUsers());
            _dbContext.doctors.AddRange(AppointmentMockData.getDoctUserDoctors());
            _dbContext.patients.AddRange(AppointmentMockData.GetPatients());
            _dbContext.appointments.AddRange(AppointmentMockData.getAppointments());
            _dbContext.SaveChangesAsync();

          ;

            mockPatientRepository.Setup(repo => repo.Add(It.IsAny<Patient>()))
                   .Returns((Patient patient) =>
                   {
                       _dbContext.patients.Add(patient);
                       return Task.CompletedTask;
                   });



            var mockEmailSender = new Mock<IEmailSender>();
            mockEmailSender.Setup(sender => sender.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(Task.CompletedTask);
            var sut = new AppointmentService(_dbContext, mockAppointmentRepository.Object, mockPatientRepository.Object, mockDoctorRepository.Object, mockUnableDateRepository.Object, mockNotificationRepository.Object);

            //Act

            var result =  sut.RegisterPatient(AppointmentMockData.GetPatient());
            _dbContext.SaveChangesAsync();


            //Assert
            mockPatientRepository.Verify(repo => repo.Add(It.IsAny<Patient>()), Times.Once);
            Assert.Equal(_dbContext.patients.Count(), 3); ;

        }





    }
}
