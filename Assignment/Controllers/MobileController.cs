using Assignment.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileController : ControllerBase
    {

       
        private List<MobilePhone> _mobilePhones;
        private List<Sale> _sales;
        private List<Purchase> _purchases;

        public MobileController()
        {

            _mobilePhones = new List<MobilePhone> { };
            _sales = new List<Sale> { };
            _purchases = new List<Purchase> { };
        }

        #region get all data
        [HttpGet]
        public IActionResult Get()
        {
            var allMobilePhones = _mobilePhones.ToList();
            return Ok(allMobilePhones);
        }
        #endregion

        #region get data by id
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var mobilePhone = _mobilePhones.FirstOrDefault(m => m.Id == id);
            if (mobilePhone == null)
            {
                return NotFound();
            }
            return Ok(mobilePhone);
        }
        #endregion


        #region  add data
        [HttpPost]
        public IActionResult Post([FromBody] MobilePhone mobilePhone)
        {
            _mobilePhones.Add(mobilePhone);
            return Ok(mobilePhone);
        }
        #endregion

        #region update data
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] MobilePhone mobilePhone)
        {
            var existingMobilePhone = _mobilePhones.FirstOrDefault(m => m.Id == id);
            if (existingMobilePhone == null)
            {
                return NotFound();
            }
            existingMobilePhone.Name = mobilePhone.Name;
            existingMobilePhone.Brand = mobilePhone.Brand;
            // Update other properties as needed
            return Ok(existingMobilePhone);
        }
        #endregion


        #region delete data
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var mobilePhone = _mobilePhones.FirstOrDefault(m => m.Id == id);
            if (mobilePhone == null)
            {
                return NotFound();
            }
            _mobilePhones.Remove(mobilePhone);
            return Ok("Data deleted Sucessfully");
        }
        #endregion


        #region add bulk data
        [HttpPost("bulk-insert")]
        public IActionResult BulkInsert([FromBody] List<MobilePhone> mobilePhones)
        {
           
            _mobilePhones.AddRange(mobilePhones);
            return Ok(mobilePhones);
        }
        #endregion

        #region update bulk data
        [HttpPut("bulk-update")]
        public IActionResult BulkUpdate([FromBody] List<MobilePhone> mobilePhones)
        {
           
            foreach (var phone in mobilePhones)
            {
                var existingMobilePhone = _mobilePhones.FirstOrDefault(m => m.Id == phone.Id);
                if (existingMobilePhone != null)
                {
                    // Update existing phone properties
                    existingMobilePhone.Name = phone.Name;
                    existingMobilePhone.Brand = phone.Brand;
                    // Update other properties as needed
                }
            }
            return Ok(_mobilePhones);
        }
        #endregion

        #region monthly sales report
        [HttpGet("monthly-mobile-sales-report")]
        public IActionResult GetMonthlyMobileSalesReport(DateTime fromDate, DateTime toDate)
        {
            // Implement logic to view monthly sale of mobile sales report
            var monthlyMobileSales = _sales.Where(s => s.Date >= fromDate && s.Date <= toDate && s.Product.StartsWith("Mobile"))
                                          .GroupBy(s => new { Year = s.Date.Year, Month = s.Date.Month })
                                          .Select(g => new
                                          {
                                              Year = g.Key.Year,
                                              Month = g.Key.Month,
                                              TotalSales = g.Sum(s => s.Amount)
                                          });
            return Ok(monthlyMobileSales);
        }
        #endregion

        #region monthly brand wise report
        [HttpGet("monthly-brand-wise-sales-report")]
        public IActionResult GetMonthlyBrandWiseSalesReport(DateTime fromDate, DateTime toDate)
        {
            // Implement logic to view monthly brand-wise mobile sales report
            var monthlyBrandWiseSales = _sales.Where(s => s.Date >= fromDate && s.Date <= toDate && s.Product.StartsWith("Mobile"))
                                              .GroupBy(s => new { Year = s.Date.Year, Month = s.Date.Month, Brand = s.Product })
                                              .Select(g => new
                                              {
                                                  Year = g.Key.Year,
                                                  Month = g.Key.Month,
                                                  Brand = g.Key.Brand,
                                                  TotalSales = g.Sum(s => s.Amount)
                                              });
            return Ok(monthlyBrandWiseSales);
        }
        #endregion

        #region profit loss report
        [HttpGet("profit-loss-report")]
        public IActionResult GetProfitLossReport(DateTime fromDate, DateTime toDate)
        {
            // Implement logic to view profit/loss report with discounts
            var monthlyProfitLossReport = _purchases.Where(p => p.Date >= fromDate && p.Date <= toDate)
                                                    .GroupBy(p => new { Year = p.Date.Year, Month = p.Date.Month })
                                                    .Select(g => new
                                                    {
                                                        Year = g.Key.Year,
                                                        Month = g.Key.Month,
                                                        ProfitLoss = g.Sum(p => p.Amount - p.Discount)
                                                    });
            return Ok(monthlyProfitLossReport);
        }
        #endregion



    }
}
