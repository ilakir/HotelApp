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


namespace HotelApp.Controllers
{
    //Контроллер для работы системного оадминистратора
    public class SysAdminController : Controller
    {
        //Определяем контекст данных
        private readonly DataContext _dbContext;
        //Инициализируем его через механизм внедрения зависимостей
        public SysAdminController(DataContext context){
            _dbContext = context;
        }
        [Authorize(Roles = "sysadmin")]
        public async Task<IActionResult> Index(string searchedValue, string searchInColumn, SortState sortOrder = SortState.NameAsc)
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

            SysAdminViewModel sysViewModel = new SysAdminViewModel()
            {
                //Определяем не кэшированные данные для вывода в модели представление
                Apartments = await apartments.AsNoTracking().ToListAsync(),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchedValue,searchInColumn)
                
            };
            return View(sysViewModel);         
        }
        [Authorize(Roles = "sysadmin")]
        [HttpGet]
        public IActionResult Create()=>View();
        //Медод обтаботки post запросов про сощдании новых аппартаменов
        [Authorize(Roles = "sysadmin")]
        [HttpPost]
        public async Task<IActionResult> Create(Apartment apartmentModel)
        { 
            Apartment newApartment = new Apartment { Name = apartmentModel.Name
                ,NumberOfRooms = apartmentModel.NumberOfRooms, MaxTenants = apartmentModel.MaxTenants
                ,ApartmentType = apartmentModel.ApartmentType, PricePerHour = apartmentModel.PricePerHour
            };
            //Добаляем новые аппартамены в БД ( insert SQL команда) 
            _dbContext.Apartments.Add(newApartment);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "SysAdmin");
        }
        [HttpGet]
        [Authorize(Roles = "sysadmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                //Ищем в базе данных нужный нам элемент для редактирования, если он есть, то передаем его представлению
                Apartment editableApartment = await _dbContext.Apartments.FirstOrDefaultAsync(x => x.Id == id);
                if (editableApartment != null)
                    return View(editableApartment);
            }
            return NotFound();
        }
        [HttpPost]
        [Authorize(Roles = "sysadmin")]
        public async Task<IActionResult> Edit(Apartment apartment)
        {
            //Отредактированная модель передается в контекст данных для обновления (update SQL команда)
            _dbContext.Update(apartment);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index","SysAdmin");
        }
        [HttpGet]
        [ActionName("Delete")]
        [Authorize(Roles = "sysadmin")]
        public async Task<IActionResult> PreparingForDeleting(int? id)
        {
            if (id != null)
            {
                //Определяем если ли нужный нам элемент в базе данных
                Apartment deleteAppartment = await _dbContext.Apartments.FirstOrDefaultAsync(x => x.Id == id);
                //Если есть то не удаляем его сразу а передаем представление чтобы удалить данные POST запростом (для безопасности данных)
                if (deleteAppartment != null)
                    return View(deleteAppartment);
            }
            return NotFound();    
        }

        [HttpPost]
        [Authorize(Roles = "sysadmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            //Удаляем POST запростом (id не нужно определять в http запросе)
            if (id != null)
            {
                //Получаем нужный элемент
                Apartment deleteAppartment = await _dbContext.Apartments.FirstOrDefaultAsync(x => x.Id == id);
                if (deleteAppartment != null)
                {
                    //Удаляем из БД модель аппартаментов(delete SQL команда)
                    _dbContext.Remove(deleteAppartment);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckAppartment(Apartment appartmentNumber)
        {
            //Проверка на наличие такого же номера в БД
            Apartment result = _dbContext.Apartments.FirstOrDefault(x => x.Name == appartmentNumber.Name);
            if (result != null)
                return Json(false);
            return Json(true);
        }
    }
}
