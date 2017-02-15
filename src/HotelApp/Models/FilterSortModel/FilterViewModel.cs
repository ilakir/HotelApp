using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelApp.Models.FilterSortModel
{
    public class FilterViewModel
    {
        public string SearchedValue { get;private set; }
        public SelectList NamesOfColumns { get; set; } 
        public string SearchInColumn { get; private set; }

        public FilterViewModel(string Value, string Column)
        {
            SearchedValue = Value;
            SearchInColumn = Column;
            NamesOfColumns = new SelectList(new string[] {"", "Apartment №", "NumberOfRooms", "MaxTenants", "PricePerHour" });
        }
    }
}
