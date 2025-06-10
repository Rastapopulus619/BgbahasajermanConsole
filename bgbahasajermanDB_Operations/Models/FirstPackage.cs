using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Libraries.DateFormatting;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using static bgbahasajermanDB_Operations.PrintTables;
using bgbahasajermanDB_Libraries.LessonDataModels;
using bgbahasajermanDB_Libraries;
using System.Collections.Generic;
using System.Text;


namespace bgbahasajermanDB_Operations.Models
{
    public class FirstPackage : Package
    {
        public SortedDictionary<int, string[]> WeekdaysAndTimes { get; set; } = new SortedDictionary<int, string[]>();

        public FirstPackage(Student student)
            : base()
        {
            //student.SetStudentIntensity();    done in the Student class code** works?

            Console.WriteLine("Set First Lesson Date:");

            //GetAndSetFirstLessonDate
            DateTime userInputFirstDate = GetValidDateInput();
            string dateString = ConvertDateTimeToDateString(userInputFirstDate);
            SetFirstLesson(dateString, student.StudentID);

            PackageCompleted = false;
            PackageNumber = 1;

            //student.SetStudentIntensity();
            Console.WriteLine($"Students new Intensity: {student.NewIntensity}");

            PackageLessonDates = GetNewStudentsPackageDates(student.StudentID, student.NewIntensity);

            Console.WriteLine("FETCHED LESSONS COUNT: " + PackageLessonDates.Count);

            List<LessonData> newLessonDataList = GetNewLessonData(PackageLessonDates, student.StudentID, student.NewIntensity.ToString());

            Console.WriteLine($"NewLessonDataList Count: {newLessonDataList.Count()}");
            Intensity = newLessonDataList.Count();

            int NumberInPackageCounter = 0;//************ new

            foreach (LessonData newLessonData in newLessonDataList)
            {
                NewLesson lesson = new NewLesson(newLessonData);

                NumberInPackageCounter++;//************ new
                lesson.NumberInPackage = NumberInPackageCounter;//************ new

                lesson.ProductID = GetProductIDInt(student.Currency, student.Level);

                Lessons.Add(lesson);
            }

            OutstandingLessons = Intensity;
            CompletedLessons = 0;


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
    }
}
