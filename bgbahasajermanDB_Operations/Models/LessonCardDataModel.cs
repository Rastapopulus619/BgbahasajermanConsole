using System.Text;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;


namespace bgbahasajermanDB_Operations.Models
{
    public class LessonCardDataModel
    {
        public string PageTitle { get; set; } //fertig
        public string StudentName { get; set; } //fertig
        public string Level { get; set; } //fertig
        public int NewIntensity { get; set; } //fertig
        public int OldIntensity { get; set; } //fertig
        public bool AddOldLessonChoice { get; set; } //fertig
        //  -   -   -   -   -   -   -   -   -  -   -   -   -   -   -   -   -   -  -   -   -   -   -   -   -   -   -
        public int NumberInPackage { get; set; }
        public int PackageID { get; set; }
        public int BookingID { get; set; }
        public int LessonID { get; set; }
        public DateTime Date { get; set; }
        public bool Paraf {  get; set; }
        public string FormattedDate { get; set; }
        public int? OriginalLessonID { get; set; }
        public DateTime? OriginalLessonDate { get; set; }
        public string? FormattedOriginalLessonDate { get; set; }
        public string? Reason { get; set; }
        //  -   -   -   -   -   -   -   -   -  -   -   -   -   -   -   -   -   -  -   -   -   -   -   -   -   -   -
        //public List<int> WeekdayNumbers { get; set; } unused?
        //public Dictionary<int, string> Weekdays { get; set; } = new Dictionary<int, string>(); //fertig
        public List<int> Weekdays { get; set; } = new List<int>(); //testing *********
        public string WeekdaysHtmlString { get; set; } //fertig
        public Dictionary<int, string> LessonTimes { get; set; } = new Dictionary<int, string>(); //fertig
        public string LessonTimesHtmlString { get; set; } //fertig
        //  -   -   -   -   -   -   -   -   -  -   -   -   -   -   -   -   -   -  -   -   -   -   -   -   -   -   -
        public string NewTableHtmlString { get; set; }
        public string OldTableHtmlString { get; set; }
        public Dictionary<int, string> OldPackageDateStrings { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, bool> OldPackageParafList { get; set; } = new Dictionary<int, bool>();
        public Dictionary<int, string> NewPackageDateStrings { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, bool> NewPackageParafList { get; set; } = new Dictionary<int, bool>();

        public LessonCardDataModel(Student student)
        {

            #region Card Header Data

            PageTitle = $"slots{student.NewIntensity}x";
            StudentName = student.StudentName;
            Level = student.Level;
            NewIntensity = student.NewIntensity;
            OldIntensity = student.OldIntensity;



            /*            // Assuming student.OldPackage.Lessons is the list of lessons
                        List<Lesson> uniqueSlotLessons = student.NewPackage.Lessons
                            .GroupBy(lesson => lesson.SlotID) // Group lessons by SlotID
                            .Select(group => group.First())    // Select the first lesson from each group
                            .OrderBy(lesson => lesson.SlotID)  // Order the lessons by SlotID
                            .ToList();


                        // Create Weekdays and LessonTimes Html strings
                        for (int i = 0; i < uniqueSlotLessons.Count; i++)
                        {
                            if(i != uniqueSlotLessons.Count - 1)
                            {
                                Weekdays.Add(i, $"{student.NewPackage.Lessons[i].Weekday}<br>");
                                LessonTimes.Add(i, $"{student.NewPackage.Lessons[i].LessonSlotTime}<br>");
                            }
                            else
                            {
                                Weekdays.Add(i, student.NewPackage.Lessons[i].Weekday);
                                LessonTimes.Add(i, student.NewPackage.Lessons[i].LessonSlotTime);
                            }
                        }




                        // Concatenate Weekdays
                        StringBuilder weekdaysBuilder = new StringBuilder();
                        foreach (var kvp in Weekdays.OrderBy(kvp => kvp.Key))
                        {
                            weekdaysBuilder.Append(kvp.Value);
                        }
                        WeekdaysHtmlString = weekdaysBuilder.ToString();

                        // Concatenate LessonTimes
                        StringBuilder lessonTimesBuilder = new StringBuilder();
                        foreach (var kvp in LessonTimes.OrderBy(kvp => kvp.Key))
                        {
                            lessonTimesBuilder.Append(kvp.Value);
                        }
                        LessonTimesHtmlString = lessonTimesBuilder.ToString();
            */

            // Create Weekdays and LessonTimes Html strings
            StringBuilder weekdaysBuilder = new StringBuilder();
            StringBuilder lessonTimesBuilder = new StringBuilder();

            foreach (var kvp in student.NewPackage.WeekdaysAndTimes.OrderBy(kvp => kvp.Key))
            {
                string weekday = kvp.Value[0];
                string lessonTime = kvp.Value[1];

                // Append weekday and lesson time to the respective builders
                weekdaysBuilder.Append(weekday);
                lessonTimesBuilder.Append(lessonTime);

                // Add <br> tag after each entry except the last one
                if (!kvp.Equals(student.NewPackage.WeekdaysAndTimes.Last()))
                {
                    weekdaysBuilder.Append("<br>");
                    lessonTimesBuilder.Append("<br>");
                }
            }

            // Set WeekdaysHtmlString and LessonTimesHtmlString
            WeekdaysHtmlString = weekdaysBuilder.ToString();
            LessonTimesHtmlString = lessonTimesBuilder.ToString();



            #endregion

            #region New Package TableHtml Creator

            for (int i = 0; i < student.NewPackage.Lessons.Count; i++)
            {
                NewPackageDateStrings.Add(i, $"<td>{student.NewPackage.Lessons[i].LessonDateString}</td>");
                NewPackageParafList.Add(i, student.NewPackage.Lessons[i].Paraf);
            }

            StringBuilder tableBuilder = new StringBuilder();
            tableBuilder.Append("<table class=\"slotsTable\"><tr>");

            for (int i = 0; i < student.NewPackage.Lessons.Count; i++)
            {
                if (i % 2 == 0 && i != 0)
                {
                    tableBuilder.Append("</tr><tr>");
                }

                tableBuilder.Append(NewPackageDateStrings[i]);
            }

            tableBuilder.Append("</tr></table>");

            NewTableHtmlString = tableBuilder.ToString();

            #endregion

            #region Old Package TableHtml Creator

            AddOldLessonChoice = YesNoChoice("Add Prior Package to LessonCard?");
            if (AddOldLessonChoice)
            {
                for (int i = 0; i < student.OldPackage.Lessons.Count; i++)
                {
                    if (!student.OldPackage.Lessons[i].Rescheduled)
                    {
                        OldPackageDateStrings.Add(i, $"<td>{student.OldPackage.Lessons[i].LessonDateString}</td>");
                    }
                    else
                    {
                        OldPackageDateStrings.Add(i, $"<td>{student.OldPackage.Lessons[i].OriginalLessonDateString}<br>↪ {student.OldPackage.Lessons[i].LessonDateString}</td>");
                    }

                    OldPackageParafList.Add(i, student.OldPackage.Lessons[i].Paraf);
                }

                // Now, construct the HTML table string using a StringBuilder
                StringBuilder oldTableBuilder = new StringBuilder();
                oldTableBuilder.Append("<table class=\"slotsTable\"><tr>");

                for (int i = 0; i < student.OldPackage.Lessons.Count; i++)
                {
                    if (i % 2 == 0 && i != 0)
                    {
                        oldTableBuilder.Append("</tr><tr>");
                    }

                    oldTableBuilder.Append(OldPackageDateStrings[i]);
                }

                oldTableBuilder.Append("</tr></table>");

                // Store the HTML table string in the OldTableHtmlString property
                OldTableHtmlString = oldTableBuilder.ToString();

            }
            else
            {

            }

            #endregion

        }

    }

}