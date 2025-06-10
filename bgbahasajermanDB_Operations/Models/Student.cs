using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using bgbahasajermanDB_Libraries.LessonDataModels;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using static bgbahasajermanDB_Libraries.DateFormatting;
using static bgbahasajermanDB_Operations.PrintTables;
using static bgbahasajermanDB_Operations.ParafInsert;
using System.IO;

namespace bgbahasajermanDB_Operations.Models
{
    public class Student
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string Title { get; set; }
        public string StudyStatus { get; set; }
        public string Level { get; set; }   //***************************NEW ! write into Flowchart
        public string Currency { get; set; }   //***************************NEW ! write into Flowchart
        public string DiscountID { get; set; }  //***************************NEW ! write into Flowchart
        public int DiscountInt { get; set; }  //***************************NEW ! write into Flowchart
        public bool IsCurrentlyBookingSlot { get; set; }
        public bool HasLessonBookings { get; set; }
        public int MaxPackageNumber { get; set; }
        public bool IsNewStudent { get; set; } = false;
        public bool DoublePayment { get; set; } //
        public int OldIntensity { get; set; }
        public int NewIntensity { get; set; }
        public Package OldPackage { get; set; }///////////////// delete?
        public Package NewPackage { get; set; }///////////////// update to suit new package model??
        public Package NewPackage2 { get; set; } // could this be just a repetition of the procedure making/converting a NewPackage into an OldPackage class object?
        public Package SelectedPackage { get; set; }
        public DateTime UserInputDate { get; set; }
        public List<Package> Packages { get; set; } = new List<Package>();
        public LessonCardDataModel LessonCardData { get; set; }




