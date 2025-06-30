Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class Courses
    Inherits System.Web.UI.Page

    Private ReadOnly SupabaseUrl As String = Login.SupabaseUrl
    Private ReadOnly SupabaseKey As String = Login.SupabaseKey

    Protected Async Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            RegisterAsyncTask(New PageAsyncTask(AddressOf LoadCourses))
        End If
    End Sub

    Private Async Function LoadCourses() As Task
        Try
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

                Dim response = Await client.GetAsync($"{SupabaseUrl}/rest/v1/courses?select=*")

                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim courses = JsonConvert.DeserializeObject(Of List(Of Course))(json)
                    gvCourses.DataSource = courses
                    gvCourses.DataBind()
                Else
                    lblMessage.Text = "Failed to load courses"
                End If
            End Using
        Catch ex As Exception
            lblMessage.Text = "Error: " & ex.Message
        End Try
    End Function
    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("AdminDashboard.aspx")
    End Sub
    Protected Async Sub btnreset_Click(sender As Object, e As EventArgs)
        txtCourseName.Text = ""
        txtECTS.Text = ""
        txtHours.Text = ""
        txtFormat.Text = ""
        txtInstructor.Text = ""
    End Sub

    Protected Async Sub btnAddCourse_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtCourseName.Text) OrElse
           String.IsNullOrWhiteSpace(txtECTS.Text) OrElse
           String.IsNullOrWhiteSpace(txtHours.Text) OrElse
           String.IsNullOrWhiteSpace(txtInstructor.Text) OrElse
           String.IsNullOrWhiteSpace(txtFormat.Text) Then

            lblMessage.CssClass = "text-danger"
            lblMessage.Text = "All required fields must be filled."
            Return
        End If


        Dim newCourse = New With {
            .course_name = txtCourseName.Text,
            .ects = Integer.Parse(txtECTS.Text),
            .hours = Integer.Parse(txtHours.Text),
            .format = txtFormat.Text,
            .instructor = txtInstructor.Text
        }

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            Dim content = New StringContent(JsonConvert.SerializeObject(newCourse), Encoding.UTF8, "application/json")
            Dim response = Await client.PostAsync($"{SupabaseUrl}/rest/v1/courses", content)

            If response.IsSuccessStatusCode Then
                lblMessage.Text = "Course added successfully!"
                Await LoadCourses()
                txtCourseName.Text = ""
                txtECTS.Text = ""
                txtHours.Text = ""
                txtFormat.Text = ""
                txtInstructor.Text = ""
            Else
                lblMessage.Text = "Failed to add course"
            End If
        End Using
    End Sub

    Protected Sub gvCourses_RowEditing(sender As Object, e As GridViewEditEventArgs)
        gvCourses.EditIndex = e.NewEditIndex
        RegisterAsyncTask(New PageAsyncTask(AddressOf LoadCourses))
    End Sub

    Protected Sub gvCourses_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs)
        gvCourses.EditIndex = -1
        RegisterAsyncTask(New PageAsyncTask(AddressOf LoadCourses))
    End Sub

    Protected Sub gvCourses_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)
        RegisterAsyncTask(New PageAsyncTask(
            Async Function()
                Try
                    Dim row = gvCourses.Rows(e.RowIndex)
                    Dim id = gvCourses.DataKeys(e.RowIndex).Value.ToString()

                    Dim updatedCourse = New With {
                        .course_name = CType(row.FindControl("txtCourseNameEdit"), TextBox).Text,
                        .ects = Integer.Parse(CType(row.FindControl("txtECTSEdit"), TextBox).Text),
                        .hours = Integer.Parse(CType(row.FindControl("txtHoursEdit"), TextBox).Text),
                        .format = CType(row.FindControl("txtFormatEdit"), TextBox).Text,
                        .instructor = CType(row.FindControl("txtInstructorEdit"), TextBox).Text
                    }

                    Using client As New HttpClient()
                        client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")
                        client.DefaultRequestHeaders.Add("Prefer", "return=representation")

                        Dim content = New StringContent(JsonConvert.SerializeObject(updatedCourse), Encoding.UTF8, "application/json")
                        Dim patchMethod As New HttpMethod("PATCH")
                        Dim request = New HttpRequestMessage(patchMethod, $"{SupabaseUrl}/rest/v1/courses?course_id=eq.{id}") With {.Content = content}

                        Dim response = Await client.SendAsync(request)

                        If response.IsSuccessStatusCode Then
                            gvCourses.EditIndex = -1
                            Await LoadCourses()
                        Else
                            lblMessage.Text = "Update failed"
                        End If
                    End Using
                Catch ex As Exception
                    lblMessage.Text = "Error: " & ex.Message
                End Try
            End Function))
    End Sub
    Protected Sub gvCourses_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        RegisterAsyncTask(New PageAsyncTask(
        Async Function()
            Try
                Dim id = gvCourses.DataKeys(e.RowIndex).Value.ToString()

                Using client As New HttpClient()
                    client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

                    ' Step 1: Get all assignments for the course
                    Dim resAssignments = Await client.GetAsync($"{SupabaseUrl}/rest/v1/assignments?course_id=eq.{id}&select=assignment_id")
                    If resAssignments.IsSuccessStatusCode Then
                        Dim json = Await resAssignments.Content.ReadAsStringAsync()
                        Dim assignments = JsonConvert.DeserializeObject(Of List(Of AssignmentIdOnly))(json)

                        ' Step 2: Delete assignment grades for each assignment
                        For Each assignment In assignments
                            Await client.DeleteAsync($"{SupabaseUrl}/rest/v1/assignment_grades?assignment_id=eq.{assignment.assignment_id}")
                        Next
                    End If

                    ' Step 3: Delete assignments of the course
                    Await client.DeleteAsync($"{SupabaseUrl}/rest/v1/assignments?course_id=eq.{id}")

                    ' Step 4: Delete enrollments of the course
                    Await client.DeleteAsync($"{SupabaseUrl}/rest/v1/enrollments?course_id=eq.{id}")

                    ' Step 5: Delete the course
                    Dim response = Await client.DeleteAsync($"{SupabaseUrl}/rest/v1/courses?course_id=eq.{id}")

                    If response.IsSuccessStatusCode Then
                        Await LoadCourses()
                        lblMessage.Text = "Course and related data deleted."
                    Else
                        lblMessage.Text = "Course deletion failed."
                    End If
                End Using
            Catch ex As Exception
                lblMessage.Text = "Failed to delete. Maybe this is linked elsewhere."
                lblMessage.Visible = True
            End Try
        End Function))
    End Sub

    Private Class AssignmentIdOnly
        Public Property assignment_id As Integer
    End Class


End Class

Public Class Course
    Public Property course_id As Integer
    Public Property course_name As String
    Public Property ects As Integer
    Public Property hours As Integer
    Public Property format As String
    Public Property instructor As String
End Class
