using System;
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
            Console.WriteLine("Please, insert the month (1-12):");
            var month = Console.ReadLine();
            Console.WriteLine("Please, insert the year (e.g 2021):");
            var year = Console.ReadLine();

            var firstDay = new DateTime(int.Parse(year), int.Parse(month), 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);
            var random = new Random();
            var gap = 0.0f;

            var schedule = Enumerable.Range(1, lastDay.Day).Select(x =>
            {
                var day = new DateTime(int.Parse(year), int.Parse(month), x);

                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    return $"{day.ToString("yyyy-MM-dd")};-;-;00:00;0";
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
    }
}

// dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true