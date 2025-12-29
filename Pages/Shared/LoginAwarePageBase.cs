using eVybir.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Concurrent;

namespace eVybir.Pages.Shared
{
    public abstract class LoginAwarePageBase : PageModel
    {
        private bool _loginCheckDone = false;
        public Login? LoginData
        {
            get
            {
                if (!_loginCheckDone)
                {
                    _loginCheckDone = true;
                    var cookieValue = HttpContext.Request.Cookies[Login.COOKIE];
                    if (cookieValue != null) field = Login.Deserialize(cookieValue);
                }
                return field;
            }
        }

        public bool IsLoggedIn { get => LoginData != null; }

        public abstract string Title { get; }

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

        public void SetPageData(ViewDataDictionary viewData)
        {
            viewData[nameof(LoginData)] = LoginData;
            viewData[nameof(IsLoggedIn)] = IsLoggedIn;
            viewData[nameof(Title)] = Title;
        }

        private static readonly ConcurrentDictionary<Type, string> Locations = new();

        protected static string Location(Type t)
        {
            if (Locations.TryGetValue(t, out var location)) return location;
            const int prefix = 5;// "Pages".Length;
            var result = t.Name.Substring(prefix)
                          .Replace('_', '/')
                          .Split("#")[0];
            Locations[t] = result;
            return result;
        }

        public static string Location<T>() where T : Page => Location(typeof(T));
    }
}
