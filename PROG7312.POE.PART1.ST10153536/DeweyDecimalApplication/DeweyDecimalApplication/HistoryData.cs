using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeweyDecimalApplication.ReplacingBooks;

namespace DeweyDecimalApplication
{
    public partial class HistoryData
    {
        public List<AttemptHistory> SharedHistoryList { get; set; }

        public HistoryData()
        {
            SharedHistoryList = new List<AttemptHistory>();
        }
    }
}
