﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrugsController : ControllerBase
    {
        private readonly DrugsService _drg;
        public DrugsController(DrugsService drg)
        {
            _drg = drg;
        }
        [HttpPost]
        public async Task<ActionResult<string>> AddDrug(Drug item)
        {
            await _drg.AddDrug(item);
            return Ok("Success");
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Drug>>> GetDrugs()
        {
            var drugs = await _drg.GetDrugs();
            return Ok(drugs);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteDrug(int id)
        {
            var result = await _drg.DeleteDrug(id);
            if (result)
                return Ok("Drug deleted successfully.");
            else
                return NotFound("Drug not found.");
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Drug>> GetDrug(int id)
        {
            var drug = await _drg.GetDrugById(id);
            if (drug == null)
                return NotFound("Drug not found.");

            return Ok(drug);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateDrug(int id, [FromBody] Drug updatedDrug)
        {
            var result = await _drg.UpdateDrug(id, updatedDrug);
            if (result)
                return Ok("Drug updated successfully.");
            else
                return NotFound("Drug not found.");
        }
    }

}
