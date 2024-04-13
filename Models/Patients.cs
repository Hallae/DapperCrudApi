namespace DapperCrudApi.Models
{
    public class Patients
    {

        public int id { get; set; }

        public int age { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CauseofDeath { get; set; }

        public string Address { get; set; }

        public string Nationality { get; set; }

        public DateTime TimeofDeath{get; set;} 

    }
}
