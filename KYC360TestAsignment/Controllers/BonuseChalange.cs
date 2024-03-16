using KYC360TestAsignment.Database;
using KYC360TestAsignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KYC360TestAsignment.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class BonuseChalange : ControllerBase
    {

        private readonly DBconnect dbconnect;

        public BonuseChalange(DBconnect dbConnect)
        {
            this.dbconnect = dbConnect;
        }

        [HttpGet]
        public async Task<IActionResult> paginationAndShorting(
            [FromQuery] int pageNo = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string shortBy = "Id",
            [FromQuery] string shortOrder = "asc")
        {
            IQueryable<Entity> query = dbconnect.entitys;

            // Sorting
            switch (shortBy.ToLower())
            {
                case "id":
                    query = shortOrder.ToLower() == "asc" ? query.OrderBy(e => e.Id) : query.OrderByDescending(e => e.Id);
                    break;
                case "name":
                    query = shortOrder.ToLower() == "asc" ? query.OrderBy(e => e.Names.FirstOrDefault().FirstName) : query.OrderByDescending(e => e.Names.FirstOrDefault().FirstName);
                    break;
                case "startdate":
                    query = shortOrder.ToLower() == "asc" ? query.OrderBy(e => e.Dates.FirstOrDefault().date) : query.OrderByDescending(e => e.Dates.FirstOrDefault().date);
                    break;
                case "address":
                    query = shortOrder.ToLower() == "asc" ? query.OrderBy(e => e.Addresses.FirstOrDefault().AddressLine) : query.OrderByDescending(e => e.Addresses.FirstOrDefault().AddressLine);
                    break;
                default:
                    return BadRequest("Invalid sort by parameter.");
            }

            // Applying Pagination 
            var totalSize = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalSize / (double)pageSize);
            var entities = await query.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new
            {
                TotalSize = totalSize,
                TotalPages = totalPages,
                CurrentPage = pageNo,
                PageSize = pageSize,
                Entities = entities
            });
        }
    }
}
