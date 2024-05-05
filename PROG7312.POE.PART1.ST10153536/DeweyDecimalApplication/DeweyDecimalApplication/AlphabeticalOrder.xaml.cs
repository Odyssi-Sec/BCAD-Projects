using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using static DeweyDecimalApplication.HistoryAttemp;
using static DeweyDecimalApplication.ReplacingBooks;

namespace DeweyDecimalApplication
{
    public partial class AlphabeticalOrder : Page
    {

        private List<string> userCallNumbersOrdered = new List<string>();

        public List<string> UserOrderedNumericalCallNumbers = new List<string>();
        private List<string> hintOrderCopy = new List<string>();




        private List<string> userOrderedAlphabeticalCallNumbers = new List<string>();
        private List<AttemptHistory> attemptHistoryList = new List<AttemptHistory>();
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;
        private int correctOrderCount = 0;
        private int hintUsageCount = 0;
        private List<AttemptHistory> sharedHistoryList;
        private int correctOrderPosition = 0;

        public IEnumerable AttemptHistoryList { get; private set; }

        public AlphabeticalOrder(string[] orderedCodesText, List<AttemptHistory> attemptHistoryList)
        {
            InitializeComponent();
            //
            //();
            timer = new DispatcherTimer();
            DataContext = this;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            userOrderedAlphabeticalCallNumbers = orderedCodesText.OrderBy(cn => cn).ToList();
            hintOrderCopy = new List<string>(userOrderedAlphabeticalCallNumbers);
            NumericalListBox.ItemsSource = userOrderedAlphabeticalCallNumbers;
            UserOrderedNumericalCallNumbers = new List<string>(userCallNumbersOrdered);
            NumericalListBox.ItemsSource = null;
            NumericalListBox.ItemsSource = UserOrderedNumericalCallNumbers;
            LoadOrderedNumericalCallNumbers();
            DisplayOrderedCallNumbers();
            timer.Start();
            AttemptHistoryList = ((HistoryData)Application.Current).SharedHistoryList;






            if (orderedCodesText != null && orderedCodesText.Length > 0)
            {
                UserOrderedNumericalCallNumbers.AddRange(orderedCodesText);
                NumericalListBox.ItemsSource = UserOrderedNumericalCallNumbers;
            }

            DisplayOrderedCallNumbers();

        }

