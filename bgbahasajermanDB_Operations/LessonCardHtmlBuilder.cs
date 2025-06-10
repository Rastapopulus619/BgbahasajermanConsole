//using bgbahasajermanDB_Operations_Test.Models;
using bgbahasajermanDB_Libraries.LessonDataModels;
using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using static bgbahasajermanDB_Libraries.DateFormatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bgbahasajermanDB_Operations.Models;

namespace bgbahasajermanDB_Operations
{
    public class LessonCardHtmlBuilder
    {
        public string HtmlInputPath = @"C:\Programmieren\bgbahasajermanApp_Combo\bgbahasajermanDB_Operations\Html\ScheduleCard.html";
        //** delete? //public string HtmlInputPath = @"C:/Programmieren/bgbahasajermanApp/TimCoreyCourseConsoleApp1/TimCoreyCourseConsoleApp1/ScheduleCard.html";
        //public string HtmlOutputPath = @"C:\Programmieren\bgbahasajermanApp_Combo\bgbahasajermanDB_Operations\ScheduleCardTesting\testing.html";
        //** delete? //public string HtmlOutputPath = @"C:/Programmieren/ProgrammingProjects/PythonProject/src/GoogleAppsAutomation/GoogleDriveNative/Downloads/SlotsPictures/test/testing.html";
        //public Student Student { get; set; }
        //public Package ChosenPriorPackage { get; set; }
        //public Package ChosenPrintPackage { get; set; }
        //public string OldPackageTable { get; set; }
        //public string NewPackageTable { get; set; }
        //public string ReplacementLessons { get; set; }
        public string SeperationBar { get; set; } = "< div class=\"subtitleBox\" id=\"subtitleBox1\"></div>";
        public string GeneratedHtmlContent { get; private set; }
        public LessonCardHtmlBuilder(LessonCardDataModel lessonCardData) 
        {
/*            Student = chosenStudent;
            ChosenPrintPackage = selectedPackage;*/

            StringBuilder htmlBuilder = FetchHtmlContent();

            htmlBuilder.Replace("{name}", lessonCardData.StudentName)
                       .Replace("{stufe}", lessonCardData.Level)
                       .Replace("{intensität}", lessonCardData.NewIntensity.ToString());


            //Create List for adding days and times
           /* List<Lesson> daysAndTimesSelectedLessons = new List<Lesson>();
            
            for (int i = 1; i < 8; i++)
            {
                Lesson lesson = selectedPackage.Lessons.FirstOrDefault(x => x.WeekdayNum == i);
                if (lesson != null && !daysAndTimesSelectedLessons.Any(l => l.SlotID == lesson.SlotID))
                {
                    daysAndTimesSelectedLessons.Add(lesson);
                }
            }

            string tage = "";
            string zeiten = "";

            // Create a StringBuilder to efficiently concatenate strings
            StringBuilder tageStringBuilder = new StringBuilder();
            StringBuilder zeitenStringBuilder = new StringBuilder();

            foreach (Lesson lesson in daysAndTimesSelectedLessons)
            {
                // Append the lesson details to the StringBuilder
                tageStringBuilder.Append($"{lesson.Weekday}");
                zeitenStringBuilder.Append($"{lesson.LessonSlotTime}");

                // Add <br> tag after each lesson except the last one
                if (lesson != daysAndTimesSelectedLessons.Last())
                {
                    tageStringBuilder.Append("<br>");
                    zeitenStringBuilder.Append("<br>");
                }
            }*/

            // Convert the StringBuilder to a string
/*            string tageString = tageStringBuilder.ToString();
            string zeitenString = tageStringBuilder.ToString();*/

            htmlBuilder.Replace("{tage}", lessonCardData.WeekdaysHtmlString)
                       .Replace("{zeit}", lessonCardData.LessonTimesHtmlString);

            if(lessonCardData.AddOldLessonChoice)
            {
/*                ChosenPriorPackage = chosenStudent.Packages.FirstOrDefault(p => p.PackageNumber == selectedPackage.PackageNumber - 1);
                //LessonCardDataModel lessonCard = new LessonCardDataModel();
                OldPackageTable = tableBuilder(ChosenPriorPackage);
                htmlBuilder.Replace("{oldSlotstable}", OldPackageTable);*/
                htmlBuilder.Replace("{oldSlotstable}", lessonCardData.OldTableHtmlString);
            }
            else
            {
                htmlBuilder.Replace("{oldSlotstable}", "");
                //htmlBuilder.Replace("<div class=\"subtitleBox\" id=\"subtitleBox1\">ALTES PAKET</div>", "");
                htmlBuilder.Replace("<div class=\"subtitleBox\" id=\"subtitleBox1\">NEUES PAKET</div>", "");
            }

            htmlBuilder.Replace("{lesTambahanTable}", "");

            /*NewPackageTable = tableBuilder(ChosenPrintPackage);
            htmlBuilder.Replace("{newSlotstable}", NewPackageTable);*/

            htmlBuilder.Replace("{newSlotstable}", lessonCardData.NewTableHtmlString);

            /*File.WriteAllText(HtmlOutputPath, htmlBuilder.ToString());*/
            GeneratedHtmlContent = htmlBuilder.ToString();
        }

