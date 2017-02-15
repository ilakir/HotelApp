using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HotelApp.Models.DataModels;

namespace HotelApp.Models.DataModels
{
    //Класс контекста данных
    public class DataContext: IdentityDbContext<HotelUser>
    {
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DataContext(DbContextOptions<DataContext> contextOption):base(contextOption)
        {
        }
    }
}
