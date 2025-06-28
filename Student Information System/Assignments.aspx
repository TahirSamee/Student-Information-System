<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Assignments.aspx.vb" MasterPageFile="~/Site.master" Inherits="Student_Information_System.Assignments" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
   Manage Assignments
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <div class="container mt-4">

        

        <!-- Assignment Form -->
        <div class="section bg-white shadow-sm rounded p-4 mb-4">
            <!-- Heading -->
        <h4 class="text-primary fw-bold text-center mb-3">Create Assignment</h4>
            <div class="row g-3">
                <div class="col-md-4">
                    <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-select form-select-sm">
                        <asp:ListItem Text="-- Select Course --" Value="" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control form-control-sm" placeholder="Assignment Title" />
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="txtDueDate" runat="server" CssClass="form-control form-control-sm" TextMode="Date" />
                </div>
                <div class="col-md-12">
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control form-control-sm" placeholder="Description" TextMode="MultiLine" Rows="3" />
                </div>
                <div class="col-12 text-end mt-2">
                    <asp:Button ID="btnAddAssignment" runat="server" CssClass="btn btn-sm btn-success" Text="Create Assignment" OnClick="btnAddAssignment_Click" />
                    <asp:Label ID="lblMessage" runat="server" CssClass="ms-3 text-success fw-semibold small" />
                </div>
            </div>
        </div>

        <!-- Existing Assignments Grid -->
        <div class="section bg-white shadow-sm rounded p-3 mb-4">
            <h5 class="text-dark fw-bold mb-3">Existing Assignments</h5>
            <asp:GridView ID="gvAssignments" runat="server" AutoGenerateColumns="False" CssClass="table table-sm table-hover  text-center gridview mb-0">
                <HeaderStyle CssClass="table-primary text-dark small" />
                <Columns>
                    <asp:BoundField DataField="assignment_id" HeaderText="ID" />
                    <asp:BoundField DataField="course_name" HeaderText="Course" />
                    <asp:BoundField DataField="title" HeaderText="Title" />
                    <asp:BoundField DataField="due_date" HeaderText="Due Date" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="description" HeaderText="Description" />
                </Columns>
            </asp:GridView>
        </div>

        <!-- Back Button -->
        <div class="text-end mb-4">
            <asp:Button ID="btnBack" runat="server" Text="← Back to Dashboard" CssClass="btn btn-sm btn-outline-secondary" OnClick="btnBack_Click" />
        </div>

    </div>
</asp:Content>
