namespace StepTracker.Windows
{
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for WndAddDataManually.xaml
    /// </summary>
    public partial class WndAddDataManually : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WndAddDataManually"/> class.
        /// </summary>
        public WndAddDataManually()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the PreviewTextInput event of the tbDuration control.
        /// Used to prevent anything but numbers from getting entered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextCompositionEventArgs"/> instance containing the event data.</param>
        private void tbDuration_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnAdd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Handles the KeyDown event of the tbDuration control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void tbDuration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}