Imports System.IO

Public Class Form1
    'Gabe Stang
    '
    'LAN has been removed.  Top-down game.
    'You move by pressing W or S to move to and from the mouse, left click to attack.  You have 3 hp (top left)  Try to kill all of the AI bugs.
    'To Note: I focused on getting everything working instead of making the code function super nice, sorry ^-^. 

    Private strCurrentFileDirectory As String = Directory.GetCurrentDirectory.Remove(Directory.GetCurrentDirectory.IndexOf("\bin\Debug"), 10) + "\"
    Private imgGoodSpriteSheet As Image
    Private imgEnemySpriteSheet As Image
    Private imgCircle As Image
    Private imgHpGood As Image
    Private imgHpBad As Image

    Private imgCollisionMap As Image
    Private imgDrawMap As Image

    Private lstComputers As New List(Of String)
    Public meObj As MainCharacter
    Private lstAI As New List(Of AI)
    Private lstMapCircles As New List(Of OverDropObject)  'Holds the circles that make the map look cool.  Yeah, thats what they're for...

    Private mapMain As CollisionMap

    'Start Init Code
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Attach textures to variables.
        imgGoodSpriteSheet = Image.FromFile(strCurrentFileDirectory & "MainChar2_SpriteSheet.png")
        imgEnemySpriteSheet = Image.FromFile(strCurrentFileDirectory & "BugOne_MiniEnemy_SpriteSheet.png")
        imgDrawMap = Image.FromFile(strCurrentFileDirectory & "ActualMapDraw2.png")
        imgCollisionMap = Image.FromFile(strCurrentFileDirectory & "ActualMapMath.png")
        imgCircle = Image.FromFile(strCurrentFileDirectory & "Circle.png")
        imgHpGood = Image.FromFile(strCurrentFileDirectory & "HealthGood.png")
        imgHpBad = Image.FromFile(strCurrentFileDirectory & "HealthBad.png")

        'Create the character and its attack circle.
        meObj = New MainCharacter(New Point(700, 700), imgGoodSpriteSheet, 12, 100)
        meObj.lstPointPosition.Add(New CircleBox(New Point(32, 32), 16))

        mapMain = New CollisionMap(imgDrawMap, imgCollisionMap)  'Create the map.
        AddCircles()  'Add the circles to the map.

        'Create the AI's.
        lstAI.Add(New AI(New Point(780, 350), imgEnemySpriteSheet, 22, 100))
        lstAI.Add(New AI(New Point(353 - 32, 189 - 32), imgEnemySpriteSheet, 22, 100))
        lstAI.Add(New AI(New Point(425 - 32, 255 - 32), imgEnemySpriteSheet, 22, 100))
        lstAI.Add(New AI(New Point(185 - 32, 517 - 32), imgEnemySpriteSheet, 22, 100))
        lstAI.Add(New AI(New Point(191 - 32, 631 - 32), imgEnemySpriteSheet, 22, 100))
        lstAI.Add(New AI(New Point(135 - 32, 610 - 32), imgEnemySpriteSheet, 22, 100))

        PlayLoopingBackgroundSoundFile() 'Start awesome music

        Me.lblInfo.Text = "Controls: " & vbNewLine & "W - Moves player towards mouse." & vbNewLine & "S - Moves player away from mouse." & vbNewLine & "LeftClick - Player attacks."  'Set info label.

        Me.KeyPreview = True  'This makes getting key input possible.  This needs to exist, for some reason...
    End Sub

    Public Const shtCircleColliderSize = 64
    Private Sub AddCircles()  'Circles look cool.
        lstMapCircles.Add(New OverDropObject(New Point(220, 400), imgCircle, shtCircleColliderSize))
        lstMapCircles.Add(New OverDropObject(New Point(615, 285), imgCircle, shtCircleColliderSize))
        lstMapCircles.Add(New OverDropObject(New Point(880, 420), imgCircle, shtCircleColliderSize))
        lstMapCircles.Add(New OverDropObject(New Point(44, 490), imgCircle, shtCircleColliderSize))
        lstMapCircles.Add(New OverDropObject(New Point(780, 660), imgCircle, shtCircleColliderSize))
        lstMapCircles.Add(New OverDropObject(New Point(650, 120), imgCircle, shtCircleColliderSize))
        lstMapCircles.Add(New OverDropObject(New Point(720, 500), imgCircle, shtCircleColliderSize))
    End Sub

    Private Sub PlayLoopingBackgroundSoundFile()
        My.Computer.Audio.Play(strCurrentFileDirectory & "126(3 - Chaosotacitc Skies).wav", AudioPlayMode.BackgroundLoop)  'I make awesome music, yeah?
    End Sub
    'End Init Code

    'Start Update Code (Loop Code)
    Private sngRotationFactor As Single
    Private Sub PaintMain(ByVal o As Object, ByVal e As PaintEventArgs) Handles pbxPlayArea.Paint  'This method draws everything in the game and is triggered by "Refresh()" calls.
        Dim bmpTexture As New Bitmap(imgCircle)  'Used for rotation.
        Dim mtxRotate As New Drawing2D.Matrix(1, 0, 0, 1, 1, 0)  'Used for rotation.

        mapMain.Draw(e)  'Draws the map at the bottom.

        For Each obj As OverDropObject In lstMapCircles  'Draws circle things that are part of the map, at bottom too.
            e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetDrawPoint(0).X - obj.GetMainPoint(0).sngRadius / 2, obj.GetDrawPoint(0).Y - obj.GetMainPoint(0).sngRadius / 2, obj.GetMainPoint(0).sngRadius * 2, obj.GetMainPoint(0).sngRadius * 2))
        Next

        meObj.DrawHealth(e, imgHpGood, imgHpBad)  'Draws the health points.

        For Each obj As AI In lstAI 'Draws the AI and sets their rotations.
            obj.SetLookAngle()

            'Sets AI rotation.
            mtxRotate = New Drawing2D.Matrix(1, 0, 0, 1, 1, 0)
            mtxRotate.RotateAt(obj.shtLookAngle, New PointF(obj.GetMainPoint(0).pnt.X + 32, obj.GetMainPoint(0).pnt.Y + 32), Drawing2D.MatrixOrder.Append)
            e.Graphics.Transform = mtxRotate

            'Draws the current animation frame for the AI if the animation is playing.
            If obj.pntCurrentImgIndexes.X <> -1 Then
                e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetMainPoint(0).pnt.X, obj.GetMainPoint(0).pnt.Y, 64, 64),
                                     obj.lstAnimations(obj.pntCurrentImgIndexes.X)(obj.pntCurrentImgIndexes.Y), System.Drawing.GraphicsUnit.Pixel)
            Else
                e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetMainPoint(0).pnt.X, obj.GetMainPoint(0).pnt.Y, 64, 64),
                                     obj.lstAnimations(0)(0), GraphicsUnit.Pixel)
            End If
        Next

        'Sets Character rotation.
        mtxRotate = New Drawing2D.Matrix(1, 0, 0, 1, 1, 0)
        mtxRotate.RotateAt(sngRotationFactor, New PointF(meObj.GetMainPoint(0).pnt.X + 32, meObj.GetMainPoint(0).pnt.Y + 32), Drawing2D.MatrixOrder.Append)
        e.Graphics.Transform = mtxRotate

        'Draws the current animation frame for the character if the animation is playing.
        If meObj.pntCurrentImgIndexes.X <> -1 Then
            e.Graphics.DrawImage(meObj.imgMainImage, New Rectangle(meObj.GetMainPoint(0).pnt.X, meObj.GetMainPoint(0).pnt.Y, 64, 64),
                                 meObj.lstAnimations(meObj.pntCurrentImgIndexes.X)(meObj.pntCurrentImgIndexes.Y), System.Drawing.GraphicsUnit.Pixel)
        Else
            e.Graphics.DrawImage(meObj.imgMainImage, New Rectangle(meObj.GetMainPoint(0).pnt.X, meObj.GetMainPoint(0).pnt.Y, 64, 64),
                                 meObj.lstAnimations(0)(0), GraphicsUnit.Pixel)
        End If
    End Sub

    Private Const shtMeCharSpeed As Short = 8
    Private Sub tmrGameUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrGameUpdate.Tick
        LookAtMouse()

        If meObj.pntCurrentImgIndexes.X = 1 Then  'Ends the animation after attacking is done.  
            If meObj.pntCurrentImgIndexes.Y = 3 Then  'Yes, it is supposed to be 3 frames, the fourth fame looked bad.
                meObj.SetAnimationInterval(100)
                meObj.PlayAnimation(0)  'Animation 0 is idle
                blnSwordOut = False
            End If
        End If

        If blnSwordOut = True Then 'If sword is attacking set rotation and pos of sword.  (The sword collision circle is slighly protruding from the front of the character.)
            meObj.SetSwordPointRotation((sngRotationFactor - 90) / 57.2958)  'Division converts degrees to radians.
        End If

        For Each ai As AI In lstAI
            ai.AIMove()  'Moves the AI.
        Next

        MovementStuff()

        CollisionStuff()

        Refresh()  'Updates the screen.  (Calls the PaintMain() method.)
    End Sub

    Private Sub MovementStuff()  'Finds the rise and run between the mouse and player and roughly scales it to the move speed.
        If blnWKeyDown = True Then
            Dim rise As Double = pntMouse.Y - meObj.GetMainPoint(0).pnt.Y - 32
            Dim run As Double = pntMouse.X - meObj.GetMainPoint(0).pnt.X - 32

            Dim dis As Double = FindDistance(pntMouse, meObj.GetMainPoint(0).pnt)
            Dim scale As Double = shtMeCharSpeed / dis

            rise *= scale
            run *= scale

            Dim num = Math.Abs(rise) + Math.Abs(run)
            If num <> 0 Then
                Dim scale2 As Double = shtMeCharSpeed / num
                rise *= scale2
                run *= scale2
            End If

            If GeneralPointComparison(New Point(meObj.GetMainPoint(0).pnt.X + 32, meObj.GetMainPoint(0).pnt.Y + 32), pntMouse) = False Then  'Make sure the player stops before going directly over the mouse.
                meObj.SetMainPoint(New Point(meObj.GetMainPoint(0).pnt.X + run, meObj.GetMainPoint(0).pnt.Y + rise))
            End If

        ElseIf blnSKeyDown = True Then
            Dim rise As Double = pntMouse.Y - meObj.GetMainPoint(0).pnt.Y - 32
            Dim run As Double = pntMouse.X - meObj.GetMainPoint(0).pnt.X - 32

            Dim dis As Double = FindDistance(pntMouse, meObj.GetMainPoint(0).pnt)
            Dim scale As Double = shtMeCharSpeed / dis

            rise *= scale
            run *= scale

            Dim num = Math.Abs(rise) + Math.Abs(run)
            If num <> 0 Then
                Dim scale2 As Double = shtMeCharSpeed / num
                rise *= scale2
                run *= scale2
            End If

            meObj.SetMainPoint(New Point(meObj.GetMainPoint(0).pnt.X - run, meObj.GetMainPoint(0).pnt.Y - rise))

        End If
    End Sub

    Private Const shtDiffNum As Short = 8
    Private Function GeneralPointComparison(ByVal pnt1 As Point, ByVal pnt2 As Point) As Boolean  'Checks if two points are roughly similar.
        Return IIf(Math.Abs(pnt1.X - pnt2.X) < shtDiffNum AndAlso Math.Abs(pnt1.Y - pnt2.Y) < shtDiffNum, True, False)  'Ternary operator?  Yes! .\_/.
    End Function

    Public Sub CollisionStuff()
        'AI & More AI
        For index As Short = 0 To lstAI.Count - 1 Step 1
            For jIndex As Short = lstAI.Count - 1 To index + 1 Step -1
                CircleCollisionDynamic(lstAI(index), lstAI(jIndex))
            Next
        Next

        'My Com & Walls
        Dim pntMoveOffset As Point = mapMain.CheckCollision(meObj.GetMainPoint(0), meObj.pntLastPos)
        meObj.SetMainPoint(New Point(meObj.GetMainPoint(0).pnt.X + pntMoveOffset.X, meObj.GetMainPoint(0).pnt.Y + pntMoveOffset.Y))

        'My Com & Circle Walls
        For Each objCircle As OverDropObject In lstMapCircles
            CircleCollisionWall(objCircle, meObj)
        Next

        'My Com & AIs
        For Each objAI As AI In lstAI
            Dim pntPushAmount As Point = CircleCollisionDynamic(objAI, meObj)
            If pntPushAmount <> New Point(0, 0) Then
                If objAI.blnIsDead = False Then
                    meObj.HitAI(pntPushAmount)
                    meObj.InvulnerablityActivate()
                End If
                objAI.PushBack(pntPushAmount)
            End If
        Next

        'Walls & AIs
        For Each objAI As AI In lstAI
            Dim pntMoveOffset2 As Point = mapMain.CheckCollision(objAI.GetMainPoint(0), objAI.pntLastPos)
            objAI.SetMainPoint(New Point(objAI.GetMainPoint(0).pnt.X + pntMoveOffset2.X, objAI.GetMainPoint(0).pnt.Y + pntMoveOffset2.Y))
        Next

        'Circle Walls & AIs
        For Each objCircle As OverDropObject In lstMapCircles
            For Each objAI As AI In lstAI
                CircleCollisionWall(objCircle, objAI)
            Next
        Next

        'MySword & AIs
        If blnSwordOut = True Then
            For Each objAI As AI In lstAI
                If objAI.blnIsDead = False Then
                    If CircleCollisionDetect(objAI, meObj) = True Then
                        objAI.PushBack(CircleCollisionNumbers(objAI, meObj))
                        objAI.HitPlayerSword()
                    End If
                End If
            Next
        End If
    End Sub

    Private Function CircleCollisionDynamic(ByRef objDynamic1 As OverDropObject, ByRef objDynamic2 As OverDropObject) As Point 'Checks collision and pushes both objects back, used for pushing.
        'Finds the rise and run of the two radius of the circles and scales it down to the overlap.
        If FindDistance(objDynamic1.GetDrawPoint(0), objDynamic2.GetDrawPoint(0)) < objDynamic2.GetMainPoint(0).sngRadius + objDynamic1.GetMainPoint(0).sngRadius Then
            Dim sngXMove, sngYMove As Single

            Dim sngRun As Single = objDynamic2.GetMainPoint(0).pnt.X - objDynamic1.GetMainPoint(0).pnt.X
            Dim sngRise As Single = objDynamic2.GetMainPoint(0).pnt.Y - objDynamic1.GetMainPoint(0).pnt.Y

            Dim sngSmallDistance As Single = objDynamic2.GetMainPoint(0).sngRadius + objDynamic1.GetMainPoint(0).sngRadius - FindDistance(objDynamic1.GetDrawPoint(0), objDynamic2.GetDrawPoint(0))
            Dim sngScaleFactor As Single = sngSmallDistance / (objDynamic2.GetMainPoint(0).sngRadius + objDynamic1.GetMainPoint(0).sngRadius)

            'Scales the rise run to the amount to push back amount and cuts it in half.
            sngXMove = sngRun * sngScaleFactor / 2
            sngYMove = sngRise * sngScaleFactor / 2

            objDynamic1.SetMainPoint(New Point(objDynamic1.GetDrawPoint(0).X - sngXMove, objDynamic1.GetDrawPoint(0).Y - sngYMove))
            objDynamic2.SetMainPoint(New Point(objDynamic2.GetDrawPoint(0).X + sngXMove, objDynamic2.GetDrawPoint(0).Y + sngYMove))
            Return New Point(sngXMove, sngYMove)
        End If
        Return New Point(0, 0)
    End Function

    Private Sub CircleCollisionWall(ByRef objWall As OverDropObject, ByRef objDynamic As OverDropObject) 'Checks collision and pushes "objDynamic" back, used for wall collision
        'Finds the rise and run of the two radius of the circles and scales it down to the overlap.
        If FindDistance(objWall.GetDrawPoint(0), objDynamic.GetDrawPoint(0)) < objDynamic.GetMainPoint(0).sngRadius + objWall.GetMainPoint(0).sngRadius Then
            Dim sngXMove, sngYMove As Single

            Dim sngRun As Single = objDynamic.GetMainPoint(0).pnt.X - objWall.GetMainPoint(0).pnt.X
            Dim sngRise As Single = objDynamic.GetMainPoint(0).pnt.Y - objWall.GetMainPoint(0).pnt.Y

            Dim sngSmallDistance As Single = objDynamic.GetMainPoint(0).sngRadius + objWall.GetMainPoint(0).sngRadius - FindDistance(objWall.GetDrawPoint(0), objDynamic.GetDrawPoint(0))
            Dim sngScaleFactor As Single = sngSmallDistance / (objDynamic.GetMainPoint(0).sngRadius + objWall.GetMainPoint(0).sngRadius)

            'Scales the rise run to the amount to push back.
            sngXMove = sngRun * sngScaleFactor
            sngYMove = sngRise * sngScaleFactor

            objDynamic.SetMainPoint(New Point(objDynamic.GetDrawPoint(0).X + sngXMove, objDynamic.GetDrawPoint(0).Y + sngYMove))
        End If
    End Sub

    Private Function CircleCollisionDetect(ByRef objDynamic1 As OverDropObject, ByRef objDynamicSword As OverDropObject) As Boolean 'only checks collision detection and gives a true / false answer.
        Return IIf(FindDistance(objDynamic1.GetDrawPoint(0), objDynamicSword.GetMainPoint(1).pnt) < objDynamicSword.GetMainPoint(1).sngRadius + objDynamic1.GetMainPoint(0).sngRadius, True, False)
    End Function

    Private Function CircleCollisionNumbers(ByRef objDynamic1 As OverDropObject, ByRef objDynamic2 As OverDropObject) As Point 'This doesn't push back but returns the point.  This is one of the time constraint solutions.
        'Finds the rise and run of the two radius of the circles and scales it down to the overlap.
        Dim sngXMove, sngYMove As Single

        Dim sngRun As Single = objDynamic2.GetMainPoint(0).pnt.X - objDynamic1.GetMainPoint(0).pnt.X
        Dim sngRise As Single = objDynamic2.GetMainPoint(0).pnt.Y - objDynamic1.GetMainPoint(0).pnt.Y

        Dim sngSmallDis As Single = objDynamic2.GetMainPoint(0).sngRadius + objDynamic1.GetMainPoint(0).sngRadius - FindDistance(objDynamic1.GetDrawPoint(0), objDynamic2.GetDrawPoint(0))
        Dim sngScaleFactor As Single = sngSmallDis / (objDynamic2.GetMainPoint(0).sngRadius + objDynamic1.GetMainPoint(0).sngRadius)

        'Scale the rise run to the amount to push back and cuts it to make it smaller.
        sngXMove = sngRun * sngScaleFactor / 8
        sngYMove = sngRise * sngScaleFactor / 8

        Return New Point(-sngXMove, -sngYMove) 'Negitave cause idk.
    End Function

    Private Sub LookAtMouse()  'Finds the angle between the mouse and the y axis.
        Dim sngAngle As Single = FindAngle(pntMouse, New Point(meObj.GetMainPoint(0).pnt.X + 32, meObj.GetMainPoint(0).pnt.Y + 32),
                              New Point(0, Short.MinValue), New Point(0, Short.MaxValue)) * 57.2958
        sngRotationFactor = sngAngle
    End Sub
    'End Update Code (Loop Code)

    'Start Device Interaction Code
    Private pntMouse As Point
    Private Sub Form1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove, pbxPlayArea.MouseMove 'Mouse has to be on the form for the character to look at it. 
        pntMouse = New Point(e.X, e.Y)
        'Debug.Print("Mouse Pos is : " & pntMouse.ToString())
    End Sub

    Private blnSwordOut As Boolean
    Private Sub MouseClickDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles pbxPlayArea.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            meObj.SetAnimationInterval(25)  'The attack animation has to move faster.
            meObj.PlayAnimation(1)  '1 is the atack animation
            blnSwordOut = True
        End If
    End Sub

    Private blnWKeyDown As Boolean = False
    Private blnSKeyDown As Boolean = False
    Private Sub ButtonPress(ByVal o As System.Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        'Movement Keys
        If e.KeyCode = Keys.W And blnWKeyDown = False Then
            blnWKeyDown = True
        End If
        If e.KeyCode = Keys.S And blnSKeyDown = False Then
            blnSKeyDown = True
        End If

        'Debug Keys
        If e.KeyCode = Keys.E Then
            lstAI(0).PlayAnimation(0)
        ElseIf e.KeyCode = Keys.Up Then
            lstAI(0).PlayAnimation(1)
        ElseIf e.KeyCode = Keys.T Then
            lstAI(0).StopAnimation()
        ElseIf e.KeyCode = Keys.O Then
            meObj.ChangeHealth(1)
        ElseIf e.KeyCode = Keys.P Then
            meObj.ChangeHealth(-1)
        ElseIf e.KeyCode = Keys.R Then
            For Each obj As AI In lstAI 'Debug, for Colliding Ai with walls
                obj.SetMainPoint(New Point(obj.GetMainPoint(0).pnt.X - 1, obj.GetMainPoint(0).pnt.Y))
            Next
        End If
    End Sub

    Private Sub ButtonUnPress(ByVal o As System.Object, ByVal e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.W Then
            blnWKeyDown = False
        End If
        If e.KeyCode = Keys.S Then
            blnSKeyDown = False
        End If
    End Sub
    'End Device Interaction Code

    'Start Converted Internet Code
    Public Function Squared(ByVal num) As Double
        Return num * num
    End Function

    Public Function DistanceSquared(ByVal pnt1 As Point, ByVal pnt2 As Point) As Integer
        Return Squared(pnt1.X - pnt2.X) + Squared(pnt1.Y - pnt2.Y)
    End Function

    Public Function DistanceToSegmentSquared(ByVal pnt1 As Point, ByVal pnt2 As Point, ByVal pnt3 As Point) As Integer
        Dim l2 = DistanceSquared(pnt2, pnt3)
        If l2 = 0 Then
            Return DistanceSquared(pnt1, pnt2)
        End If

        Dim t = ((pnt1.X - pnt2.X) * (pnt3.X - pnt2.X) + (pnt1.Y - pnt2.Y) * (pnt3.Y - pnt2.Y)) / l2
        t = Math.Max(0, Math.Min(1, t))
        Return DistanceSquared(pnt1, New Point(pnt2.X + t * (pnt3.X - pnt2.X), pnt2.Y + t * (pnt3.Y - pnt2.Y)))
    End Function

    Public Function DistanceToSegment(ByVal pnt1 As Point, ByVal pnt2 As Point, ByVal pnt3 As Point)
        Return Math.Sqrt(DistanceToSegmentSquared(pnt1, pnt2, pnt3))
    End Function

    Public Function FindIntersectPoint(ByVal pntL1P1 As Point, ByVal pntL1P2 As Point, ByVal pntL2P1 As Point, ByVal pntL2P2 As Point) As Point
        Dim dy1 As Double = pntL1P2.Y - pntL1P1.Y

        Dim dx1 As Double = pntL1P2.X - pntL1P1.X
        Dim dy2 As Double = pntL2P2.Y - pntL2P1.Y
        Dim dx2 As Double = pntL2P2.X - pntL2P1.X
        Dim pnt As New Point
        'check whether the two line parallel
        If dy1 * dx2 = dy2 * dx1 Then
            'Return P with a specific data
        Else
            Dim x As Double = ((pntL2P1.Y - pntL1P1.Y) * dx1 * dx2 + dy1 * dx2 * pntL1P1.X - dy2 * dx1 * pntL2P1.X) / (dy1 * dx2 - dy2 * dx1)
            Dim y As Double = pntL1P1.Y + (dy1 / dx1) * (x - pntL1P1.X)
            pnt = New Point(x, y)
            Return pnt
        End If
    End Function

    Public Function FindAngle(ByVal pntL1P1 As Point, ByVal pntL1P2 As Point, ByVal pntL2P1 As Point, ByVal pntL2P2 As Point) As Single
        Return (Math.Atan2(pntL1P2.Y - pntL1P1.Y, pntL1P2.X - pntL1P1.X) - Math.Atan2(pntL2P2.Y - pntL2P1.Y, pntL2P2.X - pntL2P1.X))
    End Function

    Public Function FindDistance(ByVal pnt1 As Point, ByVal pnt2 As Point) As Single
        Return Math.Sqrt(Squared(pnt2.X - pnt1.X) + Squared(pnt2.Y - pnt1.Y))
    End Function
    'End Converted Internet Code

End Class
