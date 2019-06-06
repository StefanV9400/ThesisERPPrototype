using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ExactOnline.CustomAuth;
using ExactOnline.Models.Logistics;
using ExactOnline.Models.PurchaseOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VERPS.WebApp.Areas.ExactOnline.Models.Dashboard;
using VERPS.WebApp.Areas.ExactOnline.Models.OrderManagement;
using VERPS.WebApp.Areas.ExactOnline.Models.OrderManagement.ExactOrderXML;
using VERPS.WebApp.Database;
using VERPS.WebApp.Database.Models;
using VERPS.WebApp.Helpers;
using VERPS.WebApp.Models.General;

namespace VERPS.WebApp.Areas.ExactOnline.Controllers
{
    [Authorize]
    [Area("ExactOnline")]
    public class OrderManagementController : Controller
    {
        private readonly VERPSDBContext _context;
        private readonly DBHelper _dbHelper;
        private readonly ExactHelper _exactHelper;


        public OrderManagementController(VERPSDBContext context)
        {
            _context = context;
            _dbHelper = new DBHelper();
            _exactHelper = new ExactHelper();
        }

        /// <summary>
        /// Orders that are ready to be sent to ExactOnline
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        public IActionResult Index(StateMessage sm = StateMessage.None)
        {
            // Get and check user
            var user = _dbHelper.GetUser(_context, User.Identity.Name);
            if (user == null)
                return RedirectToAction("LogOut", "Account");


            var vm = new Areas.ExactOnline.Models.OrderManagement.IndexVM();
            var orders = _context.ExactOrders
                .Where(x => x.DBUser.UserID == user.CreatorId && !x.IsStoredInExact)
                .Select(x => new
                {
                    x.Id,
                    Lines = x.Lines.Select(y => new
                    {
                        y.Id,
                        Total = (y.NetPrice * y.Quantity),
                    }),
                    x.Created,
                    x.Supplier.Name,
                })
                .ToList()
                .Select(x => new DBOrderVM
                {
                    Id = x.Id,
                    AmountofLines = x.Lines.Count(),
                    Created = x.Created,
                    Total = x.Lines
                    .Select(y => y.Total)
                    .ToList()
                    .Sum()
                    .Value,
                    SupplierName = x.Name,
                })
                .ToList();

            vm.Orders = orders;
            vm.StateMessage = sm;
            vm.UserID = user.CreatorId;
            vm.IsOpenOrderPage = true;

            return View(vm);
        }

        /// <summary>
        /// Orders that has been stored within Exact online
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        public IActionResult OrderHistory(StateMessage sm = StateMessage.None)
        {
            // Get and check user
            var user = _dbHelper.GetUser(_context, User.Identity.Name);
            if (user == null)
                return RedirectToAction("LogOut", "Account");


            var vm = new Areas.ExactOnline.Models.OrderManagement.IndexVM();
            var orders = _context.ExactOrders
                .Where(x => x.DBUser.UserID == user.CreatorId && x.IsStoredInExact)
                .Select(x => new
                {
                    x.Id,
                    Lines = x.Lines.Select(y => new
                    {
                        y.Id,
                        y.NetPrice,
                    }),
                    x.Created,
                    x.Supplier.Name,

                })
                .ToList()
                .Select(x => new DBOrderVM
                {
                    Id = x.Id,
                    AmountofLines = x.Lines.Count(),
                    Created = x.Created,
                    Total = x.Lines
                    .Select(y => y.NetPrice)
                    .Sum()
                    .Value,
                    SupplierName = x.Name,
                })
                .ToList();

            vm.Orders = orders;
            vm.StateMessage = sm;
            vm.UserID = user.CreatorId;
            vm.IsOpenOrderPage = false;
            return View("Index", vm);
        }


