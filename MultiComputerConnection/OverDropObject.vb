Public Structure CircleBox
    Sub New(ByVal pntTemp As Point, ByVal sngRadiusTemp As Single)
        pnt = pntTemp
        sngRadius = sngRadiusTemp
    End Sub

    Public pnt As Point
    Public sngRadius As Single
End Structure

Public Class OverDropObject
    'The thing that happened with GetMainPoint() and GetDrawPoint() was really weird and also accidental.
    'GetMainPoint() was supposed to get the centre but now it acts as a way to get the radius.  IDK how it got like this but it's too late to change it.
    'Also the inheritance thing is sort of weird too, sorry.

    Public lstPointPosition As New List(Of CircleBox) 'Holds all of the circles.  'This is also sort of weird and should have not been a list.
    Public imgMainImage As Image
    Public pntLastPos As Point

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal shtRadius As Short)
        lstPointPosition.Add(New CircleBox(pnt, shtRadius))
        imgMainImage = img
    End Sub

    Public Function GetDrawPoint(ByVal shtIndex As Short) As Point 'Only gets the top left corner point
        Return lstPointPosition(shtIndex).pnt
    End Function

    Public Function GetMainPoint(ByVal shtIndex As Short) As CircleBox  'This gets the top right corner as a circlebox so it also has radius.
        Return New CircleBox(New Point(lstPointPosition(shtIndex).pnt.X, lstPointPosition(shtIndex).pnt.Y), lstPointPosition(shtIndex).sngRadius)
    End Function

    Public Sub SetMainPoint(ByVal pnt As Point)
        pntLastPos = lstPointPosition(0).pnt
        lstPointPosition(0) = New CircleBox(pnt, lstPointPosition(0).sngRadius)
    End Sub
End Class
