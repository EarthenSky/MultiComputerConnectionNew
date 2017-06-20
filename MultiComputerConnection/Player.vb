Public Class Player
    Inherits OverDropObject  'TODO: make animation

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal shtRadius As Short)
        MyBase.New(pnt, img, shtRadius)
    End Sub

    Public blnWDown As Boolean = False
    Public blnADown As Boolean = False
    Public blnSDown As Boolean = False
    Public blnDDown As Boolean = False

    Public pntMyMousePos As Point

    Public Sub PushBack(ByVal pntPushBack As Point)
        'TODO: stuffs
    End Sub

    Public Sub SetKeyPressed(ByVal key As Char)
        If key = "W" Then
            blnWDown = True
        ElseIf key = "A" Then
            blnADown = True
        ElseIf key = "S" Then
            blnSDown = True
        ElseIf key = "D" Then
            blnDDown = True
        End If
    End Sub

    Public Sub SetKeyUp(ByVal key As Char)
        If key = "W" Then
            blnWDown = False
        ElseIf key = "A" Then
            blnADown = False
        ElseIf key = "S" Then
            blnSDown = False
        ElseIf key = "D" Then
            blnDDown = False
        End If
    End Sub
End Class
