using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ExactOnline.CustomAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VERPS.WebApp.Areas.ExactOnline.Models.SupplierConfig;
using VERPS.WebApp.Database;
using VERPS.WebApp.Database.Models;
using VERPS.WebApp.Helpers;
using VERPS.WebApp.Models.General;

namespace VERPS.WebApp.Areas.ExactOnline.Controllers
{
    [Authorize]
    [Area("ExactOnline")]
    public class SupplierConfigController : Controller
    {
        private readonly VERPSDBContext _context;
        private readonly ExactHelper _exactHelper;
        private readonly DBHelper _dbHelper;

        public SupplierConfigController(VERPSDBContext context)
        {
            _context = context;
            _exactHelper = new ExactHelper();
            _dbHelper = new DBHelper();
        }

        public StateMessage StateMessag { get; private set; }

        public async Task<IActionResult> Index()
        {

            // Get user info
            var user = _dbHelper.GetUser(_context, User.Identity.Name);

            // Set exact info
            ExactOnlineConnect.data.Context = _context;
            ExactOnlineConnect.data.AccessToken = user.Token;

            // Get data
            var suppliers = await ExactOnlineConnect.data.GetSuppliers(user.DivisionId);
            var supList = _exactHelper.SuppliersToSelectList(suppliers);

            var setSuppliers = _dbHelper.GetExactSupplierConfigs(_context, user.Id);

            var vm = new IndexVM
            {
                UserID = user.Id,
                ExactSupplierConfigs = setSuppliers,
                SuppliersSelectList = supList,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SupplierConfigForm(SupplierConfigVM vm = null)
        {

            // Get user info
            var user = _dbHelper.GetUser(_context, User.Identity.Name);

            // Set Exact data
            ExactOnlineConnect.data.Context = _context;
            ExactOnlineConnect.data.AccessToken = user.Token;



            ExactSupplierConfig config = new ExactSupplierConfig();

            if (vm.Id == 0)
            {
                // Get SupplierInfo
                var supplier = await ExactOnlineConnect.data.GetSupplier(user.DivisionId, vm.SupplierId);

                vm = new SupplierConfigVM();
                vm.Phone = supplier.Phone;
                vm.IsSet = false;
                vm.Website = supplier.Website;
                vm.VATNumber = supplier.VATNumber;
                vm.SupplierName = supplier.Name;
                vm.PostalCode = supplier.Postcode;
                vm.SupplierId = vm.SupplierId;
                vm.StateMessage = StateMessage.WarningNotCreatedYet;
            }
            else
            {
                config = _context.ExactSupplierConfigurations
                    .FirstOrDefault(x => x.Id == vm.Id);
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult ConfigureOrder(IFormFile file)
        {
            var vm = new OrderConfigVM();

            if (file != null)
            {
                // Check if XML
                if (file.ContentType == "text/xml")
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        vm.ListOfElements = StreamToListOfElements(reader.BaseStream);
                    }
                }
            }

            return View(vm);
        }


        public List<string> StreamToListOfElements(Stream xmlFile)
        {
            var elements = new List<string>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(xmlFile, settings);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        elements.Add(reader.Name);
                        break;
                }
            }

            return elements.Distinct().ToList();
        }

    }
}