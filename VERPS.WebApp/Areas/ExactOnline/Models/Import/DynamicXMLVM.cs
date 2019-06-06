using System.Collections.Generic;

namespace VERPS.WebApp.Areas.ExactOnline.Models.Import
{
    public class DynamicXMLVM
    {
        public List<ValueInValue> Values { get; set; }
    }

    public class ValueInValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
