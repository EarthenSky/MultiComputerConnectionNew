Imports System.Net.Sockets
Imports System.Net
Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.IO

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
    'Can connect multiple computers (3) together and can ping name to console.

    Public Const chrStartProcessingText As Char = Chr(0)
    Public Const chrAddComToConnectListEnd As Char = Chr(1)
    Public Const chrGiveComs As Char = Chr(2)
    Public Const chrMessageEnd As Char = Chr(3)
    Public Const chrKeyPress As Char = Chr(4) 'send key pressed
    Public Const chrKeyUnPress As Char = Chr(7) 'send key released
    Public Const chrSendFrom As Char = Chr(5) 'person from
    Public Const chrMakeNum As Char = Chr(6) 'makes a num value

    Private currentFileDirectory As String = Directory.GetCurrentDirectory.Remove(Directory.GetCurrentDirectory.IndexOf("\bin\Debug"), 10) + "\"
    Private imgEnemyOne As Image
    Private imgEnemyTwo As Image
    Public imgCircle As Image

    Private listener As New TcpListener(5019)
    Private client As New TcpClient

    Public lstComputers As New List(Of String)
    Public meObj As OverDropObject
    Public otherComObj As New List(Of Player)

    Public lstAnimationObjects As New List(Of AnimationObject)

    Public pntTest1 As New Point(300, 200)
    Public pntTest2 As New Point(400, 300)

    Public collisionMap1 As Image
    Public drawMap1 As Image

    Public mapObj As CollisionMap

    Public stw As New Stopwatch()

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        imgEnemyOne = Image.FromFile(currentFileDirectory & "EnemyFOne.png")
        imgEnemyTwo = Image.FromFile(currentFileDirectory & "BugOne_MiniEnemy_SpriteSheet.png")

        imgCircle = Image.FromFile(currentFileDirectory & "Circle.png")

        drawMap1 = Image.FromFile(currentFileDirectory & "ColliderMap2Draw.png")
        collisionMap1 = Image.FromFile(currentFileDirectory & "ColliderMap2.png")

        mapObj = New CollisionMap(drawMap1, collisionMap1)

        meObj = New OverDropObject(New Point(200, 200), imgEnemyOne, 32)
        lstAnimationObjects.Add(New AnimationObject(New Point(0, 0), imgEnemyTwo, 32, 100))

        Me.KeyPreview = True

        Dim ListenerThread As New Thread(New ThreadStart(AddressOf Listening))
        tbxConnectionComputerName.Text = My.Computer.Name
        ListenerThread.Start()

        stw.Start()
    End Sub

    Private Sub PaintMain(ByVal o As Object, ByVal e As PaintEventArgs) Handles pbxPlayArea.Paint
        mapObj.Draw(e)  'Draw at the bottom.

        For Each obj As Player In otherComObj 'Draws otherComputer controlled objects
            e.Graphics.DrawImage(imgEnemyTwo, New Rectangle(obj.GetDrawPoint(0).X, obj.GetDrawPoint(0).Y, meObj.imgMainImage.Width / 4, meObj.imgMainImage.Height / 4))
            'e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetDrawPoint().X, obj.GetDrawPoint().Y, obj.imgMainImage.Width, obj.imgMainImage.Height / 2))
        Next

        For Each obj As AnimationObject In lstAnimationObjects 'Draws animations
            If obj.pntCurrentImgIndexes.X <> -1 Then
                e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetMainPoint(0).pnt.X, obj.GetMainPoint(0).pnt.Y, 256, 256),
                                     obj.lstAnimations(obj.pntCurrentImgIndexes.X)(obj.pntCurrentImgIndexes.Y), System.Drawing.GraphicsUnit.Pixel)
            Else
                e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetMainPoint(0).pnt.X, obj.GetMainPoint(0).pnt.Y, 256, 256),
                                     obj.lstAnimations(0)(0), GraphicsUnit.Pixel)
            End If
        Next

        e.Graphics.DrawImage(imgCircle, New Rectangle(meObj.GetDrawPoint(0).X, meObj.GetDrawPoint(0).Y, meObj.imgMainImage.Width / 4, meObj.imgMainImage.Height / 4))

        'e.Graphics.DrawImage(imgCircle, New Rectangle(pntTest1.X, pntTest1.Y, meObj.imgMainImage.Width / 8, meObj.imgMainImage.Height / 8))
        'e.Graphics.DrawImage(imgCircle, New Rectangle(pntTest2.X, pntTest2.Y, meObj.imgMainImage.Width / 8, meObj.imgMainImage.Height / 8))
        'Debug.Print(meObj.GetMainPointMiddle().pnt.ToString & ", hey this is pos")
    End Sub

    Private shtCharSpeed As Short = 15
    Private Sub tmrGameUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrGameUpdate.Tick
        stw.Stop()
        Me.Text = "Last tick, ms : " & Math.Round(stw.ElapsedMilliseconds / 10).ToString
        stw.Restart()

        If blnWDown = True Then
            meObj.SetMainPoint(New Point(meObj.GetDrawPoint(0).X, meObj.GetDrawPoint(0).Y - shtCharSpeed))
        End If
        If blnADown = True Then
            meObj.SetMainPoint(New Point(meObj.GetDrawPoint(0).X - shtCharSpeed, meObj.GetDrawPoint(0).Y))
        End If
        If blnSDown = True Then
            meObj.SetMainPoint(New Point(meObj.GetDrawPoint(0).X, meObj.GetDrawPoint(0).Y + shtCharSpeed))
        End If
        If blnDDown = True Then
            meObj.SetMainPoint(New Point(meObj.GetDrawPoint(0).X + shtCharSpeed, meObj.GetDrawPoint(0).Y))
        End If

        For Each plyr As Player In otherComObj
            If plyr.blnWDown = True Then
                plyr.SetMainPoint(New Point(plyr.GetDrawPoint(0).X, plyr.GetDrawPoint(0).Y - shtCharSpeed))
            End If
            If plyr.blnADown = True Then
                plyr.SetMainPoint(New Point(plyr.GetDrawPoint(0).X - shtCharSpeed, plyr.GetDrawPoint(0).Y))
            End If
            If plyr.blnSDown = True Then
                plyr.SetMainPoint(New Point(plyr.GetDrawPoint(0).X, plyr.GetDrawPoint(0).Y + shtCharSpeed))
            End If
            If plyr.blnDDown = True Then
                plyr.SetMainPoint(New Point(plyr.GetDrawPoint(0).X + shtCharSpeed, plyr.GetDrawPoint(0).Y))
            End If
        Next

        'Check other computer for collision with walls
        For index As Short = 0 To otherComObj.Count - 1
            Dim pntTemp As Point = mapObj.CheckCollision(otherComObj(index).GetMainPoint(0), otherComObj(index).pntLastPos)
            otherComObj(index).SetMainPoint(New Point(otherComObj(index).GetMainPoint(0).pnt.X + pntTemp.X, otherComObj(index).GetMainPoint(0).pnt.Y + pntTemp.Y))

        Next

        'Check main character for collision with the walls
        Dim pntTemp2 As Point = mapObj.CheckCollision(meObj.GetMainPoint(0), meObj.pntLastPos)
        meObj.SetMainPoint(New Point(meObj.GetMainPoint(0).pnt.X + pntTemp2.X, meObj.GetMainPoint(0).pnt.Y + pntTemp2.Y))

