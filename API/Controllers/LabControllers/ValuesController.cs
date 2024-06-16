﻿using AppointmentNotificationHandler;
using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO.Lab.UploadResults;
using Services.AppointmentService;
using Services.LabService;
using System.Diagnostics;
using System.Net.Mail;
using System.Security.Claims;

namespace API.Controllers.LabControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ValueService _vs;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHubContext<AppointmentNotificationHub, IAppointmentNotificationClient> _hubContext;

        public ValuesController(ValueService vs, IHubContext<AppointmentNotificationHub, IAppointmentNotificationClient> hubContext,ApplicationDbContext dbContext) 
        {
            _vs = vs;
            _hubContext = hubContext;
            _dbContext = dbContext;
        }

        [Authorize(Policy = "Lab")]
        [HttpGet("ReportRequest")]
        public async Task<ActionResult<object>> GetPatientPrescriptionData()
        {
            var res = await _vs.RequestList();
            return Ok(res);
        }

        [Authorize(Policy = "Lab")]
        [HttpGet("SetAccept")]
        async public Task<ActionResult> AccceptSample(int id)
        {
            var tmp= await _vs.AcceptSample(id);
            if (tmp)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = "Lab")]
        [HttpGet("Accept")]
        async public Task<ActionResult<IEnumerable<Object>>> AcceptedSamplesList()
        {
            var tmp=await _vs.AcceptedSamplesList();
            return Ok(tmp);
        }

        [Authorize(Policy = "Lab")]
        [HttpPost("Result")]
        async public Task<ActionResult> UploadResults(Result data)
        {
            var claim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "RoleId")?.Value;
            int roleId = int.Parse(claim);

            if (data != null && data.Results!=null)
            {
                var tmp = await _vs.UplaodResults(data,roleId);
                if (tmp)
                {
                    var dataObj = await _dbContext.labReports
                        .Include(lr => lr.Prescription)
                        .ThenInclude(p => p.Appointment)
                        .ThenInclude(a => a.Patient)
                        .Include(lr => lr.Prescription)
                        .ThenInclude(p => p.Appointment)
                        .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.User)
                        .Include(lr => lr.Test)
                        .FirstOrDefaultAsync(lr => lr.Id == data.ReportId);

                    var labReportInfo = new
                    {
                        PatientName = dataObj.Prescription.Appointment.Patient.Name,
                        TestName = dataObj.Test.TestName,
                        AcceptedDate = dataObj.AcceptedDate,
                        UserId = dataObj.Prescription.Appointment.Doctor.UserId
                    };
                    
                    var sendMail = new EmailSender();
                    string emsg = "Mr/Mrs. " + labReportInfo.PatientName + "<br/><br/>Results of your recent lab test (" + labReportInfo.TestName + ") on " + labReportInfo.AcceptedDate + "" +
                        " is ready and available.";
                    string notMsg = "Results of recent lab test (" + labReportInfo.TestName + ") of " + labReportInfo.PatientName + " on " + labReportInfo.AcceptedDate +
                    " is ready and available.";

                    if (data.Servere == true)
                    {
                        emsg = emsg + "<br> It appears that there are some conditions that require immediate attention.Therefore, we strongly recommend that you schedule an appointment with your doctor as soon as possible.";
                        notMsg = notMsg + "It appears that there are some conditions that require immediate attention.";
                    }


                    var iconUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRdq0Qw2AUbCppR3IQBWOZx94oZ2NWVuY1vMQ&s";

                    var htmlContent = $@"
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; padding: 20px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
        <h1 style='color: #09D636; text-align: center;'>
            <img src='{iconUrl}' style='margin-right: 8px;' width='32' height='32' alt='Hospital Icon'/>
            Medicare <span style='color: #AFDCBB;'>Hub</span>
        </h1>
        <p style='color: #555555; font-size: 16px; font-weight :bold'>{emsg}</p>
        <p style='font-size: 16px; color: #555;'>
            <span >Our team looks forward to providing you with exceptional care and service.</span> 
            <br/><br/>
            If you have any questions or need further assistance, 
            <span >please do not hesitate to contact us</span>. 
            <br/><br/>
            <span>Thank you for choosing Medicare Hub!</span>
        </p>
        <p style='font-size: 16px; color: #555;'>
            Best regards,
            <br/>
            <span style='color: #007BFF;'>Medicare Hub Team</span>
        </p>
    </div>
</body>
</html>";

 
                    Notification newNotification = new Notification();
                    newNotification.Message = notMsg;
                    newNotification.From = "1";//Add lab Id when authorized
                    newNotification.To = labReportInfo.UserId.ToString();
                    newNotification.SendAt = DateTime.Now;
                    newNotification.Seen = false;
                    
                    await sendMail.SendMail(labReportInfo.TestName + " results", "kwalskinick@gmail.com", labReportInfo.PatientName, htmlContent);
                    
                    if (labReportInfo.UserId != null && ConnectionManager._userConnections.TryGetValue(labReportInfo.UserId.ToString(), out var connectionId))
                    {
                        Debug.WriteLine($"User ConnectionId: {connectionId}");
                        await _hubContext.Clients.Client(connectionId).ReceiveNotification(newNotification);
                        Debug.WriteLine("Notification sent via SignalR.");
                    }
                    
                    await _dbContext.notification.AddAsync(newNotification);

                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest("Empty");
            }

        }


        //check the results of the report doctor requested is available
        [Authorize(Policy = "Doct")]
        [HttpGet("Result")]
        public async Task<ActionResult> CheckResult(int Pid)
        {
            return Ok(await _vs.CheckResult(Pid));
        }

        [Authorize(Policy = "Doct")]
        [HttpPost("Mark")]//mark a labreport as visited as it is opened
        public async Task<ActionResult> MarkCheck(List<int> ids)
        {
            await _vs.MarkCheck(ids);
            return Ok();
        }
    }
}
