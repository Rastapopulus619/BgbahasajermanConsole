using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Libraries.DateFormatting;
using bgbahasajermanDB_Libraries;

namespace bgbahasajermanDB_Operations_Test.Models
{
    public class NewLesson : Lesson
    {
        public NewLesson(LessonData lessonData)
            : base(lessonData)
        {

            SlotID = lessonData.SlotID;

            LessonID = lessonData.LessonID.ToString();  // *********** new addition
            LessonDateTime = lessonData.Date;
            LessonSlotTime = lessonData.Time;
            StartTime = lessonData.StartTime;
            EndTime = lessonData.EndTime;
            WeekdayNum = lessonData.WeekdayNum;

            LessonDateString = ConvertToCardDateFormat(lessonData.Date);

            SetParaf();
        }




    }
}
