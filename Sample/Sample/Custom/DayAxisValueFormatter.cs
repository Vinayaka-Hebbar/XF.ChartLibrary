using System;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Formatter;

namespace Sample.Custom
{
    public class DayAxisValueFormatter : IAxisValueFormatter
    {
        private readonly string[] months = new string[]{
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        private readonly IBarLineChartBase chart;

        public DayAxisValueFormatter(IBarLineChartBase chart)
        {
            this.chart = chart;
        }

        public string GetFormattedValue(float value, AxisBase axis)
        {
            int days = (int)value;

            int year = DetermineYear(days);

            int month = DetermineMonth(days);
            string monthName = months[month % months.Length];
            string yearName = year.ToString();

            if (chart.VisibleXRange > 30 * 6)
            {

                return monthName + " " + yearName;
            }
            else
            {

                int dayOfMonth = DetermineDayOfMonth(days, month + 12 * (year - 2016));

                string appendix = "th";

                switch (dayOfMonth)
                {
                    case 1:
                        appendix = "st";
                        break;
                    case 2:
                        appendix = "nd";
                        break;
                    case 3:
                        appendix = "rd";
                        break;
                    case 21:
                        appendix = "st";
                        break;
                    case 22:
                        appendix = "nd";
                        break;
                    case 23:
                        appendix = "rd";
                        break;
                    case 31:
                        appendix = "st";
                        break;
                }

                return dayOfMonth == 0 ? "" : dayOfMonth + appendix + " " + monthName;
            }
        }

        private int GetDaysForMonth(int month, int year)
        {
            // month is 0-based
            if (month == 1)
            {
                bool is29Feb = false;

                if (year < 1582)
                    is29Feb = (year < 1 ? year + 1 : year) % 4 == 0;
                else if (year > 1582)
                    is29Feb = year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);

                return is29Feb ? 29 : 28;
            }

            if (month == 3 || month == 5 || month == 8 || month == 10)
                return 30;
            else
                return 31;
        }

        private int DetermineMonth(int dayOfYear)
        {
            int month = -1;
            int days = 0;

            while (days < dayOfYear)
            {
                month += 1;

                if (month >= 12)
                    month = 0;

                int year = DetermineYear(days);
                days += GetDaysForMonth(month, year);
            }

            return Math.Max(month, 0);
        }

        private int DetermineDayOfMonth(int days, int month)
        {
            int count = 0;
            int daysForMonths = 0;

            while (count < month)
            {

                int year = DetermineYear(daysForMonths);
                daysForMonths += GetDaysForMonth(count % 12, year);
                count++;
            }

            return days - daysForMonths;
        }

        private int DetermineYear(int days)
        {
            if (days <= 366)
                return 2016;
            else if (days <= 730)
                return 2017;
            else if (days <= 1094)
                return 2018;
            else if (days <= 1458)
                return 2019;
            else
                return 2020;

        }
    }
}
