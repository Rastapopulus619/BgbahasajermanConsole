using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using bgbahasajermanDB_Libraries.LessonDataModels;
using static bgbahasajermanDB_Libraries.DateFormatting;
using bgbahasajermanDB_Libraries;

namespace bgbahasajermanDB_Operations.Models
{
    public class OldLesson : Lesson
    {
        public OldLesson(LessonData lessonData)
            : base(lessonData)
        {
            BookingID = lessonData.BookingID;
            SlotID = lessonData.SlotID;
            LessonID = lessonData.LessonID;
            //Console.WriteLine($"OldLessonID : {LessonID}");
            LessonDateTime = lessonData.Date;
            LessonSlotTime = lessonData.Time;
            StartTime = lessonData.StartTime;
            EndTime = lessonData.EndTime;
            WeekdayNum = lessonData.WeekdayNum;

            LessonDateString = ConvertToCardDateFormat(lessonData.Date);
            SetReplacementLessonProperties(lessonData);
            Paraf = SetParaf(LessonDateTime, StartTime);

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
