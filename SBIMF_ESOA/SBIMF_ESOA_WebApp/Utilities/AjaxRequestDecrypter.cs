using Newtonsoft.Json.Linq;
using SBIMF_ESOA_BAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SBIMF_ESOA_WebApp.Utilities
{
    public static class AjaxRequestUtility
    {
        public static string DecryptRequest(Stream EncodedRequestStream, string key)
        {
            var DecodedRequestStream = "";
            try
            {
                using (StreamReader reader = new StreamReader(EncodedRequestStream))
                {
                    reader.DiscardBufferedData();
                    reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                    var _json = JObject.Parse(reader.ReadToEnd());
                    var RequestBody = _json["body"].ToString();
                    DecodedRequestStream = DecryptStringAES(RequestBody, key);

                }
            }
            catch (Exception e)
            {
                DecodedRequestStream = "Cannot Decrypt the Request";
            }

            return DecodedRequestStream;
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
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
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
                catch (Exception ex)
                {
                    plaintext = "keyError";

                }
            }

            return plaintext;
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
            catch (Exception ex)
            {

            }
            return decriptedFromJavascript;
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
            catch (Exception)
            {
                
            }
            return decriptedFromJavascript;
        }

        

    }
    public class RequestUtility
    {
        public Dictionary<string, string> GetRequestParamValues(string Request, ref Dictionary<string,string> ParamDictionary, params string[] AllParametrs)
        {
            var _json = JObject.Parse(Convert.ToString(Request));
            for (int i = 0; i < AllParametrs.Length; i++)
            {
                string _paramName = Convert.ToString(AllParametrs[i]);
                if(!ParamDictionary.ContainsKey(_paramName))
                    ParamDictionary.Add(_paramName, Convert.ToString(_json[_paramName]));
            }
            return ParamDictionary;
        }
  
    }

    
    
}