using ExactOnline.Models.Cashflow;
using ExactOnline.Models.CRM;
using ExactOnline.Models.Current;
using ExactOnline.Models.Logistics;
using ExactOnline.Models.Purchase;
using ExactOnline.Models.PurchaseEntry;
using ExactOnline.Models.PurchaseOrder;
using ExactOnline.Models.SystemBase;
using ExactOnline.Models.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VERPS.WebApp.Database;

namespace ExactOnline.CustomAuth
{
    public partial class ExactOnlineConnect
    {
        public static Data data = Data.data;
        public class Data
        {
            private static Data instance;
            public static Data data => instance ?? (instance = new Data());


            private readonly ExactService service = new ExactService();

           public string UserId
            {
                set
                {
                    service.UserID = value;
                }
            }

            public string AccessToken
            {
                set
                {
                    service.AccessToken = value;
                }
            }

            public string RefreshToken
            {
                set
                {
                    service.RefreshToken = value;
                }
            }

            public VERPSDBContext Context
            {
                set
                {
                    service.Context = value;
                }
            }

            private Data()
            {
                Init();
            }

            public bool hasToken;
            public int CurrentDivision;
            private Me _me;
            private List<PurchaseInvoice> _purchaseInvoices;
            private IEnumerable<PurchaseEntry> _purchaseEntries;
            private IEnumerable<Account> _suppliers;
            private IEnumerable<Division> _divisions;



            public async Task<bool> IsActiveCheck()
            {
                var check = await service.Get<dynamic>("current/Me?$select=*");

                if (check == null)
                {
                    return false;
                }

                return true;
            }


            public async Task<IEnumerable<Account>> GetSuppliers(int division)
            {

                if (_suppliers == null)
                {
                    return await service.Get<IEnumerable<Account>>(String.Format($"{division}/crm/Accounts?$select=ID,IsSupplier,Name"), isSingle: false);
                }
                return _suppliers;
            }

            public async Task<Account> GetSupplier(int division, string guid)
            {
                return await service.Get<Account>(String.Format($"{division}/crm/Accounts?$filter=ID eq guid'{guid}'&$select=ID,City,Currency,Email,Name,Phone,Postcode,VATNumber,Website"));
            }


            public async Task<IEnumerable<User>> getUsers(int division)
            {
                return await service.Get<IEnumerable<User>>(String.Format($"{division}/users/Users?$select=UserID,Creator,FirstName,LastName"), isSingle: false);
            }

            public async Task<IEnumerable<PurchaseOrder>> GetOrders(int division)
            {
                return await service.Get<IEnumerable<PurchaseOrder>>(String.Format($"{division}/purchaseorder/PurchaseOrders?$select=*"), isSingle: false);
            }

            public async Task<Me> GetMe()
            {
                if (_me?.UserName != null) return _me;
                var _tme = await service.Get<Me>("current/Me?$select=*");
                if (_tme == default(Me))
                    return (Me)null;
                return await service.Get<Me>("current/Me?$select=*");
            }

            public async Task<IEnumerable<PurchaseEntry>> GetPurchaseEntries(int division)
            {
                _purchaseEntries = await service.Get<IEnumerable<PurchaseEntry>>(division + "/purchaseentry/PurchaseEntries?$select=*");
                return _purchaseEntries;
            }

            public async Task<PurchaseEntry> GetPurchaseEntry(int division)
            {
                return await service.Get<PurchaseEntry>($"{division}/purchaseentry/PurchaseEntries?$select=*");
            }

            public async Task<IEnumerable<PaymentCondition>> GetPaymentConditions(int division)
            {
                return await service.Get<IEnumerable<PaymentCondition>>(String.Format($"{division}/cashflow/PaymentConditions?$select=*"), isSingle: false);
            }

            public async Task<IEnumerable<Division>> GetDivisions(int division)
            {

                if (division != 0)
                {
                    return await service.Get<IEnumerable<Division>>(String.Format($"{division}/system/Divisions?select=*"), isSingle: false);
                }
                else
                {
                    return await service.Get<IEnumerable<Division>>(String.Format($"{CurrentDivision}/system/Divisions?select=*"), isSingle: false);

                }
            }

            public async Task<IEnumerable<Item>> GetItems(int division)
            {
                return await service.Get<IEnumerable<Item>>(String.Format($"{division}/logistics/Items?select=*"), isSingle: false);
            }

            public async Task<IEnumerable<ItemGroup>> GetItemGroups(int division)
            {
                return await service.Get<IEnumerable<ItemGroup>>(String.Format($"{division}/logistics/ItemGroups?select=*"), isSingle: false);
            }

            public async Task<IEnumerable<SalesItemPrice>> GetSalesItemPrices(int division)
            {
                return await service.Get<IEnumerable<SalesItemPrice>>(String.Format($"{division}/logistics/SalesItemPrices?$select=*"), isSingle: false);
            }

            public async Task<IEnumerable<SupplierItem>> GetSupplierItems(int divison)
            {
                return await service.Get<IEnumerable<SupplierItem>>($"{divison}/logistics/SupplierItem", isSingle: false);
            }

            public async Task<IEnumerable<AvailableFeature>> GetAvailableFeatures(int division)
            {
                return await service.Get<IEnumerable<AvailableFeature>>(String.Format($"{division}/system/AvailableFeatures?select=*"), isSingle: false);
            }



            /// <summary>
            /// div = 2467200
            /// </summary>
            public void Init()
            {
                _me = new Me();
            }

            public void setDivision(int div)
            {
                this.CurrentDivision = div;
            }


            public async Task<HttpResponseMessage> StoreOrder(int division, PurchaseOrder order)
            {
                return await service.Post(String.Format($"{division}/purchaseorder/PurchaseOrders"), data: order);
            }

            public async Task<Item> StoreItem(int division, Item item)
            {
                return await service.PostNew<Item>(String.Format($"{division}/logistics/Items"), data: item);
            }

            public async Task<SupplierItem> StoreSupplierItem(int division, SupplierItem supplierItem)
            {
                return await service.PostNew<SupplierItem>(String.Format($"{division}/logistics/SupplierItem"), data: supplierItem);
            }
        }

    }
}
