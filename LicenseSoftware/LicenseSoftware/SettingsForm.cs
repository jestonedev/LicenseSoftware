using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LicenseSoftware
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            textBoxActivityManagerConfigsPath.Text = LicenseSoftwareSettings.ActivityManagerConfigsPath;
            textBoxActivityManagerOutputCodepage.Text = LicenseSoftwareSettings.ActivityManagerOutputCodePage;
            textBoxActivityManagerPath.Text = LicenseSoftwareSettings.ActivityManagerPath;
            textBoxConnectionString.Text = LicenseSoftwareSettings.ConnectionString;
            textBoxLDAPPassword.Text = LicenseSoftwareSettings.LDAPPassword;
            textBoxLDAPUserName.Text = LicenseSoftwareSettings.LDAPUserName;
            numericUpDownMaxDBConnectionCount.Value = LicenseSoftwareSettings.MaxDBConnectionCount;
            numericUpDownDataModelsCallbackUpdateTimeout.Value = LicenseSoftwareSettings.DataModelsCallbackUpdateTimeout;
            numericUpDownCalcDataModelsUpdateTimeout.Value = LicenseSoftwareSettings.CalcDataModelsUpdateTimeout;
            checkBoxUseLDAP.Checked = LicenseSoftwareSettings.UseLDAP;
        }

        private void vButton2_Click(object sender, EventArgs e)
        {
            LicenseSoftwareSettings.ActivityManagerConfigsPath = textBoxActivityManagerConfigsPath.Text;
            LicenseSoftwareSettings.ActivityManagerOutputCodePage = textBoxActivityManagerOutputCodepage.Text;
            LicenseSoftwareSettings.ActivityManagerPath = textBoxActivityManagerPath.Text;
            LicenseSoftwareSettings.ConnectionString = textBoxConnectionString.Text;
            LicenseSoftwareSettings.LDAPPassword = textBoxLDAPPassword.Text;
            LicenseSoftwareSettings.LDAPUserName = textBoxLDAPUserName.Text;
            LicenseSoftwareSettings.MaxDBConnectionCount = Convert.ToInt32(numericUpDownMaxDBConnectionCount.Value);
            LicenseSoftwareSettings.DataModelsCallbackUpdateTimeout = Convert.ToInt32(numericUpDownDataModelsCallbackUpdateTimeout.Value);
            LicenseSoftwareSettings.CalcDataModelsUpdateTimeout = Convert.ToInt32(numericUpDownCalcDataModelsUpdateTimeout.Value);
            LicenseSoftwareSettings.UseLDAP = checkBoxUseLDAP.Checked;
            LicenseSoftwareSettings.Save();
            Close();
        }
    }
}
