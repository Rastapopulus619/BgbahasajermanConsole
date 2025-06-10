namespace bgbahasajermanDB_Libraries.LessonDataModels
{
    public class ExistingLessonDataModel
    {
        public int PackageID { get; set; }
        public int PackageNumber { get; set; }
        public int NumberInPackage { get; set; }
        public int BookingID { get; set; }
        public int ProductID { get; set; } // BELUM!***
        public int LessonID { get; set; }
        public int SlotID { get; set; }
        public DateTime LessonDateTime { get; set; }
        public string LessonDateString { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string LessonSlotTime { get; set; }
        public int WeekdayNum { get; set; }
        public string Weekday { get; set; } // add to execution method
        public int OriginalLessonID { get; set; }
        public DateTime OriginalLessonDate { get; set; }
        public string OriginalLessonDateString { get; set; }
        public string Reason { get; set; }

        //public bool Rescheduled { get; set; } ? needed?
        public bool Paraf { get; set; }
        public DateTime SortingDate { get; set; }
    }
}

