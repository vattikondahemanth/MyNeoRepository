using Newtonsoft.Json.Linq;
using SBIMF_ESOA_WebApp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SBIMF_ESOA_WebApp.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedActionParameterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            IEnumerable<KeyValuePair<string, object>> _parametersList = filterContext.ActionParameters.AsEnumerable();

            if (filterContext.HttpContext.Request.IsAjaxRequest() && _parametersList.Count() > 0)
            {

                Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
                var DecryptedRequest = "";
                string key = filterContext.HttpContext.Session.Contents["Pass_Enc_Key"] != null ? filterContext.HttpContext.Session.Contents["Pass_Enc_Key"].ToString() : null;
                Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
                Dictionary<string, object> tempParameterDictionary = new Dictionary<string, object>();
                var _json = new JObject();
                var RequestBody = "";
                try
                {
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Request.InputStream))
                    {
                        reader.DiscardBufferedData();
                        reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                        _json = JObject.Parse(reader.ReadToEnd());
                    }
                    RequestBody = _json["body"].ToString();
                    DecryptedRequest = DecryptStringAES(RequestBody, key);

                    foreach (KeyValuePair<string, object> param in _parametersList)
                    {
                        tempParameterDictionary.Add(param.Key.ToString(), param.Value);
                    }
                    GetRequestParamValues(DecryptedRequest, ref ParamDictionary, tempParameterDictionary.Keys.ToList());
                    foreach (KeyValuePair<string, object> param in tempParameterDictionary)
                    {
                        if (ParamDictionary.ContainsKey(param.Key.ToString()))
                        {
                            filterContext.ActionParameters[param.Key.ToString()] = ParamDictionary[param.Key.ToString()];
                        }
                    }
                }
                catch (Exception e)
                {
                    DecryptedRequest = "Cannot Decrypt the Request";
                }

            }
            base.OnActionExecuting(filterContext);

        }

        private string Decrypt(string encryptedText)
        {

            string key = "jdsg432387#";
            byte[] DecryptKey = { };
            byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
            byte[] inputByte = new byte[encryptedText.Length];
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return "";
        }



        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            AesManaged aes = new AesManaged();
            aes.Padding = PaddingMode.Zeros;
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
            string plaintext = null;
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = key;
                rijAlg.IV = iv;
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
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
            catch (Exception)
            {

            }
            return decriptedFromJavascript;
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
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
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = key;
                rijAlg.IV = iv;
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

        public static string EncryptStringAES(string cipherText, string key)
        {
            var decriptedFromJavascript = "";
            try
            {
                var keybytes = Encoding.UTF8.GetBytes(key);
                var iv = Encoding.UTF8.GetBytes(key);
                decriptedFromJavascript = Convert.ToBase64String(EncryptStringToBytes(cipherText, keybytes, iv));
            }
            catch (Exception)
            {

            }
            return decriptedFromJavascript;
        }

        public static Dictionary<string, string> GetRequestParamValues(string Request, ref Dictionary<string, string> ParamDictionary, List<string> AllParametrs)
        {
            var _json = JObject.Parse(Convert.ToString(Request));
            for (int i = 0; i < AllParametrs.Count; i++)
            {
                string _paramName = Convert.ToString(AllParametrs[i]);
                if (!ParamDictionary.ContainsKey(_paramName))
                    ParamDictionary.Add(_paramName, Convert.ToString(_json[_paramName]));
            }
            return ParamDictionary;
        }
    }
}