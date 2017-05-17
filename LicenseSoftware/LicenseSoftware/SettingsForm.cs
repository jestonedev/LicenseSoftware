using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Settings;

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
            textBoxLDAPPassword.Text = LicenseSoftwareSettings.LdapPassword;
            textBoxLDAPUserName.Text = LicenseSoftwareSettings.LdapUserName;
            numericUpDownMaxDBConnectionCount.Value = LicenseSoftwareSettings.MaxDbConnectionCount;
            numericUpDownDataModelsCallbackUpdateTimeout.Value = LicenseSoftwareSettings.DataModelsCallbackUpdateTimeout;
            numericUpDownCalcDataModelsUpdateTimeout.Value = LicenseSoftwareSettings.CalcDataModelsUpdateTimeout;
            checkBoxUseLDAP.Checked = LicenseSoftwareSettings.UseLdap;
        }

        private void vButton2_Click(object sender, EventArgs e)
        {
            LicenseSoftwareSettings.ActivityManagerConfigsPath = textBoxActivityManagerConfigsPath.Text;
            LicenseSoftwareSettings.ActivityManagerOutputCodePage = textBoxActivityManagerOutputCodepage.Text;
            LicenseSoftwareSettings.ActivityManagerPath = textBoxActivityManagerPath.Text;
            LicenseSoftwareSettings.ConnectionString = textBoxConnectionString.Text;
            LicenseSoftwareSettings.LdapPassword = textBoxLDAPPassword.Text;
            LicenseSoftwareSettings.LdapUserName = textBoxLDAPUserName.Text;
            LicenseSoftwareSettings.MaxDbConnectionCount = Convert.ToInt32(numericUpDownMaxDBConnectionCount.Value);
            LicenseSoftwareSettings.DataModelsCallbackUpdateTimeout = Convert.ToInt32(numericUpDownDataModelsCallbackUpdateTimeout.Value);
            LicenseSoftwareSettings.CalcDataModelsUpdateTimeout = Convert.ToInt32(numericUpDownCalcDataModelsUpdateTimeout.Value);
            LicenseSoftwareSettings.UseLdap = checkBoxUseLDAP.Checked;
            LicenseSoftwareSettings.Save();
            Close();
        }
    }
}
