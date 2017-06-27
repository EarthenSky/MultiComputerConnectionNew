Public Structure PixelPoint  'Holds a point and a value, values is used to connect to other pixel points and made the walls of the collider map.
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

Public Class PosList  'Is a list but also has the bounds of the area it encloses.  (Holds the lines.)
    Sub New(ByVal lstPnt As List(Of PixelPoint), ByVal pnt As Point)
        Me.lstPnt = lstPnt
        Me.pnt = pnt
    End Sub

    Public lstPnt As New List(Of PixelPoint)  'Holds the red pixel points
    Public pnt As Point  'Top left of box.

    Public lstLines As New List(Of Line)  'Holds collider lines
End Class

'When making a map put the highest Red value at the start of a specific box, like a line through the box. 
'Last point connects back To first point.
Public Class CollisionMap
    'Instantiate this class to make a collider map of lines, call the CheckCollision() method to check it's collision with a circle.

    Private lstLstSections As New List(Of PosList)
    Private arySectionsX As New List(Of Short)
    Private arySectionsY As New List(Of Short)
    Private imgDrawMap As Image

    Sub New(ByVal drawMap As Image, ByVal colMap As Image)
        LoadMap(colMap)
        imgDrawMap = drawMap
    End Sub

    Private Sub LoadMap(ByVal colMap As Image)
        Dim bmpCollision As New Bitmap(colMap)

        'Im not really using the green blue things that creates sections but the code relies on parts of it and it still sort of works so i'm going to leave it.
        'My map only has a single line going through it.
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
                lstLstSections.Add(New PosList(New List(Of PixelPoint), New Point(arySectionsX(jIndex) * shtColliderMapScale, arySectionsY(index) * shtColliderMapScale)))
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

    Private Sub AddRedPixelToList(ByVal pxPnt As PixelPoint)
        Dim shtListIndexX As Short = 0
        Dim shtListIndexY As Short = 0

        'Find x 
        For index As Short = 0 To lstLstSections.Count - 1  'This is slightly inefficient, only loop the x ones if possible.
            If pxPnt.pnt.X > lstLstSections(index).pnt.X Then
                shtListIndexX = lstLstSections(index).pnt.X
            End If
        Next

        'Find y
        For index As Short = 0 To lstLstSections.Count - 1  'This is slightly inefficient
            If pxPnt.pnt.Y > lstLstSections(index).pnt.Y Then
                shtListIndexY = lstLstSections(index).pnt.Y
            End If
        Next

        'Find list
        For index As Short = 0 To lstLstSections.Count - 1
            If lstLstSections(index).pnt.Y = shtListIndexY And lstLstSections(index).pnt.X = shtListIndexX Then
                lstLstSections(index).lstPnt.Add(pxPnt)
                Exit Sub  'After the last line it is done
            End If
        Next
    End Sub

    Private Const shtColliderMapScale As Short = 8
    Private Const shtExtraLineLength As Single = 1.5
    Private Sub MakeLines()
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

    Public Function CheckCollision(ByVal cbx As CircleBox, ByVal pntLast As Point) As Point  'Call this to check collision between a circle and the map.  'Sends back distance to move.
        Dim shtXPush As Short = 0
        Dim shtYPush As Short = 0

        CollisionLoop(cbx, shtXPush, shtYPush, pntLast)

        Return New Point(shtXPush, shtYPush)
    End Function

    Private Sub CollisionLoop(ByVal cbx As CircleBox, ByRef shtXPush As Short, ByRef shtYPush As Short, ByVal pntLast As Point)
        Dim shtDiv As Short = 0

        For index As Short = 0 To lstLstSections(FindSection(cbx)).lstLines.Count - 1  'Loops through all of the lines in the specified list.
            If Form1.DistanceToSegment(cbx.pnt, lstLstSections(FindSection(cbx)).lstLines(index).pnt1, lstLstSections(FindSection(cbx)).lstLines(index).pnt2) < cbx.sngRadius Then
                Dim temp As Point = PushBack(cbx, lstLstSections(FindSection(cbx)).lstLines(index).pnt1, lstLstSections(FindSection(cbx)).lstLines(index).pnt2, pntLast)
                shtXPush += temp.X
                shtYPush += temp.Y
                shtDiv += 1
            End If
        Next

        If shtDiv > 0 Then
            shtXPush /= shtDiv
            shtYPush /= shtDiv
        End If
    End Sub

    Private Function FindSection(ByVal cbx As CircleBox) As Short 'Returns "Index" of list.
        Dim shtListIndexX = 0
        Dim shtListIndexY = 0

        'Find x 
        For index As Short = 0 To lstLstSections.Count - 1  'This is slightly inefficient, only loop the x ones if possible.  Actually this entire proccess is, there is probably a better way.
            If cbx.pnt.X > lstLstSections(index).pnt.X Then
                shtListIndexX = lstLstSections(index).pnt.X
            End If
        Next

        'Find y
        For index As Short = 0 To lstLstSections.Count - 1
            If cbx.pnt.Y > lstLstSections(index).pnt.Y Then
                shtListIndexY = lstLstSections(index).pnt.Y
            End If
        Next

        'Find list
        For index As Short = 0 To lstLstSections.Count - 1
            If lstLstSections(index).pnt.Y = shtListIndexY And lstLstSections(index).pnt.X = shtListIndexX Then
                Return index
            End If
        Next

        Return -1 'Fail state
    End Function

    Private Function PushBack(ByVal cbx As CircleBox, ByVal pnt1In As Point, ByVal pnt2In As Point, ByVal pntLast As Point) As Point  'Uses inefficient math I though of to push the circle back and it mostly works.  I have a diagram somewhere...
        Dim pnt1 As Point
        Dim pnt2 As Point
        If pnt1In.X > pnt2In.X Then
            pnt1 = pnt1In
            pnt2 = pnt2In
        Else
            pnt1 = pnt2In
            pnt2 = pnt1In
        End If

        Dim sngXMove, sngYMove As Single
        '1.5708 = 90 deg in radians

        Dim sngAAngle As Single = Math.Abs(Form1.FindAngle(pnt1, pnt2, cbx.pnt, pnt2))
        If sngAAngle > 1.5708 * 1.5 Then
            sngAAngle -= 1.5708 * 2  'IDK why but this sort of fixes the weird bug.
        End If

        Dim sngBAngle As Single = 1.5708 - sngAAngle

        Dim sngNSide As Single = Math.Cos(sngBAngle) * Form1.FindDistance(cbx.pnt, pnt2)

        Dim snglLength As Single = Math.Abs(cbx.sngRadius - sngNSide)

        Dim sngAngleCDDL As Single = Form1.FindAngle(pnt1, pnt2, Form1.FindIntersectPoint(pnt1, pnt2, New Point(0, Short.MaxValue), New Point(0, Short.MinValue)), New Point(0, Short.MaxValue))

        Dim lngSideValue As Long = (pntLast.X - pnt1.X) * (pnt2.Y - pnt1.Y) - (pntLast.Y - pnt1.Y) * (pnt2.X - pnt1.X)

        If pnt1.Y < pnt2.Y Then
            If lngSideValue < 0 Then
                sngAngleCDDL = (1.5708 * 2) + sngAngleCDDL
            End If
        Else
            If lngSideValue < 0 Then
                sngAngleCDDL = (1.5708 * 2) + sngAngleCDDL
            End If
        End If

        Dim sngRise As Single = Math.Sin(sngAngleCDDL) * cbx.sngRadius

        Dim sngRun As Single = Math.Cos(sngAngleCDDL) * cbx.sngRadius

        Dim sngScale As Single = snglLength / cbx.sngRadius

        sngYMove = (sngRise * sngScale)
        sngXMove = (sngRun * sngScale)

        Return New Point(sngXMove, sngYMove)
    End Function

    Private Function FindMidPoint(ByVal pnt1 As Point, ByVal pnt2 As Point) As Point  'Finds the midpoint between two points.
        Dim shtXDiff As Short = (pnt1.X - pnt2.X) / 2
        Dim shtYDiff As Short = (pnt1.Y - pnt2.Y) / 2

        Return New Point(pnt1.X - shtXDiff, pnt1.Y - shtYDiff)
    End Function

    Private Const shtDrawMapScale As Short = 1
    Private pntMapPos As Point
    Public Sub Draw(ByVal e As PaintEventArgs)  'Call this to draw the map to the screen
        'The magic numbers are some weird offset that makes the walls be in the right place.
        e.Graphics.DrawImage(imgDrawMap, New Rectangle((Form1.pbxPlayArea.Location().X * 2.5) + pntMapPos.X, (Form1.pbxPlayArea.Location().Y * 2.5) + pntMapPos.Y,
                                                        imgDrawMap.Width * shtDrawMapScale, imgDrawMap.Height * shtDrawMapScale))
    End Sub
End Class
