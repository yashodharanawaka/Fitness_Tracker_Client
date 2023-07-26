<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CheatMealTracking.aspx.cs" Inherits="EAD_CourseWork1.CheatMealTracking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section id="main-content">
        <h2>Cheat Meal Tracking</h2>
        <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="text-danger"></asp:Label>
        <div class="form-group">
            <label for="txtMealName">Meal Name:</label>
            <asp:TextBox ID="txtMealName" runat="server" placeholder="Enter meal name" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="txtCalories">Calories:</label>
            <asp:TextBox ID="txtCalories" runat="server" placeholder="Enter calories" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="form-group">
            <asp:Button ID="btnAddMeal" runat="server" Text="Add Meal" OnClick="btnAddMeal_Click" CssClass="btn btn-primary mt-3" />
        </div>
        <div class="d-flex justify-content-end mb-3">
            <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Daily Cheat Meal Report" OnClick="btnGenerateReport_Click" CssClass="btn btn-secondary" />
        </div>
        <div id="cheatMealReportContainer" runat="server" visible="false" class="mt-3">
            <table id="tblCheatMealReport" class="cheat-meal-table table table-bordered">
            </table>
        </div>
    </section>
</asp:Content>

