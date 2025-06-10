using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Operations_Test.SetIntensity;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using bgbahasajermanDB_Operations_Test.Models;
using System.ComponentModel;

namespace bgbahasajermanDB_Operations_Test
{
    class Program
    {
        public static void Main(string[] args)
        {

            while(true) 
            {

                Student student = new Student();

                Console.WriteLine($"Do you want to add or delete a package for {student.StudentName}?");
                Console.WriteLine("1.Add");
                Console.WriteLine("2.Delete");

                string addOrDeleteChoice = NumericChoice().ToString();

                if(addOrDeleteChoice == "1") {

                    SetStudentIntensity(student);

                    student.CreateNewPackage();

                    bool continueToInsertDataChoice = YesNoChoice("Do you want to proceed to insert the new package into the Bg.DataBase?");

                    if (continueToInsertDataChoice)
                    {
                        InsertPackageLessonbookings(student);
                        InsertLessonPackage(student);
                        SetLessonsPackageIDs(student);
                        InsertLessonPayments(student);
                        AddAttendances(student);
                        Console.WriteLine("Successfully Inserted Package.");
                    }

                    bool printCardChoice = YesNoChoice("Proceed to Create LessonCard?");
                    if (printCardChoice)
                    {
                        createCardHtmlContent();
                        saveHtmlFile();
                    }

                }
                else
                {
                    Console.WriteLine("Deleting not yet implemented. Delete manually");
                    //deleteNewPackageFromDB(student);?
                    //PrintNewLastPackage();?

                }
            }
        }

        private static void saveHtmlFile()
        {
            throw new NotImplementedException();
        }

        private static void createCardHtmlContent()
        {
            throw new NotImplementedException();
        }

        private static void AddAttendances(Student student)
        {
            foreach (Lesson lesson in student.NewPackage.Lessons)
            {
                string attendedAlready = lesson.Paraf ? "1" : "0";
                if(attendedAlready == "1")
                {
                    InsertAttendance(lesson.BookingID, attendedAlready);
                }
            }
        }

        private static void InsertLessonPayments(Student student)
        {
            foreach (Lesson lesson in student.NewPackage.Lessons)
            {

                InsertPayment(student.StudentID, student.NewPackage.PaymentDateTimeString, lesson.ProductID, student.NewPackage.PackageID);
            }
        }


        //private static void deleteNewPackageFromDB(Student student)
        //{
        //    bool deleteLastAddedPackageChoice = YesNoChoice("Delete this Package?");
        //    if (deleteLastAddedPackageChoice)
        //    {
        //        //
        //    }
        //}

        private static void SetLessonsPackageIDs(Student student)
        {
            foreach(Lesson lesson in student.NewPackage.Lessons) 
            {
                SetPackageIDAndNumberInPackage(student.NewPackage.PackageID, lesson.NumberInPackage.ToString(), lesson.BookingID);
            }
        }

        private static void InsertPackageLessonbookings(Student student)
        {
            Console.WriteLine();
            Console.WriteLine("Inserting Lessonbookings..");
            foreach (Lesson lesson in student.NewPackage.Lessons)
            {
                InsertLessonBooking(student.StudentID, lesson.LessonID, lesson.ProductID);
                //Console.WriteLine("Lesson Added: " + lesson.LessonDateString);

                string lastInsertedBookingID = GetLastInsertedBookingID().ToString();
                student.NewPackage.BookingIDs.Add(lastInsertedBookingID);
                lesson.BookingID = lastInsertedBookingID;

            }
            //Console.WriteLine();


        }

        private static void InsertLessonPackage(Student student)
        {
            //convert bool to string for query
            string packageCompleted = student.NewPackage.PackageCompleted ? "1" : "0";
            
            InsertPackage(
                student.StudentID, 
                student.NewPackage.PackageNumber.ToString(), 
                packageCompleted, 
                student.NewPackage.Intensity.ToString(), // = lessonsAmount
                student.NewPackage.OutstandingLessons.ToString(), 
                student.NewPackage.CompletedLessons.ToString()
                );

            string lastInsertedPackageID = GetLastInsertedPackageID().ToString();

            student.NewPackage.PackageID = lastInsertedPackageID;

            foreach(Lesson lesson in student.NewPackage.Lessons)
            {
                lesson.PackageID = lastInsertedPackageID;
            }

        }
    }
}