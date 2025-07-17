Imports System.Runtime.InteropServices
Imports System.Drawing

Namespace ShellApi
    ' Enumerace pro vlajky GetFileInfo
    <Flags()>
    Public Enum SHGFI As UInteger
        Icon = &H100                  ' Get icon
        DisplayName = &H200           ' Get display name
        TypeName = &H400              ' Get type name
        Attributes = &H800            ' Get attributes
        IconLocation = &H1000         ' Get icon location
        ExeType = &H2000              ' Return exe type
        SysIconIndex = &H4000         ' Get system icon index
        LinkOverlay = &H8000          ' Put link overlay on icon
        Selected = &H10000            ' Show icon in selected state
        Attr_Specified = &H20000      ' Get only specified attributes
        LargeIcon = &H0               ' Get large icon
        SmallIcon = &H1               ' Get small icon
        OpenIcon = &H2                ' Get open icon
        ShellIconSize = &H4           ' Get shell size icon
        PIDL = &H8                    ' Use pidl
        AddOverlays = &H20            ' Add overlays
        OverlayIndex = &H40           ' Get overlay index
    End Enum

    ' Struktura pro informace o souboru
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Public Structure SHFILEINFO
        Public hIcon As IntPtr      ' Handle k ikoně
        Public iIcon As Integer     ' Index ikony v systémovém seznamu ikon
        Public dwAttributes As UInteger ' Atributy souboru
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)>
        Public szDisplayName As String ' Zobrazované jméno souboru
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=80)>
        Public szTypeName As String     ' Popis typu souboru
    End Structure

    Public Module NativeMethods ' Můžeš to nazvat jakkoli, např. 'ShellInterop' nebo 'WinApi'

        ' Deklarace funkce SHGetFileInfo
        <DllImport("shell32.dll", CharSet:=CharSet.Auto)>
        Public Function SHGetFileInfo(
            ByVal pszPath As String,
            ByVal dwFileAttributes As UInteger,
            ByRef psfi As SHFILEINFO,
            ByVal cbFileInfo As UInteger,
            ByVal uFlags As SHGFI
        ) As IntPtr
        End Function

        ' Také funkce DestroyIcon, pokud ji máš v ShellApi.vb
        <DllImport("user32.dll", CharSet:=CharSet.Auto)>
        Public Function DestroyIcon(ByVal hIcon As IntPtr) As Boolean
        End Function

    End Module ' Konec modulu

End Namespace