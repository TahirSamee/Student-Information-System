Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class StudentDashboard
    Inherits System.Web.UI.Page

    Private ReadOnly SupabaseUrl As String = Login.SupabaseUrl
    Private ReadOnly SupabaseKey As String = Login.SupabaseKey

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("UserId") Is Nothing OrElse Session("IsAdmin") Is Nothing OrElse CBool(Session("IsAdmin")) = True Then
            Response.Redirect("Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            lblStudentName.Text = Convert.ToString(Session("UserName"))

            ' 🟢 Register async tasks correctly
            RegisterAsyncTask(New PageAsyncTask(AddressOf LoadEnrollments))
            RegisterAsyncTask(New PageAsyncTask(AddressOf LoadAssignmentsAndGrades))
        End If
    End Sub


    Private Async Function LoadEnrollments() As Task
        Dim studentId = Convert.ToInt32(Session("UserId"))
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            ' Get student's enrollments with course info
            Dim url = $"{SupabaseUrl}/rest/v1/enrollments?student_id=eq.{studentId}&select=enrollment_date,courses(course_name,format,instructor)"
            Dim response = Await client.GetAsync(url)

            If response.IsSuccessStatusCode Then
                Dim json = Await response.Content.ReadAsStringAsync()
                Dim enrollments = JsonConvert.DeserializeObject(Of List(Of EnrollmentCombined))(json)

                Dim display = enrollments.Select(Function(e) New With {
                    .course_name = e.courses.course_name,
                    .format = e.courses.format,
                    .instructor = e.courses.instructor,
                    .enrollment_date = e.enrollment_date.ToString("yyyy-MM-dd")
                }).ToList()

                gvEnrollments.DataSource = display
                gvEnrollments.DataBind()
            End If
        End Using
    End Function

    Protected Sub btnLogout_Click(sender As Object, e As EventArgs)
        Session.Clear()
        Response.Redirect("Login.aspx")
    End Sub

    Protected Sub btnEnroll_Click(sender As Object, e As EventArgs)
        Response.Redirect("Enroll.aspx")
    End Sub

    Private Async Function LoadAssignmentsAndGrades() As Task
        Dim studentId = Convert.ToInt32(Session("UserId"))
        Dim totalGrades As New List(Of Decimal)

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            ' Query assignments joined with grades and course name
            'Dim url = $"{SupabaseUrl}/rest/v1/assignment_grades?student_id=eq.{studentId}&select=grade,assignments(title,due_date,courses(course_name))"
            Dim url = $"{SupabaseUrl}/rest/v1/assignment_grades?student_id=eq.{studentId}&select=grade,assignments(assignment_id,title,due_date,courses(course_name))"

            Dim res = Await client.GetAsync(url)

            If res.IsSuccessStatusCode Then
                Dim json = Await res.Content.ReadAsStringAsync()
                Dim data = JsonConvert.DeserializeObject(Of List(Of GradeCombined))(json)

                Dim display = data.Select(Function(a)
                                              Dim g As String = "Not yet graded"
                                              If a.grade.HasValue AndAlso a.grade.Value > 0D Then
                                                  g = a.grade.Value.ToString("0.0")
                                                  totalGrades.Add(a.grade.Value)
                                              End If

                                              Return New With {
        .assignment_id = a.assignments.assignment_id,
        .title = a.assignments.title,
        .due_date = a.assignments.due_date,
        .course_name = a.assignments.courses.course_name,
        .grade_display = g
    }
                                          End Function).ToList()


                gvAssignments.DataSource = display
                gvAssignments.DataBind()

                lblGPA.Text = If(totalGrades.Count > 0, (totalGrades.Average()).ToString("0.00"), "N/A")
            End If
        End Using
    End Function

    Public Class GradeCombined
        Public Property grade As Decimal?
        Public Property assignments As AssignmentWithCourse
    End Class

    Public Class AssignmentWithCourse
        Public Property assignment_id As Integer
        Public Property title As String
        Public Property due_date As Date
        Public Property courses As CourseInfo
    End Class



    Public Class EnrollmentCombined
        Public Property enrollment_date As Date
        Public Property courses As CourseInfo
    End Class

    Public Class CourseInfo
        Public Property course_name As String
        Public Property format As String
        Public Property instructor As String
    End Class
End Class
