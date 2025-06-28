Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class Enroll
    Inherits System.Web.UI.Page

    Private ReadOnly SupabaseUrl As String = Login.SupabaseUrl
    Private ReadOnly SupabaseKey As String = Login.SupabaseKey

    Private isStudentUser As Boolean = False
    Private currentStudentId As Integer = -1

    Protected Async Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("UserId") Is Nothing Then
            Response.Redirect("Login.aspx")
            Return
        End If

        isStudentUser = Not CBool(Session("IsAdmin"))
        currentStudentId = Convert.ToInt32(Session("UserId"))

        If Not IsPostBack Then
            Await LoadStudents()
            Await LoadCourses()
            Await LoadEnrollments()
        End If
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("AdminDashboard.aspx")
    End Sub
    Private Async Function LoadStudents() As Task
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            Dim res = Await client.GetAsync($"{SupabaseUrl}/rest/v1/students?select=id,first_name,last_name")
            If res.IsSuccessStatusCode Then
                Dim json = Await res.Content.ReadAsStringAsync()
                Dim students = JsonConvert.DeserializeObject(Of List(Of Student))(json)

                ddlStudents.DataSource = students
                ddlStudents.DataTextField = "full_name"
                ddlStudents.DataValueField = "id"
                ddlStudents.DataBind()
                ddlStudents.Items.Insert(0, New ListItem("-- Select Student --", ""))

                If isStudentUser Then
                    ' Pre-select and disable for student
                    Dim selected = ddlStudents.Items.FindByValue(currentStudentId.ToString())
                    If selected IsNot Nothing Then
                        ddlStudents.ClearSelection()
                        selected.Selected = True
                        ddlStudents.Enabled = False
                    End If
                End If
            End If
        End Using
    End Function


    Private Async Function LoadCourses() As Task
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            Dim res = Await client.GetAsync($"{SupabaseUrl}/rest/v1/courses?select=course_id,course_name")
            If res.IsSuccessStatusCode Then
                Dim json = Await res.Content.ReadAsStringAsync()
                Dim courses = JsonConvert.DeserializeObject(Of List(Of Course))(json)
                ddlCourses.DataSource = courses
                ddlCourses.DataTextField = "course_name"
                ddlCourses.DataValueField = "course_id"
                ddlCourses.DataBind()
                ddlCourses.Items.Insert(0, New ListItem("-- Select Course --", ""))
            End If
        End Using
    End Function

    Protected Async Sub btnEnroll_Click(sender As Object, e As EventArgs)
        Dim studentId = ddlStudents.SelectedValue
        Dim courseId = ddlCourses.SelectedValue
        Dim enrollDate = txtEnrollmentDate.Text

        If String.IsNullOrWhiteSpace(studentId) OrElse String.IsNullOrWhiteSpace(courseId) OrElse String.IsNullOrWhiteSpace(enrollDate) Then
            lblMessage.CssClass = "text-danger"
            lblMessage.Text = "All fields are required."
            Return
        End If

        Dim enrollment = New With {
            .student_id = Convert.ToInt32(studentId),
            .course_id = Convert.ToInt32(courseId),
            .enrollment_date = enrollDate
        }


        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            ' Duplicate check with headers
            Dim checkUrl = $"{SupabaseUrl}/rest/v1/enrollments?student_id=eq.{studentId}&course_id=eq.{courseId}"
            Dim checkResponse = Await client.GetAsync(checkUrl)

            If checkResponse.IsSuccessStatusCode Then
                Dim existing = Await checkResponse.Content.ReadAsStringAsync()
                If Not String.IsNullOrWhiteSpace(existing) AndAlso existing <> "[]" Then
                    lblMessage.CssClass = "text-danger"
                    lblMessage.Text = "Student is already enrolled in this course."
                    Return
                End If
            End If
            Dim payload As String = JsonConvert.SerializeObject(enrollment)
            ' Continue with POST
            Dim content = New StringContent(JsonConvert.SerializeObject(enrollment), Encoding.UTF8, "application/json")
            Dim response = Await client.PostAsync($"{SupabaseUrl}/rest/v1/enrollments", content)
            Dim err = Await response.Content.ReadAsStringAsync()
            If response.IsSuccessStatusCode Then
                lblMessage.CssClass = "text-success"
                lblMessage.Text = "Enrollment successful!"
                Await LoadEnrollments()
            ElseIf response.StatusCode = HttpStatusCode.Conflict Then
                lblMessage.CssClass = "text-danger"
                lblMessage.Text = "Conflict error: Possibly duplicate or constraint violation."
            Else
                lblMessage.CssClass = "text-danger"
                lblMessage.Text = $"Enrollment failed: {response.ReasonPhrase}"
            End If
        End Using

    End Sub

    Private Async Function LoadEnrollments() As Task
        ' Join enrollments with students and courses
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            Dim role = Convert.ToBoolean(Session("IsAdmin"))
            Dim query As String
            Dim currentStudentId = Convert.ToInt32(Session("UserId"))
            If role = False Then
                query = $"{SupabaseUrl}/rest/v1/enrollments?student_id=eq.{currentStudentId}&select=enrollment_id,enrollment_date,students(first_name,last_name),courses(course_name)"
            Else
                query = $"{SupabaseUrl}/rest/v1/enrollments?select=enrollment_id,enrollment_date,students(first_name,last_name),courses(course_name)"
            End If

            'Dim query = $"{SupabaseUrl}/rest/v1/enrollments?select=enrollment_id,enrollment_date,students(first_name,last_name),courses(course_name)"
            Dim response = Await client.GetAsync(query)

            If response.IsSuccessStatusCode Then
                Dim json = Await response.Content.ReadAsStringAsync()
                Dim enrollmentsRaw = JsonConvert.DeserializeObject(Of List(Of EnrollmentCombined))(json)

                ' Project data for display
                Dim displayData = enrollmentsRaw.Select(Function(e) New With {
                    .enrollment_id = e.enrollment_id,
                    .student_name = $"{e.students.first_name} {e.students.last_name}",
                    .course_name = e.courses.course_name,
                    .enrollment_date = e.enrollment_date.ToString("yyyy-MM-dd")
                }).ToList()

                gvEnrollments.DataSource = displayData
                gvEnrollments.DataBind()
            End If
        End Using
    End Function

    Public Class Student
        Public Property id As Integer
        Public Property first_name As String
        Public Property last_name As String
        Public ReadOnly Property full_name As String
            Get
                Return $"{first_name} {last_name}"
            End Get
        End Property
    End Class

    Public Class Course
        Public Property course_id As Integer
        Public Property course_name As String
    End Class

    Public Class EnrollmentCombined
        Public Property enrollment_id As Integer
        Public Property enrollment_date As Date
        Public Property students As Student
        Public Property courses As Course
    End Class
End Class
