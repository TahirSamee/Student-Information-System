<%@ Page Title="Add Student" Async="true" Language="vb" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeBehind="AddStudent.aspx.vb" Inherits="Student_Information_System.AddStudent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Add Student
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-center mt-4">
        <div class="form-container section p-4 shadow-sm" style="max-width: 500px; width: 100%; background-color: #fff; border-radius: 8px;">
            
            <h4 class="fw-bold text-primary text-center mb-4">Add New Student</h4>

            <div class="mb-3">
                <label for="txtFirstName" class="form-label small">First Name</label>
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control form-control-sm" />
            </div>

            <div class="mb-3">
                <label for="txtLastName" class="form-label small">Last Name</label>
                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control form-control-sm" />
            </div>

            <div class="mb-3">
                <label for="txtEmail" class="form-label small">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control form-control-sm" TextMode="Email" />
            </div>

            <div class="mb-3">
                <label for="txtPassword" class="form-label small">Password</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control form-control-sm" TextMode="Password" />
            </div>

            <div class="mb-3">
                <label for="txtEnrollmentDate" class="form-label small">Enrollment Date</label>
                <asp:TextBox ID="txtEnrollmentDate" runat="server" CssClass="form-control form-control-sm" TextMode="Date" />
            </div>

            <div class="form-check mb-4">
                <asp:CheckBox ID="chkIsAdmin" runat="server" CssClass="form-check-input" />
                <label class="form-check-label small ms-2" for="chkIsAdmin">Is Admin</label>
            </div>

            <asp:Button ID="btnSubmit" runat="server" Text="Add Student" CssClass="btn btn-sm btn-success w-100 mb-2" OnClick="btnSubmit_Click" />
            <asp:Button ID="btnBack" runat="server" Text="← Back to Dashboard" CssClass="btn btn-sm btn-outline-secondary w-100" OnClick="btnBack_Click" />

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success mt-3 d-block text-center fw-semibold small" EnableViewState="false" />
        </div>
    </div>
</asp:Content>




