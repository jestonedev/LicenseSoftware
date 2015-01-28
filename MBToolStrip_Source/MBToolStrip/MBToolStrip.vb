#Region "   About Auhor"
'
'   UserControl Name    :   MBToolStrip Control
'   Created             :   15 Jan 2012
'   Purpose             :   Extends style capabilities of a ToolStrip Control
'   Vision              :   1.0.4362.39367
'   IDE                 :   Visual Basic .Net 2008
'   Author              :   Manoj K Bhoir
'
'   You can not:
'   Sell or redistribute this code or the binary for profit.
'   Use this in spyware, malware, or any generally acknowledged form of malicious software.
'   Remove or alter the above author accreditation, or this disclaimer.
'
'   You can:
'   Use this code in your applications in any way you like.
'   Use this in a published program, (a credit to MBUserControls would be nice)
'
'   I will not:
'   Except any responsibility for this code whatsoever. 
'   There is no guarantee of fitness, nor should you have any expectation of support. 
'   
'                                                                   Manoj K Bhoir
'                                                                   manojbhoir28@gmail.com    
#End Region

#Region "   Imports"

Imports System.Text
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Windows.Forms
Imports System.Drawing.Drawing2D
Imports System.ComponentModel

#End Region

#Region "   MBMenuStrip"

''' <summary>
''' MBToolStrip Class © Manoj K Bhoir
''' </summary>
''' <remarks>Version 1.0</remarks>
<ToolboxItem(True), ToolboxBitmap(GetType(MBToolStrip), "MBToolStrip.MBToolStrip.bmp"), ToolboxItemFilter("System.Windows.Forms"), Description("Display the ToolStrip.")> _
Public Class MBToolStrip
    Inherits ToolStrip
    ''' <summary>
    ''' Constructor of MBToolStrip
    ''' </summary>
    Public Sub New()
        MyBase.New()
        InitializeComponent()
    End Sub
    ''' <summary>
    ''' Initialize MBToolStrip and MBToolItemRenderClass
    ''' </summary>
    Public Sub InitializeComponent()
        Me.Name = "MBToolStrip"
        Me.Renderer = New MBMenuRenderer
    End Sub

   

End Class

#End Region

