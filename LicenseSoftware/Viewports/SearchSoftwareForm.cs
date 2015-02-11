using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LicenseSoftware.DataModels;
using LicenseSoftware.CalcDataModels;
using System.Globalization;

namespace LicenseSoftware.SearchForms
{
    internal partial class SearchSoftwareForm : SearchForm
    {
        SoftwareDataModel softwares = null;
        SoftTypesDataModel softTypes = null;
        SoftMakersDataModel softMakers = null;

        BindingSource v_softTypes = null;
        BindingSource v_softMakers = null;

        internal override string GetFilter()
        {
            string filter = "";
            if ((checkBoxSoftwareTypeEnable.Checked) && (comboBoxSoftwareType.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID SoftType] = '{0}'", comboBoxSoftwareType.SelectedValue.ToString());
            }
            if ((checkBoxSoftwareMakerEnable.Checked) && (comboBoxSoftwareMaker.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID SoftMaker] = '{0}'", comboBoxSoftwareMaker.SelectedValue.ToString());
            }
            if (checkBoxSoftwareNameEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "Software LIKE '%{0}%'", textBoxSoftwareName.Text.Trim().Replace("'", ""));
            }
            return filter;
        }

        public SearchSoftwareForm()
        {
            InitializeComponent();
            softwares = SoftwareDataModel.GetInstance();
            softMakers = SoftMakersDataModel.GetInstance();
            softTypes = SoftTypesDataModel.GetInstance();

            softwares.Select();
            softMakers.Select();
            softTypes.Select();

            v_softMakers = new BindingSource();
            v_softMakers.DataMember = "SoftMakers";
            v_softMakers.DataSource = DataSetManager.DataSet;

            v_softTypes = new BindingSource();
            v_softTypes.DataMember = "SoftTypes";
            v_softTypes.DataSource = DataSetManager.DataSet;

            comboBoxSoftwareMaker.DataSource = v_softMakers;
            comboBoxSoftwareMaker.ValueMember = "ID SoftMaker";
            comboBoxSoftwareMaker.DisplayMember = "SoftMaker";

            comboBoxSoftwareType.DataSource = v_softTypes;
            comboBoxSoftwareType.ValueMember = "ID SoftType";
            comboBoxSoftwareType.DisplayMember = "SoftType";

            foreach (Control control in this.Controls)
                control.KeyDown += (sender, e) =>
                {
                    ComboBox comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        vButtonSearch_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                };
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if ((checkBoxSoftwareNameEnable.Checked) && String.IsNullOrEmpty(textBoxSoftwareName.Text.Trim()))
            {
                MessageBox.Show("Введите наименование ПО или уберите галочку поиска по наименованию ПО",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxSoftwareName.Focus();
                return;
            }
            if ((checkBoxSoftwareTypeEnable.Checked) && (comboBoxSoftwareType.SelectedValue == null))
            {
                MessageBox.Show("Выберите вид ПО или уберите галочку поиска по виду ПО", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareType.Focus();
                return;
            }
            if ((checkBoxSoftwareMakerEnable.Checked) && (comboBoxSoftwareMaker.SelectedValue == null))
            {
                MessageBox.Show("Выберите разработчика ПО или уберите галочку поиска по разработчику ПО", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareMaker.Focus();
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void checkBoxSoftwareTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxSoftwareType.Enabled = checkBoxSoftwareTypeEnable.Checked;
        }

        private void checkBoxSoftwareNameEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxSoftwareName.Enabled = checkBoxSoftwareNameEnable.Checked;
        }

        private void checkBoxSoftwareMakerEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxSoftwareMaker.Enabled = checkBoxSoftwareMakerEnable.Checked;
        }
    }
}
