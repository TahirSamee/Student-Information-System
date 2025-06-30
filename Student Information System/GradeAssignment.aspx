<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GradeAssignment.aspx.vb" MasterPageFile="~/Site.master" Inherits="Student_Information_System.GradeAssignment" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Grade Assignments
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">

        

        <!-- Filters -->
        <div class="section bg-white shadow-sm rounded p-4 mb-4">
            <!-- Page Title -->
        <h4 class="text-primary fw-bold text-center mb-4">Grade Assignments</h4>
            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label fw-semibold">Select Course</label>
                    <asp:DropDownList ID="ddlCourse" runat="server" AutoPostBack="True" CssClass="form-select form-select-sm" OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged" />
                </div>
                <div class="col-md-6">
                    <label class="form-label fw-semibold">Select Assignment</label>
                    <asp:DropDownList ID="ddlAssignment" runat="server" CssClass="form-select form-select-sm" AutoPostBack="True" OnSelectedIndexChanged="ddlAssignment_SelectedIndexChanged" />

                </div>
            </div>
        </div>

        <!-- Grades Table -->
        <div class="section bg-white shadow-sm rounded p-3 mb-4">
            <h5 class="fw-bold text-dark mb-3">Enter Grades</h5>
            <asp:GridView ID="gvGrades" runat="server" AutoGenerateColumns="False" DataKeyNames="student_id"
                          CssClass="table table-sm  table-hover text-center gridview mb-0">
                <HeaderStyle CssClass="table-primary text-dark small" />
                <Columns>
                    <asp:BoundField DataField="student_id" HeaderText="Student ID" ReadOnly="True" />
                    <asp:BoundField DataField="student_name" HeaderText="Student Name" ReadOnly="True" />
                    <asp:TemplateField HeaderText="Grade (1.0 - 6.0)">
                        <ItemTemplate>
                            <asp:TextBox ID="txtGrade" runat="server" CssClass="form-control form-control-sm text-center" Text='<%# Eval("grade") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <!-- Buttons -->
        <div class="text-end mb-4">
            <asp:Button ID="btnSaveGrades" runat="server" Text="Save Grades" CssClass="btn btn-sm btn-success me-2" OnClick="btnSaveGrades_Click" />
            <asp:Button ID="btnBack" runat="server" Text="← Back to Dashboard" CssClass="btn btn-sm btn-outline-secondary" OnClick="btnBack_Click" />
            <asp:Label ID="lblMessage" runat="server" CssClass="text-success fw-semibold d-block mt-2 text-center small" />
        </div>

    </div>
</asp:Content>
