Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class Assignments
    Inherits System.Web.UI.Page

    Private ReadOnly SupabaseUrl As String = Login.SupabaseUrl
    Private ReadOnly SupabaseKey As String = Login.SupabaseKey

    Protected Async Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("IsAdmin") Is Nothing OrElse Not CBool(Session("IsAdmin")) Then
            Response.Redirect("Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            Await LoadCourses()
            Await LoadAssignments()
        End If
    End Sub

    ''' <summary>
    ''' Loads all courses for the course dropdown
    ''' </summary>
    Private Async Function LoadCourses() As Task
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            Dim res = Await client.GetAsync($"{SupabaseUrl}/rest/v1/courses?select=course_id,course_name")
            If res.IsSuccessStatusCode Then
                Dim json = Await res.Content.ReadAsStringAsync()
                Dim courses = JsonConvert.DeserializeObject(Of List(Of Course))(json)

                ddlCourse.DataSource = courses
                ddlCourse.DataTextField = "course_name"
                ddlCourse.DataValueField = "course_id"
                ddlCourse.DataBind()
                ddlCourse.Items.Insert(0, New ListItem("-- Select Course --", ""))
            End If
        End Using
    End Function

    ''' <summary>
    ''' Handles assignment creation and saving to Supabase
    ''' </summary>
    Protected Async Sub btnAddAssignment_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(ddlCourse.SelectedValue) OrElse String.IsNullOrWhiteSpace(txtTitle.Text) OrElse String.IsNullOrWhiteSpace(txtDueDate.Text) Then
            lblMessage.CssClass = "text-danger"
            lblMessage.Text = "Course, Title, and Due Date are required."
            Return
        End If

        Dim assignment = New With {
            .course_id = Convert.ToInt32(ddlCourse.SelectedValue),
            .title = txtTitle.Text.Trim(),
            .description = txtDescription.Text.Trim(),
            .due_date = txtDueDate.Text
        }

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            Dim content = New StringContent(JsonConvert.SerializeObject(assignment), Encoding.UTF8, "application/json")
            Dim res = Await client.PostAsync($"{SupabaseUrl}/rest/v1/assignments", content)

            If res.IsSuccessStatusCode Then
                lblMessage.CssClass = "text-success"
                lblMessage.Text = "Assignment created successfully!"
                txtTitle.Text = ""
                txtDescription.Text = ""
                txtDueDate.Text = ""
                Await LoadAssignments()
            Else
                lblMessage.CssClass = "text-danger"
                lblMessage.Text = "Error creating assignment."
            End If
        End Using
    End Sub

    ''' <summary>
    ''' Loads assignments with course info
    ''' </summary>
    Private Async Function LoadAssignments() As Task
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            Dim query = $"{SupabaseUrl}/rest/v1/assignments?select=assignment_id,title,due_date,description,courses(course_name)"
            Dim res = Await client.GetAsync(query)

            If res.IsSuccessStatusCode Then
                Dim json = Await res.Content.ReadAsStringAsync()
                Dim raw = JsonConvert.DeserializeObject(Of List(Of AssignmentCombined))(json)

                Dim formatted = raw.Select(Function(a) New With {
                    .assignment_id = a.assignment_id,
                    .course_name = a.courses.course_name,
                    .title = a.title,
                    .due_date = a.due_date,
                    .description = a.description
                }).ToList()

                gvAssignments.DataSource = formatted
                gvAssignments.DataBind()
            End If
        End Using
    End Function
    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("AdminDashboard.aspx")
    End Sub
    Public Class Course
        Public Property course_id As Integer
        Public Property course_name As String
    End Class

    Public Class AssignmentCombined
        Public Property assignment_id As Integer
        Public Property title As String
        Public Property description As String
        Public Property due_date As Date
        Public Property courses As Course
    End Class
End Class
