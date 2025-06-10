using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using bgbahasajermanDB_Libraries.LessonDataModels;
using static bgbahasajermanDB_Libraries.DateFormatting;
using static bgbahasajermanDB_Operations.PrintTables;

namespace bgbahasajermanDB_Operations.Models
{
    public class ExistingPackage : Package
    {
        public ExistingPackage(string studentID, int packageNumber) 
            : base()
        {
            List<ExistingLessonDataModel> lessonDataList = GetAndSetExistingLessonsData(studentID, packageNumber);
            //List<LessonData> lessonDataList = GetAnyPackagesLessonData(studentID, packageNumber.ToString());          //old lessonData line 

            //PackageLessonDates = GetLastPackageDates(studentID);
            //CreateSortedDatesList();
            PackageID = lessonDataList[0].PackageID;
            PackageNumber = packageNumber;
            PackageCompleted = lessonDataList[lessonDataList.Max(x => x.NumberInPackage)-1].Paraf;
            Intensity = lessonDataList.Count();
            OutstandingLessons = lessonDataList.Count(x => x.Paraf == false);
            CompletedLessons = Intensity - OutstandingLessons;


            //PackageNumber = GetLastPackageNumber(studentID); // do I need this here or can I get it from the query?

            foreach (ExistingLessonDataModel lessonData in lessonDataList) // take it from here ******
            {
                Lesson lesson = new ExistingLesson(lessonData);

                Lessons.Add(lesson);
                
                /*          Necessary? Or 
                PackageLessonDates.Add(lesson.LessonDateTime);
                BookingIDs.Add(lesson.BookingID);
                WeekdaysInt.Add(lesson.WeekdayNum);
                Weekdays.Add(lesson.Weekday);
                Times.Add(lesson.LessonSlotTime);
                */

            }

            SetWeekdaysAndTimes();

            //SortedPackageLessons = Lessons.OrderBy(x => x.LessonDateTime).ToList();
            // Selected Lesson is user-defined

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
