<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WeightTracking.aspx.cs" Inherits="EAD_CourseWork1.WeightTracking" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section id="main-content">
        <h2>Weight Tracking</h2>
        <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="text-danger"></asp:Label>
        <div class="form-group row">
            <label for="txtWeight" class="col-sm-2 col-form-label">Weight:</label>
            <div class="col-sm-4">
                <div class="input-group">
                    <asp:TextBox ID="txtWeight" runat="server" placeholder="Enter your weight" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="form-group mt-3">
            <div class="row">
                <div class="col-sm-6">
                    <asp:Button ID="btnAddWeight" runat="server" Text="Add Weight" OnClick="btnAddWeight_Click" CssClass="btn btn-primary" />
                </div>
                <div class="col-sm-6 d-flex justify-content-end">
                    <asp:Button ID="btnPredictFitness" runat="server" Text="Predict Fitness Status" OnClick="btnPredictFitness_Click" CssClass="btn btn-secondary ml-2" />
                </div>
            </div>
        </div>
        <div class="form-group mt-3">
            <div class="row">
                <div class="col-sm-6">
                    Next Weight Prediction: 
                    <strong>
                        <asp:Label ID="lblWeightPrediction" runat="server"></asp:Label></strong>
                </div>
                <div class="col-sm-6 d-flex justify-content-end">

                    <strong>
                        <asp:Label ID="lblFitnessStatus" runat="server" Visible="false" CssClass="ml-2"></asp:Label></strong>
                </div>
            </div>
        </div>
        <div class="table-responsive mt-3">
            <asp:GridView ID="gridWeights" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered">
                <Columns>
                    <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="Weight" HeaderText="Weight" />
                </Columns>
            </asp:GridView>
        </div>
    </section>
</asp:Content>
