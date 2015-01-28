using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LicenseSoftware.SearchForms
{
    public class SearchForm: Form
    {
        internal virtual string GetFilter() { return ""; }
    }
}
