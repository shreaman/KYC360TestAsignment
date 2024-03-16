using KYC360TestAsignment.Database;
using KYC360TestAsignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace KYC360TestAsignment.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class curdOperation : ControllerBase
    {
        private readonly DBconnect dbconect;
        private Guid id;


        //Get Method to get the entity
        public curdOperation(DBconnect dbconect)
        {
            this.dbconect = dbconect;
        }
        [HttpGet("Get All Entities")]
        public async Task<IActionResult> getEntity()
        {
            var entities = await dbconect.entitys
                .Include(e => e.Addresses)
                .Include(e => e.Dates)
                .Include(e => e.Names)
                .ToListAsync();
            return Ok(entities);
        }


        //Get the element by id
        [HttpGet]
        [Route("Get Entity By Id{id}")]

        public async Task<IActionResult> getById([FromRoute] string id)
        {
            var connect = await dbconect.entitys
                .Include(e => e.Addresses)
                .Include(e => e.Dates)
                .Include(e => e.Names)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (connect != null)
            {
                return Ok(connect);
            }
            else
            {
                return NotFound("No Entity exists for the ID " + id);
            }
        }

        //Search Entities
        [HttpGet("Search Entity")]
        public async Task<IActionResult> serchEntity([FromQuery] string search)
        {
            IQueryable<Entity> query = dbconect.entitys
                .Include(e => e.Addresses)
                .Include(e => e.Names);

            if (string.IsNullOrEmpty(search))
            {
                return NotFound("Not Found");
            }
            query = query.Where(e => e.Addresses.Any(a => a.Country.Contains(search) || a.AddressLine.Contains(search)) || e.Names.Any(n => n.FirstName.Contains(search) || n.MiddleName.Contains(search) || n.Surname.Contains(search)));
            var entities = await query.ToListAsync();
            return Ok(entities);
        }


        //Advance Filtering
        [HttpGet("Advance Filtering")]
        public async Task<IActionResult> FilterEntities(
            [FromQuery] string gender,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string[] countries)
        {
            IQueryable<Entity> query = dbconect.entitys
                .Include(e => e.Addresses)
                .Include(e => e.Dates)
                .Include(e => e.Names);

            // Apply gender filter if provided
            if (!string.IsNullOrEmpty(gender))
            {
                query = query.Where(e => e.Gender == gender);
            }

            // Apply date range filter if start date and end date are provided
            if (startDate.HasValue && endDate.HasValue)
            {
                query = ApplyDateRangeFilter(query, startDate.Value, endDate.Value);
            }

            // Apply country filter if countries are provided
            if (countries != null && countries.Length > 0)
            {
                query = ApplyCountryFilter(query, countries);
            }

            var filteredEntities = await query.ToListAsync();
            return Ok(filteredEntities);
        }

        private IQueryable<Entity> ApplyDateRangeFilter(
            IQueryable<Entity> query, DateTime startDate, DateTime endDate)
        {
            return query.Where(e => e.Dates.Any(d => d.date >= startDate && d.date <= endDate));
        }

        private IQueryable<Entity> ApplyCountryFilter(
            IQueryable<Entity> query, string[] countries)
        {
            return query.Where(e => e.Addresses.Any(a => countries.Contains(a.Country)));
        }


        //Insert A new entity
        [HttpPost("Insert Entity")]
        public async Task<IActionResult> addEntity(Entity entity)
        {
            // Accessing variables within the Entity class
            id = Guid.NewGuid();
            bool deceased = entity.Deceased;
            string gender = entity.Gender;

            // Accessing variables within the Address class (assuming there's only one address)
            string addressLine = entity.Addresses.FirstOrDefault().AddressLine;
            string city = entity.Addresses.FirstOrDefault()?.City;
            string country = entity.Addresses.FirstOrDefault()?.Country;

            // Accessing variables within the Name class (assuming there's only one name)
            string firstName = entity.Names.FirstOrDefault()?.FirstName;
            string middleName = entity.Names.FirstOrDefault()?.MiddleName;
            string surname = entity.Names.FirstOrDefault()?.Surname;

            // Accessing variables within the Date class (assuming there's only one date)
            string dateType = entity.Dates.FirstOrDefault()?.DateType;
            DateTime dateValue = (DateTime)(entity.Dates.FirstOrDefault()?.date);


            int maxAttempt = 3;
            int currentAttempt = 0;
            TimeSpan intitialDelay = TimeSpan.FromSeconds(1);
            
            while(currentAttempt < maxAttempt)
            {
                try
                {
                    dbconect.entitys.AddAsync(entity);
                    await dbconect.SaveChangesAsync();

                    logInformation(currentAttempt + 1, intitialDelay, "success");
                    return Ok(entity);
                }
                catch (Exception ex)
                {
                    logInformation(currentAttempt + 1, intitialDelay, $"Error:{ex.Message}");

                    await Task.Delay(intitialDelay);
                    intitialDelay *= 2;

                    currentAttempt++;
                }
            }

            logInformation(maxAttempt, intitialDelay, "faield");
            return StatusCode(500, "failed");
        }

        private void logInformation(int attempt, TimeSpan delay, string status)
        {
            //getting the log information
            Console.WriteLine($"Retry attempt #{attempt}. Delay: {delay.TotalSeconds} seconds. Status: {status}");
        }


        //Update entity
        [HttpPut]
        [Route("Update Entity{id}")]
        public async Task<IActionResult> UpdateEntity([FromRoute]String id,[FromBody] Entity entity) 
        {
            var connect = await dbconect.entitys
                .Include(e => e.Addresses)
                .Include(e => e.Dates)
                .Include(e => e.Names)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (connect != null)
            {
                //Updating the address
                foreach (var address in connect.Addresses)
                {
                    var updateAddress = entity.Addresses.FirstOrDefault();
                    if (updateAddress != null)
                    {
                        address.AddressLine = updateAddress.AddressLine;
                        address.City = updateAddress.City;
                        address.Country = updateAddress.Country;
                    }

                    //updating the date
                    foreach (var date in entity.Dates)
                    {
                        var updateDate = entity.Dates.FirstOrDefault();
                        if (updateDate != null)
                        {
                            date.DateType = updateDate.DateType;
                            date.date = updateDate.date;
                        }
                    }

                    connect.Deceased = entity.Deceased;
                    connect.Gender = entity.Gender;

                    //upaditng Name
                    foreach (var name in entity.Names)
                    {
                        var updateName = entity.Names.FirstOrDefault(); 
                        if(updateName != null)
                        {
                            name.FirstName = updateName.FirstName;
                            name.MiddleName = updateName.MiddleName;
                            name.Surname = updateName.Surname;
                        }
                    }
                }
            }


            else
            {
                return NotFound();//return 400 if the no entity exist with that ID
            }

            await dbconect.SaveChangesAsync();  
            
            return Ok(entity);

        }

        //delete entity
        [HttpDelete]
        [Route("Delete Entity{id}")]
        public async Task<IActionResult> DeleteEntity([FromRoute]String id, [FromBody] Entity entity)
        {
            var connect = await dbconect.entitys.FindAsync(id);
            if(connect != null)
            {
                dbconect.Remove(connect);
                await dbconect.SaveChangesAsync();
                return Ok(connect);
                
            }

            return NotFound("No Entity for Id "+id);
               


            

        }

         


    }

}
