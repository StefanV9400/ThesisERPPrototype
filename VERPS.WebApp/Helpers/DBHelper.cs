using ExactOnline.Models.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VERPS.WebApp.Areas.ExactOnline.Models.General;
using VERPS.WebApp.Areas.ExactOnline.Models.OrderManagement;
using VERPS.WebApp.Database;
using VERPS.WebApp.Database.Models;
using VERPS.WebApp.Models;
using VERPS.WebApp.Models.Configuration;

namespace VERPS.WebApp.Helpers
{
    public class DBHelper
    {
        public UserVM GetUser(VERPSDBContext context, string userName)
        {
            return context.Users
                .Where(x => x.UserName == userName)
                .Select(x => new UserVM
                {
                    Id = x.Id,
                    CompanyName = x.CompanyName,
                    ConfigID = x.ExactConfiguration.Id,
                    DivisionId = x.ExactConfiguration.DivsionId,
                    CreatorId = x.UserID,
                    UserName = x.UserName,
                    ItemGroupId = x.ExactConfiguration.ItemGroupId.ToString(),
                    SupplierId = x.ExactConfiguration.SupplierId.ToString(),
                    Token = x.ExactToken.Token,
                })
                .FirstOrDefault();
        }

        public ExactConfigurationVM GetExactConfig(VERPSDBContext context, int id)
        {
            return context.ExactConfigurations
                .Where(x => x.Id == id)
                .Select(x => new ExactConfigurationVM
                {
                    Id = x.Id,
                    ItemGroupId = x.ItemGroupId,
                    SupplierId = new Guid(x.SupplierId),
                    PaymentConditionId = x.PaymentConditionId,
                    UserId = x.BuyerId,
                    ConfigType = x.ConfigType,
                })
                .FirstOrDefault();
        }


        public ExactSupplier CreateOrGetSupplier(VERPSDBContext context, Account supplier, string exactUserId, ILogger logger)
        {
            var dbSupplier = context.ExactSuppliers
                .FirstOrDefault(x => x.ExactUser.ToString() == exactUserId && x.ExactId == supplier.ID);

            // Create new
            if (dbSupplier == null)
            {
                logger.LogInformation($"New supplier with GUID: {supplier.ID} created for User: {exactUserId}");
                return new ExactSupplier
                {
                    Name = supplier.Name,
                    Currency = supplier.Currency,
                    Email = supplier.Email,
                    Phone = supplier.Phone,
                    VATNumber = supplier.VATNumber,
                    ExactId = supplier.ID,
                    Website = supplier.Website,
                    Zipcode = supplier.Postcode,
                    ExactUser = new Guid(exactUserId),
                };
            }
            else
            {
                return dbSupplier;
            }

        }

        public List<ExactSupplierConfig> GetExactSupplierConfigs(VERPSDBContext context, string userId)
        {
            return context.ExactSupplierConfigurations
                .Where(x => x.User.Id == userId)
                .ToList();
        }

        public ExactOrderVM GetExactOrderByIdToVM(VERPSDBContext context, int Id)
        {
            var order = context.ExactOrders
                //.Include(nameof(ExactOrder.Lines) + "." + nameof(ExactOrderLine.Item))
                //.Include(nameof(ExactOrder.DBUser))
                .Where(x => x.Id == Id)
                .Select(x => new
                {
                    x.Id,
                    x.Description,
                    x.Created,
                    x.Currency,
                    x.Project,
                    x.OrderNumber,
                    x.YourRef,
                    x.Document,
                    x.OrderDate,
                    x.PaymentCondition,
                    UserGuid = x.User,
                    supplierName = x.Supplier.Name,
                    x.CreatorId,
                    Lines = x.Lines
                                .Select(y => new
                                {
                                    Id = y.Id,
                                    Item = new
                                    {
                                        y.Item.Id,
                                        y.Item.ExactID,
                                        y.Item.Name,
                                        y.Item.Description,
                                        y.Item.Code,
                                        y.Item.IsComplete,
                                        y.Item.MessageSeen,
                                    },
                                    Description = y.Description,
                                    Quantity = y.Quantity,
                                    Unit = y.Unit,
                                    NetPrice = y.NetPrice,
                                    ReceiptDate = y.ReceiptDate,
                                    VATCode = y.VATCode,
                                    VATPercentage = y.VATPercentage,
                                    AmountDC = y.AmountDC,
                                    VATAmount = y.VATAmount,
                                    Project = y.Project,
                                })
                                .ToList(),
                    x.IsStoredInExact,
                    x.ItemsAreInExact,
                    User = new
                    {
                        Id = x.DBUser.Id,
                        Email = x.DBUser.Email
                    },
                    x.TimeSend,
                })
                .ToList();


            var result = order.Select(x => new ExactOrderVM
            {
                Id = x.Id,
                Description = x.Description,
                Created = x.Created,
                Currency = x.Currency,
                Project = x.Project,
                OrderNumber = x.OrderNumber,
                YourRef = x.YourRef,
                Document = x.Document,
                OrderDate = x.OrderDate,
                PaymentConditionId = x.PaymentCondition,
                CreatorId = x.User.Id,
                SupplierName = x.supplierName,
                Lines = x.Lines
                        .Select(y => new OrderLineVM
                        {
                            Id = y.Id,
                            Item = new ItemVM
                            {
                                Id = y.Item.Id,
                                ExactId = y.Item.ExactID,
                                Code = y.Item.Code,
                                Description = y.Item.Description,
                                Name = y.Item.Name,
                                MessageSeen = y.Item.MessageSeen,
                                IsComplete = y.Item.IsComplete,
                            },
                            Description = y.Description,
                            Quantity = y.Quantity,
                            Unit = y.Unit,
                            NetPrice = y.NetPrice,
                            ReceiptDate = y.ReceiptDate,
                            VATCode = y.VATCode,
                            VATPercentage = y.VATPercentage,
                            AmountDC = y.AmountDC,
                            VATAmount = y.VATAmount,
                            Project = y.Project,
                        })
                        .ToList(),
                IsStoredInExact = x.IsStoredInExact,
                ItemsAreInExact = x.ItemsAreInExact,
                DBUser = new DBUserVM
                {
                    Id = x.User.Id,
                    Email = x.User.Email,
                },
                TimeSend = x.TimeSend,
            })
                .FirstOrDefault();

            return result;
        }
    }
}
