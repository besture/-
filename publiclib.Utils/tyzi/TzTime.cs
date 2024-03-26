using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace publiclib.Utils
{
    public class TzTime
    {
        private DateTime _dt;
        private string _Year, _Month, _Day, _Hours, _Min, _Sec, _MiSec, _SpanTime, _Week;
        public TzTime() { }
        public TzTime(DateTime dt)
        {
            _dt = dt;
            this.LoadTime(_dt);
            this.LoadWeek(_dt);
        }
        public TzTime(string dt)
        {
            if (!DateTime.TryParse(dt, out _dt))
                _dt = DateTime.Now;
            this.LoadTime(_dt);
            this.LoadWeek(_dt);
        }

        private void LoadTime(DateTime dt)
        {
            _Year = dt.Year.ToString();
            _Month = dt.Month.ToString();
            _Day = dt.Day.ToString();
            _Hours = dt.Hour.ToString();
            _Min = dt.Minute.ToString();
            _Sec = dt.Second.ToString();
            _MiSec = dt.Millisecond.ToString();
        }

        /// <summary>
        /// 星期[一二三四五六日]
        /// </summary>
        private void LoadWeek(DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    _Week = "星期一";
                    break;
                case DayOfWeek.Tuesday:
                    _Week = "星期二";
                    break;
                case DayOfWeek.Wednesday:
                    _Week = "星期三";
                    break;
                case DayOfWeek.Thursday:
                    _Week = "星期四";
                    break;
                case DayOfWeek.Friday:
                    _Week = "星期五";
                    break;
                case DayOfWeek.Saturday:
                    _Week = "星期六";
                    break;
                case DayOfWeek.Sunday:
                    _Week = "星期日";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 周[一二三四五六日]
        /// </summary>
        private void LoadWeekShort()
        {
            if (string.IsNullOrEmpty(_Week))
                _Week.Replace("星期", "周");
        }

        /// <summary>
        /// [一二三四五六日]
        /// </summary>
        private void LoadWeekShortNum()
        {
            if (string.IsNullOrEmpty(_Week))
                _Week.Replace("星期", "");
        }

        /// <summary>
        /// 返回日期中文格式 xx年xx月xx日 星期xx xx时xx分xx秒xx毫秒
        /// </summary>
        /// <param name="week">是否显示星期</param>
        /// <param name="hours">是否显示小时</param>
        /// <param name="min">是否显示分钟</param>
        /// <param name="sec">是否显示秒</param>
        /// <param name="misec">是否显示毫秒</param>
        public string ToChDate(bool week = false, bool hours = false, bool min = false, bool sec = false, bool misec = false)
        {
            string result = string.Empty;
            result = string.Format("{0}年{1}月{2}日", _Year, _Month, _Day);
            if (week && string.IsNullOrEmpty(_Week))
                result += string.Format("{0} {1}", result, _Week);
            if (hours)
                result += string.Format("{0} {1}时", result, hours);
            if (min)
                result += string.Format("{0}{1}分", result, min);
            if (sec)
                result += string.Format("{0}{1}秒", result, sec);
            if (misec)
                result += string.Format("{0}{1}毫秒", result, misec);
            return result;
        }

        /// <summary>
        /// 与Model最新时间差
        /// </summary>
        /// <param name="dtOld">前时间</param>
        /// <param name="day">是否显示年月日</param>
        /// <returns>[xx年xx月xx日]xx时xx分xx秒</returns>
        public string Diff(DateTime dtOld, bool day = false)
        {
            TimeSpan ts = _dt - dtOld;
            DateTime t = new DateTime(0, 0, ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds, DateTimeKind.Local);
            return day ? ToChDate(false, true, true, true) : ToChDate(false, true, true, true).Split(' ')[1];
        }

        /// <summary>
        /// 时间差
        /// </summary>
        /// <param name="dt">后时间</param>
        /// <param name="dtOld">前时间</param>
        /// <param name="day">是否显示年月日</param>
        /// <returns>[xx年xx月xx日]xx时xx分xx秒</returns>
        public string Diff(DateTime dt, DateTime dtOld, bool day = false)
        {
            TimeSpan ts = dt - dtOld;
            DateTime t = new DateTime(0, 0, ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds, DateTimeKind.Local);
            return day ? ToChDate(false, true, true, true) : ToChDate(false, true, true, true).Split(' ')[1];
        }

        //DateTime类型转换为时间戳(毫秒值)
        public static long DateToTicks(DateTime? time)
        {
            long ticks = ((time.HasValue ? time.Value.Ticks : DateTime.Parse("1990-01-01").Ticks) - 621355968000000000) / 10000;
            return ticks - 28800;
        }

        //时间戳(毫秒值)String转换为DateTime类型转换
        public static DateTime TicksToDate(string time)
        {
            return new DateTime((Convert.ToInt64(time) * 10000) + 621355968000000000);
        }

        /// <summary>
        /// 判断日期组中的年份是否相同
        /// </summary>
        /// <param name="dates">日期数组</param>
        /// <returns></returns>
        public static bool IsSameYear(out int outYear, params DateTime?[] dates)
        {
            outYear = DateTime.Now.Year;
            if (dates.Length == 0) return true;

            int i = 0;
            int year = 0;
            while (i < dates.Length - 1)
            {
                var d1 = dates[i];
                var d2 = dates[i + 1];
                if (d1 != null && d2 != null)
                {
                    if (d1.Value.Year == d2.Value.Year)
                    {
                        if (year == 0)
                            year = d1.Value.Year;
                        else if (year != d1.Value.Year)
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (d1 == null && d2 != null)
                {
                    if (year == 0)
                        year = d2.Value.Year;
                    else if (year != d2.Value.Year)
                        return false;
                }
                else if (d1 != null && d2 == null)
                {
                    if (year == 0)
                        year = d1.Value.Year;
                    else if (year != d1.Value.Year)
                        return false;
                }
                i++;
            }

            if (year != 0) outYear = year;
            return true;
        }

        /// <summary>
        /// 获取月份最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetMonthEndDate(DateTime dt)
        {
            var d1 = DateTime.Parse(dt.ToString("yyyy-MM-01")).AddMonths(1);
            return dt.AddDays(-1);
        }
    }
}
