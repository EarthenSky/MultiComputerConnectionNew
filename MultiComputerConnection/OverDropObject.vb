Public Structure CircleBox
    Sub New(ByVal pntTemp As Point, ByVal sngRadiusTemp As Single)
        pnt = pntTemp
        sngRadius = sngRadiusTemp
    End Sub

    Public pnt As Point
    Public sngRadius As Single
End Structure

Public Class OverDropObject
    Public lstPointPosition As New List(Of CircleBox) 'Holds all of the circles.
    Public imgMainImage As Image

    Sub New(ByVal pnt As Point, ByVal img As Image)
        lstPointPosition.Add(New CircleBox(pnt, 64)) 'TODO: this presets radius to 1
        imgMainImage = img
    End Sub

    Public Function GetDrawPoint() As Point
        If lstPointPosition.Count = 1 Then  'TODO: take this away in the future, this is debug
            Return lstPointPosition(0).pnt
        End If
    End Function

    Public Function GetMainPointMiddle() As CircleBox
        If lstPointPosition.Count = 1 Then
            Return New CircleBox(New Point(lstPointPosition(0).pnt.X + (imgMainImage.Width / 2), lstPointPosition(0).pnt.Y + (imgMainImage.Height / 2)), lstPointPosition(0).sngRadius)
        ElseIf lstPointPosition.Count > 1 Then
            Return lstPointPosition(0) 'TODO: NOT DONE YET, THIS SHOULD OUTPUT THE MAIN POINT (THAT THE OBJECT TURNS AROUND?)
        End If

    End Function

    Public Sub SetMainPoint(ByVal pnt As Point)
        lstPointPosition(0) = New CircleBox(pnt, lstPointPosition(0).sngRadius)
    End Sub
End Class
