using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace HotelApp.Controllers.TagHelpers
{
    public enum SortState
    {
        NameAsc,
        NameDesc,
        NumberOfRoomsAsc,
        NumberOfRoomsDesc,
        ApartmentTypeAsc,
        ApartmentTypeDesc,
        PricePerHourAsc,
        PricePerHourDesc,
        FreeAsc,
        FreeDesc,
        MaxTenantsAsc,
        MaxTenantsDesc
    }
}
