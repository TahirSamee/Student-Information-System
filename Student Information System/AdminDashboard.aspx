<%@ Page Title="Admin Dashboard" Async="true" Language="vb" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeBehind="AdminDashboard.aspx.vb" Inherits="Student_Information_System.AdminDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Admin Dashboard
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-3 px-2">
        <div class="d-flex justify-content-between align-items-center mb-3">
            <h4 class="fw-bold mb-0 text-primary">Student Management</h4>
            <asp:Button ID="Button1" runat="server" Text="New Student" 
                CssClass="btn btn-sm btn-success" OnClick="btnAddStudent_Click" />
        </div>

        <div class="alert alert-danger py-2 px-3" id="Div1" runat="server" visible="false"></div>

        <div class="section mb-4 p-2">
            <asp:GridView ID="gvStudents" runat="server" AutoGenerateColumns="False" 
                CssClass="table table-sm  table-hover text-center gridview mb-0"
                OnRowEditing="gvStudents_RowEditing"
                OnRowUpdating="gvStudents_RowUpdating" 
                OnRowCancelingEdit="gvStudents_RowCancelingEdit"
                OnRowDeleting="gvStudents_RowDeleting" 
                DataKeyNames="id">
                <HeaderStyle CssClass="table-primary text-dark small" />
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="ID" ReadOnly="True" ItemStyle-CssClass="fw-bold small" />

                    <asp:TemplateField HeaderText="First Name">
                        <ItemTemplate><%-- Item View --%>
                            <asp:Label runat="server" Text='<%# Eval("first_name") %>' CssClass="small"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate><%-- Edit View --%>
                            <asp:TextBox ID="txtFirstName" runat="server" Text='<%# Bind("first_name") %>' CssClass="form-control form-control-sm"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Last Name">
                        <ItemTemplate><asp:Label runat="server" Text='<%# Eval("last_name") %>' CssClass="small"></asp:Label></ItemTemplate>
                        <EditItemTemplate><asp:TextBox ID="txtLastName" runat="server" Text='<%# Bind("last_name") %>' CssClass="form-control form-control-sm"></asp:TextBox></EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Email">
                        <ItemTemplate><asp:Label runat="server" Text='<%# Eval("email") %>' CssClass="small"></asp:Label></ItemTemplate>
                        <EditItemTemplate><asp:TextBox ID="txtEmail" runat="server" Text='<%# Bind("email") %>' TextMode="Email" CssClass="form-control form-control-sm"></asp:TextBox></EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Enroll Date">
                        <ItemTemplate><asp:Label runat="server" Text='<%# Eval("enrollment_date", "{0:d}") %>' CssClass="small"></asp:Label></ItemTemplate>
                        <EditItemTemplate><asp:TextBox ID="txtEnrollmentDate" runat="server" Text='<%# Eval("enrollment_date", "{0:yyyy-MM-dd}") %>' TextMode="Date" CssClass="form-control form-control-sm"></asp:TextBox></EditItemTemplate>
                    </asp:TemplateField>

                   <asp:TemplateField HeaderText="Role">
    <ItemTemplate>
        <%# If(CBool(Eval("is_admin")),
                                                                "<span>Admin</span>",
                                                                "<span>Student</span>") %>
    </ItemTemplate>
    <EditItemTemplate>
        <asp:CheckBox ID="chkIsAdmin" runat="server" Checked='<%# Bind("is_admin") %>' CssClass="form-check-input" />
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

        <!-- Chart Section -->
        <div class="section p-3 mb-3 row">
    <div class="col-md-6">
        <h5 class="text-primary mb-3">Distribution of GPA across Students</h5>
        <div style="height:300px;">
            <canvas id="gpaDistributionChart"></canvas>
        </div>
    </div>
    <div class="col-md-6">
        <h5 class="text-primary mb-3 text-center">Students per Course</h5>
        <div class="d-flex justify-content-center">
            <div style="width: 320px; height: 320px;">
                <canvas id="studentsPerCourseChart"></canvas>
            </div>
        </div>
    </div>
