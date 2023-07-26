using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace EAD_CourseWork1
{
    public partial class Sign_Up : System.Web.UI.Page
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string apiGatewayUrl = "https://localhost:7278";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if the user is authenticated
                if (Sign_In.IsUserAuthenticated())
                {
                    //Redirect the user to the login page
                    Response.Redirect("~/Default.aspx");
                    return;
                }
            }
        }

        // List to store registered users in memory
        //public static List<UserData> registeredUsers = new List<UserData>();

        public class UserData
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Gender { get; set; }
            public double Height { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public double Weight { get; set; }
        }

        protected async void btnSignUp_Click(object sender, EventArgs e)
        {
            // Retrieve input values from the form
            string name = txtName.Text;
            int age = Convert.ToInt32(txtAge.Text);
            string gender = ddlGender.SelectedValue;
            double height = Convert.ToDouble(txtHeight.Text);
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            double weight = Convert.ToDouble(txtWeight.Text);

            // Check if the username is already taken
            string endpointUrl = $"{apiGatewayUrl}/gateway/User";

            try
            {

                HttpResponseMessage response = await httpClient.GetAsync(endpointUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response to a list of User objects
                    List<UserData> users = JsonConvert.DeserializeObject<List<UserData>>(responseBody);

                    if (users.Any(user => user.Username == username))
                    {
                        lblMessage.Text = "Username is already taken. Please choose a different username.";
                        lblMessage.Visible = true;
                        return;
                    }
                    else
                    {
                        // Create a new instance of UserData
                        UserData newUser = new UserData
                        {
                            Name = name,
                            Age = age,
                            Gender = gender,
                            Height = height,
                            Username = username,
                            Password = password,
                            Weight = weight
                        };

                        // Authenticate the user
                        AuthorizeUser(newUser);

                        // Clear the form fields
                        txtName.Text = string.Empty;
                        txtAge.Text = string.Empty;
                        ddlGender.ClearSelection();
                        txtHeight.Text = string.Empty;
                        txtUsername.Text = string.Empty;
                        txtPassword.Text = string.Empty;
                        txtWeight.Text = string.Empty;

                        Response.Redirect("Default.aspx");
                    }
                }
                else
                {
                    // Handle error response
                    lblMessage.Text = $"Failed to fetch users: {response.StatusCode} - {response.ReasonPhrase}";
                    lblMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                lblMessage.Text = $"An error occurred: {ex.Message}";
            }
        }

        private async void AuthorizeUser(UserData newUser)
        {
            // Add the new user to the list
            try
            {
                string endpointUrl = $"{apiGatewayUrl}/user-microservice/api/User";

                // Serialize the User object to JSON
                string jsonUser = JsonConvert.SerializeObject(newUser);
                var content = new StringContent(jsonUser, Encoding.UTF8, "application/json");

                // Make the POST request to add the user
                HttpResponseMessage response = await httpClient.PostAsync(endpointUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // User added successfully
                    // User is authenticated
                    Sign_In.IsAuthenticated = true;
                    Sign_In.LoggedInUser = newUser;
                }
                else
                {
                    // Handle error response
                    string errorMessage = $"Failed to add user: {response.StatusCode} - {response.ReasonPhrase}";
                    ClientScript.RegisterStartupScript(this.GetType(), "errorPopup", $"showErrorPopup('{errorMessage}');", true);
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                string errorMessage = $"An error occurred: {ex.Message}";
                ClientScript.RegisterStartupScript(this.GetType(), "errorPopup", $"showErrorPopup('{errorMessage}');", true);
            }
        }
    }
}
