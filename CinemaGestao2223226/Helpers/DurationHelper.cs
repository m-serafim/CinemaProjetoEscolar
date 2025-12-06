namespace CinemaGestao.Helpers
{
    public static class DurationHelper
    {
        /// <summary>
        /// Formats duration from minutes to a human-readable format (hours and minutes)
        /// </summary>
        /// <param name="durationMinutes">Duration in minutes</param>
        /// <returns>Formatted duration string</returns>
        public static string FormatDuration(int durationMinutes)
        {
            if (durationMinutes < 60)
            {
                // Less than 1 hour - show only minutes
                return $"{durationMinutes} min";
            }
            else
            {
                // 1 hour or more - show hours and minutes
                int hours = durationMinutes / 60;
                int minutes = durationMinutes % 60;

                if (minutes == 0)
                {
                    // Exact hours
                    return hours == 1 ? "1 hour" : $"{hours} hours";
                }
                else
                {
                    // Hours and minutes
                    return $"{hours}h {minutes}min";
                }
            }
        }
    }
}