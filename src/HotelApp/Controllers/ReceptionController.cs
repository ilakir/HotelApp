using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HotelApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using HotelApp.Models.SysAdminVIewModel;
using HotelApp.Controllers.TagHelpers;
using HotelApp.Models.SortViewModel;
using HotelApp.Models.FilterSortModel;
using Microsoft.AspNetCore.Authorization;
using HotelApp.Models.AdminViewModel;
using HotelApp.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelApp.Controllers
{
    public class ReceptionController : Controller
    {
        DataContext _dbContext;
        public ReceptionController(DataContext dataContext)
        {
            _dbContext = dataContext;
        }
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> MainReception(string searchedValue, string searchInColumn, SortState sortOrder = SortState.NameAsc)
        {
            IQueryable<Apartment> apartments = null;
            if (searchedValue != null && searchInColumn != null)
            {
                switch (searchInColumn)
                {
                    case ("Apartment №"):
                        apartments = _dbContext.Apartments.Where(x => x.Name.ToString() == searchedValue);
                        break;
                    case ("NumberOfRooms"):
                        apartments = _dbContext.Apartments.Where(x => x.NumberOfRooms.ToString() == searchedValue);
                        break;
                    case ("MaxTenants"):
                        apartments = _dbContext.Apartments.Where(x => x.MaxTenants.ToString() == searchedValue);
                        break;
                    case ("PricePerHour"):
                        apartments = _dbContext.Apartments.Where(x => x.PricePerHour.ToString() == searchedValue);
                        break;
                }
            }
            else{
                apartments = _dbContext.Apartments.Select(x => x);
            }

            switch (sortOrder)
            {
                case SortState.NameDesc:
                    apartments = apartments.OrderByDescending(x => x.Name);
                    break;
                case SortState.ApartmentTypeAsc:
                    apartments = apartments.OrderBy(x => x.ApartmentType);
                    break;
                case SortState.ApartmentTypeDesc:
                    apartments = apartments.OrderByDescending(x => x.ApartmentType);
                    break;
                case SortState.FreeAsc:
                    apartments = apartments.OrderBy(x => x.Free);
                    break;
                case SortState.FreeDesc:
                    apartments = apartments.OrderByDescending(x => x.Free);
                    break;
                case SortState.NumberOfRoomsAsc:
                    apartments = apartments.OrderBy(x => x.NumberOfRooms);
                    break;
                case SortState.NumberOfRoomsDesc:
                    apartments = apartments.OrderByDescending(x => x.NumberOfRooms);
                    break;
                case SortState.PricePerHourAsc:
                    apartments = apartments.OrderBy(x => x.PricePerHour);
                    break;
                case SortState.PricePerHourDesc:
                    apartments = apartments.OrderByDescending(x => x.PricePerHour);
                    break;
                case SortState.MaxTenantsAsc:
                    apartments = apartments.OrderBy(x => x.MaxTenants);
                    break;
                case SortState.MaxTenantsDesc:
                    apartments = apartments.OrderByDescending(x => x.MaxTenants);
                    break;
                default:
                    apartments = apartments.OrderBy(x => x.Name);
                    break;
            }

            AdminViewModel sysViewModel = new AdminViewModel()
            {
                //Определяем не кэшированные данные для вывода в модели представление
                Apartments = await apartments.AsNoTracking().ToListAsync(),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchedValue, searchInColumn)
            };
            return View(sysViewModel);
        }      
        [HttpGet]
        [ActionName("Eviction")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> preparationForEviction(int? apartmentId)
        {
            if (apartmentId != null)
            {
                Apartment dismissedApartmen = await _dbContext.Apartments.FirstOrDefaultAsync(x => x.Id == apartmentId);
                if (dismissedApartmen != null){
                    return View(dismissedApartmen);
                }
            }
            return NotFound();
        }
        [HttpPost]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Eviction(int? apartmentId)
        {
            if (apartmentId != null)
            {
                //Обнулям параметры (путая комната)
                Apartment dismissedApartmen = await _dbContext.Apartments.FirstOrDefaultAsync(x => x.Id == apartmentId);
                Visitor visitor = await _dbContext.Visitors.FirstOrDefaultAsync(x => x.Name == dismissedApartmen.Visitor);
                //Удаляем гостя из вписка посетителей
                _dbContext.Visitors.Remove(visitor);
                if (dismissedApartmen!=null){
                    dismissedApartmen = SetToDefaultApartment(dismissedApartmen);
                }            
                //Обновляем данные комнаты
                _dbContext.Update(dismissedApartmen);
                var result = await _dbContext.SaveChangesAsync();
                return RedirectToAction("MainReception");
            }
            return NotFound();
        }
        public Apartment SetToDefaultApartment(Apartment apartment)
        {
            if (apartment != null)
            {
                apartment.ArrivalDate = new DateTime();
                apartment.CurrentTenants = 0;
                apartment.DateOfDeparture = new DateTime();
                apartment.FinalPrice = 0;
                apartment.Free = true;
                apartment.Visitor = "";
            } 
            return apartment;
        }

    }
}

