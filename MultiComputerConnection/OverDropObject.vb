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
        lstPointPosition.Add(New CircleBox(pnt, 5)) 'TODO: this presets radius to 1
        imgMainImage = img
    End Sub

    Public Function GetMainPoint() As CircleBox
        If lstPointPosition.Count = 1 Then
            Return lstPointPosition(0)
        ElseIf lstPointPosition.Count > 1 Then
            Return lstPointPosition(0) 'TODO: NOT DONE YET, THIS SHOULD OUTPUT THE MAIN POINT (THAT THE OBJECT TURNS AROUND?)
        End If
    End Function

    Public Sub SetMainPoint(ByVal pnt As Point)
        lstPointPosition(0) = New CircleBox(pnt, lstPointPosition(0).sngRadius)
    End Sub
End Class
