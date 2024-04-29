using Dapper;
using DapperCrudApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DapperCrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class PatientsController : ControllerBase
    {
        private readonly DatabaseContext _dbContext; 
        public PatientsController(DatabaseContext dbcontext)
        {
            _dbContext = dbcontext;
        }


        [HttpGet]
        public ActionResult<List<Patients>> Get()
        {
            using (var connection = _dbContext.DefaultConnection)
            {
                connection.Open();
                var patients = connection.Query<Patients>("SELECT * FROM Patients").ToList();
                return Ok(patients);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Patients> GetPatient(int id)
        {
            using (var connection = _dbContext.DefaultConnection)
            {
                connection.Open();
                var patient = connection.QueryFirstOrDefault<Patients>("SELECT * FROM Patients WHERE id = @Id", new { Id = id });
                if (patient == null)
                {
                    return NotFound();
                }
                return Ok(patient);
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeletePatient(int id)
        {
            using (var connection = _dbContext.DefaultConnection)
            {
                connection.Open();
                var query = "DELETE FROM Patients WHERE id = @Id";
                var affectedRows = connection.Execute(query, new { Id = id });

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPatch("Update")]
        public IActionResult UpdatePatient(int id, [FromBody] Patients updatedPatient)
        {
            using (var connection = _dbContext.DefaultConnection)
            {
                connection.Open();
                var query = @"UPDATE Patients 
                      SET age = @Age, FirstName = @FirstName, LastName = @LastName, CauseofDeath = @CauseofDeath, 
                          Address = @Address, Nationality = @Nationality, TimeofDeath = @TimeofDeath 
                      WHERE id = @Id";
                var affectedRows = connection.Execute(query, updatedPatient);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }




        [HttpPost]
        public ActionResult<Patients> CreatePatient(Patients patient)
        {
            using (var connection = _dbContext.DefaultConnection)
            {
                connection.Open();
                var query = "INSERT INTO Patients (age, FirstName, LastName, CauseofDeath, Address, Nationality, TimeofDeath) VALUES (@Age, @FirstName, @LastName, @CauseofDeath, @Address, @Nationality, @TimeofDeath); SELECT LASTVAL();";
                var patientId = connection.QuerySingle<int>(query, patient);
                patient.id = patientId;
                return CreatedAtAction(nameof(GetPatient), new { id = patient.id }, patient);
            }
        }
      


    }

}
