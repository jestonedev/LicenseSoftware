using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using System.Reflection;
using System.Threading;
using System.Globalization;

namespace LicenseSoftware
{
    static class Program
    {
        private const string m_appName = "LicenseSoftware";  
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool canCreateNewApp;
            using (Mutex mutex = new Mutex(true, m_appName, out canCreateNewApp))
            {
                if (canCreateNewApp)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    if (args.Length > 0 && args.Contains("--config"))
                    {
                        using (SettingsForm sf = new SettingsForm())
                        {
                            sf.ShowDialog();
                        }
                    }
                    try
                    {
                        Application.Run(new MainForm());
                    }
                    catch (TargetInvocationException)
                    {
                        //На данный момент не знаю, чем вызвано данное исключение, скорее всего Ribbon-панелью
                    }
                }
                else
                {
                    MessageBox.Show("Приложение уже запущено", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
        }
    }
}
