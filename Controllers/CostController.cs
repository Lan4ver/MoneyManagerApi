using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyManagerApi.Controllers.Base;
using MoneyManagerApi.Models.API.Cost;
using MoneyManagerApi.Models.API.Error;
using MoneyManagerApi.Models.API.Wallet;
using MoneyManagerApi.Models.Db;
using MoneyManagerApi.Services.Db;
using System.ComponentModel.DataAnnotations;

namespace MoneyManagerApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("cost")]
    public class CostController : UserController
    {
        public CostController(PostgreSqlDbContext dbContext) : base(dbContext)
        {
        }

        [HttpPost("add")]
        public IActionResult PostCost([FromBody] CostRequest costRequest)
        {
            var wallets = GetUserWallets();
            var wallet = wallets.First(wallet => wallet.Id == costRequest.WalletId);
            if (!string.IsNullOrEmpty(costRequest.Name) && costRequest.Sum != 0 && wallet != null)
            {
                if (GetUserIncomes().Sum((income) => income.Sum) < (GetUserCosts().Sum((cost) => cost.Sum)) + costRequest.Sum)
                    return BadRequest(new MessageError("Error", "Insufficient funds"));

                var cost = new Cost() { Name = costRequest.Name, Date = costRequest.Date, Sum = costRequest.Sum, WalletId = costRequest.WalletId, CostTypeId = costRequest.CostTypeId };
                dbContext.Add(cost);
                dbContext.SaveChanges();
                return Ok(new CostResponse()
                {
                    CostId = cost.Id,
                    WalletId = cost.WalletId,
                    Name = cost.Name,
                    Date = cost.Date.ToString("s"),
                    Sum = cost.Sum,
                    CostTypeId = cost.CostTypeId
                });
            }
            return BadRequest(new IncorrectData());
        }

        [HttpGet]
        public IActionResult GetCost([Required] int costId)
        {
            var cost = GetUserCosts().FirstOrDefault((cost) => cost.Id == costId);
            if (cost == null)
                return BadRequest(new NoExistsError());
            return Ok(new CostResponse()
            {
                CostId = cost.Id,
                WalletId = cost.WalletId,
                Name = cost.Name,
                Date = cost.Date.ToString("s"),
                Sum = cost.Sum,
                CostTypeId = cost.CostTypeId
            });
        }

        [HttpGet("all")]
        public IActionResult GetCosts()
        {
            var id = GetUserId();
            var costs = new List<CostResponse>();
            GetUserCosts().ForEach(cost =>
            {
                costs.Add(new CostResponse()
                {
                    CostId = cost.Id,
                    WalletId = cost.WalletId,
                    Name = cost.Name,
                    Date = cost.Date.ToString("s"),
                    Sum = cost.Sum,
                    CostTypeId = cost.CostTypeId
                });
            });
            return Ok(costs);
        }

        [HttpPut("update")]
        public IActionResult UpdateCost([FromBody] CostUpdateRequest costRequest)
        {
            var costToChange = GetUserCosts().FirstOrDefault(cost => cost.Id == costRequest.CostId);
            if (costToChange != null)
            {
                costToChange.Name = costRequest.Name;
                costToChange.Sum = costRequest.Sum;
                costToChange.Date = costRequest.Date;
                costToChange.CostTypeId = costRequest.CostTypeId;
                dbContext.Update(costToChange);
                dbContext.SaveChanges();
                return Ok(new CostResponse()
                {
                    CostId = costToChange.Id,
                    Name = costToChange.Name,
                    Date = costToChange.Date.ToString("s"),
                    Sum = costToChange.Sum,
                    CostTypeId = costToChange.CostTypeId
                });

            }
            else
            {
                return BadRequest(new NoExistsError());
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteCost([Required] int costId)
        {
            var id = GetUserId();
            var cost = GetUserCosts().FirstOrDefault(cost => cost.Id == costId);
            if (cost != null)
            {
                dbContext.Remove(cost);
                dbContext.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest(new NoExistsError());
            }
        }
    }
}

