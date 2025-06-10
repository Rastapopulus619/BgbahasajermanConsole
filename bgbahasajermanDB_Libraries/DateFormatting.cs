using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace bgbahasajermanDB_Libraries
{
    public static class DateFormatting
    {
        public static string FormatDateString(string dateString)
        {
            //Console.WriteLine(inputDate);
            // Assuming the inputDate is in 'DD/MM/YYYY' format
            string[] dateParts = dateString.Split('/');
            if (dateParts.Length == 3)
            {
                // Arrange the date parts in 'YYYY-MM-DD' format
                string convertedDate = $"{dateParts[2]}-{dateParts[1]}-{dateParts[0]}";
                //Console.WriteLine(convertedDate);

                return convertedDate;
            }
            else if (dateString == "")
            {
                Console.WriteLine("Empty date");
                return dateString;
            }
            else
            {
                // Handle invalid input
                //Console.WriteLine("Invalid date format");
                return dateString;
            }
        }
        public static DateTime ConvertDateStringToDateTime(string dateString)
        {
            // Assuming dateString is in 'YYYY-MM-DD' format
            if (DateTime.TryParse(dateString, out DateTime result))
            {
                return result;
            }
            else
            {
                // Handle invalid input
                Console.WriteLine("Invalid date format:");
                Console.WriteLine(dateString);
                return DateTime.MinValue; // or throw an exception, depending on your requirements
            }
        }
        public static string ConvertDateTimeToDateString(DateTime date)
        {
            // Format the DateTime object as 'YYYY-MM-DD'
            return date.ToString("yyyy-MM-dd");
        }

        public static string ConvertUserDateTimeStringToDBFormat(string dateString)
        {
            // Parse the input string in the format 'dd/MM/yyyy HH:mm'
            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                // Format the DateTime object as 'yyyy-MM-dd HH:mm:ss'
                return dateTime.ToString("yyyy-MM-dd HH:mm:00");
            }
            else
            {
                // Handle invalid input
                throw new ArgumentException("Invalid date format. Please use 'dd/MM/yyyy HH:mm'.");
            }
        }



        public static string ConvertToCardDateFormat(DateTime date)
        {
            string dayString = date.Day.ToString("00");
            string monthString = "";
            string yearString = date.Year.ToString("0000");

            switch (date.Month)
            {
                case 1:
                    monthString = "Jan";
                    break;
                case 2:
                    monthString = "Feb";
                    break;
                case 3:
                    monthString = "Mär"; // Mär for March
                    break;
                case 4:
                    monthString = "Apr";
                    break;
                case 5:
                    monthString = "Mai"; // Mai for May
                    break;
                case 6:
                    monthString = "Jun";
                    break;
                case 7:
                    monthString = "Jul";
                    break;
                case 8:
                    monthString = "Aug";
                    break;
                case 9:
                    monthString = "Sep";
                    break;
                case 10:
                    monthString = "Okt";
                    break;
                case 11:
                    monthString = "Nov";
                    break;
                case 12:
                    monthString = "Dez";
                    break;
                default:
                    // Handle the case for an unexpected month
                    break;
            }

            string formattedDate = $"{dayString} {monthString} {yearString}";
            return formattedDate;
        }
        public static string WeekdayNumberToWeekdayName(int weekdayNumber)
        {
            string weekdayName = "";

            switch (weekdayNumber)
            {
                case 1:
                    weekdayName = "Montag";
                    break;
                case 2:
                    weekdayName = "Dienstag";
                    break;
                case 3:
                    weekdayName = "Mittwoch";
                    break;
                case 4:
                    weekdayName = "Donnerstag";
                    break;
                case 5:
                    weekdayName = "Freitag";
                    break;
                case 6:
                    weekdayName = "Samstag";
                    break;
                case 7:
                    weekdayName = "Sonntag";
                    break;
            }

                    return weekdayName;
        }

        public static DateTime ConvertUserInputToDateObject()
        {
            string format = "dd/MM/yy";
            DateTime date = DateTime.MinValue; // Assign an initial value to date

            bool validDateEntered = false;
            while (!validDateEntered)
            {
                Console.WriteLine("Enter date in the format dd/MM/yy:");
                string userInput = Console.ReadLine();

                try
                {
                    date = DateTime.ParseExact(userInput, format, System.Globalization.CultureInfo.InvariantCulture);
                    validDateEntered = true;
                }
                catch (FormatException)
                {
                    // Display the error message and continue the loop
                    Console.WriteLine("Invalid date format. Please enter date in the format dd/MM/yy.");
                }
            }

            return date;
        }
        public static string ConvertUserInputToDBDateString()
        {
            try
            {
                // Prompt the user to enter a date in the format dd/MM/yy
                Console.WriteLine("Please enter a date in the format dd/MM/yy:");
                string userInput = Console.ReadLine();

                // Parse the input string into a DateTime object
                DateTime date = DateTime.ParseExact(userInput, "dd/MM/yy", null);

                // Get the year part and append "20" to it
                string yearPart = "20" + date.ToString("yy");

                // Format the date as "yyyy-MM-dd"
                string formattedDate = date.ToString($"yyyy-MM-dd");

                return formattedDate;
            }
            catch (FormatException)
            {
                // Handle invalid input format
                Console.WriteLine("Invalid date format. Please enter a date in the format dd/MM/yy.");
                return null;
            }
        }
        public static string ConvertDateTimeToDBDateString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        public static bool SetParaf(DateTime fullDateTime)
        {
            bool paraf = false;

            if (fullDateTime <= DateTime.Now)
            {
                paraf = true;
            }
            else
            {
                paraf = false;
            }

            return paraf;
        }
        public static bool SetParaf(DateTime lessonDateTime, TimeSpan lessonStartTime)
        {
            DateTime lessonEnd = lessonDateTime + lessonStartTime;
            bool paraf = false;

            if (lessonEnd <= DateTime.Now)
            {
                paraf = true;
            }
            else
            {
                paraf = false;
            }

            return paraf;
        }
    }
}
