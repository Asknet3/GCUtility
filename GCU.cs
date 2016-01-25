using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GCUtility
{
    /// <summary>
    /// Utility create da GC
    /// </summary>
    public class GCU
    {
        // ********** START METODI PER DATABASE SQL SERVER **********************************************************************
        /// <summary>
        /// Classe per gestione funzioni su DB
        /// </summary>
        public class Db
        {
            #region GET CONNECTION
            /// <summary>
            /// Restituisce una SqlConnection costruendo automaticamente una stringConnection dai parametri passati.
            /// </summary>
            /// <param name="dataSource"> è il server name di SQLServer. <para/>Es. nome_macchia\istanza (es. vespa\ieo).</param>
            /// <param name="catalog">E' il nome del database</param>
            /// <param name="integratedSecurity">True o False. Se non settato sarà False di default.</param>
            /// <param name="userId">username di SQL Server.</param>
            /// <param name="password">Password di SQL Server.</param>
            /// <param name="multipleActiveResultSets">True o False. Se non settato sarà True di default.</param>
            /// <returns>Restituisce una SqlConnection costruendo automaticamente una stringConnection dai parametri passati.</returns>
            public static SqlConnection GetConnection(String dataSource, String catalog, String userId, String password, String integratedSecurity = "False", String multipleActiveResultSets = "True")
            {
                string connectionString = "Data Source=" + dataSource + ";Initial Catalog=" + catalog + ";Integrated Security=" + integratedSecurity + ";User ID=" + userId + ";Password=" + password + ";MultipleActiveResultSets=" + multipleActiveResultSets;
                return new SqlConnection(connectionString);
            }

            /// <summary>
            /// Restituisce una SqlConnection partendo da una stringConnection come parametro passato.
            /// </summary>
            /// <param name="connectionString">Stringa di connessione del tipo: <para/> "Data Source=xxxx;Initial Catalog=yyyy;Integrated Security=False;User ID=uuuuu;Password=ppppp;MultipleActiveResultSets=True;</param>
            /// <returns></returns>
            public static SqlConnection GetConnection(String connectionString)
            {
                return new SqlConnection(connectionString);
            }
            #endregion

            #region SELECT
            /// <summary>
            /// Effettua una SELECT sul DB.
            /// </summary>
            /// <param name="conn"> Connessione </param>
            /// <param name="dataToExtract"></param>
            /// <param name="tableName">Nome della tabella da cui estrarre i dati</param>
            /// <param name="where">Condizione.<para/>Lasciare vuoto nel caso non servano condizioni</param>
            /// <returns>Restituisce i sisultati della query all'interno di un DataTable </returns>
            public static DataTable getDataTableFrom_Select(SqlConnection conn, String dataToExtract, String tableName, String where = "1=1")
            {
                SqlConnection connection = conn;
                SqlCommand command;

                string sqlQuery = String.Format("SELECT {0} FROM {1} WHERE {2}", dataToExtract, tableName, where);

                try
                {
                    command = new SqlCommand(sqlQuery, connection);
                    connection.Open();
                    SqlDataReader myreader = command.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(myreader);

                    return dt;
                }
                finally
                { connection.Close(); }
            }


            /// <summary>
            /// Effettua una SELECT sul DB.
            /// </summary>
            /// <param name="conn"> Connessione </param>
            /// <param name="dataToExtract"></param>
            /// <param name="tableName">Nome della tabella da cui estrarre i dati</param>
            /// <param name="where">Condizione.<para/>Lasciare vuoto nel caso non servano condizioni</param>
            /// <returns>Restituisce un SqlDataReader </returns>
            public static SqlDataReader getSqlDataReaderFrom_Select(SqlConnection conn, String dataToExtract, String tableName, String where = "1=1")
            {
                SqlConnection connection = conn;
                SqlCommand command;

                string sqlQuery = String.Format("SELECT {0} FROM {1} WHERE {2}", dataToExtract, tableName, where);

                try
                {
                    command = new SqlCommand(sqlQuery, connection);
                    connection.Open();
                    SqlDataReader myreader = command.ExecuteReader();

                    return myreader;
                }
                finally
                { connection.Close(); }
            }

            #endregion

            #region INSERT
            /// <summary>Effettua una Insert sul DB.</summary>
            /// <param name="conn"> Connessione </param>
            /// <param name="tableName">Nome della tabella da cui estrarre i dati</param>
            /// <param name="where">Condizione.<para/>Lasciare vuoto nel caso non servano condizioni</param>
            /// <returns>Restituisce il numero di righe che sono state inserite</returns>
            public static Int32 Insert(SqlConnection conn, String tableName, String where = "1=1")
            {
                SqlConnection connection = conn;
                SqlCommand command;

                string sqlQuery = String.Format("INSERT INTO {0} WHERE {1}", tableName, where);

                try
                {
                    command = new SqlCommand(sqlQuery, connection);
                    connection.Open();
                    int numberOfRows = command.ExecuteNonQuery();

                    return numberOfRows;
                }
                finally
                { connection.Close(); }
            }
            #endregion

            #region UPDATE
            /// <summary>
            /// Effettua un Update sul DB restituendo il numero di righe modificate
            /// </summary>
            /// <param name="conn"> Connessione </param>
            /// <param name="tableName">Nome della tabella da cui estrarre i dati</param>
            /// <param name="data">Campi da settare</param>
            /// <param name="where">Condizione.<para/>Lasciare vuoto nel caso non servano condizioni</param>
            /// <returns>Restituisce il numero di righe che sono state inserite</returns>
            public static Int32 Update(SqlConnection conn, String tableName, String data, String where = "1=1")
            {
                SqlConnection connection = conn;
                SqlCommand command;

                string sqlQuery = String.Format("UPDATE {0} SET {1} WHERE {2}", tableName, data, where);

                try
                {
                    command = new SqlCommand(sqlQuery, connection);
                    connection.Open();
                    int numberOfRows = command.ExecuteNonQuery();

                    return numberOfRows;
                }
                finally
                { connection.Close(); }
            }
            #endregion
        }
        // ********** END METODI PER DATABASE SQL SERVER **********





        // ********** START METODI PER GESTIONE EXPORT *************************************************************************
        /// <summary>
        /// Classe per gestione Export
        /// </summary>
        public class Export
        {
            #region EXPORT TO EXCEL
            /// <summary>
            /// Crea un file excel (preferibilmente .xlsx) partendo da un DataTable
            /// </summary>
            /// <param name="dt">Tabella di input da convertire in Excel</param>
            /// <param name="path">Percorso + nome del file + estensione dove salvare il file.<para/>Consigliato salvare il file in .xlsx per evitare problemi di compatibilità. </param>
            /// <param name="worksheetName">Nome del Foglio di Lavoro</param>
            /// <param name="title">Titolo del documento</param>
            /// <param name="author">Autore del documento</param>
            /// <param name="company">Company di appartenenza dell'autore</param>
            /// <returns>Restituisce True se il file viene esportato correttamente. Altrimenti restituisce False.</returns>
            public static bool ExportToExcel(DataTable dt, String path, String worksheetName, String title = "", String author = "", String company = "")
            {
                FileInfo file = new FileInfo(path);
                var package = new ExcelPackage(file);
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);

                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 1, j + 1].Value = dt.Rows[i][j];
                        }
                    }

                    // Set some document properties
                    package.Workbook.Properties.Title = title;
                    package.Workbook.Properties.Author = author;
                    package.Workbook.Properties.Company = company;

                    worksheet.Column(1).AutoFit();

                    // save our new workbook and we are done!
                    package.Save();
                    return true;
                }
                catch (Exception ex)
                { 
                    var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                    log.Error(ex.Message, ex);
                    return false;
                    throw ex;
                }
            }
            #endregion


            #region EXPORT TO CSV
            /// <summary>
            /// Crea un file excel (preferibilmente .xlsx) partendo da un DataTable
            /// </summary>
            /// <param name="dt">DataTable di input da cui generare il CSV</param>
            /// <param name="path">Percorso + nome del file + estensione dove salvare il file.</param>
            /// <param name="encoding"> Encoding utilizzato per salvare il file CSV. <para/>Consigliato salvare in Encoding.UTF8</param>
            /// <returns></returns>
            public static bool ExportToCSV(DataTable dt, string path, Encoding encoding)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(path, false, encoding);

                    int columnCount = dt.Columns.Count;

                    for (int i = 0; i < columnCount; i++)
                    {
                        sw.Write(dt.Columns[i]);

                        if (i < columnCount - 1)
                        {
                            sw.Write(",");
                        }
                    }

                    sw.Write(sw.NewLine);

                    foreach (DataRow dr in dt.Rows)
                    {
                        for (int i = 0; i < columnCount; i++)
                        {
                            if (!Convert.IsDBNull(dr[i]))
                            {
                                sw.Write(dr[i].ToString());
                            }

                            if (i < columnCount - 1)
                            {
                                sw.Write(",");
                            }
                        }
                        sw.Write(sw.NewLine);
                    }

                    sw.Close();

                    return true;
                }
                catch (Exception ex)
                {

                    return false;
                    throw ex;
                }
            }
            #endregion


            #region HEXA BYTE[] TO STRING
            /// <summary>
            /// Converte un array di byte in stringa di formato HEXA
            /// </summary>
            /// <param name="ba">Array di Byte </param>
            /// <returns>Restituisce una stringa in formato Hexa</returns>
            public static string ByteArrayToHexString(byte[] ba)
            {
                string hex = BitConverter.ToString(ba);
                return hex.Replace("-", "");
            }
            #endregion
        }
        // ********** END ESTRAZIONE DATATABLE SU EXCEL **********






        // ********** START SICUREZZA ********************************************************************************************
        /// <summary>
        /// Classe per gestione metodi su Sicurezza
        /// </summary>
        public class Security
        {
            #region CALCOLA MD5 HASH DA UNA STRINGA
            /// <summary>
            /// Restituisce un MD5 hash come stringa
            /// </summary>
            /// <param name="TextToHash">String to be hashed.</param>
            /// <returns>Restituisce un hash come stringa.</returns>
            public static String GetMD5Hash(String TextToHash)
            {
                //verifica che la stringa contenga qualcosa
                if ((TextToHash == null) || (TextToHash.Length == 0))
                {
                    return String.Empty;
                }

                //Calcola l'MD5 hash. Questo necessita che la stringa venga splittata in un byte[].
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] textToHash = Encoding.Default.GetBytes(TextToHash);
                byte[] result = md5.ComputeHash(textToHash);

                // Converte il risultato nuovamente in stringa string.
                return System.BitConverter.ToString(result);
            }
            #endregion

            #region GENERATE TOKEN
            /// <summary>
            /// Restituisce un Token avente lunghezza specificata come parametro
            /// </summary>
            /// <param name="length">Lunghezza che dovrà avere il token restituito.</param>
            /// <returns>Restituisce un Token avente lunghezza specificata come parametro</returns>
            public static String GenerateToken(int length)
            {
                char[] AvailableCharacters = {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };

                char[] identifier = new char[length];
                byte[] randomData = new byte[length];

                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(randomData);
                }

                for (int idx = 0; idx < identifier.Length; idx++)
                {
                    int pos = randomData[idx] % AvailableCharacters.Length;
                    identifier[idx] = AvailableCharacters[pos];
                }

                return new string(identifier);
            }
            #endregion
        }
        // ********** END SICUREZZA **********





        // ********** START UTILITY WEB *****************************************************************************************
        /// <summary>
        /// Classe per la gestione di metodi da usare nel Web
        /// </summary>
        public class UtilityWeb
        {
            #region INVIA EMAIL
            /// <summary>
            /// Permette l'invio Email
            /// </summary>
            /// <param name="from">Mittente</param>
            /// <param name="toEmail">Destinatario</param>
            /// <param name="subject">Oggetto</param>
            /// <param name="msg">Corpo del messaggio</param>
            /// <param name="username">Username con cui accedere al server SMTP</param>
            /// <param name="password">Password con cui accedere al server SMTP</param>
            /// <param name="useDefaultcredential">Default: False</param>
            /// <param name="port">Porta SMTP. Settare a 0 per usare quella di default.</param>
            /// <param name="enableSsl">Default: True</param>
            /// <param name="timeout">Default: 1000</param>
            /// <returns></returns>
            public static bool SendMail(MailAddress from, MailAddress toEmail, String subject, String msg, String username, String password, Boolean useDefaultcredential = false, int port=0, Boolean enableSsl=true, int timeout=1000)
            {
                using (MailMessage message = new MailMessage())
                {
                    message.From = from;
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = msg;
                    message.IsBodyHtml = true;
                    NetworkCredential credential = new NetworkCredential(username, password);
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        try
                        {
                            smtp.UseDefaultCredentials = useDefaultcredential;
                            if(port != 0)  smtp.Port = port;
                            smtp.EnableSsl = enableSsl;
                            smtp.Timeout = timeout;
                            smtp.Credentials = credential;

                            smtp.Send(message);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                            log.Error(ex.Message, ex);
                            return false;
                        }
                    }
                }
            }
            #endregion
            // ********** END UTILITY WEB **********

        }
    }
}

