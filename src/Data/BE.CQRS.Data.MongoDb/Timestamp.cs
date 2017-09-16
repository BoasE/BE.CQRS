using System;

namespace BE.CQRS.Data.MongoDb
{
    public static class Timestamp
    {
        private static readonly DateTimeOffset RefDate = new DateTimeOffset(2017, 7, 1, 0, 0, 0, TimeSpan.Zero);

        public static long FromNow()
        {
            return From(DateTimeOffset.Now);
        }

        public static long From(DateTimeOffset source)
        {
            if (source < RefDate)
            {
                throw new ArgumentOutOfRangeException(nameof(source), source, $"Value must be greater than {RefDate}");
            }
            return (long)(source - RefDate).TotalMilliseconds;
        }
    }
}