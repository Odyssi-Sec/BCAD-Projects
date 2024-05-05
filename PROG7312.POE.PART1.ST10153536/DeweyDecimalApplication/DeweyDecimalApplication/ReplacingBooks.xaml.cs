using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace DeweyDecimalApplication
{
    public partial class ReplacingBooks : Page, INotifyPropertyChanged
    {
        private List<string> generatedCallNumbers = new List<string>();
        private List<string> userCallNumbersOrdered = new List<string>();      
        private List<AttemptHistory> attemptHistoryList = new List<AttemptHistory>();
        private List<string> userOrderedCallNumbers = new List<string>();
        private List<string> userOrderedAlphabeticalCallNumbers = new List<string>(); 
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        public event PropertyChangedEventHandler? PropertyChanged;
        private bool hintUsed = false;
        private int hintUsageCount = 0;
        public List<string> UserCallNumbersOrdered
        {
            get { return userCallNumbersOrdered; }
            set
            {
                if (userCallNumbersOrdered != value)
                {
                    userCallNumbersOrdered = value;
                    OnPropertyChanged(nameof(UserCallNumbersOrdered));
                }
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ReplacingBooks()
        {
            InitializeComponent();
            //ReinforceImagePaths();
            userOrderedCallNumbers = new List<string>();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            generatedCallNumbers = GenerateDeweyDecimalCallNumbers(10);
            userCallNumbersOrdered.Clear();
            DisplayGeneratedCallNumbers();
            userOrderedCallNumbers.Clear();

            // Disable Generate button and show message
            GenerateButton.IsEnabled = false;
            MessageBox.Show("Please arrange the codes in numerical ascending order.");

            // Clear attempt history and reset timer
            attemptHistoryList.Clear();
            ResetTimer();

            // Start the timer
            timer.Start();
        }
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            ShowHistoryWindow();
        }
        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            if (GeneratedListBox.SelectedItem != null)
            {
                string? selectedItem = GeneratedListBox.SelectedItem.ToString();

                if (selectedItem != null)
                {
                    userOrderedCallNumbers.Add(selectedItem);
                    GeneratedListBox.Items.Remove(selectedItem);
                    SelectedCallNumbersListBox.Items.Add(selectedItem);
                    UpdateProgressBar();
                }
            }
        }
        private void TransferBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCallNumbersListBox.SelectedItem != null)
            {
                string? selectedItem = SelectedCallNumbersListBox.SelectedItem.ToString();
                SelectedCallNumbersListBox.Items.Remove(selectedItem);
                GeneratedListBox.Items.Add(selectedItem);

                UpdateProgressBar(); 
            }
        }
        private void ClearSelectionsButton_Click(object sender, RoutedEventArgs e)
        {
            // Display a confirmation message box
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // User clicked "Yes," so proceed with clearing the selections

                foreach (string callNumber in SelectedCallNumbersListBox.Items.Cast<string>())
                {
                    GeneratedListBox.Items.Add(callNumber);
                }

                SelectedCallNumbersListBox.Items.Clear();
                userOrderedCallNumbers.Clear();

                ProgressBar.Value = 0;
                GenerateButton.IsEnabled = true;
                NumericalCheckButton.Visibility = Visibility.Visible;
                ResetTimer();
            }
            else
            {
                // User clicked "No" or closed the message box, do nothing
            }
        }


        private void CheckOrderButton_Click(object sender, RoutedEventArgs e)
                {
                    bool isNumericalCorrect = CheckNumericalOrder();
                    bool isCorrectAlphabetical = userOrderedAlphabeticalCallNumbers.SequenceEqual(generatedCallNumbers.OrderBy(cn => cn));
                    string message = "";
                    if (isNumericalCorrect)
                    {
                        message = "Congratulations! Both arrangements are correct.";
                         AlphabeticalOrder alphabeticalOrderPage = new AlphabeticalOrder(userOrderedCallNumbers.ToArray(), attemptHistoryList);
                         NavigationService.Navigate(alphabeticalOrderPage);
                         userOrderedCallNumbers.Clear();
                     }
                    else if (!isNumericalCorrect)
                    {
                        message = "Oops! Please check your arrangements and try again.";
                    }
                    else if (!isNumericalCorrect)
                    {
                        message = "Oops! Please check your numerical arrangement and try again.";
                    }                 
                    AttemptHistory attempt = new AttemptHistory
                    {
                        CallNumbers = string.Join(", ", userOrderedCallNumbers),
                        IsCorrect = message.Contains("Congratulations") ? "Yes" : "No",
                        ElapsedTime = elapsedTime,
                        HintUsed = hintUsed ? "Yes" : "No" 
                    };
                    attemptHistoryList.Add(attempt);
                    CustomMessageBox customMessageBox = new CustomMessageBox(message, message.Contains("Congratulations"), generatedCallNumbers);
                    customMessageBox.ShowDialog();
                    ShowHistoryWindow();
                    ResetUI();
                    ResetTimer();
        }
                   private void NumericalCheckButton_Click(object sender, RoutedEventArgs e)
                    {
                        if (SelectedCallNumbersListBox.Items.Count == 0)
                     {
                             MessageBox.Show("No codes to check. Please transfer codes to arrange in numerical order.");
                             return;
                        }

                         bool isNumericalCorrect = CheckNumericalOrder();

                          if (isNumericalCorrect)
                                         {
                         // Proceed to order the arranged codes into alphabetical order.
                        MessageBox.Show("Correct! Proceed to order the arranged codes into alphabetical order.");
                        NumericalCheckButton.Visibility = Visibility.Collapsed;

                        // Update userCallNumbersOrdered with items from SelectedCallNumbersListBox
                        userCallNumbersOrdered = SelectedCallNumbersListBox.Items.Cast<string>().ToList();

                        // Clear current arrangement
                        SelectedCallNumbersListBox.Items.Clear();

                        // Reset progress bar
                        ProgressBar.Value = 0;


                        // Navigate to the next page before clearing the list
                        AlphabeticalOrder alphabeticalOrderPage = new(userCallNumbersOrdered.ToArray(), attemptHistoryList);
                        NavigationService.Navigate(alphabeticalOrderPage);

                        // Now you can clear the list
                        userCallNumbersOrdered.Clear();

                          }
                     else
                    {
                        MessageBox.Show("Incorrect! Please arrange the call numbers in numerical ascending order and try again.");
                        ResetTimer();
                    }
                }
        private void UpdateProgressBar()
        {
            int correctCount = 0;
            for (int i = 0; i < userOrderedCallNumbers.Count; i++)
            {
                if (userOrderedCallNumbers[i] == generatedCallNumbers.OrderBy(cn => cn).ElementAt(i))
                {
                    correctCount++;
                }
            }
            ProgressBar.Value = correctCount;
        }
        public class AttemptHistory
        {
            public string? CallNumbers { get; set; }
            public string? IsCorrect { get; set; }
            public TimeSpan ElapsedTime { get; set; }
            public DateTime Timestamp { get; set; }
            public string HintUsed { get; set; }
        }
        private bool CheckNumericalOrder()
        {
            List<int> orderedUserNumericParts = SelectedCallNumbersListBox.Items
                .Cast<string>()
                .Select(cn => int.Parse(new string(cn.Where(char.IsDigit).ToArray())))
                .ToList();

            bool isCorrectOrder = true;
            for (int i = 1; i < orderedUserNumericParts.Count; i++)
            {
                if (orderedUserNumericParts[i] < orderedUserNumericParts[i - 1])
                {
                    isCorrectOrder = false;
                    break;
                }
            }

            if (isCorrectOrder)
            {
                MessageBox.Show("Correct! The call numbers are in numerical ascending order.");
            }
            else
            {
                MessageBox.Show("Incorrect! Please arrange the call numbers in numerical ascending order and try again.");
                ResetTimer();
            }

            return isCorrectOrder;
        }    
        private void ShowHistoryWindow()
        {
            HistoryAttemp historyAttemp = new HistoryAttemp();
            historyAttemp.HistoryListView.ItemsSource = attemptHistoryList;
            historyAttemp.ShowDialog();
            userOrderedAlphabeticalCallNumbers = userOrderedCallNumbers.OrderBy(cn => cn).ToList();
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            TimerTextBlock.Text = $"Time: {elapsedTime:mm\\:ss}";
        }
        private void ResetTimer()
        {
            timer.Stop();
            elapsedTime = TimeSpan.Zero;
            TimerTextBlock.Text = "Time: 00:00";
        }
        private List<string> GenerateDeweyDecimalCallNumbers(int count)
        {
            Random random = new Random();
            List<string> callNumbers = new List<string>();

            for (int i = 0; i < count; i++)
            {
                // Generate a whole number between 000 and 999 (Dewey Decimal categories).
                string wholeNumber = random.Next(0, 1000).ToString("D3");

                // Generate a decimal fraction between 0.00 and 0.99 (Dewey Decimal subdivisions).
                string decimalFraction = $"{random.Next(0, 100):D2}";

                // Generate three author initials (e.g., JAM).
                string authorInitials = $"{(char)random.Next('A', 'Z' + 1)}" +
                                        $"{(char)random.Next('A', 'Z' + 1)}" +
                                        $"{(char)random.Next('A', 'Z' + 1)}";

                // Combine the whole number, decimal fraction, and author initials to create the Dewey Decimal call number.
                string deweyDecimalCallNumber = $"{wholeNumber}.{decimalFraction} {authorInitials}";

                callNumbers.Add(deweyDecimalCallNumber);
            }

            return callNumbers;
        }


        private void DisplayGeneratedCallNumbers()
        {
            GeneratedListBox.Items.Clear();
            foreach (string callNumber in generatedCallNumbers)
            {
                GeneratedListBox.Items.Add(callNumber);
            }
        }
        private void ClosePageButton_Click(object sender, RoutedEventArgs e)
        {
            // Display a confirmation message box
            MessageBoxResult result = MessageBox.Show("Are you sure you want to close this page?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // User clicked "Yes," so proceed with closing the page
                this.Visibility = Visibility.Hidden;
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.Show();
                }
            }
            else
            {
                // User clicked "No" or closed the message box, do nothing
            }
        }

        //private void ReinforceImagePaths()
        //{
        //    string basePath = "C:\\Users\\Josh\\source\\repos\\DeweyDecimalApplication\\DeweyDecimalApplication\\Images\\";
        //    BitmapImage transferImage = new BitmapImage(new Uri(basePath + "transfer.png", UriKind.Absolute));
        //    BitmapImage transferBackImage = new BitmapImage(new Uri(basePath + "transferBack.png", UriKind.Absolute));
        //    TransferButton.Content = new Image { Source = transferImage, Width = 30, Height = 30 };
        //    TransferBackButton.Content = new Image { Source = transferBackImage, Width = 30, Height = 30 };            
        //}
        private void HintButton_Click(object sender, RoutedEventArgs e)
        {
            if (hintUsageCount < 3)
            {
                // Provide a hint to the user by displaying the correct order of the generated codes.
                string correctOrder = string.Join("\n", generatedCallNumbers.OrderBy(cn => cn));
                MessageBox.Show($"Hint: Try arranging the call numbers in the following order:\n{correctOrder}");

                // Increment the hint usage count and update the hint count text
                hintUsageCount++;
                UpdateHintCount();
            }
            else
            {
                MessageBox.Show("You have used the hint three times. No more hints allowed.");
            }
        }


        private void UpdateHintCount()
        {
            int hintsLeft = 3 - hintUsageCount;
            HintCountTextBlock.Text = $"Hints Left: {hintsLeft}";
        }
        private void ResetUI()
        {
            GeneratedListBox.Items.Clear();
            SelectedCallNumbersListBox.Items.Clear();
            userOrderedCallNumbers.Clear();
        }
    }
}
