using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyManagerApi.Controllers.Base;
using MoneyManagerApi.Models.API.CostType;
using MoneyManagerApi.Models.API.Currency;
using MoneyManagerApi.Models.API.Error;
using MoneyManagerApi.Models.Db;
using MoneyManagerApi.Services.Db;

namespace MoneyManagerApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("currency")]
    public class CurrencyController : UserController
    {
        public CurrencyController(PostgreSqlDbContext dbContext) : base(dbContext)
        { }

        [HttpPost("add")]
        public IActionResult PostCurrency([FromBody] CurrencyRequest request)
        {
            if (!string.IsNullOrEmpty(request.Name))
            {
                var currency = new Currency() { Name = request.Name, Symbol = request.Symbol };
                dbContext.Add(currency);
                dbContext.SaveChanges();
                return Ok(new CurrencyResponse()
                {
                    CurrencyId = currency.Id,
                    Name = currency.Name,
                    Symbol = currency.Symbol
                });
            }
            return BadRequest(new IncorrectData());
        }

        [HttpGet("all")]
        public IActionResult GetCurrencies()
        {
            var currenciesResponse = new List<CurrencyResponse>();
            dbContext.Currencies
                .ToList()
                .ForEach((currency) =>
                {
                    currenciesResponse.Add(new CurrencyResponse()
                    {
                        CurrencyId = currency.Id,
                        Name = currency.Name,
                        Symbol = currency.Symbol
                    });
                });
            return Ok(currenciesResponse);
        }
    }
}
