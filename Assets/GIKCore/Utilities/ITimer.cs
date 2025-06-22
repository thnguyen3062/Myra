using System.Collections.Generic;
using System;

namespace GIKCore.Utilities
{
    public class ITimeSpanFormat
    {
        public const string S = "%s";
        public const string SS = "ss";
        public const string M = "%m";
        public const string MM = "mm";
        public const string H = "%h";
        public const string HH = "hh";
        public const string D = "%d";
        public const string DD = "dd";
        public const string MM_SS = @"mm\:ss";
        public const string HH_MM = @"hh\:mm";
        public const string HH_MM_SS = @"hh\:mm\:ss";
        public const string HH_MM_NEW = @"h :mm\:ss";
    }
    public class IDateTimeFormat
    {
        public const string HH_MM_DD_MM_YYYY = "HH:mm dd/MM/yyyy";
        public const string DD_MM_YYYY = "dd/MM/yyyy";
    }

    public class ITimer
    {
        private static DateTime Jan1st1970Utc { get { return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); } }
        public static long UtcNowTicks { get { return DateTime.UtcNow.Ticks; } }
        public static long GetTimestampInMilliseconds(DateTime? utc = null)
        {
            if (utc == null) utc = DateTime.UtcNow;
            return (long)(utc.Value - Jan1st1970Utc).TotalMilliseconds;
        }
        public static long GetTimestampInSeconds(DateTime? utc = null)
        {
            if (utc == null) utc = DateTime.UtcNow;
            return (long)(utc.Value - Jan1st1970Utc).TotalSeconds;
        }
        public static long GetTimeDeltaFromNowInSeconds(long endSeconds)
        {
            DateTime dateStart = DateTime.UtcNow;
            DateTime dateEnd = GetUtcTimeFromTimestamp(endSeconds);

            long delta = (long)(dateEnd - dateStart).TotalSeconds;
            return Math.Abs(delta);
        }
        public static long GetTimeDeltaFromNowInMilliseconds(long endMilliseconds)
        {
            DateTime dateStart = DateTime.UtcNow;
            DateTime dateEnd = GetUtcTimeFromTimestamp2(endMilliseconds);

            long delta = (long)(dateEnd - dateStart).TotalMilliseconds;
            return Math.Abs(delta);
        }
        public static long GetTimePassInSeconds(long lastTicks)
        {
            long delta = UtcNowTicks - lastTicks;
            long seconds = (long)TimeSpan.FromTicks(delta).TotalSeconds;
            return seconds;
        }
        public static long GetTimePassInSeconds(DateTime utc)
        {
            return GetTimePassInSeconds(utc.Ticks);
        }
        public static long GetTimePassInMilliseconds(long lastTicks)
        {
            long delta = UtcNowTicks - lastTicks;
            long milliseconds = (long)TimeSpan.FromTicks(delta).TotalMilliseconds;
            return milliseconds;
        }
        public static long GetTimePassInMilliseconds(DateTime utc)
        {
            return GetTimePassInMilliseconds(utc.Ticks);
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings?redirectedfrom=MSDN
        /// </summary>
        /// <param name="remainSeconds"></param>
        /// <param name="timeSpanFormat"></param>
        /// <returns></returns>
        public static string GetTimeDisplay(long remainSeconds, string timeSpanFormat)
        {
            TimeSpan ts = TimeSpan.FromSeconds(remainSeconds);
            return ts.ToString(timeSpanFormat);
        }
        public static string GetTimeDisplay2(long remainMilliseconds, string timeSpanFormat)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(remainMilliseconds);
            return ts.ToString(timeSpanFormat);
        }

        public static DateTime GetLocalTimeFromTimestamp(long seconds)
        {
            DateTime utc = Jan1st1970Utc.AddSeconds(seconds);
            DateTime local = utc.ToLocalTime();
            return local;
        }
        public static DateTime GetUtcTimeFromTimestamp(long seconds)
        {
            DateTime utc = Jan1st1970Utc.AddSeconds(seconds);
            return utc;
        }
        public static DateTime GetUtcTimeFromTimestamp2(long milliseconds)
        {
            DateTime utc = Jan1st1970Utc.AddMilliseconds(milliseconds);
            return utc;
        }
        public static string FormatTimestampToLocalTime(long seconds, string dateTimeFormat = IDateTimeFormat.HH_MM_DD_MM_YYYY, string todayDtFormat = "")
        {
            DateTime local = GetLocalTimeFromTimestamp(seconds);
            if (local.Date == DateTime.Today && !string.IsNullOrEmpty(todayDtFormat))
                return local.ToString(todayDtFormat);
            return local.ToString(dateTimeFormat);
        }
        public static string FormatTimeSpan(long remainSeconds, string timeSpanFormat = ITimeSpanFormat.HH_MM)
        {
            TimeSpan ts = TimeSpan.FromSeconds(remainSeconds);
            return ts.ToString(timeSpanFormat);
        }
        public static bool CheckLocalToday(long seconds)
        {
            DateTime local = GetLocalTimeFromTimestamp(seconds);
            return (local.Date == DateTime.Today);
        }

