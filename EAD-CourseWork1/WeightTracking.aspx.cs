using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static EAD_CourseWork1.Sign_Up;

namespace EAD_CourseWork1
{
    public partial class WeightTracking : System.Web.UI.Page
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string apiGatewayUrl = "https://localhost:7278/gateway";
        private readonly ILogger<WeightTracking> _logger;
        // List to store weight tracking data
        public static List<WeightData> weightDataList = new List<WeightData>();

        // Define a class to represent weight tracking data
        public class WeightData
        {
            public int UserId { get; set; }
            public DateTime Date { get; set; }
            public double Value { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if the user is authenticated
                if (!Sign_In.IsAuthenticated)
                {
                    // Store the current page URL in the session
                    Sign_In.RedirectUrl = Request.UrlReferrer?.ToString();
                    //Redirect the user to the login page
                    Response.Redirect("~/Sign_In.aspx");
                    return;
                }

                BindWeightData();
            }
        }

        protected async void Page_PreRender(object sender, EventArgs e)
        {
            // Update the weight prediction label
            double nextWeight = await PredictNextWeightAsync();
            lblWeightPrediction.Text = nextWeight.ToString("F2");
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
                    // Create a new weight data instance
                    WeightData weightData = new WeightData
                    {
                        Date = DateTime.Now,
                        Value = weight,
                        UserId = Sign_In.LoggedInUser.Id
                    };

                    // Add the weight data to the list
                    weightDataList.Add(weightData);

                    // Clear the weight input field
                    txtWeight.Text = string.Empty;

                    // Refresh the grid view
                    BindWeightData();

                    // Update the weight prediction label after saving the new weight
                    double nextWeight = await PredictNextWeightAsync();
                    lblWeightPrediction.Text = nextWeight.ToString("F2");
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
            // Check if there is enough data to make a prediction
            if (weightDataList.Count == 0)
                return 0; // No weight data available

            // Handle special case with just one weight entered
            if (weightDataList.Count == 1)
                return weightDataList[0].Value; // Return the only entered weight as the prediction

            // Get the user ID of the logged-in user
            int userId = Sign_In.LoggedInUser.Id;

            // Make an HTTP GET request to the weight tracking microservice to calculate the next weight

            HttpResponseMessage response = await httpClient.GetAsync($"{apiGatewayUrl}/weight/nextweight/{userId}");

            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the response body to a double
                if (double.TryParse(responseBody, out double nextWeight))
                {
                    return nextWeight;
                }
            }

            // If the request fails or parsing fails
            return 0;
        }


        protected void btnPredictFitness_Click(object sender, EventArgs e)
        {
            // Retrieve the necessary data from other pages
            List<WorkoutTracking.WorkoutRecord> workoutDataList = WorkoutTracking.workoutDataList;
            List<CheatMealTracking.CheatMealRecord> cheatMealList = CheatMealTracking.cheatMealList;
            Sign_Up.UserData loggedInUser = Sign_In.LoggedInUser;
            List<WeightData> weightDataList = WeightTracking.weightDataList;

            if (loggedInUser != null)
            {
                // Doing fitness status prediction based on the available data
                double bmi = CalculateBMI(loggedInUser.Height, loggedInUser.Weight);
                int exerciseDuration = GetTotalExerciseDuration(workoutDataList);
                int cheatMealCount = GetCheatMealCount(cheatMealList);
                double recentWeight = GetRecentWeight(weightDataList);

                string fitnessStatus = PredictFitnessStatus(bmi, exerciseDuration, cheatMealCount, recentWeight);

                lblFitnessStatus.Text = "Your predicted fitness status: " + fitnessStatus;
                lblFitnessStatus.Visible = true;
            }
            else
            {
                lblFitnessStatus.Text = "User data not found. Please sign up and provide necessary information.";
                lblFitnessStatus.Visible = true;
            }
        }

        private double CalculateBMI(double height, double weight)
        {
            // Calculating BMI using the formula: weight (kg) / (height (m))^2
            // Converting height from cm to meters
            double heightInMeters = height / 100;
            return weight / (heightInMeters * heightInMeters);
        }

        private int GetTotalExerciseDuration(List<WorkoutTracking.WorkoutRecord> workoutDataList)
        {
            // Calculate the total exercise duration for the user
            return workoutDataList.Sum(data => data.Duration);
        }

        private int GetCheatMealCount(List<CheatMealTracking.CheatMealRecord> cheatMealList)
        {
            if (cheatMealList != null)
            {
                // Count the number of cheat meals recorded for the user
                return cheatMealList.Count;
            }
            else
            {
                // Handle the case when cheatMealList is null
                return 0;
            }
        }

        private double GetRecentWeight(List<WeightData> weightDataList)
        {
            // Get the most recent weight entry for the user
            if (weightDataList.Count > 0)
            {
                WeightTracking.WeightData recentWeightData = weightDataList.OrderByDescending(data => data.Date).First();
                return recentWeightData.Value;
            }
            else
            {
                // Default value if no weight data is available
                return 0;
            }
        }

        private string PredictFitnessStatus(double bmi, int exerciseDuration, int cheatMealCount, double recentWeight)
        {
            // fitness status prediction logic
            if (bmi < 18.5 && exerciseDuration > 120 && cheatMealCount < 3 && recentWeight < 70)
            {
                return "Fit";
            }
            else if (bmi >= 18.5 && bmi <= 24.9 && exerciseDuration > 60 && cheatMealCount < 5 && recentWeight >= 70 && recentWeight <= 80)
            {
                return "Average";
            }
            else
            {
                return "Unfit";
            }
        }

    }
}
