﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.AdminServices;

namespace API.Controllers.AdminControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        public AnalyticController (IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }
        // GET: api/<AnalyticsController>
        [HttpGet("GetAmounts")]
        public async Task<ActionResult> GetAmounts()
        {
            var values = await _analyticsService.GetAllAmount();
            return Ok(values);
        }
        // GET: api/Analytic/GetPatients
        [HttpGet("GetPatients")]
        public async Task<ActionResult> GetPatients()
        {
            try
            {
                var result = await _analyticsService.GetAllPatientDetails();
                var patientDOBs = result.PatientDOBs;
                var appointmentDates = result.AppointmentDates;

                // You can further process or return the data as needed
                return Ok(new { PatientDOBs = patientDOBs, AppointmentDates = appointmentDates });
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, log, and return an error response
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}