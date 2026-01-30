using eVybir.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Concurrent;

namespace eVybir.Pages.Shared
{
    public abstract class LoginAwarePageBase : PageModel
    {
        #region Authentication
        public async Task LoginUser(string userId)
        {
            CookieOptions LoginCookieOptions = new() { Path = HttpContext.Request.PathBase, HttpOnly = true };
            if (!string.IsNullOrEmpty(userId))
            {
                var identity = await Login.LogIn(userId);
                if (identity != null) //success
                {
                    HttpContext.Response.Cookies.Append(Login.COOKIE, Login.Serialize(identity), LoginCookieOptions);
                    LoginData = identity;
                    return;
                }
            }
            HttpContext.Response.Cookies.Delete(Login.COOKIE, LoginCookieOptions);
        }

        private bool _loginCheckDone = false;
        public Login? LoginData
        {
            get
            {
                if (!_loginCheckDone)
                {
                    _loginCheckDone = true;
                    var cookieValue = HttpContext.Request.Cookies[Login.COOKIE];
                    if (cookieValue != null)
                    {
                        try
                        {
                            var cookieData = Login.Deserialize(cookieValue);
                            field = cookieData?.expire > DateTime.Now ? cookieData : null;
                        }
                        catch
                        {
                            field = null;
                        }
                    }
                }
                return field;
            }
            private set { field = value; }
        }

        public bool IsLoggedIn { get => LoginData != null; }
        #endregion

        #region Authorization
        protected bool CheckRole<T>(out IActionResult? result) where T : Page => CheckRole(typeof(T), out result);

        protected bool CheckRole(Type t, out IActionResult? result)
        {
            if (!IsLoggedIn)
            {
                result = Unauthorized();
                return false;
            }
            if (!LoginData!.Granted(t))
            {
                result = new StatusCodeResult(403);
                return false;
            }
            result = null;
            return true;
        }
        #endregion

        #region Metadata
        public abstract string Title { get; }

        public void SetPageData(ViewDataDictionary viewData)
        {
            viewData[nameof(LoginData)] = LoginData;
            viewData[nameof(IsLoggedIn)] = IsLoggedIn;
            viewData[nameof(Title)] = Title;
        }
        #endregion

        #region Location
        private static readonly ConcurrentDictionary<Type, string> Locations = new();

        protected static string Location(Type t)
        {
            if (Locations.TryGetValue(t, out var location)) return location;
            const int prefix = 6;// "Pages_".Length;
            var result = t.Name.Substring(prefix)
                          .Replace('_', '/')
                          .Split("#")[0];
            Locations[t] = result;
            return result;
        }

        public static string Location<T>() where T : Page => Location(typeof(T));
        #endregion
    }
}
