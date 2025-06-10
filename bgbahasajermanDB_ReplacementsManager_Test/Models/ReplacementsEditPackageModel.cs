using bgbahasajermanDB_Libraries.LessonDataModels;
using bgbahasajermanDB_Operations_Test;
using System.Net.WebSockets;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using static bgbahasajermanDB_Libraries.DateFormatting;
using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace bgbahasajermanDB_ReplacementsManager_Test.Models
{
    public class ReplacementsEditPackageModel : LessonPackageModel
    {
        public List<ReplacementLessonDataModel> LessonDataList = new List<ReplacementLessonDataModel>();
        public DateTime UserInputDate { get; set; }
        public string StudentID { get; set; }
        public string PackageNumber { get; set; }
        public int PackageNumberInt { get; set; }

        public ReplacementsEditPackageModel(string studentID, int packageNumber) : base(studentID, packageNumber)
        {

            LessonDataList = GetNewReplacementLessonData(studentID, packageNumber.ToString());

            PrintReplacementLessonData(LessonDataList);

            StudentID = studentID;
            PackageNumberInt = packageNumber;
            PackageNumber = packageNumber.ToString();
        }

        // 1.Add Replacement
        public void AddReplacement()
        {
            bool confirmReplacementChoice = false;
            
            while (!confirmReplacementChoice)
            {
                ReplacementLessonDataModel selectedLesson = ChooseLessonForRescheduling();
                int lessonID = ChooseNewDateAndSlot(selectedLesson);


                confirmReplacementChoice = ConfirmChoice(ConvertToCardDateFormat(selectedLesson.Date), ConvertToCardDateFormat(UserInputDate));

                if (confirmReplacementChoice)
                {
                    selectedLesson.OriginalLessonDate = selectedLesson.Date;
                    selectedLesson.FormattedOriginalLessonDate = ConvertToCardDateFormat(selectedLesson.Date);
                    selectedLesson.Date = UserInputDate;
                    selectedLesson.FormattedDate = ConvertToCardDateFormat(UserInputDate);

                    selectedLesson.OriginalLessonID = selectedLesson.LessonID;
                    selectedLesson.LessonID = lessonID;

                    SetLessonID(lessonID, selectedLesson.BookingID); // in DB

                    bool addReasonChoice = YesNoChoice("Rescheduling Booking.. Do you want to add a Reason into the DB?");
                    if(addReasonChoice)
                    {
                        string userInputReason = Console.ReadLine();
                        InsertRescheduledBooking(selectedLesson.BookingID.ToString(), selectedLesson.OriginalLessonID.ToString(), userInputReason);
                    }
                    else
                    {
                        InsertRescheduledBooking(selectedLesson.BookingID.ToString(), selectedLesson.OriginalLessonID.ToString());
                    }

                    ReOrderNumberInPackageValues();

                    LessonDataList = GetNewReplacementLessonData(StudentID, PackageNumber); // *******new attempt
                    PrintReplacementLessonData(LessonDataList);
                }
                else
                {
                    Console.WriteLine("Rescheduling needs to be adjusted. (nothing was set yet)");
                }
            }
            
        }
        // 2.Revert Replacement
        public void RevertReplacement()
        {

            bool confirmRevertReplacementChoice = false;

            while (!confirmRevertReplacementChoice)
            {
                ReplacementLessonDataModel selectedLesson = ChooseLessonForRescheduling();

                confirmRevertReplacementChoice = ConfirmChoice(selectedLesson);

                if (confirmRevertReplacementChoice)
                {
                    int originalLessonID = GetOriginalLessonID(selectedLesson.BookingID.ToString());
                    selectedLesson.LessonID = originalLessonID;
                    selectedLesson.OriginalLessonID = null;

                    DeleteRescheduledBooking(selectedLesson.BookingID.ToString());
                    SetLessonID(selectedLesson.LessonID, selectedLesson.BookingID);

                    LessonDataList = GetNewReplacementLessonData(StudentID, PackageNumber); // *******new  
                    PrintReplacementLessonData(LessonDataList);
                }
                else
                {
                    Console.WriteLine("Reversion needs to be adjusted. (nothing was reverted yet)");
                }
            }
        }
        // 3.Edit Package
        // 4.Delete Package
        public void DeletePackage()
        {

            bool confirmDeletionChoice = false;

            while (!confirmDeletionChoice)
            {
                PrintReplacementLessonData(LessonDataList);
                confirmDeletionChoice = YesNoChoice("Do you really want to delete this package?");

                if (confirmDeletionChoice)
                {
                    // DeleteCompletePackage(LessonDataList[0].PackageID.ToString());
                    DeletePackageDatesByPackageID(LessonDataList[0].PackageID.ToString());
                    DeletePackageByPackageID(LessonDataList[0].PackageID.ToString());

                }
                else
                {
                    Console.WriteLine("Deletion needs to be adjusted. (nothing was deleted yet)");
                    return;
                }
            }
        }
        // 5.Print Lesson Card
        public void PrintLessonCard(ReplacementManagerStudent student, ReplacementsEditPackageModel packageData)
        {
            LessonCardHtmlBuilder lessonCardHtmlBuilder = new LessonCardHtmlBuilder(student, packageData);




        }
        // 6.
        // 7.
        // 8.
        // 9.
        // 0. Choose Different Package


        private bool ConfirmChoice(string replacedLessonDateString, string replacementLessonDateString)
        {
            bool replacementChoice = YesNoChoice($"Is Replacement: '{replacedLessonDateString} -> {replacementLessonDateString}' correct?");
            return replacementChoice;
        }
        private bool ConfirmChoice(ReplacementLessonDataModel selectedLesson)
        {
            bool replacementRevertChoice = YesNoChoice($"Set Replacement Lesson: '{selectedLesson.FormattedDate}' back to {selectedLesson.FormattedOriginalLessonDate}?");
            return replacementRevertChoice;
        }

        private void ReOrderNumberInPackageValues()
        {
            // Sort the lessonDataList based on the Date property
            var sortedLessonDataList = LessonDataList.OrderBy(x => x.Date).ToList();

            // Loop through the sorted list and update the NumberInLesson property sequentially
            for (int i = 0; i < sortedLessonDataList.Count; i++)
            {
                sortedLessonDataList[i].NumberInPackage = i + 1;
                SetNumberInPackage(sortedLessonDataList[i].BookingID.ToString(), sortedLessonDataList[i].NumberInPackage.ToString()); // in DB
            }
        }

        private int ChooseNewDateAndSlot(ReplacementLessonDataModel selectedLesson)
        {
            Console.WriteLine($"The Selected Lesson you want to replace is: {selectedLesson.FormattedDate}");

            UserInputDate = ConvertUserInputToDateObject();

            Console.WriteLine($"The Selected Replacement Lesson is: {ConvertToCardDateFormat(UserInputDate)}"); 
            string inputDateString = ConvertDateTimeToDBDateString(UserInputDate);

            DisplayAllSlots();
            int chosenSlotID = GetValidSlotIDInput();

            int lessonID = GetSingleLessonID(inputDateString, chosenSlotID);

            return lessonID;
        }


        public ReplacementLessonDataModel ChooseLessonForRescheduling()
        {

            int maxNumberInPackage = LessonDataList.Max(x => x.NumberInPackage);
            int usersLessonChoiceInt = GetValidNumberInRange(1, maxNumberInPackage);

            ReplacementLessonDataModel selectedLesson = LessonDataList.FirstOrDefault(x => x.NumberInPackage == usersLessonChoiceInt);

            //Console.WriteLine($"The Selected Lesson is: {selectedLesson.FormattedDate}");

            return selectedLesson;

        }

        private void PrintReplacementLessonData(List<ReplacementLessonDataModel> dataList)
        {
            Console.WriteLine("+-----------------+-----------------+-----------------+-----------------+-----------------+-----------------+-----------------+-----------------+");
            Console.WriteLine("| NumberInPackage | PackageID       | BookingID       | LessonID        | Date            | OriLessonID     | OriLessonDate   | Reason          |");
            Console.WriteLine("+-----------------+-----------------+-----------------+-----------------+-----------------+-----------------+-----------------+-----------------+");

            foreach (var data in dataList)
            {
                string formattedLessonDate = ConvertToCardDateFormat(data.Date);
                string originalLessonDate = "";
                if (data.OriginalLessonDate.HasValue)
                {
                    originalLessonDate = ConvertToCardDateFormat(data.OriginalLessonDate.Value);
                }

                Console.WriteLine($"| {data.NumberInPackage,-15} | {data.PackageID,-15} | {data.BookingID,-15} | {data.LessonID,-15} | {formattedLessonDate,-15} | {data.OriginalLessonID,-15} | {originalLessonDate,-15} | {data.Reason,-15} |");
            }

            Console.WriteLine("+-----------------+-----------------+-----------------+-----------------+-----------------+-----------------+-----------------+-----------------+");
        }
    }
}