        public Student()
        {
            Console.Clear();
            //Console.WriteLine("\nStudent Object Created\n");

            // Set <Name>, <Level>, <StudentID> ?? ***
            SetStudentName();
            StudentID = GetStudentIdByName(StudentName);


            #region IF THE STUDENT IS NEW:

            if (IsNewStudent)
            {
                // Title has been set in the SetStudentName method
                CheckAndUpdateLevel();


                // Setup Pricing Details
                bool SetCurrencyAndDiscountChoice = YesNoChoice("Continue with Currency and Discount standard values? (IDR, no discount)");
                if (SetCurrencyAndDiscountChoice)
                {
                    GetAndSetStudentPricing();
                }
                else
                {
                    SetCurrencyAndDiscount();
                    //*** not yet implemented
                    GetAndSetStudentPricing();
                }

                // Double Check whether student really is a new student
                IsCurrentlyBookingSlot = CheckIfCurrentlyBooking(StudentID);
                HasLessonBookings = CheckIfLessonBookingsExist(StudentID);
                bool doubleCheckIsNewStudent = SetIsNewStudent();
                if (!doubleCheckIsNewStudent) { throw new DataMisalignedException(); }

                Console.WriteLine("************");
                Console.WriteLine("Choosing Add Package Automatically");
                Console.WriteLine("************");

                StudyStatus = "booking";
                SetStudyStatus(StudyStatus, StudentID);

                MaxPackageNumber = 1;
                OldIntensity = 0; //??

                CreateFirstPackage();
            }
            #endregion

            #region IF THE STUDENT IS NOT NEW:

            else if (!IsNewStudent)
            {
                Title = GetTitle(StudentID);
                Level = GetLevel(StudentID);

                // Double Check whether student really is not a student
                IsCurrentlyBookingSlot = CheckIfCurrentlyBooking(StudentID);
                HasLessonBookings = CheckIfLessonBookingsExist(StudentID);
                bool doubleCheckIsNewStudent = SetIsNewStudent();
                if (doubleCheckIsNewStudent) { throw new DataMisalignedException(); }

                StudyStatus = GetStudyStatus(StudentID);
                MaxPackageNumber = GetLastPackageNumber(StudentID);
                OldIntensity = GetCurrentIntensity(StudentID) * 4;
                
                // Create Complete list of Packages of existing lessons
                for (int i = 0; i < MaxPackageNumber; i++)
                {
                    Package package = new ExistingPackage(StudentID, i + 1);
                    Packages.Add(package);

                    if (package.PackageNumber == MaxPackageNumber)
                    {
                        Console.Clear();
                        PrintNameStatusAndLastPackage();
                        //PrintExistingPackageTable(package.Lessons);
                        PrintLessonsList(package.Lessons);
                        OldPackage = Packages[MaxPackageNumber - 1];
                    }
                }

            }

            #endregion

            /*                                                                                  DoublePayment?
             *                                                                                  NewPackage2?
             */
        }
        public void SetStudentName()
        {
            List<string> students = GetAllStudentNames();

            while (true)
            {
                string studentName = ValidateStudentName("Enter student who's Package should be updated: ");

                if (students.Contains(studentName))
                {
                    StudentName = studentName;
                    //StudentID = GetStudentIdByName(StudentName);
                    //CheckAndUpdateLevel();
                }
                else
                {
                    Console.WriteLine($"{studentName} can't be found in the student list\n");
                    bool addNewStudentChoice = YesNoChoice("Add New Student?");
                    if (addNewStudentChoice)
                    {
                        string newStudentName = AddNewStudentToDatabase();

                        students = GetAllStudentNames();

                        if (students.Contains(newStudentName))
                        {
                            StudentName = newStudentName;
                            IsNewStudent = true;
                            //StudentID = GetStudentIdByName(StudentName);
                            //CheckAndUpdateLevel();
                            //Console.WriteLine("Loop Ending break");
                            //break;
                        }
                        else
                        {
                            throw new Exception($"Creating New Student {newStudentName} Failed.");
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                break;
            }
        }
        private void SetCurrencyAndDiscount()
        {
            throw new NotImplementedException();
        }
        private void CheckAndUpdateLevel()
        {
            //maybe later add more dynamic level change with level change logging information
            //to calculate the real price of a package with a level change in it
            bool levelChangeChoice = true;

            if (Level != null)
            {
                Console.WriteLine($"\nStudent's level is: {Level}");
                levelChangeChoice = YesNoChoice("Does the level have to change?");
            }

            if (levelChangeChoice)
            {
                string levelInput = GetValidLevelInput();
                Level = levelInput;

                UpdateLevel(StudentID, levelInput);

            }
        }
        private bool SetIsNewStudent()
        {
            bool doubleCheckIsNewStudent = false;

            if (IsCurrentlyBookingSlot || HasLessonBookings)
            {
                doubleCheckIsNewStudent = false;
            }
            else if (IsCurrentlyBookingSlot == false && HasLessonBookings == false)
            {
                doubleCheckIsNewStudent = true;
            }
            else
            {

            }

            return doubleCheckIsNewStudent;
        }
        public void GetAndSetStudentPricing()
        {
            // Call the GetStudentPricing method to retrieve the values
            (string currency, string discountID) = GetStudentPricing(StudentID);

            // Assign the retrieved values to the properties
            Currency = currency;
            DiscountID = discountID;

            if (DiscountID == "")
            {
                DiscountInt = 0;
            }
            else
            {
                DiscountInt = GetDiscountAmount(DiscountID);
            }
        }
        public void SetStudentIntensity()
        {
            Console.WriteLine("Set Intensity:");

            bool userIntensityChoice = false;

            if (!IsNewStudent && OldIntensity > 0)
            {
                userIntensityChoice = YesNoChoice($"Do you want to renew the package with an intensity of {OldIntensity}?");
            }

            if (userIntensityChoice)
            {
                DisplayBookedSlots(StudentID);
                NewIntensity = OldIntensity;

                bool changeSlotChoice = YesNoChoice("Do you want to change the slot(s)?");
                if (changeSlotChoice)
                {
                    Console.WriteLine("Delete and then Add. Don't change intensity/slot amount!");
                    DeleteBooking(StudentID);
                    InsertBooking(StudentID);
                }

                // unchanged intensity value
            }
            else
            {
                // change intensity value
                NewIntensity = CheckAndSetIntensity();
            }

        }
        public int CheckAndSetIntensity()
        {

            int intensity;

            while (true)
            {
                intensity = GetValidIntensityInput();

                int currentLessonsPerWeek = GetCurrentIntensity(StudentID);
                int desiredLessonsPerWeek = intensity / 4;

                DisplayBookedSlots(StudentID);

                if (currentLessonsPerWeek == desiredLessonsPerWeek)
                {
                    bool userSlotsValidaitonChoice = YesNoChoice("Are the chosen slots correct?");

                    if (userSlotsValidaitonChoice)
                    {
                        Console.WriteLine($"Chosen intensity is {intensity}");
                        break;
                    }
                    else
                    {
                        // CHANGE bookedslots and try the loop again
                        DisplayBookedSlots(StudentID);
                        Console.WriteLine($"Chosen intensity of {intensity} is wrong, how to continue from here?");

                        Console.WriteLine("Add or delete slots?");
                        bool userAddSlotsChoice = YesNoChoice("Do you want to add slots?");
                        bool userDeleteSlotsChoice = YesNoChoice("Do you want to delete slots?");

                        if (userAddSlotsChoice)
                        {
                            InsertBooking(StudentID);
                        }

                        if (userDeleteSlotsChoice)
                        {
                            DeleteBooking(StudentID);
                        }

                        continue;

                    }
                }
                else
                {
                    Console.WriteLine("Current slots amount: " + currentLessonsPerWeek + " - Desired slots amount: " + desiredLessonsPerWeek);

                    if (currentLessonsPerWeek < desiredLessonsPerWeek)
                    {
                        // add a slotbooking for a student
                        InsertBooking(StudentID);
                    }
                    else if (currentLessonsPerWeek > desiredLessonsPerWeek)
                    {
                        // delete a slotbooking(s) for a student
                        DeleteBooking(StudentID);
                    }

                    continue;
                }

            }

            return intensity;
        }
        public void CreateFirstPackage()
        {
            SetStudentIntensity();

            new FirstPackage(this);
            //var newPackage = new FirstPackage(this);

            //Packages.Add(newPackage); //add to Packages list
            //NewPackage = newPackage; //make it easier to edit

            bool continueToInsertDataChoice = InsertPackageIntoDB();
            PrintOrNot(continueToInsertDataChoice);
        }
        public void AddPackage()
        {
            CheckAndUpdateLevel();
            GetAndSetStudentPricing();
            SetStudentIntensity();

            new NewPackage(this);
            //NewPackage = new NewPackage(this);

            //Packages.Add(NewPackage); //add to Packages list


            bool continueToInsertDataChoice = InsertPackageIntoDB();
            PrintOrNot(continueToInsertDataChoice);
        }
        public bool InsertPackageIntoDB()
        {
            bool continueToInsertDataChoice = YesNoChoice("Do you want to proceed to insert the new package into the Bg.DataBase?");

            if (continueToInsertDataChoice)
            {
                InsertPackageLessonbookings();
                InsertLessonPackage();
                SetLessonsPackageIDs();
                InsertLessonPayments();
                AddAttendances();
                Console.WriteLine("Successfully Inserted Package.");
            }
            else
            {

            }

            return continueToInsertDataChoice;
        }
        public void PrintOrNot(bool continueToInsertDataChoice)
        {
            bool printCardChoice = YesNoChoice("Proceed to Create LessonCard?");
            if (printCardChoice)
            {
                PrintLessonCard(continueToInsertDataChoice);
            }
            else
            {
                Console.WriteLine("Skipping print card..");
            }
        }
        private void saveHtmlFile(LessonCardHtmlBuilder lessonCardHtmlBuilder, bool continueToInsertDataChoice)
        {
            DateTime now = DateTime.Now;
            string formattedDateTime = now.ToString("ddMMyyyy_HHmmss");
        
            //string htmlOutputPath = $"C:\\Programmieren\\bgbahasajermanApp_Combo\\bgbahasajermanDB_Operations\\ScheduleCardTesting\\{formattedDateTime}_{StudentName}_Zahlungsbestätigung.html";
            string htmlOutputPath = $"C:\\Programmieren\\ProgrammingProjects\\PythonProject\\src\\GoogleAppsAutomation\\GoogleDriveNative\\Downloads\\HtmlSlotsTemp\\{formattedDateTime}_{StudentName}_Zahlungsbestätigung_raw.html";
            string htmlFileName = $"{formattedDateTime}_{StudentName}_Zahlungsbestätigung_raw.html";
            string jpgFileName = $"{formattedDateTime}_{StudentName}_Zahlungsbestätigung_raw.jpg";
            string jpgOutputPath = $"C:\\Programmieren\\ProgrammingProjects\\PythonProject\\src\\GoogleAppsAutomation\\GoogleDriveNative\\Downloads\\SlotsPictures\\{jpgFileName}";

            try
            {
                // Write the HTML content to the file
                File.WriteAllText(htmlOutputPath, lessonCardHtmlBuilder.GeneratedHtmlContent);
                Console.WriteLine("HTML file saved successfully!");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during file write
                Console.WriteLine($"Error saving HTML file: {ex.Message}");
            }

            bool insertParafChoice = false;

            while (!insertParafChoice)
            {
                insertParafChoice = YesNoChoice("Wait for it...  And now: Insert Signature Parafs?");

                if (insertParafChoice)
                {
                    InsertParafs(jpgOutputPath, jpgFileName, OldPackage.Intensity, OldPackage.Lessons.Count(x => x.Paraf == true), NewPackage.Intensity, NewPackage.Lessons.Count(x => x.Paraf == true), LessonCardData, this.NewPackage.WeekdaysAndTimes, continueToInsertDataChoice);
                }
                else
                {
                    Console.WriteLine("Waiting for the right timing...");
                }
            }
        }

    private void AddAttendances()
        {
            foreach (Lesson lesson in NewPackage.Lessons)
            {
                string attendedAlready = lesson.Paraf ? "1" : "0";
                if (attendedAlready == "1")
                {
                    InsertAttendance(lesson.BookingID.ToString(), attendedAlready);
                }
            }
        }
        private void InsertLessonPayments()
        {
            foreach (Lesson lesson in NewPackage.Lessons)
            {

                InsertPayment(StudentID, NewPackage.PaymentDateTimeString, lesson.ProductID.ToString(), NewPackage.PackageID.ToString());
            }
        }

        //private static void deleteNewPackageFromDB(Student student)
        //{
        //    bool deleteLastAddedPackageChoice = YesNoChoice("Delete this Package?");
        //    if (deleteLastAddedPackageChoice)
        //    {
        //        //
        //    }
        //}

        private void SetLessonsPackageIDs()
        {
            foreach (Lesson lesson in NewPackage.Lessons)
            {
                SetPackageIDAndNumberInPackage(NewPackage.PackageID.ToString(), lesson.NumberInPackage.ToString(), lesson.BookingID.ToString());
            }
        }
        private void InsertPackageLessonbookings()
        {
            Console.WriteLine();
            Console.WriteLine("Inserting Lessonbookings..");
            foreach (Lesson lesson in NewPackage.Lessons)
            {
                InsertLessonBooking(StudentID, lesson.LessonID.ToString(), lesson.ProductID.ToString());
                //Console.WriteLine("Lesson Added: " + lesson.LessonDateString);

                int lastInsertedBookingID = GetLastInsertedBookingID();
                NewPackage.BookingIDs.Add(lastInsertedBookingID);
                lesson.BookingID = lastInsertedBookingID;
            }
        }
        private void InsertLessonPackage()
        {
            //convert bool to string for query
            string packageCompleted = NewPackage.PackageCompleted ? "1" : "0";

            InsertPackage
                (
                    StudentID,
                    NewPackage.PackageNumber.ToString(),
                    packageCompleted,
                    NewPackage.Intensity.ToString(), // = lessonsAmount
                    NewPackage.OutstandingLessons.ToString(),
                    NewPackage.CompletedLessons.ToString()
                );

            int lastInsertedPackageID = GetLastInsertedPackageID();

            NewPackage.PackageID = lastInsertedPackageID;

            foreach (Lesson lesson in NewPackage.Lessons)
            {
                lesson.PackageID = lastInsertedPackageID;
            }

        }
        private void PrintNameStatusAndLastPackage()
        {
            Console.WriteLine($"{StudentName}'s current status is {StudyStatus}");
            Console.WriteLine($"Last Package ({MaxPackageNumber}) Lesson Dates:\n");

        }
        private void PrintNameStatusAndLastPackage(string studentName, string studyStatus, int maxPackageNumber)
        {
            Console.WriteLine($"{studentName}'s current status is {studyStatus}");
            Console.WriteLine($"Last Package ({maxPackageNumber}) Lesson Dates:\n");

        }
        public string AddNewStudentToDatabase()
        {
            string newStudentName = ValidateStudentName("\nRe-enter new Student's name:");
            int newStudentNumber = GetStudentNumberPlusOne();
            string title = "";

            bool addTitleChoice = YesNoChoice("\nAdd Title (Herr/Frau)?");
            if (addTitleChoice)
            {

                Console.WriteLine("Pick a Title:\n1. for: 'Herr'\n2. for: 'Frau'");
                int numberChoice = NumericChoice();
                if (numberChoice == 1)
                {
                    title = "Herr";
                }
                else
                {
                    title = "Frau";
                }

                InsertNewStudent(newStudentNumber, newStudentName, title);
                Title = title;
            }
            else
            {
                InsertNewStudent(newStudentNumber, newStudentName, title);
                Title = title;
            }


            return newStudentName;
        }
        public static string ValidateStudentName(string EnterStudentMessage)
        {
            if (EnterStudentMessage == null)
            {
                EnterStudentMessage = "Enter Student Name:";
            }

            string newStudentName;

            while (true)
            {
                Console.WriteLine($"{EnterStudentMessage}");

                // Get user input
                newStudentName = Console.ReadLine();

                // Check if the input is not empty
                if (string.IsNullOrWhiteSpace(newStudentName))
                {
                    Console.WriteLine("Error: Name cannot be empty. Please try again.");
                    continue;
                }

                // Check if the input is not just one character
                if (newStudentName.Length == 1)
                {
                    Console.WriteLine("Error: Name must be longer than one character. Please try again.");
                    continue;
                }

                // Check if the input contains only alphabetical characters, spaces, and dots
                if (!newStudentName.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '.'))
                {
                    Console.WriteLine("Error: Name can only contain alphabetical characters, spaces, and dots. Please try again.");
                    continue;
                }

                // Check if the first letter of each word is capital
                if (!newStudentName.Split(' ').All(word => char.IsUpper(word[0])))
                {
                    Console.WriteLine("Error: The first letter of each word must be capitalized. Please try again.");
                    continue;
                }

                // All conditions met, return the processed name
                return newStudentName;
            }
        }

        // 1.Add Replacement
        public void AddReplacement()
        {
            bool confirmReplacementChoice = false;

            while (!confirmReplacementChoice)
            {
                //Lesson selectedLesson = ChooseLessonForRescheduling();
                ChooseLessonForModification();
                int lessonID = ChooseNewDateAndSlot(SelectedPackage.SelectedLesson);

                string userInputDateString = ConvertToCardDateFormat(UserInputDate);
                confirmReplacementChoice = ConfirmChoice(ConvertToCardDateFormat(SelectedPackage.SelectedLesson.LessonDateTime), userInputDateString);

                if (confirmReplacementChoice)
                {
                    SelectedPackage.SelectedLesson.OriginalLessonDate = SelectedPackage.SelectedLesson.LessonDateTime;
                    SelectedPackage.SelectedLesson.OriginalLessonDateString = SelectedPackage.SelectedLesson.LessonDateString;
                    //selectedLesson.FormattedOriginalLessonDate = ConvertToCardDateFormat(selectedLesson.Date);
                    SelectedPackage.SelectedLesson.LessonDateTime = UserInputDate;
                    SelectedPackage.SelectedLesson.LessonDateString = userInputDateString;

                    SelectedPackage.SelectedLesson.OriginalLessonID = SelectedPackage.SelectedLesson.LessonID;
                    SelectedPackage.SelectedLesson.LessonID = lessonID;

                    SetLessonID(lessonID, SelectedPackage.SelectedLesson.BookingID); // in DB

                    bool addReasonChoice = YesNoChoice("Rescheduling Booking.. Do you want to add a Reason into the DB?");
                    if (addReasonChoice)
                    {
                        string userInputReason = Console.ReadLine();
                        InsertRescheduledBooking(SelectedPackage.SelectedLesson.BookingID.ToString(), SelectedPackage.SelectedLesson.OriginalLessonID.ToString(), userInputReason);
                    }
                    else
                    {
                        InsertRescheduledBooking(SelectedPackage.SelectedLesson.BookingID.ToString(), SelectedPackage.SelectedLesson.OriginalLessonID.ToString());
                    }

                    SelectedPackage = new ExistingPackage(StudentID, SelectedPackage.PackageNumber); // *******new attempt
                    ReOrderNumberInPackageValues();
                    /*
                    //SelectedPackage = GetNewReplacementLessonData(StudentID, PackageNumber); // *******new attempt
                    //SelectedPackage = new ExistingPackage(StudentID, SelectedPackage.PackageNumber); // *******new attempt
                    //Package modifiedPackage = this.Packages.Find(x => x.PackageNumber == SelectedPackage.PackageNumber);

                    //if (SelectedPackage.Lessons.Any(x => x.LessonID == selectedLesson.LessonID))
                    //{
                    //    Console.WriteLine("Package in StudentObject was successfully updated.");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Package in StudentObject was NOT updated!!");
                    //}
                    */

                    PrintLessonsList(SelectedPackage.Lessons);

                    return;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Rescheduling needs to be adjusted. (nothing was set yet)\n");
                    PrintLessonsList(SelectedPackage.Lessons);
                }
            }

        }

        // 2.Revert Replacement
        public void RevertReplacement()
        {

            bool confirmRevertReplacementChoice = false;

            while (!confirmRevertReplacementChoice)
            {
                bool lessonHasReplacement = false;
                while (!lessonHasReplacement)
                {
                    ChooseLessonForModification(false);

                    if (SelectedPackage.SelectedLesson.OriginalLessonID != 0)
                    {
                        lessonHasReplacement = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Lesson Choice! Lesson is not a Replacement lesson!");
                    }
                }


                //Lesson selectedLesson = ChooseLessonForRescheduling();
                //SelectedPackage.SelectedLesson = selectedLesson;

                confirmRevertReplacementChoice = ConfirmChoice(SelectedPackage.SelectedLesson);

                if (confirmRevertReplacementChoice)
                {
                    //int originalLessonID = GetOriginalLessonID(selectedLesson.BookingID.ToString()); // is this stored already or do I have to fetch all this manually?
                    int originalLessonID = SelectedPackage.SelectedLesson.OriginalLessonID; // is this stored already or do I have to fetch all this manually?
                    SelectedPackage.SelectedLesson.LessonID = originalLessonID;
                    SelectedPackage.SelectedLesson.OriginalLessonID = 0;

                    DeleteRescheduledBooking(SelectedPackage.SelectedLesson.BookingID.ToString());
                    SetLessonID(SelectedPackage.SelectedLesson.LessonID, SelectedPackage.SelectedLesson.BookingID);

                    Packages[SelectedPackage.PackageNumber - 1] = new ExistingPackage(StudentID, SelectedPackage.PackageNumber); // *******new attempt
                    SelectedPackage = Packages[SelectedPackage.PackageNumber - 1]; // do I need this?

                    ReOrderNumberInPackageValues();
                    PrintLessonsList(SelectedPackage.Lessons);


                    //this.Lessons = GetAndSetExistingLessonsData(StudentID, SelectedPackage.PackageNumber); // *******new  
                    //PrintReplacementLessonData(LessonDataList);

                    //*********** Set LessonData From Database to populate a new Lesson Object to Replace the Lesson Object with the selected Package Number, then print that object!
                    //***************************************************************************************************************************************

                    return;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Reversion needs to be adjusted. (nothing was reverted yet\n)");
                    PrintLessonsList(SelectedPackage.Lessons);

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
                PrintLessonsList(SelectedPackage.Lessons);
                confirmDeletionChoice = YesNoChoice("Do you really want to delete this package?");

                if (confirmDeletionChoice)
                {
                    // DeleteCompletePackage(this.SelectedPackage.PackageID);
                    DeletePackageDatesByPackageID(SelectedPackage.PackageID.ToString());
                    DeletePackageByPackageID(SelectedPackage.PackageID.ToString());

                    // Delete Package from Packages List
                    var packageToDelete = Packages.Find(x => x.PackageID == SelectedPackage.PackageID);

                    if (packageToDelete != null)
                    {
                        Packages.Remove(packageToDelete);
                        MaxPackageNumber = Packages.Max(x => x.PackageNumber);
                        Console.WriteLine("Package was successfully removed from StudentObject");
                    }
                    else
                    {
                        Console.WriteLine("Package was NOT removed from StudentObject!!");
                    }


                }
                else
                {
                    Console.WriteLine("Deletion needs to be adjusted. (nothing was deleted yet)");
                    return;
                }
            }
        }
        // 5.Print Lesson Card
        //public void PrintLessonCard(Student student, ReplacementsEditPackageModel packageData)
        public void PrintLessonCard(bool continueToInsertDataChoice)
        {
            LessonCardData = new LessonCardDataModel(this);
            LessonCardHtmlBuilder lessonCardHtmlBuilder = new LessonCardHtmlBuilder(LessonCardData);

            saveHtmlFile(lessonCardHtmlBuilder, continueToInsertDataChoice); // where to do this?
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
        private bool ConfirmChoice(Lesson selectedLesson)
        {
            bool replacementRevertChoice = YesNoChoice($"Set Replacement Lesson: '{selectedLesson.LessonDateString}' back to {selectedLesson.OriginalLessonDateString}?");
            return replacementRevertChoice;
        }
        private void ReOrderNumberInPackageValues()
        {
            // Sort the lessonDataList based on the Date property
            SelectedPackage.Lessons = SelectedPackage.Lessons.OrderBy(x => x.LessonDateTime).ToList();

            // Loop through the sorted list and update the NumberInLesson property sequentially
            for (int i = 0; i < SelectedPackage.Lessons.Count; i++)
            {
                SelectedPackage.Lessons[i].NumberInPackage = i + 1; //***********************  CHECK ALL THIS AGAIN!!
                SetNumberInPackage(SelectedPackage.Lessons[i].BookingID.ToString(), SelectedPackage.Lessons[i].NumberInPackage.ToString()); // in DB
            }
        }
        private int ChooseNewDateAndSlot(Lesson selectedLesson)
        {
            Console.WriteLine($"The Selected Lesson you want to replace is: {selectedLesson.LessonDateString}");

            UserInputDate = ConvertUserInputToDateObject();

            Console.WriteLine($"The Selected Replacement Lesson is {UserInputDate.DayOfWeek}: {ConvertToCardDateFormat(UserInputDate)}");
            string inputDateString = ConvertDateTimeToDBDateString(UserInputDate);

            DisplayAllSlots();
            //int chosenSlotID = GetValidSlotIDInput();
            int chosenSlotID = GetValidSlotIDForDateInput(UserInputDate);

            int lessonID = GetSingleLessonID(inputDateString, chosenSlotID);

            return lessonID;
        }
        //public Lesson ChooseLessonForRescheduling() ******* older version .. new version: ChooseLessonForModification()
        //{

        //    int maxNumberInPackage = this.SelectedPackage.Lessons.Max(x => x.NumberInPackage);
        //    int usersLessonChoiceInt = GetValidNumberInRange(1, maxNumberInPackage);

        //    Lesson selectedLesson = this.SelectedPackage.Lessons.FirstOrDefault(x => x.NumberInPackage == usersLessonChoiceInt);

        //    //Console.WriteLine($"The Selected Lesson is: {selectedLesson.FormattedDate}");

        //    return selectedLesson;

        //}
        public void ChooseLessonForModification(bool print)
        {
            int maxNumberInPackage = SelectedPackage.Lessons.Max(x => x.NumberInPackage);
            int usersLessonChoiceInt = GetValidNumberInRange(1, maxNumberInPackage);

            SelectedPackage.SelectedLesson = SelectedPackage.Lessons.FirstOrDefault(x => x.NumberInPackage == usersLessonChoiceInt);

            //if (print)
            //{
            //    Console.WriteLine($"The Selected Lesson is: {SelectedPackage.SelectedLesson.LessonDateString}");
            //}

        }
        public void ChooseLessonForModification()
        {
            int maxNumberInPackage = SelectedPackage.Lessons.Max(x => x.NumberInPackage);
            int usersLessonChoiceInt = GetValidNumberInRange(1, maxNumberInPackage);

            SelectedPackage.SelectedLesson = SelectedPackage.Lessons.FirstOrDefault(x => x.NumberInPackage == usersLessonChoiceInt);

            Console.WriteLine($"The Selected Lesson is: {SelectedPackage.SelectedLesson.LessonDateString}");

        }

    }
}
