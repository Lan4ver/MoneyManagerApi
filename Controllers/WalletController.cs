using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoneyManagerApi.Controllers.Base;
using MoneyManagerApi.Models.API.Error;
using MoneyManagerApi.Models.API.Wallet;
using MoneyManagerApi.Models.Db;
using MoneyManagerApi.Services.Db;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MoneyManagerApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("wallet")]
    public class WalletController : UserController
    {
        public WalletController(PostgreSqlDbContext dbContext) : base(dbContext)
        {
        }

        [HttpPost("add")]
        public IActionResult PostWallet([FromBody] WalletRequest walletRequest)
        {
            if (!walletRequest.Name.IsNullOrEmpty())
            {
                var wallet = new Wallet() { Name = walletRequest.Name, UserId = GetUserId(), CurrencyId = walletRequest.CurrencyId };
                dbContext.Add(wallet);
                dbContext.SaveChanges();
                return Ok(new WalletResponse() { WalletId = wallet.Id, Name = wallet.Name, CurrencyId = wallet.CurrencyId });
            }
            return BadRequest(new IncorrectData());
        }

        [HttpGet]
        public IActionResult GetWallet([Required] int walletId)
        {
            var wallet = GetUserWallets().FirstOrDefault(x => x.Id == walletId);
            if (wallet == null)
                return BadRequest(new NoExistsError());
            return Ok(new WalletResponse() { WalletId = wallet.Id, Name = wallet.Name, CurrencyId = wallet.CurrencyId });
        }

        [HttpGet("all")]
        public IActionResult GetWallets()
        {
            var wallets = new List<WalletResponse>();
            GetUserWallets().ForEach(wallet =>
            {
                wallets.Add(new WalletResponse() { WalletId = wallet.Id, Name = wallet.Name, CurrencyId = wallet.CurrencyId });
            });
            return Ok(wallets);
        }

        [HttpGet("all/details")]
        public IActionResult GetWalletsDetails()
        {
            var wallets = new List<WalletDetailsResponse>();
            GetUserWallets().ForEach(wallet =>
            {
                var income = wallet.Incomes.Sum(income => income.Sum);
                var outcome = wallet.Costs.Sum(outcome => outcome.Sum);
                var balance = income - outcome;
                wallets.Add(new WalletDetailsResponse()
                {
                    WalletId = wallet.Id,
                    Name = wallet.Name,
                    Income = income,
                    Outcome = outcome,
                    Balance = balance,
                    Currency = wallet.Currency.Symbol
                });
            });
            return Ok(wallets);
        }

        [HttpPut("update")]
        public IActionResult UpdateWallet([FromBody] WalletUpdateRequest walletRequest)
        {
            var walletToChange = GetUserWallets().FirstOrDefault(wallet => wallet.Id == walletRequest.WalletId);
            if (walletToChange != null)
            {
                walletToChange.Name = walletRequest.Name;
                dbContext.Update(walletToChange);
                dbContext.SaveChanges();
                return Ok(new WalletUpdateResponse() { WalletId = walletToChange.Id, Name = walletToChange.Name });
            }
            else
            {
                return BadRequest(new NoExistsError());
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteWallet([Required] int walletId)
        {
            var id = GetUserId();
            var wallet = dbContext.Wallets
                .Where(wallet => wallet.UserId == id)
                .Include(wallet => wallet.Incomes)
                .Include(wallet => wallet.Costs)
                .FirstOrDefault(wallet => wallet.Id == walletId);
            if (wallet != null)
            {
                dbContext.Remove(wallet);
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