#If False Then

        'Debug.Print(int.ToString & " is Distance")
        If DistanceToSegment(meObj.GetMainPoint(0).pnt, pntTest1, pntTest2) < meObj.GetMainPoint(0).sngRadius Then

            Dim pnt1 As Point
            Dim pnt2 As Point
            If pntTest1.X > pntTest2.X Then
                pnt1 = pntTest1
                pnt2 = pntTest2
            Else
                pnt1 = pntTest1
                pnt2 = pntTest2
            End If

            Dim xMove, yMove As Short
            'Works  1.5708 = 90 deg in radians
            Dim bAngle As Single = 1.5708 - Math.Abs(FindAngle(pnt1, pnt2, meObj.GetMainPoint(0).pnt, pnt2))
            Debug.Print(bAngle & "=b, " & pnt1.ToString & "=pnt1, " & pnt2.ToString & "=pnt2, " & meObj.GetMainPoint(0).pnt.ToString & "=cbx.pnt.")
            Dim nSide As Single = Math.Cos(bAngle) * FindDistance(meObj.GetMainPoint(0).pnt, pnt2)
            'Works
            Dim lLength As Single = Math.Abs(meObj.GetMainPoint(0).sngRadius - nSide)

            Dim angleCDDL As Single = FindAngle(pnt1, pnt2, FindIntersectPoint(pnt1, pnt2, New Point(0, Short.MaxValue), New Point(0, Short.MinValue)), New Point(0, Short.MaxValue))

            Dim shtSideValue As Short = (meObj.pntLastPos.X - pnt1.X) * (pnt2.Y - pnt1.Y) - (meObj.pntLastPos.Y - pnt1.Y) * (pnt2.X - pnt1.X)  'Negitave and positive says what side the point is on, thanks internet code.
            Debug.Print(shtSideValue)
            If pnt1.Y < pnt2.Y Then
                If shtSideValue < 0 Then
                    angleCDDL = (1.5708 * 2) + angleCDDL
                End If
                Me.Text = "1" 'TODO: TYPE 1
            Else
                If shtSideValue < 0 Then
                    angleCDDL = (1.5708 * 2) + angleCDDL
                End If
                Me.Text = "2"  'TODO: TYPE 2 
            End If

            Dim rise As Single = Math.Sin(angleCDDL) * meObj.GetMainPoint(0).sngRadius

            Dim run As Single = Math.Cos(angleCDDL) * meObj.GetMainPoint(0).sngRadius

            Dim scale As Single = lLength / meObj.GetMainPoint(0).sngRadius

            yMove = (rise * scale)
            xMove = (run * scale)

            meObj.SetMainPoint(New Point(meObj.GetDrawPoint(0).X + xMove, meObj.GetDrawPoint(0).Y + yMove))
        End If
