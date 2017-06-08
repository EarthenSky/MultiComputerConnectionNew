Public Structure PixelPoint  'Holds a point and a value, values is used to connect to other pixel points and made the walls of the collidermap.
    Sub New(ByVal pnt As Point, ByVal val As Short)
        Me.pnt = pnt
        Me.val = val
    End Sub

    Public pnt As Point
    Public val As Short
End Structure

Public Structure PosList  'Is a list but also has the bounds of the area it encloses.
    Sub New(ByVal lstPnt As List(Of PixelPoint), ByVal rect As Rectangle)
        Me.lstPnt = lstPnt
        Me.rect = rect
    End Sub

    Public lstPnt As List(Of PixelPoint)
    Public rect As Rectangle
End Structure

Public Class CollisionMap 'Instantiate to make a collider map of lines, call the collide method to check it's collision with a circle.

    Public lstLstSections As List(Of PosList) 'Shit just got real

    Sub New(ByVal drawMap As Image, ByVal colMap As Image)
        LoadMap(drawMap, colMap)
    End Sub

    Private Sub LoadMap(ByVal drawMap As Image, ByVal colMap As Image)
        Dim bmpCollision As New Bitmap(colMap)

        Dim shtLastIndex As Short = 0

        'Check for Green
        For index As Short = 0 To colMap.Width - 1
            If bmpCollision.GetPixel(index, 0).G > 0 Then
                Debug.Print("cut at : " & index)

                lstLstSections.Add(New PosList(New List(Of PixelPoint), New Rectangle(shtLastIndex, 0, 0, 0)))  'TODO: SET BOUNDS OF RECT SOMEHOW
                shtLastIndex = index
            End If
        Next

        shtLastIndex = 0

        'Check for Blue
        For index As Short = 0 To colMap.Height - 1
            If bmpCollision.GetPixel(0, index).B > 0 Then
                Debug.Print("cut at : " & index)

                lstLstSections.Add(New PosList())  'TODO: SET BOUNDS OF RECT SOMEHOW
                shtLastIndex = index
            End If
        Next

    End Sub

    Public Sub Collision()

    End Sub

    Public Sub Draw()

    End Sub
End Class
