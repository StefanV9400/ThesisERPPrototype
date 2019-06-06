using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VERPS.WebApp.Areas.ExactOnline.Models.General
{
    public class ExactTokenVM
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
