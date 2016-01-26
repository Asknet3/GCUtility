using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GCU
{
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
}
