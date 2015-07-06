using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LicenseSoftware.Viewport
{
    internal sealed class InstallatorsViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idInstallator;
        private DataGridViewTextBoxColumn fullName;
        private DataGridViewTextBoxColumn profession;
        private DataGridViewCheckBoxColumn inactive;
        #endregion Components

        #region Models
        SoftInstallatorsDataModel installators = null;
        DataTable snapshotInstallators = new DataTable("snapshotInstallators");
        #endregion Models

        #region Views
        BindingSource v_installators = null;
        BindingSource v_snapshotInstallators = null;
        #endregion Views



        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private InstallatorsViewport()
            : this(null)
        {
        }

        public InstallatorsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshotInstallators.Locale = CultureInfo.InvariantCulture;
        }

        public InstallatorsViewport(InstallatorsViewport installatorsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = installatorsViewport.DynamicFilter;
            this.StaticFilter = installatorsViewport.StaticFilter;
            this.ParentRow = installatorsViewport.ParentRow;
            this.ParentType = installatorsViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            List<SoftInstallator> list_from_view = InstallatorsFromView();
            List<SoftInstallator> list_from_viewport = InstallatorsFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            bool founded = false;
            for (int i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (int j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["ID Installator"], 
                dataRowView["FullName"],
                dataRowView["Profession"],
                ViewportHelper.ValueOrNull<bool>(dataRowView,"inactive") == true
            };
        }

        private static bool ValidateViewportData(List<SoftInstallator> list)
        {
            foreach (SoftInstallator installator in list)
            {
                if (installator.FullName == null)
                {
                    MessageBox.Show("ФИО установщика не может быть пустым", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftInstallator RowToInstallator(DataRow row)
        {
            SoftInstallator si = new SoftInstallator();
            si.IdInstallator = ViewportHelper.ValueOrNull<int>(row, "ID Installator");
            si.FullName = ViewportHelper.ValueOrNull(row, "FullName");
            si.Profession = ViewportHelper.ValueOrNull(row, "Profession");
            si.Inactive = ViewportHelper.ValueOrNull<bool>(row, "Inactive");
            return si;
        }

        private List<SoftInstallator> InstallatorsFromViewport()
        {
            List<SoftInstallator> list = new List<SoftInstallator>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    SoftInstallator si = new SoftInstallator();
                    DataGridViewRow row = dataGridView.Rows[i];
                    si.IdInstallator = ViewportHelper.ValueOrNull<int>(row, "idInstallator");
                    si.FullName = ViewportHelper.ValueOrNull(row, "FullName");
                    si.Profession = ViewportHelper.ValueOrNull(row, "Profession");
                    si.Inactive = ViewportHelper.ValueOrNull<bool>(row, "Inactive") == true;
                    list.Add(si);
                }
            }
            return list;
        }

        private List<SoftInstallator> InstallatorsFromView()
        {
            List<SoftInstallator> list = new List<SoftInstallator>();
            for (int i = 0; i < v_installators.Count; i++)
            {
                SoftInstallator si = new SoftInstallator();
                DataRowView row = ((DataRowView)v_installators[i]);
                si.IdInstallator = ViewportHelper.ValueOrNull<int>(row, "ID Installator");
                si.FullName = ViewportHelper.ValueOrNull(row, "FullName");
                si.Profession = ViewportHelper.ValueOrNull(row, "Profession");
                si.Inactive = ViewportHelper.ValueOrNull<bool>(row, "Inactive") == true;
                list.Add(si);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshotInstallators.Count;
        }

        public override void MoveFirst()
        {
            v_snapshotInstallators.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshotInstallators.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshotInstallators.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshotInstallators.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshotInstallators.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshotInstallators.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshotInstallators.Position > -1) && (v_snapshotInstallators.Position < (v_snapshotInstallators.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshotInstallators.Position > -1) && (v_snapshotInstallators.Position < (v_snapshotInstallators.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            installators = SoftInstallatorsDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            installators.Select();

            v_installators = new BindingSource();
            v_installators.DataMember = "SoftInstallators";
            v_installators.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < installators.Select().Columns.Count; i++)
                snapshotInstallators.Columns.Add(new DataColumn(
                    installators.Select().Columns[i].ColumnName, installators.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_installators.Count; i++)
                snapshotInstallators.Rows.Add(DataRowViewToArray(((DataRowView)v_installators[i])));
            v_snapshotInstallators = new BindingSource();
            v_snapshotInstallators.DataSource = snapshotInstallators;
            v_snapshotInstallators.CurrentItemChanged += new EventHandler(v_snapshotInstallators_CurrentItemChanged);

            dataGridView.DataSource = v_snapshotInstallators;
            idInstallator.DataPropertyName = "ID Installator";
            fullName.DataPropertyName = "fullName";
            profession.DataPropertyName = "profession";
            inactive.DataPropertyName = "inactive";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            installators.Select().RowChanged += new DataRowChangeEventHandler(InstallatorsViewport_RowChanged);
            installators.Select().RowDeleting += new DataRowChangeEventHandler(InstallatorsViewport_RowDeleting);
            installators.Select().RowDeleted += new DataRowChangeEventHandler(InstallatorsViewport_RowDeleted);
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshotInstallators.AddNew();
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshotInstallators.Position != -1) && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshotInstallators[v_snapshotInstallators.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshotInstallators.Clear();
            for (int i = 0; i < v_installators.Count; i++)
                snapshotInstallators.Rows.Add(DataRowViewToArray(((DataRowView)v_installators[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            dataGridView.EndEdit();
            sync_views = false;
            List<SoftInstallator> list = InstallatorsFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = installators.Select().Rows.Find(((SoftInstallator)list[i]).IdInstallator);
                if (row == null)
                {
                    int idInstallator = SoftInstallatorsDataModel.Insert(list[i]);
                    if (idInstallator == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshotInstallators[i])["ID Installator"] = idInstallator;
                    installators.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshotInstallators[i]));
                }
                else
                {

                    if (RowToInstallator(row) == list[i])
                        continue;
                    if (SoftInstallatorsDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["FullName"] = list[i].FullName == null ? DBNull.Value : (object)list[i].FullName;
                    row["Profession"] = list[i].Profession == null ? DBNull.Value : (object)list[i].Profession;
                    row["Inactive"] = list[i].Inactive == null ? DBNull.Value : (object)list[i].Inactive;
                }
            }
            list = InstallatorsFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idInstallator"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["idInstallator"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idInstallator"].Value == list[i].IdInstallator))
                        row_index = j;
                if (row_index == -1)
                {
                    if (SoftInstallatorsDataModel.Delete(list[i].IdInstallator.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    installators.Select().Rows.Find(((SoftInstallator)list[i]).IdInstallator).Delete();
                }
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            InstallatorsViewport viewport = new InstallatorsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения об установщиках в базу данных в базу данных?", "Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
            }
            installators.Select().RowChanged -= new DataRowChangeEventHandler(InstallatorsViewport_RowChanged);
            installators.Select().RowDeleting -= new DataRowChangeEventHandler(InstallatorsViewport_RowDeleting);
            installators.Select().RowDeleted -= new DataRowChangeEventHandler(InstallatorsViewport_RowDeleted);
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "fullName":
                    if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                        cell.ErrorText = "ФИО установщика не может быть пустым";
                    else
                        cell.ErrorText = "";
                    break;
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void InstallatorsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void InstallatorsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshotInstallators.Find("ID Installator", e.Row["ID Installator"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshotInstallators[row_index]).Delete();
            }
        }

        void InstallatorsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            int row_index = v_snapshotInstallators.Find("ID Installator", e.Row["ID Installator"]);
            if (row_index == -1 && v_installators.Find("ID Installator", e.Row["ID Installator"]) != -1)
            {
                snapshotInstallators.Rows.Add(new object[] { 
                        e.Row["ID Installator"], 
                        e.Row["FullName"],   
                        e.Row["Profession"],                 
                        e.Row["Inactive"]
                    });
            }
            else
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshotInstallators[row_index]);
                    row["FullName"] = e.Row["FullName"];
                    row["Profession"] = e.Row["Profession"];
                    row["Inactive"] = e.Row["Inactive"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshotInstallators_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallatorsViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.idInstallator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fullName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.profession = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inactive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idInstallator,
            this.fullName,
            this.profession,
            this.inactive});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(648, 281);
            this.dataGridView.TabIndex = 8;
            // 
            // idInstallator
            // 
            this.idInstallator.Frozen = true;
            this.idInstallator.HeaderText = "Идентификатор";
            this.idInstallator.Name = "idInstallator";
            this.idInstallator.ReadOnly = true;
            this.idInstallator.Visible = false;
            // 
            // fullName
            // 
            this.fullName.HeaderText = "ФИО установщика";
            this.fullName.MaxInputLength = 128;
            this.fullName.MinimumWidth = 150;
            this.fullName.Name = "fullName";
            // 
            // profession
            // 
            this.profession.HeaderText = "Должность";
            this.profession.MaxInputLength = 128;
            this.profession.MinimumWidth = 150;
            this.profession.Name = "profession";
            // 
            // inactive
            // 
            this.inactive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.inactive.HeaderText = "Неактивный";
            this.inactive.Name = "inactive";
            this.inactive.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.inactive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // InstallatorsViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(654, 287);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InstallatorsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Исполнители";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
