#Region "About Auhor"
'
'   UserControl Name    :   MBToolStrip/StatusStrip Renderer Class
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

#Region "Imports"
Imports System.Drawing
Imports System.Drawing.Text
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Collections.Generic
#End Region

#Region "MBToolStripRenderer Class"

''' <summary>
''' MBToolStripRenderer Class © Manoj K Bhoir
''' </summary>
''' <remarks>Version 1.0.1096.2286</remarks>
Public Class MBMenuRenderer
    Inherits ToolStripProfessionalRenderer

#Region "Gradient Item Color"
    ''' <summary>
    ''' GradientItemColor Class which Provides Gradient Colors
    ''' </summary>
    Private Class GradientItemColor

#Region "           Public Fields"
        Public InsideTop1 As Color
        Public InsideTop2 As Color
        Public InsideBottom1 As Color
        Public InsideBottom2 As Color
        Public FillTop1 As Color
        Public FillTop2 As Color
        Public FillBottom1 As Color
        Public FillBottom2 As Color
        Public Border1 As Color
        Public Border2 As Color
#End Region

#Region "           Identity"
        ''' <summary>
        ''' Constructor for GradientItemColor Class
        ''' </summary>
        Public Sub New(ByVal _insideTop1 As Color, ByVal _insideTop2 As Color, _
                                      ByVal _insideBottom1 As Color, ByVal _insideBottom2 As Color, _
                                      ByVal _fillTop1 As Color, ByVal _fillTop2 As Color, _
                                      ByVal _fillBottom1 As Color, ByVal _fillBottom2 As Color, _
                                      ByVal _border1 As Color, ByVal _border2 As Color)
            InsideTop1 = _insideTop1
            InsideTop2 = _insideTop2
            InsideBottom1 = _insideBottom1
            InsideBottom2 = _insideBottom2
            FillTop1 = _fillTop1
            FillTop2 = _fillTop2
            FillBottom1 = _fillBottom1
            FillBottom2 = _fillBottom2
            Border1 = _border1
            Border2 = _border2
        End Sub
#End Region

    End Class

#End Region

#Region "Private Variables"
    Private Const _gripOffset As Int32 = 1
    Private Const _gripSquare As Int32 = 2
    Private Const _gripSize As Int32 = 3
    Private Const _gripMove As Int32 = 4
    Private Const _gripLines As Int32 = 3
    Private Const _checkInset As Int32 = 1
    Private Const _marginInset As Int32 = 2
    Private Const _separatorInset As Int32 = 31
    Private Const _cutToolItemMenu As Single = 1.0
    Private Const _cutContextMenu As Single = 0.0F
    Private Const _cutMenuItemBack As Single = 1.2F
    Private Const _contextCheckTickThickness As Single = 1.6F
    Private _statusStripBlend As Blend
#End Region

#Region "Declaration Of Colors"

    Private _color1 As Color = Color.FromArgb(167, 167, 167)
    Private _color2 As Color = Color.FromArgb(21, 66, 139)
    Private _color3 As Color = Color.FromArgb(76, 83, 92)
    Private _color4 As Color = Color.FromArgb(250, 250, 250)
    Private _color5 As Color = Color.FromArgb(248, 248, 248)
    Private _color6 As Color = Color.FromArgb(243, 243, 243)
    Private _rcolor1 As Color = Color.FromArgb(255, 255, 251)
    Private _rcolor2 As Color = Color.FromArgb(255, 249, 227)
    Private _rcolor3 As Color = Color.FromArgb(255, 242, 201)
    Private _rcolor4 As Color = Color.FromArgb(255, 248, 181)
    Private _rcolor5 As Color = Color.FromArgb(255, 252, 229)
    Private _rcolor6 As Color = Color.FromArgb(255, 235, 166)
    Private _rcolor7 As Color = Color.FromArgb(255, 213, 103)
    Private _rcolor8 As Color = Color.FromArgb(255, 228, 145)
    Private _rcolor9 As Color = Color.FromArgb(160, 188, 228)
    Private _rcolorA As Color = Color.FromArgb(255, 214, 78) 'Color.FromArgb(121, 153, 194)
    Private _rcolorB As Color = Color.FromArgb(182, 190, 192)
    Private _rcolorC As Color = Color.FromArgb(155, 163, 167)
    Private _rcolorD As Color = Color.FromArgb(233, 168, 97)
    Private _rcolorE As Color = Color.FromArgb(247, 164, 39)
    Private _rcolorF As Color = Color.FromArgb(246, 156, 24)
    Private _rcolorG As Color = Color.FromArgb(253, 173, 17)
    Private _rcolorH As Color = Color.FromArgb(254, 185, 108)
    Private _rcolorI As Color = Color.FromArgb(253, 164, 97)
    Private _rcolorJ As Color = Color.FromArgb(252, 143, 61)
    Private _rcolorK As Color = Color.FromArgb(255, 208, 134)
    Private _rcolorL As Color = Color.FromArgb(249, 192, 103)
    Private _rcolorM As Color = Color.FromArgb(250, 195, 93)
    Private _rcolorN As Color = Color.FromArgb(248, 190, 81)
    Private _rcolorO As Color = Color.FromArgb(255, 208, 49)
    Private _rcolorP As Color = Color.FromArgb(254, 214, 168)
    Private _rcolorQ As Color = Color.FromArgb(252, 180, 100)
    Private _rcolorR As Color = Color.FromArgb(252, 161, 54)
    Private _rcolorS As Color = Color.FromArgb(254, 238, 170)
    Private _rcolorT As Color = Color.FromArgb(249, 202, 113)
    Private _rcolorU As Color = Color.FromArgb(250, 205, 103)
    Private _rcolorV As Color = Color.FromArgb(248, 200, 91)
    Private _rcolorW As Color = Color.FromArgb(255, 218, 59)
    Private _rcolorX As Color = Color.FromArgb(254, 185, 108)
    Private _rcolorY As Color = Color.FromArgb(252, 161, 54)
    Private _rcolorZ As Color = Color.FromArgb(254, 238, 170)

    Private _textDisabled As Color = _color1
    Private _textMenuStripItem As Color = _color2
    Private _textStatusStripItem As Color = _color2
    Private _textContextMenuItem As Color = _color2
    Private _arrowDisabled As Color = _color1
    Private _arrowLight As Color = Color.FromArgb(106, 126, 197)
    Private _arrowDark As Color = Color.FromArgb(64, 70, 90)
    Private _separatorMenuLight As Color = Color.FromArgb(245, 245, 245)
    Private _separatorMenuDark As Color = Color.FromArgb(197, 197, 197)
    Private _contextMenuBack As Color = _color4
    Private _contextCheckBorder As Color = Color.FromArgb(242, 149, 54)
    Private _contextCheckTick As Color = Color.FromArgb(66, 75, 138)
    Private _statusStripBorderDark As Color = Color.FromArgb(86, 125, 176)
    Private _statusStripBorderLight As Color = Color.White
    Private _gripDark As Color = Color.FromArgb(114, 152, 204)
    Private _gripLight As Color = _color5
    Private _itemContextItemEnabledColors As GradientItemColor = New GradientItemColor(_rcolor1, _rcolor2, _rcolor3, _rcolor4, _rcolor5, _rcolor6, _rcolor7, _rcolor8, Color.FromArgb(217, 203, 150), Color.FromArgb(192, 167, 118))
    Private _itemDisabledColors As GradientItemColor = New GradientItemColor(_color4, _color6, Color.FromArgb(236, 236, 236), Color.FromArgb(230, 230, 230), _color6, Color.FromArgb(224, 224, 224), Color.FromArgb(200, 200, 200), Color.FromArgb(210, 210, 210), Color.FromArgb(212, 212, 212), Color.FromArgb(195, 195, 195))
    Private _itemToolItemSelectedColors As GradientItemColor = New GradientItemColor(_rcolor1, _rcolor2, _rcolor3, _rcolor4, _rcolor5, _rcolor6, _rcolor7, _rcolor8, _rcolor9, _rcolorA)
    Private _itemToolItemPressedColors As GradientItemColor = New GradientItemColor(_rcolorD, _rcolorE, _rcolorF, _rcolorG, _rcolorH, _rcolorI, _rcolorJ, _rcolorK, _rcolor9, _rcolorA)
    Private _itemToolItemCheckedColors As GradientItemColor = New GradientItemColor(_rcolorL, _rcolorM, _rcolorN, _rcolorO, _rcolorP, _rcolorQ, _rcolorR, _rcolorS, _rcolor9, _rcolorA)
    Private _itemToolItemCheckPressColors As GradientItemColor = New GradientItemColor(_rcolorT, _rcolorU, _rcolorV, _rcolorW, _rcolorX, _rcolorI, _rcolorY, _rcolorZ, _rcolor9, _rcolorA)
