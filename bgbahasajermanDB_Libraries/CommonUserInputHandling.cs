using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static bgbahasajermanDB_Libraries.MySqlQueryManager;

namespace bgbahasajermanDB_Libraries
{
    public static class CommonUserInputHandling
    {
        public static bool YesNoChoice(string yesNoQuestion)
        {
            while (true)
            {
                Console.WriteLine(yesNoQuestion);
                Console.Write("Enter 'y' for yes or 'n' for no: ");
                string input = Console.ReadLine();

                if (string.Equals(input, "y", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else if (string.Equals(input, "n", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter 'y' or 'n'.");
                }
            }
        }

        public static int GetValidIntensityInput()
        {
            int intensity;

            Console.WriteLine(); // add an empty line
            Console.WriteLine("Enter desired intensity (4, 8, 12, 16, ..) up to 56: ");

            while (true)
            {
                string intensityInput = Console.ReadLine();

                if (int.TryParse(intensityInput, out intensity) && intensity > 0 && intensity % 4 == 0 && intensity < 57)
                {
                    return intensity; // return the valid input
                }
                else
                {
                    Console.WriteLine("Invalid intensity. Please enter a value that is divisible by 4 and up to 56.");
                    Console.WriteLine();
                }
            }
        }
        public static int GetValidSlotIDInput()
        {
            int slotID;

            Console.WriteLine(); // add an empty line
            Console.WriteLine("Enter SlotID (1 to 56): ");

            while (true)
            {
                string slotIDInput = Console.ReadLine();

                if (int.TryParse(slotIDInput, out slotID) && slotID >= 1 && slotID <= 56)
                {
                    return slotID; // return the valid input
                }
                else
                {
                    Console.WriteLine("Invalid SlotID. Please enter a value between 1 and 56.");
                    Console.WriteLine();
                }
            }
        }
        public static int GetValidSlotIDForDateInput(DateTime date)
        {
            int slotID;
            int min = 0;
            int max = 0;

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    //weekdayName = "Montag";
                    min = 1;
                    max = 8;
                    break;
                case DayOfWeek.Tuesday:
                    //weekdayName = "Dienstag";
                    min = 9;
                    max = 16;
                    break;
                case DayOfWeek.Wednesday:
                    //weekdayName = "Mittwoch";
                    min = 17;
                    max = 24;
                    break;
                case DayOfWeek.Thursday:
                    //weekdayName = "Donnerstag";
                    min = 25;
                    max = 32;
                    break;
                case DayOfWeek.Friday:
                    //weekdayName = "Freitag";
                    min = 33;
                    max = 40;
                    break;
                case DayOfWeek.Saturday:
                    //weekdayName = "Samstag";
                    min = 41;
                    max = 48;
                    break;
                case DayOfWeek.Sunday:
                    //weekdayName = "Sonntag";
                    min = 49;
                    max = 56;
                    break;
            }


            Console.WriteLine(); // add an empty line
            Console.WriteLine($"Enter SlotID ({min} to {max}): ");

            while (true)
            {
                string slotIDInput = Console.ReadLine();

                if (int.TryParse(slotIDInput, out slotID) && slotID >= min && slotID <= max)
                {
                    return slotID; // return the valid input
                }
                else
                {
                    Console.WriteLine($"Invalid SlotID. Please enter a value between {min} and {max}.");
                    Console.WriteLine();
                }
            }
        }

        public static bool CheckSlotAvailable(string slotNumber)
        {
            List<string> bookedSlots = GetAllBookedSlotIDs();

            bool available = true;

            if (bookedSlots.Contains(slotNumber))
            {
                available = false;
            }
            else
            {
                available = true;
            }

            return available;
        }

        public static string GetValidStudentName() 
        {
            List<string> students = GetAllStudentNames();

            while (true) 
            {
                Console.WriteLine("Type Student Name:");
                string studentName = Console.ReadLine();
                Console.WriteLine();

                if (students.Contains(studentName)) 
                { 
                    return studentName; 
                }
                else 
                {
                    Console.WriteLine("Invalid Student Name!");
                    continue; 
                }
            }
        }

        public static int NumericChoice()
        {
            while (true)
            {
                Console.Write("Enter '1' for option 1 or '2' for option 2: ");
                string input = Console.ReadLine();

                if (string.Equals(input, "1", StringComparison.OrdinalIgnoreCase))
                {
                    return 1;
                }
                else if (string.Equals(input, "2", StringComparison.OrdinalIgnoreCase))
                {
                    return 2;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter '1' or '2'.");
                }
            }
        }
        public static string GetValidLevelInput()
        {
            // Get the list of all levels from the database
            List<string> allLevels = GetAllLevels();

            // Prompt the user for input until a valid level is entered
            while (true)
            {
                Console.WriteLine("Enter a valid new level (A1, Prüfungstraining A1, Gespräch, ...):");
                string level = Console.ReadLine().Trim(); // Trim any leading or trailing whitespaces

                if (allLevels.Contains(level))
                {
                    return level; // Return the validated level
                }
                else
                {
                    Console.WriteLine("Invalid level! Please enter a valid level.");
                }
            }
        }

        public static DateTime GetValidDateTimeInput()
        {
            Console.WriteLine("\nEnter payment date and time (DD/MM/YYYY HH:MM):");

            while (true)
            {
                string input = Console.ReadLine();
                if (DateTime.TryParseExact(input, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime paymentDateTime))
                {
                    return paymentDateTime;
                }
                else
                {
                    Console.WriteLine("Invalid format. Please enter date and time in DD/MM/YYYY HH:MM format:");
                }
            }
        }
        public static DateTime GetValidDateInput()
        {
            Console.WriteLine("\nEnter date (DD/MM/YYYY):");

            while (true)
            {
                string input = Console.ReadLine();
                if (DateTime.TryParseExact(input, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }
                else
                {
                    Console.WriteLine("Invalid format. Please enter date in DD/MM/YYYY format:");
                }
            }
        }


        public static int GetValidNumberInRange(int min, int max)
        {
            int userInput;
            bool isValidInput = false;

            do
            {
                Console.WriteLine($"\nEnter a number between {min} and {max}, or press Enter to choose the maximum value.");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    userInput = max;
                    isValidInput = true;
                }
                else if (int.TryParse(input, out userInput))
                {
                    if (userInput >= min && userInput <= max)
                    {
                        isValidInput = true;
                    }
                    // take this out if 0 should be invalid!
                    //else if(userInput == 0)
                    //{
                    //    isValidInput = true;
                    //}
                    else
                    {
                        Console.WriteLine($"Input must be between {min} and {max}.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter an integer.");
                }

            } while (!isValidInput);

            return userInput;
        }
    }
}
