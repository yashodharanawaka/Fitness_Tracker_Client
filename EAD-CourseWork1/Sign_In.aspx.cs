﻿using Newtonsoft.Json;
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
        private readonly string apiGatewayUrl = "https://localhost:7278/gateway";

        // public static property to store the logged-in user data
        public static UserData LoggedInUser { get; set; }
        public static bool IsAuthenticated { get; set; }
        public static string RedirectUrl { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsAuthenticated && (LoggedInUser != null))
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
            string endpointUrl = $"{apiGatewayUrl}/user";
            HttpResponseMessage response = await httpClient.GetAsync(endpointUrl);
            string responseBody = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON response to a list of User objects
            List<UserData> users = JsonConvert.DeserializeObject<List<UserData>>(responseBody);
            return users;
        }

        protected async void btnSignUp_Click(object sender, EventArgs e)
        {
            Response.Redirect("Sign_Up.aspx", false);
        }

        protected async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            await AuthenticateUser(username, password);
        }


        private async Task AuthenticateUser(string username, string password)
        {
            // Get all users from the User Microservice
            List<UserData> users = await GetAllUsers();

            // Find the user by username and password
            UserData user = users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                if (user.Password == password) {
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
                } else
                {
                    // Display an error message if authentication fails
                    lblMessage.Text = "Invalid username and password combination.";
                    lblMessage.Visible = true;
                }


            }
            else
            {
                // Display an error message if authentication fails
                lblMessage.Text = "No user found for the username.";
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