#End Region

#Region "Constructor"
    ''' <summary>
    ''' Constructor for MBToolStripRenderer Class
    ''' </summary>
    Sub New()
        _statusStripBlend = New Blend()
        _statusStripBlend.Positions = New Single() {0.0F, 0.25F, 0.25F, 0.57F, 0.86F, 1.0F}
        _statusStripBlend.Factors = New Single() {0.1F, 0.6F, 1.0F, 0.4F, 0.0F, 0.95F}
    End Sub
#End Region

#Region "OnRenderArrow"
    ''' <summary>
    ''' MBToolStripRenderer Class Event to Render Arrow
    ''' </summary>
    Protected Overrides Sub OnRenderArrow(ByVal e As System.Windows.Forms.ToolStripArrowRenderEventArgs)
        If ((e.ArrowRectangle.Width > 0) And (e.ArrowRectangle.Height > 0)) Then
            Using arrowPath As GraphicsPath = DrawArrowPath(e.Item, e.ArrowRectangle, e.Direction)
                Dim boundsF As RectangleF = arrowPath.GetBounds()
                boundsF.Inflate(1.0F, 1.0F)
                Dim color1 As Color
                Dim color2 As Color
                If e.Item.Enabled Then
                    color1 = _arrowLight
                    color2 = _arrowDark
                Else
                    color1 = _arrowDisabled
                    color2 = _arrowDisabled
                End If
                Dim angle As Single = 0
                Select Case (e.Direction)
                    Case ArrowDirection.Right
                        angle = 0
                    Case ArrowDirection.Left
                        angle = 180.0F
                    Case ArrowDirection.Down
                        angle = 90.0F
                    Case ArrowDirection.Up
                        angle = 270.0F
                End Select
                Using arrowBrush As LinearGradientBrush = New LinearGradientBrush(boundsF, color1, color2, angle)
                    e.Graphics.FillPath(arrowBrush, arrowPath)
                End Using
            End Using
        End If
    End Sub
#End Region

#Region "OnRenderButtonBackground"
    ''' <summary>
    ''' MBToolStripRenderer Class Event to Render ToolStrip Button Background
    ''' </summary>
    Protected Overrides Sub OnRenderButtonBackground(ByVal e As System.Windows.Forms.ToolStripItemRenderEventArgs)
        Dim button As ToolStripButton = e.Item
        If (button.Selected Or button.Pressed Or button.Checked) Then RenderToolButtonBackground(e.Graphics, button, e.ToolStrip)
    End Sub
#End Region

#Region "OnRenderDropDownButtonBackground"
    ''' <summary>
    ''' MBToolStripRenderer Class Event to Render ToolStrip DropDownButtonBackground
    ''' </summary>
    Protected Overrides Sub OnRenderDropDownButtonBackground(ByVal e As System.Windows.Forms.ToolStripItemRenderEventArgs)
        If (e.Item.Selected Or e.Item.Pressed) Then RenderToolDropButtonBackground(e.Graphics, e.Item, e.ToolStrip)
    End Sub
#End Region

