using bgbahasajermanDB_Libraries;
using bgbahasajermanDB_Libraries.LessonDataModels;
using static bgbahasajermanDB_Libraries.MySqlQueryManager;

namespace bgbahasajermanDB_Operations.Models
{
    public class Lesson
    {
        public int PackageID { get; set; }
        public int PackageNumber { get; set; }
        public int NumberInPackage { get; set; }
        public int BookingID { get; set; }
        public int ProductID { get; set; }
        public int LessonID { get; set; }
        public int SlotID { get; set; }
        public DateTime LessonDateTime { get; set; }
        public string LessonDateString { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string LessonSlotTime { get; set; }
        public int WeekdayNum { get; set; }
        public string Weekday { get; set; }
        public int OriginalLessonID { get; set; }
        public DateTime OriginalLessonDate { get; set; }
        public string OriginalLessonDateString { get; set; }
        public string Reason { get; set; }
        public bool Rescheduled { get; set; } // same functionality as 'bool ReplacementLesson'? //
        public bool Paraf { get; set; }
        public bool ReplacementLesson { get; set; } // same functionality as 'bool Rescheduled'? //

        public Lesson(LessonData lessonData)
        {

        }
        public Lesson()
        {

        }

    }
}

/*      public string BookingID { get; set; }
        public string ProductID { get; set; }
        public string LessonID { get; set; }
        public DateTime LessonDateTime { get; set; }
        public string LessonDateString { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotID { get; set; }
        public string PackageID { get; set; }
        public int WeekdayNum { get; set; }
        public int PackageNumber { get; set; }
        public int NumberInPackage { get; set; }
        public bool Paraf { get; set; }
        public bool Rescheduled { get; set; }
        public DateTime OriginalLessonDate { get; set; }
        public string OriginalLessonDateString { get; set; } = "";
        public bool Payment { get; set; }
        public string LessonSlotTime { get; set; }
        public bool ReplacementLesson { get; set; }
*/