#End If

        If otherComObj.Count > 0 AndAlso FindDistance(otherComObj(0).GetDrawPoint(0), meObj.GetDrawPoint(0)) < meObj.GetMainPoint(0).sngRadius + otherComObj(0).GetMainPoint(0).sngRadius Then
            Dim xMove, yMove As Single

            'Finds the rise and run of the two radius of the circles and scales it down to the overlap.

            Dim run As Single = meObj.GetMainPoint(0).pnt.X - otherComObj(0).GetMainPoint(0).pnt.X
            Dim rise As Single = meObj.GetMainPoint(0).pnt.Y - otherComObj(0).GetMainPoint(0).pnt.Y

            Dim smallDis As Single = meObj.GetMainPoint(0).sngRadius + otherComObj(0).GetMainPoint(0).sngRadius - FindDistance(otherComObj(0).GetDrawPoint(0), meObj.GetDrawPoint(0))
            Dim scaleFactor As Single = smallDis / (meObj.GetMainPoint(0).sngRadius + otherComObj(0).GetMainPoint(0).sngRadius)

            xMove = run * scaleFactor / 2
            yMove = rise * scaleFactor / 2

            If (blnADown = True Or blnWDown = True) And (blnSDown = False Or blnDDown = False) Then
                otherComObj(0).SetMainPoint(New Point(otherComObj(0).GetDrawPoint(0).X - xMove, otherComObj(0).GetDrawPoint(0).Y - yMove))
                meObj.SetMainPoint(New Point(meObj.GetDrawPoint(0).X + xMove, meObj.GetDrawPoint(0).Y + yMove))
            Else
                otherComObj(0).SetMainPoint(New Point(otherComObj(0).GetDrawPoint(0).X - xMove, otherComObj(0).GetDrawPoint(0).Y - yMove))
                meObj.SetMainPoint(New Point(meObj.GetDrawPoint(0).X + xMove, meObj.GetDrawPoint(0).Y + yMove))
                'otherComObj(0).SetMainPoint(New Point(otherComObj(0).GetDrawPoint().X + xMove, otherComObj(0).GetDrawPoint().Y + yMove))
                'meObj.SetMainPoint(New Point(meObj.GetDrawPoint().X - xMove, meObj.GetDrawPoint().Y - yMove))
            End If
        End If

        Refresh()
    End Sub

    Public blnWDown As Boolean = False
    Public blnADown As Boolean = False
    Public blnSDown As Boolean = False
    Public blnDDown As Boolean = False
    Private Sub MoveButtonPress(ByVal o As System.Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        'Movement Keys
        If e.KeyCode = Keys.W And blnWDown = False Then
            'give button down
            blnWDown = True
            GiveKeyDownToFriends(My.Computer.Name, "W")
        End If
        If e.KeyCode = Keys.A And blnADown = False Then
            blnADown = True
            GiveKeyDownToFriends(My.Computer.Name, "A")
        End If
        If e.KeyCode = Keys.S And blnSDown = False Then
            blnSDown = True
            GiveKeyDownToFriends(My.Computer.Name, "S")
        End If
        If e.KeyCode = Keys.D And blnDDown = False Then
            blnDDown = True
            GiveKeyDownToFriends(My.Computer.Name, "D")
        End If

        'Debug keys
        If e.KeyCode = Keys.E Then
            lstAnimationObjects(0).PlayAnimation(1)
        ElseIf e.KeyCode = Keys.Up Then
            lstAnimationObjects(0).PlayAnimation(0)
        ElseIf e.KeyCode = Keys.T Then
            lstAnimationObjects(0).StopAnimation()
        End If
    End Sub

    Private Sub MoveButtonUp(ByVal o As System.Object, ByVal e As KeyEventArgs) Handles Me.KeyUp
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

    Private Sub GiveKeyDownToFriends(ByVal strMyName As String, ByVal key As Char)
        'send my computers the name
        For index As Short = 0 To lstComputers.Count - 1
            If lstComputers(index).ToString = My.Computer.Name Then
                Continue For  'Don't send it to my name
            End If

            client = New TcpClient(lstComputers(index).ToString, 5019)

            Dim Writer As New StreamWriter(client.GetStream())
            Writer.Write(chrStartProcessingText & strMyName & chrSendFrom &
                         chrStartProcessingText & key & chrKeyPress)
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

    '#If False Then
    'Start Converted Internet
    Function sqr(ByVal x) As Integer
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
            MessageBox.Show("no point")
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
    '#End If

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
            'Dim Errorresult As String = ex.Message
            'MessageBox.Show(Errorresult & vbCrLf & vbCrLf & "???", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
                otherComObj(lstComputers.IndexOf(strPersonFrom)).SetKeyPressed(shtInfo)

                shtInfo = String.Empty
                strPersonFrom = String.Empty

            ElseIf currentProcess = Process.UnPressedKey Then  'Gets an unpressed key from another computer. 'X is sent first, then y   
                otherComObj(lstComputers.IndexOf(strPersonFrom)).SetKeyUp(shtInfo)
                otherComObj(lstComputers.IndexOf(strPersonFrom)).SetMainPoint(New Point(lstSht(0), lstSht(1)))

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
            'Dim Errorresult As String = ex.Message
            'MessageBox.Show(Errorresult & vbCrLf & vbCrLf & "Please Review Client Address", "Error Sending Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
        otherComObj.Add(New Player(New Point(0, 0), imgEnemyOne, 32))  'Adds a new character to the screen
        lstComputers.Add(strName)
        lbxComputersConnectedTo.Items.Add(strName)
    End Sub

End Class
