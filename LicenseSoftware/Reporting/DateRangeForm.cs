using System;
using System.Windows.Forms;

namespace Registry.Reporting
{
    public partial class DateRangeForm : Form
    {
        public DateTime DateFrom
        {
            get
            {
                return dateTimePickerFrom.Value;
            }
        }

        public DateTime DateTo
        {
            get
            {
                return dateTimePickerTo.Value;
            }
        }

        public DateRangeForm()
        {
            InitializeComponent();
        }
    }
}
