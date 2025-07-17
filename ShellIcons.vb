Imports System.Drawing
Imports System.IO
Imports System.Runtime.InteropServices
Imports FileManager.ShellApi

Public Class ShellIcons
    Private Const SHGSI_ICON As UInteger = &H100    ' hIcon
    Private Const SHGSI_SMALLICON As UInteger = &H1      ' malá verze
    Public Shared Function GetFileIcon(ByVal filePath As String, ByVal isSmallIcon As Boolean) As Icon
        Dim shfi As New SHFILEINFO()
        Dim flags As SHGFI = SHGFI.Icon Or SHGFI.SysIconIndex ' Chceme ikonu a její systémový index

        ' Přidáme vlajku pro velikost ikony
        If isSmallIcon Then
            flags = flags Or SHGFI.SmallIcon ' Malá ikona
        Else
            flags = flags Or SHGFI.LargeIcon ' Velká ikona
        End If

        ' Pokud je to adresář, můžeme chtít "otevřenou" ikonu
        If System.IO.Directory.Exists(filePath) Then
            flags = flags Or SHGFI.OpenIcon ' Pro složky chceme "otevřenou" ikonu
        End If

        ' Volání SHGetFileInfo
        Dim result As IntPtr = NativeMethods.SHGetFileInfo(filePath,
                                                       CInt(System.IO.FileAttributes.Normal),
                                                       shfi,
                                                       CUInt(Marshal.SizeOf(shfi)),
                                                       flags)

        If result <> IntPtr.Zero Then
            Try
                Dim icon As Icon = Icon.FromHandle(shfi.hIcon)
                ' Vytvoříme kopii ikony, protože originální handle bude uvolněn
                Dim clonedIcon As Icon = CType(icon.Clone(), Icon)
                NativeMethods.DestroyIcon(shfi.hIcon) ' Uvolníme handle získaný z SHGetFileInfo
                Return clonedIcon
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine($"ERROR: Chyba při vytváření ikony pro {filePath}: {ex.Message}")
            End Try
        End If

        Return Nothing ' Pokud se nepodařilo získat ikonu
    End Function

    Public Shared Function GetFolderIcon(ByVal smallIcon As Boolean) As Icon
        Dim flags As Integer = SHGFI.Icon
        If smallIcon Then
            flags = flags Or SHGFI.SmallIcon
        Else
            flags = flags Or SHGFI.LargeIcon
        End If

        Dim shfi As SHFILEINFO = New SHFILEINFO()
        Dim tempFolderPath As String = Path.GetTempPath()
        If Not Directory.Exists(tempFolderPath) Then
            tempFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        End If

        SHGetFileInfo(tempFolderPath, System.IO.FileAttributes.Normal, shfi, Marshal.SizeOf(shfi), flags)

        If shfi.hIcon <> IntPtr.Zero Then
            Dim icon As Icon = Icon.FromHandle(shfi.hIcon)
            Return icon
        Else
            Return Nothing
        End If
    End Function
End Class