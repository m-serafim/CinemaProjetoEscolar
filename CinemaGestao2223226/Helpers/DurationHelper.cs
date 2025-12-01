namespace CinemaGestao.Helpers
{
    public static class DurationHelper
    {
        /// <summary>
        /// Formats duration from minutes to a human-readable format (hours and minutes)
        /// </summary>
        /// <param name="durationMinutes">Duration in minutes</param>
        /// <param name="isEnglish">Whether to use English or Portuguese</param>
        /// <returns>Formatted duration string</returns>
        public static string FormatDuration(int durationMinutes, bool isEnglish = false)
        {
            if (durationMinutes < 60)
            {
                // Less than 1 hour - show only minutes
                return isEnglish ? $"{durationMinutes} min" : $"{durationMinutes} min";
            }
            else
            {
                // 1 hour or more - show hours and minutes
                int hours = durationMinutes / 60;
                int minutes = durationMinutes % 60;

                if (minutes == 0)
                {
                    // Exact hours
                    return isEnglish 
                        ? (hours == 1 ? "1 hour" : $"{hours} hours")
                        : (hours == 1 ? "1 hora" : $"{hours} horas");
                }
                else
                {
                    // Hours and minutes
                    return isEnglish 
                        ? $"{hours}h {minutes}min"
                        : $"{hours}h {minutes}min";
                }
            }
        }
    }
}