using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace EAD_CourseWork1
{
    public partial class CheatMealTracking : System.Web.UI.Page
    {
        // List to store cheat meal tracking data
        public static List<CheatMealRecord> cheatMealList;

        // Define a class to represent cheat meal tracking data
        public class CheatMealRecord
        {
            public string MealName { get; set; }
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

        protected void btnAddMeal_Click(object sender, EventArgs e)
        {
            // Make the table visible when new data is added
            cheatMealReportContainer.Visible = true;

            // Retrieve meal name and calories inputs
            string mealName = txtMealName.Text;
            if (int.TryParse(txtCalories.Text, out int calories))
            {
                // Create a new cheat meal data instance
                CheatMealRecord cheatMealData = new CheatMealRecord
                {
                    MealName = mealName,
                    Calories = calories,
                    Date = DateTime.Now
                };

                // Add the cheat meal data to the list
                cheatMealList.Add(cheatMealData);

                // Clear the inputs
                txtMealName.Text = string.Empty;
                txtCalories.Text = string.Empty;
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
                    Text = cheatMeal.MealName
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
