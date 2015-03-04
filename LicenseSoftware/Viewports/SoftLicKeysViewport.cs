using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using LicenseSoftware.CalcDataModels;
using System.Text.RegularExpressions;
using CustomControls;
using Security;
using System.Globalization;
using LicenseSoftware.Reporting;

namespace LicenseSoftware.Viewport
{
    internal sealed class SoftLicKeysViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        SoftLicKeysDataModel softLicKeys = null;
        DataTable snapshotsoftLicKeys = new DataTable("snapshotSoftLicKeys");
        #endregion Models

        #region Views
        BindingSource v_softLicKeys = null;
        BindingSource v_snapshotSoftLicKeys = null;
        #endregion Views
        private DataGridViewTextBoxColumn idLicenseKey;
        private DataGridViewTextBoxColumn idLicense;
        private DataGridViewTextBoxColumn LicKey;


        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private SoftLicKeysViewport()
            : this(null)
        {
        }

        public SoftLicKeysViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshotsoftLicKeys.Locale = CultureInfo.InvariantCulture;
        }

        public SoftLicKeysViewport(SoftLicKeysViewport softLicKeysViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = softLicKeysViewport.DynamicFilter;
            this.StaticFilter = softLicKeysViewport.StaticFilter;
            this.ParentRow = softLicKeysViewport.ParentRow;
            this.ParentType = softLicKeysViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            List<SoftLicKey> list_from_view = SoftLicKeysFromView();
            List<SoftLicKey> list_from_viewport = SoftLicKeysFromViewport();
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
                dataRowView["ID LicenseKey"], 
                dataRowView["ID License"], 
                dataRowView["LicKey"]
            };
        }

        private bool ValidateSoftLicKeys(List<SoftLicKey> softLicKeys)
        {
            foreach (SoftLicKey softLicKey in softLicKeys)
            {
                if (softLicKey.LicKey == null)
                {
                    MessageBox.Show("Лицензионный ключ является обязательным для заполнения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softLicKey.LicKey != null && softLicKey.LicKey.Length > 200)
                {
                    MessageBox.Show("Длина лицензионного ключа не может превышать 200 символов", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            if (ParentRow["InstallationsCount"] != DBNull.Value && v_snapshotSoftLicKeys.Count > (int)ParentRow["InstallationsCount"])
            {
                DialogResult result = MessageBox.Show("Количество внесенных ключей превышает количество разрешенных установок данного ПО. " +
                    "Вы уверены, что хотите сохранить изменения в базу данных?", "Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (result != System.Windows.Forms.DialogResult.Yes)
                    return false;
            }
            return true;
        }

        private static SoftLicKey RowToSoftLicKey(DataRow row)
        {
            SoftLicKey softLicKey = new SoftLicKey();
            softLicKey.IdLicenseKey = ViewportHelper.ValueOrNull<int>(row, "ID LicenseKey");
            softLicKey.IdLicense = ViewportHelper.ValueOrNull<int>(row, "ID License");
            softLicKey.LicKey = ViewportHelper.ValueOrNull(row, "LicKey");
            return softLicKey;
        }

        private List<SoftLicKey> SoftLicKeysFromViewport()
        {
            List<SoftLicKey> list = new List<SoftLicKey>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    SoftLicKey slk = new SoftLicKey();
                    DataGridViewRow row = dataGridView.Rows[i];
                    slk.IdLicenseKey = ViewportHelper.ValueOrNull<int>(row, "idLicenseKey");
                    slk.IdLicense = ViewportHelper.ValueOrNull<int>(row, "idLicense");
                    slk.LicKey = ViewportHelper.ValueOrNull(row, "LicKey");
                    list.Add(slk);
                }
            }
            return list;
        }

        private List<SoftLicKey> SoftLicKeysFromView()
        {
            List<SoftLicKey> list = new List<SoftLicKey>();
            for (int i = 0; i < v_softLicKeys.Count; i++)
            {
                SoftLicKey slk = new SoftLicKey();
                DataRowView row = ((DataRowView)v_softLicKeys[i]);
                slk.IdLicenseKey = ViewportHelper.ValueOrNull<int>(row, "ID LicenseKey");
                slk.IdLicense = ViewportHelper.ValueOrNull<int>(row, "ID License");
                slk.LicKey = ViewportHelper.ValueOrNull(row, "LicKey");
                list.Add(slk);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshotSoftLicKeys.Count;
        }

        public override void MoveFirst()
        {
            v_snapshotSoftLicKeys.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshotSoftLicKeys.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshotSoftLicKeys.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshotSoftLicKeys.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshotSoftLicKeys.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshotSoftLicKeys.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshotSoftLicKeys.Position > -1) && (v_snapshotSoftLicKeys.Position < (v_snapshotSoftLicKeys.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshotSoftLicKeys.Position > -1) && (v_snapshotSoftLicKeys.Position < (v_snapshotSoftLicKeys.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            softLicKeys = SoftLicKeysDataModel.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            softLicKeys.Select();

            v_softLicKeys = new BindingSource();
            v_softLicKeys.DataMember = "SoftLicKeys";
            v_softLicKeys.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                v_softLicKeys.Filter += " AND ";
            v_softLicKeys.Filter += DynamicFilter;
            v_softLicKeys.DataSource = DataSetManager.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.License)
                this.Text = String.Format(CultureInfo.InvariantCulture, "Лицензионные ключи лицензии №{0}", ParentRow["ID License"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < softLicKeys.Select().Columns.Count; i++)
                snapshotsoftLicKeys.Columns.Add(new DataColumn(softLicKeys.Select().Columns[i].ColumnName, softLicKeys.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_softLicKeys.Count; i++)
                snapshotsoftLicKeys.Rows.Add(DataRowViewToArray(((DataRowView)v_softLicKeys[i])));
            v_snapshotSoftLicKeys = new BindingSource();
            v_snapshotSoftLicKeys.DataSource = snapshotsoftLicKeys;
            v_snapshotSoftLicKeys.CurrentItemChanged += new EventHandler(v_snapshotLicKeys_CurrentItemChanged);

            dataGridView.DataSource = v_snapshotSoftLicKeys;
            idLicenseKey.DataPropertyName = "ID LicenseKey";
            idLicense.DataPropertyName = "ID License";
            LicKey.DataPropertyName = "LicKey";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            softLicKeys.Select().RowChanged += new DataRowChangeEventHandler(SoftLicKeysViewport_RowChanged);
            softLicKeys.Select().RowDeleting += new DataRowChangeEventHandler(SoftLicKeysViewport_RowDeleting);
            softLicKeys.Select().RowDeleted += SoftLicKeysViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return (ParentType == ParentTypeEnum.License) && (ParentRow != null) &&
                AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override void InsertRecord()
        {
            if ((ParentRow == null) || (ParentType != ParentTypeEnum.License))
                return;
            DataRowView row = (DataRowView)v_snapshotSoftLicKeys.AddNew();
            row["ID License"] = ParentRow["ID License"];
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshotSoftLicKeys.Position != -1) &&
                AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshotSoftLicKeys[v_snapshotSoftLicKeys.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshotsoftLicKeys.Clear();
            for (int i = 0; i < v_softLicKeys.Count; i++)
                snapshotsoftLicKeys.Rows.Add(DataRowViewToArray(((DataRowView)v_softLicKeys[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() &&
                AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<SoftLicKey> list = SoftLicKeysFromViewport();
            if (!ValidateSoftLicKeys(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = softLicKeys.Select().Rows.Find(((SoftLicKey)list[i]).IdLicenseKey);
                if (row == null)
                {
                    int idLicKey = SoftLicKeysDataModel.Insert(list[i]);
                    if (idLicKey == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshotSoftLicKeys[i])["ID LicenseKey"] = idLicKey;
                    softLicKeys.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshotSoftLicKeys[i]));
                }
                else
                {
                    SoftLicKey softLicKeyFromView = RowToSoftLicKey(row);
                    if (softLicKeyFromView == list[i])
                        continue;
                    if (SoftLicKeysDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["ID License"] = list[i].IdLicense == null ? DBNull.Value : (object)list[i].IdLicense;
                    row["LicKey"] = list[i].LicKey == null ? DBNull.Value : (object)list[i].LicKey;
                }
            }
            list = SoftLicKeysFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idLicenseKey"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["idLicenseKey"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idLicenseKey"].Value == list[i].IdLicenseKey))
                        row_index = j;
                if (row_index == -1)
                {
                    if (SoftLicKeysDataModel.Delete(list[i].IdLicenseKey.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    softLicKeys.Select().Rows.Find(((SoftLicKey)list[i]).IdLicenseKey).Delete();
                }
            }
            sync_views = true;
            MenuCallback.EditingStateUpdate();
            //TODO тут будет вызов обновления вычисляемой модели доступных лицензионных ключей
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            SoftLicKeysViewport viewport = new SoftLicKeysViewport(this, MenuCallback);
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
                DialogResult result = MessageBox.Show("Сохранить изменения о комнатах в базу данных?", "Внимание",
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
            softLicKeys.Select().RowChanged -= new DataRowChangeEventHandler(SoftLicKeysViewport_RowChanged);
            softLicKeys.Select().RowDeleting -= new DataRowChangeEventHandler(SoftLicKeysViewport_RowDeleting);
            softLicKeys.Select().RowDeleted -= new DataRowChangeEventHandler(SoftLicKeysViewport_RowDeleted);
        }

        public override void ForceClose()
        {
            softLicKeys.Select().RowChanged -= new DataRowChangeEventHandler(SoftLicKeysViewport_RowChanged);
            softLicKeys.Select().RowDeleting -= new DataRowChangeEventHandler(SoftLicKeysViewport_RowDeleting);
            softLicKeys.Select().RowDeleted -= new DataRowChangeEventHandler(SoftLicKeysViewport_RowDeleted);
            base.Close();
        }

        

        void v_snapshotLicKeys_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "LicKey":
                    if (cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                        cell.ErrorText = "Номер лицензионного ключа не может быть пустым";
                    else
                    if (cell.Value.ToString().Trim().Length > 200)
                        cell.ErrorText = "Длина лицензионного ключа не может превышать 200 символов";
                    break;
            }
        }

        void SoftLicKeysViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        void SoftLicKeysViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshotSoftLicKeys.Find("ID LicenseKey", e.Row["ID LicenseKey"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshotSoftLicKeys[row_index]).Delete();
            }
        }

        void SoftLicKeysViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            int row_index = v_snapshotSoftLicKeys.Find("ID LicenseKey", e.Row["ID LicenseKey"]);
            if (row_index == -1 && v_softLicKeys.Find("ID LicenseKey", e.Row["ID LicenseKey"]) != -1)
            {
                snapshotsoftLicKeys.Rows.Add(new object[] { 
                        e.Row["ID LicenseKey"], 
                        e.Row["ID License"],           
                        e.Row["LicKey"]
                    });
            }
            else
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshotSoftLicKeys[row_index]);
                    row["ID License"] = e.Row["ID License"];
                    row["LicKey"] = e.Row["LicKey"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftLicKeysViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.idLicenseKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idLicense = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LicKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
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
            this.idLicenseKey,
            this.idLicense,
            this.LicKey});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(795, 333);
            this.dataGridView.TabIndex = 0;
            // 
            // idLicenseKey
            // 
            this.idLicenseKey.HeaderText = "Внутренний номер лицензионного ключа";
            this.idLicenseKey.Name = "idLicenseKey";
            this.idLicenseKey.Visible = false;
            // 
            // idLicense
            // 
            this.idLicense.HeaderText = "Внутренний номер лицензии";
            this.idLicense.Name = "idLicense";
            this.idLicense.Visible = false;
            // 
            // LicKey
            // 
            this.LicKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LicKey.HeaderText = "Лицензионный ключ";
            this.LicKey.MinimumWidth = 150;
            this.LicKey.Name = "LicKey";
            // 
            // SoftLicKeysViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(801, 339);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SoftLicKeysViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень лицензионных ключей";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
