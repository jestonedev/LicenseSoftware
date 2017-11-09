using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using Security;
using System.Globalization;
using System.Linq;
using LicenseSoftware.DataModels.CalcDataModels;
using LicenseSoftware.DataModels.DataModels;

namespace LicenseSoftware.Viewport
{
    internal sealed class SoftVersionsViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idVersion;
        private DataGridViewTextBoxColumn idSoftware;
        private DataGridViewTextBoxColumn Version;
        #endregion Components

        #region Models

        private SoftVersionsDataModel _softVersions;
        private readonly DataTable _snapshotSoftVersions = new DataTable("snapshotSoftVersions");
        #endregion Models

        #region Views

        private BindingSource _vSoftVersions;
        private BindingSource _vSnapshotSoftVersions;
        #endregion Views



        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private SoftVersionsViewport()
            : this(null)
        {
        }

        public SoftVersionsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            _snapshotSoftVersions.Locale = CultureInfo.InvariantCulture;
        }

        public SoftVersionsViewport(Viewport softLicKeysViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = softLicKeysViewport.DynamicFilter;
            StaticFilter = softLicKeysViewport.StaticFilter;
            ParentRow = softLicKeysViewport.ParentRow;
            ParentType = softLicKeysViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = SoftVersionsFromView();
            var listFromViewport = SoftVersionsFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            for (var i = 0; i < listFromView.Count; i++)
            {
                var founded = false;
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
                dataRowView["ID Version"], 
                dataRowView["ID Software"], 
                dataRowView["Version"]
            };
        }

        private bool ValidateSoftVersions(List<SoftVersion> softVersionsParam)
        {
            foreach (var softLicKey in softVersionsParam)
            {
                if (softLicKey.Version != null && softLicKey.Version.Length > 50)
                {
                    MessageBox.Show(@"Длина наименования версии не может превышать 50 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                var localSoftLicKey = softLicKey;
                if (softVersionsParam.Count(v => v.Version == localSoftLicKey.Version) > 1)
                {
                    MessageBox.Show(string.Format("Вы пытаетесь добавить одну и ту же версию \"{0}\" дважды", localSoftLicKey.Version),
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftVersion RowToSoftVersion(DataRow row)
        {
            var softVersion = new SoftVersion
            {
                IdVersion = ViewportHelper.ValueOrNull<int>(row, "ID Version"),
                IdSoftware = ViewportHelper.ValueOrNull<int>(row, "ID Software"),
                Version = ViewportHelper.ValueOrNull(row, "Version")
            };
            return softVersion;
        }

        private List<SoftVersion> SoftVersionsFromViewport()
        {
            var list = new List<SoftVersion>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var row = dataGridView.Rows[i];
                    var sv = new SoftVersion
                    {
                        IdVersion = ViewportHelper.ValueOrNull<int>(row, "idVersion"),
                        IdSoftware = ViewportHelper.ValueOrNull<int>(row, "idSoftware"),
                        Version = ViewportHelper.ValueOrNull(row, "Version")
                    };
                    list.Add(sv);
                }
            }
            return list;
        }

        private List<SoftVersion> SoftVersionsFromView()
        {
            var list = new List<SoftVersion>();
            for (var i = 0; i < _vSoftVersions.Count; i++)
            {
                var row = (DataRowView)_vSoftVersions[i];
                var sv = new SoftVersion
                {
                    IdVersion = ViewportHelper.ValueOrNull<int>(row, "ID Version"),
                    IdSoftware = ViewportHelper.ValueOrNull<int>(row, "ID Software"),
                    Version = ViewportHelper.ValueOrNull(row, "Version")
                };
                list.Add(sv);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return _vSnapshotSoftVersions.Count;
        }

        public override void MoveFirst()
        {
            _vSnapshotSoftVersions.MoveFirst();
        }

        public override void MoveLast()
        {
            _vSnapshotSoftVersions.MoveLast();
        }

        public override void MoveNext()
        {
            _vSnapshotSoftVersions.MoveNext();
        }

        public override void MovePrev()
        {
            _vSnapshotSoftVersions.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return _vSnapshotSoftVersions.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return _vSnapshotSoftVersions.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (_vSnapshotSoftVersions.Position > -1) && (_vSnapshotSoftVersions.Position < (_vSnapshotSoftVersions.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (_vSnapshotSoftVersions.Position > -1) && (_vSnapshotSoftVersions.Position < (_vSnapshotSoftVersions.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            _softVersions = SoftVersionsDataModel.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            _softVersions.Select();

            _vSoftVersions = new BindingSource
            {
                DataMember = "SoftVersions",
                Filter = StaticFilter
            };
            if (!string.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                _vSoftVersions.Filter += " AND ";
            _vSoftVersions.Filter += DynamicFilter;
            _vSoftVersions.DataSource = DataSetManager.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.Software)
                Text = string.Format(CultureInfo.InvariantCulture, "Версии ПО №{0}", ParentRow["ID Software"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < _softVersions.Select().Columns.Count; i++)
                _snapshotSoftVersions.Columns.Add(new DataColumn(_softVersions.Select().Columns[i].ColumnName, _softVersions.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < _vSoftVersions.Count; i++)
                _snapshotSoftVersions.Rows.Add(DataRowViewToArray(((DataRowView)_vSoftVersions[i])));
            _vSnapshotSoftVersions = new BindingSource {DataSource = _snapshotSoftVersions};
            _vSnapshotSoftVersions.CurrentItemChanged += VSnapshotVersionsCurrentItemChanged;

            dataGridView.DataSource = _vSnapshotSoftVersions;
            idVersion.DataPropertyName = "ID Version";
            idSoftware.DataPropertyName = "ID Software";
            Version.DataPropertyName = "Version";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            _softVersions.Select().RowChanged += SoftLicKeysViewport_RowChanged;
            _softVersions.Select().RowDeleting += SoftLicKeysViewport_RowDeleting;
            _softVersions.Select().RowDeleted += SoftLicKeysViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return (ParentType == ParentTypeEnum.Software) && (ParentRow != null) &&
                AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            if ((ParentRow == null) || (ParentType != ParentTypeEnum.Software))
                return;
            var row = (DataRowView)_vSnapshotSoftVersions.AddNew();
            if (row == null) return;
            row["ID Software"] = ParentRow["ID Software"];
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (_vSnapshotSoftVersions.Position != -1) &&
                AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)_vSnapshotSoftVersions[_vSnapshotSoftVersions.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotSoftVersions.Clear();
            for (var i = 0; i < _vSoftVersions.Count; i++)
                _snapshotSoftVersions.Rows.Add(DataRowViewToArray(((DataRowView)_vSoftVersions[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() &&
                AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            dataGridView.EndEdit();
            _syncViews = false;
            var list = SoftVersionsFromViewport();
            if (!ValidateSoftVersions(list))
            {
                _syncViews = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = _softVersions.Select().Rows.Find(list[i].IdVersion);
                if (row == null)
                {
                    var idLicKey = SoftVersionsDataModel.Insert(list[i]);
                    if (idLicKey == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    ((DataRowView)_vSnapshotSoftVersions[i])["ID Version"] = idLicKey;
                    _softVersions.Select().Rows.Add(DataRowViewToArray((DataRowView)_vSnapshotSoftVersions[i]));
                }
                else
                {
                    var softLicKeyFromView = RowToSoftVersion(row);
                    if (softLicKeyFromView == list[i])
                        continue;
                    if (SoftVersionsDataModel.Update(list[i]) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    row["ID Software"] = list[i].IdSoftware == null ? DBNull.Value : (object)list[i].IdSoftware;
                    row["Version"] = list[i].Version == null ? DBNull.Value : (object)list[i].Version;
                }
            }
            list = SoftVersionsFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idVersion"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["idVersion"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idVersion"].Value == list[i].IdVersion))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (SoftVersionsDataModel.Delete(list[i].IdVersion.Value) == -1)
                {
                    _syncViews = true;
                    return;
                }
                _softVersions.Select().Rows.Find(list[i].IdVersion).Delete();
            }
            _syncViews = true;
            MenuCallback.EditingStateUpdate();
            if (CalcDataModelSoftwareConcat.HasInstance())
                CalcDataModelSoftwareConcat.GetInstance().Refresh(EntityType.Unknown, null, true);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new SoftVersionsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения о версиях в базу данных?", @"Внимание",
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
            _softVersions.Select().RowChanged -= SoftLicKeysViewport_RowChanged;
            _softVersions.Select().RowDeleting -= SoftLicKeysViewport_RowDeleting;
            _softVersions.Select().RowDeleted -= SoftLicKeysViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            _softVersions.Select().RowChanged -= SoftLicKeysViewport_RowChanged;
            _softVersions.Select().RowDeleting -= SoftLicKeysViewport_RowDeleting;
            _softVersions.Select().RowDeleted -= SoftLicKeysViewport_RowDeleted;
            Close();
        }


        public override bool HasAssocLicenses()
        {
            return (_vSnapshotSoftVersions.Position > -1) &&
                AccessControl.HasPrivelege(Priveleges.LicensesRead);
        }

        public override void ShowAssocLicenses()
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Перед открытием связных объектов необходимо сохранить изменения в базу данных. " +
                    @"Вы хотите это сделать?", @"Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                switch (result)
                {
                    case DialogResult.Yes:
                        SaveRecord();
                        break;
                    case DialogResult.No:
                        CancelRecord();
                        break;
                    default:
                        return;
                }
            }
            if (_vSnapshotSoftVersions.Position == -1)
            {
                MessageBox.Show(@"Не выбрана версия ПО для отображения списка лицензий", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, ViewportType.LicensesViewport,
                "[ID Version] = " + Convert.ToInt32(((DataRowView)_vSnapshotSoftVersions[_vSnapshotSoftVersions.Position])["ID Version"], CultureInfo.InvariantCulture),
                ((DataRowView)_vSnapshotSoftVersions[_vSnapshotSoftVersions.Position]).Row,
                ParentTypeEnum.SoftVersion);
        }

        private void VSnapshotVersionsCurrentItemChanged(object sender, EventArgs e)
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
                case "Version":
                    cell.ErrorText = cell.Value.ToString().Trim().Length > 50 ? "Длина лицензионного ключа не может превышать 50 символов" : "";
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
            var rowIndex = _vSnapshotSoftVersions.Find("ID Version", e.Row["ID Version"]);
            if (rowIndex != -1)
                ((DataRowView)_vSnapshotSoftVersions[rowIndex]).Delete();
        }

        private void SoftLicKeysViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            var rowIndex = _vSnapshotSoftVersions.Find("ID Version", e.Row["ID Version"]);
            if (rowIndex == -1 && _vSoftVersions.Find("ID Version", e.Row["ID Version"]) != -1)
            {
                _snapshotSoftVersions.Rows.Add(e.Row["ID Version"], e.Row["ID Software"], e.Row["Version"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)_vSnapshotSoftVersions[rowIndex]);
                    row["ID Software"] = e.Row["ID Software"];
                    row["Version"] = e.Row["Version"];
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftVersionsViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.idVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idSoftware = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Version = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.idVersion,
            this.idSoftware,
            this.Version});
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
            // idVersion
            // 
            this.idVersion.HeaderText = "Внутренний номер версии ПО";
            this.idVersion.Name = "idVersion";
            this.idVersion.Visible = false;
            // 
            // idSoftware
            // 
            this.idSoftware.HeaderText = "Внутренний номер ПО";
            this.idSoftware.Name = "idSoftware";
            this.idSoftware.Visible = false;
            // 
            // Version
            // 
            this.Version.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Version.HeaderText = "Версия ПО";
            this.Version.MinimumWidth = 150;
            this.Version.Name = "Version";
            // 
            // SoftVersionsViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(801, 339);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SoftVersionsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Перечень версий программного обеспечения";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
