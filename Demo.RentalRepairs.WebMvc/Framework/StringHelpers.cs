using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Demo.RentalRepairs.WebMvc.Framework
{
    public static class StringHelpers
    {

        public static string  SplitCamelCase(this string thisIsMyCapsDelimitedString)
        {
            return  Regex.Replace(thisIsMyCapsDelimitedString, "(\\B[A-Z])", " $1");
            
        }
    }
}
