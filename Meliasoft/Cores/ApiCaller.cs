using Newtonsoft.Json;
using System;

using System.Diagnostics;

using System.Threading.Tasks;
using System.Configuration;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Meliasoft.Cores
{
    public static class ApiCaller
    {
        //#region ---Define---
        //private static readonly ILog log4NetClient = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly ILog log4NetServer = LogManager.GetLogger(AppInfo.LogServer);
        //#endregion

        public static string HOST_URL = ConfigurationManager.AppSettings["HOST_API_URL"];
        public static string API_NAME = "api/WebService/DataTransfer";

        public static string TOKEN_KEY_SCHEMA = "Meliasoft.WebAPI.TokenKey";
        public static string TOKEN_KEY_VALUE = ConfigurationManager.AppSettings["API_ADMIN_TOKEN_KEY"]; //"Ce1dzyf4Vj/wr3jfhNTVAD8L3JinzVLrf4hy852mHzM="; //tmp key

        public static bool BlnCancelRequest = false;
        public static int RequestTimeOut = Convert.ToInt16(ConfigurationManager.AppSettings["RequestTimeOut"]);

        #region " HTTP Web API "

        #region " HTTP Get Web API "

        public static async Task<T> GetAPI<T>(string url)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string fullURL = url;
            if (!fullURL.Contains("http://") && !fullURL.Contains("https://"))
                fullURL = HOST_URL + url;

            int length = url.IndexOf("?") == -1 ? url.Length : url.IndexOf("?");
            string logRequest = "request: " + url.Substring(0, length);
            string logStatus = "status: ";
            string logResponse = "response: ";
            // bool blnException = false;
            try
            {
                try
                {
                    ApiCaller.BlnCancelRequest = false;
                    using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = TimeSpan.FromSeconds(ApiCaller.RequestTimeOut);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiCaller.TOKEN_KEY_SCHEMA, ApiCaller.TOKEN_KEY_VALUE); //AppInfo.staffInfor.TOKEN_KEY
                        //client.DefaultRequestHeaders.Add("PreLog", string.Format("CLOUD_CODE: {0}; STAFF_CODE: {1}; USER_PATIENT_CODE: {2}; SCREEN_ID: {3}; ", AppInfo.staffInfor.CLOUD_CODE, AppInfo.staffInfor.STAFF_CODE, AppInfo.ClientUserPatientCode, AppInfo.ClientScreenID));
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        Console.WriteLine("Before call API");
                        // Call API
                        HttpResponseMessage res = await client.GetAsync(fullURL);
                        Console.WriteLine("After call API");
                        logStatus += (int)res.StatusCode;
                        if (res.IsSuccessStatusCode)
                        {
                            logResponse += res.Content.ReadAsStringAsync().Result;
                            return res.Content.ReadAsAsync<T>().Result;
                        }
                    }
                }
                catch (HttpRequestException httpEx)
                {

                    //blnException = true;
                    throw httpEx;
                }
                return default(T);
            }
            catch (Exception ex)
            {
                logResponse += ex;
                //blnException = true;
                throw new ArgumentException(ex.Message);
            }
            finally
            {
                stopWatch.Stop();
                //TimeSpan ts = stopWatch.Elapsed;
                //double countMB = ConvertBytesToMegabytes(System.Text.Encoding.Default.GetByteCount(logResponse));
                //logStatus += " - Size object (" + countMB + " MB) - Run time (" + ts.TotalSeconds + " s)";
                //if (blnException)
                //{
                //    log4NetClient.Error(logRequest);
                //    log4NetClient.Error(logStatus);
                //    log4NetClient.Error(logResponse);
                //}
                //else
                //{
                //    log4NetClient.Info(logRequest);
                //    log4NetClient.Info(logStatus);
                //    //log4NetClient.Info(logResponse);
                //}
            }
        }

        #endregion

        #region " HTTP Post Web API "

        public static async Task<T> PostObject<T>(string url, object objPost = null)
        {
            string fullURL = url;
            if (!fullURL.Contains("http://") && !fullURL.Contains("https://"))
                fullURL = HOST_URL + url;

            int length = url.IndexOf("?") == -1 ? url.Length : url.IndexOf("?");
            string logRequest = "request: " + url.Substring(0, length);
            string logStatus = "status: ";
            string logResponse = "response: ";

            try
            {
                try
                {
                    ApiCaller.BlnCancelRequest = false;
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = TimeSpan.FromSeconds(ApiCaller.RequestTimeOut);
                        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiCaller.TOKEN_KEY_SCHEMA, ApiCaller.TOKEN_KEY_VALUE);
                        //client.DefaultRequestHeaders.Add("PreLog", string.Format("CLOUD_CODE: {0}; STAFF_CODE: {1}; USER_PATIENT_CODE: {2}; SCREEN_ID: {3}; ", AppInfo.staffInfor.CLOUD_CODE, AppInfo.staffInfor.STAFF_CODE, AppInfo.ClientUserPatientCode, AppInfo.ClientScreenID));
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Call API
                        HttpResponseMessage res = await client.PostAsJsonAsync(fullURL, objPost);
                        logStatus += (int)res.StatusCode;
                        if (res.IsSuccessStatusCode)
                        {
                            logResponse += res.Content.ReadAsStringAsync().Result;
                            return res.Content.ReadAsAsync<T>().Result;
                        }
                    }
                }
                catch (HttpRequestException httpEx)
                {

                    throw httpEx;
                }
                return default(T);
            }
            catch (Exception ex)
            {
                logResponse += ex;
                throw new ArgumentException(ex.Message);
            }
            finally
            {
                //if (blnException)
                //{
                //    log4NetClient.Error(logRequest);
                //    log4NetClient.Error(logStatus);
                //    log4NetClient.Error(logResponse);
                //}
                //else
                //{
                //    log4NetClient.Info(logRequest);
                //    log4NetClient.Info(logStatus);
                //    //log4NetClient.Info(logResponse);
                //}
            }
        }

        public static async Task<T> PostJsonString<T>(string url, string jsonData = "{\"value\":{}}")
        {
            string fullURL = url;
            if (!fullURL.Contains("http://") && !fullURL.Contains("https://"))
                fullURL = HOST_URL + url;

            int length = url.IndexOf("?") == -1 ? url.Length : url.IndexOf("?");
            string logRequest = "request: " + url.Substring(0, length);
            string logStatus = "status: ";
            string logResponse = "response: ";

            try
            {
                try
                {
                    ApiCaller.BlnCancelRequest = false;
                    using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = TimeSpan.FromSeconds(ApiCaller.RequestTimeOut);
                        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiCaller.TOKEN_KEY_SCHEMA, ApiCaller.TOKEN_KEY_VALUE);
                        //client.DefaultRequestHeaders.Add("PreLog", string.Format("CLOUD_CODE: {0}; STAFF_CODE: {1}; USER_PATIENT_CODE: {2}; SCREEN_ID: {3}; ", AppInfo.staffInfor.CLOUD_CODE, AppInfo.staffInfor.STAFF_CODE, AppInfo.ClientUserPatientCode, AppInfo.ClientScreenID));
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        StringContent jsonStringData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                        // Call API
                        //jsonResult = await client.PostAsync(url, new StringContent(myJson, Encoding.UTF8, "application/json"));
                        //HttpResponseMessage res = await client.PostAsJsonAsync(fullURL, jsonStringData);
                        object jsonObject = JsonConvert.DeserializeObject<object>(jsonData);
                        HttpResponseMessage res = await client.PostAsJsonAsync(fullURL, jsonObject).ConfigureAwait(false);
                        logStatus += (int)res.StatusCode;
                        if (res.IsSuccessStatusCode)
                        {
                            logResponse += res.Content.ReadAsStringAsync().Result;
                            return res.Content.ReadAsAsync<T>().Result;
                        }
                    }
                }
                catch (HttpRequestException httpEx)
                {

                    throw httpEx;
                }
                return default(T);
            }
            catch (Exception ex)
            {
                logResponse += ex;
                throw new ArgumentException(ex.Message);
            }
            finally
            {
                //if (blnException)
                //{
                //    log4NetClient.Error(logRequest);
                //    log4NetClient.Error(logStatus);
                //    log4NetClient.Error(logResponse);
                //}
                //else
                //{
                //    log4NetClient.Info(logRequest);
                //    log4NetClient.Info(logStatus);
                //    //log4NetClient.Info(logResponse);
                //}
            }
        }

        #endregion

        #region " HTTP Put Web API "

        public static async Task<T> PutAPI<T>(string url, object objPut)
        {
            try
            {
                try
                {
                    string fullURL = url;
                    if (!fullURL.Contains("http://") && !fullURL.Contains("https://"))
                        fullURL = HOST_URL + url;

                    ApiCaller.BlnCancelRequest = false;
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = TimeSpan.FromSeconds(ApiCaller.RequestTimeOut);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiCaller.TOKEN_KEY_SCHEMA, ApiCaller.TOKEN_KEY_VALUE);
                        //client.DefaultRequestHeaders.Add("PreLog", string.Format("CLOUD_CODE: {0}; STAFF_CODE: {1}; USER_PATIENT_CODE: {2}; SCREEN_ID: {3}; ", AppInfo.staffInfor.CLOUD_CODE, AppInfo.staffInfor.STAFF_CODE, AppInfo.ClientUserPatientCode, AppInfo.ClientScreenID));
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Call API
                        HttpResponseMessage res = await client.PutAsJsonAsync(fullURL, objPut);
                        if (res.IsSuccessStatusCode)
                        {
                            return res.Content.ReadAsAsync<T>().Result;
                        }
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    //CloseWaitingForm();
                    ApiCaller.BlnCancelRequest = true;
                    //TuanNA: fix bug: http://redmine.ominext.co/issues/8755
                    //MessageBox.Show(LanguagesMessage.GetLanguagesMessage("MSG_CONNECTSERVER_FAIL"));
                    throw httpEx;
                }
                return default(T);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
                // throw;
            }
        }

        #endregion

        #region " HTTP Delete Web API "

        public static async Task<T> DelAPI<T>(string url)
        {
            try
            {
                try
                {
                    string fullURL = url;
                    if (!fullURL.Contains("http://") && !fullURL.Contains("https://"))
                        fullURL = HOST_URL + url;

                    ApiCaller.BlnCancelRequest = false;
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = TimeSpan.FromSeconds(ApiCaller.RequestTimeOut);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiCaller.TOKEN_KEY_SCHEMA, ApiCaller.TOKEN_KEY_VALUE);
                        //client.DefaultRequestHeaders.Add("PreLog", string.Format("CLOUD_CODE: {0}; STAFF_CODE: {1}; USER_PATIENT_CODE: {2}; SCREEN_ID: {3}; ", AppInfo.staffInfor.CLOUD_CODE, AppInfo.staffInfor.STAFF_CODE, AppInfo.ClientUserPatientCode, AppInfo.ClientScreenID));
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Call API
                        HttpResponseMessage res = await client.DeleteAsync(fullURL);
                        if (res.IsSuccessStatusCode)
                        {
                            return res.Content.ReadAsAsync<T>().Result;
                        }
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    // CloseWaitingForm();
                    ApiCaller.BlnCancelRequest = true;
                    //TuanNA: fix bug: http://redmine.ominext.co/issues/8755
                    //MessageBox.Show(LanguagesMessage.GetLanguagesMessage("MSG_CONNECTSERVER_FAIL"));
                    throw httpEx;
                }
                return default(T);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
                //    throw;
            }
        }

        #endregion

        #endregion

    }
}