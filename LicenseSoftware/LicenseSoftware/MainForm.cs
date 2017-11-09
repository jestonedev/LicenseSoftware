using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using LicenseSoftware.Viewport;
using LicenseSoftware.DataModels;
using WeifenLuo.WinFormsUI.Docking;
using LicenseSoftware.Reporting;
using Security;
using System.Text.RegularExpressions;
using LicenseSoftware.DataModels.DataModels;
using Settings;

namespace LicenseSoftware
{
    public partial class MainForm : Form, IMenuCallback
    {
        private readonly ReportLogForm _reportLogForm = new ReportLogForm();
        private int _reportCounter;
        private readonly string _computerNameCommandLineArg;

        private void ChangeViewportsSelectProprty()
        {
            for (var i = dockPanel.Documents.Count() - 1; i >= 0; i--)
            {
                var document = dockPanel.Documents.ElementAt(i) as IMenuController;
                if (document != null)
                    document.Selected = false;
            }
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).Selected = true;
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
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                toolStripLabelRecordCount.Text = @"Всего записей: " + document.GetRecordCount();
            else
                toolStripLabelRecordCount.Text = "";
        }

        public MainForm(IEnumerable<string> args)
        {
            InitializeComponent();
            var arguments = args.Select(r => r.Split(new[] { '=' }, 2));
            var computerCommandLineArg = arguments.FirstOrDefault(r => r.Length > 1 && r[0] == "--computer");
            if (computerCommandLineArg != null)
            {
                _computerNameCommandLineArg = computerCommandLineArg[1];
            }
            ChangeMainMenuState();
            StatusBarStateUpdate();
        }

        private void PreLoadData()
        {
            toolStripProgressBar.Maximum = 13;
            DepartmentsDataModel.GetInstance(toolStripProgressBar, 1);
            DevicesDataModel.GetInstance(toolStripProgressBar, 1);
            SoftwareDataModel.GetInstance(toolStripProgressBar, 1);
            SoftVersionsDataModel.GetInstance(toolStripProgressBar, 1);
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
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
            {
                document.Close();
            }
        }

        private void ribbonButtonTabsClose_Click(object sender, EventArgs e)
        {
            for (var i = dockPanel.Documents.Count() - 1; i >= 0; i--)
            {
                var document = dockPanel.Documents.ElementAt(i) as IMenuController;
                if (document != null)
                    document.Close();
            }
        }

