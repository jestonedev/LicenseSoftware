using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using LicenseSoftware.Viewport;
using LicenseSoftware.DataModels;
using WeifenLuo.WinFormsUI.Docking;
using LicenseSoftware.Reporting;
using Security;
using System.Text.RegularExpressions;
using System.Threading;
using LicenseSoftware.SearchForms;

namespace LicenseSoftware
{
    public partial class MainForm : Form, IMenuCallback
    {
        private ReportLogForm reportLogForm = new ReportLogForm();
        private int reportCounter = 0;

        private void ChangeViewportsSelectProprty()
        {
            for (int i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                (dockPanel.Documents.ElementAt(i) as IMenuController).Selected = false;
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).Selected = true;
        }

        private void ChangeMainMenuState()
        {
            TabsStateUpdate();
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
        }

        public void StatusBarStateUpdate()
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                toolStripLabelRecordCount.Text = "Всего записей: " + (dockPanel.ActiveDocument as IMenuController).GetRecordCount();
            else
                toolStripLabelRecordCount.Text = "";
        }

        public MainForm()
        {
            InitializeComponent();
            ChangeMainMenuState();
            StatusBarStateUpdate();
        }

        private void PreLoadData()
        {
            toolStripProgressBar.Maximum = 12;
            DepartmentsDataModel.GetInstance(toolStripProgressBar, 1);
            DevicesDataModel.GetInstance(toolStripProgressBar, 1);
            SoftwareDataModel.GetInstance(toolStripProgressBar, 1);
            SoftInstallationsDataModel.GetInstance(toolStripProgressBar, 1);
            SoftInstallatorsDataModel.GetInstance(toolStripProgressBar, 1);
            SoftLicDocTypesDataModel.GetInstance(toolStripProgressBar, 1);
            SoftLicensesDataModel.GetInstance(toolStripProgressBar, 1);
            SoftLicKeysDataModel.GetInstance(toolStripProgressBar, 1);
            SoftLicTypesDataModel.GetInstance(toolStripProgressBar, 1);
            SoftMakersDataModel.GetInstance(toolStripProgressBar, 1);
            SoftSuppliersDataModel.GetInstance(toolStripProgressBar, 1);
            SoftTypesDataModel.GetInstance(toolStripProgressBar, 1);
        }

