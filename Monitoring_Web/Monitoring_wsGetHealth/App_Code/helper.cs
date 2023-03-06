using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring_wsGetHealth.App_Code
{
    public class helper
    {
        public static void WriteFileLog(string Message, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "TraceLog_"+DateTime.Now.ToString("yyyy-MM-dd");
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Logs\\" + fileName + ".txt";
            Message = string.Format("{0} - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Message);
            if (!File.Exists(filepath))
            {
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }

        }

    }
}