</div>
<%--<div class="section p-3 mb-3 row">
    <div class="col-md-6">
        <h5 class="text-primary mb-3">Students per Course</h5>
        <div style="height:300px;">
            <canvas id="studentsPerCourseChart"></canvas>
        </div>
    </div>
    <div class="col-md-6">
       <h5 class="text-primary mb-3 text-center">Grade Distribution</h5>
    <div class="d-flex justify-content-center">
        <div style="width: 320px; height: 320px;">
            <canvas id="gradeDistributionChart"></canvas>
        </div>
    </div>
    </div>
</div>--%>




    </div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <%--<script>
        var ctx = document.getElementById('studentsPerCourseChart').getContext('2d');
        const backgroundColors = [
            '#66c2a5', '#9e0142','#abdda4', '#d53e4f', '#f46d43', '#fdae61', '#fee08b', '#e6f598','#3288bd', '#5e4fa2'
        ];
        var chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: <%= GetCourseNames() %>,
                datasets: [{
                    label: 'Number of Students',
                    data: <%= GetStudentCounts() %>,
                    backgroundColor: backgroundColors,
                    borderColor: backgroundColors.map(c => c.replace("0.6", "1")),
                    borderWidth: 1
                }]
            },
            options: {
                plugins: {
                    legend: { display: false },
                    tooltip: { enabled: true }
                },
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });
        var gradeCtx = document.getElementById('gradeDistributionChart').getContext('2d');
        var gradeChart = new Chart(gradeCtx, {
            type: 'pie',
            data: {
                labels: <%= GetGradeLabels() %>,
            datasets: [{
                label: 'Grade Distribution',
                data: <%= GetGradeCounts() %>,
                backgroundColor: [
                    '#3288bd', '#abdda4', '#d53e4f', '#f46d43', '#fdae61','#66c2a5','#9e0142' , '#fee08b', '#e6f598', '#5e4fa2'
                ],
                borderColor: '#fff',
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                legend: {
                    position: 'bottom'
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            let label = context.label || '';
                            let value = context.parsed || 0;
                            return `${label}: ${value} students`;
                        }
                    }
                }
            }
        }
    });

    </script>--%>
    <script>
        // GPA Distribution Chart (Histogram)
        var gpaCtx = document.getElementById('gpaDistributionChart').getContext('2d');
        var gpaChart = new Chart(gpaCtx, {
            type: 'bar',
            data: {
                labels: <%= GetGradeLabels() %>, // Example: ['2.0', '2.5', '3.0', '3.5', '4.0']
            datasets: [{
                label: 'Number of Students',
                data: <%= GetGradeCounts() %>, // Example: [3, 10, 15, 5, 2]
                backgroundColor: '#9e0142', // Dark red
                borderColor: '#9e0142',
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.parsed.y} students`;
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Number of Students'
                    }
                },
                x: {
                    title: {
                        display: true,
                        text: 'GPA'
                    }
                }
            }
        }
    });

    // Students per Course Pie Chart
    var courseCtx = document.getElementById('studentsPerCourseChart').getContext('2d');
    var courseChart = new Chart(courseCtx, {
        type: 'pie',
        data: {
            labels: <%= GetCourseNames() %>,
            datasets: [{
                label: 'Students per Course',
                data: <%= GetStudentCounts() %>,
                backgroundColor: [
                    '#66c2a5', '#9e0142', '#abdda4', '#d53e4f', '#f46d43',
                    '#fdae61', '#fee08b', '#e6f598', '#3288bd', '#5e4fa2'
                ],
                borderColor: '#fff',
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                legend: {
                    position: 'bottom'
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.label}: ${context.parsed} students`;
                        }
                    }
                }
            }
        }
    });
    </script>
</asp:Content>





