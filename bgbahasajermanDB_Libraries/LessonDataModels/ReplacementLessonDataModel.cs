

namespace bgbahasajermanDB_Libraries.LessonDataModels
{
    public class ReplacementLessonDataModel 
    {
        public int NumberInPackage { get; set; }
        public int PackageID { get; set; }
        public int BookingID { get; set; }
        public int LessonID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public bool Paraf {  get; set; }
        public string FormattedDate { get; set; }
        public int? OriginalLessonID { get; set; }
        public DateTime? OriginalLessonDate { get; set; }
        public string? FormattedOriginalLessonDate { get; set; }
        public string? Reason { get; set; }
    }

}
