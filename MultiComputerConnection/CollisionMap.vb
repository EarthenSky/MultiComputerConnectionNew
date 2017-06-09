Public Structure PixelPoint  'Holds a point and a value, values is used to connect to other pixel points and made the walls of the collidermap.
    Sub New(ByVal pnt As Point, ByVal val As Short)
        Me.pnt = pnt
        Me.val = val
    End Sub

    Public pnt As Point
    Public val As Short
End Structure

Public Structure Line  'A Line has two points 
    Sub New(ByVal pnt1 As Point, ByVal pnt2 As Point)
        Me.pnt1 = pnt1
        Me.pnt2 = pnt2
    End Sub

    Public pnt1 As Point
    Public pnt2 As Point
End Structure

Public Class PosList  'Is a list but also has the bounds of the area it encloses.
    Sub New(ByVal lstPnt As List(Of PixelPoint), ByVal rect As Rectangle)
        Me.lstPnt = lstPnt
        Me.rect = rect
    End Sub

    Public lstPnt As New List(Of PixelPoint)  'Holds the red pixel points
    Public rect As Rectangle

    Public lstLines As New List(Of Line)  'holds collider lines
End Class

'When making a map put the highest R value at the start of a specific box, like a line through the box
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
                    Debug.Print(bmpCollision.GetPixel(index, jIndex).R.ToString & ", is red")
                End If
            Next
        Next

        MakeLines()

    End Sub

    Public Sub AddRedPixelToList(ByVal pxPnt As PixelPoint)
        Dim shtListIndexX = 0
        Dim shtListIndexY = 0

        'Find x 
        For index As Short = 0 To lstLstSections.Count - 1  'TODO: This is slightly inefficient, only loop the x ones if possible.
            If pxPnt.pnt.X > lstLstSections(index).rect.X Then
                shtListIndexX = lstLstSections(index).rect.X
            End If
        Next

        'Find y
        For index As Short = 0 To lstLstSections.Count - 1  'This is slightly inefficient
            If pxPnt.pnt.Y > lstLstSections(index).rect.Y Then
                shtListIndexY = lstLstSections(index).rect.Y
            End If
        Next

        'Find list
        For index As Short = 0 To lstLstSections.Count - 1
            If lstLstSections(index).rect.Y = shtListIndexY And lstLstSections(index).rect.X = shtListIndexX Then
                lstLstSections(index).lstPnt.Add(pxPnt)
                Exit Sub  'After the last line it is done
            End If
        Next
    End Sub

    Public Sub MakeLines()
        For index As Short = 0 To lstLstSections.Count - 1  'Each loop is making lines inside a section.
            SortByVal(lstLstSections(index).lstPnt)
            For jIndex As Short = lstLstSections(index).lstPnt.Count - 1 To 1 Step -1 'Each loop makes a line
                lstLstSections(index).lstLines.Add(New Line(lstLstSections(index).lstPnt(jIndex).pnt, lstLstSections(index).lstPnt(jIndex - 1).pnt))
            Next
        Next
    End Sub

    Private Sub SortByVal(ByRef lst As List(Of PixelPoint))  'Yes.  Better than my other sort
        'makes temp equal to the list box
        Dim temp As New List(Of PixelPoint)
        For index As Short = 0 To lst.Count - 1 Step 1
            temp.Add(New PixelPoint(lst(index).pnt, lst(index).val))
        Next

        Dim lstShortTemp As New List(Of PixelPoint)
        Dim shtSmallest As Short = Short.MaxValue
        Dim index2 As Short = 0

        While temp.Count - 1 >= 0
            shtSmallest = Short.MaxValue
            index2 = 0

            For index As Short = 0 To temp.Count - 1 Step 1
                If temp(index).val < shtSmallest Then
                    shtSmallest = temp(index).val
                    index2 = index
                End If
            Next

            lstShortTemp.Add(New PixelPoint(temp(index2).pnt, temp(index2).val))
            temp.RemoveAt(index2)
        End While

        For index As Short = 0 To lst.Count - 1 Step 1
            lst(index) = lstShortTemp(index)
        Next

    End Sub

    Public Sub Collision(ByVal pnt As Point)

    End Sub

    Public Sub Draw()

    End Sub
End Class