        private void ribbonButtonTabCopy_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            var viewport = ((IMenuController) dockPanel.ActiveDocument).Duplicate();
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
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).MoveFirst();
            NavigationStateUpdate();
        }

        private void ribbonButtonLast_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).MoveLast();
            NavigationStateUpdate();
        }

        private void ribbonButtonPrev_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).MovePrev();
            NavigationStateUpdate();
        }

        private void ribbonButtonNext_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            ((IMenuController) dockPanel.ActiveDocument).MoveNext();
            NavigationStateUpdate();
        }

        private void ribbonButtonSearch_Click(object sender, EventArgs e)
        {
            if (!(dockPanel.ActiveDocument is IMenuController))
                return;
            if (ribbonButtonSearch.Checked)
                ((IMenuController) dockPanel.ActiveDocument).ClearSearch();
            else
                ((IMenuController) dockPanel.ActiveDocument).SearchRecord();
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

        private void ribbonButtonOpen_Click()
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.OpenDetails();
        }

        public void TabsStateUpdate()
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            ribbonButtonTabCopy.Enabled = document != null && document.CanDuplicate();
            ribbonButtonTabClose.Enabled = document != null;
            ribbonButtonTabsClose.Enabled = dockPanel.Documents.Any();  
        }

        public void NavigationStateUpdate()
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            ribbonButtonFirst.Enabled = document != null && document.CanMoveFirst();
            ribbonButtonPrev.Enabled = document != null && document.CanMovePrev();
            ribbonButtonNext.Enabled = document != null && document.CanMoveNext();
            ribbonButtonLast.Enabled = document != null && document.CanMoveLast();
            ribbonButtonSearch.Enabled = document != null && document.CanSearchRecord();
            ribbonButtonSearch.Checked = document != null && document.SearchedRecords();
            ribbonButtonOpen.Enabled = document != null && document.CanOpenDetails();
        }

        public void EditingStateUpdate()
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            ribbonButtonDeleteRecord.Enabled = document != null && document.CanDeleteRecord();
            ribbonButtonInsertRecord.Enabled = document != null && document.CanInsertRecord();
            ribbonButtonCopyRecord.Enabled = document != null && document.CanCopyRecord();
            ribbonButtonCancel.Enabled = document != null && document.CanCancelRecord();
            ribbonButtonSave.Enabled = document != null && document.CanSaveRecord();
        }

        public void RelationsStateUpdate()
        {
            ribbonPanelRelations.Items.Clear();
            var hasActiveDocument = dockPanel.ActiveDocument is IMenuController;
            ribbon1.SuspendUpdating();
            
            if (hasActiveDocument && ((IMenuController) dockPanel.ActiveDocument).HasAssocLicenses())
                ribbonPanelRelations.Items.Add(ribbonButtonLicenses);
            if (hasActiveDocument && ((IMenuController) dockPanel.ActiveDocument).HasAssocInstallations())
                ribbonPanelRelations.Items.Add(ribbonButtonInstallations);
            if (hasActiveDocument && ((IMenuController) dockPanel.ActiveDocument).HasAssocLicKeys())
                ribbonPanelRelations.Items.Add(ribbonButtonLicKeys);
            if (hasActiveDocument && ((IMenuController) dockPanel.ActiveDocument).HasAssocSoftVersions())
                ribbonPanelRelations.Items.Add(ribbonButtonSoftVersions);

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
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.SaveRecord();
            NavigationStateUpdate();
            RelationsStateUpdate();
        }

        private void ribbonButtonDeleteRecord_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.DeleteRecord();
            NavigationStateUpdate();
            EditingStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonInsertRecord_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.InsertRecord();
            EditingStateUpdate();
            NavigationStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonCancel_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.CancelRecord();
            NavigationStateUpdate();
            RelationsStateUpdate();
            StatusBarStateUpdate();
        }

        private void ribbonButtonCopyRecord_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.CopyRecord();
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
            for (var i = dockPanel.Documents.Count() - 1; i >= 0; i--)
                if (dockPanel.Documents.ElementAt(i) is IMenuController)
                {
                    var document = dockPanel.Documents.ElementAt(i) as IMenuController;
                    if (document != null)
                        document.Close();
                }
            if (dockPanel.Documents.Count() != 0)
                e.Cancel = true;
        }

        public void ForceCloseDetachedViewports()
        {
            for (var i = dockPanel.Documents.Count() - 1; i >= 0; i--)
            {
                var document = dockPanel.Documents.ElementAt(i) as IMenuController;
                if (document != null && document.ViewportDetached())
                    document.ForceClose();
            }
        }

        private void ribbonButtonAssocLicenses_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocLicenses();
        }

        private void ribbonButtonAssocInstallations_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocInstallations();
        }

        private void ribbonButtonLicKeys_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocLicKeys();
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

        private void ribbonButtonSoftVersions_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
                document.ShowAssocSoftVersions();
        }

        private void CreateViewport(ViewportType viewportType)
        {
            var viewport = ViewportFactory.CreateViewport(this, viewportType);
            if (((IMenuController) viewport).CanLoadData())
                ((IMenuController) viewport).LoadData();
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
            if (LicenseSoftwareSettings.UseLdap)
                user = UserDomain.Current;
            if (user == null)
                toolStripLabelHelloUser.Text = "";
            else
                toolStripLabelHelloUser.Text = @"Здравствуйте, " + user.DisplayName;
            //Загружаем права пользователя
            AccessControl.LoadPriveleges();
            if (AccessControl.HasNoPriveleges())
            {
                MessageBox.Show(@"У вас нет прав на использование данного приложения", @"Ошибка",
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
            if (string.IsNullOrEmpty(_computerNameCommandLineArg)) return;
            var device = DataModelHelper.FilterRows(DevicesDataModel.GetInstance().Select())
                .FirstOrDefault(r => r.Field<string>("Device Name").Contains(_computerNameCommandLineArg));
            if (device == null) return;
            var viewport = ViewportFactory.CreateViewport(this, ViewportType.InstallationsViewport);
            viewport.DynamicFilter = string.Format("[ID Computer] = {0}", device.Field<int>("ID Device"));

            if (((IMenuController)viewport).CanLoadData())
                ((IMenuController)viewport).LoadData();
            AddViewport(viewport);
            ChangeMainMenuState();
            StatusBarStateUpdate();
            ChangeViewportsSelectProprty();
        }

        private void RunReport(ReporterType reporterType)
        {
            var reporter = ReporterFactory.CreateReporter(reporterType);
            reporter.ReportOutputStreamResponse += reporter_ReportOutputStreamResponse;
            reporter.ReportComplete += reporter_ReportComplete;
            reporter.ReportCanceled += reporter_ReportCanceled;
            _reportCounter++;
            reporter.Run();
        }

        void reporter_ReportCanceled(object sender, EventArgs e)
        {
            _reportCounter--;
            if (_reportCounter == 0)
                _reportLogForm.Hide();
        }

        void reporter_ReportComplete(object sender, EventArgs e)
        {
            _reportLogForm.Log("[" + ((Reporter)sender).ReportTitle + "]: Формирвоание отчета закончено");
            _reportCounter--;
            if (_reportCounter == 0)
                _reportLogForm.Hide();
        }

        void reporter_ReportOutputStreamResponse(object sender, ReportOutputStreamEventArgs e)
        {
            if (_reportLogForm.Visible == false)
                _reportLogForm.Show(dockPanel, DockState.DockBottomAutoHide);
            if (!String.IsNullOrEmpty(e.Text.Trim()) && (!Regex.IsMatch(e.Text.Trim(), "styles.xml")))
                _reportLogForm.Log("["+((Reporter)sender).ReportTitle+"]: "+e.Text.Trim());
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
                    ribbonButtonOpen_Click();
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

        private void ribbonButtonRepDepart_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document is LicensesViewport)
            {
                var arguments = DepRepArguments();
                RunDepReport(ReporterType.DepartmentReporter, arguments);
            }
            else
                MessageBox.Show(@"Для вывода отчета необходима активная вкладка ""Лицензии""", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }
        private List<string> DepRepArguments()
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
            {
                var arguments = document.GetIdLicenses();
                return arguments;
            }
            return new List<string>();
        }

        private void RunDepReport(ReporterType reporterType, List<string> args)
        {
            var reporter = ReporterFactory.CreateReporter(reporterType);
            reporter.ReportOutputStreamResponse += reporter_ReportOutputStreamResponse;
            reporter.ReportComplete += reporter_ReportComplete;
            reporter.ReportCanceled += reporter_ReportCanceled;
            _reportCounter++;
            reporter.Run(args);
        }

        private void ribbonButtonRepPC_Click(object sender, EventArgs e)
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document is InstallationsViewport)
            {
                var arguments = PcRepArguments();
                RunDepReport(ReporterType.PcReporter, arguments);
            }
            else
                MessageBox.Show(@"Для вывода отчета необходима активная вкладка ""Установки""", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }
        private List<string> PcRepArguments()
        {
            var document = dockPanel.ActiveDocument as IMenuController;
            if (document != null)
            {
                var arguments = document.GetIdInstallations();
                return arguments;
            }
            return new List<string>();
        }
    }
}
