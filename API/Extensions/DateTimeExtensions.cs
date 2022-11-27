using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            
            var age = today.Year - dob.Year;
            //if they havent had their birthday already this year, then minus a year off the age
            if(dob > today.AddYears(-age)) age--;

            return age;
           
        }
    }
}