namespace StepTracker.Classes
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// One Session of playing Stepmania
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="seconds">The seconds.</param>
        /// <param name="sessionsCount">The sessions count.</param>
        public Session(DateTime date, long seconds, int sessionsCount = 1)
        {
            this.Date = date;
            this.GameplaySeconds = seconds;
            this.SessionsCount = sessionsCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public Session(XElement session)
        {
            DateTime date = DateTime.Now;
            DateTime.TryParse(session.Element("Date").Value, out date);
            this.Date = date;

            long gameplaySeconds = 0;
            long.TryParse(session.Element("GameplaySeconds").Value, out gameplaySeconds);
            this.GameplaySeconds = gameplaySeconds;

            int sessionsCount = 0;
            int.TryParse(session.Element("SessionsCount").Value, out sessionsCount);
            this.SessionsCount = sessionsCount;
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the gameplay seconds.
        /// </summary>
        /// <value>
        /// The gameplay seconds.
        /// </value>
        public long GameplaySeconds { get; set; }

        /// <summary>
        /// Gets or sets the sessions count.
        /// </summary>
        /// <value>
        /// The sessions count.
        /// </value>
        public int SessionsCount { get; set; }
    }
}