using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Npgsql;
using System.Data;

namespace GRTB01
{
    class Program
    {
        NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=admin;Database=postgres;SearchPath=public;Port=5432");

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No batch option specified.");
                return;
            }
            string batchOption = args[0];
            Program Code = new Program();
            if(batchOption == "BirthdayBatch")
            Code.Run();
            if(batchOption == "WorkAnniversaryBatch")
            Code.Run2();
            if(batchOption == "FarewellBatch")
            Code.Run3();
            if(batchOption == "All"){
                Code.Run();
                Code.Run2();
                Code.Run3();
            }
            else
            {
                Console.WriteLine("Invalid batch option.");
            }
        }

        /// <summary>
        /// Run Method
        /// </summary>
        public void Run()
        {
            //Get birthday dates list
            DataTable dataTable = Get_bday();
            if(dataTable.Rows.Count > 0) 
            {
                //Get birthday dates count
                int dobCnt = Get_day(dataTable);
                if (dobCnt > 0)
                {
                    //Get Employee name and Employee Mail ID
                    DataTable details = Get_EmpDetails();
                    //Send mail
                    email_send(details);
                }
            }
        }
        /// <summary>
        /// Run Method 2
        /// </summary>
        public void Run2()
        {
            //Get Work Anniversary dates list
            DataTable dataTable = Get_WAday();
            if(dataTable.Rows.Count > 0) 
            {
                //Get Work Anniversary dates count
                int dojCnt = Get_day2(dataTable);
                if (dojCnt > 0)
                {
                    //Get Employee name, Employee Mail ID, Date of join
                    DataTable details = Get_WAEmpDetails();
                    //Send mail
                    email_send2(details);
                }
            }
        }
        /// <summary>
        /// Run Method 2
        /// </summary>
        public void Run3()
        {
            //Get Farewell dates list
            DataTable dataTable = Get_Fday();
            if(dataTable.Rows.Count > 0) 
            {
                //Get Farewell dates count
                int dolCnt = Get_day3(dataTable);
                if (dolCnt > 0)
                {
                    //Get Employee name, Employee Mail ID
                    DataTable details = Get_FEmpDetails();
                    //Send mail
                    email_send3(details);
                }
            }
        }

        /// <summary>
        /// Get birthday dates list method
        /// </summary>
        /// <returns></returns>
        public DataTable Get_bday()
        {
            connection.Open();
            var query = "select "
                           + "date_of_birth"
                       + " from"
                           + " m_employee_info";
            var cmd = new NpgsqlCommand(query, connection);
            DataTable dataTable = new DataTable();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
            adapter.Fill(dataTable);
            connection.Close();
            return dataTable;
        }
        
        /// <summary>
        /// Get Work Anniversary dates list method
        /// </summary>
        /// <returns></returns>
        public DataTable Get_WAday()
        {
            connection.Open();
            var query = "select "
                           + "date_of_join"
                       + " from"
                           + " m_employee_info";
            var cmd = new NpgsqlCommand(query, connection);
            DataTable dataTable = new DataTable();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
            adapter.Fill(dataTable);
            connection.Close();
            return dataTable;
        }
        
        /// <summary>
        /// Get Work Anniversary dates list method
        /// </summary>
        /// <returns></returns>
        public DataTable Get_Fday()
        {
            connection.Open();
            var query = "select "
                           + "date_of_leaving"
                       + " from"
                           + " m_employee_info"
                       + " Where "
                       + " date_of_leaving IS NOT NULL";
            var cmd = new NpgsqlCommand(query, connection);
            DataTable dataTable = new DataTable();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
            adapter.Fill(dataTable);
            connection.Close();
            return dataTable;
        }

        /// <summary>
        /// Get birthday date method
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public int Get_day(DataTable dataTable)
        {
            int date = 0;
            //DateTime dt = DateTime.Now;
            DateTime today = DateTime.Today;
            foreach (DataRow row in dataTable.Rows)
            {
                //String day = row[0].ToString();
                DateTime birthday = DateTime.Parse(row[0].ToString());
                //if (day.Contains($"{dt.Month.ToString()}/{dt.Day.ToString()}"))
                if (birthday.Month == today.Month && birthday.Day == today.Day)
                {
                    date++;
                }
            }
            return date;
        }
        
        /// <summary>
        /// Get Work Anniversary date method
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public int Get_day2(DataTable dataTable)
        {
            int date = 0;
            //DateTime dt = DateTime.Now;
            DateTime today = DateTime.Today;
            foreach (DataRow row in dataTable.Rows)
            {
                //String day = row[0].ToString();
                DateTime WorkAnniversaryDay = DateTime.Parse(row[0].ToString());
                //if (day.Contains($"{dt.Month.ToString()}/{dt.Day.ToString()}"))
                if (WorkAnniversaryDay.Month == today.Month && WorkAnniversaryDay.Day == today.Day && WorkAnniversaryDay.Year != today.Year)
                {
                    date++;
                }
            }
            return date;
        }
        
        /// <summary>
        /// Get Farewell date method
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public int Get_day3(DataTable dataTable)
        {
            int date = 0;
            //DateTime dt = DateTime.Now;
            DateTime today = DateTime.Today;
            foreach (DataRow row in dataTable.Rows)
            {
                //String day = row[0].ToString();
                DateTime FarewellDay = DateTime.Parse(row[0].ToString());
                //if (day.Contains($"{dt.Month.ToString()}/{dt.Day.ToString()}"))
                if (FarewellDay.Year == today.Year && FarewellDay.Month == today.Month && FarewellDay.Day == today.Day)
                {
                    date++;
                }
            }
            return date;
        }

        /// <summary>
        /// Get Employee name and Employee Mail ID method
        /// </summary>
        /// <param name="dob"></param>
        /// <returns></returns>
        public DataTable Get_EmpDetails()
        {
            DateTime dt = DateTime.Now;
            connection.Open();
            var query = "select"
                           + " employee_name, "
                           + " email_id, "
                           + " filename "
                       + " from"
                           + " m_employee_info "
                       + " Where "
                           + " EXTRACT(DAY FROM date_of_birth) "
                           + " = @day "
                           + " AND "
                           + " EXTRACT(MONTH FROM date_of_birth) "
                           + " = @month "
                           + " AND "
                           + " DISCONTINUED = 'False'";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@day", dt.Day);
            cmd.Parameters.AddWithValue("@month", dt.Month);
            DataTable dataTable = new DataTable();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
            adapter.Fill(dataTable);
            connection.Close();
            return dataTable;
        }
        
        /// <summary>
        /// Get Employee name, Employee Mail ID, Date of join method
        /// </summary>
        /// <param name="doj"></param>
        /// <returns></returns>
        public DataTable Get_WAEmpDetails()
        {
            DateTime dt = DateTime.Now;
            connection.Open();
            var query = "select"
                           + " employee_name, "
                           + " email_id, "
                           + " date_of_join "
                       + " from"
                           + " m_employee_info "
                       + " Where "
                           + " EXTRACT(DAY FROM date_of_join) "
                           + " = @day "
                           + " AND "
                           + " EXTRACT(MONTH FROM date_of_join) "
                           + " = @month "
                           + " AND "
                           + " DISCONTINUED = 'False'";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@day", dt.Day);
            cmd.Parameters.AddWithValue("@month", dt.Month);
            DataTable dataTable = new DataTable();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
            adapter.Fill(dataTable);
            connection.Close();
            return dataTable;
        }
        
        /// <summary>
        /// Get Employee name, Employee Mail ID method
        /// </summary>
        /// <param name="dol"></param>
        /// <returns></returns>
        public DataTable Get_FEmpDetails()
        {
            DateTime dt = DateTime.Now;
            connection.Open();
            var query = "select"
                           + " employee_name, "
                           + " email_id "
                       + " from"
                           + " m_employee_info "
                       + " Where "
                           + " EXTRACT(DAY FROM date_of_leaving) "
                           + " = @day "
                           + " AND "
                           + " EXTRACT(MONTH FROM date_of_leaving) "
                           + " = @month "
                           + " AND "
                           + " EXTRACT(YEAR FROM date_of_leaving) "
                           + " = @year "
                           + " AND "
                           + " DISCONTINUED = 'True'";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@day", dt.Day);
            cmd.Parameters.AddWithValue("@month", dt.Month);
            cmd.Parameters.AddWithValue("@year", dt.Year);
            DataTable dataTable = new DataTable();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
            adapter.Fill(dataTable);
            connection.Close();
            return dataTable;
        }

        /// <summary>
        /// Send mail method
        /// </summary>
        /// <param name="details"></param>
        public static void email_send(DataTable details)
        {
            try
            {
                foreach (DataRow row in details.Rows)
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("119.18.54.136");
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("sagar.473@scii.in", "yjpg8426");
                    SmtpServer.EnableSsl = true;
                    mail.IsBodyHtml = true;
                    mail.From = new MailAddress("sagar.473@scii.in");
                    mail.To.Add(row[1].ToString());
                    mail.Subject = "Happy Birthday !!";
                    mail.Body = $"<i>Dear {row[0].ToString()},<br/></i>" +
                    "<i>Greetings of the day !!</i><br/><br/>" +
                    "<i>Today is a great day to get started on another 365-day journey.</i><br/>" +

                    "<i>It’s a fresh start to new beginnings, new hopes, and great endeavors.</i><br/>" +

                    "<i>May your day be filled with happy moments, be sure you enjoy your day to the fullest.  </i></B><span class=moz-smiley-s11><span>8-)</span></span><br/><br/>" +
                    "<i><b>HRD Department wishes you a very Happy Birthday !!</b></i><span class=moz-smiley-s1></span><br/><br/>" +
                    "-- <br/>" +
                    "Best Wishes,<br/>" +
                    "HRD Dept<br/>" +
                    "System Consultant Information India (P) Ltd.<br/>";
                    string path = @"C:\GreetingPortal\wwwroot\";
                    Console.WriteLine(path);
                    string imagePath = Path.Combine(path, "Images", row[2].ToString());
                    mail.Attachments.Add(new Attachment(imagePath));

                    ServicePointManager.ServerCertificateValidationCallback =
                    delegate (
                        object s,
                        X509Certificate certificate,
                        X509Chain chain,
                        SslPolicyErrors sslPolicyErrors
                    )
                    {
                        return true;
                    };


                    SmtpServer.Send(mail);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }

        }

        /// <summary>
        /// Send mail method
        /// </summary>
        /// <param name="details"></param>
        public static void email_send2(DataTable details)
        {
            DateTime currentDate = DateTime.Now;
            DateTime date;
            try
            {
                foreach (DataRow row in details.Rows)
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("119.18.54.136");
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("sagar.473@scii.in", "yjpg8426");
                    SmtpServer.EnableSsl = true;
                    mail.IsBodyHtml = true;
                    mail.From = new MailAddress("sagar.473@scii.in");
                    mail.To.Add(row[1].ToString());
                    mail.Subject = "Happy Work Anniversary !!";
                    mail.Body = $"<i>Dear {row[0].ToString()},<br/></i>" +
                    "<i>Greetings for the day !!</i><br/><br/>" +
                    $"<i>Time flies!! It's been {currentDate.Year - DateTime.Parse(row[2].ToString()).Year} years already !! </i><br/><br/>" +
                    "<i>Congratulations on another successful year of service. </i><br/>" +
                    "<i>You bring professionalism, dedication, and passion to your department. </i><br/>" +
                    "<i>We appreciate your contributions and look forward to keep working together.</i></b><br/><br/>" +
                    "<i><b>HRD Department sends you warm wishes on your Work Anniversary !!</b></i><span class=moz-smiley-s1></span><br/><br/>" +
                    "-- <br/>" +
                    "Best Wishes,<br/>" +
                    "HRD Dept<br/>" +
                    "System Consultant Information India (P) Ltd.<br/>";
                    string path = @"C:\GreetingPortal\wwwroot\";
                    Console.WriteLine(path);
                    string imagePath = Path.Combine(path, "Images", "Anniversary.png");
                    mail.Attachments.Add(new Attachment(imagePath));

                    ServicePointManager.ServerCertificateValidationCallback =
                    delegate (
                        object s,
                        X509Certificate certificate,
                        X509Chain chain,
                        SslPolicyErrors sslPolicyErrors
                    )
                    {
                        return true;
                    };


                    SmtpServer.Send(mail);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }

        }
        /// <summary>
        /// Send mail method
        /// </summary>
        /// <param name="details"></param>
        public static void email_send3(DataTable details)
        {
            try
            {
                foreach (DataRow row in details.Rows)
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("119.18.54.136");
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("sagar.473@scii.in", "yjpg8426");
                    SmtpServer.EnableSsl = true;
                    mail.IsBodyHtml = true;
                    mail.From = new MailAddress("sagar.473@scii.in");
                    mail.To.Add(row[1].ToString());
                    mail.Subject = "Farewell Wishes !!";
                    mail.Body = $"<i>Dear {row[0].ToString()},<br/></i>" +
                    "<i>Greetings for the day !! </i><br/><br/>" +
                    "<i>We would like to thank you and extend our appreciation wishes as you bid farewell. </i><br/>" +
                    "<i>While we will miss you and have fond memories of working with you, </i><br/>" +
                    "<i>we wish you well and hope you attain all the success you deserve. </i><br/>" +
                    "<i>Your hard work and dedication were an important part of our team...!</i></b><br/><br/>" +
                    "<i><b>HRD Department join together in wishing you every success in all your future endeavors….!!</b></i><span class=moz-smiley-s1></span><br/><br/>" +
                    "-- <br/>" +
                    "Best Wishes,<br/>" +
                    "HRD Dept<br/>" +
                    "System Consultant Information India (P) Ltd.<br/>";
                    string path = @"C:\GreetingPortal\wwwroot\";
                    Console.WriteLine(path);
                    string imagePath = Path.Combine(path, "Images", "Farewell.jpg");
                    mail.Attachments.Add(new Attachment(imagePath));

                    ServicePointManager.ServerCertificateValidationCallback =
                    delegate (
                        object s,
                        X509Certificate certificate,
                        X509Chain chain,
                        SslPolicyErrors sslPolicyErrors
                    )
                    {
                        return true;
                    };


                    SmtpServer.Send(mail);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }

        }

    }
}

