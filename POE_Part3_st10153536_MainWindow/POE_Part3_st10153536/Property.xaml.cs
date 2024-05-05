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
using System.Windows.Shapes;

namespace POE_Part3_st10153536
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Property : Window
    {
        double P = 0, deposit = 0, i = 0, n = 0, A = 0;

        public Property()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private void bpropertySubmit(object sender, RoutedEventArgs e)
        {
            try
            {
                P = Convert.ToDouble(tbPrice.Text);
                deposit = Convert.ToDouble(tbdep.Text);
                i = Convert.ToDouble(tbInterestRate.Text);
                i = i / 100;
                n = Convert.ToDouble(tbNmonths.Text);

                A = (((P - deposit) * Math.Pow(1 + i, n / 12 / 12)) / n);
                this.Visibility = Visibility.Collapsed;
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Enter numbers only!!");
            }


        }

        public double getAProperty()
        {
            return this.A;
        }
    }
}

