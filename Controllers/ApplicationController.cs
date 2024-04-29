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

        [HttpGet("Activities")]
        public ActionResult<IEnumerable<Activity>> GetActivities()
        {
            var activities = new List<Activity>
            {
                new Activity { ActivityType = "Report", Description = "Доклад, 35-45 минут" },
                new Activity { ActivityType = "Masterclass", Description = "Мастеркласс, 1-2 часа" },
                new Activity { ActivityType = "Discussion", Description = "Дискуссия / круглый стол, 40-50 минут" }
            };

            return activities;
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


        [HttpPost("submit")]
        public async Task<ActionResult<Application>> Submit(Guid id)
        {
            using (var connection = _AdbContext.ApplicationConnection)
            {
               
                var existingApplication = await connection.QueryFirstOrDefaultAsync<Application>("SELECT * FROM Applications WHERE guid = @Id", new { Id = id });
                if (existingApplication == null)
                {
                    return NotFound("Application not found");
                }

                // If the application exists, update its IsSubmitted status to true
                existingApplication.IsSubmitted = true;

                // Update the application's IsSubmitted status and SubmissionDate
                var updateQuery = @"UPDATE Applications SET IsSubmitted = 1, SubmissionDate = @SubmissionDate WHERE guid = @Id";
                var affectedRows = await connection.ExecuteAsync(updateQuery, new { Id = id, SubmissionDate = DateTime.UtcNow });

                if (affectedRows == 0)
                {
                    return BadRequest("Failed to update application status");
                }

                // Refresh the application object to reflect the changes made in the database
                existingApplication = await connection.QueryFirstOrDefaultAsync<Application>("SELECT * FROM Applications WHERE guid = @Id", new { Id = id });

                return Ok(existingApplication);
            }
        }


    }
}
