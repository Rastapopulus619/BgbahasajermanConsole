using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Libraries.DateFormatting;
using bgbahasajermanDB_Libraries;

namespace bgbahasajermanDB_Operations_Test.Models
{
    public class OldLesson : Lesson
    {
        public OldLesson(LessonData lessonData)
            : base(lessonData)
        {
            BookingID = lessonData.BookingID.ToString();
            SlotID = lessonData.SlotID;
            LessonID = lessonData.LessonID.ToString();
            Console.WriteLine($"OldLessonID : {LessonID}");
            LessonDateTime = lessonData.Date;
            LessonSlotTime = lessonData.Time;
            StartTime = lessonData.StartTime;
            EndTime = lessonData.EndTime;
            WeekdayNum = lessonData.WeekdayNum;
            Reason = lessonData.Reason;

            LessonDateString = ConvertToCardDateFormat(lessonData.Date);
            SetReplacementLessonProperties(lessonData);
            SetParaf();

        }

        private void SetReplacementLessonProperties(LessonData lessonData)
        {
            OriginalLessonDate = lessonData.OriginalLessonDate.HasValue
                ? lessonData.OriginalLessonDate.Value
                : default(DateTime);

            if (OriginalLessonDate != default)
            {
                ReplacementLesson = true;
                OriginalLessonDateString = ConvertToCardDateFormat(OriginalLessonDate);
            }
            else
            {
                ReplacementLesson = false;
                OriginalLessonDateString = "";
            }

        }






    }
}