        public static List<long> GetTimestampRangeInSeconds(long secondsStart, long secondsEnd, long secondsOffset = 24 * 60 * 60, bool stopInUtcNow = true)
        {
            if (stopInUtcNow)
            {
                long secondsNow = GetTimestampInSeconds();
                if (secondsEnd > secondsNow) secondsEnd = secondsNow;
            }

            List<long> lstTimestamp = new List<long>();
            for (long seconds = secondsStart; seconds <= secondsEnd; seconds += secondsOffset)
            {
                lstTimestamp.Add(seconds);
            }
            return lstTimestamp;
        }
    }

    public class ITimeCache
    {
        private long time = 60;//in seconds
        private long lastTicks = 0;//in milliseconds

        public ITimeCache(long seconds = 60, bool renew = false) { SetCache(seconds, renew); }

        public void SetCache(long seconds, bool renew = false)
        {
            time = seconds;
            if (renew) Renew();
        }
        public long GetCache() { return time; }
        public long GetCacheRemainInSeconds()
        {
            long remain = time;
            long timePass = ITimer.GetTimePassInSeconds(lastTicks);
            if (timePass > 0) remain -= timePass;
            if (remain <= 0) remain = 0;

            return remain;
        }
        public long GetCacheRemainInMilliseconds()
        {
            long remain = time * 1000;
            long timePass = ITimer.GetTimePassInMilliseconds(lastTicks);
            if (timePass > 0) remain -= timePass;
            if (remain <= 0) remain = 0;

            return remain;
        }
        public void Renew() { lastTicks = DateTime.UtcNow.Ticks; }

        public bool CheckExpired(bool autoRenew = true)
        {
            bool expired = false;
            if (lastTicks <= 0) expired = true;
            else
            {
                long timePass = ITimer.GetTimePassInSeconds(lastTicks);
                if ((time - timePass) <= 0) expired = true;
            }

            if (expired && autoRenew) Renew();
            return expired;
        }

        public void ForceExpired() { lastTicks = 0; }
    }

    public struct ITimeDelta
    {
        public long time { get; private set; }//in seconds or milliseconds
        private long lastTicks;//in milliseconds

        public static implicit operator ITimeDelta(long value)
        {
            return new ITimeDelta { time = value, lastTicks = DateTime.UtcNow.Ticks };
        }

        public static ITimeDelta operator -(ITimeDelta source, long timePass)
        {
            source.time -= timePass;
            if (source.time <= 0) source.time = 0;
            source.lastTicks = DateTime.UtcNow.Ticks;
            return source;
        }

        public static ITimeDelta operator +(ITimeDelta source, long timePass)
        {
            source.time += timePass;
            if (source.time <= 0) source.time = 0;
            source.lastTicks = DateTime.UtcNow.Ticks;
            return source;
        }

        public static bool operator >(ITimeDelta source, long value)
        {
            return source.time > value;
        }

        public static bool operator <(ITimeDelta source, long value)
        {
            return source.time < value;
        }

        public static bool operator ==(ITimeDelta source, long value)
        {
            return source.time == value;
        }

        public static bool operator !=(ITimeDelta source, long value)
        {
            return source.time != value;
        }

        public static bool operator >=(ITimeDelta source, long value)
        {
            return source.time >= value;
        }

        public static bool operator <=(ITimeDelta source, long value)
        {
            return source.time <= value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ITimeDelta)
                return this == ((ITimeDelta)obj).time;
            else if (obj is long)
                return this == (long)obj;
            else if (obj is int)
                return this == (int)obj;
            return false;
        }

        public override int GetHashCode()
        {
            return time.GetHashCode() ^ lastTicks.GetHashCode();
        }

        public long GetTimePassInSeconds()
        {
            long delta = DateTime.UtcNow.Ticks - lastTicks;
            long seconds = (long)TimeSpan.FromTicks(delta).TotalSeconds;
            return seconds;
        }

        public long GetTimePassInMilliseconds()
        {
            long delta = DateTime.UtcNow.Ticks - lastTicks;
            long milliseconds = (long)TimeSpan.FromTicks(delta).TotalMilliseconds;
            return milliseconds;
        }
        public long timePassInSeconds
        {
            get
            {
                long delta = DateTime.UtcNow.Ticks - lastTicks;
                long seconds = (long)TimeSpan.FromTicks(delta).TotalSeconds;
                return seconds;
            }
        }

        public long timePassInMilliseconds
        {
            get
            {
                long delta = DateTime.UtcNow.Ticks - lastTicks;
                long milliseconds = (long)TimeSpan.FromTicks(delta).TotalMilliseconds;
                return milliseconds;
            }
        }
        public void MakeTimePassInSeconds()
        {
            long timePass = timePassInSeconds;
            if (timePass > 0) this -= timePass;
        }
        public void MakeTimePassInMilliseconds()
        {
            long timePass = timePassInMilliseconds;
            if (timePass > 0) this -= timePass;
        }
    }
}