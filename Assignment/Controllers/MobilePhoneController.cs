using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Assignment.Model;

namespace MobilePhoneStore.Controllers
{
    public class MobilePhoneController : ApiController
    {
        private static List<MobilePhone> mobilePhones = new List<MobilePhone>();

        #region // GET api/MobilePhone
        [HttpGet]
        [Authorize]
        public IEnumerable<MobilePhone> Get()
        {
            return mobilePhones;
        }
        #endregion


        #region // POST api/MobilePhone
        [HttpPost]
        [Authorize]
        public IHttpActionResult Post([FromBody] MobilePhone mobilePhone)
        {
            mobilePhones.Add(mobilePhone);
            return CreatedAtRoute("MobilePhone", new { id = mobilePhone.Id }, mobilePhone);
        }
        #endregion
        #region // PUT api/MobilePhone/5
        [HttpPut]
        [Authorize]
        public IHttpActionResult Put(int id, [FromBody] MobilePhone mobilePhone)
        {
            var existingMobilePhone = mobilePhones.FirstOrDefault(m => m.Id == id);
            if (existingMobilePhone == null)
            {
                return NotFound();
            }

            // Update the properties
            // existingMobilePhone.Name = mobilePhone.Name;
            // existingMobilePhone.Brand = mobilePhone.Brand;
            // ...

            return StatusCode(HttpStatusCode.NoContent);
        }
        #endregion

        #region  // DELETE api/MobilePhone/5
        [HttpDelete]
        [Authorize]
        public IHttpActionResult Delete(int id)
        {
            var mobilePhone = mobilePhones.FirstOrDefault(m => m.Id == id);
            if (mobilePhone == null)
            {
                return NotFound();
            }

            mobilePhones.Remove(mobilePhone);
            return Ok(mobilePhone);
        }
        #endregion

        #region
        [HttpPost]
        [Route("api/MobilePhone/Authenticate")]
        public IHttpActionResult Authenticate(User user)
        {
            // Check if the user exists in the database
            // Add your own authentication logic here

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("YourSecretKeyHere");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
        #endregion


        #region // POST api/MobilePhone/BulkInsert
        [HttpPost]
        [Authorize]
        [Route("api/MobilePhone/BulkInsert")]
        public IHttpActionResult BulkInsert(List<MobilePhone> mobilePhonesList)
        {
            // Check for authorization here if needed

            if (mobilePhonesList == null || mobilePhonesList.Count == 0)
            {
                return BadRequest("No mobile phones provided for bulk insert.");
            }

            mobilePhones.AddRange(mobilePhonesList);
            return Ok("Bulk insert successful.");
        }
        #endregion
        #region  // PUT api/MobilePhone/BulkUpdate
        [HttpPut]
        [Authorize]
        [Route("api/MobilePhone/BulkUpdate")]
        public IHttpActionResult BulkUpdate(List<MobilePhone> mobilePhonesList)
        {
            // Check for authorization here if needed

            if (mobilePhonesList == null || mobilePhonesList.Count == 0)
            {
                return BadRequest("No mobile phones provided for bulk update.");
            }

            foreach (var mobilePhone in mobilePhonesList)
            {
                var existingMobilePhone = mobilePhones.FirstOrDefault(m => m.Id == mobilePhone.Id);
                if (existingMobilePhone != null)
                {
                    // Update the properties
                    // existingMobilePhone.Name = mobilePhone.Name;
                    // existingMobilePhone.Brand = mobilePhone.Brand;
                    // ...
                }
            }

            return Ok("Bulk update successful.");
        }
        #endregion




#region // GET api/MobilePhone/MonthlySalesReport
        [HttpGet]
        [Authorize]
        [Route("api/MobilePhone/MonthlySalesReport")]
        public IHttpActionResult MonthlySalesReport(DateTime fromDate, DateTime toDate)
        {
            // Check for authorization here if needed

            if (fromDate > toDate)
            {
                return BadRequest("Invalid date range. 'From' date should be before 'To' date.");
            }

            var salesReport = mobilePhones
                .Where(m => m.PurchaseDate >= fromDate && m.PurchaseDate <= toDate)
                .ToList();

            return Ok(salesReport);
        }
        #endregion

        #region // GET api/MobilePhone/MonthlyBrandWiseSalesReport
        [HttpGet]
        [Authorize]
        [Route("api/MobilePhone/MonthlyBrandWiseSalesReport")]
        public IHttpActionResult MonthlyBrandWiseSalesReport(DateTime fromDate, DateTime toDate)
        {
            // Check for authorization here if needed

            if (fromDate > toDate)
            {
                return BadRequest("Invalid date range. 'From' date should be before 'To' date.");
            }

            var brandWiseSalesReport = mobilePhones
                .Where(m => m.PurchaseDate >= fromDate && m.PurchaseDate <= toDate)
                .GroupBy(m => m.Brand)
                .Select(group => new
                {
                    Brand = group.Key,
                    Sales = group.ToList()
                })
                .ToList();

            return Ok(brandWiseSalesReport);
        }
        #endregion



        #region  // GET api/MobilePhone/ProfitLossReport
        [HttpGet]
        [Authorize]
        [Route("api/MobilePhone/ProfitLossReport")]
        public IHttpActionResult ProfitLossReport(DateTime currentDate, DateTime previousDate)
        {
            // Check for authorization here if needed

            if (currentDate <= previousDate)
            {
                return BadRequest("Invalid date range. 'Current' date should be after 'Previous' date.");
            }

            var currentPeriodSales = mobilePhones
                .Where(m => m.PurchaseDate >= previousDate && m.PurchaseDate <= currentDate)
                .ToList();

            double currentPeriodRevenue = 0;
            double currentPeriodDiscounts = 0;
            foreach (var sale in currentPeriodSales)
            {
                currentPeriodRevenue += sale.Price;
                currentPeriodDiscounts += sale.Discount;
            }

            var previousPeriodSales = mobilePhones
                .Where(m => m.PurchaseDate < previousDate)
                .ToList();

            double previousPeriodRevenue = 0;
            double previousPeriodDiscounts = 0;
            foreach (var sale in previousPeriodSales)
            {
                previousPeriodRevenue += sale.Price;
                previousPeriodDiscounts += sale.Discount;
            }

            double currentPeriodProfitLoss = currentPeriodRevenue - currentPeriodDiscounts;
            double previousPeriodProfitLoss = previousPeriodRevenue - previousPeriodDiscounts;

            var report = new
            {
                CurrentPeriod = new
                {
                    Revenue = currentPeriodRevenue,
                    Discounts = currentPeriodDiscounts,
                    ProfitLoss = currentPeriodProfitLoss
                },
                PreviousPeriod = new
                {
                    Revenue = previousPeriodRevenue,
                    Discounts = previousPeriodDiscounts,
                    ProfitLoss = previousPeriodProfitLoss
                }
            };

            return Ok(report);
        }
#endregion
    }
}
