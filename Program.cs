using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TimeRangeGenerator
{
    class Program
    {
        const float StartTime = 8.5f;
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Time Range Generator v0.0.1");

            var month = askByMonth();
            var year = askByYear();
            var holidays = askByHolidays();

            var firstDay = new DateTime(year, month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);
            var random = new Random();
            var gap = 0.0f;

            var schedule = Enumerable.Range(1, lastDay.Day).Select(currentDay =>
            {
                var day = new DateTime(year, month, currentDay);

                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    return $"{day.ToString("yyyy-MM-dd")};;;;";
                }

                if (holidays.Contains(currentDay))
                {
                    return $"{day.ToString("yyyy-MM-dd")};;;;";
                }

                var journeyDuration = 9.5f;
                var timeToLunch = 1;

                if (day.DayOfWeek == DayOfWeek.Friday)
                {
                    journeyDuration = 6;
                    timeToLunch = 0;
                }

                var delay = (float)random.NextDouble() * 0.5f * (gap >= 0 ? 1 : -1);
                var initalHour = floatToTimeSpan(StartTime + delay);
                var finalHour = floatToTimeSpan(StartTime + journeyDuration - delay + (float)random.NextDouble() * 0.2f);
                var totalTime = finalHour - initalHour;
                gap += ((float)totalTime.TotalHours - journeyDuration);
                return $"{day.ToString("yyyy-MM-dd")};{initalHour};{finalHour};{totalTime - new TimeSpan(timeToLunch, 0, 0)};{(totalTime.TotalHours - timeToLunch).ToString("0.##").Replace('.', ',')}";
            }).ToList();

            File.WriteAllLines($"{month}_{year}.csv", schedule);
        }

        static TimeSpan floatToTimeSpan(float time)
        {
            return new TimeSpan((int)Math.Truncate(time), (int)((time - Math.Truncate(time)) * 60), 0);
        }

        static int askByMonth()
        {
            var month = 0;
            do
            {
                try
                {
                    Console.WriteLine("Please, insert the month (1-12):");
                    month = int.Parse(Console.ReadLine());
                }
                catch
                {
                    month = 0;
                }

            } while (month <= 0 || month > 12);
            return month;
        }

        static int askByYear()
        {
            var year = 0;
            do
            {
                try
                {
                    Console.WriteLine("Please, insert the year (e.g 2021):");
                    year = int.Parse(Console.ReadLine());
                }
                catch
                {
                    year = 0;
                }

            } while (year < 2000);
            return year;
        }

        static List<int> askByHolidays()
        {
            List<int> holidays = null;
            do
            {
                try
                {
                    Console.WriteLine("Please, insert the days off (holidays, vacation, ...) (e.g 3,7,10-15):");
                    var line = Console.ReadLine();
                    if (line == String.Empty)
                        return new List<int>();
                    return line.Split(',').Select(interval =>
                    interval.Contains('-') ? Enumerable.Range(int.Parse(interval.Split('-')[0]), int.Parse(interval.Split('-')[1]) - int.Parse(interval.Split('-')[0]) + 1).ToList()
                        : new List<int> { int.Parse(interval) }).SelectMany(x => x).ToList();
                }
                catch
                {
                }

            } while (holidays != null);
            return holidays;
        }
    }
}

// dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true