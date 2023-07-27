using System;
using System.Web.UI;

namespace EAD_CourseWork1
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bool isAuthenticated = Sign_In.IsAuthenticated;
                if (!isAuthenticated)
                {
                    // Store the current page URL in the session
                    Sign_In.RedirectUrl = Request.UrlReferrer?.ToString();
                    // Redirect the user to the login page or display an error message
                    Response.Redirect("~/Sign_In.aspx");
                    return;
                }
            }
        }
    }
}