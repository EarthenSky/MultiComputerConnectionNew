Public Structure PixelPoint  'Holds a point and a value, values is used to connect to other pixel points and made the walls of the collidermap.
    Sub New(ByVal pnt As Point, ByVal val As Short)
        Me.pnt = pnt
        Me.val = val
    End Sub

    Public pnt As Point
    Public val As Short
End Structure

Public Class PosList  'Is a list but also has the bounds of the area it encloses.
    Sub New(ByVal lstPnt As List(Of PixelPoint), ByVal rect As Rectangle)
        Me.lstPnt = lstPnt
        Me.rect = rect
    End Sub

    Public lstPnt As List(Of PixelPoint)
    Public rect As Rectangle
End Class

Public Class CollisionMap 'Instantiate to make a collider map of lines, call the collide method to check it's collision with a circle.

    Public lstLstSections As New List(Of PosList) 'Shit just got real
    Public arySectionsX As New List(Of Short)
    Public arySectionsY As New List(Of Short)

    Sub New(ByVal drawMap As Image, ByVal colMap As Image)
        LoadMap(drawMap, colMap)
    End Sub

    Private Sub LoadMap(ByVal drawMap As Image, ByVal colMap As Image)
        Dim bmpCollision As New Bitmap(colMap)

        Dim shtLastPoint As Short = 0

        'Check for Green
        For index As Short = 0 To colMap.Width - 1
            If bmpCollision.GetPixel(index, 0).G > 0 OrElse index = colMap.Width - 1 Then
                arySectionsX.Add(shtLastPoint)
                shtLastPoint = index
            End If
        Next

        shtLastPoint = 0

        'Check for Blue
        For index As Short = 0 To colMap.Height - 1
            If bmpCollision.GetPixel(0, index).B > 0 OrElse index = colMap.Height - 1 Then
                arySectionsY.Add(shtLastPoint)
                shtLastPoint = index

            End If
        Next

        'Set Boxes
        For index As Short = 0 To arySectionsY.Count - 1
            For jIndex As Short = 0 To arySectionsX.Count - 1
                lstLstSections.Add(New PosList(New List(Of PixelPoint), New Rectangle(arySectionsX(jIndex), arySectionsY(index), 0, 0)))  'TODO: MAYBE NOT USE RECT  'TODO: SET BOUNDS OF RECT SOMEHOW
            Next
        Next

        'Check for Red
        For index As Short = 0 To colMap.Width - 1
            For jIndex As Short = 0 To colMap.Height - 1
                If bmpCollision.GetPixel(index, jIndex).R > 0 Then
                    AddRedPixelToList(New PixelPoint(New Point(index, jIndex), bmpCollision.GetPixel(index, jIndex).R))
                    Debug.Print("hmm")
                End If
            Next
        Next
    End Sub

    Public Sub AddRedPixelToList(ByVal pxPnt As PixelPoint)
        Dim shtListIndexX = 0
        Dim shtListIndexY = 0

        'Find x 
        For index As Short = 0 To arySectionsX.Count - 1
            If pxPnt.pnt.X > lstLstSections(index).rect.X Then
                shtListIndexX = lstLstSections(index).rect.X
            End If
        Next

        'Find y
        For index As Short = 0 To arySectionsX.Count - 1
            If pxPnt.pnt.Y > lstLstSections(index).rect.Y Then
                shtListIndexY = lstLstSections(index).rect.Y
            End If
        Next

        'Find list
        For index As Short = 0 To lstLstSections.Count - 1
            If lstLstSections(index).rect.Y = pxPnt.pnt.Y And lstLstSections(index).rect.X = pxPnt.pnt.X Then
                lstLstSections(index).lstPnt.Add(pxPnt) 'TODO: NEVER GETS HERE!!!
            End If
        Next
    End Sub

    Public Sub Collision()

    End Sub

    Public Sub Draw()

    End Sub
End Class
