using System;
using System.Collections.Generic;
using System.Reflection;
using FluentNHibernate.Mapping;
using ZQNB.Common.Extensions;

namespace ZQNB.Common.NHExtensions
{
    //demo for use NbValueObject
    /// <summary>
    /// 自定义的时间段模型，可用于有开始时间和结束时间的场合
    /// 例如课节等
    /// </summary>
    public class DateTimeRange : Data.Model.NbValueObject<DateTimeRange>
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public DateTimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
        public DateTimeRange(DateTime start, TimeSpan duration)
        {
            Start = start;
            End = start.Add(duration);
        }
        protected DateTimeRange() { }
        public DateTimeRange NewEnd(DateTime newEnd)
        {
            return new DateTimeRange(this.Start, newEnd);
        }
        public DateTimeRange NewDuration(TimeSpan newDuration)
        {
            return new DateTimeRange(this.Start, newDuration);
        }
        public DateTimeRange NewStart(DateTime newStart)
        {
            return new DateTimeRange(newStart, this.End);
        }

        public static DateTimeRange CreateMinuteRange(DateTime startDate, int minutes)
        {
            return new DateTimeRange(startDate, startDate.AddMinutes(minutes));
        }
        public static DateTimeRange CreateHourRange(DateTime startDate, int hour)
        {
            return new DateTimeRange(startDate, startDate.AddHours(hour));
        }
        public static DateTimeRange CreateOneDayRange(DateTime day)
        {
            return new DateTimeRange(day, day.AddDays(1));
        }
        public static DateTimeRange CreateOneWeekRange(DateTime startDay)
        {
            return new DateTimeRange(startDay, startDay.AddDays(7));
        }

        /// <summary>
        /// 保留时间，把日期部分自动修正为指定的时间的日期
        /// </summary>
        /// <param name="dayDate"></param>
        /// <returns></returns>
        public DateTimeRange FixDateTimeRangeOfSomeDay(DateTime dayDate)
        {
            var defaultDayDate = dayDate.Date;
            var startFix = defaultDayDate.Add(this.Start.TimeOfDay);
            var endFix = defaultDayDate.Add(this.End.TimeOfDay);
            var dateTimeRange = new DateTimeRange(startFix, endFix);
            return dateTimeRange;
        }

        /// <summary>
        /// 只保留时间，日期部分自动修正为2000，1，1
        /// </summary>
        /// <returns></returns>
        public DateTimeRange FixDateTimeRangeOfDefaultDay()
        {
            var fixDateTimeRangeOfSomeDay = FixDateTimeRangeOfSomeDay(new DateTime().GetDefaultDate().Date);
            return fixDateTimeRangeOfSomeDay;
        }


        /// <summary>
        /// 时间间隔的分钟数部分。
        /// </summary>
        /// <returns></returns>
        public int DurationInMinutes()
        {
            return (End - Start).Minutes;
        }
        /// <summary>
        /// 查看两个时间段是否重叠
        /// </summary>
        /// <param name="dateTimeRange"></param>
        /// <returns></returns>
        public bool Overlaps(DateTimeRange dateTimeRange)
        {
            return this.Start < dateTimeRange.End &&
                this.End > dateTimeRange.Start;
        }
    }

    /// <summary>
    /// 自定义组件的映射
    /// End，Start关键字在映射时有问题，回避
    /// </summary>
    public class DateTimeRangeMap : ComponentMap<DateTimeRange>
    {
        public DateTimeRangeMap()
        {
            Map(r => r.Start).Column("StartAt");
            Map(r => r.End).Column("EndAt");
        }
    }
}