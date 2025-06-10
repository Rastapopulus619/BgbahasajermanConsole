using bgbahasajermanDB_Libraries.LessonDataModels;


namespace bgbahasajermanDB_Operations.Models
{
    public class ExistingLesson : Lesson
    {
        public ExistingLesson(ExistingLessonDataModel lessonData)
            : base()
        {

            PackageID = lessonData.PackageID;
            PackageNumber = lessonData.PackageNumber;
            NumberInPackage = lessonData.NumberInPackage;
            BookingID = lessonData.BookingID;
            ProductID = lessonData.ProductID;
            LessonID = lessonData.LessonID;
            SlotID = lessonData.SlotID;
            LessonDateTime = lessonData.LessonDateTime;
            LessonDateString = lessonData.LessonDateString;
            StartTime = lessonData.StartTime;
            EndTime = lessonData.EndTime;
            LessonSlotTime = lessonData.LessonSlotTime;
            WeekdayNum = lessonData.WeekdayNum;
            Weekday = lessonData.Weekday;
            OriginalLessonID = lessonData.OriginalLessonID;
            OriginalLessonDate = lessonData.OriginalLessonDate;
            OriginalLessonDateString = lessonData.OriginalLessonDateString;
            Reason = lessonData.Reason;
            Paraf = lessonData.Paraf;
            
            //ReplacementLesson = Rescheduled?
            Rescheduled = OriginalLessonID != 0 ? true : false; 
            ReplacementLesson = OriginalLessonID != 0 ? true : false; // neccessary?
        }
    
    }
}
