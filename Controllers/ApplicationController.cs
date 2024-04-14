using Dapper;
using DapperCrudApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace DapperCrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly DatabaseContext _AdbContext;

        public ApplicationController(DatabaseContext AdbContext)
        {
            _AdbContext = AdbContext;
        }



        [HttpGet]
        public ActionResult<List<Applications>> Get()
        {
            using (var connection = _AdbContext.Connection)
            {
                connection.Open();
                var applications = connection.Query<Applications>("SELECT * FROM Applications").ToList();
                return Ok(Applications);
            }
        }


        [HttpGet("Get by id")]
        public ActionResult<Applications> GetApplications(Guid id)
        {
            using (var connection = _AdbContext.Connection)
            {
                connection.Open();
                var application = connection.QueryFirstOrDefault<Applications>("SELECT * FROM Applications WHERE id = @Id", new { Id = id });
                if (application == null)
                {
                    return NotFound();
                }
                return Ok(application);
            }
        }

        [HttpDelete("Delete by id")]
        public IActionResult DeleteApplications(int id)
        {
            using (var connection = _AdbContext.Connection)
            {
                connection.Open();
                var query = "DELETE FROM Applications WHERE id = @Id";
                var affectedRows = connection.Execute(query, new { Id = id });

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }


        [HttpPost("submit")]
        public async Task<ActionResult<List<Application>>> Submit(Guid id)
        {
            var dbNewApplications = await _AdbContext.Application.FindAsync(id);
            if (dbNewApplications == null)
                return BadRequest("Application not found");


            dbNewApplications.IsSubmitted = true; // Set the application as submitted

            await _AdbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
