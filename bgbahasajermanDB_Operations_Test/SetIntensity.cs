using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Operations_Test.ManageSlotBookings;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using bgbahasajermanDB_Operations_Test.Models;

namespace bgbahasajermanDB_Operations_Test
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

                    Console.WriteLine("Delete and then Add. Don't change intensity/slot amount!");
                    DeleteBooking(student.StudentID);
                    InsertBooking(student.StudentID);
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
