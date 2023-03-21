using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyManagerApi.Models.Db;
using MoneyManagerApi.Services.Db;
using System.Security.Claims;

namespace MoneyManagerApi.Controllers.Base
{
    public abstract class UserController : ControllerBase
    {
        protected PostgreSqlDbContext dbContext;

        public UserController(PostgreSqlDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        protected long GetUserId() => Convert.ToInt64(User.FindFirstValue(ClaimTypes.NameIdentifier));

        protected List<Wallet> GetUserWallets()
        {
            var id = GetUserId();
            return dbContext.Wallets
                .Where(wallet => wallet.UserId == id)
                .Include(wallet => wallet.Incomes)
                .Include(wallet => wallet.Costs)
                .Include(wallet => wallet.Currency)
                .ToList();
        }

        protected List<Income> GetUserIncomes()
        {
            var id = GetUserId();
            var incomes = dbContext.Wallets.Where(wallet => wallet.UserId == id).Include(wallet => wallet.Incomes).Select(wallet => wallet.Incomes).ToList();
            var result = new List<Income>();
            incomes.ForEach(incomeList => { incomeList.ToList().ForEach(income => { result.Add(income); }); });
            return result;
        }

        protected List<Cost> GetUserCosts()
        {
            var id = GetUserId();
            var costs = dbContext.Wallets
                .Where(wallet => wallet.UserId == id)
                .Include(wallet => wallet.Costs)
                .SelectMany(wallet => wallet.Costs)
                .Include(cost => cost.CostType)
                .ToList();
            return costs;
        }

        protected List<Income> GetUserIncomesByWalletId(long walletId)
        {
            return dbContext.Incomes.Where(wallet => wallet.WalletId == walletId).ToList();
        }
    }
}