        /// <summary>
        /// Order page
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Order(int Id)
        {
            // Get and check user
            var user = _dbHelper.GetUser(_context, User.Identity.Name);
            if (user == null)
                return RedirectToAction("LogOut", "Account");

            // Get and check order
            var order = _dbHelper.GetExactOrderByIdToVM(_context, Id);
            
            if (order == null || order.Lines == null)
                return RedirectToAction(nameof(Index), new { sm = StateMessage.FailedMissingInfo });

            var vm = new ExactOrderVM();

            // Check if the order have to be sent or is already sent
            if (!order.IsStoredInExact)
            {
                // Set Exact data
                ExactOnlineConnect.data.Context = _context;
                ExactOnlineConnect.data.AccessToken = user.Token;
                // TODO: Check if active??

                // Check if items are int exact and if not it neeeds to be created first
                if (!order.ItemsAreInExact)
                    return RedirectToAction(nameof(ItemsToBeCreatedInExact), new { Id = Id });

                // Get DB values
                var config = _dbHelper.GetExactConfig(_context, user.ConfigID);

                // Checks if config is correctly saved
                if (config.PaymentConditionId == null)
                    return RedirectToAction(nameof(Areas.ExactOnline.Controllers.ConfigurationController.Index), new { sm = StateMessage.FailedPaymentConditionMissing });
                if (config.UserId == null)
                    return RedirectToAction(nameof(Areas.ExactOnline.Controllers.ConfigurationController.Index), new { sm = StateMessage.FailedOrderCreatorMissing });

                var items = await ExactOnlineConnect.data.GetItems(user.DivisionId);

                if (items == null)
                {
                    return RedirectToAction(nameof(Index), new { sm = StateMessage.FailedNoExactConnection });
                }

                var toBeCreated = new List<Item>();

                vm.Id = order.Id;
                vm.Currency = order.Currency;
                vm.Description = order.Description;
                vm.Created = order.Created;
                vm.CreatorId = user.CreatorId;
                vm.PaymentConditionId = config.PaymentConditionId.ToString();
                vm.Lines = new List<OrderLineVM>();
                vm.CreatedItems = toBeCreated.Any() ? toBeCreated
                    .Select(x => x.Description)
                    .ToList()
                    :
                    new List<string>();
                vm.HasBeenSend = order.IsStoredInExact;
                vm.SupplierName = order.SupplierName;

                // Get alertData
                var messageItems = order.Lines
                    .Select(y => y.Item)
                    .Where(y => !y.MessageSeen)
                    .ToList();

                if (messageItems.Any())
                {
                    vm.CreatedItems = messageItems.Select(y => y.Name).ToList();
                    foreach (var x in messageItems)
                    {
                        var item = _context.ExactItems
                                 .FirstOrDefault(y => y.Id == x.Id);
                        item.MessageSeen = true;
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    vm.CreatedItems = new List<string>();
                }

                foreach (var x in order.Lines)
                {
                    //var item = listItems.FirstOrDefault(y => y.Description == x.Item.Name);

                    var line = new OrderLineVM
                    {
                        Id = x.Id,
                        AmountDC = x.AmountDC,
                        Description = x.Description,
                        Item = new ItemVM
                        {
                            ExactId = x.Item.ExactId,
                            Name = x.Item.Name,
                            Code = x.Item.Code,
                            Description = x.Item.Description,
                        },
                        NetPrice = x.NetPrice,
                        Project = x.Project,
                        Quantity = x.Quantity,
                        ReceiptDate = x.ReceiptDate,
                        Unit = x.Unit,
                        VATAmount = x.VATAmount,
                        VATCode = x.VATCode,
                        VATPercentage = x.VATPercentage,
                    };
                    vm.Lines.Add(line);
                    vm.StateMessage = StateMessage.SuccessAddedItems;
                };
            }
            else
            {
                vm.Id = order.Id;
                vm.CreatedItems = new List<string>();
                vm.Currency = order.Currency;
                vm.Description = order.Description;
                vm.Created = order.Created;
                vm.Project = order.Project;
                vm.Document = order.Document;
                vm.OrderNumber = order.OrderNumber;
                vm.CreatorId = user.CreatorId;
                vm.OrderDate = order.OrderDate;
                vm.PaymentConditionName = "";
                vm.Lines = new List<OrderLineVM>();
                vm.IsStoredInExact = order.IsStoredInExact;
                foreach (var x in order.Lines)
                {

                    var line = new OrderLineVM
                    {
                        Id = x.Id,
                        AmountDC = x.AmountDC,
                        Description = x.Description,
                        Item = new ItemVM
                        {
                            ExactId = x.Item.ExactId,
                            Name = x.Item.Name,
                            Code = x.Item.Code,
                            Description = x.Item.Description,
                        },
                        NetPrice = x.NetPrice,
                        Project = x.Project,
                        Quantity = x.Quantity,
                        ReceiptDate = x.ReceiptDate,
                        Unit = x.Unit,
                        VATAmount = x.VATAmount,
                        VATCode = x.VATCode,
                        VATPercentage = x.VATPercentage,
                    };
                    vm.Lines.Add(line);
                    vm.StateMessage = StateMessage.WarningNotCreatedYet;
                };
            }

            return View(vm);
        }

  
        /// <summary>
        ///  Page with items that need to be created first
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ItemsToBeCreatedInExact(int Id)
        {
            

            // Check if order exists
            var order = _context.ExactOrders
                .Include(nameof(ExactOrder.Lines) + "." + nameof(ExactOrderLine.Item))
                .Include(nameof(ExactOrder.Supplier))
                .FirstOrDefault(x => x.Id == Id);

            if (order == null || order.Lines == null)
                return RedirectToAction(nameof(Index), new { sm = StateMessage.FailedMissingInfo });

            // Check if user exists
            var user = _dbHelper.GetUser(_context, User.Identity.Name);
            if (user == null)
                return RedirectToAction("LogOut", "Account");

            var vm = new ItemsToBeCreatedVM();

            ExactOnlineConnect.data.Context = _context;
            ExactOnlineConnect.data.AccessToken = user.Token;

            var itemsInExact = await ExactOnlineConnect.data.GetItems(user.DivisionId);
            vm.SupplierName = order.Supplier.Name;
            vm.Items = (_exactHelper.GetToBeCreatedItems(itemsInExact, order.Lines.Select(x => x.Item).ToList(), user, null, order.Lines));
            var itemgroups = await ExactOnlineConnect.data.GetItemGroups(user.DivisionId);
            vm.ItemGroups = _exactHelper.ItemGroupsToSelectList(itemgroups).OrderBy(x => x.Value == user.ItemGroupId.ToString()).ToList();
            vm.OrderId = order.Id;

            if (!vm.Items.Any())
            {
                order.ItemsAreInExact = true;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Order), new { Id = vm.OrderId });
            }

