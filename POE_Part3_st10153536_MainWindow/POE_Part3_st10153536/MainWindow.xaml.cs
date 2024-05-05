using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POE_Part3_st10153536
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
 
        public partial class MainWindow : Window
        {
            Rent rent = new Rent();
            Property property = new Property();
           

            public static List<double> myList = new List<double>();

            double incomeTemp, income, tax, grocery, wal, travel, phone, other, total;

            public MainWindow()
            {
                InitializeComponent();
                bproperty.Visibility = Visibility.Collapsed;
                brent.Visibility = Visibility.Collapsed;
            
            }

            private void rbPropety_Checked(object sender, RoutedEventArgs e)
            {
                bproperty.Visibility = Visibility.Visible;
            }

            private void rbPropety_UnChecked(object sender, RoutedEventArgs e)
            {
                bproperty.Visibility = Visibility.Collapsed;
            }

            private void rbRentChecked(object sender, RoutedEventArgs e)
            {
                brent.Visibility = Visibility.Visible;
            }

            private void rbRentUnchecked(object sender, RoutedEventArgs e)
            {
                brent.Visibility = Visibility.Collapsed;
            }

          
            private void bpropertyClicked(object sender, RoutedEventArgs e)
            {
                property.Show();
            }

            private void brentClicked(object sender, RoutedEventArgs e)
            {
                rent.Show();
            }

           
            private void bsavingClicked(object sender, RoutedEventArgs e)
            {
                Savings savings = new Savings();
                savings.Show();
            }

            private void bsubmit(object sender, RoutedEventArgs e)
            {
                try
                {
                    incomeTemp = Convert.ToDouble(tbincome.Text);
                    tax = Convert.ToDouble(tbtax.Text);
                    myList.Add(grocery = Convert.ToDouble(tbgrocery.Text));
                    myList.Add(wal = Convert.ToDouble(tbwaterLight.Text));
                    myList.Add(travel = Convert.ToDouble(tbtravel.Text));
                    myList.Add(phone = Convert.ToDouble(tbphone.Text));
                    myList.Add(other = Convert.ToDouble(tbother.Text));
                    myList.Add(tax);
                    myList.Add(rent.getRent());
                    myList.Add(property.getAProperty());
                    

                    total = myList.Sum();

                    income = incomeTemp - total;

                    MessageBox.Show("INCOME for the month: " + incomeTemp + "\n" +
                        "RENT amount: " + rent.getRent() + "\n" +
                        "PROPERTY amount: " + property.getAProperty() + "\n" +
                        "Money left over after other expenses: " + income);


                }
                catch (FormatException ex)
                {
                MessageBox.Show("Numbers Only");
                }
            }

        }
    }

