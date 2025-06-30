Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class AdminDashboard
    Inherits System.Web.UI.Page

    Public Function GetGradeLabels() As String
        Return "[" & String.Join(",", GradeStats.Select(Function(g) $"'{g.grade}'")) & "]"
    End Function

    Public Function GetGradeCounts() As String
        Return "[" & String.Join(",", GradeStats.Select(Function(g) g.count.ToString())) & "]"
    End Function

    Private Shared GradeStats As List(Of GradeCount)


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Session("IsAdmin") Is Nothing OrElse Not Boolean.TryParse(Session("IsAdmin").ToString(), Nothing) OrElse Not CBool(Session("IsAdmin")) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            RegisterAsyncTask(New PageAsyncTask(AddressOf LoadStudents))
            RegisterAsyncTask(New PageAsyncTask(AddressOf LoadChartStats))
        End If
    End Sub
    Private Async Function LoadChartStats() As Task
        Try
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Add("apikey", Login.SupabaseKey)
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Login.SupabaseKey}")

                ' Supabase join query using embedded syntax
                Dim url = $"{Login.SupabaseUrl}/rest/v1/enrollments?select=course_id,courses(course_name)&limit=10000"
                Dim response = Await client.GetAsync(url)

                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim enrollments = JsonConvert.DeserializeObject(Of List(Of EnrollmentWithCourse))(json)

                    ' Group by course and count students
                    CourseStats = enrollments.GroupBy(Function(e) e.courses.course_name).
                    Select(Function(g) New CourseEnrollmentStat With {
                        .course_name = g.Key,
                        .student_count = g.Count()
                    }).ToList()
                Else
                    CourseStats = New List(Of CourseEnrollmentStat)()
                End If

                ' Grade distribution from assignment_grades table
                Dim gradeResponse = Await client.GetAsync($"{Login.SupabaseUrl}/rest/v1/assignment_grades?select=grade&limit=10000")

                If gradeResponse.IsSuccessStatusCode Then
                    Dim gradeJson = Await gradeResponse.Content.ReadAsStringAsync()
                    Dim gradeList = JsonConvert.DeserializeObject(Of List(Of AssignmentGrade))(gradeJson)


                    GradeStats = gradeList.
                        Where(Function(g) g.grade.HasValue).
                        GroupBy(Function(g) g.grade.Value).
                        Select(Function(g) New GradeCount With {
                            .grade = g.Key.ToString("0.0"),
                            .count = g.Count()
                        }).OrderByDescending(Function(g) g.count).ToList()
                Else
                    GradeStats = New List(Of GradeCount)()
                End If


            End Using ' ✅ This was missing
        Catch ex As Exception
            CourseStats = New List(Of CourseEnrollmentStat)()
        End Try
    End Function
    '    Private Async Function LoadChartStats() As Task
    '        Try
    '            Using client As New HttpClient()
    '                client.DefaultRequestHeaders.Add("apikey", Login.SupabaseKey)
    '                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Login.SupabaseKey}")

    '                ' Supabase join query using embedded syntax
    '                Dim url = $"{Login.SupabaseUrl}/rest/v1/enrollments?select=course_id,courses(course_name)&limit=10000"
    '                Dim response = Await client.GetAsync(url)

    '                If response.IsSuccessStatusCode Then
    '                    Dim json = Await response.Content.ReadAsStringAsync()
    '                    Dim enrollments = JsonConvert.DeserializeObject(Of List(Of EnrollmentWithCourse))(json)

    '                    ' Group by course and count students
    '                    CourseStats = enrollments.GroupBy(Function(e) e.courses.course_name).
    '                    Select(Function(g) New CourseEnrollmentStat With {
    '                        .course_name = g.Key,
    '                        .student_count = g.Count()
    '                    }).ToList()
    '                Else
    '                    CourseStats = New List(Of CourseEnrollmentStat)()
    '                End If

    '                ' Grade distribution from assignment_grades table
    '                Dim gradeResponse = Await client.GetAsync($"{Login.SupabaseUrl}/rest/v1/assignment_grades?select=(grade,student_id)&limit=10000")

    '                If gradeResponse.IsSuccessStatusCode Then
    '                    Dim gradeJson = Await gradeResponse.Content.ReadAsStringAsync()
    '                    Dim gradeList = JsonConvert.DeserializeObject(Of List(Of AssignmentGrade))(gradeJson)

    '                    ' Step 1: Get average GPA per student
    '                    Dim studentAverages = gradeList.
    '    Where(Function(g) g.grade.HasValue).
    '    GroupBy(Function(g) g.student_id).
    '    Select(Function(g) New With {
    '        .student_id = g.Key,
    '        .average_gpa = g.Average(Function(x) x.grade.Value)
    '    }).ToList()

    '                    ' Step 2: Group student averages by rounded GPA bucket (e.g., 2.0, 2.5, etc.)
    '                    Dim groupedByBucket = studentAverages.
    '    GroupBy(Function(s) Math.Floor(s.average_gpa * 2) / 2).
    '    ToDictionary(Function(g) g.Key, Function(g) g.Count())

    '                    ' Step 3: Prepare final bucket list from 1.0 to 6.0
    '                    Dim buckets = Enumerable.Range(2, 9).Select(Function(x) x / 2.0).ToList() ' 1.0 to 6.0 in steps of 0.5

    '                    ' Step 4: Map buckets to actual student counts (or 0 if not present)
    '                    GradeStats = buckets.Select(Function(bucket) New GradeCount With {
    '    .grade = bucket.ToString("0.0"),
    '    .count = If(groupedByBucket.ContainsKey(bucket), groupedByBucket(bucket), 0)
    '}).ToList()

    '                Else
    '                    GradeStats = New List(Of GradeCount)()
    '                End If


    '            End Using ' ✅ This was missing
    '        Catch ex As Exception
    '            CourseStats = New List(Of CourseEnrollmentStat)()
    '        End Try
    '    End Function

    Public Class AssignmentGrade
        Public Property grade As Decimal?
        Public Property student_id As String
    End Class

    Public Class GradeCount
        Public Property grade As String
        Public Property count As Integer
    End Class


    Public Class EnrollmentWithCourse
        Public Property course_id As Integer
        Public Property courses As Course
    End Class

    Public Class Course
        Public Property course_name As String
    End Class

    Public Class CourseEnrollmentStat
        Public Property course_name As String
        Public Property student_count As Integer
    End Class

    ' Returns JavaScript-safe course names array
    Public Function GetCourseNames() As String
        Dim names As New List(Of String)()
        For Each item In CourseStats
            names.Add($"'{item.course_name.Replace("'", "\'")}'")
        Next
        Return "[" & String.Join(",", names) & "]"
    End Function

    ' Returns student count per course array
    Public Function GetStudentCounts() As String
        Dim counts As New List(Of String)()
        For Each item In CourseStats
            counts.Add(item.student_count.ToString())
        Next
        Return "[" & String.Join(",", counts) & "]"
    End Function

    ' Holds course stats retrieved from Supabase
    Private Shared CourseStats As List(Of CourseEnrollmentStat)

    Protected Async Sub btnRefresh_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Re-bind the data to refresh the GridView
        Await LoadStudents()
    End Sub

    Private Async Function LoadStudents() As Task
        Try
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Add("apikey", Login.SupabaseKey)
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer { Login.SupabaseKey}")

                Dim response = Await client.GetAsync($"{ Login.SupabaseUrl}/rest/v1/students?select=*")

                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim students = JsonConvert.DeserializeObject(Of List(Of Student))(json)
                    gvStudents.DataSource = students
                    gvStudents.DataBind()
                Else
                    ShowError("Failed to load students")
                End If
            End Using
        Catch ex As Exception
            ShowError($"Error loading students: {ex.Message}")
        End Try
    End Function

    Protected Sub gvStudents_RowEditing(sender As Object, e As GridViewEditEventArgs)
        gvStudents.EditIndex = e.NewEditIndex
        RegisterAsyncTask(New PageAsyncTask(AddressOf LoadStudents))
    End Sub

    Protected Sub gvStudents_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)
        RegisterAsyncTask(New PageAsyncTask(
            Async Function() As Task
                Try
                    Dim id = Convert.ToInt32(gvStudents.DataKeys(e.RowIndex).Value)
                    Dim row = gvStudents.Rows(e.RowIndex)

                    Dim firstName = CType(row.FindControl("txtFirstName"), TextBox).Text
                    Dim lastName = CType(row.FindControl("txtLastName"), TextBox).Text
                    Dim email = CType(row.FindControl("txtEmail"), TextBox).Text
                    Dim enrollmentDate = CType(row.FindControl("txtEnrollmentDate"), TextBox).Text
                    Dim isAdmin = CType(row.FindControl("chkIsAdmin"), CheckBox).Checked

                    Dim student = New With {
                        .first_name = firstName,
                        .last_name = lastName,
                        .email = email,
                        .enrollment_date = enrollmentDate,
                        .is_admin = isAdmin
                    }

                    Using client As New HttpClient()
                        client.DefaultRequestHeaders.Add("apikey", Login.SupabaseKey)
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Login.SupabaseKey}")
                        client.DefaultRequestHeaders.Add("Prefer", "return=representation")

                        Dim json = JsonConvert.SerializeObject(student)
                        Dim content = New StringContent(json, Encoding.UTF8, "application/json")

                        ' Create PATCH request using HttpMethod.Patch
                        Dim request = New HttpRequestMessage(New HttpMethod("PATCH"), $"{Login.SupabaseUrl}/rest/v1/students?id=eq.{id}")
                        request.Content = content

                        Dim response = Await client.SendAsync(request)

                        If response.IsSuccessStatusCode Then
                            gvStudents.EditIndex = -1
                            Await LoadStudents()
                        Else
                            Dim errorContent = Await response.Content.ReadAsStringAsync()
                            ShowError($"Update failed: {errorContent}")
                        End If
                    End Using
                Catch ex As Exception
                    ShowError($"Error updating student: {ex.Message}")
                End Try
            End Function))
    End Sub

    Protected Sub gvStudents_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs)
        gvStudents.EditIndex = -1
        RegisterAsyncTask(New PageAsyncTask(AddressOf LoadStudents))
    End Sub

    Protected Sub gvStudents_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        RegisterAsyncTask(New PageAsyncTask(
        Async Function() As Task
            Try
                Dim id = Convert.ToInt32(gvStudents.DataKeys(e.RowIndex).Value)

                Using client As New HttpClient()
                    client.DefaultRequestHeaders.Add("apikey", Login.SupabaseKey)
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Login.SupabaseKey}")

                    ' 1. Delete from assignment_grades
                    Dim deleteGrades = Await client.DeleteAsync($"{Login.SupabaseUrl}/rest/v1/assignment_grades?student_id=eq.{id}")
                    If Not deleteGrades.IsSuccessStatusCode Then
                        Dim err = Await deleteGrades.Content.ReadAsStringAsync()
                        ShowError($"Failed to delete grades: {err}")
                        Return
                    End If

                    ' 2. Delete from enrollments
                    Dim deleteEnrollments = Await client.DeleteAsync($"{Login.SupabaseUrl}/rest/v1/enrollments?student_id=eq.{id}")
                    If Not deleteEnrollments.IsSuccessStatusCode Then
                        Dim err = Await deleteEnrollments.Content.ReadAsStringAsync()
                        ShowError($"Failed to delete enrollments: {err}")
                        Return
                    End If

                    ' 3. Delete from bookmarks (user_id = student_id)
                    Dim deleteBookmarks = Await client.DeleteAsync($"{Login.SupabaseUrl}/rest/v1/bookmarks?user_id=eq.{id}")
                    If Not deleteBookmarks.IsSuccessStatusCode Then
                        Dim err = Await deleteBookmarks.Content.ReadAsStringAsync()
                        ShowError($"Failed to delete bookmarks: {err}")
                        Return
                    End If

                    ' 4. Delete from marketplace (user_id = student_id)
                    Dim deleteMarketplace = Await client.DeleteAsync($"{Login.SupabaseUrl}/rest/v1/marketplace?user_id=eq.{id}")
                    If Not deleteMarketplace.IsSuccessStatusCode Then
                        Dim err = Await deleteMarketplace.Content.ReadAsStringAsync()
                        ShowError($"Failed to delete listings: {err}")
                        Return
                    End If

                    ' 5. Delete from students
                    Dim deleteStudent = Await client.DeleteAsync($"{Login.SupabaseUrl}/rest/v1/students?id=eq.{id}")
                    If deleteStudent.IsSuccessStatusCode Then
                        Await LoadStudents()
                    Else
                        Dim err = Await deleteStudent.Content.ReadAsStringAsync()
                        ShowError($"Failed to delete student: {err}")
                    End If
                End Using

            Catch ex As Exception
                ShowError($"Error deleting student: {ex.Message}")
            End Try
        End Function))
    End Sub


    Protected Sub btnLogout_Click(sender As Object, e As EventArgs)
        Session.Clear()
        Response.Redirect("Login.aspx")
    End Sub
    Protected Sub btnAddStudent_Click(sender As Object, e As EventArgs)
        Response.Redirect("AddStudent.aspx")
    End Sub
    Protected Sub btnCourses_Click(sender As Object, e As EventArgs)
        Response.Redirect("Courses.aspx")
    End Sub
    Protected Sub btnEnroll_Click(sender As Object, e As EventArgs)
        Response.Redirect("Enroll.aspx")
    End Sub

    Private Sub ShowError(message As String)
        ' Implement your error display logic here
        ' For example: lblError.Text = message
        Div1.InnerText = message
        Div1.Visible = True

    End Sub
End Class

