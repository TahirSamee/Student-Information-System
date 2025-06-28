Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class Marketplace
    Inherits System.Web.UI.Page

    Private ReadOnly SupabaseUrl As String = Login.SupabaseUrl
    Private ReadOnly SupabaseKey As String = Login.SupabaseKey
    Private userId As Integer

    Protected Async Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("UserId") Is Nothing Then
            Response.Redirect("Login.aspx")
            Return
        End If
        userId = CInt(Session("UserId"))
        If Not IsPostBack Then
            Await LoadListings("all")
        End If
    End Sub
    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Dim role = Convert.ToBoolean(Session("IsAdmin"))
        If role = True Then
            Response.Redirect("AdminDashboard.aspx")
        Else
            Response.Redirect("StudentDashboard.aspx")
        End If

    End Sub
    Protected Async Sub ddlFilter_SelectedIndexChanged(sender As Object, e As EventArgs)
        Await LoadListings(ddlFilter.SelectedValue)
    End Sub

    Private Async Function LoadListings(filter As String) As Task
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            Dim url As String

            If filter = "bookmarked" Then
                url = $"{SupabaseUrl}/rest/v1/bookmarks?user_id=eq.{userId}&select=marketplace(listing_id,category,description,price,picture_url,email)"
            ElseIf filter = "all" Then
                url = $"{SupabaseUrl}/rest/v1/marketplace"
            Else
                url = $"{SupabaseUrl}/rest/v1/marketplace?category=eq.{filter}"
            End If

            Dim response = Await client.GetAsync(url)
            If response.IsSuccessStatusCode Then
                Dim json = Await response.Content.ReadAsStringAsync()

                If filter = "bookmarked" Then
                    Dim bookmarked = JsonConvert.DeserializeObject(Of List(Of BookmarkCombined))(json)
                    Dim listings = bookmarked.Select(Function(b) b.marketplace).ToList()
                    rptListings.DataSource = listings
                Else
                    Dim listings = JsonConvert.DeserializeObject(Of List(Of Listing))(json)
                    rptListings.DataSource = listings
                End If

                rptListings.DataBind()
            End If
        End Using
    End Function

    Protected Async Sub btnBookmark_Click(sender As Object, e As EventArgs)
        Dim btn = CType(sender, Button)
        Dim listingId = Convert.ToInt32(btn.CommandArgument)

        Dim bookmark = New With {
            .listing_id = listingId,
            .user_id = userId
        }

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")
            client.DefaultRequestHeaders.Add("Prefer", "resolution=merge-duplicates")

            Dim content = New StringContent(JsonConvert.SerializeObject(bookmark), Encoding.UTF8, "application/json")
            Await client.PostAsync($"{SupabaseUrl}/rest/v1/bookmarks", content)
        End Using

        Await LoadListings(ddlFilter.SelectedValue)
    End Sub
    Protected Sub btnEdit_Click(sender As Object, e As EventArgs)
        If Not IsAdmin() Then
            Response.Redirect("Marketplace.aspx")
            Return
        End If

        Dim btn = CType(sender, Button)
        Dim listingId = Convert.ToInt32(btn.CommandArgument)
        Response.Redirect($"CreateListing.aspx?id={listingId}")
    End Sub

    Protected Async Sub btnDelete_Click(sender As Object, e As EventArgs)
        If Not IsAdmin() Then
            Response.Redirect("Marketplace.aspx")
            Return
        End If

        Dim btn = CType(sender, Button)
        Dim listingId = Convert.ToInt32(btn.CommandArgument)

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")
            client.DefaultRequestHeaders.Add("Prefer", "return=minimal")

            ' Optional: Delete related bookmarks first (if this table exists)
            Await client.DeleteAsync($"{SupabaseUrl}/rest/v1/bookmarks?listing_id=eq.{listingId}")

            ' Then delete listing
            Dim url = $"{SupabaseUrl}/rest/v1/marketplace?listing_id=eq.{listingId}"
            Dim res = Await client.DeleteAsync(url)

            If res.IsSuccessStatusCode Then
                Await LoadListings(ddlFilter.SelectedValue)
            Else
                lblMessage.CssClass = "text-danger"
                lblMessage.Text = "Failed to delete listing. It may be linked to other data."
            End If
        End Using
    End Sub


    Private Function IsAdmin() As Boolean
        Return Session("IsAdmin") IsNot Nothing AndAlso CBool(Session("IsAdmin"))
    End Function


    Protected Sub btnCreateListing_Click(sender As Object, e As EventArgs)
        Response.Redirect("CreateListing.aspx")
    End Sub

    Public Class Listing
        Public Property listing_id As Integer
        Public Property user_id As Integer
        Public Property title As String
        Public Property category As String
        Public Property description As String
        Public Property price As Decimal
        Public Property picture_url As String
        Public Property email As String
    End Class

    Public Class BookmarkCombined
        Public Property marketplace As Listing
    End Class
End Class
