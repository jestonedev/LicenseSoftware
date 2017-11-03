using System.Windows.Forms;

namespace LicenseSoftware.SearchForms
{
    public class SearchForm: Form
    {
        internal virtual string GetFilter() { return ""; }
    }
}
