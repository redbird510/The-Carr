using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PI_Portal.Classes
{
    public class LHRFAddresses
    {
        public int ID { get; set; }
        public string DropdownOption { get; set; } 
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public LHRFAddresses(int id, string dropdownOption, string locationName, string address, string city, string state, string postalCode)
        {
            ID = id;
            DropdownOption = dropdownOption;
            LocationName = locationName;
            Address = address;
            City = city;
            State = state;
            PostalCode = postalCode;
        }
    }
}