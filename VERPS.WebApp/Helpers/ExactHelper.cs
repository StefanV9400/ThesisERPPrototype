using ExactOnline.Models.Cashflow;
using ExactOnline.Models.CRM;
using ExactOnline.Models.Logistics;
using ExactOnline.Models.SystemBase;
using ExactOnline.Models.Users;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VERPS.WebApp.Areas.ExactOnline.Models.OrderManagement.ExactOrderXML;
using VERPS.WebApp.Database.Models;
using VERPS.WebApp.Models;

namespace VERPS.WebApp.Helpers
{
    public class ExactHelper
    {
        public List<SelectListItem> SuppliersToSelectList(IEnumerable<Account> accounts)
        {
            var result = new List<SelectListItem>();

            foreach (var x in accounts)
            {
                var item = new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.ID.ToString(),
                };
                result.Add(item);
            }
            return result;
        }

        public List<SelectListItem> UsersToSelectList(IEnumerable<ExactOnline.Models.Users.User> users)
        {
            var result = new List<SelectListItem>();

            foreach (var x in users)
            {
                var item = new SelectListItem()
                {
                    Text = (x.FirstName + " " + x.LastName),
                    Value = x.UserID.ToString(),
                };
                result.Add(item);
            }
            return result;
        }

        public List<SelectListItem> PaymentConditionsToSelectList(IEnumerable<PaymentCondition> paymentConditions)
        {
            var result = new List<SelectListItem>();

            foreach (var x in paymentConditions)
            {
                var item = new SelectListItem()
                {
                    Text = x.Description,
                    Value = x.Code.ToString(),
                };
                result.Add(item);
            }
            return result;
        }

        public List<SelectListItem> DivisionsToSelectList(IEnumerable<Division> divisions)
        {
            var result = new List<SelectListItem>();

            foreach (var x in divisions)
            {
                var item = new SelectListItem()
                {
                    Text = (x.Hid + " - " + x.Description),
                    Value = x.Code.ToString(),
                };
                result.Add(item);
            }
            return result;
        }

        public List<SelectListItem> ItemGroupsToSelectList(IEnumerable<ItemGroup> itemGroups)
        {
            if (itemGroups != null)
            {
                var result = new List<SelectListItem>();

                foreach (var x in itemGroups)
                {
                    var item = new SelectListItem()
                    {
                        Text = x.Description,
                        Value = x.ID.ToString(),
                    };
                    result.Add(item);
                }
                return result;
            }
            else
            {
                return new List<SelectListItem>();
            }
        }

        public ExactOrder XMLToDBOrder(OrderXML orderXml, UserVM user, Database.Models.User dbUser, Guid SupplierId)
        {
            var order = new ExactOrder()
            {
                Description = orderXml.Description,
                Created = DateTime.Now,
                Currency = orderXml.Currency,
                TimeSend = DateTime.Now,
                OrderDate = DateTime.Now,
                DBUser = dbUser,
                IsStoredInExact = false,
                ItemsAreInExact = false,
                CreatorId = user.CreatorId,
                Supplier = new ExactSupplier
                {
                    ExactId = SupplierId,
                },
            };

            var orderLines = new List<ExactOrderLine>();

            foreach (var x in orderXml.OrderLines)
            {
                var line = new ExactOrderLine
                {
                    Item = new ExactItem
                    {
                        Name = x.Item,
                        ExactUserId = new Guid(user.CreatorId),
                        SupplierId = new Guid(user.SupplierId),
                    },
                    Description = x.Description,
                    AmountDC = x.AmountDC,
                    Unit = x.Unit,
                    NetPrice = x.NetPrice,
                    ReceiptDate = DateTime.Now,
                    VATAmount = x.VATAmount,
                    VATCode = x.VATCode,
                    VATPercentage = x.VATPercentage,
                    Quantity = x.Quantity,
                };
                orderLines.Add(line);
            }

            order.Lines = orderLines;

            return order;
        }


        /// <summary>
        /// Get the to be created items that are missing in Exact Online. 
        /// Important to add these first before creating orders as they need to be existing within Exact Online.
        /// </summary>
        /// <param name="ItemsInExact"></param>
        /// <param name="SelectedItems"></param>
        /// <param name="orderXml"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<Item> GetToBeCreatedItems(IEnumerable<Item> ItemsInExact, List<ExactItem> SelectedItems, UserVM user, OrderXML orderXml = null, List<ExactOrderLine> OrderLines = null)
        {
            var toBeCreated = new List<string>();
            var containingItems = ItemsInExact
                .Select(x => x.Description)
                .ToList();

            foreach (var x in SelectedItems)
            {
                if (!containingItems.Contains(x.Name))
                {
                    toBeCreated.Add(x.Name);
                }
            }

            var toBeCreatedItems = toBeCreated.Distinct().ToList();


            // Set to be created into a list
            var result = new List<Item>();
            foreach (var x in toBeCreatedItems)
            {
                if (orderXml != null)
                {
                    var orderLineItem = orderXml.OrderLines
                   .FirstOrDefault(y => y.Item == x);

                    var toBeCreatedItem = new Item
                    {
                        Description = orderLineItem.Item,
                        ExtraDescription = orderLineItem.Description,
                        Creator = new Guid(user.CreatorId),
                        ItemGroup = new Guid(user.ItemGroupId),
                        Code = "XXX_" + orderLineItem.Item,
                    };

                    result.Add(toBeCreatedItem);
                }
                else
                {
                    var orderLineItem = OrderLines
                  .FirstOrDefault(y => y.Item.Name == x);
                    var itemname = orderLineItem.Item.Name;
                    var toBeCreatedItem = new Item
                    {
                        Description = itemname,
                        ExtraDescription = orderLineItem.Description,
                        Creator = new Guid(user.CreatorId),
                        ItemGroup = new Guid(user.ItemGroupId),
                        Code = "XXX_" + itemname,
                    };

                    result.Add(toBeCreatedItem);
                }

            }
            return result;
        }

    }
}
