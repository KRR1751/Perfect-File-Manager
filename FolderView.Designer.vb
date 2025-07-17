<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FolderView
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        TreeView1 = New TreeView()
        ListView1 = New ListView()
        ImageList1 = New ImageList(components)
        ListViewView = New ContextMenuStrip(components)
        ViewToolStripMenuItem = New ToolStripMenuItem()
        LargeToolStripMenuItem = New ToolStripMenuItem()
        SmallToolStripMenuItem = New ToolStripMenuItem()
        DetailsToolStripMenuItem = New ToolStripMenuItem()
        ListToolStripMenuItem = New ToolStripMenuItem()
        TilesToolStripMenuItem = New ToolStripMenuItem()
        RefreshToolStripMenuItem = New ToolStripMenuItem()
        ImageList2 = New ImageList(components)
        ComboBox1 = New ComboBox()
        ToolStrip1 = New ToolStrip()
        Button1 = New Button()
        Panel1 = New Panel()
        ProgressBar1 = New ProgressBar()
        ToolStrip2 = New ToolStrip()
        tsBtnBack = New ToolStripButton()
        tsBtnForward = New ToolStripButton()
        tsBtnUp = New ToolStripButton()
        ToolStripSeparator1 = New ToolStripSeparator()
        tsBtnDiskList = New ToolStripDropDownButton()
        ToolStripSeparator2 = New ToolStripSeparator()
        Splitter1 = New Splitter()
        Panel2 = New Panel()
        TreeView2 = New TreeView()
        MainTreeView = New Panel()
        Panel3 = New Panel()
        Label1 = New Label()
        Button2 = New Button()
        Panel4 = New Panel()
        Label2 = New Label()
        ImageList3 = New ImageList(components)
        FolderCM = New ContextMenuStrip(components)
        OpenInMDIToolStripMenuItem = New ToolStripMenuItem()
        OpenInNewInstanceToolStripMenuItem = New ToolStripMenuItem()
        ToolTip1 = New ToolTip(components)
        ImageList4 = New ImageList(components)
        NewDropDown = New ContextMenuStrip(components)
        FolderToolStripMenuItem = New ToolStripMenuItem()
        ToolStripSeparator3 = New ToolStripSeparator()
        FileToolStripMenuItem = New ToolStripMenuItem()
        ListViewView.SuspendLayout()
        Panel1.SuspendLayout()
        ToolStrip2.SuspendLayout()
        Panel2.SuspendLayout()
        MainTreeView.SuspendLayout()
        Panel3.SuspendLayout()
        Panel4.SuspendLayout()
        FolderCM.SuspendLayout()
        NewDropDown.SuspendLayout()
        SuspendLayout()
        ' 
        ' TreeView1
        ' 
        TreeView1.Dock = DockStyle.Fill
        TreeView1.LabelEdit = True
        TreeView1.Location = New Point(0, 20)
        TreeView1.Name = "TreeView1"
        TreeView1.Size = New Size(180, 0)
        TreeView1.TabIndex = 0
        ' 
        ' ListView1
        ' 
        ListView1.Dock = DockStyle.Fill
        ListView1.LabelEdit = True
        ListView1.Location = New Point(180, 34)
        ListView1.Name = "ListView1"
        ListView1.Size = New Size(682, 421)
        ListView1.TabIndex = 1
        ListView1.UseCompatibleStateImageBehavior = False
        ListView1.View = View.Details
        ' 
        ' ImageList1
        ' 
        ImageList1.ColorDepth = ColorDepth.Depth32Bit
        ImageList1.ImageSize = New Size(16, 16)
        ImageList1.TransparentColor = Color.Transparent
        ' 
        ' ListViewView
        ' 
        ListViewView.Items.AddRange(New ToolStripItem() {ViewToolStripMenuItem, RefreshToolStripMenuItem})
        ListViewView.Name = "ContextMenuStrip1"
        ListViewView.Size = New Size(114, 48)
        ' 
        ' ViewToolStripMenuItem
        ' 
        ViewToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {LargeToolStripMenuItem, SmallToolStripMenuItem, DetailsToolStripMenuItem, ListToolStripMenuItem, TilesToolStripMenuItem})
        ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        ViewToolStripMenuItem.Size = New Size(113, 22)
        ViewToolStripMenuItem.Text = "View"
        ' 
        ' LargeToolStripMenuItem
        ' 
        LargeToolStripMenuItem.Name = "LargeToolStripMenuItem"
        LargeToolStripMenuItem.Size = New Size(109, 22)
        LargeToolStripMenuItem.Text = "Large"
        ' 
        ' SmallToolStripMenuItem
        ' 
        SmallToolStripMenuItem.Name = "SmallToolStripMenuItem"
        SmallToolStripMenuItem.Size = New Size(109, 22)
        SmallToolStripMenuItem.Text = "Small"
        ' 
        ' DetailsToolStripMenuItem
        ' 
        DetailsToolStripMenuItem.Name = "DetailsToolStripMenuItem"
        DetailsToolStripMenuItem.Size = New Size(109, 22)
        DetailsToolStripMenuItem.Text = "Details"
        ' 
        ' ListToolStripMenuItem
        ' 
        ListToolStripMenuItem.Name = "ListToolStripMenuItem"
        ListToolStripMenuItem.Size = New Size(109, 22)
        ListToolStripMenuItem.Text = "List"
        ' 
        ' TilesToolStripMenuItem
        ' 
        TilesToolStripMenuItem.Name = "TilesToolStripMenuItem"
        TilesToolStripMenuItem.Size = New Size(109, 22)
        TilesToolStripMenuItem.Text = "Tiles"
        ' 
        ' RefreshToolStripMenuItem
        ' 
        RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        RefreshToolStripMenuItem.Size = New Size(113, 22)
        RefreshToolStripMenuItem.Text = "Refresh"
        ' 
        ' ImageList2
        ' 
        ImageList2.ColorDepth = ColorDepth.Depth32Bit
        ImageList2.ImageSize = New Size(16, 16)
        ImageList2.TransparentColor = Color.Transparent
        ' 
        ' ComboBox1
        ' 
        ComboBox1.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        ComboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        ComboBox1.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories
        ComboBox1.FormattingEnabled = True
        ComboBox1.Location = New Point(139, 6)
        ComboBox1.Name = "ComboBox1"
        ComboBox1.Size = New Size(689, 23)
        ComboBox1.TabIndex = 2
        ComboBox1.Visible = False
        ' 
        ' ToolStrip1
        ' 
        ToolStrip1.AutoSize = False
        ToolStrip1.BackColor = SystemColors.Control
        ToolStrip1.Dock = DockStyle.Fill
        ToolStrip1.GripStyle = ToolStripGripStyle.Hidden
        ToolStrip1.Location = New Point(141, 0)
        ToolStrip1.Name = "ToolStrip1"
        ToolStrip1.RenderMode = ToolStripRenderMode.System
        ToolStrip1.Size = New Size(687, 34)
        ToolStrip1.TabIndex = 3
        ToolStrip1.Text = "ToolStrip1"
        ' 
        ' Button1
        ' 
        Button1.BackColor = SystemColors.ControlLight
        Button1.BackgroundImageLayout = ImageLayout.Center
        Button1.Dock = DockStyle.Right
        Button1.FlatStyle = FlatStyle.Flat
        Button1.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Button1.Image = My.Resources.Resources.RefreshBlackExtraSmall
        Button1.Location = New Point(828, 0)
        Button1.Name = "Button1"
        Button1.Size = New Size(34, 34)
        Button1.TabIndex = 4
        Button1.UseVisualStyleBackColor = False
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = SystemColors.ControlLightLight
        Panel1.Controls.Add(ComboBox1)
        Panel1.Controls.Add(ToolStrip1)
        Panel1.Controls.Add(ProgressBar1)
        Panel1.Controls.Add(ToolStrip2)
        Panel1.Controls.Add(Button1)
        Panel1.Dock = DockStyle.Top
        Panel1.Location = New Point(0, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(862, 34)
        Panel1.TabIndex = 5
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Dock = DockStyle.Fill
        ProgressBar1.Location = New Point(141, 0)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(687, 34)
        ProgressBar1.TabIndex = 9
        ProgressBar1.Value = 50
        ' 
        ' ToolStrip2
        ' 
        ToolStrip2.AutoSize = False
        ToolStrip2.Dock = DockStyle.Left
        ToolStrip2.GripStyle = ToolStripGripStyle.Hidden
        ToolStrip2.Items.AddRange(New ToolStripItem() {tsBtnBack, tsBtnForward, tsBtnUp, ToolStripSeparator1, tsBtnDiskList, ToolStripSeparator2})
        ToolStrip2.LayoutStyle = ToolStripLayoutStyle.Flow
        ToolStrip2.Location = New Point(0, 0)
        ToolStrip2.Name = "ToolStrip2"
        ToolStrip2.RenderMode = ToolStripRenderMode.System
        ToolStrip2.Size = New Size(141, 34)
        ToolStrip2.TabIndex = 7
        ToolStrip2.Text = "ToolStrip2"
        ' 
        ' tsBtnBack
        ' 
        tsBtnBack.AutoSize = False
        tsBtnBack.DisplayStyle = ToolStripItemDisplayStyle.Image
        tsBtnBack.Enabled = False
        tsBtnBack.Image = My.Resources.Resources.BackBlack
        tsBtnBack.ImageTransparentColor = Color.Magenta
        tsBtnBack.Name = "tsBtnBack"
        tsBtnBack.Size = New Size(32, 32)
        tsBtnBack.Text = "ToolStripButton1"
        ' 
        ' tsBtnForward
        ' 
        tsBtnForward.AutoSize = False
        tsBtnForward.DisplayStyle = ToolStripItemDisplayStyle.Image
        tsBtnForward.Enabled = False
        tsBtnForward.Image = My.Resources.Resources.ForwardBlack
        tsBtnForward.ImageTransparentColor = Color.Magenta
        tsBtnForward.Name = "tsBtnForward"
        tsBtnForward.Size = New Size(32, 32)
        tsBtnForward.Text = "ToolStripButton2"
        ' 
        ' tsBtnUp
        ' 
        tsBtnUp.AutoSize = False
        tsBtnUp.BackColor = SystemColors.ControlLightLight
        tsBtnUp.DisplayStyle = ToolStripItemDisplayStyle.Image
        tsBtnUp.Image = My.Resources.Resources.UpBlack
        tsBtnUp.ImageTransparentColor = Color.Magenta
        tsBtnUp.Name = "tsBtnUp"
        tsBtnUp.Size = New Size(32, 32)
        tsBtnUp.Text = "ToolStripButton3"
        ' 
        ' ToolStripSeparator1
        ' 
        ToolStripSeparator1.AutoSize = False
        ToolStripSeparator1.Name = "ToolStripSeparator1"
        ToolStripSeparator1.Size = New Size(6, 32)
        ' 
        ' tsBtnDiskList
        ' 
        tsBtnDiskList.AutoSize = False
        tsBtnDiskList.DisplayStyle = ToolStripItemDisplayStyle.Image
        tsBtnDiskList.Image = My.Resources.Resources.DownBlack
        tsBtnDiskList.ImageTransparentColor = Color.Magenta
        tsBtnDiskList.Name = "tsBtnDiskList"
        tsBtnDiskList.Size = New Size(32, 32)
        tsBtnDiskList.Text = "Disk List"
        tsBtnDiskList.ToolTipText = "Computer (Disk List)"
        ' 
        ' ToolStripSeparator2
        ' 
        ToolStripSeparator2.AutoSize = False
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New Size(6, 32)
        ' 
        ' Splitter1
        ' 
        Splitter1.Location = New Point(180, 34)
        Splitter1.Name = "Splitter1"
        Splitter1.Size = New Size(3, 421)
        Splitter1.TabIndex = 7
        Splitter1.TabStop = False
        ' 
        ' Panel2
        ' 
        Panel2.Controls.Add(TreeView2)
        Panel2.Controls.Add(MainTreeView)
        Panel2.Controls.Add(Panel4)
        Panel2.Dock = DockStyle.Left
        Panel2.Location = New Point(0, 34)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(180, 421)
        Panel2.TabIndex = 8
        ' 
        ' TreeView2
        ' 
        TreeView2.Dock = DockStyle.Fill
        TreeView2.FullRowSelect = True
        TreeView2.LabelEdit = True
        TreeView2.Location = New Point(0, 20)
        TreeView2.Name = "TreeView2"
        TreeView2.ShowRootLines = False
        TreeView2.Size = New Size(180, 381)
        TreeView2.TabIndex = 1
        ' 
        ' MainTreeView
        ' 
        MainTreeView.Controls.Add(TreeView1)
        MainTreeView.Controls.Add(Panel3)
        MainTreeView.Dock = DockStyle.Bottom
        MainTreeView.Location = New Point(0, 401)
        MainTreeView.Name = "MainTreeView"
        MainTreeView.Size = New Size(180, 20)
        MainTreeView.TabIndex = 2
        ' 
        ' Panel3
        ' 
        Panel3.Controls.Add(Label1)
        Panel3.Controls.Add(Button2)
        Panel3.Dock = DockStyle.Top
        Panel3.Location = New Point(0, 0)
        Panel3.Name = "Panel3"
        Panel3.Size = New Size(180, 20)
        Panel3.TabIndex = 1
        ' 
        ' Label1
        ' 
        Label1.AutoEllipsis = True
        Label1.Dock = DockStyle.Fill
        Label1.ForeColor = SystemColors.ControlText
        Label1.Location = New Point(0, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(160, 20)
        Label1.TabIndex = 1
        Label1.Text = "More Folders:"
        Label1.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' Button2
        ' 
        Button2.BackgroundImage = My.Resources.Resources.UpBlack
        Button2.BackgroundImageLayout = ImageLayout.Zoom
        Button2.Dock = DockStyle.Right
        Button2.FlatStyle = FlatStyle.Flat
        Button2.Location = New Point(160, 0)
        Button2.Name = "Button2"
        Button2.Size = New Size(20, 20)
        Button2.TabIndex = 0
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Panel4
        ' 
        Panel4.Controls.Add(Label2)
        Panel4.Dock = DockStyle.Top
        Panel4.Location = New Point(0, 0)
        Panel4.Name = "Panel4"
        Panel4.Size = New Size(180, 20)
        Panel4.TabIndex = 3
        ' 
        ' Label2
        ' 
        Label2.AutoEllipsis = True
        Label2.Dock = DockStyle.Fill
        Label2.ForeColor = SystemColors.ControlText
        Label2.Location = New Point(0, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(180, 20)
        Label2.TabIndex = 1
        Label2.Text = "Pinned && Favorites"
        Label2.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' ImageList3
        ' 
        ImageList3.ColorDepth = ColorDepth.Depth32Bit
        ImageList3.ImageSize = New Size(16, 16)
        ImageList3.TransparentColor = Color.Transparent
        ' 
        ' FolderCM
        ' 
        FolderCM.Items.AddRange(New ToolStripItem() {OpenInMDIToolStripMenuItem, OpenInNewInstanceToolStripMenuItem})
        FolderCM.Name = "FolderCM"
        FolderCM.Size = New Size(191, 48)
        ' 
        ' OpenInMDIToolStripMenuItem
        ' 
        OpenInMDIToolStripMenuItem.Name = "OpenInMDIToolStripMenuItem"
        OpenInMDIToolStripMenuItem.Size = New Size(190, 22)
        OpenInMDIToolStripMenuItem.Text = "Open in New MDI"
        ' 
        ' OpenInNewInstanceToolStripMenuItem
        ' 
        OpenInNewInstanceToolStripMenuItem.Enabled = False
        OpenInNewInstanceToolStripMenuItem.Name = "OpenInNewInstanceToolStripMenuItem"
        OpenInNewInstanceToolStripMenuItem.Size = New Size(190, 22)
        OpenInNewInstanceToolStripMenuItem.Text = "Open in New Instance"
        ' 
        ' ToolTip1
        ' 
        ToolTip1.IsBalloon = True
        ' 
        ' ImageList4
        ' 
        ImageList4.ColorDepth = ColorDepth.Depth32Bit
        ImageList4.ImageSize = New Size(16, 16)
        ImageList4.TransparentColor = Color.Transparent
        ' 
        ' NewDropDown
        ' 
        NewDropDown.Items.AddRange(New ToolStripItem() {FolderToolStripMenuItem, ToolStripSeparator3, FileToolStripMenuItem})
        NewDropDown.Name = "NewDropDown"
        NewDropDown.Size = New Size(181, 76)
        ' 
        ' FolderToolStripMenuItem
        ' 
        FolderToolStripMenuItem.Name = "FolderToolStripMenuItem"
        FolderToolStripMenuItem.Size = New Size(180, 22)
        FolderToolStripMenuItem.Text = "Folder"
        ' 
        ' ToolStripSeparator3
        ' 
        ToolStripSeparator3.Name = "ToolStripSeparator3"
        ToolStripSeparator3.Size = New Size(177, 6)
        ' 
        ' FileToolStripMenuItem
        ' 
        FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        FileToolStripMenuItem.Size = New Size(180, 22)
        FileToolStripMenuItem.Text = "File"
        ' 
        ' FolderView
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(862, 455)
        Controls.Add(Splitter1)
        Controls.Add(ListView1)
        Controls.Add(Panel2)
        Controls.Add(Panel1)
        Name = "FolderView"
        Text = "FolderView"
        WindowState = FormWindowState.Maximized
        ListViewView.ResumeLayout(False)
        Panel1.ResumeLayout(False)
        ToolStrip2.ResumeLayout(False)
        ToolStrip2.PerformLayout()
        Panel2.ResumeLayout(False)
        MainTreeView.ResumeLayout(False)
        Panel3.ResumeLayout(False)
        Panel4.ResumeLayout(False)
        FolderCM.ResumeLayout(False)
        NewDropDown.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents TreeView1 As TreeView
    Friend WithEvents ListView1 As ListView
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents ListViewView As ContextMenuStrip
    Friend WithEvents ImageList2 As ImageList
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents Button1 As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents ViewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LargeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SmallToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DetailsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ListToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RefreshToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStrip2 As ToolStrip
    Friend WithEvents tsBtnBack As ToolStripButton
    Friend WithEvents tsBtnForward As ToolStripButton
    Friend WithEvents tsBtnUp As ToolStripButton
    Friend WithEvents Splitter1 As Splitter
    Friend WithEvents Panel2 As Panel
    Friend WithEvents TreeView2 As TreeView
    Friend WithEvents MainTreeView As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Button2 As Button
    Friend WithEvents Panel4 As Panel
    Friend WithEvents Label2 As Label
    Friend WithEvents ImageList3 As ImageList
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents FolderCM As ContextMenuStrip
    Friend WithEvents OpenInMDIToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenInNewInstanceToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents tsBtnDiskList As ToolStripDropDownButton
    Friend WithEvents ImageList4 As ImageList
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents NewDropDown As ContextMenuStrip
    Friend WithEvents FolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
End Class
