Public Class CollisionMap 'Instantiate to make a collider map of lines, call the collide method to check it's collision with a circle.

    Sub New(ByVal drawMap As Image, ByVal colMap As Image)
        LoadMap(drawMap, colMap)
    End Sub

    Private Sub LoadMap(ByVal drawMap As Image, ByVal colMap As Image)
        Dim bmpCollision As New Bitmap(colMap)

        'Check for Green
        For index As Short = 0 To colMap.Width - 1
            If bmpCollision.GetPixel(index, 0).G > 0 Then
                Debug.Print("cut at : " & index)
                'TODO: Actually cut it
            End If
        Next

        'Check for Blue
        For index As Short = 0 To colMap.Height - 1
            If bmpCollision.GetPixel(0, index).B > 0 Then
                Debug.Print("cut at : " & index)
                'TODO: Actually cut it
            End If
        Next

    End Sub

    Public Sub Collision()

    End Sub

    Public Sub Draw()

    End Sub
End Class
