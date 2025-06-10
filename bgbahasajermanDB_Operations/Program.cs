using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Operations.SetIntensity;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using bgbahasajermanDB_Operations.Models;
using System.ComponentModel;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using static bgbahasajermanDB_Libraries.DateFormatting;
using bgbahasajermanDB_Libraries;
using static bgbahasajermanDB_Operations.PrintTables;
//using bgbahasajermanDB_ReplacementsManager.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using bgbahasajermanDB_ReplacementsManager.Enums;


namespace bgbahasajermanDB_Operations
{
    class Program
    {
        public static Student ChosenStudent { get; set; }
        public static void Main(string[] args)
        {

        ChooseStudent:

            ChosenStudent = new Student();


            //PrintExistingPackageTable(student.Packages[student.MaxPackageNumber-1].Lessons);

            bool changeStudentChoice = YesNoChoice("Do you want to edit a different Student's Package?");
            if (changeStudentChoice)
            {
                goto ChooseStudent;
            }

            bool changePackageChoice = false;

        ChoosePackage:
            if (changePackageChoice)
            {
                Console.WriteLine($"Current Package is: {ChosenStudent.MaxPackageNumber}");
                PrintLessonsList(ChosenStudent.Packages[ChosenStudent.MaxPackageNumber - 1].Lessons);
            }

            int userInput = GetValidNumberInRange(1, ChosenStudent.MaxPackageNumber);
            Console.Clear();
            Console.WriteLine($"Chosen PackageNumber is: {userInput}");
            ChosenStudent.SelectedPackage = ChosenStudent.Packages.FirstOrDefault(p => p.PackageNumber == userInput);

            PrintLessonsList(ChosenStudent.Packages[userInput - 1].Lessons);

            ChooseAction();
            
            bool changeSelectedStudent = false;

            changePackageChoice = YesNoChoice("Do you want to edit a different Package?");
            if (changePackageChoice)
            {
                Console.Clear();
                goto ChoosePackage;
            }

            changeStudentChoice = YesNoChoice("Do you want to edit a different Student's Package?");
            if (changeStudentChoice)
            {
                goto ChooseStudent;
            }
            else
            {
                bool exitChoice = YesNoChoice("Exit application?");
                if (exitChoice)
                {
                    goto Exit;
                }
                else
                {
                    goto ChooseStudent;
                }
            }

        Exit:
            Console.ReadLine();
        }

        public static void DisplayAPackage(string studentName, int lastPackageNumber, List<DateTime> packageDates)
        {
            //Console.Clear();

            Console.WriteLine($"\n{studentName}'s Package {lastPackageNumber} Dates:");
            foreach (var date in packageDates)
            {
                Console.WriteLine(ConvertToCardDateFormat(date));
            }

        }

        public static void ChooseAction()
        //public static bool ChooseAction()
        {
            while (true)
            {
                Console.WriteLine("1. Add Replacement");
                Console.WriteLine("2. Revert Replacement");
                Console.WriteLine("3. Add Package (not yet)");
                Console.WriteLine("4. Print Lesson Card (not yet)");
                Console.WriteLine("5. Delete Package");
                Console.WriteLine("6. Edit Package (not yet)");
                Console.WriteLine("0. Choose Different Package");


                string actionChoice = Console.ReadLine();

                // Parse the user input into the enum
                if (Enum.TryParse<ActionChoices>(actionChoice, out ActionChoices chosenAction))
                {

                    switch (chosenAction)
                    {
                        case ActionChoices.Choose_Different_Package:
                            //Console.WriteLine("Choose different Package was chosen! *****");
                            // ChosenStudent.MaxPackageNumber = GetLastPackageNumber(ChosenStudent.StudentID); // for what? ***
                            return;
                            break;
                        //return false;
                        case ActionChoices.Add_Replacement:
                            //Console.WriteLine("Add Replacement was chosen! *****");
                            ChosenStudent.AddReplacement();
                            break;
                        //return false;
                        case ActionChoices.Revert_Replacement:
                            //Console.WriteLine("Revert Replacement was chosen! *****");
                            ChosenStudent.RevertReplacement();
                            break;
                        //return false;
                        case ActionChoices.Add_Package:
                            //Console.WriteLine("Add Package was chosen! *****");
                            ChosenStudent.AddPackage();
                            break;
                        //return false;
                        case ActionChoices.Print_Lesson_Card:
                            //Console.WriteLine("Print Lesson Card was chosen! *****");
                            ChosenStudent.NewPackage = ChosenStudent.SelectedPackage;
                            ChosenStudent.NewIntensity = ChosenStudent.SelectedPackage.Lessons.Count();
                            if(ChosenStudent.Packages.Count() > 1)
                            {
                                ChosenStudent.OldPackage = ChosenStudent.Packages[ChosenStudent.SelectedPackage.PackageNumber-2];
                            }
                            else
                            {
                                ChosenStudent.OldPackage = ChosenStudent.Packages[ChosenStudent.SelectedPackage.PackageNumber-1];
                            }
                            //ChosenStudent.PrintLessonCard();
                            ChosenStudent.PrintLessonCard(false); // should this be true or does it work at all like this?
                            break;
                            //return false;
                        case ActionChoices.Delete_Package:
                            //Console.WriteLine("Delete Package was chosen! *****");
                            ChosenStudent.DeletePackage();
                            break;
                            //return true;
                        case ActionChoices.Edit_Package:
                            // MethodEditPackage();
                            break;
                            //return false;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }

                return;
            }

        }


    }
}