<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Sign_In.aspx.cs" Inherits="EAD_CourseWork1.Sign_In" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section id="main-content">
        <div class="container d-flex justify-content-center">
            <div class="card">
                <div class="card-header">
                    <h5 class="card-title">Sign In</h5>
                </div>
                <div class="card-body">
                    <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="text-danger"></asp:Label>
                    <div class="form-group px-3 py-2">
                        <label for="txtUsername" class="col-sm-5 col-form-label h5">Username:</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="txtUsername" runat="server" placeholder="Username" CssClass="form-control form-control-lg"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group px-3 py-2">
                        <label for="txtPassword" class="col-sm-5 col-form-label h5">Password:</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Password" CssClass="form-control form-control-lg"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group px-3 py-2">
                        <div class="offset-sm-3 col-sm-9">
                            <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" CssClass="btn btn-primary" />
                            <asp:Button ID="btnSignUp" runat="server" Text="Sign Up" OnClick="btnSignUp_Click" CssClass="btn btn-link" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
