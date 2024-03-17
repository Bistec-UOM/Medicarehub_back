﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.LabService;

namespace API.Controllers.LabControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ValueService _vs;
        public ValuesController(ValueService vs) 
        {
            _vs = vs;
        }

        [HttpPost("Accept")]
        async public Task<ActionResult> AccceptSample(int id)
        {
            await _vs.AcceptSample(id);
            return Ok();
        }
    }
}