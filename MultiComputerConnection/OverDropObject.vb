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
    Public pntLastPos As Point

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal shtRadius As Short)
        lstPointPosition.Add(New CircleBox(pnt, shtRadius))
        imgMainImage = img
    End Sub

    Public Function GetDrawPoint(ByVal shtIndex As Short) As Point
        If lstPointPosition.Count >= 1 Then  'TODO: take this away in the future, this is debug
            Return lstPointPosition(shtIndex).pnt
        End If
    End Function

    Public Function GetMainPoint(ByVal shtIndex As Short) As CircleBox
        If lstPointPosition.Count >= 1 Then  'TODO: take this away in the future, this is debug
            'Return New CircleBox(New Point(lstPointPosition(shtIndex).pnt.X + (imgMainImage.Width / 2), lstPointPosition(shtIndex).pnt.Y + (imgMainImage.Height / 2)), lstPointPosition(shtIndex).sngRadius)
            Return New CircleBox(New Point(lstPointPosition(shtIndex).pnt.X, lstPointPosition(shtIndex).pnt.Y), lstPointPosition(shtIndex).sngRadius)
        End If
    End Function

    Public Sub SetMainPoint(ByVal pnt As Point)
        pntLastPos = lstPointPosition(0).pnt
        lstPointPosition(0) = New CircleBox(pnt, lstPointPosition(0).sngRadius)
    End Sub
End Class
