using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Meliasoft.Data;
using Meliasoft.Models;
using Meliasoft.Utilities;
using RestSharp;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;
using AllowAnonymousAttribute = System.Web.Mvc.AllowAnonymousAttribute;
using ActionResult = System.Web.Mvc.ActionResult;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
using ControllerContext = System.Web.Mvc.ControllerContext;
using System.Xml.Linq;
using System.Net.PeerToPeer;
using System.Reflection;
using System.Security.Cryptography;

namespace Meliasoft.Controllers
{
  [Authorize]
  public class AccountController : BaseController
  {
    public AccountController(IMeliasoftData meliasoftData)
        : base(meliasoftData)
    {
    }

    //
    // GET: /Account/Login
    [AllowAnonymous]
    public async Task<ActionResult> Login(string returnUrl, string webai, string accKey, string urname, string Ma_DVCS = "") //string user, string pwd, , string address
    {
      try
      {
        string username = urname; // string.Empty; //user; // string.Empty;
        string password = string.Empty; //pwd; // string.Empty;
        string decryptInfo = string.Empty;
        string decodedUrlString = accKey;
        string accInfo = accKey;
        string maDVCS = Ma_DVCS;
        //string addr = address;

        //Username=ngoctbk;Password=ngoctbk
        //dWnZCIYdkf3U+lucAKpXqLUefsw5ZWixTSA5Mbu6viYiZG7TzHi5K+V7mXSM8qG9v0abcoPpp+koK92P7Q==

        //string inputData = "Data Source=118.70.180.226,3001;Initial Catalog=Meliasoft2016_Nb;User ID=Meliasoft_run;Password=111110117515253545556";
        //string encryptData = await this.EncryptLoginInfoAsync(inputData);

        ///Report/Home?webai=yes&urname=mls&accKey=5pnIOjbJwhCmbKYHFGktLin4mNt%2F2QfsBgykatXUZutwYw1kZdpmwBuUeD9airGYvPurw%2BoqgjdCeIB5iYPZ5m%2FnpwCaufrIhIABzyDIcn%2F0iqna96q%2BvQ%3D%3D

        if (!string.IsNullOrEmpty(returnUrl) && returnUrl.ToLower().Contains("acckey="))
        {
          int idxOfUserName = returnUrl.ToLower().IndexOf("urname=");
          int idxOfKey = returnUrl.ToLower().IndexOf("acckey=");
          //int idxOfaddress = returnUrl.ToLower().IndexOf("address=");
          int idxMaDVCS = returnUrl.ToLower().IndexOf("ma_dvcs=");

          username = returnUrl.Substring(idxOfUserName + 7, idxOfKey - (idxOfUserName + 7 + 1));

          if (idxMaDVCS > idxOfKey)
          {
            accInfo = returnUrl.Substring(idxOfKey + 7, idxMaDVCS - (idxOfKey + 7 + 1));
            maDVCS = returnUrl.Substring(idxMaDVCS + 8);
          }
          else
          {
            accInfo = returnUrl.Substring(idxOfKey + 7);
          }


          if (!string.IsNullOrEmpty(accInfo))
          {
            decodedUrlString = HttpUtility.UrlDecode(accInfo);

            ////string decryptInfo = await this.DecryptLoginInfoAsync(accInfo);
            //decryptInfo = this.MlsDecrypt(decodedUrlString); // this.DecryptLoginInfoAsyncHttp(accInfo);

          }
        } 

        if (!string.IsNullOrEmpty(decodedUrlString))
        {
          decryptInfo = this.MlsDecrypt(decodedUrlString);

          if (decryptInfo.ToLower().Contains("user id") && decryptInfo.ToLower().Contains("password"))
          {

            _meliasoftData.UserName = username;// "blank_user";
            var claims = new List<Claim>();

            // create required claims
            claims.Add(new Claim(ClaimTypes.NameIdentifier, username + "_blank_user")); //"blank_user", username
            claims.Add(new Claim(ClaimTypes.Name, username)); //"blank_user"
            claims.Add(new Claim(ClaimTypes.UserData, decryptInfo));
            //claims.Add(new Claim(ClaimTypes.StreetAddress, addr));
            claims.Add(new Claim(ClaimTypes.StateOrProvince, maDVCS));

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
              AllowRefresh = true,
              IsPersistent = true,
              ExpiresUtc = DateTime.UtcNow.AddDays(180)
            }, identity);

            return RedirectToLocal(returnUrl);

          }
          else if (decryptInfo.ToLower().Contains("username"))
          {
            List<string> stringList = decryptInfo.Split(';').ToList();
            if (stringList.Count >= 2)
            {

              List<string> uList = stringList[0].Split('=').ToList();
              List<string> pList = stringList[1].Split('=').ToList();
              if (uList.Count >= 2 && pList.Count >= 2)
              {
                username = uList[1];
                password = pList[1];
              }

            }
          }

        }



