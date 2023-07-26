using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static EAD_CourseWork1.Sign_Up;

namespace EAD_CourseWork1
{
    public partial class Sign_In : System.Web.UI.Page
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string apiGatewayUrl = "https://localhost:7278";

        // public static property to store the logged-in user data
        public static UserData LoggedInUser { get; set; }
        public static bool IsAuthenticated { get; set; }
        public static string RedirectUrl { get; set; }

        protected async void Page_Load(object sender, EventArgs e)
        {
                if (IsAuthenticated && LoggedInUser != null)
                {
                    if (RedirectUrl != null)
                    {
                        Response.Redirect(RedirectUrl);
                    }
                    else
                    {
                        Response.Redirect("~/Default.aspx");
                    }
                }
        }

        private async Task<List<UserData>> GetAllUsers()
        {
            string endpointUrl = $"{apiGatewayUrl}/gateway/User";
            HttpResponseMessage response = await httpClient.GetAsync(endpointUrl);
            string responseBody = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON response to a list of User objects
            List<UserData> users = JsonConvert.DeserializeObject<List<UserData>>(responseBody);
            return users;
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            Response.Redirect("Sign_Up.aspx");
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            AuthenticateUser(username, password);
        }


        private void AuthenticateUser(string username, string password)
        {
            UserData user = GetAllUsers().FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Set an authenticated flag in the session
                IsAuthenticated = true;
                LoggedInUser = user;

                // Check if there is a stored redirect URL
                if (RedirectUrl != null)
                {
                    // Retrieve the stored URL
                    string redirectUrl = RedirectUrl;

                    // Clear the stored URL from the session
                    RedirectUrl = null;

                    // Redirect to the original page
                    Response.Redirect(redirectUrl);
                }
                else
                {
                    // Redirect to a default page if no stored URL is found
                    Response.Redirect("~/Default.aspx");
                }
            }
            else
            {
                // Display an error message if authentication fails
                lblMessage.Text = "Invalid username or password";
                lblMessage.Visible = true;
            }
        }
        public static bool IsUserAuthenticated()
        {
            // return true if the user is logged in
            return IsAuthenticated;
        }
    }
}