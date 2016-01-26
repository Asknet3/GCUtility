using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCU
{
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
}
