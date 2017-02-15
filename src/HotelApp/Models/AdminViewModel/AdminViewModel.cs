using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApp.Models.FilterSortModel;
using HotelApp.Models.DataModels;

namespace HotelApp.Models.AdminViewModel
{
    public class AdminViewModel
    {
        public IEnumerable<Apartment> Apartments { get; set; }
        public SortViewModel.SortViewModel SortViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
    }
}
