Imports System.Net.Sockets
Imports System.Net
Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Drawing.Drawing2D

Public Enum Process
    None
    StringStart
    AddComEnd
    GiveComs
    MessageEnd
    PressedKey
    UnPressedKey
    SendFrom
    PointGet
    NumGet
End Enum

Public Class Form1
    'Gabe Stang
    '
    'Connect 3 computers together
    'Can connect multiple computers (3) together and runs a LAN game.  You move by pressing w and s to move to and from the mouse, left click to attack.
    'To Note: I focused on getting everything working instead of making the code look super nice, sorry ^-^. 
    'MAIN STUFF TODO:
    '- MAKE AI
    ':D - ADD A FEW MORE ENEMIES
    ':D - FIX LAG PROBLEM (Nevermind, false alarm)
    '- 0.1S INVULNERABILITY AFTER GETTING HIT
    '- MAKE SURE LAN WORKS
    ':D - MORE SMALL STUFF

    Public Const chrStartProcessingText As Char = Chr(0)
    Public Const chrAddComToConnectListEnd As Char = Chr(1)
    Public Const chrGiveComs As Char = Chr(2)
    Public Const chrMessageEnd As Char = Chr(3)
    Public Const chrKeyPress As Char = Chr(4) 'send key pressed
    Public Const chrKeyUnPress As Char = Chr(7) 'send key released
    Public Const chrSendFrom As Char = Chr(5) 'person from
    Public Const chrMakeNum As Char = Chr(6) 'makes a num value

    Private listener As New TcpListener(5019)
    Private client As New TcpClient

    Private strCurrentFileDirectory As String = Directory.GetCurrentDirectory.Remove(Directory.GetCurrentDirectory.IndexOf("\bin\Debug"), 10) + "\"
    Private imgGoodSpriteSheet As Image
    Private imgEnemySpriteSheet As Image
    Private imgCircle As Image
    Private imgHpGood As Image
    Private imgHpBad As Image

    Private imgCollisionMap As Image
    Private imgDrawMap As Image

    Private lstComputers As New List(Of String)
    Private meObj As MainCharacter
    Private lstOtherComputerObjects As New List(Of Player)
    Private lstAI As New List(Of AI)
    Private lstMapCircles As New List(Of OverDropObject)  'Holds the circles that make the map look cool.  Yeah, thats what they're for...

    Private mapMain As CollisionMap

    Private stwDebug As New Stopwatch()

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Attach textures to project
        imgGoodSpriteSheet = Image.FromFile(strCurrentFileDirectory & "MainChar2_SpriteSheet.png")
        imgEnemySpriteSheet = Image.FromFile(strCurrentFileDirectory & "BugOne_MiniEnemy_SpriteSheet.png")
        imgDrawMap = Image.FromFile(strCurrentFileDirectory & "ActualMapDraw2.png")
        imgCollisionMap = Image.FromFile(strCurrentFileDirectory & "ActualMapMath.png")
        imgCircle = Image.FromFile(strCurrentFileDirectory & "Circle.png")
        imgHpGood = Image.FromFile(strCurrentFileDirectory & "HealthGood.png")
        imgHpBad = Image.FromFile(strCurrentFileDirectory & "HealthBad.png")

        'create the character and its attack circle.
        meObj = New MainCharacter(New Point(700, 700), imgGoodSpriteSheet, 12, 100)
        meObj.lstPointPosition.Add(New CircleBox(New Point(32, 32), 16))

        'create the map
        mapMain = New CollisionMap(imgDrawMap, imgCollisionMap)

        'create the AI's
        lstAI.Add(New AI(New Point(780, 350), imgEnemySpriteSheet, 22, 100))
        lstAI.Add(New AI(New Point(353 - 32, 189 - 32), imgEnemySpriteSheet, 22, 100))
        lstAI.Add(New AI(New Point(425 - 32, 255 - 32), imgEnemySpriteSheet, 22, 100))
        lstAI.Add(New AI(New Point(185 - 32, 517 - 32), imgEnemySpriteSheet, 22, 100))
        lstAI.Add(New AI(New Point(191 - 32, 631 - 32), imgEnemySpriteSheet, 22, 100))

        AddCircles()

        PlayLoopingBackgroundSoundFile() 'Start awesome music

        Me.KeyPreview = True

        Dim ListenerThread As New Thread(New ThreadStart(AddressOf Listening))
        tbxConnectionComputerName.Text = My.Computer.Name
        ListenerThread.Start()

        stwDebug.Start()

    End Sub

    Public Const shtCircleColliderSize = 64
    Private Sub AddCircles()  'There isn't much time left so I did this to fix collsion.
        lstMapCircles.Add(New OverDropObject(New Point(220, 400), imgCircle, shtCircleColliderSize))
        lstMapCircles.Add(New OverDropObject(New Point(620, 300), imgCircle, shtCircleColliderSize))
        lstMapCircles.Add(New OverDropObject(New Point(880, 420), imgCircle, shtCircleColliderSize))
        lstMapCircles.Add(New OverDropObject(New Point(44, 490), imgCircle, shtCircleColliderSize))
        lstMapCircles.Add(New OverDropObject(New Point(780, 660), imgCircle, shtCircleColliderSize))
        lstMapCircles.Add(New OverDropObject(New Point(650, 125), imgCircle, shtCircleColliderSize))
    End Sub

    Sub PlayLoopingBackgroundSoundFile()
        'My.Computer.Audio.Play(strCurrentFileDirectory & "126(3 - Chaosotacitc Skies).wav", AudioPlayMode.BackgroundLoop)  'I make awesome music, yeah?
    End Sub

    Public sngRotationFactor As Single
    Private Sub PaintMain(ByVal o As Object, ByVal e As PaintEventArgs) Handles pbxPlayArea.Paint
        mapMain.Draw(e)  'Draw at the bottom.
        For Each obj As OverDropObject In lstMapCircles 'Draws circle things  'Draw at bottom too.
            e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetDrawPoint(0).X - obj.GetMainPoint(0).sngRadius / 2, obj.GetDrawPoint(0).Y - obj.GetMainPoint(0).sngRadius / 2, obj.GetMainPoint(0).sngRadius * 2, obj.GetMainPoint(0).sngRadius * 2))
        Next

        For Each obj As Player In lstOtherComputerObjects 'Draws otherComputer controlled objects
            e.Graphics.DrawImage(imgEnemySpriteSheet, New Rectangle(obj.GetDrawPoint(0).X, obj.GetDrawPoint(0).Y, meObj.imgMainImage.Width / 4, meObj.imgMainImage.Height / 4))
            'e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetDrawPoint().X, obj.GetDrawPoint().Y, obj.imgMainImage.Width, obj.imgMainImage.Height / 2))
        Next

        For Each obj As AnimationObject In lstAI 'Draws AI
            If obj.pntCurrentImgIndexes.X <> -1 Then
                e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetMainPoint(0).pnt.X, obj.GetMainPoint(0).pnt.Y, 64, 64),
                                     obj.lstAnimations(obj.pntCurrentImgIndexes.X)(obj.pntCurrentImgIndexes.Y), System.Drawing.GraphicsUnit.Pixel)
            Else
                e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetMainPoint(0).pnt.X, obj.GetMainPoint(0).pnt.Y, 64, 64),
                                     obj.lstAnimations(0)(0), GraphicsUnit.Pixel)
            End If
        Next

        meObj.DrawHealth(e, imgHpGood, imgHpBad)

        'Rotates the Character.
        Dim bmpTexture As New Bitmap(imgCircle)
        Dim mtxRotate As New Drawing2D.Matrix(1, 0, 0, 1, 1, 0)
        mtxRotate.RotateAt(sngRotationFactor, New PointF(meObj.GetMainPoint(0).pnt.X + 32, meObj.GetMainPoint(0).pnt.Y + 32), Drawing2D.MatrixOrder.Append)
        e.Graphics.Transform = mtxRotate

        'Draws Character Animation.
        If meObj.pntCurrentImgIndexes.X <> -1 Then
            e.Graphics.DrawImage(meObj.imgMainImage, New Rectangle(meObj.GetMainPoint(0).pnt.X, meObj.GetMainPoint(0).pnt.Y, 64, 64),
                                 meObj.lstAnimations(meObj.pntCurrentImgIndexes.X)(meObj.pntCurrentImgIndexes.Y), System.Drawing.GraphicsUnit.Pixel)
        Else
            e.Graphics.DrawImage(meObj.imgMainImage, New Rectangle(meObj.GetMainPoint(0).pnt.X, meObj.GetMainPoint(0).pnt.Y, 64, 64),
                                 meObj.lstAnimations(0)(0), GraphicsUnit.Pixel)
        End If
    End Sub

    Private shtCharSpeed As Short = 8
    Private shtMeCharSpeed As Short = 8
    Private Sub tmrGameUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrGameUpdate.Tick
        stwDebug.Stop()
        Me.Text = "Last tick, ms : " & Math.Round(stwDebug.ElapsedMilliseconds / 5).ToString  'This is debug for checking if lag.
        stwDebug.Restart()

        LookAtMouse()

        If meObj.pntCurrentImgIndexes.X = 1 Then 'Ends the animation after attacking, yes it is supposed to be 3 frames, the fourth fame looked bad.
            If meObj.pntCurrentImgIndexes.Y = 3 Then
                meObj.SetAnimationInterval(100)
                meObj.PlayAnimation(0)
                blnSwordOut = False
            End If
        End If

        If blnSwordOut = True Then 'If sword is attacking set rot and pos of sword.
            meObj.SetSecondPointRotation((sngRotationFactor - 90) / 57.2958)
        End If


        MovementStuff()

        CollisionStuff()

        Refresh() 'Actually Changes the screen
    End Sub

    Private Sub MovementStuff()
        If blnWDown = True Then
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

            meObj.SetMainPoint(New Point(meObj.GetMainPoint(0).pnt.X + run, meObj.GetMainPoint(0).pnt.Y + rise))
            'Debug.Print(Math.Sqrt(sqr(rise) + sqr(rise)) & " is size")
        ElseIf blnSDown = True Then
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
            'Debug.Print(Math.Sqrt(sqr(rise) + sqr(rise)) & " is size")
        End If

        For Each plyr As Player In lstOtherComputerObjects  'moves the Other computers' objects
            If plyr.blnWDown = True Then
                Dim run As Double = plyr.pntMyMousePos.X - plyr.GetMainPoint(0).pnt.X - 32
                Dim rise As Double = plyr.pntMyMousePos.Y - plyr.GetMainPoint(0).pnt.Y - 32

                Dim dis As Double = FindDistance(plyr.pntMyMousePos, plyr.GetMainPoint(0).pnt)
                Dim scale As Double = shtCharSpeed / dis

                rise *= scale
                run *= scale

                Dim num = Math.Abs(rise) + Math.Abs(run)
                If num <> 0 Then
                    Dim scale2 As Double = shtCharSpeed / num
                    rise *= scale2
                    run *= scale2
                End If

                plyr.SetMainPoint(New Point(plyr.GetMainPoint(0).pnt.X + run, plyr.GetMainPoint(0).pnt.Y + rise))
            ElseIf plyr.blnSDown = True Then
                Dim run As Double = plyr.pntMyMousePos.X - plyr.GetMainPoint(0).pnt.X - 32
                Dim rise As Double = plyr.pntMyMousePos.Y - plyr.GetMainPoint(0).pnt.Y - 32

                Dim dis As Double = FindDistance(plyr.pntMyMousePos, plyr.GetMainPoint(0).pnt)
                Dim scale As Double = shtCharSpeed / dis

                rise *= scale
                run *= scale

                Dim num = Math.Abs(rise) + Math.Abs(run)
                If num <> 0 Then
                    Dim scale2 As Double = shtCharSpeed / num
                    rise *= scale2
                    run *= scale2
                End If

                plyr.SetMainPoint(New Point(plyr.GetMainPoint(0).pnt.X - run, plyr.GetMainPoint(0).pnt.Y - rise))
            End If
        Next
    End Sub

    Public Sub CollisionStuff()
        'Other Coms & Walls
        For index As Short = 0 To lstOtherComputerObjects.Count - 1
            Dim pntTemp As Point = mapMain.CheckCollision(lstOtherComputerObjects(index).GetMainPoint(0), lstOtherComputerObjects(index).pntLastPos)
            lstOtherComputerObjects(index).SetMainPoint(New Point(lstOtherComputerObjects(index).GetMainPoint(0).pnt.X + pntTemp.X, lstOtherComputerObjects(index).GetMainPoint(0).pnt.Y + pntTemp.Y))
        Next

        'My Com & Walls
        Dim pntTemp2 As Point = mapMain.CheckCollision(meObj.GetMainPoint(0), meObj.pntLastPos)
        meObj.SetMainPoint(New Point(meObj.GetMainPoint(0).pnt.X + pntTemp2.X, meObj.GetMainPoint(0).pnt.Y + pntTemp2.Y))

        'My Com & Other Coms
        If lstOtherComputerObjects.Count > 0 Then
            CircleCollisionDynamic(lstOtherComputerObjects(0), meObj)  'TODO: test this
        End If

        'Other Coms & Circle Walls
        For index As Short = 0 To lstOtherComputerObjects.Count - 1
            For Each obj As OverDropObject In lstMapCircles
                CircleCollisionWall(obj, lstOtherComputerObjects(index))
            Next
        Next

        'My Com & Circle Walls
        For Each obj As OverDropObject In lstMapCircles
            CircleCollisionWall(obj, meObj)
        Next

        'My Com & AIs
        For Each obj As AI In lstAI
            Dim pntPush As Point = CircleCollisionDynamic(obj, meObj)
            If pntPush <> New Point(0, 0) Then
                If obj.blnIsDead = False Then
                    meObj.HitAI(pntPush)
                End If
                obj.PushBack(pntPush)
            End If
        Next

        'Walls & AIs
        For Each obj As AI In lstAI
            Dim pntTemp As Point = mapMain.CheckCollision(obj.GetMainPoint(0), obj.pntLastPos)
            obj.SetMainPoint(New Point(obj.GetMainPoint(0).pnt.X + pntTemp.X, obj.GetMainPoint(0).pnt.Y + pntTemp.Y))
        Next

        'Circle Walls & AIs
        For Each objCircle As OverDropObject In lstMapCircles
            For Each objAI As AI In lstAI
                CircleCollisionWall(objCircle, objAI)
            Next
        Next

        'Sword & AIs
        If blnSwordOut = True Then
            For Each obj As AI In lstAI
                If obj.blnIsDead = False Then
                    If CircleCollisionDetect(obj, meObj) = True Then
                        obj.HitPlayerSword(CircleCollisionNumbers(obj, meObj))
                    End If
                End If
            Next
        End If
    End Sub

    Private Function CircleCollisionDynamic(ByRef objDynamic1 As OverDropObject, ByRef objDynamic2 As OverDropObject) As Point 'checks collision and pushes both objects back, used for pushing . Also, Inheritance :D
        'Finds the rise and run of the two radius of the circles and scales it down to the overlap.
        If FindDistance(objDynamic1.GetDrawPoint(0), objDynamic2.GetDrawPoint(0)) < objDynamic2.GetMainPoint(0).sngRadius + objDynamic1.GetMainPoint(0).sngRadius Then
            Dim xMove, yMove As Single

            Dim run As Single = objDynamic2.GetMainPoint(0).pnt.X - objDynamic1.GetMainPoint(0).pnt.X
            Dim rise As Single = objDynamic2.GetMainPoint(0).pnt.Y - objDynamic1.GetMainPoint(0).pnt.Y

            Dim smallDis As Single = objDynamic2.GetMainPoint(0).sngRadius + objDynamic1.GetMainPoint(0).sngRadius - FindDistance(objDynamic1.GetDrawPoint(0), objDynamic2.GetDrawPoint(0))
            Dim scaleFactor As Single = smallDis / (objDynamic2.GetMainPoint(0).sngRadius + objDynamic1.GetMainPoint(0).sngRadius)

            'Scale the rise run to the amount to push back and cuts it in half.
            xMove = run * scaleFactor / 2
            yMove = rise * scaleFactor / 2

            objDynamic1.SetMainPoint(New Point(objDynamic1.GetDrawPoint(0).X - xMove, objDynamic1.GetDrawPoint(0).Y - yMove))
            objDynamic2.SetMainPoint(New Point(objDynamic2.GetDrawPoint(0).X + xMove, objDynamic2.GetDrawPoint(0).Y + yMove))
            Return New Point(xMove, yMove)
        End If
        Return New Point(0, 0)
    End Function

    Private Sub CircleCollisionWall(ByRef objWall As OverDropObject, ByRef objDynamic As OverDropObject) 'checks collision and pushes both objects back, used for pushing . Also, Inheritance :D
        'Finds the rise and run of the two radius of the circles and scales it down to the overlap.
        If FindDistance(objWall.GetDrawPoint(0), objDynamic.GetDrawPoint(0)) < objDynamic.GetMainPoint(0).sngRadius + objWall.GetMainPoint(0).sngRadius Then
            Dim xMove, yMove As Single

            Dim run As Single = objDynamic.GetMainPoint(0).pnt.X - objWall.GetMainPoint(0).pnt.X
            Dim rise As Single = objDynamic.GetMainPoint(0).pnt.Y - objWall.GetMainPoint(0).pnt.Y

            Dim smallDis As Single = objDynamic.GetMainPoint(0).sngRadius + objWall.GetMainPoint(0).sngRadius - FindDistance(objWall.GetDrawPoint(0), objDynamic.GetDrawPoint(0))
            Dim scaleFactor As Single = smallDis / (objDynamic.GetMainPoint(0).sngRadius + objWall.GetMainPoint(0).sngRadius)

            'Scales the rise run to the amount to push back.
            xMove = run * scaleFactor
            yMove = rise * scaleFactor

            objDynamic.SetMainPoint(New Point(objDynamic.GetDrawPoint(0).X + xMove, objDynamic.GetDrawPoint(0).Y + yMove))
        End If
    End Sub

    Private Function CircleCollisionDetect(ByRef objDynamic1 As OverDropObject, ByRef objDynamicSword As OverDropObject) As Boolean 'only checks collision detection and gives a true / false answer
        Debug.Print("If " & "FindDistance( " & objDynamic1.GetDrawPoint(0).ToString() & "," & objDynamicSword.GetMainPoint(1).pnt.ToString & ")" & "<" & objDynamicSword.GetMainPoint(1).sngRadius & "+" & objDynamic1.GetMainPoint(0).sngRadius & "Then")
        If FindDistance(objDynamic1.GetDrawPoint(0), objDynamicSword.GetMainPoint(1).pnt) < objDynamicSword.GetMainPoint(1).sngRadius + objDynamic1.GetMainPoint(0).sngRadius Then
            Return True
        End If
        Return False
    End Function

    Private Function CircleCollisionNumbers(ByRef objDynamic1 As OverDropObject, ByRef objDynamic2 As OverDropObject) As Point 'ok...  This doesn't push back but gives the number
        'Finds the rise and run of the two radius of the circles and scales it down to the overlap.
            Dim xMove, yMove As Single

            Dim run As Single = objDynamic2.GetMainPoint(0).pnt.X - objDynamic1.GetMainPoint(0).pnt.X
            Dim rise As Single = objDynamic2.GetMainPoint(0).pnt.Y - objDynamic1.GetMainPoint(0).pnt.Y

            Dim smallDis As Single = objDynamic2.GetMainPoint(0).sngRadius + objDynamic1.GetMainPoint(0).sngRadius - FindDistance(objDynamic1.GetDrawPoint(0), objDynamic2.GetDrawPoint(0))
            Dim scaleFactor As Single = smallDis / (objDynamic2.GetMainPoint(0).sngRadius + objDynamic1.GetMainPoint(0).sngRadius)

            'Scale the rise run to the amount to push back and cuts it in half.
            xMove = run * scaleFactor / 2
            yMove = rise * scaleFactor / 2

            Return New Point(xMove, yMove)
    End Function

    Private Sub LookAtMouse()
        Dim angle = FindAngle(pntMouse, New Point(meObj.GetMainPoint(0).pnt.X + 32, meObj.GetMainPoint(0).pnt.Y + 32),
                              New Point(0, Short.MinValue), New Point(0, Short.MaxValue)) * 57.2958
        sngRotationFactor = angle
    End Sub

    Public pntMouse As Point
    Private Sub Form1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbxPlayArea.MouseMove
        pntMouse = New Point(e.X, e.Y)
        Debug.Print("num is, " & pntMouse.ToString())
    End Sub

    Public blnSwordOut As Boolean
    Private Sub MouseClickDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles pbxPlayArea.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            meObj.SetAnimationInterval(25)
            meObj.PlayAnimation(1)
            blnSwordOut = True
        End If
    End Sub

    Public blnWDown As Boolean = False
    Public blnADown As Boolean = False
    Public blnSDown As Boolean = False
    Public blnDDown As Boolean = False
    Public blnDashOn As Boolean = False
    Private Sub ButtonPress(ByVal o As System.Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        'Movement Keys
        If e.KeyCode = Keys.W And blnWDown = False Then
            'give button down
            blnWDown = True
            GiveKeyDownToFriends(My.Computer.Name, "W", pntMouse)
        End If
        If e.KeyCode = Keys.A And blnADown = False Then
            blnADown = True
            GiveKeyDownToFriends(My.Computer.Name, "A", pntMouse)
        End If
        If e.KeyCode = Keys.S And blnSDown = False Then
            blnSDown = True
            GiveKeyDownToFriends(My.Computer.Name, "S", pntMouse)
        End If
        If e.KeyCode = Keys.D And blnDDown = False Then
            blnDDown = True
            GiveKeyDownToFriends(My.Computer.Name, "D", pntMouse)
        End If

        If e.KeyCode = Keys.Space Then
            blnDashOn = True
            shtMeCharSpeed = 16
        End If  

        'Debug keys
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
            For Each obj As AI In lstAI 'Colliding Ai with walls
                obj.SetMainPoint(New Point(obj.GetMainPoint(0).pnt.X - 1, obj.GetMainPoint(0).pnt.Y))
            Next
        End If
    End Sub

    Private Sub ButtonUnPress(ByVal o As System.Object, ByVal e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.W Then
            blnWDown = False
            GiveKeyUpToFriends(My.Computer.Name, "W", meObj.GetDrawPoint(0))
        End If
        If e.KeyCode = Keys.A Then
            blnADown = False
            GiveKeyUpToFriends(My.Computer.Name, "A", meObj.GetDrawPoint(0))
        End If
        If e.KeyCode = Keys.S Then
            blnSDown = False
            GiveKeyUpToFriends(My.Computer.Name, "S", meObj.GetDrawPoint(0))
        End If
        If e.KeyCode = Keys.D Then
            blnDDown = False
            GiveKeyUpToFriends(My.Computer.Name, "D", meObj.GetDrawPoint(0))
        End If
    End Sub

    Private Sub GiveKeyDownToFriends(ByVal strMyName As String, ByVal key As Char, ByVal mousePnt As Point)
        'send my computers the name
        For index As Short = 0 To lstComputers.Count - 1
            If lstComputers(index).ToString = My.Computer.Name Then
                Continue For  'Don't send it to my name
            End If

            client = New TcpClient(lstComputers(index).ToString, 5019)

            Dim Writer As New StreamWriter(client.GetStream())  'Give MousePos Too
            Writer.Write(chrStartProcessingText & strMyName & chrSendFrom &
                         chrStartProcessingText & mousePnt.X & chrMakeNum &
                         chrStartProcessingText & mousePnt.Y & chrMakeNum &
                         chrStartProcessingText & key & chrKeyUnPress)
            Writer.Flush()
            Writer.Flush()
        Next
    End Sub

    Private Sub GiveKeyUpToFriends(ByVal strMyName As String, ByVal key As Char, ByVal myPnt As Point)
        'send my computers the name
        For index As Short = 0 To lstComputers.Count - 1
            If lstComputers(index).ToString = My.Computer.Name Then
                Continue For  'Don't send it to my name
            End If

            client = New TcpClient(lstComputers(index).ToString, 5019)

            Dim Writer As New StreamWriter(client.GetStream())
            Writer.Write(chrStartProcessingText & strMyName & chrSendFrom &
                         chrStartProcessingText & myPnt.X & chrMakeNum &
                         chrStartProcessingText & myPnt.Y & chrMakeNum &
                         chrStartProcessingText & key & chrKeyUnPress)
            Writer.Flush()

            'Send Pos too.
        Next
    End Sub

    'Start Converted Internet
    Function sqr(ByVal x) As Double
        Return x * x
    End Function

    Function DistanceSquared(ByVal v As Point, ByVal w As Point) As Integer
        Return sqr(v.X - w.X) + sqr(v.Y - w.Y)
    End Function

    Function DistanceToSegmentSquared(ByVal p As Point, ByVal v As Point, ByVal w As Point) As Integer
        Dim l2 = DistanceSquared(v, w)
        If l2 = 0 Then
            Return DistanceSquared(p, v)
        End If

        Dim t = ((p.X - v.X) * (w.X - v.X) + (p.Y - v.Y) * (w.Y - v.Y)) / l2
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
            'Return P with a specific data
        Else
            Dim x As Double = ((C.Y - A.Y) * dx1 * dx2 + dy1 * dx2 * A.X - dy2 * dx1 * C.X) / (dy1 * dx2 - dy2 * dx1)
            Dim y As Double = A.Y + (dy1 / dx1) * (x - A.X)
            p = New Point(x, y)
            Return p
        End If
    End Function

    Function FindAngle(ByVal l11 As Point, ByVal l12 As Point, ByVal l21 As Point, ByVal l22 As Point) As Single
        Return (Math.Atan2(l12.Y - l11.Y, l12.X - l11.X) - Math.Atan2(l22.Y - l21.Y, l22.X - l21.X)) '* (180 / Math.PI)
    End Function

    Function FindDistance(ByVal pnt1 As Point, ByVal pnt2 As Point) As Single
        Return Math.Sqrt(sqr(pnt2.X - pnt1.X) + sqr(pnt2.Y - pnt1.Y))
    End Function
    'End Converted Internet

    'Main LAN Stuff vvv
    Private Sub Listening()  'starts listening for info from other com.
        listener.Start()
    End Sub

    Private Sub tmrListenerUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrListenerUpdate.Tick   'Get info from other computers here.
        Try
            If listener.Pending = True Then  'If a Computer wants to send something.
                client = listener.AcceptTcpClient() 'Accepts the "message".

                ProcessInformation(client)

            End If
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub

    Private Sub ProcessInformation(ByRef client As System.Net.Sockets.TcpClient)
        Dim Reader As New StreamReader(client.GetStream())  'Start getting the other com info.

        Dim currentProcess As Process = Process.None 'Current thing to do with the recieved information.

        Dim strPersonFrom As String = ""
        Dim shtInfo As String = ""

        Dim lstSht As New List(Of Short)

        While Reader.Peek > -1  'Changes each character sent into a string and adds it to the 
            Dim chrCurrent As Char = Convert.ToChar(Reader.Read())
            Debug.Print("char " & chrCurrent)

            'Set process if needed
            If chrCurrent = chrStartProcessingText Then
                currentProcess = Process.StringStart
                Continue While

            ElseIf chrCurrent = chrAddComToConnectListEnd Then
                currentProcess = Process.AddComEnd

            ElseIf chrCurrent = chrGiveComs Then
                currentProcess = Process.GiveComs

            ElseIf chrCurrent = chrMessageEnd Then
                currentProcess = Process.MessageEnd

            ElseIf chrCurrent = chrKeyPress Then
                currentProcess = Process.PressedKey

            ElseIf chrCurrent = chrKeyUnPress Then
                currentProcess = Process.UnPressedKey

            ElseIf chrCurrent = chrSendFrom Then
                currentProcess = Process.SendFrom

            ElseIf chrCurrent = chrMakeNum Then
                currentProcess = Process.NumGet

            End If

            If currentProcess = Process.StringStart Then 'Adds chars to be proccessed
                shtInfo += chrCurrent

            ElseIf currentProcess = Process.AddComEnd Then  'this is inefficient cause all coms have to give anmes and it repeats.
                Debug.Print(" Almost Got Computer : " & shtInfo)
                If lstComputers.Contains(shtInfo) = False And shtInfo <> My.Computer.Name Then
                    'Don't need this, maybe breaking it?  think i do need it
                    GiveComNamesToFriends(shtInfo)

                    AddComputerToList(shtInfo)
                    Debug.Print(" Got Computer : " & shtInfo)

                End If
                shtInfo = String.Empty

            ElseIf currentProcess = Process.GiveComs Then 'Other computer tells it's name, this computer gives any other names to that com.
                Debug.Print(" Was Given : " & shtInfo)

                If lstComputers.Count <> 0 Then 'If lstComputers has items in it, send them
                    Dim Writer As StreamWriter

                    For index As Short = 0 To lstComputers.Count - 1
                        client = New TcpClient(shtInfo, 5019)
                        Writer = New StreamWriter(client.GetStream())
                        Writer.Write(chrStartProcessingText & lstComputers(index).ToString() & chrAddComToConnectListEnd)
                        Writer.Flush()
                    Next

                End If

                If lstComputers.Contains(shtInfo) = False And shtInfo <> My.Computer.Name Then
                    'then adds
                    AddComputerToList(shtInfo)

                    'Don't need this, maybe breaking it?  think i do need it
                    GiveComNamesToFriends(shtInfo)
                End If

                shtInfo = String.Empty

            ElseIf currentProcess = Process.MessageEnd Then
                lbxChatConsole.Items.Add(shtInfo.ToString())

                shtInfo = String.Empty

            ElseIf currentProcess = Process.PressedKey Then  'Gets a pressed key from another computer.
                lstOtherComputerObjects(lstComputers.IndexOf(strPersonFrom)).SetKeyPressed(shtInfo)
                lstOtherComputerObjects(lstComputers.IndexOf(strPersonFrom)).pntMyMousePos = New Point(lstSht(0), lstSht(1)) 'Gets the mouse pos

                shtInfo = String.Empty
                strPersonFrom = String.Empty

            ElseIf currentProcess = Process.UnPressedKey Then  'Gets an unpressed key from another computer. 'X is sent first, then y   
                lstOtherComputerObjects(lstComputers.IndexOf(strPersonFrom)).SetKeyUp(shtInfo)
                lstOtherComputerObjects(lstComputers.IndexOf(strPersonFrom)).SetMainPoint(New Point(lstSht(0), lstSht(1)))

                'clear variables.
                shtInfo = String.Empty
                strPersonFrom = String.Empty
                lstSht.Clear()

                Debug.Print("Keyup")

            ElseIf currentProcess = Process.SendFrom Then
                strPersonFrom = shtInfo

                shtInfo = String.Empty

            ElseIf currentProcess = Process.NumGet Then
                lstSht.Add(Val(shtInfo))

                shtInfo = String.Empty

            Else
                lbxChatConsole.Items.Add(" No Understandable Message Header ")

            End If

        End While
    End Sub

    Private Sub btnUpdateChatConsole_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdateChatConsole.Click
        Try
            ConnectToComputersAndSendMessages()

            lbxChatConsole.Items.Add(tbxMessageToSend.Text)
            lbxChatConsole.Items.Add(My.Computer.Name)

            Me.tbxMessageToSend.Text = "Sent!"
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub

    Private Sub ConnectToComputersAndSendMessages()
        For index As Short = 0 To lstComputers.Count - 1
            client = New TcpClient(lstComputers(index), 5019)

            'Sends the message.
            Dim Writer As New StreamWriter(client.GetStream())
            Writer.Write(chrStartProcessingText & tbxMessageToSend.Text & chrMessageEnd &
                         chrStartProcessingText & My.Computer.Name & chrMessageEnd)
            Writer.Flush()
        Next
    End Sub

    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click  'Connects to a computer
        If lstComputers.Contains(tbxConnectionComputerName.Text) Then
            Exit Sub
        End If

        'Send my name and ask for it's computers
        client = New TcpClient(tbxConnectionComputerName.Text, 5019)
        Dim Writer As New StreamWriter(client.GetStream())
        Writer.Write(chrStartProcessingText & My.Computer.Name & chrGiveComs)
        Writer.Flush()

        'send *it* my computers.

        For index As Short = 0 To lstComputers.Count - 1
            If lstComputers(index).ToString = My.Computer.Name Then
                Continue For  'Don't include my name
            End If

            client = New TcpClient(tbxConnectionComputerName.Text, 5019)

            Writer = New StreamWriter(client.GetStream())
            Writer.Write(chrStartProcessingText & lstComputers(index).ToString & chrAddComToConnectListEnd)
            Writer.Flush()
        Next

        'Give my friends it's name
        GiveComNamesToFriends(tbxConnectionComputerName.Text)

        AddComputerToList(tbxConnectionComputerName.Text)
    End Sub

    Private Sub GiveComNamesToFriends(ByVal name As String)
        Try
            'send my computers the name
            For index As Short = 0 To lstComputers.Count - 1
                If lstComputers(index).ToString = My.Computer.Name Then
                    Continue For  'Don't send it to my name
                End If

                client = New TcpClient(lstComputers(index).ToString, 5019)

                Dim Writer As New StreamWriter(client.GetStream())
                Writer.Write(chrStartProcessingText & name & chrAddComToConnectListEnd)
                Writer.Flush()
            Next
        Catch ex As Exception
            Console.WriteLine(ex)
            'Dim Errorresult As String = ex.Message
            'MessageBox.Show(Errorresult & vbCrLf & vbCrLf & "Please Review Client Address", "Error Sending Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AddComputerToList(ByVal strName As String)
        lstOtherComputerObjects.Add(New Player(New Point(0, 0), imgGoodSpriteSheet, 32))  'Adds a new character to the screen
        lstComputers.Add(strName)
        lbxComputersConnectedTo.Items.Add(strName)
    End Sub

End Class
