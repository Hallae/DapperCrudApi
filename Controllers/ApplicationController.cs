using Dapper;
using DapperCrudApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using static System.Net.Mime.MediaTypeNames;
using Application = DapperCrudApi.Models.Application;

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
     public ActionResult<List<Application>> GetApplications()
     {
         using (var connection = _AdbContext.ApplicationConnection)
         {
             connection.Open();
             var applications = connection.Query<Application>("SELECT * FROM Applications").ToList();
             return Ok(applications); 
         }
     }


        [HttpPost]
        public ActionResult<Application> CreateApplication(Application application)
        {
            // Ensure a new GUID is generated for every new application request post
            application.id = Guid.NewGuid();
            application.author = Guid.NewGuid();

            using (var connection = _AdbContext.ApplicationConnection)
            {
                connection.Open();
                // Adjusted SQL query to match the Application class properties
                var query = @"
    INSERT INTO Applications (guid, author, activity, name, description, outline, IsSubmitted, SubmissionDate) 
    VALUES (@id, @author, @activity, @name, @description, @outline, @IsSubmitted, @SubmissionDate);
";
                var affectedRows = connection.Execute(query, application);

                if (affectedRows == 0)
                {
                    return BadRequest("Failed to create application");
                }

                return CreatedAtAction(nameof(GetApplications), new { guid = application.id }, application);
            }
        }




        [HttpGet("Get by id")]
     public ActionResult<Application> GetApplications(Guid id)
     {
         using (var connection = _AdbContext.ApplicationConnection)
         {
             connection.Open();
             var application = connection.QueryFirstOrDefault<Application>("SELECT * FROM Applications WHERE guid = @Id", new { Id = id });
             if (application == null)
             {
                 return NotFound();
             }
             return Ok(application);
         }
     }

     [HttpDelete("Delete by id")]
     public IActionResult DeleteApplications(Guid id)
     {
         using (var connection = _AdbContext.ApplicationConnection)
         {
             connection.Open();
             var query = "DELETE FROM Applications WHERE guid = @Id";
             var affectedRows = connection.Execute(query, new { Id = id });

             if (affectedRows == 0)
             {
                 return NotFound();
             }

             return NoContent();
         }
     }


     /*   [HttpPost("submit")]
        public async Task<ActionResult<Application>> Submit(Guid id)
        {
            using (var connection = _AdbContext.ApplicationConnection)
            {
                await connection.OpenAsync();

                // Find the application by its ID
                var application = await connection.QueryFirstOrDefaultAsync<Application>("SELECT * FROM Applications WHERE id = @Id", new { Id = id });
                if (application == null)
                {
                    return NotFound("Application not found");
                }

                // Update the application's IsSubmitted status
                var updateQuery = @"UPDATE Applications SET IsSubmitted = 1 WHERE id = @Id";
                var affectedRows = await connection.ExecuteAsync(updateQuery, new { Id = id });

                if (affectedRows == 0)
                {
                    return BadRequest("Failed to update application status");
                }

                return Ok(application);
            }
        }*/


    }
}