#Region "OnRenderItemCheck"
    ''' <summary>
    ''' MBToolStripRenderer Class Event Raised When Item Checked
    ''' </summary>
    Protected Overrides Sub OnRenderItemCheck(ByVal e As System.Windows.Forms.ToolStripItemImageRenderEventArgs)
        Dim checkBox As Rectangle = e.ImageRectangle
        checkBox.Inflate(1, 1)
        If (checkBox.Top > _checkInset) Then
            Dim diff As Int32 = checkBox.Top - _checkInset
            checkBox.Y -= diff
            checkBox.Height += diff
        End If
        If (checkBox.Height <= (e.Item.Bounds.Height - (_checkInset * 2))) Then
            Dim diff As Int32 = e.Item.Bounds.Height - (_checkInset * 2) - checkBox.Height
            checkBox.Height += diff
        End If
        Using uaa As UseAntiAlias = New UseAntiAlias(e.Graphics)
            Using borderPath As GraphicsPath = DrawBorderPath(checkBox, _cutMenuItemBack)
                Using fillBrush As SolidBrush = New SolidBrush(ColorTable.CheckBackground)
                    e.Graphics.FillPath(fillBrush, borderPath)
                    Using borderPen As Pen = New Pen(_contextCheckBorder)
                        e.Graphics.DrawPath(borderPen, borderPath)
                        If Not (e.Image Is Nothing) Then
                            Dim checkState As CheckState = checkState.Unchecked
                            If TypeOf (e.Item) Is ToolStripMenuItem Then
                                Dim item As ToolStripMenuItem = e.Item
                                checkState = item.CheckState
                            End If
                            Select Case (checkState)
                                Case checkState.Checked
                                    Using tickPath As GraphicsPath = DrawTickPath(checkBox)
                                        Using tickPen As Pen = New Pen(_contextCheckTick, _contextCheckTickThickness)
                                            e.Graphics.DrawPath(tickPen, tickPath)
                                        End Using
                                    End Using
                                Case checkState.Indeterminate
                                    Using tickPath As GraphicsPath = DrawIndeterminatePath(checkBox)
                                        Using tickBrush As SolidBrush = New SolidBrush(_contextCheckTick)
                                            e.Graphics.FillPath(tickBrush, tickPath)
                                        End Using
                                    End Using
                            End Select
                        End If
                    End Using
                End Using
            End Using
        End Using
    End Sub
#End Region

#Region "OnRenderItemText"
    ''' <summary>
    ''' MBToolStripRenderer Class Event to Render ItemText
    ''' </summary>
    Protected Overrides Sub OnRenderItemText(ByVal e As System.Windows.Forms.ToolStripItemTextRenderEventArgs)
        If TypeOf (e.ToolStrip) Is MenuStrip Or TypeOf (e.ToolStrip) Is ToolStrip Or _
                 TypeOf (e.ToolStrip) Is ContextMenuStrip Or _
                 TypeOf (e.ToolStrip) Is ToolStripDropDownMenu Then
            If (Not e.Item.Enabled) Then
                e.TextColor = _textDisabled
            Else
                If (TypeOf (e.ToolStrip) Is MenuStrip And Not e.Item.Pressed And Not e.Item.Selected) Then
                    e.TextColor = _textMenuStripItem
                ElseIf (TypeOf (e.ToolStrip) Is StatusStrip And Not e.Item.Pressed And Not e.Item.Selected) Then
                    e.TextColor = _textStatusStripItem

                Else
                    e.TextColor = _textContextMenuItem
                End If
                Using clearTypeGridFit As UseClearTypeGridFit = New UseClearTypeGridFit(e.Graphics)
                    MyBase.OnRenderItemText(e)
                End Using
            End If
        Else
            MyBase.OnRenderItemText(e)
        End If
    End Sub
#End Region

#Region "OnRenderItemImage"
    ''' <summary>
    ''' MBToolStripRenderer Class Event to Render Item Image
    ''' </summary>
    Protected Overrides Sub OnRenderItemImage(ByVal e As System.Windows.Forms.ToolStripItemImageRenderEventArgs)
        If TypeOf (e.ToolStrip) Is ContextMenuStrip Or TypeOf (e.ToolStrip) Is ToolStripDropDownMenu Then
            If Not (e.Image Is Nothing) Then
                If (e.Item.Enabled) Then
                    e.Graphics.DrawImage(e.Image, e.ImageRectangle)
                Else
                    ControlPaint.DrawImageDisabled(e.Graphics, e.Image, _
                                                   e.ImageRectangle.X, _
                                                   e.ImageRectangle.Y, _
                                                   Color.Transparent)
                End If
            End If
        Else
            MyBase.OnRenderItemImage(e)
        End If
    End Sub
#End Region

#Region "OnRenderMenuItemBackground"
    ''' <summary>
    ''' MBToolStripRenderer Class Event to Render MenuItemBackground
    ''' </summary>
    Protected Overrides Sub OnRenderMenuItemBackground(ByVal e As System.Windows.Forms.ToolStripItemRenderEventArgs)
        If (TypeOf (e.ToolStrip) Is MenuStrip Or _
                 TypeOf (e.ToolStrip) Is ContextMenuStrip Or _
                 TypeOf (e.ToolStrip) Is ToolStripDropDownMenu) Then
            If (e.Item.Pressed) And TypeOf (e.ToolStrip) Is MenuStrip Then
                DrawContextMenuHeader(e.Graphics, e.Item)
            Else
                If (e.Item.Selected) Then
                    If (e.Item.Enabled) Then
                        If TypeOf (e.ToolStrip) Is MenuStrip Then
                            DrawGradientToolItem(e.Graphics, e.Item, _itemToolItemSelectedColors)
                        Else
                            DrawGradientContextMenuItem(e.Graphics, e.Item, _itemContextItemEnabledColors)
                        End If
                    Else
                        Dim mousePos As Point = e.ToolStrip.PointToClient(Control.MousePosition)
                        If (Not e.Item.Bounds.Contains(mousePos)) Then
                            If TypeOf (e.ToolStrip) Is MenuStrip Then
                                DrawGradientToolItem(e.Graphics, e.Item, _itemDisabledColors)
                            Else
                                DrawGradientContextMenuItem(e.Graphics, e.Item, _itemDisabledColors)
                            End If
                        End If
                    End If
                End If
            End If
        Else
            MyBase.OnRenderMenuItemBackground(e)
        End If
    End Sub
#End Region

