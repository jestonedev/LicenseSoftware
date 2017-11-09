using System;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.DataModels;
using LicenseSoftware.DataModels.DataModels;

namespace LicenseSoftware.Viewport.SearchForms
{
    internal partial class SearchSoftwareForm : SearchForm
    {
        internal override string GetFilter()
        {
            var filter = "";
            if ((checkBoxSoftwareTypeEnable.Checked) && (comboBoxSoftwareType.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID SoftType] = '{0}'", comboBoxSoftwareType.SelectedValue);
            }
            if ((checkBoxSoftwareMakerEnable.Checked) && (comboBoxSoftwareMaker.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID SoftMaker] = '{0}'", comboBoxSoftwareMaker.SelectedValue);
            }
            if (checkBoxSoftwareNameEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "Software LIKE '%{0}%'", textBoxSoftwareName.Text.Trim().Replace("'", ""));
            }
            return filter;
        }

        public SearchSoftwareForm()
        {
            InitializeComponent();
            var softwares = SoftwareDataModel.GetInstance();
            var softMakers = SoftMakersDataModel.GetInstance();
            var softTypes = SoftTypesDataModel.GetInstance();

            softwares.Select();
            softMakers.Select();
            softTypes.Select();

            var vSoftMakers = new BindingSource
            {
                DataMember = "SoftMakers",
                DataSource = DataSetManager.DataSet
            };

            var vSoftTypes = new BindingSource
            {
                DataMember = "SoftTypes",
                DataSource = DataSetManager.DataSet
            };

            comboBoxSoftwareMaker.DataSource = vSoftMakers;
            comboBoxSoftwareMaker.ValueMember = "ID SoftMaker";
            comboBoxSoftwareMaker.DisplayMember = "SoftMaker";

            comboBoxSoftwareType.DataSource = vSoftTypes;
            comboBoxSoftwareType.ValueMember = "ID SoftType";
            comboBoxSoftwareType.DisplayMember = "SoftType";

            foreach (Control control in Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                            vButtonSearch_Click(sender, e);
                            break;
                        case Keys.Escape:
                            DialogResult = DialogResult.Cancel;
                            break;
                    }
                };
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if ((checkBoxSoftwareNameEnable.Checked) && string.IsNullOrEmpty(textBoxSoftwareName.Text.Trim()))
            {
                MessageBox.Show(@"Введите наименование ПО или уберите галочку поиска по наименованию ПО",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxSoftwareName.Focus();
                return;
            }
            if ((checkBoxSoftwareTypeEnable.Checked) && (comboBoxSoftwareType.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите вид ПО или уберите галочку поиска по виду ПО", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareType.Focus();
                return;
            }
            if ((checkBoxSoftwareMakerEnable.Checked) && (comboBoxSoftwareMaker.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите разработчика ПО или уберите галочку поиска по разработчику ПО", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareMaker.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
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
