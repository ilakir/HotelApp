using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using HotelApp.Models.DataModels;
using HotelApp.Models.SortViewModel;
using Microsoft.AspNetCore.Mvc;
using HotelApp.Models.FilterSortModel;

namespace HotelApp.Models.SysAdminVIewModel
{
    public class SysAdminViewModel
    {
        public IEnumerable<Apartment> Apartments { get; set; }
        public SortViewModel.SortViewModel SortViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
    }
}
