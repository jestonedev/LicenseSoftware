using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
