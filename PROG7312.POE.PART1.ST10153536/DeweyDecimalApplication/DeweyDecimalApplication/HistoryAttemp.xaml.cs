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

namespace DeweyDecimalApplication
{
    /// <summary>
    /// Interaction logic for HistoryAttemp.xaml
    /// </summary>
    public partial class HistoryAttemp : Window
    {
        public class History
        {
            public DateTime Timestamp { get; set; }
            public string CallNumbers { get; set; }
            public bool IsCorrect { get; set; }
            public TimeSpan ElapsedTime { get; set; }
            public int HintUsed { get; set; }
        }
        public HistoryAttemp()
        {
            InitializeComponent();
        }
    }
}

