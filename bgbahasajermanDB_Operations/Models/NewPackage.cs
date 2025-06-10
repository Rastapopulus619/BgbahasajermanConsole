using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using bgbahasajermanDB_Libraries.LessonDataModels;
using static bgbahasajermanDB_Libraries.DateFormatting;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using static bgbahasajermanDB_Operations.PrintTables;
using bgbahasajermanDB_Libraries;
using System.Collections.Generic;
using System.Text;

namespace bgbahasajermanDB_Operations.Models
{
    public class NewPackage : Package
    {

        public NewPackage(Student student)
            : base()
        {
            bool setPackageStartingDate = YesNoChoice("Set Lesson Starting Date manually? (for example returning student..");
            if(setPackageStartingDate) { new NotImplementedException(); }

            PackageNumber = student.OldPackage.PackageNumber + 1;
            PackageCompleted = false;

            Console.WriteLine($"Students new Intensity: {student.NewIntensity}");

            PackageLessonDates = GetNextPackageDates(student.StudentID, student.NewIntensity);

            Console.WriteLine("FETCHED LESSONS COUNT: " + PackageLessonDates.Count); 

            // ORI******* List<LessonData> newLessonDataList = GetNewLessonData(PackageLessonDates, student.StudentID);
            List<LessonData> newLessonDataList = GetNewLessonData(PackageLessonDates, student.StudentID, student.NewIntensity.ToString());

            Console.WriteLine($"NewLessonDataList Count: {newLessonDataList.Count()}");
            Intensity = newLessonDataList.Count();

            //Set NumberInPackage Values
            int NumberInPackageCounter = 0;//************ new

            //Double check whether a lesson with the same lessonID aleady exists in lessonbookings
            foreach (LessonData newLessonData in newLessonDataList)
            {
                NewLesson lesson = new NewLesson(newLessonData);

                //************ new
                bool studentIDExists = student.OldPackage.Lessons.Any(oldLesson => oldLesson.LessonID == lesson.LessonID);

                if (studentIDExists)
                {
                    continue;
                }
                //************ new


                NumberInPackageCounter++;//************ new
                lesson.NumberInPackage = NumberInPackageCounter;//************ new

                lesson.ProductID = GetProductIDInt(student.Currency, student.Level);

                Lessons.Add(lesson);
            }

            SetOutstandingAndCompletedLessons();

            PaymentDateTime = GetValidDateTimeInput();
            PaymentDateTimeString = PaymentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            SetWeekdaysAndTimes();

            student.Packages.Add(this);
            student.NewPackage = this;

            PrintNewLessonsTable(student);

        }

        private void SetWeekdaysAndTimes()
        {
            foreach (Lesson lesson in Lessons)
            {
                // Create an array containing the weekday name and lesson slot time
                string[] weekdayAndTime = { WeekdayNumberToWeekdayName(lesson.WeekdayNum), lesson.LessonSlotTime };

                // Check if the key already exists in the dictionary
                if (!WeekdaysAndTimes.ContainsKey(lesson.SlotID))
                {
                    // Add the array to the dictionary only if the key doesn't exist
                    WeekdaysAndTimes.Add(lesson.SlotID, weekdayAndTime);
                }
                //else
                //{
                    // Handle the case where the key already exists (optional)
                    // You can choose to ignore, update, or handle this case according to your requirements
                    //Console.WriteLine($"Key {lesson.WeekdayNum} already exists in the dictionary.");
                //}
            }
        }
        private void SetOutstandingAndCompletedLessons()
        {

            int outstandingLessonsCounter = 0;
            int completedLessonsCounter = 0;

            foreach (Lesson lesson in Lessons)
            {
                if(lesson.Paraf)
                {
                    completedLessonsCounter++;
                }
                else
                {
                    outstandingLessonsCounter++; 
                }
            }

            OutstandingLessons = outstandingLessonsCounter;
            CompletedLessons = completedLessonsCounter;

        }
        private void CheckAndUpdateLevel(Student student)
        {
            //maybe later add more dynamic level change with level change logging information
            //to calculate the real price of a package with a level change in it

            Console.WriteLine($"\nStudent's level is: {student.Level}");
            bool levelChangeChoice = YesNoChoice("Does the level have to change?");

            if (levelChangeChoice)
            {
                string levelInput = GetValidLevelInput();
                student.Level = levelInput;

                UpdateLevel(student.StudentID, levelInput);

            }
        }

    //old print table code>>
    /*
            public void PrintNewLessonsTable(Student student)
            {
                // Print the table header
                PrintNewLessonsTableHeaderSection(student);

                // Print the lesson rows
                foreach (var lesson in Lessons)
                {
                    PrintLessonRow(lesson);
                }

                // Print the bottom border of the table
                Console.WriteLine("+---------------+---------------+---------------+---------------+\n");
            }

            private void PrintLessonRow(Lesson lesson)
            {
                string parafStatus = lesson.Paraf ? "Paraf" : "No Paraf";
                string test = "";


                // Print the formatted string
                Console.WriteLine($"| {lesson.LessonDateString.PadRight(13)} | {lesson.LessonSlotTime.PadRight(13)} | {test.PadRight(13)} | {parafStatus.PadRight(13)} |");
            }

            public void PrintNewLessonsTableHeaderSection(Student student)
            {
                Console.WriteLine("\nLast Package Lesson Dates:\n");
                Console.WriteLine("+---------------+---------------+---------------+---------------+");
                Console.WriteLine($"| Name:         | {student.StudentName.PadRight(13)} | Dauer:        | 90min/UE      |");
                Console.WriteLine("+---------------+---------------+---------------+---------------+");
                Console.WriteLine($"| Stufe:        | {student.Level.PadRight(13)} | Intensität:   | {Intensity.ToString().PadRight(13)} |");
                Console.WriteLine("+---------------+---------------+---------------+---------------+");
                
                bool firstRow = true;

                foreach (var kvp in WeekdaysAndTimes)
                {
                    if (firstRow)
                    {
                        // Print the formatted string for the first row
                        Console.WriteLine($"| Tage:         | {kvp.Value[0].PadRight(13)} | Zeit:         | {kvp.Value[1].PadRight(13)} |");
                        Console.WriteLine("+---------------+---------------+---------------+---------------+");
                        firstRow = false;
                    }
                    else
                    {
                        // Print the formatted string for subsequent rows
                        Console.WriteLine($"|               | {kvp.Value[0].PadRight(13)} |               | {kvp.Value[1].PadRight(13)} |");
                        Console.WriteLine("+---------------+---------------+---------------+---------------+");
                    }
                }
            }
    */

    }
}
