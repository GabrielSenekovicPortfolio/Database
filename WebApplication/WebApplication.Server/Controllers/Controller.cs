using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Database;

namespace PriceOptimizerAPI.Controllers
{
    [Route("api/prices")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly SkuDatabase database;

        public PriceController(SkuDatabase database)
        {
            this.database = database;
        }

        [HttpGet("all-skus")]
        public ActionResult<List<string>> GetAllSkus()
        {
            database.GetAllKeys(out var allSkus);
            return Ok(allSkus);
        }


        [HttpGet("{sku}")]
        public ActionResult<List<SkuDatabase.Entry>> GetOptimizedPrices(string sku)
        {
            if (database.TryGetValue(sku, out List<SkuDatabase.Entry> entries))
            {
                return Ok(entries);
            }
            return NotFound(new { message = "No prices found for the given SKU." });
        }
    }
}
