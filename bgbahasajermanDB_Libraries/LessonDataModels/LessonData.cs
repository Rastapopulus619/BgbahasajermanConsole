namespace bgbahasajermanDB_Libraries.LessonDataModels
{
    public class LessonData // This is a class for the capturing of the oldLessons DataSet ** DON'T DELETE!
    {
        public DateTime Date { get; set; }
        public int BookingID { get; set; }
        public int LessonID { get; set; }
        public int SlotID { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Time { get; set; }
        public int? OriginalLessonID { get; set; }
        public DateTime? OriginalLessonDate { get; set; } // Adjusted for OriginalLessonDate
        public int WeekdayNum { get; set; } // Added WeekdayNum
        public string Reason { get; set; }
    }

}
