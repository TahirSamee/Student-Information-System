<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StudentDashboard.aspx.vb" MasterPageFile="~/Site.master" Inherits="Student_Information_System.StudentDashboard" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Student Dashboard
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-3 px-2">
        <div class="section p-3 mb-4 shadow-sm bg-white rounded">
            <h4 class="fw-bold text-primary">Welcome, <asp:Label ID="lblStudentName" runat="server" /></h4>
            <p class="small text-muted mb-3">Your enrolled courses are listed below:</p>

            <asp:GridView ID="gvEnrollments" runat="server" AutoGenerateColumns="False"
                CssClass="table table-sm table-hover text-center gridview mb-0">
                <HeaderStyle CssClass="table-primary text-dark small" />
                <Columns>
                    <asp:BoundField DataField="course_name" HeaderText="Course" />
                    <asp:BoundField DataField="format" HeaderText="Format" />
                    <asp:BoundField DataField="instructor" HeaderText="Instructor" />
                    <asp:BoundField DataField="enrollment_date" HeaderText="Enrollment Date" DataFormatString="{0:yyyy-MM-dd}" />
                </Columns>
            </asp:GridView>
        </div>

        <div class="section p-3 mb-4 shadow-sm bg-white rounded">
            <h5 class="fw-bold text-primary">Assignments & Grades</h5>

            <asp:GridView ID="gvAssignments" runat="server" AutoGenerateColumns="False"
                CssClass="table table-sm table-hover text-center gridview mb-0">
                <HeaderStyle CssClass="table-primary text-dark small" />
                <Columns>
                    <asp:TemplateField HeaderText="Assignment Title">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkView" runat="server"
                                Text='<%# Eval("title") %>'
                                CssClass="text-decoration-none"
                                OnClientClick='<%# "showDescriptionModal(" & Eval("assignment_id") & "); return false;" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="course_name" HeaderText="Course" />
                    <asp:BoundField DataField="due_date" HeaderText="Due Date" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="grade_display" HeaderText="Grade" 
                        DataFormatString="{0:F1}" NullDisplayText="Not yet graded" />
                </Columns>
            </asp:GridView>

            <div class="mt-3 small">
                <strong>GPA:</strong>
                <asp:Label ID="lblGPA" runat="server" CssClass="fw-bold text-success ms-1"></asp:Label>
            </div>
        </div>
    </div>

    <!-- 🧾 Assignment Description Modal -->
    <div class="modal fade" id="assignmentModal" tabindex="-1" aria-labelledby="assignmentModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="assignmentModalLabel">Assignment Description</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div id="assignmentDescriptionBody">Loading...</div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-sm btn-secondary" data-bs-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <!-- ✅ JavaScript to Load and Show Modal -->
    <script>
        function showDescriptionModal(assignmentId) {
            const modal = new bootstrap.Modal(document.getElementById("assignmentModal"));
            const body = document.getElementById("assignmentDescriptionBody");

            body.innerText = "Loading...";

            fetch(`/AssignmentDescription.ashx?id=${assignmentId}`)
                .then(res => {
                    if (!res.ok) throw new Error("Failed to fetch description.");
                    return res.text();
                })
                .then(text => {
                    body.innerText = text || "No description available.";
                    modal.show();
                })
                .catch(err => {
                    body.innerText = "Unable to load assignment description.";
                    console.error(err);
                    modal.show();
                });
        }
    </script>
</asp:Content>
