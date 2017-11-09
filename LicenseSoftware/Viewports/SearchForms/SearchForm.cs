using System.Windows.Forms;

namespace LicenseSoftware.Viewport.SearchForms
{
    public class SearchForm: Form
    {
        internal virtual string GetFilter() { return ""; }
    }
}
