using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SOCE.Library.UI
{
    public enum ExpenseEnum
    {
        [Description("Miscellaneous")]
        Miscellaneous = 0,

        [Description("Mileage")]
        Mileage = 1,

        [Description("Food")]
        Food = 2,

        [Description("Car")]
        Car = 3,

        [Description("Gas")]
        Gas = 4,

        [Description("Parking")]
        Parking = 5,

        [Description("Hotel")]
        Hotel = 6,

        [Description("Flight")]
        Flight = 7,

        [Description("Printing")]
        Printing = 8,



    }
}
