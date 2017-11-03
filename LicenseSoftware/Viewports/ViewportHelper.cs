using System;
using System.Windows.Forms;
using System.Reflection;
using System.Data;
using System.Globalization;

namespace LicenseSoftware.Viewport
{
    internal static class ViewportHelper
    {
        internal static void SetDoubleBuffered(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, control, new object[] { true }, CultureInfo.InvariantCulture);
        }

        internal static DateTime ValueOrDefault(DateTime? value)
        {
            if (value != null)
                return value.Value;
            return DateTime.Now.Date;
        }

        internal static double ValueOrDefault(double? value)
        {
            if (value != null)
                return value.Value;
            return 0;
        }

        internal static int ValueOrDefault(int? value)
        {
            if (value != null)
                return value.Value;
            return 0;
        }

        internal static bool ValueOrDefault(bool? value)
        {
            if (value != null)
                return value.Value;
            return false;
        }

        internal static decimal ValueOrDefault(decimal? value)
        {
            if (value != null)
                return value.Value;
            return 0;
        }

        internal static string ValueOrNull(TextBox control)
        {
            if (String.IsNullOrEmpty(control.Text.Trim()))
                return null;
            return control.Text.Trim().Trim();
        }

        internal static T? ValueOrNull<T>(ComboBox control) where T: struct
        {
            if (control.SelectedValue == null)
                return null;
            return (T?)control.SelectedValue;
        }

        internal static string ValueOrNull(ComboBox control)
        {
            if (control.SelectedValue == null)
                return null;
            return control.SelectedValue.ToString().Trim();
        }

        internal static DateTime? ValueOrNull(DateTimePicker control)
        {
            if (control.Checked)
                return control.Value.Date;
            return null;
        }

        internal static T? ValueOrNull<T>(DataRowView row, string property) where T : struct
        {
            if (row[property] is DBNull)
                return null;
            return (T?)Convert.ChangeType(row[property], typeof(T), CultureInfo.InvariantCulture);
        }

        internal static string ValueOrNull(DataRowView row, string property)
        {
            if (row[property] is DBNull)
                return null;
            return row[property].ToString().Trim();
        }

        internal static T? ValueOrNull<T>(DataRow row, string property) where T : struct
        {
            if (row[property] is DBNull)
                return null;
            return (T?)Convert.ChangeType(row[property], typeof(T), CultureInfo.InvariantCulture);
        }

        internal static string ValueOrNull(DataRow row, string property)
        {
            if (row[property] is DBNull)
                return null;
            return row[property].ToString().Trim();
        }

        internal static T? ValueOrNull<T>(DataGridViewRow row, string property) where T : struct
        {
            if (row.Cells[property].Value is DBNull)
                return null;
            if (row.Cells[property].Value == null)
                return null;
            return (T?)Convert.ChangeType(row.Cells[property].Value, typeof(T), CultureInfo.InvariantCulture);
        }

        internal static string ValueOrNull(DataGridViewRow row, string property)
        {
            if (row.Cells[property].Value is DBNull)
                return null;
            if (row.Cells[property].Value == null)
                return null;
            return row.Cells[property].Value.ToString().Trim();
        }

        internal static object ValueOrDBNull(object value)
        {
            return value == null ? DBNull.Value : value;
        }
    }
}
