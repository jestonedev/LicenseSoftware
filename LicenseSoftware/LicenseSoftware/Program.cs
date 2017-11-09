using System;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;

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
                        using (var sf = new SettingsForm())
                        {
                            sf.ShowDialog();
                        }
                    }
                    try
                    {
                        Application.Run(new MainForm(args));
                    }
                    catch (TargetInvocationException)
                    {
                        //На данный момент не знаю, чем вызвано данное исключение, скорее всего Ribbon-панелью
                    }
                }
                else
                {
                    MessageBox.Show(@"Приложение уже запущено", @"Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
        }
    }
}
