using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApp.Controllers.TagHelpers;

namespace HotelApp.Models.SortViewModel
{
    public class SortViewModel
    {
        public SortState ApartmentNumberSort { get; set; } = SortState.NameAsc;
        public SortState NumberOfRoomsSort { get; set; } = SortState.NumberOfRoomsAsc;
        public SortState ApartmentTypeSort { get; set; } = SortState.ApartmentTypeAsc;
        public SortState FreeSort { get; set; } = SortState.FreeAsc;
        public SortState PricePerHourSort { get; set; } = SortState.PricePerHourAsc;
        public SortState MaxTetansSort { get; set; } = SortState.MaxTenantsAsc;
        public SortState Current { get; set; }

        public SortViewModel(SortState sortOrder)
        {
            //Меняем состояние сортировки на противоположное
            ApartmentNumberSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            NumberOfRoomsSort = sortOrder == SortState.NumberOfRoomsAsc ? SortState.NumberOfRoomsDesc : SortState.NumberOfRoomsAsc;
            ApartmentTypeSort = sortOrder == SortState.ApartmentTypeAsc ? SortState.ApartmentTypeDesc : SortState.ApartmentTypeAsc;
            FreeSort = sortOrder == SortState.FreeAsc ? SortState.FreeDesc : SortState.FreeAsc;
            PricePerHourSort = sortOrder == SortState.PricePerHourAsc ? SortState.PricePerHourDesc : SortState.PricePerHourAsc;
            MaxTetansSort = sortOrder == SortState.MaxTenantsAsc ? SortState.MaxTenantsDesc : SortState.MaxTenantsAsc;
            Current = sortOrder;
        }
    }
}
