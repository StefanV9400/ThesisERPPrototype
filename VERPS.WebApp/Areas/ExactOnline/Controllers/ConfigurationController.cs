using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExactOnline.CustomAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using VERPS.WebApp.Areas.ExactOnline.Models.Configuration;
using VERPS.WebApp.Database;
using VERPS.WebApp.Helpers;
using VERPS.WebApp.Models.General;

namespace VERPS.WebApp.Areas.ExactOnline.Controllers
{
    [Authorize]
    [Area("ExactOnline")]
    public class ConfigurationController : Controller
    {
        private readonly VERPSDBContext _context;
        private readonly ExactHelper _exactHelper;
        private readonly DBHelper _dbHelper;
        private readonly IHtmlLocalizer<ConfigurationController> _loc;

        public ConfigurationController(VERPSDBContext context, IHtmlLocalizer<ConfigurationController> loc)
        {
            _context = context;
            _exactHelper = new ExactHelper();
            _dbHelper = new DBHelper();
            _loc = loc;
        }

        /// <summary>
        /// Configuration page for changing the user's Exact configurations
        /// </summary>
        /// <param name="sm">StateMessage</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(StateMessage sm = StateMessage.None)
        {
            // DB Info
            var user = _dbHelper.GetUser(_context, User.Identity.Name);
            var config = _dbHelper.GetExactConfig(_context, user.ConfigID);

            // Exact data
            ExactOnlineConnect.data.Context = _context;
            ExactOnlineConnect.data.AccessToken = user.Token;

            var suppliers = await ExactOnlineConnect.data.GetSuppliers(user.DivisionId);
            var divisions = await ExactOnlineConnect.data.GetDivisions(user.DivisionId);
            var itemgroups = await ExactOnlineConnect.data.GetItemGroups(user.DivisionId);
            var paymentConditions = await ExactOnlineConnect.data.GetPaymentConditions(user.DivisionId);

            var vm = new Areas.ExactOnline.Models.Configuration.ConfigVM
            {
                ConfigurationId = config.Id,
                ItemGroupId = config.ItemGroupId.ToString(),
                ItemGroups = _exactHelper.ItemGroupsToSelectList(itemgroups),
                PaymentConditionId = config.PaymentConditionId,
                PaymentConditions = _exactHelper.PaymentConditionsToSelectList(paymentConditions),
                SupplierId = config.SupplierId.ToString(),
                Suppliers = _exactHelper.SuppliersToSelectList(suppliers),
                DivisionId = user.DivisionId.ToString(),
                Divisions = _exactHelper.DivisionsToSelectList(divisions),
                UserId = user.Id,
                StateMessage = sm,
                ConfigType = config.ConfigType,
            };

            return View(vm);
        }
        /// <summary>
        /// Saving changed configurations
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveConfiguration(ConfigVM vm)
        {
            // Get DB info
            var config = _context.ExactConfigurations
                .FirstOrDefault(x => x.Id == vm.ConfigurationId);

            config.ItemGroupId = new Guid(vm.ItemGroupId);
            config.DivsionId = int.Parse(vm.DivisionId);
            config.BuyerId = new Guid(vm.UserId);
            config.PaymentConditionId = vm.PaymentConditionId;
            config.Modified = DateTime.Now;
            config.SupplierId = vm.SupplierId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { sm = StateMessage.SuccessSave });
        }

    }
}