        private void ribbonButtonTabClose_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).Close();
        }

        private void ribbonButtonTabsClose_Click(object sender, EventArgs e)
        {
            for (int i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                if (dockPanel.Documents.ElementAt(i) as IMenuController != null)
                    (dockPanel.Documents.ElementAt(i) as IMenuController).Close();
        }

        private void ribbonButtonTabCopy_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            LicenseSoftware.Viewport.Viewport viewport = (dockPanel.ActiveDocument as IMenuController).Duplicate();
            viewport.Show(dockPanel, DockState.Document);
        }

        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            ChangeMainMenuState();
            StatusBarStateUpdate();
            ChangeViewportsSelectProprty();
        }

        private void ribbonButtonFirst_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).MoveFirst();
            NavigationStateUpdate();
        }

        private void ribbonButtonLast_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).MoveLast();
            NavigationStateUpdate();
        }

        private void ribbonButtonPrev_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).MovePrev();
            NavigationStateUpdate();
        }

        private void ribbonButtonNext_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            (dockPanel.ActiveDocument as IMenuController).MoveNext();
            NavigationStateUpdate();
        }

        private void ribbonButtonSearch_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument == null) || (dockPanel.ActiveDocument as IMenuController == null))
                return;
            if (ribbonButtonSearch.Checked)
                (dockPanel.ActiveDocument as IMenuController).ClearSearch();
            else
                (dockPanel.ActiveDocument as IMenuController).SearchRecord();
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        public void SearchButtonToggle(bool value)
        {
            ribbonButtonSearch.Checked = value;
        }

        public void AddViewport(Viewport.Viewport viewport)
        {
            if (viewport != null)
                viewport.Show(dockPanel, DockState.Document);
        }

        private void ribbonButtonOpen_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).OpenDetails();
        }

        public void TabsStateUpdate()
        {
            ribbonButtonTabCopy.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanDuplicate();
            ribbonButtonTabClose.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null);
            ribbonButtonTabsClose.Enabled = (dockPanel.Documents.Count() > 0);  
        }

        public void NavigationStateUpdate()
        {
            ribbonButtonFirst.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanMoveFirst();
            ribbonButtonPrev.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanMovePrev();
            ribbonButtonNext.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanMoveNext();
            ribbonButtonLast.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanMoveLast();
            ribbonButtonSearch.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanSearchRecord();
            ribbonButtonSearch.Checked = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).SearchedRecords();
            ribbonButtonOpen.Enabled = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanOpenDetails();
        }

        public void EditingStateUpdate()
        {
            ribbonButtonDeleteRecord.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanDeleteRecord();
            ribbonButtonInsertRecord.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanInsertRecord();
            ribbonButtonCopyRecord.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanCopyRecord();
            ribbonButtonCancel.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanCancelRecord();
            ribbonButtonSave.Enabled = (dockPanel.ActiveDocument != null) && 
                (dockPanel.ActiveDocument as IMenuController != null) && (dockPanel.ActiveDocument as IMenuController).CanSaveRecord();
        }

        public void RelationsStateUpdate()
        {
            ribbonPanelRelations.Items.Clear();
            bool hasActiveDocument = (dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null);
            ribbon1.SuspendUpdating();
            
            if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocLicenses())
                ribbonPanelRelations.Items.Add(ribbonButtonLicenses);
            if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocInstallations())
                ribbonPanelRelations.Items.Add(ribbonButtonInstallations);
            if (hasActiveDocument && (dockPanel.ActiveDocument as IMenuController).HasAssocLicKeys())
                ribbonPanelRelations.Items.Add(ribbonButtonLicKeys);

            if (ribbonPanelRelations.Items.Count == 0)
                ribbonTabGeneral.Panels.Remove(ribbonPanelRelations);
            else
                if (!ribbonTabGeneral.Panels.Contains(ribbonPanelRelations))
                    ribbonTabGeneral.Panels.Insert(2, ribbonPanelRelations);
            ribbon1.ResumeUpdating(true);
        }

        private void MainMenuStateUpdate()
        {
            // Empty
        }

        private void ribbonButtonSave_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).SaveRecord();
            NavigationStateUpdate();
            RelationsStateUpdate();
        }

        private void ribbonButtonDeleteRecord_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).DeleteRecord();
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonInsertRecord_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).InsertRecord();
            EditingStateUpdate();
            NavigationStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonCancel_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).CancelRecord();
            NavigationStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonCopyRecord_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).CopyRecord();
            EditingStateUpdate();
            NavigationStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonOrbMenuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                if (dockPanel.Documents.ElementAt(i) as IMenuController != null)
                    (dockPanel.Documents.ElementAt(i) as IMenuController).Close();
            if (dockPanel.Documents.Count() != 0)
                e.Cancel = true;
        }

        public void ForceCloseDetachedViewports()
        {
            for (int i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                if ((dockPanel.Documents.ElementAt(i) as IMenuController != null) && (dockPanel.Documents.ElementAt(i) as IMenuController).ViewportDetached())
                    (dockPanel.Documents.ElementAt(i) as IMenuController).ForceClose();
        }

        private void ribbonButtonAssocLicenses_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowAssocLicenses();
        }

        private void ribbonButtonAssocInstallations_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowAssocInstallations();
        }

        private void ribbonButtonLicKeys_Click(object sender, EventArgs e)
        {
            if ((dockPanel.ActiveDocument != null) && (dockPanel.ActiveDocument as IMenuController != null))
                (dockPanel.ActiveDocument as IMenuController).ShowAssocLicKeys();
        }

        private void ribbonOrbMenuItemSoftware_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.SoftwareViewport);
        }

        private void ribbonOrbMenuItemLicenses_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.LicensesViewport);
        }

        private void ribbonOrbMenuItemInstallations_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.InstallationsViewport);
        }

        private void ribbonOrbRecentItemSoftType_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.SoftTypesViewport);
        }

        private void ribbonOrbRecentItemSuppliers_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.SoftSuppliersViewport);
        }

        private void ribbonOrbRecentItemSoftMakers_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.SoftMakersViewport);
        }

        private void ribbonOrbRecentItemDocTypes_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.SoftLicDocTypesViewport);
        }

        private void ribbonOrbRecentItemLicTypes_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.SoftLicTypesViewport);
        }

        private void ribbonOrbRecentItemInstallers_Click(object sender, EventArgs e)
        {
            CreateViewport(ViewportType.InstallatorsViewport);
        }

        private void CreateViewport(ViewportType viewportType)
        {
            LicenseSoftware.Viewport.Viewport viewport = LicenseSoftware.Viewport.ViewportFactory.CreateViewport(this, viewportType);
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            AddViewport(viewport);
            ChangeMainMenuState();
            StatusBarStateUpdate();
            ChangeViewportsSelectProprty();
        }

        public void SwitchToPreviousViewport()
        {
            if (dockPanel.ActiveDocument != null && dockPanel.ActiveDocument.DockHandler.PreviousActive != null)
                dockPanel.ActiveDocument.DockHandler.PreviousActive.DockHandler.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UserDomain user = null;
            if (LicenseSoftwareSettings.UseLDAP)
                user = UserDomain.Current;
            if (user == null)
                toolStripLabelHelloUser.Text = "";
            else
                toolStripLabelHelloUser.Text = "Здравствуйте, " + user.DisplayName;
            //Загружаем права пользователя
            AccessControl.LoadPriveleges();
            if (AccessControl.HasNoPriveleges())
            {
                MessageBox.Show("У вас нет прав на использование данного приложения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                Application.Exit();
                return;
            }
            //Инициируем начальные параметры CallbackUpdater
            DataModelsCallbackUpdater.GetInstance().Initialize();
            //Загружаем данные в асинхронном режиме
            PreLoadData();
            //Обновляем состояние главного меню
            MainMenuStateUpdate();
        }

        private void RunReport(Reporting.ReporterType reporterType)
        {
            Reporter reporter = Reporting.ReporterFactory.CreateReporter(reporterType);
            reporter.ReportOutputStreamResponse += new EventHandler<ReportOutputStreamEventArgs>(reporter_ReportOutputStreamResponse);
            reporter.ReportComplete += new EventHandler<EventArgs>(reporter_ReportComplete);
            reporter.ReportCanceled += new EventHandler<EventArgs>(reporter_ReportCanceled);
            reportCounter++;
            reporter.Run();
        }

        void reporter_ReportCanceled(object sender, EventArgs e)
        {
            reportCounter--;
            if (reportCounter == 0)
                reportLogForm.Hide();
        }

        void reporter_ReportComplete(object sender, EventArgs e)
        {
            reportLogForm.Log("[" + ((Reporter)sender).ReportTitle + "]: Формирвоание отчета закончено");
            reportCounter--;
            if (reportCounter == 0)
                reportLogForm.Hide();
        }

        void reporter_ReportOutputStreamResponse(object sender, ReportOutputStreamEventArgs e)
        {
            if (reportLogForm.Visible == false)
                reportLogForm.Show(dockPanel, DockState.DockBottomAutoHide);
            if (!String.IsNullOrEmpty(e.Text.Trim()) && (!Regex.IsMatch(e.Text.Trim(), "styles.xml")))
                reportLogForm.Log("["+((Reporter)sender).ReportTitle+"]: "+e.Text.Trim());
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                if (ribbonButtonSave.Enabled)
                    ribbonButtonSave_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.Z))
            {
                if (ribbonButtonCancel.Enabled)
                    ribbonButtonCancel_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.N))
            {
                if (ribbonButtonInsertRecord.Enabled)
                    ribbonButtonInsertRecord_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.O))
            {
                if (ribbonButtonOpen.Enabled)
                    ribbonButtonOpen_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.Home))
            {
                if (ribbonButtonFirst.Enabled)
                    ribbonButtonFirst_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.End))
            {
                if (ribbonButtonLast.Enabled)
                    ribbonButtonLast_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.Left))
            {
                if (ribbonButtonPrev.Enabled)
                    ribbonButtonPrev_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.Right))
            {
                if (ribbonButtonNext.Enabled)
                    ribbonButtonNext_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.F))
            {
                if (ribbonButtonSearch.Enabled)
                    ribbonButtonSearch_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Q))
            {
                if (ribbonButtonTabClose.Enabled)
                    ribbonButtonTabClose_Click(this, new EventArgs());
                return true;
            }
            if (keyData == (Keys.Control | Keys.Alt | Keys.Q))
            {
                if (ribbonButtonTabsClose.Enabled)
                    ribbonButtonTabsClose_Click(this, new EventArgs());
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ribbonButtonLogInstallationsReport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.LogInstallationsReporter);
        }

        private void ribbonButtonLogLicensesReport_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.LogLicensesReporter);
        }

        private void ribbonButtonLicensesBySoftCount_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.LicensesBySoftCountReporter);
        }

        private void ribbonButtonInstallationsInfo_Click(object sender, EventArgs e)
        {
            RunReport(ReporterType.InstallationsInfoReporter);
        }

        private void ribbonOrbMenuItemHelp_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @"Documentation\Руководство пользователя.odt");
            if (!File.Exists(fileName))
            {
                MessageBox.Show(
                    string.Format("Не удалось найти руководство пользователя по пути {0}", fileName),
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            using (var process = new Process())
            {
                var psi = new ProcessStartInfo(fileName);
                process.StartInfo = psi;
                process.Start();
            }
        }
    }
}
