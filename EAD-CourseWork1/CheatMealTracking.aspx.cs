using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace EAD_CourseWork1
{
    public partial class CheatMealTracking : System.Web.UI.Page
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string apiGatewayUrl = "https://localhost:7278/gateway";
        // List to store cheat meal tracking data
        public static List<CheatMealRecord> cheatMealList;

        // Define a class to represent cheat meal tracking data
        public class CheatMealRecord
        {
    
            public int userId { get; set; }
            public string Name { get; set; }
            public int Calories { get; set; }
            public DateTime Date { get; set; }
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

                cheatMealList = new List<CheatMealRecord>();
            }
        }


        private async Task<bool> SaveCheatMealDataAsync(string mealName, int calories)
        {
            try
            {
                var cheatMealData = new CheatMealRecord
                {
                    userId = Sign_In.LoggedInUser.Id,
                    Name = mealName,
                    Calories = calories,
                    Date = DateTime.Now
                };

                // Serialize the cheat meal data object to JSON
                var cheatMealDataJson = JsonConvert.SerializeObject(cheatMealData);

                // Create a StringContent with the JSON data
                var content = new StringContent(cheatMealDataJson, Encoding.UTF8, "application/json");

                // Make a POST request to the cheat meal tracking service through the API gateway
                var response = await httpClient.PostAsync($"{apiGatewayUrl}/cheatmeal", content);
                response.EnsureSuccessStatusCode();

                return true; // Successfully saved cheat meal data
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or display error message)
                // ...

                return false; // Failed to save cheat meal data
            }
        }

        private async Task GenerateCheatMealReportAsync()
        {
            try
            {
                // Make a GET request to the cheat meal tracking service through the API gateway
                var response = await httpClient.GetAsync($"{apiGatewayUrl}/cheatmeal/{Sign_In.LoggedInUser.Id}");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                // Deserialize the JSON response to a list of CheatMealRecord objects
                List<CheatMealRecord> cheatMealDataList = JsonConvert.DeserializeObject<List<CheatMealRecord>>(responseBody);

                // Create a table to display the cheat meals
                Table cheatMealTable = new Table
                {
                    CssClass = "table table-bordered table-striped cheat-meal-table"
                };

                // ... Code to create the table headers and rows as before ...

                // Clear the previous report
                cheatMealReportContainer.Controls.Clear();

                // Add the table to the page
                cheatMealReportContainer.Controls.Add(cheatMealTable);

                // Make the table visible
                cheatMealReportContainer.Visible = true;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or display error message)
            }
        }

        protected async void btnAddMeal_Click(object sender, EventArgs e)
        {
            // Retrieve meal name and calories inputs
            string mealName = txtMealName.Text;
            if (int.TryParse(txtCalories.Text, out int calories))
            {
                // Call the SaveCheatMealDataAsync method to save the cheat meal data
                var isSaved = await SaveCheatMealDataAsync(mealName, calories);

                if (isSaved)
                {
                    // Clear the inputs
                    txtMealName.Text = string.Empty;
                    txtCalories.Text = string.Empty;

                    // Refresh the cheat meal report
                    await GenerateCheatMealReportAsync();
                }
                else
                {
                    lblMessage.Text = "Failed to save cheat meal data. Please try again.";
                    lblMessage.Visible = true;
                }
            }
            else
            {
                lblMessage.Text = "Invalid calories input. Please enter a valid number of calories.";
                lblMessage.Visible = true;
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            List<CheatMealRecord> cheatMealsForDay = cheatMealList.FindAll(meal => meal.Date.Date == DateTime.Now.Date);

            // Create a table to display the cheat meals
            Table cheatMealTable = new Table
            {
                CssClass = "table table-bordered table-striped cheat-meal-table"
            };

            // Create table headers
            TableHeaderRow headerRow = new TableHeaderRow();
            TableHeaderCell cellMeal = new TableHeaderCell
            {
                Text = "Cheat Meal"
            };
            TableHeaderCell cellQuantity = new TableHeaderCell
            {
                Text = "Calories"
            };
            TableHeaderCell cellTime = new TableHeaderCell
            {
                Text = "Time"
            };
            headerRow.Cells.Add(cellMeal);
            headerRow.Cells.Add(cellQuantity);
            headerRow.Cells.Add(cellTime);
            cheatMealTable.Rows.Add(headerRow);

            // Create table rows for each cheat meal
            foreach (CheatMealRecord cheatMeal in cheatMealsForDay)
            {
                TableRow row = new TableRow();
                TableCell cellMealName = new TableCell
                {
                    Text = cheatMeal.Name
                };
                TableCell cellMealQuantity = new TableCell
                {
                    Text = cheatMeal.Calories.ToString()
                };
                TableCell cellMealTime = new TableCell
                {
                    Text = cheatMeal.Date.ToString("HH:mm:ss")
                };
                row.Cells.Add(cellMealName);
                row.Cells.Add(cellMealQuantity);
                row.Cells.Add(cellMealTime);
                cheatMealTable.Rows.Add(row);
            }

            // Clear the previous report
            cheatMealReportContainer.Controls.Clear();

            // Add the table to the page
            cheatMealReportContainer.Controls.Add(cheatMealTable);

            // Make the table visible
            cheatMealReportContainer.Visible = true;
        }
    }
}
