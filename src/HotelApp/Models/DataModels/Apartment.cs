using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Models.DataModels
{
    //Модель аппартаментов отеля
    public class Apartment
    {
        [Required]
        [Display(Name = "Apartment №")]
        [Remote("CheckAppartment","SysAdmin",ErrorMessage = "Number is already exist")]
        [DataType(DataType.Text)]
        public int Name { get; set; }
        [Display(Name = "Number of rooms")]
        [DataType(DataType.Text)]
        public int NumberOfRooms { get; set; } = 1;
        [Required]
        [Display(Name = "Apartment type")]
        public  string ApartmentType { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "The maximum number of residents")]
        public int MaxTenants { get; set; }
        [Required]
        [Display(Name = "Price per hour")]
        [DataType(DataType.Text)]
        public int PricePerHour { get; set; }
        [Required]
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Text)]
        //[Remote("CheckNumberOfRooms", "Admin", ErrorMessage = "Number greater than its capacity")]
        public int CurrentTenants { get; set; }
        [Required]
        public bool Free { get; set; } = true;
        [Display(Name = "Arrival Date")]
        [DataType(DataType.DateTime)]
        public DateTime ArrivalDate { get; set; }
        [Display(Name = "Date of departure")]
        [DataType(DataType.DateTime)]
        public DateTime DateOfDeparture { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public int FinalPrice { get; set; }
        [Required]
        public string Visitor { get; set; } = "";

    }
    public enum TypesOfApartments
    {
        [Display(Name = "Standard")]
        Standard,
        [Display(Name = "Semi lux")]
        SemiLux,
        [Display(Name = "Lux")]
        Lux
    }
}