#Region "OnRenderSplitButtonBackground"
    ''' <summary>
    ''' MBToolStripRenderer Class Event to Render SplitButtonBckground
    ''' </summary>
    Protected Overrides Sub OnRenderSplitButtonBackground(ByVal e As System.Windows.Forms.ToolStripItemRenderEventArgs)
        If (e.Item.Selected Or e.Item.Pressed) Then
            Dim splitButton As ToolStripSplitButton = e.Item
            RenderToolSplitButtonBackground(e.Graphics, splitButton, e.ToolStrip)
            Dim arrowBounds As Rectangle = splitButton.DropDownButtonBounds
            OnRenderArrow(New ToolStripArrowRenderEventArgs(e.Graphics, _
                                                            splitButton, _
                                                            arrowBounds, _
                                                            SystemColors.ControlText, _
                                                            ArrowDirection.Down))
        Else
            MyBase.OnRenderSplitButtonBackground(e)
        End If
    End Sub
#End Region

#Region "OnRenderStatusStripSizingGrip"
    ''' <summary>
    ''' MBToolStripRenderer Class Event to Render Status Strip Sizing Grip
    ''' </summary>
    Protected Overrides Sub OnRenderStatusStripSizingGrip(ByVal e As System.Windows.Forms.ToolStripRenderEventArgs)
        Using darkBrush As SolidBrush = New SolidBrush(_gripDark), lightBrush = New SolidBrush(_gripLight)
            Dim rtl As Boolean = (e.ToolStrip.RightToLeft = RightToLeft.Yes)
            Dim y As Int32 = e.AffectedBounds.Bottom - _gripSize * 2 + 1
            For i As Int32 = _gripLines To 1 Step -1
                Dim x As Int32
                If rtl Then
                    x = e.AffectedBounds.Left + 1
                Else
                    x = e.AffectedBounds.Right - _gripSize * 2 + 1
                End If
                For j As Int32 = 0 To i
                    DrawGripGlyph(e.Graphics, x, y, darkBrush, lightBrush)
                    If rtl Then
                        x -= -_gripMove
                    Else
                        x -= _gripMove
                    End If
                Next
                y -= _gripMove
            Next
        End Using
    End Sub
#End Region

#Region "OnRenderToolStripContentPanelBackground"
    Protected Overrides Sub OnRenderToolStripContentPanelBackground(ByVal e As System.Windows.Forms.ToolStripContentPanelRenderEventArgs)
        MyBase.OnRenderToolStripContentPanelBackground(e)
        If ((e.ToolStripContentPanel.Width > 0) And (e.ToolStripContentPanel.Height > 0)) Then
            Using backBrush As LinearGradientBrush = New LinearGradientBrush(e.ToolStripContentPanel.ClientRectangle, _
                                                                           ColorTable.ToolStripContentPanelGradientEnd, _
                                                                           ColorTable.ToolStripContentPanelGradientBegin, _
                                                                           90.0F)
                e.Graphics.FillRectangle(backBrush, e.ToolStripContentPanel.ClientRectangle)
            End Using
        End If
    End Sub
#End Region

#Region "OnRenderSeparator"
    ''' <summary>
    ''' MBToolStripRenderer Class to Render Seperator
    ''' </summary>
    Protected Overrides Sub OnRenderSeparator(ByVal e As System.Windows.Forms.ToolStripSeparatorRenderEventArgs)
        If (TypeOf (e.ToolStrip) Is ContextMenuStrip Or TypeOf (e.ToolStrip) Is ToolStripDropDownMenu) Then
            Using lightPen As Pen = New Pen(_separatorMenuLight), darkPen = New Pen(_separatorMenuDark)
                DrawSeparator(e.Graphics, e.Vertical, e.Item.Bounds, _
                              lightPen, darkPen, _separatorInset, _
                              (e.ToolStrip.RightToLeft = RightToLeft.Yes))
            End Using
        ElseIf TypeOf (e.ToolStrip) Is StatusStrip Then

            Using lightPen As Pen = New Pen(ColorTable.SeparatorLight), _
                darkPen = New Pen(ColorTable.SeparatorDark)

                DrawSeparator(e.Graphics, e.Vertical, e.Item.Bounds, lightPen, darkPen, 0, False)
            End Using

        Else
            MyBase.OnRenderSeparator(e)
        End If
    End Sub
#End Region

#Region "OnRenderToolStripBackground"
    ''' <summary>
    ''' MBToolStripRenderer Class to Render ToolStrip Background
    ''' </summary>
    Protected Overrides Sub OnRenderToolStripBackground(ByVal e As System.Windows.Forms.ToolStripRenderEventArgs)
        If (TypeOf (e.ToolStrip) Is ContextMenuStrip) Or TypeOf (e.ToolStrip) Is ToolStripDropDownMenu Then
            Using borderPath As GraphicsPath = DrawBorderPath(e.AffectedBounds, _cutContextMenu), _
                                  clipPath = DrawClipBorderPath(e.AffectedBounds, _cutContextMenu)
                Using clipping As UseClipping = New UseClipping(e.Graphics, clipPath)
                    Using backBrush As SolidBrush = New SolidBrush(_contextMenuBack)
                        e.Graphics.FillPath(backBrush, borderPath)
                    End Using
                End Using
            End Using
        ElseIf TypeOf (e.ToolStrip) Is StatusStrip Then
            Dim backRect As RectangleF = New RectangleF(0, 1.5F, e.ToolStrip.Width, e.ToolStrip.Height - 2)
            If ((backRect.Width > 0) And (backRect.Height > 0)) Then
                Using backBrush As LinearGradientBrush = New LinearGradientBrush(backRect, _
                                                                               ColorTable.StatusStripGradientBegin, _
                                                                               ColorTable.StatusStripGradientEnd, _
                                                                               90.0F)
                    backBrush.Blend = _statusStripBlend
                    e.Graphics.FillRectangle(backBrush, backRect)
                End Using
            End If
        Else
            MyBase.OnRenderToolStripBackground(e)
        End If
    End Sub
#End Region

