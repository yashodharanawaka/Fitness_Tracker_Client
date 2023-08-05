using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EAD_CourseWork1
{
    public partial class WeightTracking : System.Web.UI.Page
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string apiGatewayUrl = "https://localhost:7278/gateway";
        private readonly ILogger<WeightTracking> _logger;
        public static List<WeightData> weightDataList = new List<WeightData>();

        public class WeightData
        {
            public int UserId { get; set; }
            public DateTime Date { get; set; }
            public double Value { get; set; }
        }

        public class FitnessPrediction
        {
            public int UserId { get; set; }
            public double FitnessScore { get; set; }
            public string FitnessStatus { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!Sign_In.IsAuthenticated)
                {
                    Sign_In.RedirectUrl = Request.UrlReferrer?.ToString();
                    Response.Redirect("~/Sign_In.aspx");
                    return;
                }
            }
        }

        private async Task LoadWeightData()
        {
            double? nextWeight = await PredictNextWeightAsync();
            if (nextWeight.HasValue)
            {
                lblWeightPrediction.Text = nextWeight.Value.ToString("F2");
            }
            else
            {
                // next weight prediction is not available
                lblWeightPrediction.Text = "Next weight prediction is not avaiblable.";
            }
        }

        protected async void Page_PreRender(object sender, EventArgs e)
        {
            await LoadWeightData();
        }

        private async Task<bool> SaveWeightDataAsync(double weight)
        {
            try
            {
                var weightData = new WeightData
                {
                    Date = DateTime.Now,
                    Value = weight,
                    UserId = Sign_In.LoggedInUser.Id
                };

                // Serialize the weight data object to JSON
                var weightDataJson = JsonConvert.SerializeObject(weightData);

                // Create a StringContent with the JSON data
                var content = new StringContent(weightDataJson, Encoding.UTF8, "application/json");

                // Make a POST request to the weight tracking service through the API gateway
                var response = await httpClient.PostAsync($"{apiGatewayUrl}/weight", content);
                response.EnsureSuccessStatusCode();

                return true; // Successfully saved weight data
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured : ", ex.Message);

                return false; // Failed to save weight data
            }
        }


        protected async void btnAddWeight_Click(object sender, EventArgs e)
        {
            // Retrieve weight input
            if (double.TryParse(txtWeight.Text, out double weight))
            {
                // Call the SaveWeightDataAsync method to save the weight data
                var isSaved = await SaveWeightDataAsync(weight);

                if (isSaved)
                {
                    // Update the weight prediction label after saving the new weight
                    double? nextWeight = await PredictNextWeightAsync();
                    if (nextWeight.HasValue)
                    {
                        lblWeightPrediction.Text = nextWeight.Value.ToString("F2");
                    }
                    else
                    {
                        // next weight prediction is not available
                        lblWeightPrediction.Text = "Next weight prediction is not avaiblable.";
                    }
                }
                else
                {
                    lblMessage.Text = "Failed to save weight data. Please try again.";
                    lblMessage.Visible = true;
                }
            }
            else
            {
                lblMessage.Text = "Invalid weight input. Please enter a valid weight.";
                lblMessage.Visible = true;
            }
        }



        private async void BindWeightData()
        {
            try
            {
                // Make a GET request to the weight tracking service through the API gateway
                var response = await httpClient.GetAsync($"{apiGatewayUrl}/weight");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                // Deserialize the JSON response to a list of User objects
                List<WeightData> weightDataList = JsonConvert.DeserializeObject<List<WeightData>>(responseBody);

                // Bind the weight data list to the grid view
                gridWeights.DataSource = weightDataList;
                gridWeights.DataBind();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or display error message)
                // ...
            }
        }

        private async Task<double> PredictNextWeightAsync()
        {
            // Get the user ID of the logged-in user
            int userId = Sign_In.LoggedInUser.Id;


            HttpResponseMessage response = await httpClient.GetAsync($"{apiGatewayUrl}/weight/nextweight/{userId}");

            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the response body to a double
                if (double.TryParse(responseBody, out double nextWeight))
                {
                    // Check if the next weight is negative (-1)
                    if (nextWeight >= 0)
                    {
                        return nextWeight;
                    }
                }
            }

            // If the request fails or parsing fails
            return 0;
        }


        protected async void btnPredictFitness_Click(object sender, EventArgs e)
        {
            Sign_Up.UserData loggedInUser = Sign_In.LoggedInUser;

            if (loggedInUser != null)
            {
                try
                {
                    // Make a GET request to the weight tracking service through the API gateway
                    var response = await httpClient.GetAsync($"{apiGatewayUrl}/fitnessprediction/{loggedInUser.Id}");
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    // Deserialize the JSON response to a list of User objects
                    FitnessPrediction fitnessPrediction = JsonConvert.DeserializeObject<FitnessPrediction>(responseBody);

                    // set fitness status
                    lblFitnessStatus.Text = $"Fitness Status : {fitnessPrediction.FitnessStatus}";
                    lblFitnessStatus.Visible = true;
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log or display error message)
                    // ...
                }
            }
            else
            {
                lblFitnessStatus.Text = "User data not found. Please sign up and provide necessary information.";
                lblFitnessStatus.Visible = true;
            }
        }

    }
}
