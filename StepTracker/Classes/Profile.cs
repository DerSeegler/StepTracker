namespace StepTracker.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// A User/Stepmania Profile
    /// </summary>
    public class Profile
    {
        /// <summary>
        /// The save file
        /// </summary>
        private XDocument saveFile;

        /// <summary>
        /// The x profile
        /// </summary>
        private XElement xProfile;

        /// <summary>
        /// Initializes a new instance of the <see cref="Profile"/> class.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="saveFile">The save file.</param>
        public Profile(XElement profile, XDocument saveFile)
        {
            this.saveFile = saveFile;
            this.xProfile = profile;
            this.FilePath = profile.Element("FilePath").Value;

            int lastSessionCount = 0;
            int.TryParse(profile.Element("LastSessionCount").Value, out lastSessionCount);
            this.LastSessionCount = lastSessionCount;

            long lastGameplaySecondsCount = 0;
            long.TryParse(profile.Element("LastGameplaySecondsCount").Value, out lastGameplaySecondsCount);
            this.LastGameplaySecondsCount = lastGameplaySecondsCount;

            long totalGameplaySeconds = 0;
            long.TryParse(profile.Element("TotalGameplaySeconds").Value, out totalGameplaySeconds);
            this.TotalGameplaySeconds = totalGameplaySeconds;

            this.Sessions = new List<Session>();
            this.NewSessions = new List<Session>();
            foreach (XElement session in profile.Element("Sessions").Elements("Session"))
            {
                this.Sessions.Add(new Session(session));
            }
        }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the last session count.
        /// </summary>
        /// <value>
        /// The last session count.
        /// </value>
        public int LastSessionCount { get; set; }

        /// <summary>
        /// Gets or sets the last gameplay seconds count.
        /// </summary>
        /// <value>
        /// The last gameplay seconds count.
        /// </value>
        public long LastGameplaySecondsCount { get; set; }

        /// <summary>
        /// Gets the total sessions.
        /// </summary>
        /// <value>
        /// The total sessions.
        /// </value>
        public int TotalSessions
        {
            get { return this.Sessions.Count; }
        }

        /// <summary>
        /// Gets or sets the total gameplay seconds.
        /// </summary>
        /// <value>
        /// The total gameplay seconds.
        /// </value>
        public long TotalGameplaySeconds { get; set; }

        /// <summary>
        /// Gets or sets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        public List<Session> Sessions { get; set; }

        /// <summary>
        /// Gets or sets the new sessions.
        /// </summary>
        /// <value>
        /// The new sessions.
        /// </value>
        public List<Session> NewSessions { get; set; }

        /// <summary>
        /// Gets the gameplay seconds this year.
        /// </summary>
        /// <value>
        /// The gameplay seconds this year.
        /// </value>
        public long GameplaySecondsThisYear
        {
            get { return this.Sessions.Where(s => s.Date.Year == DateTime.Now.Year).Sum(s => s.GameplaySeconds); }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            xProfile.Element("LastSessionCount").Value = this.LastSessionCount.ToString();
            xProfile.Element("LastGameplaySecondsCount").Value = this.LastGameplaySecondsCount.ToString();
            xProfile.Element("TotalGameplaySeconds").Value = this.TotalGameplaySeconds.ToString();
            foreach (Session session in this.NewSessions.ToList())
            {
                xProfile.Element("Sessions").Add(
                    new XElement("Session", new XElement[]
                    {
                        new XElement("Date", session.Date.ToShortDateString()),
                        new XElement("GameplaySeconds", session.GameplaySeconds.ToString()),
                        new XElement("SessionsCount", session.SessionsCount.ToString())
                    }));
                this.NewSessions.Remove(session);
            }

            saveFile.Save(MainWindow.SaveFilePath);
        }
    }
}