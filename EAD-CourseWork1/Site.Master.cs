﻿using System;
using System.Web;
using System.Web.UI;

namespace EAD_CourseWork1
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int currentYear = DateTime.Now.Year;
                lblFooter.Text = $"&copy; {currentYear} - Fitness Tracker";
            }
        }
    }
}