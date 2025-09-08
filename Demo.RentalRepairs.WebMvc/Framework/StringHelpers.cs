using System.Text.RegularExpressions;

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
