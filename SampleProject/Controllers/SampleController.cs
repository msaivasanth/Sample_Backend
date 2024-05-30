using Microsoft.AspNetCore.Mvc;
using SampleProject.Data;
using SampleProject.Models;

namespace SampleProject.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public SampleController(ApplicationDbContext db)
        {
            // Access to the database.
            _db = db;
        }

        // Sample test api.
        [HttpGet("/")]
        public IActionResult Get() {

            return Ok("Hello World");
        }

        // Fetch all the records from the table.
        [HttpGet("getUsers")]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<Villa>> GetUsers() {
            return Ok(_db.Villas.ToList());
        }

        // To fetch the details of a single record from sample table.
        [HttpGet("getUser/{id:int}")]
        [ProducesResponseType(200)]
        public ActionResult<Villa> GetUser(int id) {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null) {
                return NotFound();
            }
            return Ok(villa);
        }
        
        // To add a new record in a sample table.
        [HttpPost("addNewUser")]
        [ProducesResponseType(200)]
        public ActionResult<Villa> CreateUser([FromBody] Villa villa)
        {
            if (_db.Villas.FirstOrDefault(v => v.Name.ToLower() == villa.Name.ToLower()) != null) {
                ModelState.AddModelError("", "Name already exits!");
                return BadRequest(ModelState);
            }
            if (villa == null) { return BadRequest(); }

            _db.Villas.Add(villa);
            _db.SaveChanges();

            return Ok(villa);
        }

        // Delete a record from the table by Id.
        [HttpDelete("delete/{id:int}")]
        public IActionResult DeleteUser(int id) {
            if (id == 0) { return BadRequest(); };

            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null) { return NotFound(); };

            _db.Villas.Remove(villa);
            _db.SaveChanges();


            return NoContent();
        }

        // Update a record by Id.
        [HttpPut("update/{id:int}")]
        [ProducesResponseType(200)]
        public IActionResult UpdateUser(int id, [FromBody]Villa villa) {
            if (villa == null || id != villa.Id) { return BadRequest(); }

            _db.Villas.Update(villa);
            _db.SaveChanges();

            return Ok(villa);
        }
    }
}
