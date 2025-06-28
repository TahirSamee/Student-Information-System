Imports System.Web
Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Threading.Tasks

Public Class AssignmentDescription
    Implements IHttpHandler

    ' Get credentials from Login class
    Private ReadOnly SupabaseUrl As String = Login.SupabaseUrl
    Private ReadOnly SupabaseKey As String = Login.SupabaseKey

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "text/plain"

        Dim idStr = context.Request.QueryString("id")
        If String.IsNullOrEmpty(idStr) OrElse Not Integer.TryParse(idStr, Nothing) Then
            context.Response.Write("Invalid or missing assignment ID.")
            Return
        End If

        Dim assignmentId = Convert.ToInt32(idStr)

        Try
            ' Safe way to call async method in ASHX
            Dim result = GetAssignmentDescriptionAsync(assignmentId).GetAwaiter().GetResult()
            context.Response.Write(result)
        Catch ex As Exception
            context.Response.Write("Exception: " & ex.Message)
        End Try
    End Sub

    Private Async Function GetAssignmentDescriptionAsync(assignmentId As Integer) As Task(Of String)
        Try
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

                Dim url = $"{SupabaseUrl}/rest/v1/assignments?assignment_id=eq.{assignmentId}&select=description"
                Dim response = Await client.GetAsync(url).ConfigureAwait(False)

                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync().ConfigureAwait(False)
                    Dim data = JsonConvert.DeserializeObject(Of List(Of Assignment))(json)

                    If data IsNot Nothing AndAlso data.Count > 0 Then
                        Dim desc = data(0).description
                        Return If(String.IsNullOrWhiteSpace(desc), "No description available.", desc)
                    Else
                        Return "No assignment found."
                    End If
                Else
                    Return $"Supabase Error: {response.StatusCode}"
                End If
            End Using
        Catch ex As Exception
            Return "Exception: " & ex.Message
        End Try
    End Function

    Public Class Assignment
        Public Property description As String
    End Class

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class
