﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.LabService;

namespace API.Controllers.LabControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TestService _tmpl;

        public TestController(TestService tmpl)
        {
            _tmpl = tmpl;
        }

        //Get the list of all lab tests to display in test list==================

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Test>>> GetAllTests()
        {
            return Ok(await _tmpl.GetAllTests());
        }




        //Delete a test from available test list=================================

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTest(int id)
        {
            await _tmpl.DeleteTest(id);
            return Ok();
        }



        //Edit test details from popup box=======================================

        [HttpPut]
        public async Task<ActionResult> EditTest(Test data)
        {
            await _tmpl.EditTest(data);
            return Ok();
        }
    }
}