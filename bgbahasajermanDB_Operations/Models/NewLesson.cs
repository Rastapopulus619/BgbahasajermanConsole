using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Libraries.DateFormatting;
using bgbahasajermanDB_Libraries.LessonDataModels;
using bgbahasajermanDB_Libraries;

namespace bgbahasajermanDB_Operations.Models
{
    public class NewLesson : Lesson
    {
        public NewLesson(LessonData lessonData)
            : base(lessonData)
        {

            SlotID = lessonData.SlotID;

            LessonID = lessonData.LessonID;  // *********** new addition
            LessonDateTime = lessonData.Date;
            LessonSlotTime = lessonData.Time;
            StartTime = lessonData.StartTime;
            EndTime = lessonData.EndTime;
            WeekdayNum = lessonData.WeekdayNum;

            LessonDateString = ConvertToCardDateFormat(lessonData.Date);

            Paraf = SetParaf(LessonDateTime, StartTime);
        }




    }
}
