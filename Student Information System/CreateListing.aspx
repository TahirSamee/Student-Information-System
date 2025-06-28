<%@ Page Title="Create Listing" Async="true" Language="vb" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeBehind="CreateListing.aspx.vb" Inherits="Student_Information_System.CreateListing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Create Listing
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-center mt-4">
        <div class="form-container section p-4 shadow-sm" style="max-width: 600px; width: 100%; background-color: #fff; border-radius: 8px;">
            
            <h4 class="fw-bold text-primary text-center mb-4">Create New Marketplace Listing</h4>

            <div class="mb-3">
                <label for="ddlCategory" class="form-label small">Category</label>
                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select form-select-sm">
                    <asp:ListItem Text="-- Select Category --" Value="" />
                    <asp:ListItem Text="Books" Value="Books" />
                    <asp:ListItem Text="Housing" Value="Housing" />
                    <asp:ListItem Text="Tutoring" Value="Tutoring" />
                </asp:DropDownList>
            </div>

            <div class="mb-3">
                <label for="txtTitle" class="form-label small">Title</label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control form-control-sm" placeholder="Enter listing title" />
            </div>

            <div class="mb-3">
                <label for="txtEmail" class="form-label small">Contact Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control form-control-sm" TextMode="Email" />
            </div>

            <div class="mb-3">
                <label for="filePicture" class="form-label small">Upload Picture</label>
                <asp:FileUpload ID="filePicture" runat="server" CssClass="form-control form-control-sm" />
                <asp:Image ID="imgPreview" runat="server" CssClass="img-thumbnail mt-2 d-block" Width="120" Visible="false" />
            </div>

            <div class="mb-3">
                <label for="txtPrice" class="form-label small">Price (CHF)</label>
                <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control form-control-sm" TextMode="Number" placeholder="0.00" />
            </div>

            <div class="mb-3">
                <label for="txtDescription" class="form-label small">Description</label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control form-control-sm" TextMode="MultiLine" Rows="4" MaxLength="300" placeholder="Max 300 characters" />
            </div>

            <asp:Button ID="btnSubmit" runat="server" Text="Post Listing" CssClass="btn btn-sm btn-success w-100 mb-2" OnClick="btnSubmit_Click" />
            <asp:Button ID="btnBack" runat="server" Text="← Back to Marketplace" CssClass="btn btn-sm btn-outline-secondary w-100" OnClick="btnBack_Click" />

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success mt-3 d-block text-center fw-semibold small" EnableViewState="false" />
        </div>
    </div>
</asp:Content>
