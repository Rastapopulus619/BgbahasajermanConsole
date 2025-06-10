namespace TimCoreyCourseConsoleApp1
{
    public static class Timetable
    {
        //Dictionary<int, (string, string)> data = new Dictionary<int, (string, string)>
        public static Dictionary<int, (string, string)> Data { get; } = new Dictionary<int, (string, string)>
        {
            { 1, ("Montag", "07:00-08:30") },
            { 2, ("Montag", "08:30-08:30") },
            { 3, ("Montag", "10:00-11:30") },
            { 4, ("Montag", "13:00-14:30") },
            { 5, ("Montag", "14:30-16:00") },
            { 6, ("Montag", "16:00-17:30") },
            { 7, ("Montag", "17:30-19:00") },
            { 8, ("Montag", "19:00-20:30") },
            { 9, ("Dienstag", "07:00-08:30") },
            { 10, ("Dienstag", "08:30-08:30") },
            { 11, ("Dienstag", "10:00-11:30") },
            { 12, ("Dienstag", "13:00-14:30") },
            { 13, ("Dienstag", "14:30-16:00") },
            { 14, ("Dienstag", "16:00-17:30") },
            { 15, ("Dienstag", "17:30-19:00") },
            { 16, ("Dienstag", "19:00-20:30") },
            { 17, ("Mittwoch", "07:00-08:30") },
            { 18, ("Mittwoch", "08:30-08:30") },
            { 19, ("Mittwoch", "10:00-11:30") },
            { 20, ("Mittwoch", "13:00-14:30") },
            { 21, ("Mittwoch", "14:30-16:00") },
            { 22, ("Mittwoch", "16:00-17:30") },
            { 23, ("Mittwoch", "17:30-19:00") },
            { 24, ("Mittwoch", "19:00-20:30") },
            { 25, ("Donnerstag", "07:00-08:30") },
            { 26, ("Donnerstag", "08:30-08:30") },
            { 27, ("Donnerstag", "10:00-11:30") },
            { 28, ("Donnerstag", "13:00-14:30") },
            { 29, ("Donnerstag", "14:30-16:00") },
            { 30, ("Donnerstag", "16:00-17:30") },
            { 31, ("Donnerstag", "17:30-19:00") },
            { 32, ("Donnerstag", "19:00-20:30") },
            { 33, ("Freitag", "07:00-08:30") },
            { 34, ("Freitag", "08:30-08:30") },
            { 35, ("Freitag", "10:00-11:30") },
            { 36, ("Freitag", "13:00-14:30") },
            { 37, ("Freitag", "14:30-16:00") },
            { 38, ("Freitag", "16:00-17:30") },
            { 39, ("Freitag", "17:30-19:00") },
            { 40, ("Freitag", "19:00-20:30") },
            { 41, ("Samstag", "07:00-08:30") },
            { 42, ("Samstag", "08:30-08:30") },
            { 43, ("Samstag", "10:00-11:30") },
            { 44, ("Samstag", "13:00-14:30") },
            { 45, ("Samstag", "14:30-16:00") },
            { 46, ("Samstag", "16:00-17:30") },
            { 47, ("Samstag", "17:30-19:00") },
            { 48, ("Samstag", "19:00-20:30") },
            { 49, ("Sonntag", "07:00-08:30") },
            { 50, ("Sonntag", "08:30-08:30") },
            { 51, ("Sonntag", "10:00-11:30") },
            { 52, ("Sonntag", "13:00-14:30") },
            { 53, ("Sonntag", "14:30-16:00") },
            { 54, ("Sonntag", "16:00-17:30") },
            { 55, ("Sonntag", "17:30-19:00") },
            { 56, ("Sonntag", "19:00-20:30") }
        };
        static Timetable()
        {

        }

        public static (string, string) GetDayTime(int value)
        {
            if (Data.ContainsKey(value))
            {
                return Data[value];
            }
            return ("", ""); // Default values if not found
        }
    }

}
