using AnnaWebDiningFin.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AnnaWebDiningFin.Data.MenuData
{
    public class RegisterMenu
    {
        public long RestaurantId { get; set; } = 1;
        public string RestaurantName { get; set; } = "Oliva";
        public int MenuItems { get; set; }
        public List<Food> MenuList { get; set; }
        public float Rating { get; set; } = 5;

        public RegisterMenu()
        {

        }

        //[JsonConstructor]
        //public RegisterMenu(long RestaurantId, string RestaurantName, int MenuItems,
        //    List<Food> MenuList, float Rating)
        //{
        //    this.RestaurantId = RestaurantId;
        //    this.RestaurantName = RestaurantName;
        //    this.MenuItems = MenuItems;
        //    this.MenuList = MenuList;
        //    this.Rating = Rating;
        //}
    }
}
