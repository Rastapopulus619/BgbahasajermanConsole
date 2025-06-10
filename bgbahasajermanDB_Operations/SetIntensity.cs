using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Operations.ManageSlotBookings;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using bgbahasajermanDB_Operations.Models;

namespace bgbahasajermanDB_Operations
{
    public static class SetIntensity
    {
        public static void SetStudentIntensity(Student student) //, bool hasBooking
        {
            Console.WriteLine("Set Intensity:");


            bool userIntensityChoice = false;
                    
            if (student.OldIntensity > 0) 
            { 
                userIntensityChoice = YesNoChoice($"Do you want to renew the package with an intensity of {student.OldIntensity}?");
            }

            if (userIntensityChoice)
            {
                DisplayBookedSlots(student.StudentID);
                student.NewIntensity = student.OldIntensity;


                bool changeSlotChoice = YesNoChoice("Do you want to change the slot(s)?");
                if (changeSlotChoice)
                {
                    int slotsAmount = 0;

                    while (student.NewIntensity != slotsAmount)
                    {
                        slotsAmount = GetCurrentIntensity(student.StudentID);
                        
                        Console.WriteLine($"Delete and then Add. Keep the Slots amount at: {slotsAmount} Slots / Intensity of {student.NewIntensity} ");

                        DeleteBooking(student.StudentID);
                        InsertBooking(student.StudentID);
                    }

                }
                        
                // unchanged intensity value
            }
            else
            {
                // change intensity value
                student.NewIntensity = CheckAndSetIntensity(student.StudentID);
            }

        }

    }
}
