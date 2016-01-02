using System.Windows.Forms;

namespace StepTracker
{
    using Classes;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Xml.Linq;

    public partial class MainWindow : Window
    {
        /// <summary>
        /// The save file path
        /// </summary>
        public static string SaveFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StepTracker", "data.xml");

        /// <summary>
        /// The application data path
        /// </summary>
        private readonly string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// The save file directory
        /// </summary>
        private readonly string saveFileDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StepTracker");

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.CreateSaveFileIfNecessary();
            this.ProfilePath = this.SelectProfile();

            if (this.ProfilePath != null)
            {
                this.SelectedProfile = this.GetProfile(this.ProfilePath);

                XDocument profileFile = XDocument.Load(this.ProfilePath);

                int newSessionCount = this.SelectedProfile.LastSessionCount;
                int.TryParse(profileFile.Descendants("TotalSessions").FirstOrDefault().Value, out newSessionCount);

                long newGameplaySeconds = this.SelectedProfile.LastGameplaySecondsCount;
                long.TryParse(profileFile.Descendants("TotalGameplaySeconds").FirstOrDefault().Value,
                    out newGameplaySeconds);

                DateTime lastPlayedDate = DateTime.Now;
                DateTime.TryParse(profileFile.Descendants("LastPlayedDate").FirstOrDefault().Value, out lastPlayedDate);

                if (this.SelectedProfile.LastSessionCount == 0)
                {
                    this.SelectedProfile.LastSessionCount = newSessionCount;
                    this.SelectedProfile.LastGameplaySecondsCount = newGameplaySeconds;
                    this.SelectedProfile.Save();
                }
                else
                {
                    if (newSessionCount > this.SelectedProfile.LastSessionCount)
                    {
                        if (newGameplaySeconds > this.SelectedProfile.LastGameplaySecondsCount)
                        {
                            int deltaSessions = newSessionCount - this.SelectedProfile.LastSessionCount;
                            long deltaGameplaySeconds = newGameplaySeconds -
                                                        this.SelectedProfile.LastGameplaySecondsCount;
                            double deltaGameplayHours = deltaGameplaySeconds / 3600.0;
                            MessageBoxResult result =
                                MessageBox.Show(
                                    string.Format(
                                        "{0} new session(s) found with {1} hours of gameplay.\r\n Do you want to add this session to the tracked data?\r\n\"No\" will skip the session and it will not show up the next time. \"Cancel\" will do nothing for now and this session will show up the next time.",
                                        deltaSessions, Math.Round(deltaGameplayHours, 1)), "New Session found",
                                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                this.SelectedProfile.LastSessionCount = newSessionCount;
                                this.SelectedProfile.LastGameplaySecondsCount = newGameplaySeconds;
                                this.SelectedProfile.TotalGameplaySeconds += deltaGameplaySeconds;
                                Session newSession = new Session(lastPlayedDate, deltaGameplaySeconds, deltaSessions);
                                this.SelectedProfile.Sessions.Add(newSession);
                                this.SelectedProfile.NewSessions.Add(newSession);
                                this.SelectedProfile.Save();
                            }
                            else if (result == MessageBoxResult.No)
                            {
                                this.SelectedProfile.LastSessionCount = newSessionCount;
                                this.SelectedProfile.LastGameplaySecondsCount = newGameplaySeconds;
                                this.SelectedProfile.Save();
                            }
                        }
                        else
                        {
                            this.SelectedProfile.LastSessionCount = newSessionCount;
                            this.SelectedProfile.Save();
                        }
                    }
                }

                this.RefreshUiValues();
            }
            else
            {
                MessageBox.Show(
                    "No profile found. The application will be closed.\r\nMake sure a Stepmania-Build is installed and at least one profile exists at the configured location.",
                    "No profile found!", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Gets or sets the profile path.
        /// </summary>
        /// <value>
        /// The profile path.
        /// </value>
        public string ProfilePath { get; set; }

        /// <summary>
        /// Gets or sets the selected profile.
        /// </summary>
        /// <value>
        /// The selected profile.
        /// </value>
        public Profile SelectedProfile { get; set; }

        /// <summary>
        /// Gets or sets the additional gameplay seconds.
        /// </summary>
        /// <value>
        /// The additional gameplay seconds.
        /// </value>
        public long AdditionalGameplaySeconds { get; set; }

        /// <summary>
        /// Gets the sum gameplay seconds.
        /// </summary>
        /// <value>
        /// The sum gameplay seconds.
        /// </value>
        public long SumGameplaySeconds
        {
            get { return this.SelectedProfile.GameplaySecondsThisYear + this.AdditionalGameplaySeconds; }
        }

        /// <summary>
        /// Refreshes the UI values.
        /// </summary>
        private void RefreshUiValues()
        {
            this.lbProgress.Content = string.Format("{0} out of 100 hours",
                Math.Round(this.SumGameplaySeconds / 3600.0, 1));
            this.pbProgress.Value = this.SumGameplaySeconds;
        }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private Profile GetProfile(string path)
        {
            XDocument saveFile = XDocument.Load(MainWindow.SaveFilePath);
            XElement xProfile =
                saveFile.Element("Data")
                    .Descendants("Profile")
                    .FirstOrDefault(d => d.Element("FilePath").Value.Equals(path));
            if (xProfile == null)
            {
                saveFile.Element("Data").Add(
                        new XElement("Profile", new XElement[]
                        {
                            new XElement("FilePath", path),
                            new XElement("LastSessionCount", "0"),
                            new XElement("LastGameplaySecondsCount", "0"),
                            new XElement("TotalGameplaySeconds", "0"),
                            new XElement("Sessions")
                        })
                    );

                saveFile.Save(MainWindow.SaveFilePath);

                xProfile =
                    saveFile.Element("Data")
                        .Descendants("Profile")
                        .FirstOrDefault(d => d.Element("FilePath").Value.Equals(path));
            }

            return new Profile(xProfile, saveFile);
        }

        /// <summary>
        /// Selects the profile.
        /// </summary>
        /// <returns></returns>
        private string SelectProfile()
        {
            XDocument saveFile = XDocument.Load(MainWindow.SaveFilePath);
            XElement xProgram = saveFile.Element("Data").Element("Program");
            if (xProgram == null)
            {
                saveFile.Element("Data").Add(new XElement("Program"));
                saveFile.Save(MainWindow.SaveFilePath);
            }

            XElement xProfilePath = saveFile.Element("Data").Element("Program").Element("ProfilePath");
            if (xProfilePath == null)
            {
                saveFile.Element("Data").Element("Program").Add(new XElement("ProfilePath", Path.Combine(this.appDataPath, "StepMania 5", "Save", "LocalProfiles")));
                saveFile.Save(MainWindow.SaveFilePath);

                xProfilePath = saveFile.Element("Data").Element("Program").Element("ProfilePath");
            }

            string localProfilesPath = xProfilePath.Value;
            if (Directory.Exists(localProfilesPath))
            {
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(localProfilesPath, "Stats.xml", SearchOption.AllDirectories);
                }
                catch
                {
                    xProfilePath.Value = Path.Combine(this.appDataPath, "StepMania 5", "Save", "LocalProfiles");
                    saveFile.Save(MainWindow.SaveFilePath);
                    return this.SelectProfile();
                }

                if (files.Length > 0)
                {
                    Dictionary<string, string> possibleProfiles = new Dictionary<string, string>();
                    foreach (string file in files)
                    {
                        XDocument xdoc = XDocument.Load(file);
                        XElement xDisplayName = xdoc.Descendants("DisplayName").FirstOrDefault();
                        if (xDisplayName != null)
                        {
                            possibleProfiles.Add(file, xDisplayName.Value);
                        }
                    }

                    if (possibleProfiles.Count > 0)
                    {
                        if (possibleProfiles.Count > 1)
                        {
                            Windows.WndProfileSelect profileSelect = new Windows.WndProfileSelect(possibleProfiles);
                            if (profileSelect.ShowDialog() == true)
                            {
                                return ((KeyValuePair<string, string>)profileSelect.lbProfiles.SelectedItem).Key;
                            }
                        }
                        else
                        {
                            return possibleProfiles.First().Key;
                        }
                    }
                }
            }

            MessageBoxResult dialogResult =
                MessageBox.Show(
                    "No profile found at the usual location.\r\nDo you want to select the correct location?\r\nThe last two folder usually are \"\\Save\\LocalProfiles\".",
                    "No profile found",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            if (dialogResult == MessageBoxResult.Yes)
            {
                FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
                if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string folderPath = folderBrowserDialog1.SelectedPath;
                    xProfilePath.Value = folderPath;
                    saveFile.Save(MainWindow.SaveFilePath);
                    return this.SelectProfile();
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the save file if necessary.
        /// </summary>
        private void CreateSaveFileIfNecessary()
        {
            if (!Directory.Exists(this.saveFileDir))
            {
                Directory.CreateDirectory(this.saveFileDir);
            }

            if (!File.Exists(MainWindow.SaveFilePath))
            {
                XDocument xdoc = new XDocument(new XElement("Data"));
                xdoc.Save(MainWindow.SaveFilePath);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnAddProfile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnAddProfile_Click(object sender, RoutedEventArgs e)
        {
            string profilePath = this.SelectProfile();
            Profile profile = this.GetProfile(profilePath);

            this.AdditionalGameplaySeconds += profile.GameplaySecondsThisYear;
            this.RefreshUiValues();
        }

        /// <summary>
        /// Handles the Click event of the btnAddManuall control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnAddManuall_Click(object sender, RoutedEventArgs e)
        {
            Windows.WndAddDataManually wnd = new Windows.WndAddDataManually();
            if (wnd.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(wnd.tbDuration.Text))
                {
                    long newGameplaySeconds = Convert.ToInt32(wnd.tbDuration.Text) * 60;
                    this.SelectedProfile.TotalGameplaySeconds += newGameplaySeconds;
                    Session newSession = new Session(wnd.dpDate.SelectedDate ?? DateTime.Now, newGameplaySeconds, 1);
                    this.SelectedProfile.Sessions.Add(newSession);
                    this.SelectedProfile.NewSessions.Add(newSession);
                    this.SelectedProfile.Save();

                    this.RefreshUiValues();
                }
            }
        }
    }
}