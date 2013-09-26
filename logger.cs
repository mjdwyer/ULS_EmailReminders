using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServiceEmailReminders
{
    class logger
    {
        private string mstr_logFilePath;
        private string mstr_logPath;

        public void SetLogPath(string strPath)
        {
            try
            {
                if (!strPath.EndsWith("\\\\"))
                {
                    strPath += '\\';
                }
                this.mstr_logPath = strPath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void WriteToAppLog(string strMsg, string strMsgType)
        {
            try
            {
                this.SetLogFile(this.mstr_logPath);
                FileInfo fileInfo = new FileInfo(this.mstr_logFilePath);
                if (fileInfo.Exists && fileInfo.LastWriteTime.AddDays(5.0) < DateTime.Now)
                {
                    fileInfo.Delete();
                }
                if (!fileInfo.Exists)
                {
                    FileStream fileStream = new FileStream(this.mstr_logFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamWriter streamWriter = new StreamWriter(fileStream);
                    streamWriter.Close();
                    fileStream.Close();
                }
                FileStream fileStream2 = new FileStream(this.mstr_logFilePath, FileMode.Append, FileAccess.Write);
                StreamWriter streamWriter2 = new StreamWriter(fileStream2);
                string text = " ==> ";
                streamWriter2.Write(string.Concat(new string[]
			{
				DateTime.Now.ToString(),
				text,
				strMsgType,
				": ",
				strMsg,
				"\r\n"
			}));
                streamWriter2.Close();
                fileStream2.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetLogFile(string strPath)
        {
            try
            {
                string str = "";
                switch (DateTime.Now.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        str = "Sun";
                        break;
                    case DayOfWeek.Monday:
                        str = "Mon";
                        break;
                    case DayOfWeek.Tuesday:
                        str = "Tue";
                        break;
                    case DayOfWeek.Wednesday:
                        str = "Wed";
                        break;
                    case DayOfWeek.Thursday:
                        str = "Thu";
                        break;
                    case DayOfWeek.Friday:
                        str = "Fri";
                        break;
                    case DayOfWeek.Saturday:
                        str = "Sat";
                        break;
                }
                this.mstr_logFilePath = strPath + str + "_Email.log";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string SpiltOutExeName(string strFullExePath)
        {
            char c = Convert.ToChar("\\");
            string result;
            try
            {
                string[] array = strFullExePath.Split(new char[]
			{
				c
			});
                int num = array.Length - 1;
                string text = array[num];
                result = text;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
