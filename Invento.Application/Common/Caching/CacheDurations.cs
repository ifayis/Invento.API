namespace Invento.Application.Common.Caching
{
    public static class CacheDurations
    {
        public static readonly TimeSpan Short =
            TimeSpan.FromMinutes(2);

        public static readonly TimeSpan Medium =
            TimeSpan.FromMinutes(10);

        public static readonly TimeSpan Long =
            TimeSpan.FromHours(1);

        public static readonly TimeSpan Dashboard =
            TimeSpan.FromMinutes(5);

        public static readonly TimeSpan Reports =
            TimeSpan.FromMinutes(30);
    }
}