using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using Settings;

namespace LicenseSoftware.Reporting
{
    public class Reporter
    {
        public event EventHandler<EventArgs> ReportComplete;
        public event EventHandler<EventArgs> ReportCanceled;
        public event EventHandler<ReportOutputStreamEventArgs> ReportOutputStreamResponse;
        public string ReportTitle { get; set; }

        public Reporter()
        {
            ReportTitle = "Unknown report";
        }

        public virtual void Run()
        {
            Run(new Dictionary<string, string>());
        }

        public virtual void Run(Dictionary<string, string> arguments)
        {
            if (!File.Exists(LicenseSoftwareSettings.ActivityManagerPath))
            {
                MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                    "Не удалось найти генератор отчетов ActivityManager. Возможно указанный путь {0} является некорректным.",
                    LicenseSoftwareSettings.ActivityManagerPath), 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            var context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem((args) =>
            {
                using (var process = new Process())
                {
                    var psi = new ProcessStartInfo(LicenseSoftwareSettings.ActivityManagerPath,
                        GetArguments((Dictionary<string, string>) args))
                    {
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        StandardOutputEncoding =
                            Encoding.GetEncoding(LicenseSoftwareSettings.ActivityManagerOutputCodePage),
                        UseShellExecute = false
                    };
                    process.StartInfo = psi;
                    process.Start();
                    if (ReportOutputStreamResponse != null)
                    {
                        var reader = process.StandardOutput;
                        do
                        {
                            var line = reader.ReadLine();
                            context.Post(
                                _ =>
                                {
                                    try
                                    {
                                        ReportOutputStreamResponse(this, new ReportOutputStreamEventArgs(line));
                                    }
                                    catch (NullReferenceException)
                                    {
                                        //Исключение происходит, когда подписчики отписываются после проверки условия на null
                                    }
                                }, null);
                        } while (!process.HasExited && ReportOutputStreamResponse != null);
                    }
                    process.WaitForExit();
                }
                if (ReportComplete != null)
                    context.Post(
                        _ =>
                        {
                            try
                            {
                                ReportComplete(this, new EventArgs());
                            }
                            catch (NullReferenceException)
                            {
                                //Исключение происходит, когда подписчики отписываются после проверки условия на null
                            }
                        }, null);
            }, arguments);
        }

        private static string GetArguments(Dictionary<string, string> arguments)
        {
            var argumentsString = "";
            foreach (var argument in arguments)
                argumentsString += string.Format(CultureInfo.InvariantCulture, "{0}=\"{1}\" ", 
                    argument.Key.Replace("\"", "\\\""), 
                    argument.Value.Replace("\"", "\\\""));
            return argumentsString;
        }

        public virtual void Cancel()
        {
            if (ReportCanceled == null) return;
            try
            {
                ReportCanceled(this, new EventArgs());
            }
            catch (NullReferenceException)
            {
                //Исключение происходит, когда подписчики отписываются после проверки условия на null в многопоточном режиме
            }
        }

        public virtual void Run(List<string> arguments)
        {
            Run(new Dictionary<string, string>());
        }
    }
}
