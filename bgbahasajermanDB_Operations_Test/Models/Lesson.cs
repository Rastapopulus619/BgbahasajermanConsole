using bgbahasajermanDB_Libraries;
using static bgbahasajermanDB_Libraries.MySqlQueryManager;

namespace bgbahasajermanDB_Operations_Test.Models
{
    public class Lesson
    {
        public string BookingID { get; set; }
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
        public string Reason { get; set; }
        public DateTime OriginalLessonDate { get; set; }
        public string OriginalLessonDateString { get; set; } = "";
        public bool Payment { get; set; }
        public string LessonSlotTime { get; set; }
        public bool ReplacementLesson { get; set; }




        //public bool Absen { get; set; }
        //public bool ReplacedLesson { get; set; }
        //public bool DoubleReplacementLesson { get; set; }
        //public bool TambahanLesson { get; set; }
        //public bool IndividualHoliday { get; set; }
        //public bool DoubleLesson { get; set; }
        //public bool DoubleLessonSecondLesson { get; set; }
        //public bool DQ { get; set; }
        //public bool Selesai { get; set; }
        //public string OriginalLessonID { get; set; }
        //public string ReplacementLessonDateString { get; set; }
        //public string ReplacementLessonID { get; set; }


        public Lesson(LessonData lessonData)
        {

        }
        public Lesson()
        {

        }

        public void SetParaf()
        {
            DateTime lessonEnd = LessonDateTime + EndTime;

            if (lessonEnd <= DateTime.Now)
            {
                Paraf = true;
            }
            else
            {
                Paraf = false;
            }
        }
    }
}
