<%@ Page Language="vb" Async="true" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="Student_Information_System.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Student Information System - Login</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" rel="stylesheet" />
    <style>
        body {
            background: linear-gradient(to right, #3288bd, #3288bd, #3288bd);
            height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: 'Segoe UI', sans-serif;
        }
        .btn-primary{
            background:#3288bd;
        }
        .login-card {
            max-width: 420px;
            width: 100%;
            padding: 30px;
            border-radius: 15px;
            box-shadow: 0 0 40px rgba(0, 0, 0, 0.2);
            background-color: #ffffff;
            animation: fadeIn 0.6s ease-in-out;
        }

        .login-title {
            font-size: 26px;
            font-weight: 700;
            margin-bottom: 20px;
            color: #3288bd;
            text-align: center;
        }

        .form-group {
            margin-bottom: 18px;
        }

        .form-control:focus {
            border-color: #0d6efd;
            box-shadow: 0 0 0 0.2rem rgba(13, 110, 253, 0.25);
        }

        .input-icon {
           
            transform: translateY(-50%);
            color: #3288bd;
        }

        .input-group .form-control {
            padding-left: 2.2rem;
        }

        .btn-primary {
            font-weight: 600;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(-15px); }
            to { opacity: 1; transform: translateY(0); }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-card">
            <div class="login-title">🔐 Log in to your Account</div>

            <div class="form-group position-relative">
                <label for="txtEmail" class="form-label"><i class="bi bi-envelope-fill input-icon"></i> Email</label>
                
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="Enter your email" />
            </div>

            <div class="form-group position-relative">
                <label for="txtPassword" class="form-label">  <i class="bi bi-lock-fill input-icon"></i> Password</label>
              
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter your password" />
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary w-100 mt-2" OnClick="btnLogin_Click" />

            <asp:Label ID="lblError" runat="server" CssClass="text-danger d-block mt-3 text-center fw-semibold" EnableViewState="false"></asp:Label>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
