Imports System.IO
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Student_Information_System.Marketplace

Public Class CreateListing
    Inherits System.Web.UI.Page

    Private listingId As Integer = 0
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


            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                listingId = Convert.ToInt32(Request.QueryString("id"))
                Await LoadListingForEdit(listingId)
            End If
        End If
    End Sub

    Private Async Function LoadListingForEdit(id As Integer) As Task
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")

            Dim res = Await client.GetAsync($"{SupabaseUrl}/rest/v1/marketplace?listing_id=eq.{id}")
            If res.IsSuccessStatusCode Then
                Dim json = Await res.Content.ReadAsStringAsync()
                Dim listings = JsonConvert.DeserializeObject(Of List(Of Listing))(json)
                If listings.Any() Then
                    Dim l = listings.First()
                    txtTitle.Text = l.title
                    ddlCategory.SelectedValue = l.category
                    txtDescription.Text = l.description
                    txtPrice.Text = l.price.ToString()
                    txtEmail.Text = l.email
                    imgPreview.ImageUrl = l.picture_url
                    imgPreview.Visible = True
                End If
            End If
        End Using
    End Function

    Protected Async Sub btnSubmit_Click(sender As Object, e As EventArgs)
        lblMessage.Text = ""
        lblMessage.CssClass = ""

        If String.IsNullOrWhiteSpace(txtTitle.Text) OrElse
           String.IsNullOrEmpty(ddlCategory.SelectedValue) OrElse
           String.IsNullOrWhiteSpace(txtDescription.Text) OrElse
           String.IsNullOrWhiteSpace(txtPrice.Text) OrElse
           String.IsNullOrWhiteSpace(txtEmail.Text) Then

            lblMessage.CssClass = "text-danger"
            lblMessage.Text = "All required fields must be filled."
            Return
        End If

        Dim imageUrl As String = "https://placehold.co/300x180?text=No+Image"

        If filePicture.HasFile Then
            Dim ext = Path.GetExtension(filePicture.FileName).ToLower()
            If ext <> ".jpg" AndAlso ext <> ".jpeg" AndAlso ext <> ".png" Then
                lblMessage.CssClass = "text-danger"
                lblMessage.Text = "Only JPG and PNG images are allowed."
                Return
            End If

            ' Generate unique filename
            Dim fileName = Guid.NewGuid().ToString() & ext
            Dim uploadFolder = Server.MapPath("~/Uploads/")

            ' ✅ Create the folder if it doesn't exist
            If Not Directory.Exists(uploadFolder) Then
                Directory.CreateDirectory(uploadFolder)
            End If

            ' Save the file
            Dim savePath = Path.Combine(uploadFolder, fileName)
            filePicture.SaveAs(savePath)

            ' Get virtual URL for display
            imageUrl = ResolveUrl("~/Uploads/" & fileName)
        End If


        Dim listing = New With {
            .user_id = userId,
            .title = txtTitle.Text.Trim(),
            .category = ddlCategory.SelectedValue,
            .description = txtDescription.Text.Trim(),
            .price = Convert.ToDecimal(txtPrice.Text),
            .picture_url = imageUrl,
            .email = txtEmail.Text.Trim()
        }

        Dim isUpdate = Not String.IsNullOrEmpty(Request.QueryString("id"))
        Dim url = If(isUpdate, $"{SupabaseUrl}/rest/v1/marketplace?listing_id=eq.{Request.QueryString("id")}", $"{SupabaseUrl}/rest/v1/marketplace")
        Dim method = If(isUpdate, New HttpMethod("PATCH"), HttpMethod.Post)

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}")
            client.DefaultRequestHeaders.Add("Prefer", "return=representation")

            Dim content = New StringContent(JsonConvert.SerializeObject(listing), Encoding.UTF8, "application/json")
            Dim request = New HttpRequestMessage(method, url) With {.Content = content}
            Dim res = Await client.SendAsync(request)

            If res.IsSuccessStatusCode Then
                lblMessage.CssClass = "text-success"
                lblMessage.Text = If(isUpdate, "Listing updated successfully.", "Listing posted successfully!")

                If Not isUpdate Then
                    ddlCategory.SelectedIndex = 0
                    txtTitle.Text = ""
                    txtEmail.Text = ""
                    txtDescription.Text = ""
                    txtPrice.Text = ""
                    imgPreview.Visible = False
                End If
            Else
                lblMessage.CssClass = "text-danger"
                lblMessage.Text = "Failed to save listing."
            End If
        End Using
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        Response.Redirect("Marketplace.aspx")
    End Sub
End Class
