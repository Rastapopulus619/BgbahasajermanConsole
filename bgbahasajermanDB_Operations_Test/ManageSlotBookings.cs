using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using MySqlX.XDevAPI.Common;

namespace bgbahasajermanDB_Operations_Test
{
    public static class ManageSlotBookings
    {
        public static int CheckAndSetIntensity(string studentID)
        { 

            int intensity;

            while (true)
            {
                    intensity = GetValidIntensityInput();

                    int currentLessonsPerWeek = GetCurrentIntensity(studentID);
                    int desiredLessonsPerWeek = intensity / 4;
                 
                    DisplayBookedSlots(studentID);

                    if(currentLessonsPerWeek == desiredLessonsPerWeek) 
                    {
                        bool userSlotsValidaitonChoice = YesNoChoice("Are the chosen slots correct?");

                        if(userSlotsValidaitonChoice) 
                        {
                            Console.WriteLine($"Chosen intensity is {intensity}");
                            break;
                        }
                        else
                        {
                            // CHANGE bookedslots and try the loop again
                            DisplayBookedSlots(studentID);
                            Console.WriteLine($"Chosen intensity of {intensity} is wrong, how to continue from here?");

                            Console.WriteLine("Add or delete slots?");
                            bool userAddSlotsChoice = YesNoChoice("Do you want to add slots?");
                            bool userDeleteSlotsChoice = YesNoChoice("Do you want to delete slots?");

                            if(userAddSlotsChoice)
                            {
                                InsertBooking(studentID);
                            }
                            
                            if(userDeleteSlotsChoice)
                            {
                                DeleteBooking(studentID);
                            }
                            
                            continue;
                            
                        }
                    }
                    else
                    {
                        Console.WriteLine("Current slots amount: " + currentLessonsPerWeek + " - Desired slots amount: " + desiredLessonsPerWeek);

                        if (currentLessonsPerWeek < desiredLessonsPerWeek)
                        {
                            // add a slotbooking for a student
                            InsertBooking(studentID);
                        }
                        else if (currentLessonsPerWeek > desiredLessonsPerWeek)
                        {
                            // delete a slotbooking(s) for a student
                            DeleteBooking(studentID);                              
                        }   

                        continue;
                    }

            }

            return intensity;
        }


    }
}
