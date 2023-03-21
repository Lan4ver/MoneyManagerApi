using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyManagerApi.Controllers.Base;
using MoneyManagerApi.Models.API.CostType;
using MoneyManagerApi.Models.API.Error;
using MoneyManagerApi.Models.API.Wallet;
using MoneyManagerApi.Models.Db;
using MoneyManagerApi.Services.Db;

namespace MoneyManagerApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("cost/type")]
    public class CostTypeController : UserController
    {
        public CostTypeController(PostgreSqlDbContext dbContext) : base(dbContext)
        { }

        [HttpPost("add")]
        public IActionResult PostCostType([FromBody] CostTypeRequest request)
        {
            if (!string.IsNullOrEmpty(request.Name))
            {
                var costType = new CostType() { Name = request.Name };
                dbContext.Add(costType);
                dbContext.SaveChanges();
                return Ok(new CostTypeResponse()
                {
                    CostTypeId = costType.Id,
                    Name = costType.Name,
                    Image = costType.Image
                });
            }
            return BadRequest(new IncorrectData());
        }

        [HttpGet("all")]
        public IActionResult GetCostTypes()
        {
            var costTypesResponse = new List<CostTypeResponse>();
            dbContext.CostTypes
                .ToList()
                .ForEach((costType) =>
                {
                    costTypesResponse.Add(new CostTypeResponse()
                    {
                        CostTypeId = costType.Id,
                        Name = costType.Name,
                        Image = costType.Image
                    });
                });
            return Ok(costTypesResponse);
        }
    }
}
