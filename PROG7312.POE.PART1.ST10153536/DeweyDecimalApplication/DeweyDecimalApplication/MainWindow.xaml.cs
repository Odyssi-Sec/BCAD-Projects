using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static DeweyDecimalApplication.ReplacingBooks;

namespace DeweyDecimalApplication
{
    public partial class MainWindow : Window
    {
        private List<AttemptHistory> sharedHistoryList;

        public MainWindow()
        {
            InitializeComponent();
            sharedHistoryList = ((HistoryData)Application.Current).SharedHistoryList;



        }


      

        private void ReplacingBooks_Click(object sender, RoutedEventArgs e)
        {
            ReplacingBooks replacingBooks = new ReplacingBooks();
            MainFrame.NavigationService.Navigate(replacingBooks);
        }

        private void IdentifyingAreas_Click(object sender, RoutedEventArgs e)
        {
            IdentifyingAreas identifyingAreasPage = new IdentifyingAreas();
            MainFrame.NavigationService.Navigate(identifyingAreasPage);
        }

        private void FindingCallNumbers_Click(object sender, RoutedEventArgs e)
        {
            FindingCallNumbers findingCallNumbersPage = new FindingCallNumbers();
            MainFrame.NavigationService.Navigate(findingCallNumbersPage);
        }


        private void ClosePageButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the main window using your frame's navigation
            if (MainFrame != null)
            {
                MainFrame.Navigate(new Uri("MainWindow.xaml", UriKind.Relative));
            }
        }
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the history window
            HistoryAttemp historyWindow = new HistoryAttemp();

            // Set the data source for the ListView in the history window
            historyWindow.HistoryListView.ItemsSource = sharedHistoryList;

            // Show the history window
            historyWindow.Show();
        }

        public void SetSharedHistoryList(List<AttemptHistory> historyList)
        {
            sharedHistoryList = historyList;
            // Update the DataContext to the shared history list
        }



    }
}
