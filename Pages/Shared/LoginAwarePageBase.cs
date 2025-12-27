using eVybir.Infra;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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


        public void SetPageData(ViewDataDictionary viewData)
        {
            viewData[nameof(LoginData)] = LoginData;
            viewData[nameof(IsLoggedIn)] = IsLoggedIn;
            viewData[nameof(Title)] = Title;
        }

        protected static string Location(Type t)
        {
            const int prefix = 5;// "Pages".Length;
            return t.Name.Substring(prefix)
                         .Replace('_', '/')
                         .Split("#")[0];
        }

        public static string Location<T>(object? id = null) where T : Page => $"{Location(typeof(T))}{(id != null ? $"/{id}" : string.Empty)}";
    }
}
