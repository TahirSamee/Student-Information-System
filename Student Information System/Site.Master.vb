Public Class SiteMaster
    Inherits MasterPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        navLinks.Controls.Clear()

        ' Get current page name without extension (e.g., "Courses" from "/Courses" or "/Courses.aspx")
        Dim currentPage As String = System.IO.Path.GetFileNameWithoutExtension(Request.Url.AbsolutePath)

        ' Helper function to add 'active' class
        Dim isActive = Function(page As String) As String
                           Return If(currentPage.Equals(System.IO.Path.GetFileNameWithoutExtension(page), StringComparison.OrdinalIgnoreCase), " active", "")
                       End Function

        If Session("IsAdmin") IsNot Nothing Then
            Dim role = Convert.ToBoolean(Session("IsAdmin"))

            If role = True Then
                navLinks.Controls.Add(New LiteralControl($"<li class='nav-item'><a class='nav-link text-light{isActive("AdminDashboard.aspx")}' href='AdminDashboard.aspx'>Students</a></li>"))
                navLinks.Controls.Add(New LiteralControl($"<li class='nav-item'><a class='nav-link text-light{isActive("Courses.aspx")}' href='Courses.aspx'>Courses</a></li>"))
                navLinks.Controls.Add(New LiteralControl($"<li class='nav-item'><a class='nav-link text-light{isActive("Enroll.aspx")}' href='Enroll.aspx'>Enrollments</a></li>"))
                navLinks.Controls.Add(New LiteralControl($"<li class='nav-item'><a class='nav-link text-light{isActive("Assignments.aspx")}' href='Assignments.aspx'>Assignments</a></li>"))
                navLinks.Controls.Add(New LiteralControl($"<li class='nav-item'><a class='nav-link text-light{isActive("GradeAssignment.aspx")}' href='GradeAssignment.aspx'>Grades</a></li>"))
                navLinks.Controls.Add(New LiteralControl($"<li class='nav-item'><a class='nav-link text-light{isActive("Marketplace.aspx")}' href='Marketplace.aspx'>Marketplace</a></li>"))
            Else
                navLinks.Controls.Add(New LiteralControl($"<li class='nav-item'><a class='nav-link text-light{isActive("StudentDashboard.aspx")}' href='StudentDashboard.aspx'>My Dashboard</a></li>"))
                navLinks.Controls.Add(New LiteralControl($"<li class='nav-item'><a class='nav-link text-light{isActive("Enroll.aspx")}' href='Enroll.aspx'>Enrollments</a></li>"))
                navLinks.Controls.Add(New LiteralControl($"<li class='nav-item'><a class='nav-link text-light{isActive("Marketplace.aspx")}' href='Marketplace.aspx'>Marketplace</a></li>"))
            End If
        End If
    End Sub

End Class