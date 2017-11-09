using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using Security;
using System.Globalization;
using System.Linq;
using LicenseSoftware.DataModels.DataModels;

namespace LicenseSoftware.Viewport
{
    internal sealed class SoftLicKeysViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idLicenseKey;
        private DataGridViewTextBoxColumn idLicense;
        private DataGridViewTextBoxColumn LicKey;
        #endregion Components

        #region Models

        private SoftLicKeysDataModel _softLicKeys;
        private readonly DataTable _snapshotsoftLicKeys = new DataTable("snapshotSoftLicKeys");
        #endregion Models

        #region Views

        private BindingSource _vSoftLicKeys;
        private BindingSource _vSnapshotSoftLicKeys;
        #endregion Views


        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private SoftLicKeysViewport()
            : this(null)
        {
        }

        public SoftLicKeysViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            _snapshotsoftLicKeys.Locale = CultureInfo.InvariantCulture;
        }

        public SoftLicKeysViewport(SoftLicKeysViewport softLicKeysViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = softLicKeysViewport.DynamicFilter;
            StaticFilter = softLicKeysViewport.StaticFilter;
            ParentRow = softLicKeysViewport.ParentRow;
            ParentType = softLicKeysViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = SoftLicKeysFromView();
            var listFromViewport = SoftLicKeysFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            bool founded;
            for (var i = 0; i < listFromView.Count; i++)
            {
                founded = false;
                for (var j = 0; j < listFromViewport.Count; j++)
                    if (listFromView[i] == listFromViewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["ID LicenseKey"], 
                dataRowView["ID License"], 
                dataRowView["LicKey"]
            };
        }

        private bool ValidateSoftLicKeys(List<SoftLicKey> softLicKeysParam)
        {
            foreach (var softLicKey in softLicKeysParam)
            {
                if (softLicKey.LicKey == null)
                {
                    MessageBox.Show(@"Лицензионный ключ является обязательным для заполнения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softLicKey.LicKey != null && softLicKey.LicKey.Length > 200)
                {
                    MessageBox.Show(@"Длина лицензионного ключа не может превышать 200 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                var localSoftLicKey = softLicKey;
                if (softLicKeysParam.Count(v => v.LicKey == localSoftLicKey.LicKey) > 1)
                {
                    MessageBox.Show(string.Format("Вы пытаетесь добавить лицензионный ключ {0} дважды", localSoftLicKey.LicKey),
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            if (ParentRow["InstallationsCount"] != DBNull.Value && _vSnapshotSoftLicKeys.Count > (int)ParentRow["InstallationsCount"])
            {
                var result = MessageBox.Show(@"Количество внесенных ключей превышает количество разрешенных установок данного ПО. " +
                    @"Вы уверены, что хотите сохранить изменения в базу данных?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (result != DialogResult.Yes)
                    return false;
            }
            return true;
        }

        private static SoftLicKey RowToSoftLicKey(DataRow row)
        {
            var softLicKey = new SoftLicKey
            {
                IdLicenseKey = ViewportHelper.ValueOrNull<int>(row, "ID LicenseKey"),
                IdLicense = ViewportHelper.ValueOrNull<int>(row, "ID License"),
                LicKey = ViewportHelper.ValueOrNull(row, "LicKey")
            };
            return softLicKey;
        }

        private List<SoftLicKey> SoftLicKeysFromViewport()
        {
            var list = new List<SoftLicKey>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var slk = new SoftLicKey();
                    var row = dataGridView.Rows[i];
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
            var list = new List<SoftLicKey>();
            for (var i = 0; i < _vSoftLicKeys.Count; i++)
            {
                var slk = new SoftLicKey();
                var row = ((DataRowView)_vSoftLicKeys[i]);
                slk.IdLicenseKey = ViewportHelper.ValueOrNull<int>(row, "ID LicenseKey");
                slk.IdLicense = ViewportHelper.ValueOrNull<int>(row, "ID License");
                slk.LicKey = ViewportHelper.ValueOrNull(row, "LicKey");
                list.Add(slk);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return _vSnapshotSoftLicKeys.Count;
        }

        public override void MoveFirst()
        {
            _vSnapshotSoftLicKeys.MoveFirst();
        }

        public override void MoveLast()
        {
            _vSnapshotSoftLicKeys.MoveLast();
        }

        public override void MoveNext()
        {
            _vSnapshotSoftLicKeys.MoveNext();
        }

        public override void MovePrev()
        {
            _vSnapshotSoftLicKeys.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return _vSnapshotSoftLicKeys.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return _vSnapshotSoftLicKeys.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (_vSnapshotSoftLicKeys.Position > -1) && (_vSnapshotSoftLicKeys.Position < (_vSnapshotSoftLicKeys.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (_vSnapshotSoftLicKeys.Position > -1) && (_vSnapshotSoftLicKeys.Position < (_vSnapshotSoftLicKeys.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            _softLicKeys = SoftLicKeysDataModel.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            _softLicKeys.Select();

            _vSoftLicKeys = new BindingSource
            {
                DataMember = "SoftLicKeys",
                Filter = StaticFilter
            };
            if (!string.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                _vSoftLicKeys.Filter += " AND ";
            _vSoftLicKeys.Filter += DynamicFilter;
            _vSoftLicKeys.DataSource = DataSetManager.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.License)
                Text = string.Format(CultureInfo.InvariantCulture, "Лицензионные ключи лицензии №{0}", ParentRow["ID License"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < _softLicKeys.Select().Columns.Count; i++)
                _snapshotsoftLicKeys.Columns.Add(new DataColumn(_softLicKeys.Select().Columns[i].ColumnName, _softLicKeys.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < _vSoftLicKeys.Count; i++)
                _snapshotsoftLicKeys.Rows.Add(DataRowViewToArray(((DataRowView)_vSoftLicKeys[i])));
            _vSnapshotSoftLicKeys = new BindingSource {DataSource = _snapshotsoftLicKeys};
            _vSnapshotSoftLicKeys.CurrentItemChanged += v_snapshotLicKeys_CurrentItemChanged;

            dataGridView.DataSource = _vSnapshotSoftLicKeys;
            idLicenseKey.DataPropertyName = "ID LicenseKey";
            idLicense.DataPropertyName = "ID License";
            LicKey.DataPropertyName = "LicKey";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            _softLicKeys.Select().RowChanged += SoftLicKeysViewport_RowChanged;
            _softLicKeys.Select().RowDeleting += SoftLicKeysViewport_RowDeleting;
            _softLicKeys.Select().RowDeleted += SoftLicKeysViewport_RowDeleted;
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
            var row = (DataRowView)_vSnapshotSoftLicKeys.AddNew();
            row["ID License"] = ParentRow["ID License"];
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (_vSnapshotSoftLicKeys.Position != -1) &&
                AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)_vSnapshotSoftLicKeys[_vSnapshotSoftLicKeys.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotsoftLicKeys.Clear();
            for (var i = 0; i < _vSoftLicKeys.Count; i++)
                _snapshotsoftLicKeys.Rows.Add(DataRowViewToArray((DataRowView)_vSoftLicKeys[i]));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() &&
                AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override void SaveRecord()
        {
            dataGridView.EndEdit();
            _syncViews = false;
            var list = SoftLicKeysFromViewport();
            if (!ValidateSoftLicKeys(list))
            {
                _syncViews = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = _softLicKeys.Select().Rows.Find(list[i].IdLicenseKey);
                if (row == null)
                {
                    var idLicKey = SoftLicKeysDataModel.Insert(list[i]);
                    if (idLicKey == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    ((DataRowView)_vSnapshotSoftLicKeys[i])["ID LicenseKey"] = idLicKey;
                    _softLicKeys.Select().Rows.Add(DataRowViewToArray((DataRowView)_vSnapshotSoftLicKeys[i]));
                }
                else
                {
                    var softLicKeyFromView = RowToSoftLicKey(row);
                    if (softLicKeyFromView == list[i])
                        continue;
                    if (SoftLicKeysDataModel.Update(list[i]) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    row["ID License"] = list[i].IdLicense == null ? DBNull.Value : (object)list[i].IdLicense;
                    row["LicKey"] = list[i].LicKey == null ? DBNull.Value : (object)list[i].LicKey;
                }
            }
            list = SoftLicKeysFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idLicenseKey"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["idLicenseKey"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idLicenseKey"].Value == list[i].IdLicenseKey))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (SoftLicKeysDataModel.Delete(list[i].IdLicenseKey.Value) == -1)
                {
                    _syncViews = true;
                    return;
                }
                _softLicKeys.Select().Rows.Find(list[i].IdLicenseKey).Delete();
            }
            _syncViews = true;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new SoftLicKeysViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения о лицевых счетах в базу данных?", @"Внимание",
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
            _softLicKeys.Select().RowChanged -= SoftLicKeysViewport_RowChanged;
            _softLicKeys.Select().RowDeleting -= SoftLicKeysViewport_RowDeleting;
            _softLicKeys.Select().RowDeleted -= SoftLicKeysViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            _softLicKeys.Select().RowChanged -= SoftLicKeysViewport_RowChanged;
            _softLicKeys.Select().RowDeleting -= SoftLicKeysViewport_RowDeleting;
            _softLicKeys.Select().RowDeleted -= SoftLicKeysViewport_RowDeleted;
            Close();
        }


        private void v_snapshotLicKeys_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        private void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "LicKey":
                    if (cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                        cell.ErrorText = "Номер лицензионного ключа не может быть пустым";
                    else
                        if (cell.Value.ToString().Trim().Length > 200)
                            cell.ErrorText = "Длина лицензионного ключа не может превышать 200 символов";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        private void SoftLicKeysViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
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

        private void SoftLicKeysViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = _vSnapshotSoftLicKeys.Find("ID LicenseKey", e.Row["ID LicenseKey"]);
            if (rowIndex != -1)
                ((DataRowView)_vSnapshotSoftLicKeys[rowIndex]).Delete();
        }

        private void SoftLicKeysViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            var rowIndex = _vSnapshotSoftLicKeys.Find("ID LicenseKey", e.Row["ID LicenseKey"]);
            if (rowIndex == -1 && _vSoftLicKeys.Find("ID LicenseKey", e.Row["ID LicenseKey"]) != -1)
            {
                _snapshotsoftLicKeys.Rows.Add(e.Row["ID LicenseKey"], e.Row["ID License"], e.Row["LicKey"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)_vSnapshotSoftLicKeys[rowIndex]);
                    row["ID License"] = e.Row["ID License"];
                    row["LicKey"] = e.Row["LicKey"];
                }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
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
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
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
