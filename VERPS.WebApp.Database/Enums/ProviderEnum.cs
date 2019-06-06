using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VERPS.WebApp.Database.Enums
{
    public enum ProviderEnum
    {
        [DisplayName("ExactOnline")]
        ExactOnline = 0,
        MicrosoftDynamics = 1,
        AFAS = 2,
    }
}
