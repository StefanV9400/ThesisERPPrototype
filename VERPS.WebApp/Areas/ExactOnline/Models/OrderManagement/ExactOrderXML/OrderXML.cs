using System;
using System.Xml.Serialization;
namespace VERPS.WebApp.Areas.ExactOnline.Models.OrderManagement.ExactOrderXML
{
        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [XmlRoot("Order")]
        public class OrderXML
        {

            private string descriptionField;

            private string createdField;

            private string currencyField;

            private object projectField;

            private OrderLine[] orderLinesField;

            /// <remarks/>
            public string Description
            {
                get
                {
                    return this.descriptionField;
                }
                set
                {
                    this.descriptionField = value;
                }
            }

            /// <remarks/>
            public string Created
            {
                get
                {
                    return this.createdField;
                }
                set
                {
                    this.createdField = value;
                }
            }

            /// <remarks/>
            public string Currency
            {
                get
                {
                    return this.currencyField;
                }
                set
                {
                    this.currencyField = value;
                }
            }

            /// <remarks/>
            public object Project
            {
                get
                {
                    return this.projectField;
                }
                set
                {
                    this.projectField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("Line", IsNullable = false)]
            public OrderLine[] OrderLines
            {
                get
                {
                    return this.orderLinesField;
                }
                set
                {
                    this.orderLinesField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class OrderLine
        {

            private string itemField;

            private string descriptionField;

            private int quantityField;

            private string unitField;

            private double netPriceField;

            private string receiptDateField;

            private string vATCodeField;

            private double vATPercentageField;

            private double amountDCField;

            private double vATAmountField;

            private object projectField;

            /// <remarks/>
            public string Item
            {
                get
                {
                    return this.itemField;
                }
                set
                {
                    this.itemField = value;
                }
            }

            /// <remarks/>
            public string Description
            {
                get
                {
                    return this.descriptionField;
                }
                set
                {
                    this.descriptionField = value;
                }
            }

            /// <remarks/>
            public int Quantity
            {
                get
                {
                    return this.quantityField;
                }
                set
                {
                    this.quantityField = value;
                }
            }

            /// <remarks/>
            public string Unit
            {
                get
                {
                    return this.unitField;
                }
                set
                {
                    this.unitField = value;
                }
            }

            /// <remarks/>
            public double NetPrice
            {
                get
                {
                    return this.netPriceField;
                }
                set
                {
                    this.netPriceField = value;
                }
            }

            /// <remarks/>
            public string ReceiptDate
            {
                get
                {
                    return this.receiptDateField;
                }
                set
                {
                    this.receiptDateField = value;
                }
            }

            /// <remarks/>
            public string VATCode
            {
                get
                {
                    return this.vATCodeField;
                }
                set
                {
                    this.vATCodeField = value;
                }
            }

            /// <remarks/>
            public double VATPercentage
            {
                get
                {
                    return this.vATPercentageField;
                }
                set
                {
                    this.vATPercentageField = value;
                }
            }

            /// <remarks/>
            public double AmountDC
            {
                get
                {
                    return this.amountDCField;
                }
                set
                {
                    this.amountDCField = value;
                }
            }

            /// <remarks/>
            public double VATAmount
            {
                get
                {
                    return this.vATAmountField;
                }
                set
                {
                    this.vATAmountField = value;
                }
            }

            /// <remarks/>
            public object Project
            {
                get
                {
                    return this.projectField;
                }
                set
                {
                    this.projectField = value;
                }
            }
        }
}
