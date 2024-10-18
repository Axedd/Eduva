﻿using SchoolSystem.Interfaces;
using System.Globalization;

namespace SchoolSystem.Services
{
    public class ScheduleCalculatorService : IScheduleCalculatorService
    {
        public (DateTime weekStart, DateTime weekEnd) GetWeekDays(int? weekNum)
        {
            DateTime weekStart;
            DateTime weekEnd;

            if (weekNum.HasValue && weekNum.Value > 0)
            {
                // Calculate the start date for the given week number
                DateTime yearStart = new DateTime(DateTime.Today.Year, 1, 1);
                int daysOffset = (weekNum.Value - 1) * 7; // Calculate days to add for the week number
                weekStart = yearStart.AddDays(daysOffset);

                // Adjust to the nearest Monday
                int dayOfWeek = (int)weekStart.DayOfWeek;
                if (dayOfWeek != 1) // 1 = Monday
                {
                    weekStart = weekStart.AddDays(-(dayOfWeek - 1)); // Move back to Monday
                }
            }
            else
            {
                // If weekNum is 0 or null, calculate the current week based on today's date
                weekStart = GetStartOfCurrentWeek();
            }

            // Calculate the end date for the week
            weekEnd = weekStart.AddDays(6);

            // Return both the start and end dates
            return (weekStart, weekEnd);
        }

        public DateTime GetStartOfCurrentWeek()
        {
            DateTime today = DateTime.Today;

            // Get the difference between today and the previous Monday
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = today.AddDays(-1 * diff); // Monday of the current week

            return startOfWeek;
        }

        public List<DateTime> GetCurrentWeekDays(int? weekNum)
        {
            List<DateTime> weekDays = new List<DateTime>();
            DateTime startOfWeek;

            if (weekNum != null)
            {
                // Get the first day of the current year
                DateTime firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);

                // Calculate the first Monday of the year
                int daysOffset = DayOfWeek.Monday - firstDayOfYear.DayOfWeek;
                DateTime firstMonday = firstDayOfYear.AddDays(daysOffset);

                // Calculate the start of the desired week
                startOfWeek = firstMonday.AddDays((weekNum.Value - 1) * 7);
            }
            else
            {
                // Use the current date (today)
                DateTime today = DateTime.Today;

                // Calculate the start of the current week (Monday)
                int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                startOfWeek = today.AddDays(-diff); // Monday of the current week
            }

            // Add the days for the week (Monday to Sunday)
            for (int i = 0; i < 7; i++)
            {
                weekDays.Add(startOfWeek.AddDays(i));
            }

            return weekDays;
        }

        public int GetWeekNumberOfYear(DateTime date)
        {
            // Get the current culture (could be modified for a specific culture)
            CultureInfo culture = CultureInfo.CurrentCulture;
            Calendar calendar = culture.Calendar;

            // Define that the week starts on Monday, and uses the first week that contains a Thursday as the first week
            CalendarWeekRule rule = culture.DateTimeFormat.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

            // Get the week number for the provided date
            int weekNumber = calendar.GetWeekOfYear(date, rule, firstDayOfWeek);

            return weekNumber;
        }

      
    }
}