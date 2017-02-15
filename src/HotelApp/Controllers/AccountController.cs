using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using HotelApp.Models;
using HotelApp.Models.AuthenticationViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HotelApp.Models.DataModels;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelApp.Controllers
{
    //Контроллер для управления регистрацией пользователей
    public class AccountController : Controller
    {
        //Определяем Identity менеджеры для работы с пользователями, ролями, и аутентификацией
        private readonly UserManager<HotelUser> _userManager;
        private readonly SignInManager<HotelUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dbContext;
        public AccountController(UserManager<HotelUser> userManager, SignInManager<HotelUser> signInManager, RoleManager<IdentityRole> roleManager, DataContext dbContext)
        {
            //Иницифлизарум через механизм внедрения зависимостей
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Registration()=> View();
        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel registerModel)
        {
            //Если нет ошибок при регистрации нового пользователя
            if (ModelState.IsValid)
            {
                HotelUser newHotelUser = new HotelUser()
                {
                    UserName = registerModel.Name,
                    SurName = registerModel.SurName,
                    Name  = registerModel.Name
                };
                //Добавляем нового псетителя отеля в бд
                var result = await _userManager.CreateAsync(newHotelUser,registerModel.Password);
                
                //Если пользователь успешно добален
                if (result.Succeeded)
                {
                    var result2 =  await _userManager.AddToRoleAsync(newHotelUser, "user");
                    //устанавливаем аутентифицированные куки к пользователю и перенаправляем на главную страницу
                    await _signInManager.SignInAsync(newHotelUser, false);
                    return RedirectToAction("Main", "Admin");
                }
                else
                {
                    for (int i = 0; i < result.Errors.Count(); i++)
                    {
                        //Если есть ошибки то передаем их в модель состояния
                        ModelState.AddModelError(string.Empty, result.Errors.ElementAt(i).Description);
                    }
                }
            }
            return View(registerModel);
        }
        [HttpGet]
        public IActionResult Login()=> View();
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                //Аутентифицируем пользоватеря по логину и паролю
                var result = await _signInManager.PasswordSignInAsync(loginModel.UserName, loginModel.Password, loginModel.Remember, false);
                if (result.Succeeded)
                {
                    //Определяем роль и перенапраляем на соответстсвующую страницу
                    HotelUser user = _dbContext.Users.FirstOrDefault(x => x.UserName == loginModel.UserName);
                    if (await _userManager.IsInRoleAsync(user, "sysadmin")) {
                        return RedirectToAction("Index", "SysAdmin");
                    }
                    else if(await _userManager.IsInRoleAsync(user, "user")){
                        return RedirectToAction("Main", "Admin");
                    }
                    else{
                        return RedirectToAction("MainReception", "Reception");
                    }
                }
                else{
                    return RedirectToAction("Index", "Home");
                }
            }
            else {
                ModelState.AddModelError("", "Incorrect username or password");
            }
            return View(loginModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            //Удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
