using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VERPS.WebApp.Database;
using VERPS.WebApp.Helpers;
using Microsoft.AspNetCore.Identity;
using VERPS.WebApp.Areas.ExactOnline.Models.Dashboard;
using VERPS.WebApp.Models.General;
using ExactOnline.CustomAuth;

namespace VERPS.WebApp.Areas.ExactOnline.Controllers
{
    [Authorize]
    [Area("ExactOnline")]
    public class DashboardController : Controller
    {
        private readonly VERPSDBContext _context;
        private readonly ExactHelper _exactHelper;
        private readonly DBHelper _dbHelper;

        public DashboardController(VERPSDBContext context)
        {
            _context = context;
            _exactHelper = new ExactHelper();
            _dbHelper = new DBHelper();
        }

        /// <summary>
        /// Homepage for the ExactUsers
        /// </summary>
        /// <param name="sm">StateMessage</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(StateMessage sm = StateMessage.None)
        {
            var user = _dbHelper.GetUser(_context, User.Identity.Name);
            if (user == null)
                return RedirectToAction("LogOut", "Account");


            var orders = _context.ExactOrders
                    .Where(x => x.DBUser.UserID == user.CreatorId)
                    .Select(x => new
                    {
                        x.Id,
                        x.IsStoredInExact,
                    })
                    .ToList();

            // Get Exact data
            ExactOnlineConnect.data.Context = _context;
            ExactOnlineConnect.data.AccessToken = user.Token;

            var suppliers = await ExactOnlineConnect.data.GetSuppliers(user.DivisionId);
            var sups = _exactHelper.SuppliersToSelectList(suppliers);
            var si = await ExactOnlineConnect.data.GetSupplierItems(user.DivisionId);

            var vm = new IndexVM
            {
                CompanyName = user.CompanyName,
                StateMessage = sm,
                AmountOpenOrders = orders
                    .Where(x => !x.IsStoredInExact)
                    .Count(),
                AmountSentOrders = orders
                    .Where(x => x.IsStoredInExact)
                    .Count(),
                ImportFileVM = new ImportFileVM
                {
                    Suppliers = sups,
                    SupplierId = new Guid(user.SupplierId),
                }
            };

            return View(vm);
        }


        public void RemoveAll()
        {
            var orders = _context
                .ExactOrders;
            
            
        }
    }
}