        private void LoadOrderedNumericalCallNumbers()
        {
            try
            {

                {
                    if (File.Exists("OrderedCodes.txt"))
                    {
                        string orderedCodesText = File.ReadAllText("OrderedCodes.txt");
                        userCallNumbersOrdered.AddRange(orderedCodesText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading ordered codes: {ex.Message}");
            }
        }

        private void DisplayOrderedCallNumbers()
        {
            if (NumericalListBox != null)
            {
                NumericalListBox.ItemsSource = null;
                NumericalListBox.Items.Clear();
                NumericalListBox.ItemsSource = UserOrderedNumericalCallNumbers;
            }
        }


        private void TransferButton2_Click(object sender, RoutedEventArgs e)
        {
            if (NumericalListBox.SelectedItem != null)
            {
                string selectedItem = NumericalListBox.SelectedItem.ToString();

                if (selectedItem != null)
                {
                    // Add the selected item to the AlphabeticalListBox
                    AlphabeticalListBox.Items.Add(selectedItem);

                    // Remove the selected item from the NumericalListBox's ItemsSource
                    NumericalListBox.ItemsSource = null;
                    UserOrderedNumericalCallNumbers.Remove(selectedItem);
                    NumericalListBox.ItemsSource = UserOrderedNumericalCallNumbers;

                    // Check if the transferred code is in the correct order
                    if (CheckAlphabeticalOrder(AlphabeticalListBox.Items.Cast<string>().ToList()))
                    {
                        // All transferred codes are in the correct order
                        correctOrderCount = AlphabeticalListBox.Items.Count;
                    }
                    UpdateProgressBar();
                    UpdateOrderedCodesFile();
                }
            }
        }






        private void TransferBackButton2_Click(object sender, RoutedEventArgs e)
        {
            if (AlphabeticalListBox.SelectedItem != null)
            {
                string selectedItem = AlphabeticalListBox.SelectedItem.ToString();

                if (selectedItem != null)
                {
                    userOrderedAlphabeticalCallNumbers.Remove(selectedItem);
                    UserOrderedNumericalCallNumbers.Add(selectedItem);
                    userCallNumbersOrdered.Add(selectedItem);

                    AlphabeticalListBox.Items.Remove(selectedItem);

                    NumericalListBox.ItemsSource = null;
                    UserOrderedNumericalCallNumbers = UserOrderedNumericalCallNumbers;
                    NumericalListBox.ItemsSource = UserOrderedNumericalCallNumbers;
                    correctOrderCount--;
                    UpdateProgressBar();
                    UpdateOrderedCodesFile();
                }
            }
        }
        private void UpdateOrderedCodesFile()
        {
            try
            {
                string orderedCodesText = string.Join(Environment.NewLine, UserOrderedNumericalCallNumbers);
                File.WriteAllText("OrderedCodes.txt", orderedCodesText);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the ordered codes file: {ex.Message}");
            }
        }






        private void CheckAlphabeticalButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the items from the AlphabeticalListBox
            List<string> itemsInAlphabeticalListBox = AlphabeticalListBox.Items.Cast<string>().ToList();

            bool isAlphabeticalCorrect = CheckAlphabeticalOrder(itemsInAlphabeticalListBox);
            AttemptHistory attempt = new AttemptHistory
            {
                CallNumbers = string.Join(", ", itemsInAlphabeticalListBox),
                IsCorrect = isAlphabeticalCorrect ? "Yes" : "No",
                ElapsedTime = elapsedTime,
                Timestamp = DateTime.Now
            };
            attemptHistoryList.Add(attempt);

            SaveHistoryToFile();

            if (isAlphabeticalCorrect)
            {
                MessageBox.Show("Congratulations! The codes are in alphabetical order.");
                UpdateOrderedCodesFile();
            }
            else
            {
                MessageBox.Show("Incorrect! Please arrange the codes in alphabetical order and try again.");
            }
        }


        private void SaveHistoryToFile()
        {
            try
            {
                // Serialize the history to a file (e.g., JSON format)
                string historyJson = JsonConvert.SerializeObject(attemptHistoryList);
                File.WriteAllText("AttemptHistory.json", historyJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the history: {ex.Message}");
            }
        }

        private void UpdateProgressBar()
        {

            // Ensure the progress value doesn't exceed the maximum
            ProgressBar.Value = Math.Min(correctOrderCount, ProgressBar.Maximum);
        }





        private bool CheckAlphabeticalOrder(List<string> itemsInAlphabeticalListBox)
        {
            // Clone and sort the items from the AlphabeticalListBox using the same logic as the hint
            List<string> sortedUserOrder = new List<string>(itemsInAlphabeticalListBox);
            sortedUserOrder.Sort((code1, code2) =>
            {
                string part1 = GetAlphabeticalPart(code1);
                string part2 = GetAlphabeticalPart(code2);
                return string.Compare(part1, part2, StringComparison.Ordinal);
            });

            // Check if the sorted user order matches the hint order
            for (int i = 0; i < sortedUserOrder.Count; i++)
            {
                if (sortedUserOrder[i] != hintOrderCopy[i])
                {
                    return false;
                }
            }

            return true;
        }


        private string GetAlphabeticalPart(string code)
        {
            string[] parts = code.Split('.');
            if (parts.Length >= 2 && parts[1].Length > 2)
            {
                return parts[1].Substring(2);
            }
            return string.Empty;
        }

        private void HintButton_Click1(object sender, RoutedEventArgs e)
        {
            if (hintUsageCount < 3)
            {
                // Sort the hintOrderCopy list based on the same logic as CheckAlphabeticalOrder
                hintOrderCopy.Sort((code1, code2) =>
                {
                    string part1 = GetAlphabeticalPart(code1);
                    string part2 = GetAlphabeticalPart(code2);
                    return string.Compare(part1, part2, StringComparison.Ordinal);
                });

                // Display the hint order in a message box
                string hintMessage = $"Correct Alphabetical Order:\n{string.Join("\n", hintOrderCopy)}\n\n{hintUsageCount + 1} Hint(s) used.";
                MessageBox.Show(hintMessage, "Hint");

                // Increment the hint usage count
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



        private void Timer_Tick(object? sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            TimerTextBlock.Text = $"Time: {elapsedTime:mm\\:ss}";
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


        private void ClearSelectionsButton_Click(object sender, RoutedEventArgs e)
        {
            // Ask the user for confirmation
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear the selections?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // User confirmed, clear the selections
                foreach (string item in AlphabeticalListBox.Items.OfType<string>().ToList())
                {
                    userOrderedAlphabeticalCallNumbers.Remove(item);
                    UserOrderedNumericalCallNumbers.Add(item);
                }

                // Clear the AlphabeticalListBox
                AlphabeticalListBox.Items.Clear();
                correctOrderCount = 0;

                // Update the NumericalListBox
                DisplayOrderedCallNumbers();

                // Reset ProgressBar
                ProgressBar.Value = 0;
            }
            // If the user selects "No" in the confirmation dialog, do nothing
        }


        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            ShowHistoryWindow();
            // Pass the updated shared history list to the MainWindow
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.SetSharedHistoryList(sharedHistoryList);
            }
        }


        private void ResetTimer()
        {
            timer.Stop();
            elapsedTime = TimeSpan.Zero;
            TimerTextBlock.Text = "Time: 00:00";
        }

        private void ShowHistoryWindow()
        {
            HistoryAttemp historyAttemp = new HistoryAttemp();
            historyAttemp.HistoryListView.ItemsSource = AttemptHistoryList;
            historyAttemp.ShowDialog();



        }
    }
}

