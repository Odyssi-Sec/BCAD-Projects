using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace DeweyDecimalApplication
{
    public partial class FindingCallNumbers : Page
    {
        private DeweyDecimalData deweyDecimalData;
        private int currentRound = 1;
        private int currentQuestionIndex = 0;
        private Random random = new Random();
        private SubcategoryTitle currentQuestionTitle;
        private int points = 0;
        private int correctCount = 0;
        private int incorrectCount = 0;
        private static int totalIncorrectCount = 0;

        public FindingCallNumbers()
        {
            InitializeComponent();

            deweyDecimalData = new DeweyDecimalSystemLoader().LoadDeweyDecimalData("deweydecimal.xml");

            StartRound();
        }

        private void EndGame()
        {
            MessageBox.Show("Incorrect answer. Try again.");

            totalIncorrectCount += incorrectCount;

            IncorrectTextBlock.Text = $"Incorrect: {totalIncorrectCount}";

            currentRound++;
            StartRound();
        }


        private void StartRound()
        {
            correctCount = 0;
            incorrectCount = 0;

            switch (currentRound)
            {
                case 1:
                    DisplayCategoryQuestion();
                    break;
                case 2:
                    DisplaySubcategoryQuestion();
                    break;
                case 3:
                    DisplaySubcategoryTitleQuestion();
                    break;
            }
        }


        private SubcategoryTitle GetRandomSubcategoryTitle()
        {
            Category randomCategory = deweyDecimalData.Categories[random.Next(deweyDecimalData.Categories.Count)];

            Subcategory randomSubcategory = randomCategory.Subcategories[random.Next(randomCategory.Subcategories.Count)];

            SubcategoryTitle randomTitle = randomSubcategory.SubcategoryTitles[random.Next(randomSubcategory.SubcategoryTitles.Count)];

            string originalName = randomTitle.Name;
            string originalCode = randomTitle.Code;

            randomTitle.Name = $"{originalName}";
            randomTitle.Code = $"{originalCode}";

            randomTitle.Tag = originalName;

            return randomTitle;
        }

        private void DisplayCategoryQuestion()
        {
            currentQuestionTitle = GetRandomSubcategoryTitle();

            QuestionTextBlock.Text = $"Select the category for: {currentQuestionTitle.Tag}";

            DisplayCategoriesInListBox(true);
        }

        private void DisplaySubcategoryQuestion()
        {
            QuestionTextBlock.Text = $"Select the subcategory for: {currentQuestionTitle.Tag}";

            DisplaySubcategoriesInListBox(true);
        }

        private void DisplaySubcategoryTitleQuestion()
        {
            QuestionTextBlock.Text = $"Match the subcategoryTitle: {currentQuestionTitle.Tag}";

            DisplaySubcategoryTitlesInListBox(true);
        }
        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to close this page?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
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
        private void DisplayCategoriesInListBox(bool shuffleOptions)
        {
            List<string> categoryNamesWithCode = deweyDecimalData.Categories
                .OrderBy(x => random.Next())
                .Select(c => $"{c.Code} - {c.Name}")
                .Take(3)
                .ToList();

            Category correctCategory = deweyDecimalData.Categories.FirstOrDefault(c => c.Subcategories.Any(s => s.SubcategoryTitles.Contains(currentQuestionTitle)));

            if (correctCategory != null)
            {
                categoryNamesWithCode.Add($"{correctCategory.Code} - {correctCategory.Name}");
            }

            categoryNamesWithCode = categoryNamesWithCode.Distinct().ToList();

            while (categoryNamesWithCode.Count < 4)
            {
                string randomCategory = deweyDecimalData.Categories.OrderBy(x => random.Next()).Select(c => $"{c.Code} - {c.Name}").First();
                categoryNamesWithCode.Add(randomCategory);
            }

            if (shuffleOptions)
            {
                categoryNamesWithCode = categoryNamesWithCode.OrderBy(x => random.Next()).ToList();
            }

            ListBox1.ItemsSource = categoryNamesWithCode;
        }


        private void DisplaySubcategoriesInListBox(bool shuffleOptions)
        {
            List<string> subcategoryNamesWithCode = deweyDecimalData.Categories
                .SelectMany(c => c.Subcategories)
                .OrderBy(x => random.Next())
                .Select(s => $"{s.Code} - {s.Name}")
                .Take(3)
                .ToList();

            Subcategory correctSubcategory = deweyDecimalData.Categories
                .SelectMany(c => c.Subcategories)
                .FirstOrDefault(s => s.SubcategoryTitles.Contains(currentQuestionTitle));

            if (correctSubcategory != null)
            {
                subcategoryNamesWithCode.Add($"{correctSubcategory.Code} - {correctSubcategory.Name}");
            }

            subcategoryNamesWithCode = subcategoryNamesWithCode.Distinct().ToList();

            while (subcategoryNamesWithCode.Count < 4)
            {
                string randomSubcategory = deweyDecimalData.Categories
                    .SelectMany(c => c.Subcategories)
                    .OrderBy(x => random.Next())
                    .Select(s => $"{s.Code} - {s.Name}")
                    .First();
                subcategoryNamesWithCode.Add(randomSubcategory);
            }

            if (shuffleOptions)
            {
                subcategoryNamesWithCode = subcategoryNamesWithCode.OrderBy(x => random.Next()).ToList();
            }

            ListBox1.ItemsSource = subcategoryNamesWithCode;
        }

        private void DisplaySubcategoryTitlesInListBox(bool shuffleOptions)
        {
            Subcategory correctSubcategory = deweyDecimalData.Categories
                .SelectMany(c => c.Subcategories)
                .FirstOrDefault(s => s.SubcategoryTitles.Contains(currentQuestionTitle));

            if (correctSubcategory != null)
            {
                List<SubcategoryTitle> allTitles = correctSubcategory.SubcategoryTitles
                    .Where(t => t.Code.Length == 3)
                    .ToList();

                List<string> titleNamesWithCode = allTitles
                    .OrderBy(x => random.Next())
                    .Select(t => $"{t.Code} - {t.Name}")
                    .Take(3)
                    .ToList();

                titleNamesWithCode.Add($"{correctSubcategory.SubcategoryTitles.First().Code} - {correctSubcategory.SubcategoryTitles.First().Name}");

                titleNamesWithCode = titleNamesWithCode.Distinct().ToList();

                while (titleNamesWithCode.Count < 4)
                {
                    SubcategoryTitle randomTitle = allTitles
                        .OrderBy(x => random.Next())
                        .First();

                    titleNamesWithCode.Add($"{randomTitle.Code} - {randomTitle.Name}");
                }

                if (shuffleOptions)
                {
                    titleNamesWithCode = titleNamesWithCode.OrderBy(x => random.Next()).ToList();
                }

                ListBox1.ItemsSource = titleNamesWithCode;
            }
        }



        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            switch (currentRound)
            {
                case 1:
                    CheckCategoryAnswer();
                    break;
                case 2:
                    CheckSubcategoryAnswer();
                    break;
                case 3:
                    CheckSubcategoryTitleAnswer();
                    break;
            }

            if (currentRound < 3)
            {
                currentRound++;
                StartRound();
            }
            else
            {
                MessageBox.Show($"Quiz completed! Your total points: {points}");

                CategoryTextBlock.Text = "";
                SubcategoryTextBlock.Text = "";
                SubcategoryTitleTextBlock.Text = "";

                CorrectTextBlock.Text = $"Correct: {correctCount}";
                IncorrectTextBlock.Text = $"Incorrect: {incorrectCount}";

                currentRound = 1;
                StartRound();

                SavePointsToFile();
            }
        }
        private void CheckCategoryAnswer()
        {
            if (ListBox1.SelectedItem != null)
            {
                string selectedCategory = ListBox1.SelectedItem.ToString();

                Category correctCategory = deweyDecimalData.Categories.FirstOrDefault(c => c.Subcategories.Any(s => s.SubcategoryTitles.Contains(currentQuestionTitle)));

                if (correctCategory != null)
                {
                    // Ask for confirmation before showing the incorrect message
                    MessageBoxResult result = MessageBox.Show("Are you sure of your selection?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // The user is sure of their selection
                        if (selectedCategory == $"{correctCategory.Code} - {correctCategory.Name}")
                        {
                            MessageBox.Show("Correct!");

                            CategoryTextBlock.Text = $"Category: {correctCategory.Name}";

                            points += 5;
                            PointsTextBlock.Text = $"Points: {points}";
                            correctCount++;
                        }
                        else
                        {
                            MessageBox.Show($"Incorrect. The correct category is: {correctCategory.Code} - {correctCategory.Name}");
                            EndGame();
                            incorrectCount++;
                        }
                    }
                    else
                    {
                        // The user wants to choose another option, you can add any additional logic here
                    }
                }
            }
        }


        private void CheckSubcategoryAnswer()
        {
            if (ListBox1.SelectedItem is string selectedSubcategory)
            {
                Subcategory correctSubcategory = deweyDecimalData.Categories
                    .SelectMany(c => c.Subcategories)
                    .FirstOrDefault(s => s.SubcategoryTitles.Contains(currentQuestionTitle));

                if (correctSubcategory != null)
                {
                    // Ask for confirmation before showing the incorrect message
                    MessageBoxResult result = MessageBox.Show("Are you sure of your selection?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // The user is sure of their selection
                        if (correctSubcategory.Code + " - " + correctSubcategory.Name == selectedSubcategory)
                        {
                            MessageBox.Show("Correct!");

                            SubcategoryTextBlock.Text = $"Subcategory: {correctSubcategory.Name}";
                            points += 5;
                            PointsTextBlock.Text = $"Points: {points}";
                            correctCount++;
                        }
                        else
                        {
                            MessageBox.Show($"Incorrect. The correct subcategory is: {correctSubcategory.Code} - {correctSubcategory.Name}");
                            EndGame();
                            incorrectCount++;
                        }
                    }
                    else
                    {
                        // The user wants to choose another option, you can add any additional logic here
                    }
                }
            }
        }


        private void CheckSubcategoryTitleAnswer()
        {
            if (ListBox1.SelectedItem is string selectedTitle)
            {
                SubcategoryTitle correctSubcategoryTitle = GetCurrentQuestionTitle();

                if (correctSubcategoryTitle != null)
                {
                    // Ask for confirmation before showing the incorrect message
                    MessageBoxResult result = MessageBox.Show("Are you sure of your selection?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // The user is sure of their selection
                        if (correctSubcategoryTitle.Code + " - " + correctSubcategoryTitle.Name == selectedTitle)
                        {
                            MessageBox.Show("Correct!");

                            SubcategoryTitleTextBlock.Text = $"SubcategoryTitle: {correctSubcategoryTitle.Name}";
                            points += 5;
                            PointsTextBlock.Text = $"Points: {points}";
                            correctCount++;
                        }
                        else
                        {
                            MessageBox.Show($"Incorrect. The correct subcategoryTitle is: {correctSubcategoryTitle.Code} - {correctSubcategoryTitle.Name}");
                            EndGame();
                            incorrectCount++;
                        }
                    }
                    else
                    {
                        // The user wants to choose another option, you can add any additional logic here
                    }
                }
            }
        }
        private void SavePointsToFile()
        {
            try
            {
                string content = $"Points: {points}\nCorrect: {correctCount}\nIncorrect: {incorrectCount}";
                File.WriteAllText("results.txt", content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving results: " + ex.Message);
            }
        }


        private SubcategoryTitle GetCurrentQuestionTitle()
        {
            return currentQuestionTitle;
        }




        public class DeweyDecimalSystemLoader
        {
            public DeweyDecimalData LoadDeweyDecimalData(string filePath)
            {
                DeweyDecimalData deweyDecimalData = new DeweyDecimalData();

                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);

                    XmlNodeList categoryNodes = xmlDoc.SelectNodes("/DeweyDecimalSystem/Category");
                    foreach (XmlNode categoryNode in categoryNodes)
                    {
                        string categoryCode = categoryNode.Attributes["code"].Value;
                        string categoryName = categoryNode.Attributes["name"].Value;

                        Category category = new Category(categoryCode, categoryName);

                        XmlNodeList subcategoryNodes = categoryNode.SelectNodes("Subcategory");
                        foreach (XmlNode subcategoryNode in subcategoryNodes)
                        {
                            string subcategoryCode = subcategoryNode.Attributes["code"].Value;
                            string subcategoryName = subcategoryNode.Attributes["name"].Value;

                            Subcategory subcategory = new Subcategory(subcategoryCode, subcategoryName);

                            XmlNodeList subcategoryTitleNodes = subcategoryNode.SelectNodes("SubcategoryTitle");
                            foreach (XmlNode subcategoryTitleNode in subcategoryTitleNodes)
                            {
                                string titleCode = subcategoryTitleNode.Attributes["code"].Value;
                                string titleName = subcategoryTitleNode.Attributes["name"].Value;

                                SubcategoryTitle subcategoryTitle = new SubcategoryTitle(titleCode, titleName);

                                subcategory.AddSubcategoryTitle(subcategoryTitle);

                                category.AddSubcategory(subcategory);
                            }
                        }

                        deweyDecimalData.AddCategory(category);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                return deweyDecimalData;
            }
        }
    }

}
