using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApp.Models.DataModels;
using HotelApp.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Models.AdminViewModel
{
    public class SettleViewModel
    {
        public Apartment Apartment { get; set; }
        public Visitor Visitor { get; set; }
    }
}
