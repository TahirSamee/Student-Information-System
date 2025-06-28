<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Marketplace.aspx.vb" MasterPageFile="~/Site.master" Inherits="Student_Information_System.Marketplace" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Mini Marketplace
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .listing-card {
            background-color: #fff;
            border: 1px solid #e6e6e6;
            border-radius: 10px;
            box-shadow: 0 1px 6px rgba(0,0,0,0.04);
            margin-bottom: 20px;
            padding: 15px;
            transition: all 0.3s ease-in-out;
        }

        .listing-card:hover {
            box-shadow: 0 6px 20px rgba(0,0,0,0.08);
            transform: translateY(-2px);
        }

        .listing-img {
            width: 100%;
            height: 140px;
            object-fit: cover;
            border-radius: 6px;
        }

        .listing-title {
            font-size: 1.1rem;
            font-weight: 600;
            margin-bottom: 5px;
            color: #2c3e50;
        }

        .listing-meta {
            font-size: 0.92rem;
            color: #555;
            margin-bottom: 4px;
        }

        .listing-price {
            font-size: 0.95rem;
            
            font-weight: 600;
        }

        .btn-group-sm .btn {
            font-size: 0.8rem;
            padding: 4px 10px;
            margin-right: 4px;
        }

        .filter-bar {
            margin-bottom: 20px;
        }

        @media (max-width: 768px) {
            .listing-img {
                height: 120px;
            }
        }
    </style>

    <div class="container mt-4">
        <h4 class="text-primary fw-bold mb-4">Marketplace</h4>

        <!-- Filter & Create Button -->
        <div class="row filter-bar align-items-center mb-3 ">
            <div class="col-md-6 col-lg-4">
                <asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="True" CssClass="form-select form-select-sm" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged">
                    <asp:ListItem Text="-- All Categories --" Value="all" />
                    <asp:ListItem Text="Books" Value="Books" />
                    <asp:ListItem Text="Housing" Value="Housing" />
                    <asp:ListItem Text="Tutoring" Value="Tutoring" />
                    <asp:ListItem Text="Bookmarked" Value="bookmarked" />
                </asp:DropDownList>
            </div>
            <div class="col-md-6 col-lg-8 text-end">
                <asp:Button ID="btnCreateListing" runat="server" Text="Create Listing" CssClass="btn btn-primary btn-sm" OnClick="btnCreateListing_Click" />
            </div>
        </div>

        <!-- Repeater Listings -->
        <asp:Repeater ID="rptListings" runat="server">
            <ItemTemplate>
                <div class="row listing-card align-items-center">
                    <div class="col-12 col-md-2 mb-2 mb-md-0">
                        <img src='<%# Eval("picture_url") %>' class="listing-img" alt="Image" />
                    </div>
                    <div class="col-12 col-md-10">
                        <div class="listing-title"><%# Eval("title") %></div>
                        <div class="listing-meta"><strong>Category:</strong> <%# Eval("category") %></div>
                        <div class="listing-meta"><%# Eval("description") %></div>
                        <div class="listing-price text-primary"><strong>Price:</strong> CHF <%# Eval("price") %></div>
                        <div class="listing-meta"><strong>Email:</strong> <%# Eval("email") %></div>

                        <div class="btn-group-sm mt-2 d-flex flex-wrap">
                            <asp:Button ID="btnBookmark" runat="server" CommandArgument='<%# Eval("listing_id") %>' Text="Bookmark" CssClass="btn btn-success me-1 mb-1" OnClick="btnBookmark_Click" />

                            <asp:PlaceHolder ID="phAdminControls" runat="server" Visible='<%# Convert.ToBoolean(Session("IsAdmin")) = True %>'>
                                <asp:Button ID="btnEdit" runat="server" CommandArgument='<%# Eval("listing_id") %>' Text="Edit" CssClass="btn btn-primary me-1 mb-1" OnClick="btnEdit_Click" />
                                <asp:Button ID="btnDelete" runat="server" CommandArgument='<%# Eval("listing_id") %>' Text="Delete" CssClass="btn btn-danger mb-1" OnClick="btnDelete_Click" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- Message and Back Button -->
        <div class="text-end mt-3">
            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger fw-semibold small d-block mb-2"></asp:Label>
            <asp:Button ID="btnBack" runat="server" Text="← Back to Dashboard" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnBack_Click" />
        </div>
    </div>
</asp:Content>
