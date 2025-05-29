namespace TuProyecto.Models
{
    public class GameClock
    {
        public int SecondsElapsed { get; set; }

        public GameClock(int seconds)
        {
            SecondsElapsed = seconds;
        }

        public string GetFormattedTime()
        {
            int totalMin = 60 * 8 + SecondsElapsed / 60;
            int hours = totalMin / 60;
            int minutes = totalMin % 60;
            int seconds = SecondsElapsed % 60;

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        public bool IsTimeOver()
        {
            return SecondsElapsed >= 90 * 60; // 1h30min = 5400s
        }
    }
}
