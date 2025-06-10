using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using bgbahasajermanDB_Libraries;
using static bgbahasajermanDB_Libraries.DateFormatting;


namespace bgbahasajermanDB_Operations_Test.Models
{
    public class OldPackage : Package
    {
        public OldPackage(string studentID) 
            : base()
        {

            List<LessonData> lessonDataList = GetLessonData(studentID);
            //PackageLessonDates = GetLastPackageDates(studentID);
            //CreateSortedDatesList();
            Intensity = lessonDataList.Count();
            PackageNumber = GetLastPackageNumber(studentID); // do I need this here or can I get it from the query?

            foreach (LessonData lessonData in lessonDataList) // take it from here ******
            {
                OldLesson lesson = new OldLesson(lessonData);

                Lessons.Add(lesson);
            }

            PrintOldLessonsTable();

        }

        public void PrintOldLessonsTable()
        {
            Console.WriteLine("\nLast Package Lesson Dates:\n");
            Console.WriteLine("+---------------+----------------+--------+");
            Console.WriteLine("| Date          | Replaced       | Paraf? |");
            Console.WriteLine("+---------------+----------------+--------+");

            foreach (var lesson in Lessons)
            {
                PrintLessonRow(lesson);
            }

            Console.WriteLine("+---------------+----------------+--------+\n");
        }

        private void PrintLessonRow(Lesson lesson)
        {
            string dateColumn = ConvertToCardDateFormat(lesson.LessonDateTime);
            string replacementColumn = lesson.ReplacementLesson
                ? $"<- {ConvertToCardDateFormat(lesson.OriginalLessonDate)}"
                : " - ";
            string parafStatus = lesson.Paraf ? "Paraf" : "No Paraf";

            Console.WriteLine($"| {dateColumn,-13} | {replacementColumn,-14} | {parafStatus,-6} |");
        }

    }
}
