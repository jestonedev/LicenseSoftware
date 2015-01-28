Imports System
Imports System.Collections.Generic
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Data
Imports System.Text
Imports System.Windows.Forms
Imports System.Threading

Namespace EasyRenderer

#Region "EasyRender Base Render Class"

    Public Class EasyRender
        Inherits ToolStripProfessionalRenderer

#Region "Initialize and Setup"

        Public Sub New()

        End Sub

#Region "Private Variable Declaration"
        Private _tsManager As IToolStrip = Nothing
		private  _btnManager As IButton= null;
		private IToolstripControls _tsCtrlManager = null;
		private IPanel _pManager = null;
		private ISplitButton _sBtnManager = null;
		private IStatusBar _sBarManager = null;
		private IMenustrip _mnuManager = null;
		private IDropDownButton _dBtnManager = null;

		private Boolean _smoothText = true;
		private Color _overrideColor = Color.FromArgb(47, 92, 150);
		private Boolean _overrideText = true;
#End Region
#End Region

       

    End Class

#End Region

    Public Class IToolStrip

        Public Sub New(ByVal Import As IToolStrip)
            Apply(Import)
        End Sub

        Protected Overrides Sub Finalize()
            GC.SuppressFinalize(Me)
        End Sub

    End Class



#Region "ToolStrip Extend Control"

    Public Class IToolStripControl

#Region "Initialize and Finalize"

        Public Sub New()

        End Sub

        Protected Overrides Sub Finalize()
            GC.SuppressFinalize(Me)
            MyBase.Finalize()
        End Sub

#End Region

#Region "Private Variable Declaration"

        Private _sepDark As Color = Color.FromArgb(154, 198, 255)
        Private _sepLight As Color = Color.White
        Private _sepHeight As Int32 = 8

        Private _gripTop As Color = Color.FromArgb(111, 157, 217)
        Private _gripBottom As Color = Color.White
        Private _gripStyle As GripType = GripType.Dotted
        Private _gripDistance As Int32 = 4
        Private _gripSize As Size = New Size(2, 2)

#End Region

#Region "Properties"

        Public Property SeperatorDark() As Color
            Get
                Return _sepDark
            End Get
            Set(ByVal value As Color)
                _sepDark = value
            End Set
        End Property

        Public Property SeperatorLight() As Color
            Get
                Return _sepLight
            End Get
            Set(ByVal value As Color)
                _sepLight = value
            End Set
        End Property

        Public Property SeperatorHeight() As Int32
            Get
                Return _sepHeight
            End Get
            Set(ByVal value As Int32)
                _sepHeight = value
            End Set
        End Property

        Public Property GripTop() As Color
            Get
                Return _gripTop
            End Get
            Set(ByVal value As Color)
                _gripTop = value
            End Set
        End Property

        Public Property GripShadow() As Color
            Get
                Return _gripBottom
            End Get
            Set(ByVal value As Color)
                _gripBottom = value
            End Set
        End Property

        Public Property GripStyle() As GripType
            Get
                Return _gripStyle
            End Get
            Set(ByVal value As GripType)
                _gripStyle = value
            End Set
        End Property

        Public Property GripDistance() As Int32
            Get
                Return _gripDistance
            End Get
            Set(ByVal value As Int32)
                _gripDistance = value
            End Set
        End Property

        Public Property GripSize() As Size
            Get
                Return _gripSize
            End Get
            Set(ByVal value As Size)
                _gripSize = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub Apply(ByVal Import As IToolStripControl)
            _sepDark = Import._sepDark
            _sepLight = Import._sepLight
            _sepHeight = Import._sepHeight
            _gripTop = Import._gripTop
            _gripBottom = Import._gripBottom
            _gripDistance = Import._gripDistance
            _gripStyle = Import._gripStyle
            _gripSize = Import._gripSize
        End Sub

#End Region

    End Class

#End Region

    Public Enum GripType
        Dotted
        Lines
        None
    End Enum


    Public Class IButton

#Region "Initialize and Finalize"

        Public Sub New()

        End Sub

        Public Sub New(ByVal _Button As IButton)

        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

#End Region

#Region "Declaration of Private Variables"

        Private _borderTop As Color = Color.FromArgb(157, 183, 217)
        Private _borderBottom As Color = Color.FromArgb(157, 183, 217)
        Private _borderInner As Color = Color.FromArgb(255, 247, 185)
        Private _borderBlend As Blend = Nothing
        Private _borderAngle As Single = 90.0F
        Private _hoverBackTop As Color = Color.FromArgb(255, 249, 218)
        Private _hoverBackBottom As Color = Color.FromArgb(237, 189, 62)
        Private _clickBackTop As Color = Color.FromArgb(245, 207, 57)
        Private _clickBackBottom As Color = Color.FromArgb(245, 225, 124)
        Private _backAngle As Single = 90.0F
        Private _backBlend As Blend = Nothing
        Private _blendRender As BlendRender = BlendRender.Hover Or BlendRender.Click Or BlendRender.Check
        Private _curve As Int32 = 1

#End Region

#Region "Properties"

        Public Property HoverBackgroundTop() As Color
            Get
                Return _hoverBackTop
            End Get
            Set(ByVal value As Color)
                _hoverBackTop = value
            End Set
        End Property

        Public Property HoverBackgroundBottom() As Color
            Get
                Return _hoverBackBottom
            End Get
            Set(ByVal value As Color)
                _hoverBackBottom = value
            End Set
        End Property

        Public Property ClickBackgroundTop() As Color
            Get
                Return _clickBackTop
            End Get
            Set(ByVal value As Color)
                _clickBackTop = value
            End Set
        End Property

#End Region

#Region "Methods"

#End Region

#Region "Eumerations"

        Public Enum BlendRender
            Normal
            Hover
            Click
            Check
            All = Normal Or Hover Or Click Or Check
        End Enum

#End Region
    End Class

End Namespace
