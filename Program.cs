using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace ServiceEmailReminders
{
    static class Program
    {
        private static logger log = new logger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            log.SetLogPath(Properties.Settings.Default.logpath);
            log.WriteToAppLog("Main: goto here", "ERROR");
            ServiceEmailReminders();
            QualificationExpireEmailReminders();
            
        }
        private static void ServiceEmailReminders()
        {
            ulsdbDataContextDataContext ulsdbDataContext = new ulsdbDataContextDataContext();
            List<svc_appointment> list = (
                from s in ulsdbDataContext.svc_appointments
                where s.svc_date == DateTime.Now.Date.AddDays(1.0)
                select s).ToList<svc_appointment>();
            svc_contact svc_contact = ulsdbDataContext.svc_contacts.Single((svc_contact s) => s.contact_id == 1);
            string contact_name = svc_contact.contact_name;
            string contact_number = svc_contact.contact_number;
            string contact_email = svc_contact.contact_email;
            string sMTPServer = Properties.Settings.Default.SMTPServer;
            string sMPTPort = Properties.Settings.Default.SMPTPort;
            string sMTPuid = Properties.Settings.Default.SMTPuid;
            string sMTPpwd = Properties.Settings.Default.SMTPpwd;
            string sMTPfrom = Properties.Settings.Default.SMTPfrom;
            SmtpClient smtpClient = new SmtpClient(sMTPServer, (int)Convert.ToInt16(sMPTPort));
            log.WriteToAppLog("Service: " + list.Count.ToString(), "MSG");

            foreach (svc_appointment current in list)
            {
                string email = current.email;
                string body = string.Concat(new string[]
				{
					"To: ",
					current.first_name,
					" ",
					current.last_name,
					"\r\n\r\nYour PECO Service upgrade is scheduled for tomorrow, ",
					string.Format("{0:dddd}", current.svc_date),
					", ",
					string.Format("{0:MM/dd/yyyy}", current.svc_date),
					". A ULS work crew will be at your home tomorrow morning and will be there most of the day. At some point in the day they will require access to the inside of your home. Please plan on having someone available to allow them access. If you have any questions please call or email ",
					contact_name,
					" (",
					contact_email,
					") at ",
					contact_number,
					".\r\n\r\nThank You,\r\n\r\nUtility Line Services (ULS)"
				});
                string subject = "Your PECO service renewal is scheduled for tommorrow";
                try
                {
                    NetworkCredential credentials = new NetworkCredential(sMTPuid, sMTPpwd);
                    smtpClient.Credentials = credentials;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Send(sMTPfrom, email, subject, body);
                    Program.log.WriteToAppLog("Email reminder successfully sent to " + email, "INFO");
                }
                catch (SmtpException ex)
                {
                    string message = ex.Message;
                    Program.log.WriteToAppLog(message + " For:" + email, "ERROR");
                }
                catch (Exception ex2)
                {
                    string message2 = ex2.Message;
                    log.WriteToAppLog(message2 + " For:" + email, "ERROR");
                }
            }
        }
        private static void QualificationExpireEmailReminders()
        {
            ulsdbDataContextDataContext ulsdbDataContext = new ulsdbDataContextDataContext();
            try
            {
                string sMTPServer = Properties.Settings.Default.SMTPServer;
                string sMPTPort = Properties.Settings.Default.SMPTPort;
                string sMTPuid = Properties.Settings.Default.SMTPuid;
                string sMTPpwd = Properties.Settings.Default.SMTPpwd;
                string sMTPfrom = Properties.Settings.Default.SMTPfrom;
                string subject = "Qualification/Drivers License/Medical Card expiration reminders";
                SmtpClient smtpClient = new SmtpClient(sMTPServer, (int)Convert.ToInt16(sMPTPort));
                List<QualReminderType> joinList = GetJoinList(90);
                List<QualReminderType> joinList2 = GetJoinList(60);
                List<QualReminderType> joinList3 = GetJoinList(30);
                List<QualReminderType> joinList4 = GetExpDLList(30);
                List<QualReminderType> joinList5 = GetExpMedCardList(30);
                List<QualReminderType> list = joinList.Union(joinList2).Union(joinList3).Union(joinList4).Union(joinList5).ToList<QualReminderType>();
                log.WriteToAppLog("Qual: " + list.Count.ToString(), "MSG");
                if (list.Count > 0)
                {
                    List<qual_notification> list2 = (
                        from q in ulsdbDataContext.qual_notifications
                        select q).ToList<qual_notification>();
                    string text = "";
                    foreach (qual_notification current in list2)
                    {
                        if (text == "")
                        {
                            text = current.email;
                        }
                        else
                        {
                            text = text + "," + current.email;
                        }
                    }
                    string[] array = text.Split(new char[]
					{
						','
					});
                    if (text == "")
                    {
                        throw new Exception("No email addresses found.");
                    }
                    string text2 = "";
                    string text3 = "";
                    string text4 = "";
                    string text5 = "";
                    string text6 = "";

                    foreach (QualReminderType current2 in list)
                    {
                        if (current2.numDays == "90")
                        {
                            if (text2.Length == 0)
                            {
                                text2 = string.Concat(new string[]
								{
									"<p>The following qualifications expire in <b>90</b> days: </br><table><tr><td>",
									current2.LName,
									"</td><td>",
									current2.FName,
									"</td><td>",
									current2.qualCompany,
									"</td><td>",
									current2.qualID,
									"</td><td>",
									current2.qualDescr,
									"</td><td>",
									string.Format("{0:MM/dd/yyyy}", current2.ExpireDt),
									"</td></r>"
								});
                            }
                            else
                            {
                                text2 = string.Concat(new string[]
								{
									text2,
									"<tr><td>",
									current2.LName,
									"</td><td>",
									current2.FName,
									"</td><td>",
									current2.qualCompany,
									"</td><td>",
									current2.qualID,
									"</td><td>",
									current2.qualDescr,
									"</td><td>",
									string.Format("{0:MM/dd/yyyy}", current2.ExpireDt),
									"</td></r>"
								});
                            }
                        }
                        else if (current2.numDays == "60")
                        {
                            
                                if (text3.Length == 0)
                                {
                                    text3 = string.Concat(new string[]
									{
										"<p>The following qualifications expire in <b>60</b> days: </br><table><tr><td>",
										current2.LName,
										"</td><td>",
										current2.FName,
										"</td><td>",
										current2.qualCompany,
										"</td><td>",
										current2.qualID,
										"</td><td>",
										current2.qualDescr,
										"</td><td>",
										string.Format("{0:MM/dd/yyyy}", current2.ExpireDt),
										"</td></r>"
									});
                                }
                                else
                                {
                                    text3 = string.Concat(new string[]
									{
										text3,
										"<tr><td>",
										current2.LName,
										"</td><td>",
										current2.FName,
										"</td><td>",
										current2.qualCompany,
										"</td><td>",
										current2.qualID,
										"</td><td>",
										current2.qualDescr,
										"</td><td>",
										string.Format("{0:MM/dd/yyyy}", current2.ExpireDt),
										"</td></r>"
									});
                                }
                            }
                            else if (current2.numDays == "30")
                            {
                                    if (text4.Length == 0)
                                    {
                                        text4 = string.Concat(new string[]
										{
											"<p>The following qualifications expire in <b>30</b> days: </br><table><tr><td>",
											current2.LName,
											"</td><td>",
											current2.FName,
											"</td><td>",
											current2.qualCompany,
											"</td><td>",
											current2.qualID,
											"</td><td>",
											current2.qualDescr,
											"</td><td>",
											string.Format("{0:MM/dd/yyyy}", current2.ExpireDt),
											"</td></r>"
										});
                                    }
                                    else
                                    {
                                        text4 = string.Concat(new string[]
										{
											text4,
											"<tr><td>",
											current2.LName,
											"</td><td>",
											current2.FName,
											"</td><td>",
											current2.qualCompany,
											"</td><td>",
											current2.qualID,
											"</td><td>",
											current2.qualDescr,
											"</td><td>",
											string.Format("{0:MM/dd/yyyy}", current2.ExpireDt),
											"</td></r>"
										});
                                    }
                                }
                            else if (current2.numDays == "31")  // 31 is code for DL expiration
                            {
                                if (text5.Length == 0)
                                {
                                    text5 = string.Concat(new string[]
										{
											"<p>The following Drivers Licences will expire in <b>30</b> days: </br><table><tr><td>",
											current2.LName,
											"</td><td>",
											current2.FName,
											"</td><td>",
											current2.qualCompany,
											"</td><td>",
											current2.qualID,
											"</td><td>",
											current2.qualDescr,
											"</td><td>",
											string.Format("{0:MM/dd/yyyy}", current2.ExpireDt),
											"</td></r>"
										});
                                }
                                else
                                {
                                    text5 = string.Concat(new string[]
										{
											text5,
											"<tr><td>",
											current2.LName,
											"</td><td>",
											current2.FName,
											"</td><td>",
											current2.qualCompany,
											"</td><td>",
											current2.qualID,
											"</td><td>",
											current2.qualDescr,
											"</td><td>",
											string.Format("{0:MM/dd/yyyy}", current2.ExpireDt),
											"</td></r>"
										});
                                }

                            }
                            else if (current2.numDays == "32")  // 32 is code for med card expiration
                            {
                                if (text6.Length == 0)
                                {
                                    text6 = string.Concat(new string[]
										{
											"<p>The following Medical Cards will expire in <b>30</b> days: </br><table><tr><td>",
											current2.LName,
											"</td><td>",
											current2.FName,
											"</td><td>",
											current2.qualCompany,
											"</td><td>",
											current2.qualID,
											"</td><td>",
											current2.qualDescr,
											"</td><td>",
											string.Format("{0:MM/dd/yyyy}", current2.ExpireDt),
											"</td></r>"
										});
                                }
                                else
                                {
                                    text6 = string.Concat(new string[]
										{
											text6,
											"<tr><td>",
											current2.LName,
											"</td><td>",
											current2.FName,
											"</td><td>",
											current2.qualCompany,
											"</td><td>",
											current2.qualID,
											"</td><td>",
											current2.qualDescr,
											"</td><td>",
											string.Format("{0:MM/dd/yyyy}", current2.ExpireDt),
											"</td></r>"
										});
                                }

                            }
                        }

                    if (text2 != "")
                    {
                        text2 += "</table></p><br>";
                    }
                    if (text3 != "")
                    {
                        text3 += "</table></p><br>";
                    }
                    if (text4 != "")
                    {
                        text4 += "</table></p><br>";
                    }
                    if (text5 != "")
                    {
                        text5 += "</table></p><br>";
                    }
                    if (text6 != "")
                    {
                        text6 += "</table></p><br>";
                    }

                    string text7 = text2 + text3 + text4 + text5 + text6;
                    text7 += "This is an automated email. Do not reply.";
                    try
                    {
                        NetworkCredential credentials = new NetworkCredential(sMTPuid, sMTPpwd);
                        smtpClient.Credentials = credentials;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.UseDefaultCredentials = false;
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress(sMTPfrom);
                        mailMessage.IsBodyHtml = true;
                        string[] array2 = array;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            string addresses = array2[i];
                            mailMessage.To.Add(addresses);
                        }
                        mailMessage.Subject = subject;
                        mailMessage.Body = text7;
                        smtpClient.Send(mailMessage);
                        Program.log.WriteToAppLog("Email qualification reminder successfully sent to " + text, "INFO");
                    }
                    catch (SmtpException ex)
                    {
                        string message = ex.Message;
                        Program.log.WriteToAppLog(message + " For:" + text, "ERROR");
                    }
                    catch (Exception ex2)
                    {
                        string message2 = ex2.Message;
                        Program.log.WriteToAppLog(message2 + " For:" + text, "ERROR");
                    }
                }
            }
            catch (Exception ex3)
            {
                string message3 = ex3.Message;
                Program.log.WriteToAppLog(message3, "ERROR");
            }
        }
        public static List<QualReminderType> GetJoinList(int numberOfDays)
        {
            ulsdbDataContextDataContext ulsdbDataContext = new ulsdbDataContextDataContext();
            return (
                from eq in ulsdbDataContext.empQuals
                join q in ulsdbDataContext.qualifications on eq.qualId equals q.qualID
                join e in ulsdbDataContext.employees on eq.employeeId equals e.employeeID
                where eq.qualExpire == (DateTime?)DateTime.Now.Date.AddDays((double)numberOfDays) && (int)e.empStatus == (int)Convert.ToChar("A")
                select new QualReminderType
                {
                    LName = e.lname,
                    FName = e.fname,
                    ExpireDt = eq.qualExpire,
                    qualCompany = q.qualCompany,
                    qualID = q.qualID,
                    qualDescr = q.qualDesc,
                    numDays = Convert.ToString(numberOfDays)
                }).ToList<QualReminderType>();
        }

        public static List<QualReminderType> GetExpDLList(int numberOfDays)
        {
            ulsdbDataContextDataContext ulsdbDataContext = new ulsdbDataContextDataContext();
            return (
                from e in ulsdbDataContext.employees
                where e.DLexpDate == (DateTime?)DateTime.Now.Date.AddDays((double)numberOfDays) && (int)e.empStatus == (int)Convert.ToChar("A")
                select new QualReminderType
                {
                    LName = e.lname,
                    FName = e.fname,
                    ExpireDt = e.DLexpDate,
                    qualCompany = e.org,
                    qualID = "",
                    qualDescr = "",
                    numDays = Convert.ToString(numberOfDays + 1)
                }).ToList<QualReminderType>();
        }

        public static List<QualReminderType> GetExpMedCardList(int numberOfDays)
        {
            ulsdbDataContextDataContext ulsdbDataContext = new ulsdbDataContextDataContext();
            return (
                from e in ulsdbDataContext.employees
                where e.medicalCardExpDt == (DateTime?)DateTime.Now.Date.AddDays((double)numberOfDays) && (int)e.empStatus == (int)Convert.ToChar("A")
                select new QualReminderType
                {
                    LName = e.lname,
                    FName = e.fname,
                    ExpireDt = e.medicalCardExpDt,
                    qualCompany = e.org,
                    qualID = "",
                    qualDescr = "",
                    numDays = Convert.ToString(numberOfDays + 2)
                }).ToList<QualReminderType>();
        }

    }
}
