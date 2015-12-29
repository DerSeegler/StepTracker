namespace StepTracker.Windows
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Interaction logic for WndProfileSelect.xaml
    /// </summary>
    public partial class WndProfileSelect : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WndProfileSelect"/> class.
        /// </summary>
        /// <param name="possibleProfiles">The possible profiles.</param>
        public WndProfileSelect(Dictionary<string, string> possibleProfiles)
        {
            InitializeComponent();

            this.lbProfiles.DisplayMemberPath = "Value";
            this.lbProfiles.ItemsSource = possibleProfiles;
        }

        /// <summary>
        /// Handles the Click event of the btnOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}