Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json

Public Class AddStudent
    Inherits System.Web.UI.Page

    Protected Async Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim student = New With {
            .first_name = txtFirstName.Text.Trim(),
            .last_name = txtLastName.Text.Trim(),
            .email = txtEmail.Text.Trim(),
            .password_hash = BCrypt.Net.BCrypt.HashPassword(txtPassword.Text),
            .enrollment_date = txtEnrollmentDate.Text,
            .is_admin = chkIsAdmin.Checked
        }

        Try
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Add("apikey", Login.SupabaseKey)
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Login.SupabaseKey}")

                Dim json = JsonConvert.SerializeObject(student)
                Dim content = New StringContent(json, Encoding.UTF8, "application/json")

                Dim response = Await client.PostAsync($"{Login.SupabaseUrl}/rest/v1/students", content)

                If response.IsSuccessStatusCode Then
                    lblMessage.Text = "Student added successfully!"
                    ClearForm()
                Else
                    lblMessage.CssClass = "text-danger"
                    lblMessage.Text = "Failed to add student: " & Await response.Content.ReadAsStringAsync()
                End If
            End Using
        Catch ex As Exception
            lblMessage.CssClass = "text-danger"
            lblMessage.Text = "Error: " & ex.Message
        End Try
    End Sub
    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("AdminDashboard.aspx")
    End Sub

    Private Sub ClearForm()
        txtFirstName.Text = ""
        txtLastName.Text = ""
        txtEmail.Text = ""
        txtPassword.Text = ""
        txtEnrollmentDate.Text = ""
        chkIsAdmin.Checked = False
    End Sub
End Class