#Region "OnRenderImageMargin"
    ''' <summary>
    ''' MBToolStripRenderer Class Event to Render Image Margin
    ''' </summary>
    Protected Overrides Sub OnRenderImageMargin(ByVal e As System.Windows.Forms.ToolStripRenderEventArgs)
        If (TypeOf (e.ToolStrip) Is ContextMenuStrip Or TypeOf (e.ToolStrip) Is ToolStripDropDownMenu) Then
            Dim marginRect As Rectangle = e.AffectedBounds
            Dim rtl As Boolean = (e.ToolStrip.RightToLeft = RightToLeft.Yes)
            marginRect.Y += _marginInset
            marginRect.Height -= _marginInset * 2
            If (Not rtl) Then
                marginRect.X += _marginInset
            Else
                marginRect.X += _marginInset / 2
            End If
            Using backBrush As SolidBrush = New SolidBrush(ColorTable.ImageMarginGradientBegin)
                e.Graphics.FillRectangle(backBrush, marginRect)
                Using lightPen As Pen = New Pen(_separatorMenuLight), _
                    darkPen = New Pen(_separatorMenuDark)
                    If (Not rtl) Then
                        e.Graphics.DrawLine(lightPen, marginRect.Right, marginRect.Top, marginRect.Right, marginRect.Bottom)
                        e.Graphics.DrawLine(darkPen, marginRect.Right - 1, marginRect.Top, marginRect.Right - 1, marginRect.Bottom)
                    Else
                        e.Graphics.DrawLine(lightPen, marginRect.Left - 1, marginRect.Top, marginRect.Left - 1, marginRect.Bottom)
                        e.Graphics.DrawLine(darkPen, marginRect.Left, marginRect.Top, marginRect.Left, marginRect.Bottom)
                    End If
                End Using
            End Using
        Else
            MyBase.OnRenderImageMargin(e)
        End If
    End Sub
#End Region

#Region "OnRenderToolStripBorder"
    ''' <summary>
    ''' MBToolStripRenderer Class Event to Render ToolStrip Border
    ''' </summary>
    Protected Overrides Sub OnRenderToolStripBorder(ByVal e As System.Windows.Forms.ToolStripRenderEventArgs)
        If (TypeOf (e.ToolStrip) Is ContextMenuStrip Or TypeOf (e.ToolStrip) Is ToolStripDropDownMenu) Then
            If (Not e.ConnectedArea.IsEmpty) Then
                Using excludeBrush As SolidBrush = New SolidBrush(_contextMenuBack)
                    e.Graphics.FillRectangle(excludeBrush, e.ConnectedArea)
                End Using

                Using borderPath As GraphicsPath = DrawBorderPath(e.AffectedBounds, e.ConnectedArea, _cutContextMenu), _
                                    insidePath = DrawInsideBorderPath(e.AffectedBounds, e.ConnectedArea, _cutContextMenu), _
                                      clipPath = DrawClipBorderPath(e.AffectedBounds, e.ConnectedArea, _cutContextMenu)
                    Using borderPen As Pen = New Pen(ColorTable.MenuBorder), _
                               insidePen = New Pen(_separatorMenuLight)
                        Using clipping As UseClipping = New UseClipping(e.Graphics, clipPath)
                            Using uaa As UseAntiAlias = New UseAntiAlias(e.Graphics)
                                e.Graphics.DrawPath(insidePen, insidePath)
                                e.Graphics.DrawPath(borderPen, borderPath)
                            End Using
                            e.Graphics.DrawLine(borderPen, e.AffectedBounds.Right, e.AffectedBounds.Bottom, _
                                                       (e.AffectedBounds.Right - 1), e.AffectedBounds.Bottom - 1)
                        End Using
                    End Using
                End Using

            ElseIf TypeOf (e.ToolStrip) Is StatusStrip Then
                Using darkBorder As Pen = New Pen(_statusStripBorderDark), _
                          lightBorder = New Pen(_statusStripBorderLight)
                    e.Graphics.DrawLine(darkBorder, 0, 0, e.ToolStrip.Width, 0)
                    e.Graphics.DrawLine(lightBorder, 0, 1, e.ToolStrip.Width, 1)
                End Using

            Else
                MyBase.OnRenderToolStripBorder(e)
            End If
        End If
    End Sub
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' RenderToolButtonBackground Method for Button Background Renderring
    ''' </summary>
    ''' <param name="g">graphics As Graphics</param>
    ''' <param name="button">button As ToolStripButton</param>
    ''' <param name="toolstrip">toolstrip As ToolStrip</param>
    Private Sub RenderToolButtonBackground(ByVal g As Graphics, ByVal button As ToolStripButton, ByVal toolstrip As ToolStrip)
        If (button.Enabled) Then
            If (button.Checked) Then
                If (button.Pressed) Then
                    DrawGradientToolItem(g, button, _itemToolItemPressedColors)
                ElseIf (button.Selected) Then
                    DrawGradientToolItem(g, button, _itemToolItemCheckPressColors)
                Else
                    DrawGradientToolItem(g, button, _itemToolItemCheckedColors)
                End If
            Else
                If (button.Pressed) Then
                    DrawGradientToolItem(g, button, _itemToolItemPressedColors)
                ElseIf (button.Selected) Then
                    DrawGradientToolItem(g, button, _itemToolItemSelectedColors)
                End If
            End If
        Else
            If (button.Selected) Then
                Dim mousePos As Point = toolstrip.PointToClient(Control.MousePosition)
                If (Not button.Bounds.Contains(mousePos)) Then DrawGradientToolItem(g, button, _itemDisabledColors)
            End If
        End If
    End Sub
    ''' <summary>
    ''' RendereToolDropButtonBackground Method for DropDown Button Background
    ''' </summary>
    ''' <param name="g">graphics As Graphics</param>
    ''' <param name="item">Item As ToolStripItem</param>
    ''' <param name="toolstrip">toolstrip As ToolStrip</param>
    Private Sub RenderToolDropButtonBackground(ByVal g As Graphics, ByVal item As ToolStripItem, ByVal toolstrip As ToolStrip)
        If (item.Selected Or item.Pressed) Then
            If (item.Enabled) Then
                If (item.Pressed) Then
                    DrawContextMenuHeader(g, item)
                Else
                    DrawGradientToolItem(g, item, _itemToolItemSelectedColors)
                End If
            Else
                Dim mousePos As Point = toolstrip.PointToClient(Control.MousePosition)
                If (Not item.Bounds.Contains(mousePos)) Then
                    DrawGradientToolItem(g, item, _itemDisabledColors)
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' RenderToolSplitButtonBackground Method for Split Button Background
    ''' </summary>
    ''' <param name="g">graphics As Graphics</param>
    ''' <param name="splitButton">Button As ToolStripSplitButton</param>
    ''' <param name="toolstrip">toolStrip As ToolStrip</param>
    Private Sub RenderToolSplitButtonBackground(ByVal g As Graphics, ByVal splitButton As ToolStripSplitButton, _
                                                     ByVal toolstrip As ToolStrip)
        If (splitButton.Selected Or splitButton.Pressed) Then
            If (splitButton.Enabled) Then
                If (Not splitButton.Pressed And splitButton.ButtonPressed) Then
                    DrawGradientToolSplitItem(g, splitButton, _
                                             _itemToolItemPressedColors, _
                                             _itemToolItemSelectedColors, _
                                             _itemContextItemEnabledColors)
                ElseIf (splitButton.Pressed And Not splitButton.ButtonPressed) Then
                    DrawContextMenuHeader(g, splitButton)
                Else
                    DrawGradientToolSplitItem(g, splitButton, _
                                                 _itemToolItemSelectedColors, _
                                                 _itemToolItemSelectedColors, _
                                                 _itemContextItemEnabledColors)
                End If
            Else
                Dim mousePos As Point = toolstrip.PointToClient(Control.MousePosition)
                If (Not splitButton.Bounds.Contains(mousePos)) Then DrawGradientToolItem(g, splitButton, _itemDisabledColors)
            End If
        End If
    End Sub
    ''' <summary>
    ''' Draw Gradient Tool Item for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Sub DrawGradientToolItem(ByVal g As Graphics, ByVal item As ToolStripItem, ByVal colors As GradientItemColor)
        DrawGradientItem(g, New Rectangle(Point.Empty, item.Bounds.Size), colors)
    End Sub
    ''' <summary>
    ''' Draw Gradient Tool Split Item for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Sub DrawGradientToolSplitItem(ByVal g As Graphics, ByVal splitButton As ToolStripSplitButton, _
                                               ByVal colorsButton As GradientItemColor, _
                                               ByVal colorsDrop As GradientItemColor, _
                                               ByVal colorsSplit As GradientItemColor)
        Dim backRect As Rectangle = New Rectangle(Point.Empty, splitButton.Bounds.Size)
        Dim backRectDrop As Rectangle = splitButton.DropDownButtonBounds
        If ((backRect.Width > 0) And (backRectDrop.Width > 0) And _
                (backRect.Height > 0) And (backRectDrop.Height > 0)) Then
            Dim backRectButton As Rectangle = backRect
            Dim splitOffset As Int32
            If (backRectDrop.X > 0) Then

                backRectButton.Width = backRectDrop.Left
                backRectDrop.X -= 1
                backRectDrop.Width += 1
                splitOffset = backRectDrop.X
            Else
                backRectButton.Width -= backRectDrop.Width - 2
                backRectButton.X = backRectDrop.Right - 1
                backRectDrop.Width += 1
                splitOffset = backRectDrop.Right - 1
            End If
            Using borderPath As GraphicsPath = DrawBorderPath(backRect, _cutMenuItemBack)
                DrawGradientBack(g, backRectButton, colorsButton)
                DrawGradientBack(g, backRectDrop, colorsDrop)
                Using splitBrush As LinearGradientBrush = New LinearGradientBrush(New Rectangle(backRect.X + splitOffset, backRect.Top, 1, backRect.Height + 1), _
                                                                                  colorsSplit.Border1, colorsSplit.Border2, 90.0F)
                    splitBrush.SetSigmaBellShape(0.5F)
                    Using splitPen As Pen = New Pen(splitBrush)
                        g.DrawLine(splitPen, backRect.X + splitOffset, backRect.Top + 1, backRect.X + splitOffset, backRect.Bottom - 1)
                    End Using
                End Using
                DrawGradientBorder(g, backRect, colorsButton)
            End Using
        End If
    End Sub
    ''' <summary>
    ''' Draw Gradient ContextMenuHeader for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Sub DrawContextMenuHeader(ByVal g As Graphics, ByVal item As ToolStripItem)
        Dim itemRect As Rectangle = New Rectangle(Point.Empty, item.Bounds.Size)
        Using borderPath As GraphicsPath = DrawBorderPath(itemRect, _cutToolItemMenu), _
                            insidePath = DrawInsideBorderPath(itemRect, _cutToolItemMenu), _
                              clipPath = DrawClipBorderPath(itemRect, _cutToolItemMenu)
            Using clipping As UseClipping = New UseClipping(g, clipPath)
                Using backBrush As SolidBrush = New SolidBrush(_separatorMenuLight)
                    g.FillPath(backBrush, borderPath)
                End Using
                Using borderPen As Pen = New Pen(ColorTable.MenuBorder)
                    g.DrawPath(borderPen, borderPath)
                End Using
            End Using
        End Using
    End Sub
    ''' <summary>
    ''' Draw Gradient ContextMenu Item for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Sub DrawGradientContextMenuItem(ByVal g As Graphics, ByVal item As ToolStripItem, ByVal colors As GradientItemColor)
        Dim backRect As Rectangle = New Rectangle(2, 0, item.Bounds.Width - 3, item.Bounds.Height)
        DrawGradientItem(g, backRect, colors)
    End Sub
    ''' <summary>
    ''' Draw Gradient Item for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Sub DrawGradientItem(ByVal g As Graphics, ByVal backRect As Rectangle, ByVal colors As GradientItemColor)
        If ((backRect.Width > 0) And (backRect.Height > 0)) Then
            DrawGradientBack(g, backRect, colors)
            DrawGradientBorder(g, backRect, colors)
        End If
    End Sub
    ''' <summary>
    ''' Draw Gradient Background for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Sub DrawGradientBack(ByVal g As Graphics, ByVal backRect As Rectangle, ByVal colors As GradientItemColor)
        backRect.Inflate(-1, -1)
        Dim y2 As Int32 = backRect.Height / 2
        Dim backRect1 As Rectangle = New Rectangle(backRect.X, backRect.Y, backRect.Width, y2)
        Dim backRect2 As Rectangle = New Rectangle(backRect.X, backRect.Y + y2, backRect.Width, backRect.Height - y2)
        Dim backRect1I As Rectangle = backRect1
        Dim backRect2I As Rectangle = backRect2
        backRect1I.Inflate(1, 1)
        backRect2I.Inflate(1, 1)
        Using insideBrush1 As LinearGradientBrush = New LinearGradientBrush(backRect1I, colors.InsideTop1, colors.InsideTop2, 90.0F), _
            insideBrush2 = New LinearGradientBrush(backRect2I, colors.InsideBottom1, colors.InsideBottom2, 90.0F)
            g.FillRectangle(insideBrush1, backRect1)
            g.FillRectangle(insideBrush2, backRect2)
        End Using
        y2 = backRect.Height / 2
        backRect1 = New Rectangle(backRect.X, backRect.Y, backRect.Width, y2)
        backRect2 = New Rectangle(backRect.X, backRect.Y + y2, backRect.Width, backRect.Height - y2)
        backRect1I = backRect1
        backRect2I = backRect2
        backRect1I.Inflate(1, 1)
        backRect2I.Inflate(1, 1)
        Using fillBrush1 As LinearGradientBrush = New LinearGradientBrush(backRect1I, colors.FillTop1, colors.FillTop2, 90.0F), _
            fillBrush2 = New LinearGradientBrush(backRect2I, colors.FillBottom1, colors.FillBottom2, 90.0F)
            backRect.Inflate(-1, -1)
            y2 = backRect.Height / 2
            backRect1 = New Rectangle(backRect.X, backRect.Y, backRect.Width, y2)
            backRect2 = New Rectangle(backRect.X, backRect.Y + y2, backRect.Width, backRect.Height - y2)
            g.FillRectangle(fillBrush1, backRect1)
            g.FillRectangle(fillBrush2, backRect2)
        End Using
    End Sub
    ''' <summary>
    ''' Draw Gradient Border for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Sub DrawGradientBorder(ByVal g As Graphics, ByVal backRect As Rectangle, ByVal colors As GradientItemColor)
        Using uaa As UseAntiAlias = New UseAntiAlias(g)
            Dim backRectI As Rectangle = backRect
            backRectI.Inflate(1, 1)
            Using borderBrush As LinearGradientBrush = New LinearGradientBrush(backRectI, colors.Border1, colors.Border2, 90.0F)
                borderBrush.SetSigmaBellShape(0.5F)
                Using borderPen As Pen = New Pen(borderBrush)
                    Using borderPath As GraphicsPath = DrawBorderPath(backRect, _cutMenuItemBack)
                        g.DrawPath(borderPen, borderPath)
                    End Using
                End Using
            End Using
        End Using
    End Sub
    ''' <summary>
    ''' Draw Grip for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Sub DrawGripGlyph(ByVal g As Graphics, ByVal x As Int32, ByVal y As Int32, ByVal darkBrush As Brush, ByVal lightBrush As Brush)
        g.FillRectangle(lightBrush, x + _gripOffset, y + _gripOffset, _gripSquare, _gripSquare)
        g.FillRectangle(darkBrush, x, y, _gripSquare, _gripSquare)
    End Sub
    ''' <summary>
    ''' Draw Gradient Separator for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Overloads Sub DrawSeparator(ByVal g As Graphics, ByVal vertical As Boolean, ByVal rect As Rectangle, ByVal lightPen As Pen, _
                                   ByVal darkPen As Pen, ByVal horizontalInset As Int32, ByVal rtl As Boolean)
        If (vertical) Then
            Dim l As Int32 = rect.Width / 2
            Dim t As Int32 = rect.Y
            Dim b As Int32 = rect.Bottom
            g.DrawLine(darkPen, l, t, l, b)
            g.DrawLine(lightPen, l + 1, t, l + 1, b)
        Else
            Dim y As Int32 = rect.Height / 2
            Dim l As Int32
            If rtl Then
                l = rect.X + 0
            Else
                l = rect.X + horizontalInset
            End If
            Dim r As Int32
            If rtl Then
                r = rect.Right - horizontalInset
            Else
                r = rect.Right - 0
            End If
            g.DrawLine(darkPen, l, y, r, y)
            g.DrawLine(lightPen, l, y + 1, r, y + 1)
        End If
    End Sub
    ''' <summary>
    ''' Draw Gradient Border Path for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Function DrawBorderPath(ByVal rect As Rectangle, ByVal exclude As Rectangle, ByVal cut As Single) As GraphicsPath
        If (exclude.IsEmpty) Then Return DrawBorderPath(rect, cut)
        rect.Width -= 1
        rect.Height -= 1
        Dim pts As List(Of PointF) = New List(Of PointF)
        Dim l As Single = rect.X
        Dim t As Single = rect.Y
        Dim r As Single = rect.Right
        Dim b As Single = rect.Bottom
        Dim x0 As Single = rect.X + cut
        Dim x3 As Single = rect.Right - cut
        Dim y0 As Single = rect.Y + cut
        Dim y3 As Single = rect.Bottom - cut
        Dim cutBack As Single
        If cut = 0.0F Then
            cutBack = 1
        Else
            cutBack = cut
        End If
        If ((rect.Y >= exclude.Top) And (rect.Y <= exclude.Bottom)) Then
            Dim x1 As Single = exclude.X - 1 - cut
            Dim x2 As Single = exclude.Right + cut
            If (x0 <= x1) Then
                pts.Add(New PointF(x0, t))
                pts.Add(New PointF(x1, t))
                pts.Add(New PointF(x1 + cut, t - cutBack))
            Else
                x1 = exclude.X - 1
                pts.Add(New PointF(x1, t))
                pts.Add(New PointF(x1, t - cutBack))
            End If
            If (x3 > x2) Then
                pts.Add(New PointF(x2 - cut, t - cutBack))
                pts.Add(New PointF(x2, t))
                pts.Add(New PointF(x3, t))
            Else
                x2 = exclude.Right
                pts.Add(New PointF(x2, t - cutBack))
                pts.Add(New PointF(x2, t))
            End If
        Else
            pts.Add(New PointF(x0, t))
            pts.Add(New PointF(x3, t))
        End If
        pts.Add(New PointF(r, y0))
        pts.Add(New PointF(r, y3))
        pts.Add(New PointF(x3, b))
        pts.Add(New PointF(x0, b))
        pts.Add(New PointF(l, y3))
        pts.Add(New PointF(l, y0))
        Dim path As GraphicsPath = New GraphicsPath()
        For i As Int32 = 1 To pts.Count - 1
            path.AddLine(pts(i - 1), pts(i))
        Next
        path.AddLine(pts(pts.Count - 1), pts(0))
        Return path
    End Function
    ''' <summary>
    ''' Draw Border Path for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Function DrawBorderPath(ByVal rect As Rectangle, ByVal cut As Single) As GraphicsPath
        rect.Width -= 1
        rect.Height -= 1
        Dim path As GraphicsPath = New GraphicsPath()
        path.AddLine(rect.Left + cut, rect.Top, rect.Right - cut, rect.Top)
        path.AddLine(rect.Right - cut, rect.Top, rect.Right, rect.Top + cut)
        path.AddLine(rect.Right, rect.Top + cut, rect.Right, rect.Bottom - cut)
        path.AddLine(rect.Right, rect.Bottom - cut, rect.Right - cut, rect.Bottom)
        path.AddLine(rect.Right - cut, rect.Bottom, rect.Left + cut, rect.Bottom)
        path.AddLine(rect.Left + cut, rect.Bottom, rect.Left, rect.Bottom - cut)
        path.AddLine(rect.Left, rect.Bottom - cut, rect.Left, rect.Top + cut)
        path.AddLine(rect.Left, rect.Top + cut, rect.Left + cut, rect.Top)
        Return path
    End Function
    ''' <summary>
    ''' Draw InsideBorder Path for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Function DrawInsideBorderPath(ByVal rect As Rectangle, ByVal cut As Single) As GraphicsPath
        rect.Inflate(-1, -1)
        Return DrawBorderPath(rect, cut)
    End Function
    ''' <summary>
    ''' Draw InsideBorder Path for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Function DrawInsideBorderPath(ByVal rect As Rectangle, ByVal exclude As Rectangle, ByVal cut As Single) As GraphicsPath
        rect.Inflate(-1, -1)
        Return DrawBorderPath(rect, exclude, cut)
    End Function
    ''' <summary>
    ''' Draw ClipBorder Path for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Function DrawClipBorderPath(ByVal rect As Rectangle, ByVal cut As Single) As GraphicsPath
        rect.Width += 1
        rect.Height += 1
        Return DrawBorderPath(rect, cut)
    End Function
    ''' <summary>
    ''' Draw ClipBorder Path for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Function DrawClipBorderPath(ByVal rect As Rectangle, ByVal exclude As Rectangle, ByVal cut As Single) As GraphicsPath
        rect.Width += 1
        rect.Height += 1
        Return DrawBorderPath(rect, exclude, cut)
    End Function
    ''' <summary>
    ''' Draw Arrow Path for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Function DrawArrowPath(ByVal item As ToolStripItem, ByVal rect As Rectangle, ByVal direction As ArrowDirection) As GraphicsPath
        Dim x As Int32, y As Int32
        If ((direction = ArrowDirection.Left) Or (direction = ArrowDirection.Right)) Then
            x = rect.Right - (rect.Width - 4) / 2
            y = rect.Y + rect.Height / 2
        Else
            x = rect.X + rect.Width / 2
            y = rect.Bottom - (rect.Height - 3) / 2
            If (TypeOf (item) Is ToolStripDropDownButton) And (item.RightToLeft = RightToLeft.Yes) Then x += 1
        End If
        Dim path As GraphicsPath = New GraphicsPath()
        Select Case (direction)
            Case ArrowDirection.Right
                path.AddLine(x, y, x - 4, y - 4)
                path.AddLine(x - 4, y - 4, x - 4, y + 4)
                path.AddLine(x - 4, y + 4, x, y)
            Case ArrowDirection.Left
                path.AddLine(x - 4, y, x, y - 4)
                path.AddLine(x, y - 4, x, y + 4)
                path.AddLine(x, y + 4, x - 4, y)
            Case ArrowDirection.Down
                path.AddLine(x + 3.0F, y - 3.0F, x - 2.0F, y - 3.0F)
                path.AddLine(x - 2.0F, y - 3.0F, x, y)
                path.AddLine(x, y, x + 3.0F, y - 3.0F)
            Case ArrowDirection.Up
                path.AddLine(x + 3.0F, y, x - 3.0F, y)
                path.AddLine(x - 3.0F, y, x, y - 4.0F)
                path.AddLine(x, y - 4.0F, x + 3.0F, y)
        End Select
        Return path
    End Function
    ''' <summary>
    ''' Draw Tick Path for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Function DrawTickPath(ByVal rect As Rectangle) As GraphicsPath
        Dim x As Int32 = rect.X + rect.Width / 2
        Dim y As Int32 = rect.Y + rect.Height / 2
        Dim path As GraphicsPath = New GraphicsPath()
        path.AddLine(x - 4, y, x - 2, y + 4)
        path.AddLine(x - 2, y + 4, x + 3, y - 5)
        Return path
    End Function
    ''' <summary>
    ''' Draw Indermidiate Path for MBToolStrip/MBStatusStrip
    ''' </summary>
    Private Function DrawIndeterminatePath(ByVal rect As Rectangle) As GraphicsPath
        Dim x As Int32 = rect.X + rect.Width / 2
        Dim y As Int32 = rect.Y + rect.Height / 2
        Dim path As GraphicsPath = New GraphicsPath()
        path.AddLine(x - 3, y, x, y - 3)
        path.AddLine(x, y - 3, x + 3, y)
        path.AddLine(x + 3, y, x, y + 3)
        path.AddLine(x, y + 3, x - 3, y)
        Return path
    End Function

#End Region

End Class

#End Region