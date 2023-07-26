<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Sign_Up.aspx.cs" Inherits="EAD_CourseWork1.Sign_Up" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row justify-content-center mt-10">
            <div class="col-lg-6">
                <div class="card">
                    <div class="card-header">
                        <h5 class="card-title text-center">Registration</h5>
                    </div>
                    <div class="card-body">
                        <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="text-danger"></asp:Label>
                        <div class="form-group px-3 py-2">
                            <label for="txtName" class="px-1 py-2">Name:</label>
                            <asp:TextBox ID="txtName" runat="server" placeholder="Name" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group px-3 py-2">
                            <label for="txtAge" class="px-1 py-2">Age:</label>
                            <asp:TextBox ID="txtAge" runat="server" placeholder="Age" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group px-3 py-2">
                            <label for="ddlGender" class="px-1 py-2">Gender:</label>
                            <asp:DropDownList ID="ddlGender" runat="server" placeholder="Gender" CssClass="form-control">
                                <asp:ListItem Text="Male" Value="Male"></asp:ListItem>
                                <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                                <asp:ListItem Text="Non-Binary" Value="Non-Binary"></asp:ListItem>
                                <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group px-3 py-2">
                            <label for="txtHeight" class="px-1 py-2">Height:</label>
                            <asp:TextBox ID="txtHeight" runat="server" placeholder="Height" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group px-3 py-2">
                            <label for="txtUsername" class="px-1 py-2">Username:</label>
                            <asp:TextBox ID="txtUsername" runat="server" placeholder="Username" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group px-3 py-2">
                            <label for="txtPassword" class="px-1 py-2">Password:</label>
                            <asp:TextBox ID="txtPassword" runat="server" placeholder="Password" TextMode="Password" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group px-3 py-2">
                            <label for="txtWeight" class="px-1 py-2">Weight:</label>
                            <asp:TextBox ID="txtWeight" runat="server" placeholder="Weight" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group px-3 py-2">
                            <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnSignUp_Click" CssClass="btn btn-primary mt-3 mx-auto d-block" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
        <script type="text/javascript">
        function showErrorPopup(message) {
            alert(message); // You can replace this with a custom modal dialog if you prefer
        }
        </script>
</asp:Content>
