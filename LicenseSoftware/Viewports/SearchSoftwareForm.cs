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
        /*KladrStreetsDataModel kladr = null;
        KladrRegionsDataModel regions = null;
        FundTypesDataModel fundTypes = null;
        ObjectStatesDataModel object_states = null;
        OwnershipRightTypesDataModel ownership_right_types = null;

        BindingSource v_kladr = null;
        BindingSource v_regions = null;
        BindingSource v_fundTypes = null;
        BindingSource v_object_states = null;
        BindingSource v_ownership_right_types = null;*/

        internal override string GetFilter()
        {
            string filter = "";
            /*IEnumerable<int> included_buildings = null;
            if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_street = '" + comboBoxStreet.SelectedValue.ToString() + "'";
            }
            if (checkBoxRegionEnable.Checked && comboBoxRegion.SelectedValue != null)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_street LIKE '"+comboBoxRegion.SelectedValue.ToString()+"%'";
            }
            if (checkBoxHouseEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "house = '{0}'", textBoxHouse.Text.Trim().Replace("'", ""));
            }
            if (checkBoxFloorsEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "floors = " + numericUpDownFloors.Value.ToString(CultureInfo.InvariantCulture);
            }
            if (checkBoxCadastralNumEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                if (!String.IsNullOrEmpty(textBoxCadastralNum.Text.Trim()))
                    filter += String.Format(CultureInfo.InvariantCulture, "cadastral_num = '{0}'", textBoxCadastralNum.Text.Trim().Replace("'", ""));
                else
                    filter += "cadastral_num is null";
            }
            if (checkBoxStartupYearEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "startup_year = " + numericUpDownStartupYear.Value;
            }
            if (checkBoxImprovementEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "improvement = " + (checkBoxImprovement.Checked ? 1 : 0).ToString(CultureInfo.InvariantCulture);
            }
            if (checkBoxElevatorEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "elevator = " + (checkBoxElevator.Checked ? 1 : 0).ToString(CultureInfo.InvariantCulture);
            }
            if ((checkBoxStateEnable.Checked) && (comboBoxState.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_state = " + comboBoxState.SelectedValue.ToString();
            }
            if (checkBoxIDBuildingEnable.Checked)
                included_buildings = DataModelHelper.Intersect(included_buildings, new List<int>() { Convert.ToInt32(numericUpDownIDBuilding.Value) });
            if ((checkBoxFundTypeEnable.Checked) && (comboBoxStreet.SelectedValue != null))
            {
                IEnumerable<int> buildings_ids = DataModelHelper.BuildingIDsByCurrentFund(
                    Convert.ToInt32(comboBoxFundType.SelectedValue, CultureInfo.InvariantCulture));
                included_buildings = DataModelHelper.Intersect(included_buildings, buildings_ids);
            }
            if (checkBoxContractNumberEnable.Checked)
            {
                IEnumerable<int> buildings_ids = DataModelHelper.BuildingIDsByRegistrationNumber(textBoxContractNumber.Text.Trim().Replace("'", ""));
                included_buildings = DataModelHelper.Intersect(included_buildings, buildings_ids);
            }
            if (checkBoxTenantSNPEnable.Checked)
            {
                string[] snp = textBoxTenantSNP.Text.Trim().Replace("'", "").Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable<int> buildings_ids = DataModelHelper.BuildingIDsBySNP(snp, (row) => { return row.Field<int>("id_kinship") == 1; });
                included_buildings = DataModelHelper.Intersect(included_buildings, buildings_ids);
            }
            if ((checkBoxOwnershipTypeEnable.Checked) && (comboBoxOwnershipType.SelectedValue != null))
            {
                IEnumerable<int> buildings_ids = DataModelHelper.BuildingIDsByOwnershipType(
                    Int32.Parse(comboBoxOwnershipType.SelectedValue.ToString(), CultureInfo.InvariantCulture));
                included_buildings = DataModelHelper.Intersect(included_buildings, buildings_ids);
            }
            if (included_buildings != null)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "id_building IN (0";
                foreach (int id in included_buildings)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }*/
            return filter;
        }

        public SearchSoftwareForm()
        {
            InitializeComponent();
            /*kladr = KladrStreetsDataModel.GetInstance();
            fundTypes = FundTypesDataModel.GetInstance();
            object_states = ObjectStatesDataModel.GetInstance();
            regions = KladrRegionsDataModel.GetInstance();
            ownership_right_types = OwnershipRightTypesDataModel.GetInstance();

            v_kladr = new BindingSource();
            v_kladr.DataSource = kladr.Select();

            v_regions = new BindingSource();
            v_regions.DataSource = regions.Select();

            v_fundTypes = new BindingSource();
            v_fundTypes.DataSource = fundTypes.Select();

            v_object_states = new BindingSource();
            v_object_states.DataSource = object_states.Select();

            v_ownership_right_types = new BindingSource();
            v_ownership_right_types.DataSource = ownership_right_types.Select();

            comboBoxStreet.DataSource = v_kladr;
            comboBoxStreet.ValueMember = "id_street";
            comboBoxStreet.DisplayMember = "street_name";

            comboBoxFundType.DataSource = v_fundTypes;
            comboBoxFundType.ValueMember = "id_fund_type";
            comboBoxFundType.DisplayMember = "fund_type";

            comboBoxState.DataSource = v_object_states;
            comboBoxState.ValueMember = "id_state";
            comboBoxState.DisplayMember = "state_neutral";

            comboBoxRegion.DataSource = v_regions;
            comboBoxRegion.ValueMember = "id_region";
            comboBoxRegion.DisplayMember = "region";

            comboBoxOwnershipType.DataSource = v_ownership_right_types;
            comboBoxOwnershipType.ValueMember = "id_ownership_right_type";
            comboBoxOwnershipType.DisplayMember = "ownership_right_type";

            numericUpDownStartupYear.Maximum = DateTime.Now.Year;*/
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

        private void checkBoxStreetEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxStreet.Enabled = checkBoxStreetEnable.Checked;
        }

        private void checkBoxHouseEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxHouse.Enabled = checkBoxHouseEnable.Checked;
        }

        private void checkBoxFloorsEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownFloors.Enabled = checkBoxFloorsEnable.Checked;
        }

        private void checkBoxCadastralNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxCadastralNum.Enabled = checkBoxCadastralNumEnable.Checked;
        }

        private void checkBoxStartupYearEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownStartupYear.Enabled = checkBoxStartupYearEnable.Checked;
        }

        private void checkBoxFundTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxFundType.Enabled = checkBoxFundTypeEnable.Checked;
        }

        private void checkBoxImprovementEnable_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxImprovement.Enabled = checkBoxImprovementEnable.Checked;
        }

        private void checkBoxElevatorEnable_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxElevator.Enabled = checkBoxElevatorEnable.Checked;
        }

        private void comboBoxStreet_Leave(object sender, EventArgs e)
        {
            /*if (comboBoxStreet.Items.Count > 0)
            {
                if (comboBoxStreet.SelectedValue == null)
                    comboBoxStreet.SelectedValue = v_kladr[v_kladr.Position];
                comboBoxStreet.Text = ((DataRowView)v_kladr[v_kladr.Position])["street_name"].ToString();
            }
            if (comboBoxStreet.SelectedValue == null)
                comboBoxStreet.Text = "";*/
        }

        private void comboBoxStreet_KeyUp(object sender, KeyEventArgs e)
        {
            /*if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back))
            {
                string text = comboBoxStreet.Text;
                int selectionStart = comboBoxStreet.SelectionStart;
                int selectionLength = comboBoxStreet.SelectionLength;
                v_kladr.Filter = "street_name like '%" + comboBoxStreet.Text + "%'";
                comboBoxStreet.Text = text;
                comboBoxStreet.SelectionStart = selectionStart;
                comboBoxStreet.SelectionLength = selectionLength;
            }*/
        }

        private void comboBoxStreet_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStreet.Items.Count == 0)
                comboBoxStreet.SelectedIndex = -1;
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            /*if ((checkBoxStreetEnable.Checked) && (comboBoxStreet.SelectedValue == null))
            {
                MessageBox.Show("Выберите улицу или уберите галочку поиска по улице", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxStreet.Focus();
                return;
            }
            if ((checkBoxHouseEnable.Checked) && String.IsNullOrEmpty(textBoxHouse.Text.Trim()))
            {
                MessageBox.Show("Введите номер здания или уберите галочку поиска по номеру дома", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxHouse.Focus();
                return;
            }
            if ((checkBoxContractNumberEnable.Checked) && String.IsNullOrEmpty(textBoxContractNumber.Text.Trim()))
            {
                MessageBox.Show("Введите номер договора найма или уберите галочку поиска по номеру договора найма", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxContractNumber.Focus();
                return;
            }
            if ((checkBoxTenantSNPEnable.Checked) && String.IsNullOrEmpty(textBoxTenantSNP.Text.Trim()))
            {
                MessageBox.Show("Введите ФИО нанимателя или уберите галочку поиска по ФИО нанимателя",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxTenantSNP.Focus();
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;*/
        }

        private void checkBoxStateEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxState.Enabled = checkBoxStateEnable.Checked;
        }

        private void checkBoxIDBuildingEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownIDBuilding.Enabled = checkBoxIDBuildingEnable.Checked;
        }

        private void checkBoxRegionEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRegion.Enabled = checkBoxRegionEnable.Checked;
        }

        private void checkBoxContractNumberEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxContractNumber.Enabled = checkBoxContractNumberEnable.Checked;
        }

        private void checkBoxTenantSNPEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTenantSNP.Enabled = checkBoxTenantSNPEnable.Checked;
        }

        private void checkBoxOwnershipTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxOwnershipType.Enabled = checkBoxOwnershipTypeEnable.Checked;
        }
    }
}
