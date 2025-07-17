Imports System.Drawing
Imports System.IO
Imports System.Media
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar
Imports System.Xml
Imports FileManager.ShellApi
Imports Shell32

' Kód do formuláře (např. Form1)
Public Class FolderView

    Private Declare Auto Function ExtractIconEx Lib "shell32.dll" (
        ByVal lpszFile As String,
        ByVal nIconIndex As Integer,
        ByVal phiconLarge() As IntPtr,
        ByVal phiconSmall() As IntPtr,
        ByVal nIcons As UInteger
    ) As UInteger

    Private Declare Auto Function DestroyIcon Lib "user32.dll" (
        ByVal hIcon As IntPtr
    ) As Boolean


    <DllImport("shell32.dll", SetLastError:=True)>
    Private Shared Function SHGetStockIconInfo(
    siid As SHSTOCKICONID,
    uFlags As UInteger,
    ByRef psii As SHSTOCKICONINFO
) As Integer
    End Function

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure SHSTOCKICONINFO
        Public cbSize As UInteger
        Public hIcon As IntPtr
        Public iSysImageIndex As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)>
        Public szPath As String
    End Structure

    Private Enum SHSTOCKICONID As Integer
        SIID_SHIELD = 77
        SIID_PROPERTIES = 124
        SIID_COPY = 236
        SIID_CUT = 233
        SIID_PASTE = 237
        SIID_DELETE = 234
        SIID_RENAME = 305
    End Enum

    Private Const SHGSI_ICON As UInteger = &H100    ' hIcon
    Private Const SHGSI_SMALLICON As UInteger = &H1      ' malá verze

    Private Function IsDriveRoot(path As String) As Boolean
        Dim root = System.IO.Path.GetPathRoot(path)
        ' Srovnání bez ohledu na lomítka na konci
        Return String.Equals(root?.TrimEnd("\"c, "/"c),
                         path.TrimEnd("\"c, "/"c),
                         StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function GetIconBitmap(path As String) As Bitmap
        Try
            Dim ico As Icon = Icon.ExtractAssociatedIcon(path)
            Return ico.ToBitmap()
        Catch
            ' fallback: generická ikona
            Return SystemIcons.WinLogo.ToBitmap()
        End Try
    End Function
    Private Function GetStockIconBmp(siid As SHSTOCKICONID) As Bitmap
        Dim info As New SHSTOCKICONINFO()
        info.cbSize = CUInt(Marshal.SizeOf(info))
        If SHGetStockIconInfo(siid, SHGSI_ICON Or SHGSI_SMALLICON, info) = 0 Then
            Dim ico = Icon.FromHandle(info.hIcon)
            Return ico.ToBitmap()
        End If
        Return Nothing
    End Function
    Private Function CleanDisplayName(rawName As String) As String
        ' 1) Odstranit ampersand (klávesová zkratka)
        Dim noAmp = rawName.Replace("&"c, "")
        ' 2) Odstranit trojtečku (…) nebo tři tečky
        noAmp = noAmp.Replace("…", "").Replace("...", "")
        ' 3) Trim pro čistý výsledek
        Return noAmp.Trim()
    End Function
    Private ReadOnly FileIconsSmall As ImageList
    Private ReadOnly FileIconsLarge As ImageList

    Public Sub New()
        MyBase.New()
        InitializeComponent()

        ImageList1 = New ImageList()
        ImageList1.ImageSize = New Size(16, 16)
        ImageList1.ColorDepth = ColorDepth.Depth32Bit

        ImageList2 = New ImageList()
        ImageList2.ImageSize = New Size(32, 32)
        ImageList2.ColorDepth = ColorDepth.Depth32Bit

        ListView1.SmallImageList = ImageList1
        ListView1.LargeImageList = ImageList2

        If Not ImageList1.Images.ContainsKey("DefaultFolder") Then
            Dim defaultFolderIcon As Icon = SystemIcons.GetStockIcon(StockIconId.Folder)
            If defaultFolderIcon IsNot Nothing Then ImageList1.Images.Add("DefaultFolder", defaultFolderIcon)
            If defaultFolderIcon IsNot Nothing Then ImageList2.Images.Add("DefaultFolder", defaultFolderIcon)
        End If

        If Not ImageList1.Images.ContainsKey("DefaultFile") Then
            Dim defaultFileIcon As Icon = SystemIcons.Application
            If defaultFileIcon IsNot Nothing Then ImageList1.Images.Add("DefaultFile", defaultFileIcon)
            If defaultFileIcon IsNot Nothing Then ImageList2.Images.Add("DefaultFile", defaultFileIcon)
        End If

        If Not ImageList1.Images.ContainsKey("NoExtension") Then
            Dim noExtIcon As Icon = SystemIcons.GetStockIcon(StockIconId.DocumentNoAssociation)
            ImageList1.Images.Add("NoExtension", noExtIcon)
            ImageList2.Images.Add("NoExtension", noExtIcon)
        End If

    End Sub


    ' Metoda pro získání ikony souboru/složky a přidání do ImageListů
    Private Function GetFileIcon(ByVal filePath As String, ByVal isDirectory As Boolean) As Integer
        Dim shfi As New SHFILEINFO()
        Dim flags As SHGFI = SHGFI.Icon Or SHGFI.SysIconIndex ' Chceme ikonu a její systémový index

        If isDirectory Then
            flags = flags Or SHGFI.OpenIcon ' Pro složky chceme "otevřenou" ikonu
        End If

        ' Zkusíme nejprve získat malou ikonu (16x16)
        If SHGetFileInfo(filePath,
                         CInt(FileAttributes.Normal),
                         shfi,
                         CUInt(Marshal.SizeOf(shfi)),
                         flags Or SHGFI.SmallIcon) <> IntPtr.Zero Then ' Získáme malou ikonu

            ' Zkontrolujeme, zda ikona již není v našem ImageListu (podle indexu)
            ' SystemIcons.GetStockIcon lze použít pro standardní ikony, ale SHGetFileInfo je spolehlivější pro soubory
            Dim iconKey As String = shfi.iIcon.ToString() ' Použijeme systémový index jako klíč

            If Not FileIconsSmall.Images.ContainsKey(iconKey) Then
                Try
                    Dim icon As Icon = icon.FromHandle(shfi.hIcon)
                    FileIconsSmall.Images.Add(iconKey, icon)
                    ' Pro velkou ikonu musíme zavolat SHGetFileInfo znovu s LargeIcon
                    If SHGetFileInfo(filePath,
                                     CInt(FileAttributes.Normal),
                                     shfi,
                                     CUInt(Marshal.SizeOf(shfi)),
                                     flags Or SHGFI.LargeIcon) <> IntPtr.Zero Then ' Získáme velkou ikonu
                        Dim largeIcon As Icon = icon.FromHandle(shfi.hIcon)
                        FileIconsLarge.Images.Add(iconKey, largeIcon)
                        DestroyIcon(largeIcon.Handle) ' Uvolníme handle velké ikony
                    End If
                    DestroyIcon(icon.Handle) ' Uvolníme handle malé ikony
                Catch ex As Exception
                    System.Diagnostics.Debug.WriteLine($"ERROR: Chyba při získávání ikony pro {filePath}: {ex.Message}")
                    Return -1 ' Vrátíme -1 nebo jinou hodnotu pro chybu
                End Try
            End If
            Return FileIconsSmall.Images.IndexOfKey(iconKey) ' Vrátíme index ikony v našem ImageListu
        End If

        Return -1 ' Pokud se nepodařilo získat ikonu, vrátíme -1
    End Function

    Private navigationHistory As New List(Of String)()
    Private currentHistoryIndex As Integer = -1
    Private BackBtnUsed As Boolean = False

    Public initialPath As String = String.Empty
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TreeView1.ImageList = ImageList4
        TreeView2.ImageList = ImageList3
        ListView1.SmallImageList = ImageList1
        ListView1.LargeImageList = ImageList2

        ImageList1.Images.Clear()
        ImageList2.Images.Clear()
        ImageList3.Images.Clear()
        ImageList4.Images.Clear()

        ImageList1.ImageSize = New Size(16, 16)
        ImageList2.ImageSize = New Size(32, 32)
        ImageList3.ImageSize = New Size(16, 16)
        ImageList4.ImageSize = New Size(16, 16)

        If ListView1.Columns.Count = 0 Then
            ListView1.Columns.Add("Name", 200)
            ListView1.Columns.Add("Type", 100)
            ListView1.Columns.Add("Date modified", 150)
            ListView1.Columns.Add("Size", 100, HorizontalAlignment.Right)
        End If

        LoadDrives()

        If Not ImageList1.Images.ContainsKey("DefaultFolder") Then
            Dim defaultFolderSmallIcon As Icon = ShellIcons.GetFolderIcon(True)
            Dim defaultFolderLargeIcon As Icon = ShellIcons.GetFolderIcon(False)
            If defaultFolderSmallIcon IsNot Nothing Then ImageList1.Images.Add("DefaultFolder", defaultFolderSmallIcon)
            If defaultFolderLargeIcon IsNot Nothing Then ImageList2.Images.Add("DefaultFolder", defaultFolderLargeIcon)
        End If

        If Not ImageList1.Images.ContainsKey("DefaultFile") Then
            Dim defaultFileSmallIcon As Icon = ShellIcons.GetFileIcon("C:\Windows\System32\notepad.exe", True)
            Dim defaultFileLargeIcon As Icon = ShellIcons.GetFileIcon("C:\Windows\System32\notepad.exe", False)
            If defaultFileSmallIcon IsNot Nothing Then ImageList1.Images.Add("DefaultFile", defaultFileSmallIcon)
            If defaultFileLargeIcon IsNot Nothing Then ImageList2.Images.Add("DefaultFile", defaultFileLargeIcon)
        End If

        If TreeView1.Nodes.Count > 0 Then
            If TreeView1.Nodes(0).Tag IsNot Nothing Then
                UpdateAddressBar(TreeView1.Nodes(0).Tag.ToString())
            End If
        End If

        TreeView2.Nodes.Clear()
        Dim PinnedFolder As New TreeNode(Environment.UserName)
        With PinnedFolder
            For Each directory As String In System.IO.Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                Dim dirInfo As New System.IO.DirectoryInfo(directory)
                Dim subNode As New TreeNode(dirInfo.Name)
                subNode.Tag = dirInfo.FullName

                Dim folderIcon As Icon = ShellIcons.GetFileIcon(dirInfo.FullName, True)
                Dim iconKey As String = "Folder_" & dirInfo.FullName.Replace(":", "").Replace("\", "_")

                If folderIcon IsNot Nothing Then
                    If Not ImageList3.Images.ContainsKey(iconKey) Then
                        ImageList3.Images.Add(iconKey, folderIcon)
                    End If
                    subNode.ImageKey = iconKey
                    subNode.SelectedImageKey = iconKey
                Else
                    subNode.ImageKey = "DefaultFolder"
                    subNode.SelectedImageKey = "DefaultFolder"
                End If

                If (dirInfo.Attributes And FileAttributes.Hidden) = FileAttributes.Hidden AndAlso Not Properties.CheckBox1.Checked Then
                    Continue For
                End If
                .Nodes.Add(subNode)
            Next
            .Tag = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            .Expand()
        End With
        TreeView2.Nodes.Add(PinnedFolder)

        Dim IconsHandles(0) As IntPtr
        ExtractIconEx(Environment.GetFolderPath(Environment.SpecialFolder.Windows) & "\System32\shell32.dll", 15, Nothing, IconsHandles, 1)

        'Disk List
        Try
            Dim ico As Icon = Icon.FromHandle(IconsHandles(0)).Clone()
            tsBtnDiskList.Image = ico.ToBitmap
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        DestroyIcon(IconsHandles(0))

        LoadDirectoryContentsToListView(initialPath)
        SyncTreeViewWithFolder(initialPath)
        'History
        navigationHistory.Add(ComboBox1.Text)
        UpdateNavigationButtons()
    End Sub

    Private Sub LoadDrives()
        TreeView1.Nodes.Clear()

        For Each drive As System.IO.DriveInfo In System.IO.DriveInfo.GetDrives()
            If drive.IsReady Then
                Dim driveNode As New TreeNode(drive.Name) ' Zobrazíme i název disku
                driveNode.Tag = drive.RootDirectory.FullName ' Uložíme plnou cestu

                If Not drive.VolumeLabel = String.Empty Then
                    driveNode.Text = drive.Name & " [" & drive.VolumeLabel & "]"
                Else
                    driveNode.Text = drive.Name
                End If
                ' Získáme ikonu pro disk přímo z jeho cesty
                Dim driveIcon As Icon = ShellIcons.GetFileIcon(drive.RootDirectory.FullName, True) ' Použijeme GetFileIcon pro získání ikony disku
                Dim driveLargeIcon As Icon = ShellIcons.GetFileIcon(drive.RootDirectory.FullName, False)
                If driveIcon IsNot Nothing Then
                    Dim iconKey As String = "Drive_" & drive.Name.Replace(":\", "") ' Unikátní klíč pro disk (např. "Drive_C")
                    If Not ImageList4.Images.ContainsKey(iconKey) Then
                        ImageList4.Images.Add(iconKey, driveIcon)
                    End If
                    driveNode.ImageKey = iconKey
                    driveNode.SelectedImageKey = iconKey
                Else
                    driveNode.ImageKey = "DefaultFolder"
                    driveNode.SelectedImageKey = "DefaultFolder"
                End If

                TreeView1.Nodes.Add(driveNode)

                driveNode.Nodes.Add("Dummy")
            End If
        Next

        If Not ImageList4.Images.ContainsKey("DefaultFolder") Then
            Dim defaultFolderIcon As Icon = ShellIcons.GetFolderIcon(False)
            If defaultFolderIcon IsNot Nothing Then
                ImageList1.Images.Add("DefaultFolder", defaultFolderIcon)
            End If
        End If
    End Sub

    Private Sub TreeView1_BeforeExpand(sender As Object, e As TreeViewCancelEventArgs) Handles TreeView1.BeforeExpand
        If e.Node.Nodes.Count = 1 AndAlso e.Node.Nodes(0).Text = "Dummy" Then
            e.Node.Nodes.Clear()
            LoadSubDirectories(e.Node)
        End If
    End Sub

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect, TreeView2.AfterSelect
        On Error Resume Next
        If e.Node IsNot Nothing AndAlso e.Node.Tag IsNot Nothing Then
            Dim selectedPath = e.Node.Tag.ToString
            LoadDirectoryContentsToListView(selectedPath)
            SyncTreeViewWithFolder(selectedPath)

            If ComboBox1.Text <> selectedPath Then
                ComboBox1.Text = selectedPath
                If Not ComboBox1.Items.Contains(selectedPath) Then
                    ComboBox1.Items.Insert(0, selectedPath)
                End If
            End If

            UpdateAddressBar(selectedPath) ' Aktualizujeme adresní řádek

            If BackBtnUsed = True Then
                BackBtnUsed = False
            Else
                If currentHistoryIndex = navigationHistory.Count - 1 Then
                    If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                        navigationHistory.Add(selectedPath)
                        currentHistoryIndex += 1
                    End If
                    UpdateNavigationButtons()
                Else
                    For i = currentHistoryIndex To navigationHistory.Count - 2
                        navigationHistory.RemoveAt(navigationHistory.Count - 1)
                    Next
                    If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                        navigationHistory.Add(selectedPath)
                        currentHistoryIndex += 1
                    End If
                    UpdateNavigationButtons()
                End If
            End If

        End If
    End Sub

    ' Nová metoda pro načtení obsahu složky do ListView
    Private Sub LoadSubDirectories(parentNode As TreeNode)
        Dim parentPath As String = parentNode.Tag.ToString()
        'ListView1.Items.Clear() ' Vyčistíme ListView před načtením nového obsahu

        Try
            ' Načítání podsložek
            For Each directory As String In System.IO.Directory.GetDirectories(parentPath)
                Dim dirInfo As New System.IO.DirectoryInfo(directory)
                Dim subNode As New TreeNode(dirInfo.Name)
                subNode.Tag = dirInfo.FullName

                Dim folderIcon As Icon = ShellIcons.GetFileIcon(dirInfo.FullName, True)
                Dim iconKey As String = "Folder_" & dirInfo.FullName.Replace(":", "").Replace("\", "_")

                If folderIcon IsNot Nothing Then
                    If Not ImageList4.Images.ContainsKey(iconKey) Then
                        ImageList4.Images.Add(iconKey, folderIcon)
                    End If
                    subNode.ImageKey = iconKey
                    subNode.SelectedImageKey = iconKey
                Else
                    subNode.ImageKey = "DefaultFolder"
                    subNode.SelectedImageKey = "DefaultFolder"
                End If

                parentNode.Nodes.Add(subNode)
                subNode.Nodes.Add("Dummy")
            Next

            ' Načítání souborů
            For Each file As String In System.IO.Directory.GetFiles(parentPath)
                Dim fileInfo As New System.IO.FileInfo(file)
                Dim fileNode As New TreeNode(fileInfo.Name)
                fileNode.Tag = fileInfo.FullName

                Dim fileIcon As Icon = ShellIcons.GetFileIcon(fileInfo.FullName, True)
                Dim fileLargeIcon As Icon = ShellIcons.GetFileIcon(fileInfo.FullName, False)

                Dim iconKey As String = fileInfo.Extension.ToLower()
                If iconKey = "" Then iconKey = "NoExtension"

                If fileIcon IsNot Nothing Then
                    If Not ImageList4.Images.ContainsKey(iconKey) Then
                        'ImageList4.Images.Add(iconKey, fileIcon)
                    End If
                    fileNode.ImageKey = iconKey
                    fileNode.SelectedImageKey = iconKey
                Else
                    fileNode.ImageKey = "DefaultFile"
                    fileNode.SelectedImageKey = "DefaultFile"
                End If

                If fileLargeIcon IsNot Nothing Then
                    If Not ImageList4.Images.ContainsKey(iconKey) Then
                        'ImageList2.Images.Add(iconKey, fileLargeIcon)
                    End If
                End If
            Next

        Catch ex As UnauthorizedAccessException
            Console.WriteLine("Přístup odepřen: " & parentPath)
        Catch ex As Exception
            Console.WriteLine("Chyba při načítání obsahu složky: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadDirectoryContentsToListView(ByVal directoryPath As String)
        ListView1.Items.Clear()

        ImageList1.Images.Clear()
        ImageList2.Images.Clear()
        Try
            For Each directory As String In System.IO.Directory.GetDirectories(directoryPath)
                Dim dirInfo As New System.IO.DirectoryInfo(directory)

                If (dirInfo.Attributes And System.IO.FileAttributes.Hidden) = System.IO.FileAttributes.Hidden Then
                    'Continue For
                End If

                Dim lvItem As New ListViewItem(dirInfo.Name)
                lvItem.Tag = dirInfo.FullName
                Dim iconKey As String = "Folder_" & dirInfo.FullName.Replace(":", "_").Replace("\", "_").Replace("/", "_")
                lvItem.ImageKey = iconKey

                If Not ImageList1.Images.ContainsKey(lvItem.ImageKey) Then
                    ' Pro složky stále preferujeme GetFileIcon s plnou cestou
                    Dim folderSmallIcon As Icon = ShellIcons.GetFileIcon(dirInfo.FullName, True)
                    Dim folderLargeIcon As Icon = ShellIcons.GetFileIcon(dirInfo.FullName, False)

                    If folderSmallIcon IsNot Nothing Then
                        ImageList1.Images.Add(lvItem.ImageKey, folderSmallIcon)
                    End If
                    If folderLargeIcon IsNot Nothing Then
                        ImageList2.Images.Add(lvItem.ImageKey, folderLargeIcon)
                    End If
                    If folderSmallIcon Is Nothing AndAlso folderLargeIcon Is Nothing Then
                        lvItem.ImageKey = "DefaultFolder"
                    End If
                End If

                lvItem.SubItems.Add("Folder")
                lvItem.SubItems.Add(dirInfo.LastWriteTime.ToString())
                lvItem.SubItems.Add("")
                ListView1.Items.Add(lvItem)
            Next
            ' #endregion

            ' #region Zpracování souborů
            For Each file As String In System.IO.Directory.GetFiles(directoryPath)
                Dim fileInfo As New System.IO.FileInfo(file)

                If (fileInfo.Attributes And System.IO.FileAttributes.Hidden) = System.IO.FileAttributes.Hidden Then
                    'Continue For
                End If

                Dim lvItem As New ListViewItem(fileInfo.Name)
                lvItem.Tag = fileInfo.FullName

                ' Klíč pro soubory je nyní unikátní (plná cesta souboru)
                Dim fileIconKey As String = "File_" & fileInfo.FullName.Replace(":", "_").Replace("\", "_").Replace("/", "_").Replace(" ", "_").ToLower()
                lvItem.ImageKey = fileIconKey

                If Not ImageList1.Images.ContainsKey(lvItem.ImageKey) Then
                    Dim fileSmallIcon As Icon = Nothing
                    Dim fileLargeIcon As Icon = Nothing

                    'fileSmallIcon = ShellIcons.GetTypeIcon(fileInfo.Extension, True)
                    'fileLargeIcon = ShellIcons.GetTypeIcon(fileInfo.Extension, False)

                    ' Priorita 2: ShellIcons.GetFileIcon s plnou cestou jako záložní řešení, pokud GetTypeIcon selhalo
                    If fileSmallIcon Is Nothing Then
                        fileSmallIcon = ShellIcons.GetFileIcon(fileInfo.FullName, True)
                    End If
                    If fileLargeIcon Is Nothing Then
                        fileLargeIcon = ShellIcons.GetFileIcon(fileInfo.FullName, False)
                    End If

                    ' Přidáme ikony do ImageListů
                    If fileSmallIcon IsNot Nothing Then
                        ImageList1.Images.Add(fileIconKey, fileSmallIcon)
                    End If
                    If fileLargeIcon IsNot Nothing Then
                        ImageList2.Images.Add(fileIconKey, fileLargeIcon)
                    End If

                    ' Pokud se stále nepodařilo získat žádnou ikonu, použij defaultní
                    If fileSmallIcon Is Nothing AndAlso fileLargeIcon Is Nothing Then
                        lvItem.ImageKey = "DefaultFile"
                    End If
                End If

                lvItem.ImageKey = fileIconKey ' ListViewItem nyní odkazuje na unikátní klíč ikony

                lvItem.SubItems.Add(If(String.IsNullOrEmpty(fileInfo.Extension), "Soubor bez přípony", fileInfo.Extension.ToUpper().Replace(".", "") & " File"))
                lvItem.SubItems.Add(fileInfo.LastWriteTime.ToString())
                lvItem.SubItems.Add(fileInfo.Length.ToString("N0") & " Bytes")
                ListView1.Items.Add(lvItem)
            Next
            Form1.ToolStripStatusLabel1.Text = ListView1.Items.Count & " items"
        Catch ex As UnauthorizedAccessException
            Console.WriteLine("Přístup odepřen: " & directoryPath)
        Catch ex As Exception
            Console.WriteLine("Chyba při načítání obsahu ListView: " & ex.Message)
        End Try
    End Sub

    Private Sub SyncTreeViewWithFolder(ByVal folderPath As String)
        Dim currentPath As String = folderPath
        Dim pathParts As New List(Of String)

        While Not String.IsNullOrEmpty(currentPath) AndAlso Directory.Exists(currentPath)
            pathParts.Insert(0, currentPath)
            Dim parentDir As DirectoryInfo = Directory.GetParent(currentPath)
            If parentDir IsNot Nothing Then
                currentPath = parentDir.FullName
            Else
                Exit While
            End If
        End While

        Dim currentNodes As TreeNodeCollection = TreeView1.Nodes
        Dim foundNode As TreeNode = Nothing

        For Each partPath As String In pathParts
            foundNode = Nothing
            For Each node As TreeNode In currentNodes
                If node.Tag IsNot Nothing AndAlso node.Tag.ToString().Equals(partPath, StringComparison.OrdinalIgnoreCase) Then
                    foundNode = node
                    Exit For
                End If
            Next

            If foundNode IsNot Nothing Then
                If foundNode.Nodes.Count = 1 AndAlso foundNode.Nodes(0).Text = "Dummy" Then
                    foundNode.Nodes.Clear()
                    LoadSubDirectories(foundNode)
                End If
                foundNode.Expand()
                currentNodes = foundNode.Nodes
            Else
                Exit For
            End If
        Next

        If foundNode IsNot Nothing Then
            TreeView1.SelectedNode = foundNode
            foundNode.EnsureVisible()
        End If
    End Sub

    Private Sub UpdateAddressBar(ByVal path As String)
        ToolStrip1.Items.Clear()

        Dim parts As New List(Of String)
        Dim currentPathBuilder As New System.Text.StringBuilder()

        Dim rootPath As String = System.IO.Path.GetPathRoot(path)
        If Not String.IsNullOrEmpty(rootPath) AndAlso Directory.Exists(rootPath) Then
            parts.Add(rootPath)
        End If

        Dim remainingPath As String = path.Substring(System.IO.Path.GetPathRoot(path).Length)
        Dim subParts As String() = remainingPath.Split(System.IO.Path.DirectorySeparatorChar)

        For Each subPart As String In subParts
            If Not String.IsNullOrEmpty(subPart) Then
                If currentPathBuilder.Length > 0 AndAlso Not currentPathBuilder.ToString().EndsWith(System.IO.Path.DirectorySeparatorChar) Then
                    currentPathBuilder.Append(System.IO.Path.DirectorySeparatorChar)
                End If
                currentPathBuilder.Append(subPart)
                parts.Add(System.IO.Path.Combine(rootPath, currentPathBuilder.ToString()))
            End If
        Next

        For i As Integer = 0 To parts.Count - 1
            Dim currentSegmentPath As String = parts(i)
            Dim displaySegment As String

            If i = 0 AndAlso System.IO.Path.IsPathRooted(currentSegmentPath) Then
                displaySegment = currentSegmentPath
            Else
                displaySegment = New DirectoryInfo(currentSegmentPath).Name
                If String.IsNullOrEmpty(displaySegment) AndAlso System.IO.Path.IsPathRooted(currentSegmentPath) Then
                    displaySegment = currentSegmentPath
                End If
            End If

            Dim dropDownButton As New ToolStripDropDownButton(displaySegment)
            dropDownButton.Tag = currentSegmentPath
            Dim segmentIcon As Icon = ShellIcons.GetFileIcon(currentSegmentPath, True)
            If segmentIcon IsNot Nothing Then
                dropDownButton.Image = segmentIcon.ToBitmap()
            Else
                If ImageList1.Images.ContainsKey("DefaultFolder") Then
                    dropDownButton.Image = ImageList1.Images("DefaultFolder")
                End If
            End If

            AddHandler dropDownButton.Click, AddressOf AddressBarSegment_Click
            AddHandler dropDownButton.DropDownOpening, AddressOf AddressBarDropDown_Opening
            ToolStrip1.Items.Add(dropDownButton)

            If i < parts.Count - 1 Then
                ToolStrip1.Items.Add(New ToolStripSeparator())
            End If

            ' MDI Manipulation
            If Directory.Exists(ComboBox1.Text) = True Then
                Dim DI As New DirectoryInfo(ComboBox1.Text)
                Me.Text = DI.Name
                Me.Icon = ShellIcons.GetFileIcon(ComboBox1.Text, True)
            Else
                Me.Text = "Folder"
            End If
        Next
    End Sub

    Private Sub AddressBarDropDown_Opening(sender As Object, e As EventArgs)
        Dim dropDownButton As ToolStripDropDownButton = CType(sender, ToolStripDropDownButton)
        dropDownButton.DropDown.Items.Clear()

        If dropDownButton.Tag IsNot Nothing Then
            Dim parentPath As String = dropDownButton.Tag.ToString()
            If Directory.Exists(parentPath) Then
                PopulateSubFoldersToDropdown(parentPath, dropDownButton.DropDown.Items)
            End If
        End If
    End Sub

    Private Sub PopulateSubFoldersToDropdown(ByVal parentPath As String, ByVal itemsCollection As ToolStripItemCollection)
        Try
            For Each directory As String In System.IO.Directory.GetDirectories(parentPath)
                Dim dirInfo As New DirectoryInfo(directory)
                Dim dropDownItem As New ToolStripMenuItem(dirInfo.Name)
                dropDownItem.Tag = dirInfo.FullName

                AddHandler dropDownItem.Click, AddressOf AddressBarDropDownItem_Click

                Dim itemIcon As Icon = ShellIcons.GetFileIcon(dirInfo.FullName, True)
                If itemIcon IsNot Nothing Then
                    dropDownItem.Image = itemIcon.ToBitmap()
                Else
                    If ImageList1.Images.ContainsKey("DefaultFolder") Then
                        dropDownItem.Image = ImageList1.Images("DefaultFolder")
                    End If
                End If

                If HasSubDirectories(dirInfo.FullName) Then
                    Dim dummyItem As New ToolStripMenuItem("Načítání...")
                    dropDownItem.DropDown.Items.Add(dummyItem)
                    AddHandler dropDownItem.DropDownOpening, AddressOf NestedDropDown_Opening
                End If

                itemsCollection.Add(dropDownItem)
            Next

        Catch ex As UnauthorizedAccessException
            Console.WriteLine("Přístup odepřen při načítání podsložek pro dropdown: " & parentPath)
        Catch ex As Exception
            Console.WriteLine("Chyba při načítání podsložek pro dropdown: " & ex.Message)
        End Try
    End Sub

    Private Sub NestedDropDown_Opening(sender As Object, e As EventArgs)
        Dim parentMenuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)

        If parentMenuItem.DropDown.Items.Count = 1 AndAlso parentMenuItem.DropDown.Items(0).Text = "Načítání..." Then
            parentMenuItem.DropDown.Items.Clear()

            If parentMenuItem.Tag IsNot Nothing Then
                Dim targetPath As String = parentMenuItem.Tag.ToString()
                If Directory.Exists(targetPath) Then
                    PopulateSubFoldersToDropdown(targetPath, parentMenuItem.DropDown.Items)
                End If
            End If
        End If
    End Sub

    Private Function HasSubDirectories(ByVal path As String) As Boolean
        Try
            Return Directory.GetDirectories(path).Length > 0
        Catch ex As UnauthorizedAccessException
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub AddressBarSegment_Click(sender As Object, e As EventArgs)
        Dim clickedItem As ToolStripItem = CType(sender, ToolStripItem)
        If clickedItem.Tag IsNot Nothing Then
            Dim targetPath As String = clickedItem.Tag.ToString()
            If Directory.Exists(targetPath) Then
                SyncTreeViewWithFolder(targetPath)
                If BackBtnUsed = True Then
                    BackBtnUsed = False
                Else
                    If currentHistoryIndex = navigationHistory.Count - 1 Then
                        If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                            navigationHistory.Add(targetPath)
                            currentHistoryIndex += 1
                        End If
                        UpdateNavigationButtons()
                    Else
                        For i As Integer = currentHistoryIndex To navigationHistory.Count - 2
                            navigationHistory.RemoveAt(navigationHistory.Count - 1)
                        Next
                        If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                            navigationHistory.Add(targetPath)
                            currentHistoryIndex += 1
                        End If
                        UpdateNavigationButtons()
                    End If
                End If
            End If
        End If
    End Sub

    ' Obsluha otevírání dropdown menu pro segment v adresním řádku
    Private Sub AddressBarDropDown_Opening11(sender As Object, e As EventArgs)
        Dim dropDownButton As ToolStripDropDownButton = CType(sender, ToolStripDropDownButton)
        dropDownButton.DropDown.Items.Clear() ' Vyčistíme před přidáním nových položek

        If dropDownButton.Tag IsNot Nothing Then
            Dim parentPath As String = dropDownButton.Tag.ToString()
            Try
                For Each directory As String In System.IO.Directory.GetDirectories(parentPath)
                    Dim dirInfo As New DirectoryInfo(directory)
                    Dim dropDownItem As New ToolStripMenuItem(dirInfo.Name)
                    dropDownItem.Tag = dirInfo.FullName ' Uložíme plnou cestu
                    AddHandler dropDownItem.Click, AddressOf AddressBarDropDownItem_Click

                    ' Získání ikony pro položku dropdownu
                    Dim itemIcon As Icon = ShellIcons.GetFileIcon(dirInfo.FullName, True)
                    If itemIcon IsNot Nothing Then
                        dropDownItem.Image = itemIcon.ToBitmap()
                    Else
                        If ImageList1.Images.ContainsKey("DefaultFolder") Then
                            dropDownItem.Image = ImageList1.Images("DefaultFolder")
                        End If
                    End If
                    dropDownButton.DropDown.Items.Add(dropDownItem)
                Next
            Catch ex As UnauthorizedAccessException
                Console.WriteLine("Přístup odepřen při načítání dropdownu: " & parentPath)
            Catch ex As Exception
                Console.WriteLine("Chyba při načítání dropdownu: " & ex.Message)
            End Try
        End If
    End Sub

    ' Obsluha kliknutí na položku v dropdown menu adresního řádku
    Private Sub AddressBarDropDownItem_Click(sender As Object, e As EventArgs)
        Dim clickedItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        If clickedItem.Tag IsNot Nothing Then
            Dim targetPath As String = clickedItem.Tag.ToString()
            If Directory.Exists(targetPath) Then
                SyncTreeViewWithFolder(targetPath)
                If BackBtnUsed = True Then
                    BackBtnUsed = False
                Else
                    If currentHistoryIndex = navigationHistory.Count - 1 Then
                        If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                            navigationHistory.Add(targetPath)
                            currentHistoryIndex += 1
                        End If
                        UpdateNavigationButtons()
                    Else
                        For i As Integer = currentHistoryIndex To navigationHistory.Count - 2
                            navigationHistory.RemoveAt(navigationHistory.Count - 1)
                        Next
                        If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                            navigationHistory.Add(targetPath)
                            currentHistoryIndex += 1
                        End If
                        UpdateNavigationButtons()
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub LargeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LargeToolStripMenuItem.Click
        ListView1.View = View.LargeIcon
        LoadDirectoryContentsToListView(ComboBox1.Text)
    End Sub

    Private Sub SmallToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SmallToolStripMenuItem.Click
        ListView1.View = View.SmallIcon
        LoadDirectoryContentsToListView(ComboBox1.Text)
    End Sub

    Private Sub DetailsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DetailsToolStripMenuItem.Click
        ListView1.View = View.Details
        LoadDirectoryContentsToListView(ComboBox1.Text)
    End Sub

    Private Sub ListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ListToolStripMenuItem.Click
        ListView1.View = View.List
        LoadDirectoryContentsToListView(ComboBox1.Text)
    End Sub

    Private Sub TileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TilesToolStripMenuItem.Click
        ListView1.View = View.Tile
        LoadDirectoryContentsToListView(ComboBox1.Text)
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged

    End Sub
    Private Sub ShowContextMenuForSelection(paths As List(Of String), clickPos As Point)
        'future
    End Sub
    Dim LastFocusedListViewItemTag As String = String.Empty
    Dim LastFocusedListViewItemLocation As Point
    Private Sub ListView1_MouseUp(sender As Object, e As MouseEventArgs) Handles ListView1.MouseUp
        If e.Button = MouseButtons.Right Then
            Dim hitTest As ListViewHitTestInfo = ListView1.HitTest(e.Location)
            Dim selectedPaths As New List(Of String)

            If hitTest.Item IsNot Nothing Then
                LastFocusedListViewItemTag = hitTest.Item.Tag
                OpenInMDIToolStripMenuItem.Tag = hitTest.Item.Tag
                LastFocusedListViewItemLocation = hitTest.Item.Position
                For Each lvItem As ListViewItem In ListView1.SelectedItems
                    If lvItem.Tag IsNot Nothing Then
                        selectedPaths.Add(lvItem.Tag.ToString())
                    End If
                Next
            Else

            End If

            If selectedPaths.Count = 1 Then
                If File.Exists(ListView1.FocusedItem.Tag) Then
                    Dim path = ListView1.FocusedItem.Tag.Trim()

                    Dim shell As New Shell()
                    Dim folder = shell.NameSpace(System.IO.Path.GetDirectoryName(path))
                    Dim item = folder.ParseName(System.IO.Path.GetFileName(path))

                    Dim cms = New ContextMenuStrip()
                    Dim bmpIcon = GetIconBitmap(path)
                    Dim verbs = item.Verbs().Cast(Of FolderItemVerb)().ToList()

                    verbs = verbs.Where(Function(v) Not String.IsNullOrWhiteSpace(v.Name.Replace("&"c, ""))).ToList()

                    Dim groupBreakIndex As Integer = Math.Min(3, verbs.Count)

                    Dim icoShield = GetStockIconBmp(SHSTOCKICONID.SIID_SHIELD)
                    Dim icoProperties = GetStockIconBmp(SHSTOCKICONID.SIID_PROPERTIES)
                    Dim icoCopy = GetStockIconBmp(SHSTOCKICONID.SIID_COPY)
                    Dim icoCut = GetStockIconBmp(SHSTOCKICONID.SIID_CUT)
                    Dim icoDelete = GetStockIconBmp(SHSTOCKICONID.SIID_DELETE)
                    Dim icoRename = GetStockIconBmp(SHSTOCKICONID.SIID_RENAME)

                    For i As Integer = 0 To verbs.Count - 1
                        If i = groupBreakIndex Then
                            cms.Items.Add(New ToolStripSeparator() With {.Enabled = False})
                        End If
                        Dim verb = verbs(i)
                        Dim title = verb.Name.Replace("&"c, "")
                        If Name = "" Then Continue For

                        Dim tsmi = New ToolStripMenuItem(title)
                        With tsmi
                            If i = 0 Then
                                .Font = New Font(tsmi.Font, FontStyle.Bold)
                                tsmi.Image = bmpIcon
                            End If
                            If title = "Open" OrElse title = "Mount" Then

                            ElseIf title = "Run as administrator" Then
                                tsmi.Image = icoShield
                            ElseIf title = "Properties" Then
                                tsmi.Image = icoProperties
                            ElseIf title = "Copy" Then
                                tsmi.Image = icoCopy
                            ElseIf title = "Cut" Then
                                tsmi.Image = icoCut
                            ElseIf title = "Delete" Then
                                tsmi.Image = icoDelete
                            ElseIf title = "Rename" Then
                                tsmi.Image = icoRename
                            End If
                        End With

                        AddHandler tsmi.Click, Sub(s, eventargs)
                                                   Try
                                                       If verb.Name.Replace("&"c, "") = "Copy as path" Then
                                                           Dim fi As New FileInfo(ListView1.FocusedItem.Tag)
                                                           Clipboard.SetText(fi.FullName)
                                                       ElseIf verb.Name.Replace("&"c, "") = "Rename" Then
                                                           hitTest.Item.BeginEdit()
                                                       Else
                                                           verb.DoIt()
                                                       End If
                                                   Catch ex As Exception
                                                       MsgBox(ex.Message)
                                                   End Try
                                               End Sub
                        cms.Items.Add(tsmi)
                    Next
                    cms.Show(MousePosition)
                ElseIf Directory.Exists(ListView1.FocusedItem.Tag) Then
                    Dim path = ListView1.FocusedItem.Tag.Trim()

                    Dim shell As New Shell()
                    Dim targetFolder As Folder
                    Dim item As FolderItem = Nothing

                    If Directory.Exists(path) Then
                        targetFolder = shell.NameSpace(path)
                    Else
                        targetFolder = shell.NameSpace(System.IO.Path.GetDirectoryName(path))
                        item = targetFolder.ParseName(System.IO.Path.GetFileName(path))
                    End If

                    If targetFolder Is Nothing Then
                        MessageBox.Show("Nelze načíst shell folder.")
                        Return
                    End If

                    Dim cms = New ContextMenuStrip()

                    Dim verbsSource = If(item IsNot Nothing, item.Verbs(), targetFolder.Self.Verbs())
                    Dim i As Integer = 0

                    Dim icoShield = GetStockIconBmp(SHSTOCKICONID.SIID_SHIELD)
                    Dim icoProperties = GetStockIconBmp(SHSTOCKICONID.SIID_PROPERTIES)
                    Dim icoCopy = GetStockIconBmp(SHSTOCKICONID.SIID_COPY)
                    Dim icoCut = GetStockIconBmp(SHSTOCKICONID.SIID_CUT)
                    Dim icoDelete = GetStockIconBmp(SHSTOCKICONID.SIID_DELETE)
                    Dim icoRename = GetStockIconBmp(SHSTOCKICONID.SIID_RENAME)

                    Dim groupBreakIndex As Integer = Math.Min(3, verbsSource.Count)

                    For Each v As FolderItemVerb In verbsSource
                        If IsDriveRoot(path) = False Then
                            If i = groupBreakIndex Then
                                cms.Items.Add(New ToolStripSeparator() With {.Enabled = False})
                            End If
                        End If

                        Dim disp = CleanDisplayName(v.Name)
                        If v.Name = "" Then Continue For

                        Dim tsmi = New ToolStripMenuItem(disp) With {.Tag = v}
                        With tsmi
                            If i = 0 Then
                                .Font = New Font(tsmi.Font, FontStyle.Bold)
                            End If
                            If disp = "Open" OrElse disp = "Mount" Then

                            ElseIf disp = "Run as administrator" Then
                                tsmi.Image = icoShield
                            ElseIf disp = "Properties" Then
                                tsmi.Image = icoProperties
                            ElseIf disp = "Copy" Then
                                tsmi.Image = icoCopy
                            ElseIf disp = "Cut" Then
                                tsmi.Image = icoCut
                            ElseIf disp = "Delete" Then
                                tsmi.Image = icoDelete
                            ElseIf disp = "Rename" Then
                                tsmi.Image = icoRename
                            End If
                        End With

                        AddHandler tsmi.Click, Sub(s, ea)
                                                   Try
                                                       If CleanDisplayName(v.Name) = "Copy as path" Then
                                                           Dim fi As New DirectoryInfo(ListView1.FocusedItem.Tag)
                                                           Clipboard.SetText(fi.FullName)
                                                       ElseIf CleanDisplayName(v.Name) = "Rename" Then
                                                           hitTest.Item.BeginEdit()
                                                       Else
                                                           CType(DirectCast(s, ToolStripMenuItem).Tag, FolderItemVerb).DoIt()
                                                       End If
                                                   Catch ex As Exception
                                                       MsgBox(ex.Message)
                                                   End Try
                                               End Sub
                        If i = 1 Then
                            cms.Items.Add(OpenInMDIToolStripMenuItem)
                        End If
                        cms.Items.Add(tsmi)
                        i += 1
                    Next
                    cms.Show(MousePosition)
                End If

            ElseIf selectedPaths.Count > 1 Then
                Dim paths = ListView1.SelectedItems _
                    .Cast(Of ListViewItem)() _
                    .Select(Function(it) it.Tag.ToString()) _
                    .ToList()
                ShowContextMenuForSelection(paths, e.Location)
            ElseIf selectedPaths.Count = 0 Then
                Dim path = ComboBox1.Text.Trim()
                If Not File.Exists(path) AndAlso Not Directory.Exists(path) Then
                    MessageBox.Show("Cesta neexistuje: " & path)
                    Return
                End If

                Dim shell As New Shell()
                Dim targetFolder As Folder
                Dim item As FolderItem = Nothing

                If Directory.Exists(path) Then
                    targetFolder = shell.NameSpace(path)
                Else
                    targetFolder = shell.NameSpace(System.IO.Path.GetDirectoryName(path))
                    item = targetFolder.ParseName(System.IO.Path.GetFileName(path))
                End If

                If targetFolder Is Nothing Then
                    MessageBox.Show("Nelze načíst shell folder.")
                    Return
                End If

                Dim cms = New ContextMenuStrip()

                Dim verbsSource = If(item IsNot Nothing, item.Verbs(), targetFolder.Self.Verbs())
                Dim i As Integer = 0

                Dim icoShield = GetStockIconBmp(SHSTOCKICONID.SIID_SHIELD)
                Dim icoProperties = GetStockIconBmp(SHSTOCKICONID.SIID_PROPERTIES)
                Dim icoCopy = GetStockIconBmp(SHSTOCKICONID.SIID_COPY)
                Dim icoCut = GetStockIconBmp(SHSTOCKICONID.SIID_CUT)
                Dim icoDelete = GetStockIconBmp(SHSTOCKICONID.SIID_DELETE)
                Dim icoRename = GetStockIconBmp(SHSTOCKICONID.SIID_RENAME)

                Dim groupBreakIndex As Integer = Math.Min(3, verbsSource.Count)

                For Each v As FolderItemVerb In verbsSource
                    If IsDriveRoot(path) = False Then
                        If i = groupBreakIndex Then
                            cms.Items.Add(New ToolStripSeparator() With {.Enabled = False})
                        End If
                    End If

                    Dim disp = CleanDisplayName(v.Name)
                    If v.Name = "" Then Continue For

                    Dim tsmi = New ToolStripMenuItem(disp) With {.Tag = v}
                    With tsmi
                        If i = 0 Then
                            .Font = New Font(tsmi.Font, FontStyle.Bold)
                        End If
                        If disp = "Open" OrElse disp = "Mount" Then

                        ElseIf disp = "Run as administrator" Then
                            tsmi.Image = icoShield
                        ElseIf disp = "Properties" Then
                            tsmi.Image = icoProperties
                        ElseIf disp = "Copy" Then
                            tsmi.Image = icoCopy
                        ElseIf disp = "Cut" Then
                            tsmi.Image = icoCut
                        ElseIf disp = "Delete" Then
                            tsmi.Image = icoDelete
                        ElseIf disp = "Rename" Then
                            tsmi.Image = icoRename
                        End If
                    End With

                    AddHandler tsmi.Click, Sub(s, ea)
                                               Try
                                                   If CleanDisplayName(v.Name) = "Copy as path" Then
                                                       Dim fi As New DirectoryInfo(ComboBox1.Text)
                                                       Clipboard.SetText(fi.FullName)
                                                   Else
                                                       CType(DirectCast(s, ToolStripMenuItem).Tag, FolderItemVerb).DoIt()
                                                   End If
                                               Catch ex As Exception
                                                   MsgBox(ex.Message)
                                               End Try
                                           End Sub
                    If i = 0 Then
                        cms.Items.Add(ViewToolStripMenuItem)
                        cms.Items.Add(RefreshToolStripMenuItem)
                        cms.Items.Add(New ToolStripSeparator() With {.Enabled = False})
                    End If

                    cms.Items.Add(tsmi)
                    i += 1
                Next

                cms.Show(MousePosition)
            End If
        End If
    End Sub

    Private Sub TreeView1_MouseUp(sender As Object, e As MouseEventArgs) Handles TreeView1.MouseUp
        If e.Button = MouseButtons.Right Then
            Dim hitNode = TreeView1.GetNodeAt(e.X, e.Y)
            If hitNode IsNot Nothing AndAlso hitNode.Tag IsNot Nothing Then
                OpenInMDIToolStripMenuItem.Tag = hitNode.Tag.Trim()
                Dim path = hitNode.Tag.Trim()
                If Not File.Exists(path) AndAlso Not Directory.Exists(path) Then
                    MessageBox.Show("Cesta neexistuje: " & path)
                    Return
                End If

                Dim shell As New Shell
                Dim targetFolder As Folder
                Dim item As FolderItem = Nothing

                If Directory.Exists(path) Then
                    targetFolder = shell.NameSpace(path)
                Else
                    targetFolder = shell.NameSpace(IO.Path.GetDirectoryName(path))
                    item = targetFolder.ParseName(IO.Path.GetFileName(path))
                End If

                If targetFolder Is Nothing Then
                    MessageBox.Show("Nelze načíst shell folder.")
                    Return
                End If

                Dim cms = New ContextMenuStrip

                Dim verbsSource = If(item IsNot Nothing, item.Verbs, targetFolder.Self.Verbs())
                Dim i = 0

                Dim icoShield = GetStockIconBmp(SHSTOCKICONID.SIID_SHIELD)
                Dim icoProperties = GetStockIconBmp(SHSTOCKICONID.SIID_PROPERTIES)
                Dim icoCopy = GetStockIconBmp(SHSTOCKICONID.SIID_COPY)
                Dim icoCut = GetStockIconBmp(SHSTOCKICONID.SIID_CUT)
                Dim icoDelete = GetStockIconBmp(SHSTOCKICONID.SIID_DELETE)
                Dim icoRename = GetStockIconBmp(SHSTOCKICONID.SIID_RENAME)

                Dim groupBreakIndex As Integer = Math.Min(3, verbsSource.Count)

                For Each v As FolderItemVerb In verbsSource
                    If IsDriveRoot(path) = False Then
                        If i = groupBreakIndex Then
                            cms.Items.Add(New ToolStripSeparator With {.Enabled = False})
                        End If
                    End If

                    Dim disp = CleanDisplayName(v.Name)
                    If v.Name = "" Then Continue For

                    Dim tsmi = New ToolStripMenuItem(disp) With {.Tag = v}
                    With tsmi
                        If i = 0 Then
                            .Font = New Font(tsmi.Font, FontStyle.Bold)
                        End If
                        If disp = "Open" OrElse disp = "Mount" Then

                        ElseIf disp = "Run as administrator" Then
                            tsmi.Image = icoShield
                        ElseIf disp = "Properties" Then
                            tsmi.Image = icoProperties
                        ElseIf disp = "Copy" Then
                            tsmi.Image = icoCopy
                        ElseIf disp = "Cut" Then
                            tsmi.Image = icoCut
                        ElseIf disp = "Delete" Then
                            tsmi.Image = icoDelete
                        ElseIf disp = "Rename" Then
                            tsmi.Image = icoRename
                        End If
                    End With

                    AddHandler tsmi.Click, Sub(s, ea)
                                               Try
                                                   If CleanDisplayName(v.Name) = "Copy as path" Then
                                                       Dim fi As New DirectoryInfo(hitNode.Tag)
                                                       Clipboard.SetText(fi.FullName)
                                                   ElseIf CleanDisplayName(v.Name) = "Rename" Then
                                                       hitNode.BeginEdit()
                                                   Else
                                                       CType(DirectCast(s, ToolStripMenuItem).Tag, FolderItemVerb).DoIt()
                                                   End If
                                               Catch ex As Exception
                                                   MsgBox(ex.Message)
                                               End Try
                                           End Sub
                    cms.Items.Add(tsmi)
                    If i = 0 Then
                        cms.Items.Add(OpenInMDIToolStripMenuItem)
                        cms.Items.Add(New ToolStripSeparator With {.Enabled = False})
                    End If
                    i += 1
                Next
                cms.Show(MousePosition)
            End If
        End If
    End Sub

    Private Const FOLDERID_Startup As String = "{B97D2CEB-5174-4C26-8D2C-361E8DCDA0F0}"
    Private Const FOLDERID_Programs As String = "{A77F5D77-2E2B-44C3-A6A2-AB1019F2B54D}"
    Private Const FOLDERID_Desktop As String = "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}"
    Private Const FOLDERID_Documents As String = "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}"
    Private Const FOLDERID_Downloads As String = "{374DE290-123F-4565-9164-39C4925E467B}"
    Private Const FOLDERID_Music As String = "{4BD8D571-6D19-48D3-BE97-4222200CE525}"
    Private Const FOLDERID_Pictures As String = "{33EE9AD3-F588-4FD9-8979-4EAE8D437C0A}"
    Private Const FOLDERID_Videos As String = "{18991DAB-9D59-4B5C-A05A-29A65AEEDC19}"
    Private Const FOLDERID_Profile As String = "{5E6C858F-0E22-4767-BCE7-EEDC0CD1DEDC}" ' User Profile Folder (C:\Users\username)

    ' Toto je metoda z Windows API pro získání cest ke známým složkám
    <DllImport("shell32.dll", CharSet:=CharSet.Unicode, ExactSpelling:=True, PreserveSig:=False)>
    Private Shared Function SHGetKnownFolderPath(ByVal rfid As Guid, ByVal dwFlags As UInteger, ByVal hToken As IntPtr, <Out()> ByRef pszPath As String) As Object
    End Function


    ' ... (Form1_Load a ostatní metody LoadDrives, LoadSubDirectories, FormatByteSize,
    '      TreeView1_AfterSelect, LoadDirectoryContentsToListView, SyncTreeViewWithFolder
    '      zůstávají stejné jako v předchozí odpovědi) ...

    ' ***** AKTUALIZOVANÁ UDÁLOST PRO COMBOBOX *****
    Private Sub ComboBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim inputPath As String = ComboBox1.Text.Trim()
            Dim resolvedPath As String = String.Empty

            If String.IsNullOrWhiteSpace(inputPath) Then
                MessageBox.Show("Prosím zadejte platnou cestu.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                e.Handled = True
                e.SuppressKeyPress = True
                Return
            End If

            Try
                If inputPath.ToLower().StartsWith("shell:") Then
                    Dim shellCommand As String = inputPath.Substring("shell:".Length).Trim()
                    resolvedPath = GetPathFromShellCommand(shellCommand)
                End If

                If String.IsNullOrEmpty(resolvedPath) Then
                    resolvedPath = Environment.ExpandEnvironmentVariables(inputPath)
                End If

                If String.IsNullOrEmpty(resolvedPath) Then
                    resolvedPath = inputPath
                End If

                If System.IO.Directory.Exists(resolvedPath) Then
                    If Not ComboBox1.Items.Contains(resolvedPath) Then
                        ComboBox1.Items.Insert(0, resolvedPath)
                    End If
                    ComboBox1.Text = resolvedPath

                    ToolStrip1.Visible = True
                    ComboBox1.Visible = False
                    LoadDirectoryContentsToListView(resolvedPath)
                    SyncTreeViewWithFolder(resolvedPath)
                    If BackBtnUsed = True Then
                        BackBtnUsed = False
                    Else
                        If currentHistoryIndex = navigationHistory.Count - 1 Then
                            If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                                navigationHistory.Add(resolvedPath)
                                currentHistoryIndex += 1
                            End If
                            UpdateNavigationButtons()
                        Else
                            For i As Integer = currentHistoryIndex To navigationHistory.Count - 2
                                navigationHistory.RemoveAt(navigationHistory.Count - 1)
                            Next
                            If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                                navigationHistory.Add(resolvedPath)
                                currentHistoryIndex += 1
                            End If
                            UpdateNavigationButtons()
                        End If
                    End If
                ElseIf System.IO.File.Exists(resolvedPath) Then
                    Dim PSI As New ProcessStartInfo(resolvedPath)
                    PSI.UseShellExecute = True
                    Process.Start(PSI)
                    ToolStrip1.Visible = True
                    ComboBox1.Visible = False
                Else
                    MessageBox.Show("Cesta '" & inputPath & "' (rozbaleno: '" & resolvedPath & "') neexistuje nebo není platná složka.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    ToolStrip1.Visible = False
                    ComboBox1.Visible = True
                    ComboBox1.SelectAll()
                End If

            Catch ex As Exception
                MessageBox.Show("Chyba při zpracování cesty: " & ex.Message, "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Public Function GetPathFromShellCommand(ByVal command As String) As String
        Select Case command.ToLower()
            Case "startup"
                Return GetKnownFolderPath(New Guid(FOLDERID_Startup))
            Case "programs"
                Return GetKnownFolderPath(New Guid(FOLDERID_Programs))
            Case "desktop"
                Return GetKnownFolderPath(New Guid(FOLDERID_Desktop))
            Case "documents"
                Return GetKnownFolderPath(New Guid(FOLDERID_Documents))
            Case "downloads"
                Return GetKnownFolderPath(New Guid(FOLDERID_Downloads))
            Case "music"
                Return GetKnownFolderPath(New Guid(FOLDERID_Music))
            Case "pictures"
                Return GetKnownFolderPath(New Guid(FOLDERID_Pictures))
            Case "videos"
                Return GetKnownFolderPath(New Guid(FOLDERID_Videos))
            Case "profile"
                Return GetKnownFolderPath(New Guid(FOLDERID_Profile))
            Case "appdata"
                Return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            Case "localappdata" ' shell:localappdata
                Return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Case "temp" ' shell:temp
                Return Path.GetTempPath()
            Case Else
                Try
                    Dim specialFolder As Environment.SpecialFolder
                    If [Enum].TryParse(command, True, specialFolder) Then
                        Return Environment.GetFolderPath(specialFolder)
                    End If
                Catch
                    ' Ignore parsing errors
                End Try
                Return String.Empty
        End Select
    End Function

    Private Function GetKnownFolderPath(ByVal folderId As Guid) As String
        Try
            Dim path As String = String.Empty
            SHGetKnownFolderPath(folderId, 0, IntPtr.Zero, path)
            Return path
        Catch ex As Exception
            Console.WriteLine("Chyba při získávání cesty pro Known Folder GUID " & folderId.ToString() & ": " & ex.Message)
            Return String.Empty
        End Try
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click, RefreshToolStripMenuItem.Click
        If Button1.Image Is My.Resources.RefreshBlackExtraSmall Then
            LoadDirectoryContentsToListView(ComboBox1.Text)
        ElseIf Button1.Image Is My.Resources.CancelBlackExtraSmall Then

        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged

    End Sub

    Private Sub ComboBox1_LostFocus(sender As Object, e As EventArgs) Handles ComboBox1.LostFocus
        ComboBox1.Visible = False
        ToolStrip1.Visible = True
    End Sub
    Private Sub ToolStrip1_MouseUp(sender As Object, e As MouseEventArgs) Handles ToolStrip1.MouseUp
        If e.Button = MouseButtons.Middle Then
            ComboBox1.Visible = True
            ToolStrip1.Visible = False
            ComboBox1.Focus()
        End If
    End Sub
    Private Sub UpdateNavigationButtons()
        tsBtnBack.Enabled = (currentHistoryIndex > 0)
        tsBtnForward.Enabled = (currentHistoryIndex < navigationHistory.Count - 1)

        Form1.ToolStripStatusLabel1.Text = ListView1.Items.Count & " items"
        Form1.ToolStripStatusLabel3.Text = ListView1.SelectedItems.Count & " selected items"
    End Sub
    Private Sub tsbtnBack_Click(sender As Object, e As EventArgs) Handles tsBtnBack.Click
        BackBtnUsed = True
        currentHistoryIndex -= 1
        LoadDirectoryContentsToListView(navigationHistory.Item(currentHistoryIndex))
        SyncTreeViewWithFolder(navigationHistory.Item(currentHistoryIndex))
        UpdateNavigationButtons()
    End Sub

    Private Sub tsbtnForward_Click(sender As Object, e As EventArgs) Handles tsBtnForward.Click
        BackBtnUsed = True
        currentHistoryIndex += 1
        LoadDirectoryContentsToListView(navigationHistory.Item(currentHistoryIndex))
        SyncTreeViewWithFolder(navigationHistory.Item(currentHistoryIndex))
        UpdateNavigationButtons()
    End Sub

    Private Sub tsBtnUp_Click(sender As Object, e As EventArgs) Handles tsBtnUp.Click
        Up()
    End Sub

    Public Sub Up()
        Dim currentPath As String = ComboBox1.Text.Trim()
        Dim parentPath As String = String.Empty
        Dim itemNameBefore As String = Me.Text

        Try
            If System.IO.Directory.Exists(currentPath) Then
                parentPath = System.IO.Path.GetDirectoryName(currentPath)

                If String.IsNullOrEmpty(parentPath) Then
                    parentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    System.Diagnostics.Debug.WriteLine("DEBUG: Jdeme na Desktop.")
                End If

                LoadDirectoryContentsToListView(parentPath)
                SyncTreeViewWithFolder(parentPath)
                If BackBtnUsed = True Then
                    BackBtnUsed = False
                Else
                    If currentHistoryIndex = navigationHistory.Count - 1 Then
                        If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                            navigationHistory.Add(parentPath)
                            currentHistoryIndex += 1
                        End If
                        UpdateNavigationButtons()
                    Else
                        For i As Integer = currentHistoryIndex To navigationHistory.Count - 2
                            navigationHistory.RemoveAt(navigationHistory.Count - 1)
                        Next
                        If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                            navigationHistory.Add(parentPath)
                            currentHistoryIndex += 1
                        End If
                        UpdateNavigationButtons()
                    End If
                End If
            Else
                parentPath = System.IO.Path.GetDirectoryName(currentPath)
                If System.IO.Directory.Exists(parentPath) Then
                    LoadDirectoryContentsToListView(parentPath)
                    SyncTreeViewWithFolder(parentPath)
                    If BackBtnUsed = True Then
                        BackBtnUsed = False
                    Else
                        If currentHistoryIndex = navigationHistory.Count - 1 Then
                            If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                                navigationHistory.Add(parentPath)
                                currentHistoryIndex += 1
                            End If
                            UpdateNavigationButtons()
                        Else
                            For i As Integer = currentHistoryIndex To navigationHistory.Count - 2
                                navigationHistory.RemoveAt(navigationHistory.Count - 1)
                            Next
                            If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                                navigationHistory.Add(parentPath)
                                currentHistoryIndex += 1
                            End If
                            UpdateNavigationButtons()
                        End If
                    End If
                Else
                    MessageBox.Show("Aktuální cesta není platná složka pro pohyb nahoru.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
            Try
                ListView1.FindItemWithText(itemNameBefore).Selected = True
                ListView1.FindItemWithText(itemNameBefore).Focused = True
            Catch ex As Exception

            End Try
        Catch ex As Exception
            MessageBox.Show("Chyba při přechodu do rodičovské složky: " & ex.Message, "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub ComboBox1_TextChanged(sender As Object, e As EventArgs) Handles ComboBox1.TextChanged
        If ComboBox1.Text.Length <= 3 Then
            tsBtnUp.Enabled = False
        Else
            tsBtnUp.Enabled = True
        End If
    End Sub

    Private Sub FolderView_Activated(sender As Object, e As EventArgs) Handles Me.Activated, Me.GotFocus
        Form1.curFormActive = Me.Tag
        Form1.ToolStripStatusLabel1.Text = ListView1.Items.Count & " items"
        Form1.ToolStripStatusLabel3.Text = ListView1.SelectedItems.Count & " selected items"
    End Sub

    Private Sub FolderView_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Form1.m_ChildFormNumber -= 1
    End Sub
    Public mainTreeViewHidden As Boolean = False
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If mainTreeViewHidden = True Then
            MainTreeView.Height = Panel2.Height / 2
            Button2.BackgroundImage = My.Resources.DownBlack
            mainTreeViewHidden = False
        Else
            MainTreeView.Height = 20
            Button2.BackgroundImage = My.Resources.UpBlack
            mainTreeViewHidden = True
        End If
    End Sub

    Private Sub Panel2_SizeChanged(sender As Object, e As EventArgs) Handles Panel2.SizeChanged
        If mainTreeViewHidden = False Then
            MainTreeView.Height = Panel2.Height / 2
            Button2.BackgroundImage = My.Resources.DownBlack
        Else
            MainTreeView.Height = 20
            Button2.BackgroundImage = My.Resources.UpBlack
        End If
    End Sub

    Private Sub ListView1_ItemActivate(sender As Object, e As EventArgs) Handles ListView1.ItemActivate
        'If Properties.RadioButton1.Checked = True Then
        If ListView1.SelectedItems.Count > 0 Then
            Dim selectedItem As ListViewItem = ListView1.SelectedItems(0)
            Dim itemPath As String = selectedItem.Tag.ToString()

            If System.IO.Directory.Exists(itemPath) Then
                SyncTreeViewWithFolder(itemPath)
                If BackBtnUsed = True Then
                    BackBtnUsed = False
                Else
                    If currentHistoryIndex = navigationHistory.Count - 1 Then
                        If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                            navigationHistory.Add(itemPath)
                            currentHistoryIndex += 1
                        End If
                        UpdateNavigationButtons()
                    Else
                        For i As Integer = currentHistoryIndex To navigationHistory.Count - 2
                            navigationHistory.RemoveAt(navigationHistory.Count - 1)
                        Next
                        If Not ComboBox1.Text = navigationHistory.Item(navigationHistory.Count - 1) Then
                            navigationHistory.Add(itemPath)
                            currentHistoryIndex += 1
                        End If
                        UpdateNavigationButtons()
                    End If
                End If
            ElseIf System.IO.File.Exists(itemPath) Then
                Try
                    Dim p As New Process()
                    p.StartInfo.FileName = itemPath
                    p.StartInfo.UseShellExecute = True
                    p.Start()
                Catch ex As Exception
                    MessageBox.Show("Nepodařilo se otevřít soubor: " & ex.Message, "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End If
        ' End If
    End Sub

    Private Sub OpenInMDIToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenInMDIToolStripMenuItem.Click

        Dim ChildForm As New FolderView
        ChildForm.MdiParent = Form1
        Form1.m_ChildFormNumber += 1
        ChildForm.Tag = Form1.m_ChildFormNumber

        ChildForm.initialPath = OpenInMDIToolStripMenuItem.Tag

        If String.IsNullOrEmpty(ChildForm.initialPath) Then
            ChildForm.initialPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            System.Diagnostics.Debug.WriteLine("DEBUG: Argument příkazového řádku nenalezen nebo neplatný, načítám výchozí cestu: " & ChildForm.initialPath)
        End If

        ChildForm.Show()
    End Sub

    Private Sub TreeView1_BeforeLabelEdit(sender As Object, e As NodeLabelEditEventArgs) Handles TreeView1.BeforeLabelEdit
        If e.Node.Text.Contains(":"c) Then
            e.CancelEdit = True
        Else
            e.CancelEdit = False
        End If
    End Sub

    Private Sub ListView1_BeforeLabelEdit(sender As Object, e As LabelEditEventArgs) Handles ListView1.BeforeLabelEdit
        If Directory.Exists(LastFocusedListViewItemTag) = True Then
            Dim DI As New DirectoryInfo(LastFocusedListViewItemTag)
            Dim ItemPath As String = DI.Parent.Name
        ElseIf File.Exists(LastFocusedListViewItemTag) = True Then
            Dim FI As New FileInfo(LastFocusedListViewItemTag)
            Dim ItemPath As String = FI.DirectoryName
        Else
            e.CancelEdit = True
        End If
    End Sub

    Private Sub ListView1_AfterLabelEdit(sender As Object, e As LabelEditEventArgs) Handles ListView1.AfterLabelEdit
        If e.Label IsNot Nothing Then
            Dim newText As String = e.Label
            Dim illegalCharacters As String = "\/:*?""<>|"
            If ContainsIllegalCharacters(newText, illegalCharacters) Then
                e.CancelEdit = True
                ToolTip1.Show("A directory name can't contain any of the following characters:" & Environment.NewLine & illegalCharacters, Me, LastFocusedListViewItemLocation.X + ListView1.Location.X, LastFocusedListViewItemLocation.Y, 7000)
            End If
        End If
    End Sub

    Private Function ContainsIllegalCharacters(text As String, illegalChars As String) As Boolean
        For Each c As Char In illegalChars
            If text.Contains(c) Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub DiskListItem_Click(sender As Object, e As EventArgs)
        Try
            LoadDirectoryContentsToListView(CType(sender, ToolStripDropDownButton).Tag)
            SyncTreeViewWithFolder(CType(sender, ToolStripDropDownButton).Tag)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub tsBtnDiskList_DropDownOpening(sender As Object, e As EventArgs) Handles tsBtnDiskList.DropDownOpening
        tsBtnDiskList.DropDownItems.Clear()
        For Each drive As DriveInfo In DriveInfo.GetDrives
            Dim item As New ToolStripDropDownButton(drive.Name & " [" & drive.VolumeLabel & "]")
            With item
                If Not drive.VolumeLabel = String.Empty Then
                    .Text = drive.Name & " [" & drive.VolumeLabel & "]"
                Else
                    .Text = drive.Name
                End If
                .Tag = drive.Name
                .Image = ShellIcons.GetFileIcon(drive.Name, True).ToBitmap
                AddHandler item.MouseUp, AddressOf DiskListItem_Click
                AddHandler item.DropDownOpening, AddressOf AddressBarDropDown_Opening
                AddHandler item.MouseHover, Sub(s, ea)
                                                item.ShowDropDown()
                                            End Sub
            End With
            tsBtnDiskList.DropDownItems.Add(item)
        Next
    End Sub

    Private Sub ListView1_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListView1.ItemSelectionChanged
        Form1.ToolStripStatusLabel1.Text = ListView1.Items.Count & " items"
        Form1.ToolStripStatusLabel3.Text = ListView1.SelectedItems.Count & " selected items"
    End Sub
End Class
