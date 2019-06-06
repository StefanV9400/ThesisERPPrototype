using System;
using System.ComponentModel;

namespace VERPS.WebApp.Database.Enums
{
    public enum ConfigType
    {
        [DisplayName("Handmatig opslaan")]
        Manual = 0,
        [DisplayName("Alleen producten handmatig opslaan, de rest automatisch")]
        SemiAuto = 1,
        [DisplayName("Alles automatisch oplaan")]
        FullAuto = 2,
    }
}
