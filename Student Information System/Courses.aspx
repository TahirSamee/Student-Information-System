<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Courses.aspx.vb" MasterPageFile="~/Site.master"  Inherits="Student_Information_System.Courses" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
   Course Management
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">

        <!-- Add Course Form -->
        <div class="form-section section p-4 shadow-sm rounded mb-4 bg-white">
            <h4 class="fw-bold text-primary mb-3 text-center">Course Management</h4> 
            
            <div class="row g-3">
                <div class="col-md-6">
                    <asp:TextBox ID="txtCourseName" runat="server" CssClass="form-control form-control-sm" placeholder="Course Name" />
                </div>
                <div class="col-md-2">
                    <asp:TextBox ID="txtECTS" runat="server" CssClass="form-control form-control-sm" placeholder="ECTS" TextMode="Number" />
                </div>
                <div class="col-md-2">
                    <asp:TextBox ID="txtHours" runat="server" CssClass="form-control form-control-sm" placeholder="Hours" TextMode="Number" />
                </div>
                <div class="col-md-6">
                    <asp:TextBox ID="txtFormat" runat="server" CssClass="form-control form-control-sm" placeholder="Format (Online / Campus / Blended)" />
                </div>
                <div class="col-md-6">
                    <asp:TextBox ID="txtInstructor" runat="server" CssClass="form-control form-control-sm" placeholder="Instructor Name" />
                </div>
                <div class="col-12 text-end mt-2">
                    <asp:Button ID="btnAddCourse" runat="server" CssClass="btn btn-sm btn-success" Text="Add Course" OnClick="btnAddCourse_Click" />
                    <asp:Button ID="btnAddreset" runat="server" CssClass="btn btn-sm btn-warning" Text="Reset" OnClick="btnreset_Click" />
                   
                </div>
                 <asp:Label ID="lblMessage" runat="server" CssClass="ms-3 text-success small fw-semibold"></asp:Label>
            </div>
        </div>

        <!-- Course List Grid -->
        <div class="section p-3 shadow-sm rounded bg-white mb-4">
            <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False"
               CssClass="table table-sm table-hover  text-center gridview"
               DataKeyNames="course_id"
               OnRowEditing="gvCourses_RowEditing"
               OnRowCancelingEdit="gvCourses_RowCancelingEdit"
               OnRowUpdating="gvCourses_RowUpdating"
               OnRowDeleting="gvCourses_RowDeleting">
               
                <HeaderStyle CssClass="table-primary text-dark small" />
                <Columns>
                    <asp:BoundField DataField="course_id" HeaderText="ID" ReadOnly="True" />

                    <asp:TemplateField HeaderText="Course Name">
                        <ItemTemplate><%# Eval("course_name") %></ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCourseNameEdit" runat="server" Text='<%# Bind("course_name") %>' CssClass="form-control form-control-sm" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="ECTS">
                        <ItemTemplate><%# Eval("ects") %></ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtECTSEdit" runat="server" Text='<%# Bind("ects") %>' CssClass="form-control form-control-sm" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Hours">
                        <ItemTemplate><%# Eval("hours") %></ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtHoursEdit" runat="server" Text='<%# Bind("hours") %>' CssClass="form-control form-control-sm" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Format">
                        <ItemTemplate><%# Eval("format") %></ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtFormatEdit" runat="server" Text='<%# Bind("format") %>' CssClass="form-control form-control-sm" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Instructor">
                        <ItemTemplate><%# Eval("instructor") %></ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtInstructorEdit" runat="server" Text='<%# Bind("instructor") %>' CssClass="form-control form-control-sm" />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    
                    <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CssClass="btn btn-sm btn-primary me-1">Edit</asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-sm btn-danger">Delete</asp:LinkButton>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" CssClass="btn btn-sm btn-success me-1">Save</asp:LinkButton>
                            <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel" CssClass="btn btn-sm btn-secondary">Cancel</asp:LinkButton>
                        </EditItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
        </div>

        <!-- Back Button -->
        <div class="text-end mb-4">
            <asp:Button ID="btnBack" runat="server" Text="← Back to Dashboard" CssClass="btn btn-sm btn-outline-secondary" OnClick="btnBack_Click" />
        </div>

    </div>
</asp:Content>



