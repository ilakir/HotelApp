using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HotelApp.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using HotelApp.Controllers.TagHelpers;
using HotelApp.Models.AdminViewModel;
using HotelApp.Models.SortViewModel;
using HotelApp.Models.FilterSortModel;
using HotelApp.Models;
using Microsoft.AspNetCore.Authorization;
public class AdminController : Controller
{
    private readonly DataContext _dbContext;
    public AdminController(DataContext context) {
        _dbContext = context;
    }
    [Authorize(Roles = "user")]
    public async Task<IActionResult> Main(string searchedValue, string searchInColumn, SortState sortOrder = SortState.NameAsc)
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
    [Authorize(Roles = "user")]
    public async Task<IActionResult> Settle(int apartmentId, string name)
    {
        //IQueryable<Visitors> allVisitors = _dbContext.Visitors.Select(x => x);
        Apartment apartment = await _dbContext.Apartments.FirstOrDefaultAsync(x => x.Id == apartmentId);
        SettleViewModel settlerModel = new SettleViewModel
        {
            Apartment = apartment,
            Visitor = new Visitor { Name = name }
        };
        return View(settlerModel);
    }
    [HttpPost]
    [Authorize(Roles = "user")]
    public async Task<IActionResult> Settle(SettleViewModel settleModel, int id, string name)
    {
        
        Apartment apartment = _dbContext.Apartments.FirstOrDefault(x => x.Id == id);
        Visitor visitor = new Visitor { ArrivalDate = settleModel.Apartment.ArrivalDate, Name = name };
        //Заполняем дату заселения и убытия
        apartment.ArrivalDate = settleModel.Apartment.ArrivalDate;
        apartment.DateOfDeparture = settleModel.Apartment.DateOfDeparture;
        apartment.FinalPrice = ((apartment.DateOfDeparture.Day * 24 + apartment.DateOfDeparture.Hour) - (apartment.ArrivalDate.Day * 24 + apartment.ArrivalDate.Hour)) * apartment.PricePerHour
            + (apartment.PricePerHour * Int32.Parse(apartment.ApartmentType));
        apartment.Visitor = name;
        apartment.Free = false;
        //Добавляем в БД нового посетителя
         _dbContext.Visitors.Add(visitor);
        //Обновляем информаю об аппартаментах
        _dbContext.Update(apartment);
        //Срхраняем изменения
        await _dbContext.SaveChangesAsync();
        return RedirectToAction("Main");
    }
    [AcceptVerbs("Get", "Post")]
    public IActionResult CheckNumberOfRooms(SettleViewModel appartmentNumber)
    {
        //Контролируем, чтобы постояльцев было не больше, чем вместимость аппартамента     
        return Json(appartmentNumber.Apartment.MaxTenants >= appartmentNumber.Apartment.CurrentTenants);
    }
}
     