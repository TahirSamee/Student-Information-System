Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class GradeAssignment
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
        End If
    End Sub

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

    Protected Async Sub ddlCourse_SelectedIndexChanged(sender As Object, e As EventArgs)
        Await LoadAssignments()
        ddlAssignment.SelectedIndex = 0
    End Sub
    Protected Async Sub ddlAssignment_SelectedIndexChanged(sender As Object, e As EventArgs)
        Await LoadEnrolledStudents()
    End Sub


    Private Async Function LoadAssignments() As Task
        ddlAssignment.Items.Clear()

        If String.IsNullOrEmpty(ddlCourse.SelectedValue) Then Return

        Dim courseId = ddlCourse.SelectedValue
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            Dim res = Await client.GetAsync($"{SupabaseUrl}/rest/v1/assignments?course_id=eq.{courseId}")
            If res.IsSuccessStatusCode Then
                Dim json = Await res.Content.ReadAsStringAsync()
                Dim assignments = JsonConvert.DeserializeObject(Of List(Of Assignment))(json)

                ddlAssignment.DataSource = assignments
                ddlAssignment.DataTextField = "title"
                ddlAssignment.DataValueField = "assignment_id"
                ddlAssignment.DataBind()
                ddlAssignment.Items.Insert(0, New ListItem("-- Select Assignment --", ""))
            End If
        End Using
    End Function

    Private Async Function LoadEnrolledStudents() As Task
        If String.IsNullOrEmpty(ddlCourse.SelectedValue) OrElse String.IsNullOrEmpty(ddlAssignment.SelectedValue) Then Return

        Dim courseId = ddlCourse.SelectedValue
        Dim assignmentId = ddlAssignment.SelectedValue

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            'Dim url = $"{SupabaseUrl}/rest/v1/enrollments_with_grades?course_id=eq.{courseId}&or=(assignment_id.eq.{assignmentId},assignment_id.is.null)"
            Dim url = $"{SupabaseUrl}/rest/v1/enrollments_with_grades?and=(course_id.eq.{courseId},assignment_id.eq.{assignmentId})"

            Dim res = Await client.GetAsync(url)
            Dim errorContent = Await res.Content.ReadAsStringAsync()
            System.Diagnostics.Debug.WriteLine("Supabase ERROR: " & errorContent)

            If res.IsSuccessStatusCode Then
                Dim data = JsonConvert.DeserializeObject(Of List(Of EnrollmentWithGradeView))(errorContent)


                Dim gridData = data.
    GroupBy(Function(s) s.student_id).
    Select(Function(g)
               Dim latestGrade = g.
                   Where(Function(x) x.grade.HasValue).
                   OrderByDescending(Function(x) x.grade).FirstOrDefault()

               Dim fallback = g.First() ' fallback in case all grades are null

               Return New With {
                   .student_id = g.Key,
                   .student_name = $"{fallback.student_first_name} {fallback.student_last_name}",
                   .grade = If(latestGrade IsNot Nothing, latestGrade.grade.Value.ToString("0.0"), "0.0")
               }
           End Function).ToList()




                gvGrades.DataSource = gridData
                gvGrades.DataBind()
            End If

        End Using
    End Function


    Protected Async Sub btnSaveGrades_Click(sender As Object, e As EventArgs)
        lblMessage.Text = ""
        lblMessage.CssClass = ""

        If String.IsNullOrEmpty(ddlAssignment.SelectedValue) Then
            lblMessage.CssClass = "text-danger"
            lblMessage.Text = "Please select an assignment."
            Return
        End If

        If gvGrades.Rows.Count = 0 Then
            lblMessage.CssClass = "text-danger"
            lblMessage.Text = "No students found to grade."
            Return
        End If

        Dim assignmentId = Convert.ToInt32(ddlAssignment.SelectedValue)
        Dim gradesToSave As New List(Of Object)

        For Each row As GridViewRow In gvGrades.Rows
            ' Ensure DataKeys is valid
            If row.RowIndex >= gvGrades.DataKeys.Count Then Continue For

            Dim studentId = Convert.ToInt32(gvGrades.DataKeys(row.RowIndex).Value)
            Dim txtGrade = CType(row.FindControl("txtGrade"), TextBox)

            If Decimal.TryParse(txtGrade.Text, Nothing) Then
                Dim gradeValue = Decimal.Parse(txtGrade.Text)
                If gradeValue >= 0D AndAlso gradeValue <= 6D Then
                    gradesToSave.Add(New With {
                    .assignment_id = assignmentId,
                    .student_id = studentId,
                    .grade = gradeValue
                })
                End If
            End If
        Next

        If gradesToSave.Count = 0 Then
            lblMessage.CssClass = "text-danger"
            lblMessage.Text = "No valid grades entered. Grades must be between 1.0 and 6.0."
            Return
        End If

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")
            client.DefaultRequestHeaders.Add("Prefer", "resolution=merge-duplicates")

            ' ... inside your Using block
            Dim content = New StringContent(JsonConvert.SerializeObject(gradesToSave), Encoding.UTF8, "application/json")
            Dim res = Await client.PostAsync($"{SupabaseUrl}/rest/v1/assignment_grades?on_conflict=assignment_id,student_id", content)


            If res.IsSuccessStatusCode Then
                lblMessage.CssClass = "text-success"
                lblMessage.Text = "Grades saved successfully."
            Else
                lblMessage.CssClass = "text-danger"
                lblMessage.Text = "Failed to save grades."
            End If
        End Using
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("AdminDashboard.aspx")
    End Sub
    Public Class Course
        Public Property course_id As Integer
        Public Property course_name As String
    End Class

    Public Class Assignment
        Public Property assignment_id As Integer
        Public Property title As String
    End Class
    Public Class Student
        Public Property first_name As String
        Public Property last_name As String
    End Class
    Public Class EnrollmentWithGradeView
        Public Property student_id As Integer
        Public Property course_id As Integer
        Public Property student_first_name As String
        Public Property student_last_name As String
        Public Property assignment_id As Integer? ' ← make nullable
        Public Property grade As Decimal?
    End Class


End Class
