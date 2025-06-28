<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Enroll.aspx.vb" MasterPageFile="~/Site.master" Inherits="Student_Information_System.Enroll" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Student Enrollment
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">

        

        <!-- Enrollment Form -->
        <div class="section p-4 bg-white shadow-sm rounded mb-4">
            <!-- Title -->
            <h4 class="mb-4 fw-bold text-primary mb-3 text-center">Student Enrollment</h4>
            <div class="row g-3">
                <div class="col-md-6">
                    <asp:DropDownList ID="ddlStudents" runat="server" CssClass="form-select form-select-sm">
                        <asp:ListItem Text="-- Select Student --" Value="" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-6">
                    <asp:DropDownList ID="ddlCourses" runat="server" CssClass="form-select form-select-sm">
                        <asp:ListItem Text="-- Select Course --" Value="" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-6">
                    <asp:TextBox ID="txtEnrollmentDate" runat="server" TextMode="Date" CssClass="form-control form-control-sm" />
                </div>
                <div class="col-md-6 text-end">
                    <asp:Button ID="btnEnroll" runat="server" Text="Enroll" CssClass="btn btn-sm btn-success" OnClick="btnEnroll_Click" />
                </div>
                <div class="col-12">
                    <asp:Label ID="lblMessage" runat="server" CssClass="text-success fw-semibold small"></asp:Label>
                </div>
            </div>
        </div>

        <!-- Enrolled Courses Grid -->
        <div class="section p-3 bg-white shadow-sm rounded mb-4">
            <asp:GridView ID="gvEnrollments" runat="server" AutoGenerateColumns="False" CssClass="table table-sm  table-hover text-center gridview mb-0">
                <HeaderStyle CssClass="table-primary text-dark small" />
                <Columns>
                    <asp:BoundField DataField="enrollment_id" HeaderText="Enrollment ID" />
                    <asp:BoundField DataField="student_name" HeaderText="Student" />
                    <asp:BoundField DataField="course_name" HeaderText="Course" />
                    <asp:BoundField DataField="enrollment_date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                </Columns>
            </asp:GridView>
        </div>

        <!-- Back Button -->
        <div class="text-end mb-4">
            <asp:Button ID="btnBack" runat="server" Text="← Back to Dashboard" CssClass="btn btn-sm btn-outline-secondary" OnClick="btnBack_Click" />
        </div>

    </div>
</asp:Content>