            return View(vm);
        }


        /// <summary>
        /// Creates the items that are not stored yet in Exact
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateToBeCreatedItems(ItemsToBeCreatedVM vm)
        {
            // Check if order exists
            var order = _context.ExactOrders
                .Include(nameof(ExactOrder.Lines) + "." + nameof(ExactOrderLine.Item))
                .Include(nameof(ExactOrder.DBUser))
                .FirstOrDefault(x => x.Id == vm.OrderId);

            if (order == null || order.Lines == null)
                return RedirectToAction(nameof(Index), new { sm = StateMessage.FailedOrderDoesNotExist });

            // Check if user exists
            var user = _dbHelper.GetUser(_context, User.Identity.Name);
            if (user == null)
                return RedirectToAction("LogOut", "Account");

            ExactOnlineConnect.data.Context = _context;
            ExactOnlineConnect.data.AccessToken = user.Token;

            // Create missing Items within Exact Online
            foreach (var x in vm.Items)
            {
                var storedItem = await ExactOnlineConnect.data.StoreItem(user.DivisionId, x);
                var tempOrder = order.Lines
                    .FirstOrDefault(y => y.Item.Name == x.Description);
                var supplierItem = new SupplierItem
                {
                    Item = storedItem.ID,
                    PurchasePrice = tempOrder.AmountDC,
                    PurchaseUnit = "PC",
                    Supplier = new Guid(user.SupplierId),
                };

                // Set new item values
                var dbItem = _context.ExactItems
                    .FirstOrDefault(y =>
                    y.Name == x.Description
                    && y.ExactUserId.ToString() == user.CreatorId
                    && y.SupplierId.ToString() == user.SupplierId);

                dbItem.Description = storedItem.ExtraDescription;
                dbItem.ExactID = storedItem.ID;
                dbItem.Code = storedItem.Code;
                dbItem.IsComplete = true;
                dbItem.MessageSeen = false;

                var storedSupplierItem = await ExactOnlineConnect.data.StoreSupplierItem(user.DivisionId, supplierItem);
            }

            // Set SalesItemPrice
            var salesItemPrices = await ExactOnlineConnect.data.GetSalesItemPrices(user.DivisionId);

            order.ItemsAreInExact = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Order), new { Id = vm.OrderId });
        }

        /// <summary>
        ///  Sends order and saves it within the database
        /// </summary>
        /// <param name="vm">Ordervm</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SendOrder(ExactOrderVM vm)
        {
            // Check if user exists
            var user = _dbHelper.GetUser(_context, User.Identity.Name);
            if (user == null)
                return RedirectToAction("LogOut", "Account");
            // DB order
            var dbOrder = _context.ExactOrders
                .Include(nameof(ExactOrder.Lines) + "." + nameof(ExactOrderLine.Item))
                .Include(nameof(ExactOrder.Supplier))
                .Include(nameof(ExactOrder.DBUser))
                .FirstOrDefault(x => x.Id == vm.Id);

            dbOrder.Description = vm.Description;
            dbOrder.Created = DateTime.Now;
            dbOrder.Currency = vm.Currency;
            dbOrder.OrderNumber = vm.OrderNumber;
            dbOrder.YourRef = vm.YourRef;
            dbOrder.Document = vm.Document;
            dbOrder.OrderDate = vm.OrderDate;
            dbOrder.PaymentCondition = vm.PaymentConditionId;
            dbOrder.CreatorId = user.CreatorId;
            dbOrder.Project = vm.Project;
            dbOrder.TimeSend = DateTime.Now;
            dbOrder.DBUser = _context.Users.FirstOrDefault(x => x.UserName == user.UserName);
            dbOrder.IsStoredInExact = true;

            foreach (var x in vm.Lines)
            {
                var line = _context.ExactOrderLines.FirstOrDefault(y => y.Id == x.Id);
                line.Description = x.Description;
                line.AmountDC = x.AmountDC;
                line.Unit = x.Unit;
                line.NetPrice = x.NetPrice;
                line.ReceiptDate = x.ReceiptDate;
                line.VATAmount = x.VATAmount;
                line.VATCode = x.VATCode;
                line.VATPercentage = x.VATPercentage;
                line.Quantity = x.Quantity;
                line.Project = x.Project;
            }

            // Exact Order
            var exactOrder = new PurchaseOrder
            {
                Description = vm.Description,
                Created = DateTime.Now,
                Supplier = dbOrder.Supplier.ExactId,
                // Currency = vm.Currency,
                YourRef = vm.YourRef,
                Document = vm.Document,
                OrderDate = DateTime.Now,
                PaymentCondition = vm.PaymentConditionId,
                Creator = new Guid(user.CreatorId),
                PurchaseOrderLines = vm.Lines
                    .Select(x => new PurchaseOrderLine
                    {
                        Item = new Guid(x.ItemID),
                        Description = x.Description,
                        AmountDC = x.AmountDC,
                        Unit = x.Unit,
                        NetPrice = x.NetPrice,
                        ReceiptDate = DateTime.Now,
                        // VATAmount = x.VATAmount,
                        // VATCode = x.VATCode,
                        // VATPercentage = x.VATPercentage,
                        Quantity = x.Quantity,
                        QuantityInPurchaseUnits = x.Quantity,
                        Project = x.Project,
                        UnitPrice = 2,

                    })
                    .ToList()
            };

            ExactOnlineConnect.data.Context = _context;
            ExactOnlineConnect.data.AccessToken = user.Token;

            var message = await ExactOnlineConnect.data.StoreOrder(user.DivisionId, exactOrder);
            if (message.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                return RedirectToAction(nameof(Index), new { sm = StateMessage.FailedSendingOrder });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { sm = StateMessage.SuccessSendingOrder });
        }


        // TODO: Can only delete own order
        [HttpPost]
        public RedirectToActionResult Delete(int Id)
        {
            var order = _context.ExactOrders
                 .Include(nameof(ExactOrder.Lines) + "." + nameof(ExactOrderLine.Item))
                 .FirstOrDefault(x => x.Id == Id);

            if (order == null)
                return RedirectToAction(nameof(Index), new { sm = StateMessage.FailedOrderDoesNotExist });

            _context.ExactOrders
                .Remove(order);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new { sm = StateMessage.SuccessRemovedOrder });
        }
    }
}