        private StringBuilder FetchHtmlContent()
        {
            string htmlContent = File.ReadAllText(HtmlInputPath);
            //Console.WriteLine(htmlContent);

            StringBuilder htmlBuilder = new StringBuilder(htmlContent);
            return htmlBuilder;
        }
        ////*****************************************************////*****************************************************////************hier weiter
        public static string tableBuilder(Package lessonPackage)
        {
            int tableColumnsAmount = 2;
            int tableRowsAmount = lessonPackage.Lessons.Count / tableColumnsAmount;

            StringBuilder newSlotsTableBuilder = new StringBuilder();

            ///////////////

            newSlotsTableBuilder.Append(" < table class=\"slotsTable\">");


            for (int i = 0; i < tableRowsAmount; i++)
            {
                Console.WriteLine("outer loop round: " + i);
                newSlotsTableBuilder.Append("<tr>");

                for (int j = 0; j < tableColumnsAmount; j++)
                {
                    int index = i * tableColumnsAmount + j;

                    string paraf;

                    if (lessonPackage.Lessons[i].Paraf == true)
                    {
                        paraf = "class=\"SlotsTableParaf\"";
                    }
                    else
                    {
                        paraf = "";
                    }

                    newSlotsTableBuilder.Append($"<td {paraf}>{lessonPackage.Lessons[index].LessonDateString}</td>");
                }

                newSlotsTableBuilder.Append("</tr>");
            }

            newSlotsTableBuilder.Append("</table>");

            ///////////////
            ///


            Console.WriteLine(newSlotsTableBuilder.ToString());
            string newSlotsTable = newSlotsTableBuilder.ToString();

            return newSlotsTable;

        }


        /*
public static void GenerateHtmlContent(StudentData studentData)
{
   //Get Content into stringBuilder
   string htmlInputPath = "C:/Programmieren/bgbahasajermanApp/TimCoreyCourseConsoleApp1/TimCoreyCourseConsoleApp1/ScheduleCard.html";
   string htmlOutputPath = "C:/Programmieren/ProgrammingProjects/PythonProject/src/GoogleAppsAutomation/GoogleDriveNative/Downloads/HtmlSlotsTemp/testing.html";
   string htmlContent = File.ReadAllText(htmlInputPath);
   StringBuilder htmlBuilder = new StringBuilder(htmlContent);

   htmlBuilder = SetHeaderData(studentData, htmlBuilder);

   //Modify Content
   Console.WriteLine("Test successful until here");





   if (studentData.DisplayOldLessonDates == true)
   {


       // string oldSlotsTable = tableBuilder(studentData);
       // Get OldLessonsTable and replace the placeholder with it
       // htmlBuilder.Replace("{oldSlotstable}", $"{oldSlotsTable}");

       if (studentData.DisplayLesTambahan == true)
       {
           // Complete Les Tambahan Method with MYSQ Query
           string lesTambahanBox = lesTambahanBoxBuilder(studentData);
           htmlBuilder.Replace("{lesTambahanTable}", $"{lesTambahanBox}");
       }
       else
       {
           htmlBuilder.Replace("{lesTambahanTable}", "");
       }

   }
   else
   {
       htmlBuilder.Replace("<div class=\"subtitleBox\" id=\"subtitleBox1\">ALTES PAKET</div>", "");
       htmlBuilder.Replace("{oldSlotstable}", "");
       htmlBuilder.Replace("{lesTambahanTable}", "");
   }

   string newSlotsTable = tableBuilder(studentData);

   htmlBuilder.Replace("{newSlotstable}", newSlotsTable);

   //Console.WriteLine(htmlBuilder.ToString());

   // Write Content to File
   File.WriteAllText(htmlOutputPath, htmlBuilder.ToString());
}*/



    }
}
