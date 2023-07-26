<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WorkoutTracking.aspx.cs" Inherits="EAD_CourseWork1.WorkoutTracking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section id="main-content">
        <h2>Workout Tracking</h2>
        <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="text-danger"></asp:Label>
        <div class="form-group">
            <label for="ddlExercise">Exercise:</label>
            <asp:DropDownList ID="ddlExercise" runat="server" placeholder="Select exercise" CssClass="form-control"></asp:DropDownList>
        </div>
        <div class="form-group">
            <label for="txtDuration">Duration (minutes):</label>
            <asp:TextBox ID="txtDuration" runat="server" placeholder="Enter duration (minutes)" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="ddlIntensity">Intensity:</label>
            <asp:DropDownList ID="ddlIntensity" runat="server" placeholder="Select intensity" CssClass="form-control"></asp:DropDownList>
        </div>
        <div class="form-group">
            <asp:Button ID="Button1" runat="server" Text="Add Workout" OnClick="btnAddWorkout_Click" CssClass="btn btn-primary mt-3" />
        </div>
        <div class="d-flex justify-content-end mb-3">
            <asp:Button ID="Button2" runat="server" Text="Generate Weekly Workout Report" OnClick="btnGenerateReport_Click" CssClass="btn btn-secondary" />
        </div>
        <asp:GridView ID="gridWorkouts" runat="server" AutoGenerateColumns="False" Visible="false" CssClass="table table-striped table-bordered mt-3">
            <Columns>
                <asp:BoundField DataField="Exercise" HeaderText="Exercise" />
                <asp:BoundField DataField="Duration" HeaderText="Duration (minutes)" />
                <asp:BoundField DataField="Intensity" HeaderText="Intensity" />
            </Columns>
        </asp:GridView>
    </section>
</asp:Content>
