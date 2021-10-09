using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Extensions
{
    public static class DateTimeExtensions
    {
        private static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }
        public static string ToPrettyDate(this DateTime d)
        {
            TimeSpan s = DateTime.UtcNow.Subtract(d);

            int monthsDiff = GetMonthDifference(d, DateTime.UtcNow);

            if (monthsDiff > 12)
            {
                return d.ToShortDateString();
            }


            int dayDiff = (int)s.TotalDays;
            // 3.
            // Get total number of seconds elapsed.
            int secDiff = (int)s.TotalSeconds;

            // 4.
            // Don't allow out of range values.
            if (dayDiff < 0 || dayDiff >= 31)
            {
                return null;
            }

            // 5.
            // Handle same-day times.
            if (dayDiff == 0)
            {
                // A.
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "just now";
                }
                // B.
                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "1 minute ago";
                }
                // C.
                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    return string.Format("{0} minutes ago", Math.Floor((double)secDiff / 60));
                }
                // D.
                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "1 hour ago";
                }
                // E.
                // Less than one day ago.
                if (secDiff < 86400)
                {
                    return string.Format("{0} hours ago", Math.Floor((double)secDiff / 3600));
                }
            }
            // 6.
            // Handle previous days.
            if (dayDiff == 1)
            {
                return "yesterday";
            }
            if (dayDiff < 7)
            {
                return $"{dayDiff} days ago";
            }
            if (dayDiff < 31)
            {
                return string.Format("{0} weeks ago", Math.Ceiling((double)dayDiff / 7));
            }
            if (monthsDiff > 0)
            {
                return monthsDiff == 1 ? $"{monthsDiff} month ago" : $"{monthsDiff} months ago";
            }
            return null;
        }

    }
}
