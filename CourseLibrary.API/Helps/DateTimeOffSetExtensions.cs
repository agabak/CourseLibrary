using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Helps
{
    public static class DateTimeOffSetExtensions
    {
        public static int GetCurrentAge(this DateTimeOffset dateOfBirth)
        {
            var currentYear = DateTime.UtcNow;
            int age = currentYear.Year - dateOfBirth.Year;
            if(currentYear < dateOfBirth.AddYears(age))
            {
                age--;
            }
            return age;
        } 
    }
}
