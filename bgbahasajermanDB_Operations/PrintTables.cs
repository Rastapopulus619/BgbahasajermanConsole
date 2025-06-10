
using static bgbahasajermanDB_Libraries.DateFormatting;
using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using bgbahasajermanDB_Operations.Models;

namespace bgbahasajermanDB_Operations
{
    public static class PrintTables
    {
        public static void PrintLessonsList(List<Lesson> lessons)
        {
            Console.WriteLine("+--------------+----------+----------+---------+--------------+--------------+--------------+-----------------+-----------------+-----------------+");
            Console.WriteLine("| NumInPack    | PackID   | BookID   | LesID   | Date         | Day          | Time         | OriLesID        | OriLesDate      | Reason          |");
            Console.WriteLine("+--------------+----------+----------+---------+--------------+--------------+--------------+-----------------+-----------------+-----------------+");

            foreach (Lesson lesson in lessons)
            {
                string formattedLessonDate = ConvertToCardDateFormat(lesson.LessonDateTime);
                string originalLessonDate = "";
                if (lesson.OriginalLessonDate != DateTime.MinValue)
                {
                    originalLessonDate = ConvertToCardDateFormat(lesson.OriginalLessonDate);
                }

                Console.WriteLine($"| {lesson.NumberInPackage.ToString(),-12} | {lesson.PackageID.ToString(),-8} | {lesson.BookingID.ToString(),-8} | {lesson.LessonID.ToString(),-7} | {formattedLessonDate,-12} | {lesson.Weekday,-12} | {lesson.LessonSlotTime,-12} | {lesson.OriginalLessonID.ToString(),-15} | {originalLessonDate,-15} | {lesson.Reason,-15} |");
            }

            Console.WriteLine("+--------------+----------+----------+---------+--------------+--------------+--------------+-----------------+-----------------+-----------------+");
        }
        //public static void PrintNewLessonsList(List<Lesson> lessons)
        //{
        //    Console.WriteLine("+--------------+----------+----------+---------+--------------+--------------+--------------+---------+");
        //    Console.WriteLine("| NumInPack    | PackID   | BookID   | LesID   | Date         | Day          | Time         | Paraf?  |");
        //    Console.WriteLine("+--------------+----------+----------+---------+--------------+--------------+--------------+---------+");

        //    foreach (Lesson lesson in lessons)
        //    {
        //        string formattedLessonDate = ConvertToCardDateFormat(lesson.LessonDateTime);
        //        string originalLessonDate = "";
        //        if (lesson.OriginalLessonDate != DateTime.MinValue)
        //        {
        //            originalLessonDate = ConvertToCardDateFormat(lesson.OriginalLessonDate);
        //        }
        //        string parafStatus = lesson.Paraf ? "Paraf" : "No Paraf";

        //        Console.WriteLine($"| {lesson.NumberInPackage.ToString(),-12} | {lesson.PackageID.ToString(),-8} | {lesson.BookingID.ToString(),-8} | {lesson.LessonID.ToString(),-7} | {formattedLessonDate,-12} | {lesson.Weekday,-12} | {lesson.LessonSlotTime,-12} | {parafStatus,-8} |");
        //    }

        //    Console.WriteLine("+--------------+----------+----------+---------+--------------+--------------+--------------+---------+");
        //}
        public static void PrintExistingPackageTable(List<Lesson> lessons)
        {
            //Console.WriteLine("\nLast Package Lesson Dates:\n");
            Console.WriteLine("+---------------+----------------+----------+");
            Console.WriteLine("| Date          | Replaced       | Paraf?   |");
            Console.WriteLine("+---------------+----------------+----------+");

            foreach (var lesson in lessons)
            {
                PrintLessonRow(lesson);
            }

            Console.WriteLine("+---------------+----------------+----------+\n");
        }

        public static void PrintLessonRow(Lesson lesson)
        {
            string dateColumn = ConvertToCardDateFormat(lesson.LessonDateTime);
            string replacementColumn = lesson.ReplacementLesson
                ? $"<- {ConvertToCardDateFormat(lesson.OriginalLessonDate)}"
                : " - ";
            string parafStatus = lesson.Paraf ? "Paraf" : "No Paraf";

            Console.WriteLine($"| {dateColumn,-13} | {replacementColumn,-14} | {parafStatus,-8} |");
        }
        public static void PrintNewLessonsTable(Student student)
        {
            // Print the table header
            PrintNewLessonsTableHeaderSection(student);

            // Print the lesson rows
            foreach (var lesson in student.NewPackage.Lessons)
            {
                PrintNewLessonRow(lesson);
            }

            // Print the bottom border of the table
            Console.WriteLine("+---------------+---------------+---------------+---------------+\n");
        }

        private static void PrintNewLessonRow(Lesson lesson)
        {
            string parafStatus = lesson.Paraf ? "Paraf" : "No Paraf";
            string test = "";


            // Print the formatted string
            Console.WriteLine($"| {lesson.LessonDateString.PadRight(13)} | {lesson.LessonSlotTime.PadRight(13)} | {test.PadRight(13)} | {parafStatus.PadRight(13)} |");
        }

        public static void PrintNewLessonsTableHeaderSection(Student student)
        {
            Console.WriteLine("\nLast Package Lesson Dates:\n");
            Console.WriteLine("+---------------+---------------+---------------+---------------+");
            Console.WriteLine($"| Name:         | {student.StudentName.PadRight(13)} | Dauer:        | 90min/UE      |");
            Console.WriteLine("+---------------+---------------+---------------+---------------+");
            Console.WriteLine($"| Stufe:        | {student.Level.PadRight(13)} | Intensität:   | {student.NewIntensity.ToString().PadRight(13)} |");
            Console.WriteLine("+---------------+---------------+---------------+---------------+");

            bool firstRow = true;

            foreach (var kvp in student.NewPackage.WeekdaysAndTimes)
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

    }
}
