using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCU
{
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
}
