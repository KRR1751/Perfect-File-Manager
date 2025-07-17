Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1
    Public m_ChildFormNumber As Integer = 0
    Public curFormActive As Integer = 0
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load, NewWindowToolStripMenuItem.Click
        Dim ChildForm As New FolderView
        ChildForm.MdiParent = Me
        m_ChildFormNumber += 1
        ChildForm.Tag = m_ChildFormNumber
        Dim args() As String = Environment.GetCommandLineArgs()

        If args.Length > 1 Then
            Dim resolvedPath As String = String.Empty
            Dim potentialPath As String = args(1)

            Try
                If potentialPath.ToLower().StartsWith("shell:") Then
                    Dim shellCommand As String = potentialPath.Substring("shell:".Length).Trim()
                    resolvedPath = FolderView.GetPathFromShellCommand(shellCommand)
                End If

                If String.IsNullOrEmpty(resolvedPath) Then
                    resolvedPath = Environment.ExpandEnvironmentVariables(potentialPath)
                End If

                If String.IsNullOrEmpty(resolvedPath) Then
                    resolvedPath = potentialPath
                End If

                If System.IO.Directory.Exists(potentialPath) Then
                    ChildForm.initialPath = potentialPath
                    System.Diagnostics.Debug.WriteLine("DEBUG: Nalezen argument příkazového řádku - cesta: " & ChildForm.initialPath)
                Else
                    System.Diagnostics.Debug.WriteLine("DEBUG: Argument příkazového řádku není platná cesta ke složce: " & potentialPath)
                End If

            Catch ex As Exception
                MessageBox.Show("Chyba při zpracování cesty: " & ex.Message, "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If

        If String.IsNullOrEmpty(ChildForm.initialPath) Then
            ChildForm.initialPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            System.Diagnostics.Debug.WriteLine("DEBUG: Argument příkazového řádku nenalezen nebo neplatný, načítám výchozí cestu: " & ChildForm.initialPath)
        End If

        ChildForm.Show()
    End Sub

    Private Sub CascadeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CascadeToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.Cascade)
    End Sub

    Private Sub TileVerticalToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TileVerticalToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.TileVertical)
    End Sub

    Private Sub TileHorizontalToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TileHorizontalToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.TileHorizontal)
    End Sub

    Private Sub CloseAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseAllToolStripMenuItem.Click
        For Each ChildForm In MdiChildren
            ChildForm.Close()
        Next
    End Sub

    Private Sub ArrangeIconsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ArrangeIconsToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.ArrangeIcons)
    End Sub
End Class
