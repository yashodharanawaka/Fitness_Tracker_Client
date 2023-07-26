using System;
using System.Collections.Generic;
using System.Linq;

namespace EAD_CourseWork1
{
    public partial class WeightTracking : System.Web.UI.Page
    {
        // List to store weight tracking data
        public static List<WeightData> weightDataList = new List<WeightData>();

        // Define a class to represent weight tracking data
        public class WeightData
        {
            public DateTime Date { get; set; }
            public double Weight { get; set; }
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

        protected void btnAddWeight_Click(object sender, EventArgs e)
        {
            // Retrieve weight input
            if (double.TryParse(txtWeight.Text, out double weight))
            {
                // Create a new weight data instance
                WeightData weightData = new WeightData
                {
                    Date = DateTime.Now,
                    Weight = weight
                };

                // Add the weight data to the list
                weightDataList.Add(weightData);

                // Clear the weight input field
                txtWeight.Text = string.Empty;

                // Refresh the grid view
                BindWeightData();
            }
            else
            {
                lblMessage.Text = "Invalid weight input. Please enter a valid weight.";
                lblMessage.Visible = true;
            }
        }

        private void BindWeightData()
        {
            // Bind the weight data list to the grid view
            gridWeights.DataSource = weightDataList;
            gridWeights.DataBind();
        }

        private double PredictNextWeight()
        {
            // Check if there is enough data to make a prediction
            if (weightDataList.Count == 0)
                return 0; // No weight data available

            // Check if there is only one weight entered
            if (weightDataList.Count == 1)
                return weightDataList[0].Weight; // Return the only entered weight as the prediction

            // Perform simple linear regression to predict the next weight

            // Calculate the total number of data points
            int n = weightDataList.Count;

            // Calculate the sum of x (dates in days)
            double sumX = weightDataList.Sum(data => (data.Date - DateTime.MinValue).TotalDays);

            // Calculate the sum of y (weights)
            double sumY = weightDataList.Sum(data => data.Weight);

            // Calculate the sum of x^2
            double sumX2 = weightDataList.Sum(data => Math.Pow((data.Date - DateTime.MinValue).TotalDays, 2));

            // Calculate the sum of xy
            double sumXY = weightDataList.Sum(data => (data.Date - DateTime.MinValue).TotalDays * data.Weight);

            // Check if there is enough data for linear regression
            double denominator = n * sumX2 - Math.Pow(sumX, 2);
            if (denominator == 0)
                return 0; // Insufficient data for prediction

            // Calculate the slope (m) of the regression line
            double slope = (n * sumXY - sumX * sumY) / denominator;

            // Calculate the intercept (b) of the regression line
            double intercept = (sumY - slope * sumX) / n;

            // Predict the next weight
            double nextWeight = slope * ((DateTime.Now - DateTime.MinValue).TotalDays + 1) + intercept;

            return nextWeight;
        }


        protected void btnPredictFitness_Click(object sender, EventArgs e)
        {
            // Retrieve the necessary data from other pages
            List<WorkoutTracking.WorkoutRecord> workoutDataList = WorkoutTracking.workoutDataList;
            List<CheatMealTracking.CheatMealRecord> cheatMealList = CheatMealTracking.cheatMealList;
            Sign_Up.UserData loggedInUser = Sign_In.LoggedInUser;
            List<WeightData> weightDataList = WeightTracking.weightDataList;

            // Retrieve the current user's data
            Sign_Up.UserData currentUserData = registeredUsers.FirstOrDefault(user => user.Username == Sign_In.LoggedInUser.Username);

            if (currentUserData != null)
            {
                // Doing fitness status prediction based on the available data
                double bmi = CalculateBMI(currentUserData.Height, currentUserData.Weight);
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
                return recentWeightData.Weight;
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

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Update the weight prediction label
            lblWeightPrediction.Text = PredictNextWeight().ToString("F2");
        }

    }
}
