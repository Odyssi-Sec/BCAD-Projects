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
    
    public partial class Savings : Window
    {
        double saveAmount = 0, i = 0, n = 0, ATemp = 0, A = 0;

        public Savings()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }
        private void
            bsavingSubmit(object sender, RoutedEventArgs e)
        {
            try
            {
                saveAmount = Convert.ToDouble(saveAmount.Text);
                i = Convert.ToDouble(IntRate.Text);
                n = Convert.ToDouble(NSavings.Text);
                reason = reason.Text;

                ATemp = (saveAmount / Math.Pow(1 + i, n / 12));
                A = ATemp / n;

                MessageBox.Show("The amount needed per month " + "\n" +
                    "to save " + saveAmount + " is: " + "\n" +
                    A + "\n" +
                    "REASON: " + reason);

                this.Visibility = Visibility.Collapsed;
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Numbers ONLY!!!");
            }



        }

    }
}



