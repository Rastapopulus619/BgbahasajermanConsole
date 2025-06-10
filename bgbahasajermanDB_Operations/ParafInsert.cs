using bgbahasajermanDB_Operations.Models;
using System.Drawing;

namespace bgbahasajermanDB_Operations
{
    public static class ParafInsert
    {
        public static void InsertParafs(string jpgOutputPath, string jpgFileName, int oldPackageIntensity, int oldPackageParafCounter, int newPackageIntensity, int newPackageParafCounter, LessonCardDataModel lessonCardData, SortedDictionary<int, string[]> weekdaysAndTimes, bool continueToInsertDataChoice)
        {
            {
                string pngFilePath = @"C:\Programmieren\bgbahasajermanApp_Combo\bgbahasajermanDB_Operations\Png\Paraf.png";

                Bitmap jpgImage = new Bitmap(jpgOutputPath);
                Bitmap pngImage = new Bitmap(pngFilePath);

                #region Position Factors
                // Define the position where the PNG image will be inserted into the JPEG image

                int x = 1690; // Column 1
                int x2 = 3674; // Column 2
                int y = 1660; // Intensität 4 Y position
                //int yR1 = 1660; // Intensität 4 Y position
                //int yR2 = 2290; // Intensität 4 Y position
                //int yR3 = 2920; // Intensität 4 Y position
                //int yR4 = 3550; // Intensität 4 Y position
                int reihenDifferenz = 630; // Differenz zwischen den Reihen
                int slotVerschiebung = 158; // Verschiebung nach unten zwischen Intensität 4 und Intensität 8 ** NUR WENN DIE SLOTS ANDERS SIND!!

                //int Y_Gesamtverschiebung = lessonCardData.Weekdays.Count() * 158;

                #endregion
                #region Positions Loop Insertion

                //int headerGesamtverschiebung = (lessonCardData.Weekdays.Count() - 1) * slotVerschiebung;


                int headerGesamtverschiebung;

                //switch (lessonCardData.Weekdays.Count())
                switch (weekdaysAndTimes.Count())
                {
                    case 1:
                        headerGesamtverschiebung = 0;
                        break;
                    case 2:
                        headerGesamtverschiebung = 160; // or any other value for a single weekday
                        break;
                    case 3:
                        headerGesamtverschiebung = 210; // or any other value for two weekdays
                        break;
                    case 4:
                        headerGesamtverschiebung = 420; // or any other value for three weekdays
                        break;
                    case 5:
                        headerGesamtverschiebung = 3 * slotVerschiebung; // or any other value for four weekdays
                        break;
                    default:
                        headerGesamtverschiebung = (lessonCardData.Weekdays.Count() - 1) * slotVerschiebung; // for any other count of weekdays
                        break;
                }


                // for LessonCards that are printed in advance:
                if (!continueToInsertDataChoice)
                {
                    oldPackageParafCounter = oldPackageIntensity;
                    newPackageParafCounter = 0;
                }


                int newPackageGesamtverschiebung = 270;


                if (lessonCardData.AddOldLessonChoice)
                {
                    // Insert the PNG image into the JPEG image
                    using (Graphics graphics = Graphics.FromImage(jpgImage))
                    {
                        int reihenGesamtverschiebung = 0;

                        for (int i = 1; i <= oldPackageParafCounter; i++) //CAREFUL! [i] starts on 1!! **********
                        {
                            //if (i % 2 == 0 || i == 0)


                            if (i % 2 != 0) // run if odd number
                            {
                                if (i > 2 && (i - 2) % 2 != 0) // run if the previous number (i - 2) is odd
                                {
                                    reihenGesamtverschiebung = reihenGesamtverschiebung + reihenDifferenz;
                                }

                                graphics.DrawImage(pngImage, x, y + reihenGesamtverschiebung + headerGesamtverschiebung, pngImage.Width, pngImage.Height); //insert into column 1
                            }
                            else // run if even number
                            {
                                graphics.DrawImage(pngImage, x2, y + reihenGesamtverschiebung + headerGesamtverschiebung, pngImage.Width, pngImage.Height); //insert into column 2
                            }
                        }

                        reihenGesamtverschiebung = reihenGesamtverschiebung + reihenDifferenz + newPackageGesamtverschiebung;



                        for (int i = 1; i <= newPackageParafCounter; i++) //CAREFUL! [i] starts on 1!! **********
                        {
                            //if (i % 2 == 0 || i == 0)


                            if (i % 2 != 0) // run if odd number
                            {
                                if (i > 2 && (i - 2) % 2 != 0) // run if the previous number (i - 2) is odd
                                {
                                    reihenGesamtverschiebung = reihenGesamtverschiebung + reihenDifferenz;
                                }

                                graphics.DrawImage(pngImage, x, y + reihenGesamtverschiebung + headerGesamtverschiebung, pngImage.Width, pngImage.Height); //insert into column 1
                            }
                            else // run if even number
                            {
                                graphics.DrawImage(pngImage, x2, y + reihenGesamtverschiebung + headerGesamtverschiebung, pngImage.Width, pngImage.Height); //insert into column 2
                            }
                        }



                    }
                }
                else
                {
                    // Insert the PNG image into the JPEG image
                    using (Graphics graphics = Graphics.FromImage(jpgImage))
                    {
                        int reihenGesamtverschiebung = 0;

                        for (int i = 1; i <= newPackageParafCounter; i++) //CAREFUL! [i] starts on 1!! **********
                        {
                            //if (i % 2 == 0 || i == 0)


                            if (i % 2 != 0) // run if odd number
                            {
                                if (i > 2 && (i - 2) % 2 != 0) // run if the previous number (i - 2) is odd
                                {
                                    reihenGesamtverschiebung = reihenGesamtverschiebung + reihenDifferenz;
                                }

                                graphics.DrawImage(pngImage, x, y + reihenGesamtverschiebung + headerGesamtverschiebung, pngImage.Width, pngImage.Height); //insert into column 1
                            }
                            else // run if even number
                            {
                                graphics.DrawImage(pngImage, x2, y + reihenGesamtverschiebung + headerGesamtverschiebung, pngImage.Width, pngImage.Height); //insert into column 2
                            }


                        }
                    }

                }
                #endregion



                /*                if (oldPackageParafCounter == 4)
                                {
                                    // Insert the PNG image into the JPEG image
                                    using (Graphics graphics = Graphics.FromImage(jpgImage))
                                    {
                                        graphics.DrawImage(pngImage, x, yR1, pngImage.Width, pngImage.Height); // 1
                                        graphics.DrawImage(pngImage, x2, yR1, pngImage.Width, pngImage.Height); // 2
                                        graphics.DrawImage(pngImage, x, yR2, pngImage.Width, pngImage.Height); // 3
                                        graphics.DrawImage(pngImage, x2, yR2, pngImage.Width, pngImage.Height); // 4
                                    }
                                }
                                else if (newPackageParafCounter == 8)
                                {
                                    // Insert the PNG image into the JPEG image
                                    using (Graphics graphics = Graphics.FromImage(jpgImage))
                                    {
                                        yR1 = yR1 + Y_Verschiebung;
                                        yR2 = yR2 + Y_Verschiebung;
                                        yR3 = yR3 + Y_Verschiebung;
                                        yR4 = yR4 + Y_Verschiebung;

                                        graphics.DrawImage(pngImage, x, yR1, pngImage.Width, pngImage.Height); // 1
                                        graphics.DrawImage(pngImage, x2, yR1, pngImage.Width, pngImage.Height); // 2
                                        graphics.DrawImage(pngImage, x, yR2, pngImage.Width, pngImage.Height); // 3
                                        graphics.DrawImage(pngImage, x2, yR2, pngImage.Width, pngImage.Height); // 4
                                        graphics.DrawImage(pngImage, x, yR3, pngImage.Width, pngImage.Height); // 5
                                        graphics.DrawImage(pngImage, x2, yR3, pngImage.Width, pngImage.Height); // 6
                                        graphics.DrawImage(pngImage, x, yR4, pngImage.Width, pngImage.Height); // 7
                                        graphics.DrawImage(pngImage, x2, yR4, pngImage.Width, pngImage.Height); // 8
                                    }
                                }*/





                // Replace "_raw" with an empty string
                string exportPath = jpgOutputPath.Replace("_raw", "");
                string exportFileName = jpgFileName.Replace("_raw", "");

                jpgImage.Save(exportPath);

                // Dispose the images to release resources
                jpgImage.Dispose();
                pngImage.Dispose();

                // Check if the file exists
                if (File.Exists(jpgOutputPath))
                {
                    File.Delete(jpgOutputPath); // Delete the existing file
                }

                Console.WriteLine($"Image with PNG inserted saved as {exportFileName}");
            }
        }

    }
}
