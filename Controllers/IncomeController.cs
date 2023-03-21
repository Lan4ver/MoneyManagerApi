using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyManagerApi.Controllers.Base;
using MoneyManagerApi.Models.API.Cost;
using MoneyManagerApi.Models.API.Error;
using MoneyManagerApi.Models.API.Income;
using MoneyManagerApi.Models.API.Wallet;
using MoneyManagerApi.Models.Db;
using MoneyManagerApi.Services.Db;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MoneyManagerApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("income")]
    public class IncomeController : UserController
    {
        public IncomeController(PostgreSqlDbContext dbContext) : base(dbContext) { }

        [HttpPost("add")]
        public IActionResult PostIncome([FromBody] IncomeRequest incomeRequest)
        {
            var wallets = GetUserWallets();
            if (!string.IsNullOrEmpty(incomeRequest.Name) && incomeRequest.Sum != 0 && wallets.Any(wallet => wallet.Id == incomeRequest.WalletId))
            {
                var income = new Income() { Name = incomeRequest.Name, Date = incomeRequest.Date, Sum = incomeRequest.Sum, WalletId = incomeRequest.WalletId };
                dbContext.Add(income);
                dbContext.SaveChanges();
                return Ok(new IncomeResponse()
                {
                    IncomeId = income.Id,
                    WalletId = income.WalletId,
                    Name = income.Name,
                    Date = income.Date.ToString("s"),
                    Sum = income.Sum
                });
            }
            return BadRequest(new IncorrectData());
        }

        [HttpGet]
        public IActionResult GetIncome([Required] int incomeId)
        {
            var income = GetUserIncomes().FirstOrDefault((cost) => cost.Id == incomeId);
            if (income == null)
                return BadRequest(new NoExistsError());
            return Ok(new IncomeResponse()
            {
                IncomeId = income.Id,
                WalletId = income.WalletId,
                Name = income.Name,
                Date = income.Date.ToString("s"),
                Sum = income.Sum
            });
        }

        [HttpGet("all")]
        public IActionResult GetIncomes()
        {
            var id = GetUserId();
            var incomes = new List<IncomeResponse>();
            GetUserIncomes().ForEach(income =>
            {
                incomes.Add(new IncomeResponse()
                {
                    IncomeId = income.Id,
                    WalletId = income.WalletId,
                    Name = income.Name,
                    Date = income.Date.ToString("s"),
                    Sum = income.Sum
                });
            });
            return Ok(incomes);
        }

        [HttpPut("update")]
        public IActionResult UpdateIncome([FromBody] IncomeUpdateRequest incomeRequest)
        {
            var incomeToChange = GetUserIncomes().FirstOrDefault(income => income.Id == incomeRequest.IncomeId);
            if (incomeToChange != null)
            {
                incomeToChange.Name = incomeRequest.Name;
                incomeToChange.Sum = incomeRequest.Sum;
                incomeToChange.Date = incomeRequest.Date;
                dbContext.Update(incomeToChange);
                dbContext.SaveChanges();
                return Ok(new IncomeResponse()
                {
                    IncomeId = incomeToChange.Id,
                    WalletId = incomeToChange.WalletId,
                    Name = incomeToChange.Name,
                    Date = incomeToChange.Date.ToString("s"),
                    Sum = incomeToChange.Sum
                });

            }
            else
            {
                return BadRequest(new NoExistsError());
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteIncome([Required] int incomeId)
        {
            var id = GetUserId();
            var income = GetUserIncomes().FirstOrDefault(income => income.Id == incomeId);
            if (income != null)
            {
                var wallet = GetUserWallets().FirstOrDefault(wallet => wallet.Incomes.Contains(income));
                if (wallet.Costs.Sum(cost => cost.Sum) > (wallet.Incomes.Sum(income => income.Sum) - income.Sum))
                    return BadRequest(new MessageError("Error", "Insufficient funds"));

                dbContext.Remove(income);
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
