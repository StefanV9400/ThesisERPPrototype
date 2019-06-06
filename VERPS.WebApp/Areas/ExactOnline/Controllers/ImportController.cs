using ExactOnline.CustomAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using VERPS.WebApp.Areas.ExactOnline.Models.Import;
using VERPS.WebApp.Areas.ExactOnline.Models.OrderManagement.ExactOrderXML;
using VERPS.WebApp.Database;
using VERPS.WebApp.Database.Models;
using VERPS.WebApp.Helpers;
using VERPS.WebApp.Models.General;

namespace VERPS.WebApp.Areas.ExactOnline.Controllers
{
    [Authorize]
    [Area("ExactOnline")]
    public class ImportController : Controller
    {
        private readonly ILogger _logger;
        private readonly VERPSDBContext _context;
        private readonly DBHelper _dbHelper;
        private readonly ExactHelper _exactHelper;


        public ImportController(VERPSDBContext context, ILogger<ImportController> logger)
        {
            _logger = logger;
            _context = context;
            _dbHelper = new DBHelper();
            _exactHelper = new ExactHelper();
        }

        public IActionResult Index(DynamicXMLVM vm)
        {

            return View(vm);
        }

      
        /// <summary>
        /// Import a specfic XML into a a DB and Exact order
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ImportOrder(ImportFileVM vm)
        {
            // Pars XML into correct order 
            string req;
            OrderXML orderXml;
            using (var reader = new StreamReader(vm.File.OpenReadStream()))
            {
                req = reader.ReadToEnd();
            }

            using (var reader = new StringReader(req))
            {
                var serializer = new XmlSerializer(typeof(OrderXML));
                orderXml = serializer.Deserialize(reader) as OrderXML;
            }

            // Store order in the DB
            var user = _dbHelper.GetUser(_context, User.Identity.Name);
            var dbuser = _context.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            // XML to DB Order
            var order = _exactHelper.XMLToDBOrder(orderXml, user, dbuser, vm.SupplierId);

            // Get Exact data
            ExactOnlineConnect.data.Context = _context;
            ExactOnlineConnect.data.AccessToken = user.Token;

            // Get supplier
            var supplier = await ExactOnlineConnect.data.GetSupplier(user.DivisionId, vm.SupplierId.ToString());
            if (supplier == null)
                return RedirectToAction("Index", "Dashboard", new { sm = StateMessage.FailedMissingInfo });

            order.Supplier = _dbHelper.CreateOrGetSupplier(_context, supplier, user.CreatorId, _logger);

            foreach (var x in order.Lines)
            {
                var item = _context.ExactItems
                    .FirstOrDefault(y => y.Name == x.Item.Name && y.ExactUserId.ToString() == user.CreatorId);
                if (item != null)
                {
                    x.Item.Description = item.Description;
                    x.Item.Code = item.Code;
                    x.Item.ExactUserId = item.ExactUserId;
                    x.Item.Id = item.Id;
                    x.Item.ExactID = item.ExactID;
                    x.Item.SupplierId = vm.SupplierId;
                }
            }

            _context.ExactOrders.Add(order);

            await _context.SaveChangesAsync();

            return RedirectToAction("Order", "OrderManagement", new { Id = order.Id });
        }

        /// <summary>
        /// Opens the imported file
        /// </summary>
        /// <param name="fileInput"></param>
        /// <returns></returns>

        [HttpPost]
        public IActionResult OpenFile([FromForm] IFormFile fileInput)
        {
            var result = new DynamicXMLVM();

            if (fileInput != null)
            {
                // Check if XML
                if (fileInput.ContentType == "text/xml")
                {

                    using (var reader = new StreamReader(fileInput.OpenReadStream()))
                    {
                        result = StreamToDynamicXMLVM(reader.BaseStream);
                    }
                }
            }


            return RedirectToAction(nameof(Index), new { vm = result });
        }

        public List<string> GetAllXMLValuesToList(Stream xmlFile)
        {
            var result = new List<string>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(xmlFile, settings);

            reader.MoveToContent();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        result.Add(reader.Value);
                        break;
                }
            }

            return result;
        }


        public DynamicXMLVM StreamToDynamicXMLVM(Stream xmlFile)
        {
            var result = new DynamicXMLVM();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(xmlFile, settings);

            while (reader.Read())
            {
                var val = new ValueInValue();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        val.Name = reader.Name;
                        break;
                    case XmlNodeType.Text:
                        val.Value = reader.Value;
                        break;
                }

                //var currenType = reader.NodeType;
                //if (currenType == XmlNodeType.Element)
                //    val.Name = reader.Name;
                //if (currenType == XmlNodeType.Text)
                //    val.Value = reader.Value;
                //if(val.Name != null && val.Value != null)
                //    result.Values.Add(val);
            }

            return result;
        }

        public void readFile(Stream file)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(file, settings);

            reader.MoveToContent();
            // Parse the file and display each of the nodes.
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        _logger.LogDebug($"<{reader.Name}>");
                        break;
                    case XmlNodeType.Text:
                        _logger.LogDebug(reader.Value);
                        break;
                    //case XmlNodeType.CDATA:
                    //    _logger.LogDebug($"<![CDATA[{reader.Value}]]>");
                    //    break;
                    //case XmlNodeType.ProcessingInstruction:
                    //    _logger.LogDebug($"<?{reader.Name} {reader.Value}?>");
                    //    break;
                    //case XmlNodeType.Comment:
                    //    _logger.LogDebug($"<!--{reader.Value}-->");
                    //    break;
                    //case XmlNodeType.XmlDeclaration:
                    //    _logger.LogDebug("<?xml version='1.0'?>");
                    //    break;
                    //case XmlNodeType.Document:
                    //    break;
                    //case XmlNodeType.DocumentType:
                    //    _logger.LogDebug($"<!DOCTYPE {reader.Name} [{reader.Value}]");
                    //    break;
                    //case XmlNodeType.EntityReference:
                    //    _logger.LogDebug(reader.Name);
                    //    break;
                    case XmlNodeType.EndElement:
                        _logger.LogDebug($"</{reader.Name}>");
                        break;
                }
            }
        }
    }
}