        if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
        {
          //return Json(new { Success = false, Error = $"Error: if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))" }, JsonRequestBehavior.AllowGet);
          ViewBag.ReturnUrl = returnUrl;
          ////return View();
        }
        else
        {
          LoginViewModel model = new LoginViewModel();
          model.UserName = username;
          model.Password = password;
          _ = this.Login(model, "/Report/Home");
        }

        return View();

      }
      catch (Exception ex)
      {
        return Json(new { Success = false, Error = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
      }
            

    }

    public async Task<string> EncryptLoginInfoAsync(string inputData, string inputPwd = "mls2026")
    {
      string encrytedText = "";

      var options = new RestClientOptions("https://api.botautoai.com/api/encrypt")
      {
        Timeout = TimeSpan.FromSeconds(60) // Set a 30-second timeout
      };
      var client = new RestClient(options);
      var request = new RestRequest("https://api.botautoai.com/api/encrypt", Method.Post);

      request.AddHeader("Content-Type", "application/json");
      var body = @"{" + "\n" +
              @"    ""text"":""" + inputData + @"""," + "\n" + //Username=ngoctbk;Password=ngoctbk"",        
              @"    ""password"":""" + inputPwd + @""" " + "\n" +
              @"}";

      request.AddStringBody(body, DataFormat.Json);
      RestResponse response = await client.ExecuteAsync(request);
      Console.WriteLine(response.Content);
      encrytedText = (string)response.Content;

      //{"result":"dWnZCIYdkf3U+lucAKpXqLUefsw5ZWixTSA5Mbu6viYiZG7TzHi5K+V7mXSM8qG9v0abcoPpp+koK92P7Q==""}

      return encrytedText;
    }


    public async Task<string> DecryptLoginInfoAsync(string inputData, string inputPwd = "mls2026")
    {
      string decryptedText = "";
      string decryptAPI = "https://api.botautoai.com/api/mlsdecrypt"; //decrypt

      var options = new RestClientOptions(decryptAPI)
      {
        Timeout = TimeSpan.FromSeconds(60) // Set a reasonable timeout (e.g., 30 seconds)
      };
      var client = new RestClient(options);
      var request = new RestRequest();
      request.Method = Method.Post;
      request.AddHeader("Content-Type", "application/json");

      // Construct request body using anonymous object
      var body = new
      {
        text = inputData,
        password = inputPwd
      };

      request.AddJsonBody(body);  // Automatically serializes the object to JSON

      try
      {
        // Execute request asynchronously
        RestResponse response = await client.ExecuteAsync(request);

        // Check if the response is successful
        if (response.IsSuccessful)
        {
          // Deserialize the response content
          var result = JsonConvert.DeserializeObject<Response>(response.Content);
          if (result != null && !string.IsNullOrEmpty(result.Result))
          {
            decryptedText = result.Result.Replace("\n", ""); // Remove any extra newlines if needed
          }
        }
        else
        {
          Console.WriteLine($"Error: {response.StatusCode} - {response.ErrorMessage}");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception occurred: {ex.Message}");
      }

      return decryptedText;
    }


    public string MlsDecrypt(string strInput)
    {
      string Mlscryptkey = "MLSmls123321MLS"; //MLSMLSmls123321MLSMLS

      bool useHashing = true;
      byte[] keyArray;
      byte[] toEncryptArray = Convert.FromBase64String(strInput);

      if (useHashing)
      {
        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
        keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Mlscryptkey));
      }
      else
        keyArray = UTF8Encoding.UTF8.GetBytes(Mlscryptkey);

      TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
      tdes.Key = keyArray;
      tdes.Mode = CipherMode.ECB;
      tdes.Padding = PaddingMode.PKCS7;

      ICryptoTransform cTransform = tdes.CreateDecryptor();

      try
      {
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return UTF8Encoding.UTF8.GetString(resultArray);
      }
      catch
      {
        //Lấy đường dẫn chứa file chạy
        string path = Assembly.GetExecutingAssembly().Location;

        //if (Meliasoft.SysVars.M_Robot_Is_Runing == true)
        //{
        //    //Dừng màn hình 2 giây
        //    Mls_MessageBox.Show("Check: [" + path + "]", "Meliasoft", 2000);
        //}
        //else
        //{
        //    MessageBox.Show("Check: [" + path + "]", "Meliasoft", MessageBoxButton.OK);
        //}

        return "";
      }
    }


    public string DecryptLoginInfoAsyncHttp(string inputData, string inputPwd = "mls2026")
    {
      string decryptAPI = "https://api.botautoai.com/api/mlsdecrypt"; ///api/decrypt
      var client = new HttpClient();
      var request = new HttpRequestMessage(HttpMethod.Post, decryptAPI);

      var content = new StringContent("{\"text\":\"" + inputData + "\",   \"password\":\"" + inputPwd + "\"}", Encoding.UTF8, "application/json");
      request.Content = content;
      var response = client.SendAsync(request).Result;
      //var response = await client.SendAsync(request);

      response.EnsureSuccessStatusCode();

      string text = response.Content.ReadAsStringAsync().Result;

      // Deserialize the response content
      var result = JsonConvert.DeserializeObject<Response>(text);
      if (result != null && !string.IsNullOrEmpty(result.Result))
      {
        text = result.Result.Replace("\n", ""); // Remove any extra newlines if needed
      }


      return text;

    }

    //
    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      try
      {

        _meliasoftData.UserName = string.Empty;

        var dtData = _meliasoftData.Query("SELECT Id, Email, Name, Open_Key FROM Sys_User WHERE Email = @Email AND Password = @Password",
                System.Data.CommandType.Text,
                new string[] { "@Email", "@Password" },
                new object[] { model.UserName.ToUpper(), Toolkit.HashString(model.Password) })
            .FirstOrDefault();

        if (dtData != null)
        {
          _meliasoftData.UserName = model.UserName;
          var claims = new List<Claim>();

          // create required claims
          claims.Add(new Claim(ClaimTypes.NameIdentifier, model.UserName));
          claims.Add(new Claim(ClaimTypes.Name, dtData.Name.ToString()));
          claims.Add(new Claim(ClaimTypes.UserData, dtData.Email.ToString() + "," + dtData.Open_Key.ToString()));

          var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

          AuthenticationManager.SignIn(new AuthenticationProperties()
          {
            AllowRefresh = true,
            IsPersistent = model.RememberMe,
            ExpiresUtc = DateTime.UtcNow.AddDays(7)
          }, identity);

          return RedirectToLocal(returnUrl);
          //return RedirectToAction("Home", "Report");
          //return Redirect("http://localhost:5000/Report/Home");
        }
        else
        {
          ModelState.AddModelError("", "Invalid login attempt.");
          return View(model);
        }
      }
      catch (Exception ex)
      {
        return Json(new { Success = false, Error = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
      }

    }

    public ActionResult ChangePassword()
    {
      return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model, string returnUrl)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      string username = GetUserName(); //_meliasoftData.UserName; // = string.Empty;

      //var dtData = _meliasoftData.Query("SELECT Id, Email, Name, Open_Key FROM Sys_User WHERE Email = @Email AND Password = @Password",
      //        System.Data.CommandType.Text,
      //        new string[] { "@Email", "@Password" },
      //        new object[] { username.ToUpper(), Toolkit.HashString(model.OldPassword) })
      //    .FirstOrDefault();

      //if (dtData != null)
      //{
      try
      {
        int resultRecordCount = _meliasoftData.ExecuteNonQuery("SET DATEFORMAT DMY; UPDATE Sys_User SET  Password = @NewPassword WHERE Email = @UserName AND Password = @OldPassword", System.Data.CommandType.Text,
                                                            new string[] { "@NewPassword", "@UserName", "@OldPassword" },
                                                            new object[] { Toolkit.HashString(model.NewPassword), username.ToUpper(), Toolkit.HashString(model.OldPassword) });

        if (resultRecordCount < 1)
        {
          ModelState.AddModelError("OldPassword", "Invalid password attempt.");
          return View(model); //model
        }
        //return RedirectToLocal(returnUrl);

      }
      catch (Exception ex)
      {
        return Json(new { Success = false, Error = string.Format("Error: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
      }

      //}
      //else
      //{
      //    ModelState.AddModelError("", "Invalid password attempt.");
      //    return View(model);
      //}
      //return View(model);

      return RedirectToLocal(returnUrl);
    }


    //
    // POST: /Account/LogOff
    [HttpPost]
    public ActionResult LogOff()
    {
      AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
      return RedirectToAction("Login", "Account");
    }

    #region Helpers
    // Used for XSRF protection when adding external logins
    private const string XsrfKey = "XsrfId";

    private IAuthenticationManager AuthenticationManager
    {
      get
      {
        return HttpContext.GetOwinContext().Authentication;
      }
    }

    private void AddErrors(IdentityResult result)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError("", error);
      }
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }

      return RedirectToAction("Home", "Report");
    }

    internal class ChallengeResult : HttpUnauthorizedResult
    {
      public ChallengeResult(string provider, string redirectUri)
          : this(provider, redirectUri, null)
      {
      }

      public ChallengeResult(string provider, string redirectUri, string userId)
      {
        LoginProvider = provider;
        RedirectUri = redirectUri;
        UserId = userId;
      }

      public string LoginProvider { get; set; }
      public string RedirectUri { get; set; }
      public string UserId { get; set; }

      public override void ExecuteResult(ControllerContext context)
      {
        var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
        if (UserId != null)
        {
          properties.Dictionary[XsrfKey] = UserId;
        }
        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
      }
    }
    #endregion
  }

  // Define a response class that matches the structure of the API response
  public class Response
  {
    [JsonProperty("result")]
    public string Result { get; set; }
  }
}