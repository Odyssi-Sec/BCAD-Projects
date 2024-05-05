using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace DeweyDecimalApplication
{
    public partial class IdentifyingAreas : Page
    {
        private Dictionary<string, string> DeweyDecimalCategories = new Dictionary<string, string>
        {
            { "000", "Generalities" },
            { "100", "Philosophy & Psychology" },
            { "200", "Religion" },
            { "300", "Social Sciences" },
            { "400", "Language" },
            { "500", "Natural Sciences & Mathematics" },
            { "600", "Technology" },
            { "700", "Arts & Recreation" },
            { "800", "Literature" },
            { "900", "History & Geography" }
        };

        private List<ListItem> leftItems = new List<ListItem>();
        private List<string> rightItems = new List<string>();
        private int attempts = 0;
        private bool matchingDescriptions = true;
        private int correctMatches = 0;
        private int incorrectMatches = 0;
        private int totalQuestions = 0;
        int totalCorrectMatches = 0;
        int totalIncorrectMatches = 0;
        int totalRoundsCompleted = 0;
        private Random random = new Random();
        private List<string> correctMatchesList = new List<string>();
        private List<string> incorrectMatchesList = new List<string>();
        public IdentifyingAreas()
        {
            InitializeComponent();
            InitializeUI();
            ShowObjectiveMessageBox();

        }

        private void InitializeUI()
        {
            // Initialize the left and right list boxes
            GenerateNewQuestion();


            // Add gamification feature (e.g., keeping track of points)
            UpdatePoints(0);
        }

        private void ShowObjectiveMessageBox()
        {
            string objectiveText = "Objective: Identify Dewey Decimal Categories\n\n";
            objectiveText += "In this activity, you will be presented with Dewey Decimal numbers and categories. Your goal is to match the Dewey Decimal numbers with their corresponding categories.\n\n";
            objectiveText += "Steps:\n";
            objectiveText += "1. Examine the Dewey Decimal categories and numbers provided on the page.\n";
            objectiveText += "2. Select a Dewey Decimal number (left) and match it with the correct category (right).\n";
            objectiveText += "3. Continue matching until you have correctly matched 4 pairs to earn points.\n";
            objectiveText += "4. The codes and categories will switch every 4th attempt, so be prepared for new challenges.\n";
            objectiveText += "5. Once you have matched 4 pairs, you can choose to proceed to the next question or reset the game.\n\n";
            objectiveText += "Have fun learning the Dewey Decimal System!\n\n";

            string dictionaryText = "Dewey Decimal Dictionary:\n";
            foreach (var entry in DeweyDecimalCategories)
            {
                dictionaryText += $"{entry.Key} - {entry.Value}\n";
            }

            string message = objectiveText + dictionaryText;

            MessageBox.Show(message, "Objective and Dewey Decimal Dictionary", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GenerateNewQuestion()
        {
            // Show a message box with the user's current correct and incorrect matches


            // Reset the number of attempts for each new question
            attempts = 0;

            leftItems.Clear();
            rightItems.Clear();

            if (totalQuestions % 2 == 0)
            {
                // Match descriptions to call numbers
                var randomCategories = DeweyDecimalCategories.Values.OrderBy(x => random.Next()).Take(4).ToList();

                foreach (var category in randomCategories)
                {
                    leftItems.Add(new ListItem
                    {
                        ItemText = DeweyDecimalCategories.FirstOrDefault(x => x.Value == category).Key,
                        BackgroundColor = GetRandomBackgroundColor()
                    });
                }

                // Include the correct descriptions
                rightItems.AddRange(randomCategories);

                // Include three random descriptions
                var randomDescriptions = DeweyDecimalCategories.Values.Except(randomCategories).OrderBy(x => random.Next()).Take(3);
                rightItems.AddRange(randomDescriptions);

                leftListBox.ItemsSource = leftItems.OrderBy(x => random.Next());
                rightListBox.ItemsSource = rightItems.OrderBy(x => random.Next());
            }
            else
            {
                // Match call numbers to descriptions
                var randomCallNumbers = DeweyDecimalCategories.Keys.OrderBy(x => random.Next()).Take(4).ToList();

                foreach (var callNumber in randomCallNumbers)
                {
                    leftItems.Add(new ListItem
                    {
                        ItemText = DeweyDecimalCategories[callNumber],
                        BackgroundColor = GetRandomBackgroundColor()
                    });
                }

                // Include the correct call numbers
                rightItems.AddRange(randomCallNumbers);

                // Include three random call numbers
                var randomOtherCallNumbers = DeweyDecimalCategories.Keys.Except(randomCallNumbers).OrderBy(x => random.Next()).Take(3);
                rightItems.AddRange(randomOtherCallNumbers);

                leftListBox.ItemsSource = leftItems.OrderBy(x => random.Next());
                rightListBox.ItemsSource = rightItems;
            }

            totalQuestions++;
        }
        private SolidColorBrush[] availableColors = new SolidColorBrush[]
            {
                Brushes.LightBlue,
                Brushes.LightGreen,
                Brushes.LightPink,
                Brushes.LightSeaGreen
            };

        private List<SolidColorBrush> usedColors = new List<SolidColorBrush>();

        private SolidColorBrush GetRandomBackgroundColor()
        {
            if (availableColors.Length == 0)
            {
                // All available colors have been used, reset the list of used colors
                usedColors.Clear();
                availableColors = new SolidColorBrush[]
                {
            Brushes.LightBlue,
            Brushes.LightGreen,
            Brushes.LightPink,
            Brushes.LightSeaGreen
                };
            }

            // Randomly select a color from the available colors
            int index = random.Next(availableColors.Length);
            SolidColorBrush color = availableColors[index];

            // Remove the selected color from available colors and add it to used colors
            availableColors = availableColors.Where((c, i) => i != index).ToArray();
            usedColors.Add(color);

            return color;
        }


        private string selectedLeftItem = null;
        private void leftListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (attempts >= 4)
            {
                GenerateNewQuestion();
                return;
            }

            if (leftListBox.SelectedIndex >= 0)
            {
                selectedLeftItem = (leftListBox.SelectedItem as ListItem)?.ItemText; // Get the ItemText property

                // Update the background color of the selected item in the left ListBox
                leftListBox.ItemContainerGenerator.StatusChanged += (s, arg) =>
                {
                    if (leftListBox.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                    {
                        ListBoxItem selectedItem = leftListBox.ItemContainerGenerator.ContainerFromItem(leftListBox.SelectedItem) as ListBoxItem;
                        if (selectedItem != null)
                        {
                            selectedItem.Background = Brushes.LightBlue;

                            // Change the background color of the matching item in the right ListBox
                            int rightIndex = rightListBox.Items.IndexOf(selectedLeftItem);
                            if (rightIndex >= 0)
                            {
                                ListBoxItem rightItem = rightListBox.ItemContainerGenerator.ContainerFromItem(selectedLeftItem) as ListBoxItem;
                                if (rightItem != null)
                                {
                                    rightItem.Background = Brushes.LightBlue;
                                }
                            }
                        }
                    }
                };
            }
        }


        private void rightListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (attempts >= 4)
            {
                CheckAndHighlightMatches();
                return;
            }

            if (rightListBox.SelectedItem != null && selectedLeftItem != null)
            {
                // Check if the match is correct based on the dictionary
                string rightItemText = (rightListBox.SelectedItem as string);

                if (rightItemText != null)
                {
                    bool isCorrect = CheckMatch(selectedLeftItem, rightItemText);

                    if (isCorrect)
                    {
                        correctMatches++;
                        correctMatchesList.Add($"{selectedLeftItem} - {rightItemText}");

                        // Create a new TextBox for the correct match and add it to the CorrectStackPanel
                        TextBox correctTextBox = new TextBox();
                        correctTextBox.Text = $"{selectedLeftItem} - {rightItemText}";
                        CorrectStackPanel.Children.Add(correctTextBox);
                    }
                    else
                    {
                        incorrectMatches++;
                        incorrectMatchesList.Add($"{selectedLeftItem} - {rightItemText}");

                        // Create a new TextBox for the incorrect match and add it to the IncorrectStackPanel
                        TextBox incorrectTextBox = new TextBox();
                        incorrectTextBox.Text = $"{selectedLeftItem} - {rightItemText}";
                        IncorrectStackPanel.Children.Add(incorrectTextBox);
                    }
                    // Set the background color of the matched items in both ListBoxes
                    ListBoxItem leftItem = leftListBox.ItemContainerGenerator.ContainerFromItem(leftListBox.SelectedItem) as ListBoxItem;
                    ListBoxItem rightItem = rightListBox.ItemContainerGenerator.ContainerFromItem(rightListBox.SelectedItem) as ListBoxItem;

                    if (rightItem != null)
                    {
                        // Set the background color for the right ListBox to match the left ListBox's background color
                        SolidColorBrush leftItemBackgroundColor = (leftListBox.SelectedItem as ListItem).BackgroundColor;
                        rightItem.Background = isCorrect ? leftItemBackgroundColor : Brushes.Red;
                    }

                    // Reset the selected items
                    leftListBox.SelectedItem = null;
                    rightListBox.SelectedItem = null;
                    selectedLeftItem = null; 
                    attempts++;
                    UpdatePoints(correctMatches);
                    CorrectTextBlock.Text = "Correct: ";
                    IncorrectTextBlock.Text = "Incorrect: ";

                    if (attempts == 4) 
                    {
                        CheckAndHighlightMatches(); 
                    }
                }
            }
        }

        private void CheckAndHighlightMatches()
        {
            if (attempts < 4)
            {
                MessageBox.Show("You must complete 4 matches before checking the results.", "Incomplete Matches", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool allCorrectInRound = correctMatches == 4;

            totalCorrectMatches += correctMatches; 
            totalIncorrectMatches += incorrectMatches;

            totalRoundsCompleted++;

            if (totalQuestions % 2 == 0)
            {
                if (allCorrectInRound)
                {
                    MessageBox.Show("Congrats! All matches are correct.", "Congratulations!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"You got {correctMatches} correct and {incorrectMatches} incorrect matches.", "Match Results", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                // Odd round - matching call numbers to descriptions
                MessageBox.Show($"You got {correctMatches} correct and {incorrectMatches} incorrect matches.", "Match Results", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Reset the selected items and generate a new question
            leftListBox.SelectedItem = null;
            rightListBox.SelectedItem = null;
            selectedLeftItem = null;
            correctMatchesList.Clear();
            incorrectMatchesList.Clear();
            attempts = 0;
            correctMatches = 0;
            incorrectMatches = 0;

            var result = MessageBox.Show("You have completed 4 matches. Do you want to proceed to the next question or end the game?", "Next Question or End Game", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                string feedbackMessage;
                if (allCorrectInRound)
                {
                    feedbackMessage = "Well done! You got all matches correct.";
                }
                else
                {
                    feedbackMessage = $"You got {totalIncorrectMatches} incorrect. Better luck next time!";
                }

                MessageBox.Show($"Final Score: {totalCorrectMatches} correct out of {totalRoundsCompleted * 4}.\n\n{feedbackMessage}", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                ClosePage();
            }
            else
            {
                // Proceed with switching questions
                GenerateNewQuestion();
            }
        }
        private void ClosePage()
        {
            // Call the same code as in the BackToMainWindow_Click function
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
        }



        private void BackToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the MainWindow
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

            }
        }


        public class ListItem
        {
            public string ItemText { get; set; }
            public SolidColorBrush BackgroundColor { get; set; }
        }



        private void ResetGame()
        {
            // You can add any game reset logic here.
            // For example, reset points and start the game again.
            correctMatches = 0;
            UpdatePoints(correctMatches);
            GenerateNewQuestion();
        }

        private bool CheckMatch(string leftItem, string rightItem)
        {
            if (DeweyDecimalCategories.ContainsKey(leftItem))
            {
                // Left item is a call number, so we should look for its description in the rightItem
                string correctDescription = DeweyDecimalCategories[leftItem];
                return rightItem.Trim() == correctDescription.Trim();
            }
            else if (DeweyDecimalCategories.ContainsValue(leftItem))
            {
                // Left item is a description, so we should look for its call number in the rightItem
                var correctMatch = DeweyDecimalCategories.FirstOrDefault(x => x.Value == leftItem);
                if (!string.IsNullOrEmpty(correctMatch.Key))
                {
                    return rightItem.Trim() == correctMatch.Key.Trim();
                }
            }
            // Handle the case when no match was found
            return false;
        }

        private void UpdatePoints(int points)
        {
            PointsTextBlock.Text = "Points: " + points;
        }
    }
}