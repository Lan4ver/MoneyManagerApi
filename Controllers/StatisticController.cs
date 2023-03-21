using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyManagerApi.Controllers.Base;
using MoneyManagerApi.Models.API.Statistic;
using MoneyManagerApi.Models.Db;
using MoneyManagerApi.Services.Db;
using System.ComponentModel.DataAnnotations;

namespace MoneyManagerApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("statistic")]
    public class StatisticController : UserController
    {
        private const string CASH_IMAGE = "https://rwzydhznyespratlabcw.supabase.co/storage/v1/object/public/images/cash.png";

        public StatisticController(PostgreSqlDbContext dbContext) : base(dbContext)
        {
        }

        [HttpGet("balance")]
        public IActionResult GetTotalBalance()
        {
            var incomes = GetUserIncomes();
            var costs = GetUserCosts();
            var incomesSum = incomes.Sum((income) => income.Sum);
            var costsSum = costs.Sum((cost) => cost.Sum);
            return Ok(new TotalBalanceResponse() { TotalBalance = incomesSum - costsSum });
        }

        [HttpGet("items")]
        public IActionResult GetStatisticByDate([Required] DateTime dateFrom, [Required] DateTime dateTo, int? walletId = null)
        {
            var result = new List<StatisticItemResponse>();
            var costs = new List<Cost>();
            if (walletId == null)
                costs = GetUserCosts().Where((cost) => cost.Date >= dateFrom && cost.Date <= dateTo).ToList();
            else
                costs = GetUserCosts().Where((cost) => cost.WalletId == walletId && cost.Date >= dateFrom && cost.Date <= dateTo).ToList();
            var costsSum = costs.Sum((cost) => cost.Sum);
            dbContext.CostTypes
                .ToList()
                .ForEach(costType =>
                {
                    var sum = costs.Where((cost) => cost.CostTypeId == costType.Id).Sum((cost) => cost.Sum);
                    var percent = "0 %";
                    if (costsSum != 0)
                        percent = $"{Math.Round((sum / costsSum) * 100)} %";
                    result.Add(
                            new StatisticItemResponse()
                            {
                                Name = costType.Name,
                                Amount = sum,
                                Percent = percent,
                                Image = costType.Image,
                                Color = costType.Color
                            });
                });
            return Ok(result.OrderByDescending(item => item.Amount).ToList());
        }

        [HttpGet("transactions/last")]
        public IActionResult GetLastTransactions(int limit = 5)
        {
            var result = new List<TransactionItemResponse>();

            var costs = GetUserCosts().OrderByDescending(cost => cost.Date).Take(limit);
            var incomes = GetUserIncomes().OrderByDescending(income => income.Date).Take(limit);

            var topVariantsDates = costs.Select(cost => cost.Date).ToList();
            topVariantsDates.AddRange(incomes.Select(income => income.Date));
            var limitedDates = topVariantsDates.OrderByDescending(date => date).Take(limit).ToList();

            var selectedCosts = costs.Where(cost => limitedDates.Contains(cost.Date)).ToList();
            var selectedIncomes = incomes.Where(incomes => limitedDates.Contains(incomes.Date)).ToList();

            selectedCosts.ForEach(cost => result.Add(new TransactionItemResponse()
            {
                Name = cost.Name,
                Amount = cost.Sum,
                DateTime = cost.Date.ToString("s"),
                TypeImage = cost.CostType.Image,
                TransactionType = "cost",
                TransactionId = cost.Id
            })
            );

            selectedIncomes.ForEach(income => result.Add(new TransactionItemResponse()
            {
                Name = income.Name,
                Amount = income.Sum,
                DateTime = income.Date.ToString("s"),
                TypeImage = CASH_IMAGE,
                TransactionType = "income",
                TransactionId = income.Id
            })
            );

            return Ok(result.OrderByDescending(transaction => transaction.DateTime).ToList());
        }

        [HttpGet("transactions/all")]
        public IActionResult GetAllTransactionsByDate([Required] DateTime dateFrom, [Required] DateTime dateTo, int? walletId = null, int limit = 20, int offset = 0)
        {
            var result = new List<TransactionItemResponse>();

            var costs = new List<Cost>();
            if (walletId == null)
                costs = GetUserCosts().Where(cost => cost.Date > dateFrom && cost.Date < dateTo).ToList();
            else
                costs = GetUserCosts().Where(cost => cost.Date > dateFrom && cost.Date < dateTo && cost.WalletId == walletId).ToList();


            var incomes = new List<Income>();
            if (walletId == null)
                incomes = GetUserIncomes().Where(income => income.Date > dateFrom && income.Date < dateTo).ToList();
            else
                incomes = GetUserIncomes().Where(income => income.Date > dateFrom && income.Date < dateTo && income.WalletId == walletId).ToList();

            var topVariantsDates = costs.Select(cost => cost.Date).ToList();
            topVariantsDates.AddRange(incomes.Select(income => income.Date));
            topVariantsDates.OrderBy(variants => variants).Take(limit).ToList();
            var selectedCosts = costs.Where(cost => topVariantsDates.Contains(cost.Date)).ToList();
            var selectedIncomes = incomes.Where(incomes => topVariantsDates.Contains(incomes.Date)).ToList();

            selectedCosts.ForEach(cost => result.Add(new TransactionItemResponse()
            {
                TransactionId = cost.Id,
                Name = cost.Name,
                Amount = cost.Sum,
                DateTime = cost.Date.ToString("s"),
                TypeImage = cost.CostType.Image,
                TransactionType = "cost"
            })
            );

            selectedIncomes.ForEach(incomes => result.Add(new TransactionItemResponse()
            {
                TransactionId = incomes.Id,
                Name = incomes.Name,
                Amount = incomes.Sum,
                DateTime = incomes.Date.ToString("s"),
                TypeImage = CASH_IMAGE,
                TransactionType = "income"
            })
            );

            return Ok(result.OrderByDescending(transaction => transaction.DateTime).Skip(offset).Take(limit));
        }
    }
}
