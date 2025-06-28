Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class Login
    Inherits System.Web.UI.Page

    Public Shared ReadOnly SupabaseUrl As String = "https://yhtgwnkquhyjscypjqsx.supabase.co"

    Public Shared ReadOnly SupabaseKey As String = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InlodGd3bmtxdWh5anNjeXBqcXN4Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA1MTE2NDEsImV4cCI6MjA2NjA4NzY0MX0.O-Cm7QKaFdAuDFcn9vqbfDaKzTVpghUPvv0Y-FE_rOs"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub
    Protected Async Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim email = txtEmail.Text.Trim()
        Dim password = txtPassword.Text

        Using client As New HttpClient()
            ' Configure headers for Supabase REST API
            client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            ' Query student by email
            Dim response = Await client.GetAsync($"{SupabaseUrl}/rest/v1/students?email=eq.{email}")

            If response.IsSuccessStatusCode Then
                Dim responseBody = Await response.Content.ReadAsStringAsync()
                Dim students = JsonConvert.DeserializeObject(Of List(Of Student))(responseBody)

                If students IsNot Nothing AndAlso students.Count = 1 Then
                    Dim student = students.First()

                    ' Verify password against stored hash
                    If BCrypt.Net.BCrypt.Verify(password, student.password_hash) Then
                        ' Set session variables
                        Session("UserId") = student.id
                        Session("IsAdmin") = student.is_admin
                        Session("UserName") = $"{student.first_name} {student.last_name}"
                        Session("UserEmail") = student.email

                        ' Redirect based on role
                        If student.is_admin Then
                            System.Web.HttpContext.Current.Response.Redirect("AdminDashboard.aspx", False)
                            Context.ApplicationInstance.CompleteRequest()

                        Else
                            System.Web.HttpContext.Current.Response.Redirect("StudentDashboard.aspx", False)
                            Context.ApplicationInstance.CompleteRequest()
                        End If
                        Return
                    End If
                End If
            End If

            ' If we reach here, authentication failed
            lblError.Text = "Invalid email or password"
        End Using
    End Sub
End Class

Public Class Student
    Public Property id As Integer
    Public Property first_name As String
    Public Property last_name As String
    Public Property email As String
    Public Property enrollment_date As DateTime
    Public Property password_hash As String
    Public Property is_admin As Boolean
End Class




