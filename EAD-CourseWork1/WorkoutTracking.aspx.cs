using System;
using System.Collections.Generic;
using System.Linq;

namespace EAD_CourseWork1
{
    public partial class WorkoutTracking : System.Web.UI.Page
    {
        // List to store workout tracking data
        public static List<WorkoutRecord> workoutDataList = new List<WorkoutRecord>();

        // class to represent workout tracking data
        public class WorkoutRecord
        {
            public string Exercise { get; set; }
            public int Duration { get; set; }
            public string Intensity { get; set; }
            public DateTime date { get; set; }
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

                BindWorkoutData();
            }
        }
        
        protected void btnAddWorkout_Click(object sender, EventArgs e)
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

                // Add the workout data to the list
                workoutDataList.Add(workoutData);

                // Clear the inputs
                ddlExercise.SelectedIndex = 0;
                txtDuration.Text = string.Empty;
                ddlIntensity.SelectedIndex = 0;

                // Refresh the table
                BindWorkoutData();
            }
            else
            {
                lblMessage.Text = "Invalid duration input. Please enter a valid number of minutes.";
                lblMessage.Visible = true;
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            DateTime currentDate = DateTime.Now;
            // Set the start date to 7 days ago
            DateTime startDate = currentDate.AddDays(-7); 

            List<WorkoutRecord> recentWorkouts = workoutDataList
                .Where(workout => workout.date >= startDate && workout.date <= currentDate)
                .ToList();

            // Bind the filtered workout data to the grid view
            gridWorkouts.DataSource = recentWorkouts;
            gridWorkouts.DataBind();

            // Display the report in the label
            gridWorkouts.Visible = true;
        }


        private void BindWorkoutData()
        {
            // Bind the workout data list to the grid view
            gridWorkouts.DataSource = workoutDataList;
            gridWorkouts.DataBind();
        }
    }
}