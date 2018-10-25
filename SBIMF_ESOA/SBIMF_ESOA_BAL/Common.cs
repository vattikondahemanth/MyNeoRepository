using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.IO;

namespace SBIMF_ESOA_BAL
{
    public static class Common
    {
        static ErrorLogBAL errLogBAL = new ErrorLogBAL();

        public static T JsonToClass<T>(string strJson)
        {
            return JsonConvert.DeserializeObject<T>(strJson);
        }
        public static T GetResultType<T>(DataTable dt)
        {
            var str = ClassToJson(dt);
            var ob = JsonToClass<T>(str);
            return ob;
        }

        public static string ClassToJson(object obj) 
        {
            return JsonConvert.SerializeObject(obj);
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string encodedString)
        {
            byte[] data = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(data);
        }

        public static string AES_Encrypt(string Data)
        {
            try
            {
                string EncryptionKey = "$B|@20!$";
                Data = AES_Encrypt(Data, EncryptionKey);
            }
            catch (Exception)
            {
            }
            return Data;
        }
        public static string AES_Encrypt(string Data, string key)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(Data);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    Data = Convert.ToBase64String(ms.ToArray());
                }
            }
            return Data;
        }

        public static string AES_Decrypt(string Data)
        {
            try
            {
                string DecryptionKey = "$B|@20!$";
                Data = AES_Decrypt(Data, DecryptionKey);
            }
            catch (Exception ex)
            {
            }
            return Data;
        }

        public static string AES_Decrypt(string Data, string key)
        {
            try
            {
                string DecryptionKey = key;
                byte[] cipherBytes = Convert.FromBase64String(Data);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(DecryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        Data = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Data;
        }

        //Encryption & Decryption for password
        public static string GenRandomKey()
        {
            string key = String.Empty;
            Guid g;
            g = Guid.NewGuid();
            key = (g.ToString().Replace("-", "")).Remove(16);
            return key;
        }

        private static Random RNG = new Random();

        public static string Create16DigitString()
        {
            var builder = new StringBuilder();
            while (builder.Length < 16)
            {
                builder.Append(RNG.Next(1000,9999).ToString());
            }
            return builder.ToString();
        }

        public static string DecryptStringAES(string cipherText, string key)
        {
            var decriptedFromJavascript = "";
            try
            {
                var keybytes = Encoding.UTF8.GetBytes(key);
                var iv = Encoding.UTF8.GetBytes(key);

                var encrypted = Convert.FromBase64String(cipherText);
                decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            }
            catch(Exception ex)
            {
                errLogBAL.InsertErroLog("DecryptStringAES", "Login", ex.Message + " Parameters: cipherText= " + cipherText + ", key = " + key,  ex.StackTrace);
            }
            return string.Format(decriptedFromJavascript);
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            AesManaged aes = new AesManaged();
            aes.Padding = PaddingMode.Zeros;

            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings  
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using(var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch(Exception ex)
                {
                    plaintext = "keyError";
                    errLogBAL.InsertErroLog("DecryptStringFromBytes", "Login", ex.Message, ex.StackTrace);
                }
            }

            return plaintext;
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.  
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.  
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.  
            return encrypted;
        }

        public static string EncryptStringAES(string cipherText, string key)
        {
            var decriptedFromJavascript = "";
            try
            {
                var keybytes = Encoding.UTF8.GetBytes(key);
                var iv = Encoding.UTF8.GetBytes(key);

               //var encrypted = Convert.tobas (cipherText);
                decriptedFromJavascript = Convert.ToBase64String(EncryptStringToBytes(cipherText, keybytes, iv));
            }
            catch (Exception ex)
            {
                errLogBAL.InsertErroLog("DecryptStringAES", "Login", ex.Message + " Parameters: cipherText= " + cipherText + ", key = " + key, ex.StackTrace);
            }
            return string.Format(decriptedFromJavascript);
        }

    }
}
