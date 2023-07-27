using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace EAD_CourseWork1
{
    public partial class WorkoutTracking : System.Web.UI.Page
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string apiGatewayUrl = "https://localhost:7278/gateway";
        // List to store workout tracking data
        public static List<WorkoutRecord> workoutDataList = new List<WorkoutRecord>();

        // class to represent workout tracking data
        public class WorkoutRecord
        {
            public int UserId { get; set; }
            public string Exercise { get; set; }
            public int Duration { get; set; }
            public string Intensity { get; set; }
            public DateTime date { get; set; }
        }

        public class Exercise
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class IntensityType
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                    if (!Sign_In.IsAuthenticated)
                    {
                    // Store the current page URL in the session
                    Sign_In.RedirectUrl = Request.UrlReferrer?.ToString();
                    // Redirect the user to the login page or display an error message
                    Response.Redirect("~/Sign_In.aspx");
                        return;
                    }

                // Populate the Intensity dropdown list
                ddlIntensity.DataSource = GetIntensityData();
                ddlIntensity.DataTextField = "Name";
                ddlIntensity.DataValueField = "Id";
                ddlIntensity.DataBind();

                // Populate the Exercise dropdown list
                ddlExercise.DataSource = GetExerciseData();
                ddlExercise.DataTextField = "Name";
                ddlExercise.DataValueField = "Id";
                ddlExercise.DataBind();
                BindWorkoutData();

            }
        }

        private async Task<bool> SaveWorkoutDataAsync(WorkoutRecord workoutData)
        {
            try
            {
                // Serialize the workout data object to JSON
                var workoutDataJson = JsonConvert.SerializeObject(workoutData);

                // Create a StringContent with the JSON data
                var content = new StringContent(workoutDataJson, Encoding.UTF8, "application/json");

                // Make a POST request to the workout tracking service through the API gateway
                var response = await httpClient.PostAsync($"{apiGatewayUrl}/workout/add", content);
                response.EnsureSuccessStatusCode();

                return true; // Successfully saved workout data
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or display error message)
                // ...

                return false; // Failed to save workout data
            }
        }

        protected async void btnAddWorkout_Click(object sender, EventArgs e)
        {
            string exercise = ddlExercise.SelectedValue;
            if (int.TryParse(txtDuration.Text, out int duration))
            {
                string intensity = ddlIntensity.SelectedValue;

                // Creating a new workout data instance
                WorkoutRecord workoutData = new WorkoutRecord
                {
                    Exercise = exercise,
                    Duration = duration,
                    Intensity = intensity,
                    date = DateTime.Now
                };

                // Call the API to save the workout data
                bool isSaved = await SaveWorkoutDataAsync(workoutData);

                if (isSaved)
                {
                    // Clear the inputs
                    ddlExercise.SelectedIndex = 0;
                    txtDuration.Text = string.Empty;
                    ddlIntensity.SelectedIndex = 0;

                    // Refresh the table
                    BindWorkoutData();
                }
                else
                {
                    lblMessage.Text = "Failed to save workout data. Please try again.";
                    lblMessage.Visible = true;
                }
            }
            else
            {
                lblMessage.Text = "Invalid duration input. Please enter a valid number of minutes.";
                lblMessage.Visible = true;
            }
        }


        protected async void btnGenerateReport_Click(object sender, EventArgs e)
        {
            // Assuming you have stored the user ID after successful login
            int userId = Sign_In.LoggedInUser.Id;

            // Make the HTTP GET request with the user ID
            List<WorkoutRecord> lastWeekWorkouts = await GetLastWeekWorkoutDataAsync(userId);

            // Bind the filtered workout data to the grid view
            gridWorkouts.DataSource = lastWeekWorkouts;
            gridWorkouts.DataBind();

            // Display the report in the label
            gridWorkouts.Visible = true;
        }

        private async Task<List<WorkoutRecord>> GetLastWeekWorkoutDataAsync(int userId)
        {
            try
            {
                // Make a GET request to the workout tracking service through the API gateway
                var response = await httpClient.GetAsync($"{apiGatewayUrl}/workout/report?userId={userId}");
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                string responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize the response content to List<WorkoutRecord> using JavaScriptSerializer
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var lastWeekWorkouts = serializer.Deserialize<List<WorkoutRecord>>(responseContent);

                return lastWeekWorkouts;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or display error message)
                // ...

                return new List<WorkoutRecord>(); // Return an empty list in case of an error
            }
        }

        //  read "Exercises.xml" and get a list of Exercise objects
        private List<Exercise> GetExerciseData()
        {
            XDocument doc = XDocument.Load(Server.MapPath("~/App_Data/Exercises.xml"));
            var exercises = doc.Descendants("Exercise")
                              .Select(x => new Exercise
                              {
                                  Id = int.Parse(x.Element("Id").Value),
                                  Name = x.Element("Name").Value
                              })
                              .ToList();
            return exercises;
        }

        //read "IntensityTypes.xml" and get a list of IntensityType objects
        private List<IntensityType> GetIntensityData()
        {
            XDocument doc = XDocument.Load(Server.MapPath("~/App_Data/IntensityTypes.xml"));
            var intensityTypes = doc.Descendants("Intensity")
                                    .Select(x => new IntensityType
                                    {
                                        Id = int.Parse(x.Element("Id").Value),
                                        Name = x.Element("Name").Value
                                    })
                                    .ToList();
            return intensityTypes;
        }

        private void BindWorkoutData()
        {
            // Bind the workout data list to the grid view
            gridWorkouts.DataSource = workoutDataList;
            gridWorkouts.DataBind();
        }
    }
}