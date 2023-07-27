<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EAD_CourseWork1._Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main class="container">
        <section class="row" aria-labelledby="fitnessTrackerTitle">
            <div class="col-12 text-center">
                <h1 id="fitnessTrackerTitle">Welcome to Fitness Tracker</h1>
                <p class="lead">Track your fitness progress and achieve your health goals.</p>
            </div>
        </section>

        <section class="row">
            <div class="col-md-4">
                <div class="card">
                    <div class="card-body">
                        <h2 id="weightTrackingTitle" class="card-title">Weight Tracking</h2>
                        <p class="card-text">Keep track of your weight over time and monitor your progress towards your target weight.</p>
                        <a class="btn btn-primary" href="WeightTracking.aspx">Track Weight &raquo;</a>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card">
                    <div class="card-body">
                        <h2 id="workoutTrackingTitle" class="card-title">Workout Tracking</h2>
                        <p class="card-text">Log your workouts, set goals, and track your fitness activities to stay motivated and improve your performance.</p>
                        <a class="btn btn-primary" href="WorkoutTracking.aspx">Track Workouts &raquo;</a>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card">
                    <div class="card-body">
                        <h2 id="cheatMealTrackingTitle" class="card-title">Cheat Meal Tracking</h2>
                        <p class="card-text">Keep a record of your cheat meals, indulge responsibly, and maintain a balanced approach to your diet.</p>
                        <a class="btn btn-primary" href="CheatMealTracking.aspx">Track Cheat Meals &raquo;</a>
                    </div>
                </div>
            </div>
        </section>
    </main>
</asp:Content>
