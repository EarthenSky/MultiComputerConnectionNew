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

Public Class PosList  'Is a list but also has the bounds of the area it encloses. (holds the lines)
    Sub New(ByVal lstPnt As List(Of PixelPoint), ByVal rect As Rectangle)
        Me.lstPnt = lstPnt
        Me.rect = rect
    End Sub

    Public lstPnt As New List(Of PixelPoint)  'Holds the red pixel points
    Public rect As Rectangle

    Public lstLines As New List(Of Line)  'holds collider lines
End Class

'When making a map put the highest Red value at the start of a specific box, like a line through the box
Public Class CollisionMap 'Instantiate to make a collider map of lines, call the collide method to check it's collision with a circle.
    Public lstLstSections As New List(Of PosList) 'Shit just got real with this one.
    Public arySectionsX As New List(Of Short)
    Public arySectionsY As New List(Of Short)
    Private imgDrawMap As Image

    Sub New(ByVal drawMap As Image, ByVal colMap As Image)
        'Debug.Print(FindMidPoint(New Point(1, 3), New Point(3, 1)).ToString())
        LoadMap(colMap)
        imgDrawMap = drawMap
    End Sub

    Private Sub LoadMap(ByVal colMap As Image)
        Dim bmpCollision As New Bitmap(colMap)

        'Im not really using the green blue things, depricated. (spelling?)
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
                lstLstSections.Add(New PosList(New List(Of PixelPoint), New Rectangle(arySectionsX(jIndex) * shtColliderMapScale, arySectionsY(index) * shtColliderMapScale, 0, 0)))  'TODO: MAYBE NOT USE RECT  'TODO: SET BOUNDS OF RECT SOMEHOW
            Next
        Next

        'Check for Red
        For index As Short = 0 To colMap.Width - 1
            For jIndex As Short = 0 To colMap.Height - 1
                If bmpCollision.GetPixel(index, jIndex).R > 0 Then
                    AddRedPixelToList(New PixelPoint(New Point(index, jIndex), bmpCollision.GetPixel(index, jIndex).R))
                    'Debug.Print(bmpCollision.GetPixel(index, jIndex).R.ToString & ", is red")
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

    Dim shtColliderMapScale As Short = 8
    Dim shtExtraLineLength As Single = 1.5
    Public Sub MakeLines()
        For index As Short = 0 To lstLstSections.Count - 1  'Each loop is making lines inside a section.
            SortByVal(lstLstSections(index).lstPnt)
            For jIndex As Short = lstLstSections(index).lstPnt.Count - 1 To 1 Step -1 'Each loop makes a line
                lstLstSections(index).lstLines.Add(New Line(New Point(lstLstSections(index).lstPnt(jIndex).pnt.X * shtColliderMapScale,
                                                                      lstLstSections(index).lstPnt(jIndex).pnt.Y * shtColliderMapScale),
                                                            New Point(lstLstSections(index).lstPnt(jIndex - 1).pnt.X * shtColliderMapScale,
                                                                      lstLstSections(index).lstPnt(jIndex - 1).pnt.Y * shtColliderMapScale)))
            Next
            lstLstSections(index).lstLines.Add(New Line(New Point(lstLstSections(index).lstPnt(lstLstSections(index).lstPnt.Count - 1).pnt.X * shtColliderMapScale,
                                                                  lstLstSections(index).lstPnt(lstLstSections(index).lstPnt.Count - 1).pnt.Y * shtColliderMapScale),
                                                            New Point(lstLstSections(index).lstPnt(0).pnt.X * shtColliderMapScale,
                                                                      lstLstSections(index).lstPnt(0).pnt.Y * shtColliderMapScale)))
        Next
    End Sub

    Private Sub SortByVal(ByRef lst As List(Of PixelPoint))  'Better than my other sort
        'makes temp equal to the list box
        Dim lstTemp As New List(Of PixelPoint)
        For index As Short = 0 To lst.Count - 1 Step 1
            lstTemp.Add(New PixelPoint(lst(index).pnt, lst(index).val))
        Next

        Dim lstShortTemp As New List(Of PixelPoint)
        Dim shtSmallest As Short = Short.MaxValue
        Dim jIndex As Short = 0

        While lstTemp.Count - 1 >= 0
            shtSmallest = Short.MaxValue
            jIndex = 0

            For index As Short = 0 To lstTemp.Count - 1 Step 1
                If lstTemp(index).val < shtSmallest Then
                    shtSmallest = lstTemp(index).val
                    jIndex = index
                End If
            Next

            lstShortTemp.Add(New PixelPoint(lstTemp(jIndex).pnt, lstTemp(jIndex).val))
            lstTemp.RemoveAt(jIndex)
        End While

        For index As Short = 0 To lst.Count - 1 Step 1
            lst(index) = lstShortTemp(index)
        Next

    End Sub

    Public Function CheckCollision(ByVal cbx As CircleBox, ByVal pntLast As Point) As Point 'Call this to check collision between a circle and the map.  'Sends back distance to move.
        Dim xPush As Short = 0
        Dim yPush As Short = 0

        CollisionLoop(cbx, xPush, yPush, pntLast)

        Return New Point(xPush, yPush)
    End Function

    Public Sub CollisionLoop(ByVal cbx As CircleBox, ByRef xPush As Short, ByRef yPush As Short, ByVal pntLast As Point)
        Dim shtDiv As Short = 0

        For index As Short = 0 To lstLstSections(FindSection(cbx)).lstLines.Count - 1  'Loops through all of the lines in the specified list.
            If DistanceToSegment(cbx.pnt, lstLstSections(FindSection(cbx)).lstLines(index).pnt1, lstLstSections(FindSection(cbx)).lstLines(index).pnt2) < cbx.sngRadius Then
                Dim temp As Point = PushBack(cbx, lstLstSections(FindSection(cbx)).lstLines(index).pnt1, lstLstSections(FindSection(cbx)).lstLines(index).pnt2, pntLast)
                xPush += temp.X
                yPush += temp.Y
                shtDiv += 1
            End If
        Next

        If shtDiv > 0 Then
            xPush /= shtDiv
            yPush /= shtDiv
        End If
    End Sub

    Public Function FindSection(ByVal cbx As CircleBox) As Short 'Returns "Index" of list.
        Dim shtListIndexX = 0
        Dim shtListIndexY = 0

        'Find x 
        For index As Short = 0 To lstLstSections.Count - 1  'TODO: This is slightly inefficient, only loop the x ones if possible.  Actually this entire proccess, there is probably a better way.
            If cbx.pnt.X > lstLstSections(index).rect.X Then
                shtListIndexX = lstLstSections(index).rect.X
            End If
        Next

        'Find y
        For index As Short = 0 To lstLstSections.Count - 1  'This is slightly inefficient
            If cbx.pnt.Y > lstLstSections(index).rect.Y Then
                shtListIndexY = lstLstSections(index).rect.Y
            End If
        Next

        'Find list
        For index As Short = 0 To lstLstSections.Count - 1
            If lstLstSections(index).rect.Y = shtListIndexY And lstLstSections(index).rect.X = shtListIndexX Then
                'Debug.Print(index.ToString & " IS QUAD IN")
                Return index
            End If
        Next

        Return -1 'Fail state
    End Function

    Public Function PushBack(ByVal cbx As CircleBox, ByVal pnt1In As Point, ByVal pnt2In As Point, ByVal pntLast As Point) As Point  'Uses stupid math I though of to push the circle back and it works.  I have a diagram somewhere...
        Dim pnt1 As Point
        Dim pnt2 As Point
        If pnt1In.X > pnt2In.X Then
            pnt1 = pnt1In
            pnt2 = pnt2In
        Else
            pnt1 = pnt2In
            pnt2 = pnt1In
        End If

        Dim xMove, yMove As Single  'I think in this situation this looks a bit better than a point.
        '1.5708 = 90 deg in radians

        Dim sngAAngle As Single = Math.Abs(FindAngle(pnt1, pnt2, cbx.pnt, pnt2))
        If sngAAngle > 1.5708 * 1.5 Then
            sngAAngle -= 1.5708 * 2  'IDK why but this sort of fixes the weird bug.
        End If

        Dim sngBAngle As Single = 1.5708 - sngAAngle
        'Debug.Print(sngBAngle & "=b, " & pnt1.ToString & "=pnt1, " & pnt2.ToString & "=pnt2, " & cbx.pnt.ToString & "=cbx.pnt.")
        Dim sngNSide As Single = Math.Cos(sngBAngle) * FindDistance(cbx.pnt, pnt2)

        Dim snglLength As Single = Math.Abs(cbx.sngRadius - sngNSide)

        Dim sngAngleCDDL As Single = FindAngle(pnt1, pnt2, FindIntersectPoint(pnt1, pnt2, New Point(0, Short.MaxValue), New Point(0, Short.MinValue)), New Point(0, Short.MaxValue))

        Dim shtSideValue As Long = (pntLast.X - pnt1.X) * (pnt2.Y - pnt1.Y) - (pntLast.Y - pnt1.Y) * (pnt2.X - pnt1.X)  'Negitave and positive says what side the point is on, thanks internet code.\

        If pnt1.Y < pnt2.Y Then
            If shtSideValue < 0 Then
                sngAngleCDDL = (1.5708 * 2) + sngAngleCDDL
            End If
        Else
            If shtSideValue < 0 Then
                sngAngleCDDL = (1.5708 * 2) + sngAngleCDDL
            End If
        End If

        Dim sngRise As Single = Math.Sin(sngAngleCDDL) * cbx.sngRadius

        Dim sngRun As Single = Math.Cos(sngAngleCDDL) * cbx.sngRadius

        Dim sngScale As Single = snglLength / cbx.sngRadius

        yMove = (sngRise * sngScale)
        xMove = (sngRun * sngScale)

        Return New Point(xMove, yMove)
    End Function

    Public Function FindMidPoint(ByVal pnt1 As Point, ByVal pnt2 As Point) As Point  'Finds the midpoint between two points.
        Dim xDiff As Short = (pnt1.X - pnt2.X) / 2
        Dim yDiff As Short = (pnt1.Y - pnt2.Y) / 2

        Return New Point(pnt1.X - xDiff, pnt1.Y - yDiff)
    End Function

    'Start Converted Internet Code
    Function sqr(ByVal x) As Integer
        Return x * x
    End Function

    Function DistanceSquared(ByVal v As Point, ByVal w As Point) As Integer
        Return sqr(v.X - w.X) + sqr(v.Y - w.Y)
    End Function

    Function DistanceToSegmentSquared(ByVal p As Point, ByVal v As Point, ByVal w As Point) As Long
        Dim line2 = DistanceSquared(v, w)
        If line2 = 0 Then
            Return DistanceSquared(p, v)
        End If

        Dim t = ((p.X - v.X) * (w.X - v.X) + (p.Y - v.Y) * (w.Y - v.Y)) / line2
        t = Math.Max(0, Math.Min(1, t))
        Return DistanceSquared(p, New Point(v.X + t * (w.X - v.X), v.Y + t * (w.Y - v.Y)))
    End Function

    Function DistanceToSegment(ByVal p As Point, ByVal v As Point, ByVal w As Point)
        Return Math.Sqrt(DistanceToSegmentSquared(p, v, w))
    End Function

    Public Function FindIntersectPoint(ByVal A As Point, ByVal B As Point, ByVal C As Point, ByVal D As Point) As Point
        Dim dy1 As Double = B.Y - A.Y

        Dim dx1 As Double = B.X - A.X
        Dim dy2 As Double = D.Y - C.Y
        Dim dx2 As Double = D.X - C.X
        Dim p As New Point
        'check whether the two line parallel
        If dy1 * dx2 = dy2 * dx1 Then
            'MessageBox.Show("no point")
            'Return P with a specific data
        Else
            Dim x As Double = ((C.Y - A.Y) * dx1 * dx2 + dy1 * dx2 * A.X - dy2 * dx1 * C.X) / (dy1 * dx2 - dy2 * dx1)
            Dim y As Double = A.Y + (dy1 / dx1) * (x - A.X)
            p = New Point(x, y)
            Return p
        End If
    End Function

    Function FindAngle(ByVal l11 As Point, ByVal l12 As Point, ByVal l21 As Point, ByVal l22 As Point) As Double
        Return (Math.Atan2(l12.Y - l11.Y, l12.X - l11.X) - Math.Atan2(l22.Y - l21.Y, l22.X - l21.X))
    End Function

    Function FindDistance(ByVal pnt1 As Point, ByVal pnt2 As Point) As Single
        Return Math.Sqrt(sqr(pnt2.X - pnt1.X) + sqr(pnt2.Y - pnt1.Y))
    End Function
    'End Converted Internet Code

    Dim shtDrawMapScale As Short = 1
    Public pntMapPos As Point
    Public Sub Draw(ByVal e As PaintEventArgs)  'Call this to draw the map to the screen
        'magic numbers are some weird offset.
        e.Graphics.DrawImage(imgDrawMap, New Rectangle((Form1.pbxPlayArea.Location().X * 2.5) + pntMapPos.X, (Form1.pbxPlayArea.Location().Y * 2.5) + pntMapPos.Y, imgDrawMap.Width * shtDrawMapScale, imgDrawMap.Height * shtDrawMapScale))
    End Sub
End Class
