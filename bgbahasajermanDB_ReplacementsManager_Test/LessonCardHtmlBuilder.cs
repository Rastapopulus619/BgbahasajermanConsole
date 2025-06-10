//using bgbahasajermanDB_Operations_Test.Models;
using bgbahasajermanDB_Libraries.LessonDataModels;
using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using static bgbahasajermanDB_Libraries.DateFormatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bgbahasajermanDB_ReplacementsManager_Test.Models;

namespace bgbahasajermanDB_Operations_Test
{
    public class LessonCardHtmlBuilder
    {
        public string HtmlInputPath = "C:/Programmieren/bgbahasajermanApp/TimCoreyCourseConsoleApp1/TimCoreyCourseConsoleApp1/ScheduleCard.html";
        public string HtmlOutputPath = "C:/Programmieren/ProgrammingProjects/PythonProject/src/GoogleAppsAutomation/GoogleDriveNative/Downloads/HtmlSlotsTemp/testing.html";
        public ReplacementManagerStudent Student { get; set; }
        public ReplacementsEditPackageModel ChosenPriorPackage { get; set; }
        public ReplacementsEditPackageModel ChosenPrintPackage { get; set; }
        public string OldPackageTable { get; set; }
        public string NewPackageTable { get; set; }
        public string ReplacementLessons { get; set; }
        public string SeperationBar { get; set; } = "< div class=\"subtitleBox\" id=\"subtitleBox1\"></div>";
        public LessonCardHtmlBuilder(ReplacementManagerStudent student, ReplacementsEditPackageModel replacementLessonData) 
        {
            Student = student;
            ChosenPrintPackage = replacementLessonData;

            StringBuilder htmlBuilder = FetchHtmlContent();

            bool printPriorPackage = YesNoChoice("Add Prior Package to LessonCard?");

            if(printPriorPackage)
            {
                ChosenPriorPackage = new ReplacementsEditPackageModel(Student.StudentID, ChosenPrintPackage.PackageNumberInt -1);
                //LessonCardDataModel lessonCard = new LessonCardDataModel();
                OldPackageTable = tableBuilder(ChosenPriorPackage);
                htmlBuilder.Replace("{oldSlotstable}", OldPackageTable);
            }
            else
            {
                Console.WriteLine("Without Old Table not yet implemented");
                htmlBuilder.Replace("{oldSlotstable}", "");
            }

            NewPackageTable = tableBuilder(ChosenPrintPackage);
            htmlBuilder.Replace("{newSlotstable}", NewPackageTable);

            File.WriteAllText(HtmlOutputPath, htmlBuilder.ToString());
        }

        private StringBuilder FetchHtmlContent()
        {
            string htmlContent = File.ReadAllText(HtmlInputPath);
            //Console.WriteLine(htmlContent);

            StringBuilder htmlBuilder = new StringBuilder(htmlContent);
            return htmlBuilder;
        }
        ////*****************************************************////*****************************************************////************hier weiter
        public static string tableBuilder(ReplacementsEditPackageModel lessonPackage)
        {
            int tableColumnsAmount = 2;
            int tableRowsAmount = lessonPackage.LessonDataList.Count / tableColumnsAmount;

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

                    if (lessonPackage.LessonDataList[i].Paraf == true)
                    {
                        paraf = "class=\"SlotsTableParaf\"";
                    }
                    else
                    {
                        paraf = "";
                    }

                    newSlotsTableBuilder.Append($"<td {paraf}>{lessonPackage.LessonDataList[index].FormattedDate}</td>");
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
