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
            using (var connection = _dbContext.Connection)
            {
                connection.Open();
                var patients = connection.Query<Patients>("SELECT * FROM Patients").ToList();
                return Ok(patients);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Patients> GetPatient(int id)
        {
            using (var connection = _dbContext.Connection)
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


        [HttpPost]
        public ActionResult<Patients> CreatePatient(Patients patient)
        {
            using (var connection = _dbContext.Connection)
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
