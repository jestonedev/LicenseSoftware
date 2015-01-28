#Region "Import"
Imports System
Imports System.Drawing
Imports System.Drawing.Text
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Runtime.InteropServices

#End Region

#Region "UseAntiAlias Class"

Friend Class UseAntiAlias
    Implements IDisposable
    Private disposedValue As Boolean = False        ' To detect redundant calls
    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free other state (managed objects).
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

#Region "Instance Fields"
    Private _g As Graphics
    Private _old As SmoothingMode
#End Region

    Public Sub New(ByVal g As Graphics)
        _g = g
        _old = _g.SmoothingMode
        _g.SmoothingMode = SmoothingMode.AntiAlias
    End Sub

    Protected Overrides Sub Finalize()
        _g.SmoothingMode = _old
        MyBase.Finalize()
    End Sub

End Class

#End Region

#Region "UseClearTypeGridFit Class"

Friend Class UseClearTypeGridFit
    Implements IDisposable

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free other state (managed objects).
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

#Region "Instance Fields"
    Private _g As Graphics
    Private _old As TextRenderingHint
#End Region

    Public Sub New(ByVal g As Graphics)
        _g = g
        _old = _g.TextRenderingHint
        _g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit
    End Sub

    Protected Overrides Sub Finalize()
        _g.TextRenderingHint = _old
        MyBase.Finalize()
    End Sub

End Class

#End Region

#Region "Use Clipping Class"

Friend Class UseClipping
    Implements IDisposable

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free other state (managed objects).
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

#Region "Instance Fields"
    Private _g As Graphics
    Private _old As Region
#End Region
    ''' <summary>
    ''' Constructor for UsingClipping Class
    ''' </summary>
    Public Sub New(ByVal g As Graphics, ByVal path As GraphicsPath)
        _g = g
        _old = g.Clip
        Dim clip As Region = _old.Clone()
        clip.Intersect(path)
        _g.Clip = clip
    End Sub

    Public Sub New(ByVal g As Graphics, ByVal region As Region)
        _g = g
        _old = g.Clip
        Dim clip As Region = _old.Clone()
        clip.Intersect(region)
        _g.Clip = clip
    End Sub

    Protected Overrides Sub Finalize()
        _g.Clip = _old
        MyBase.Finalize()
    End Sub

End Class

#End Region

