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
    ChangePos
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

    Public Const chrChangePosition As Char = Chr(4) 'send character position
    Public Const chrSendFrom As Char = Chr(5) 'person from
    Public Const chrMakeNum As Char = Chr(6) 'makes a num value

    Private currentFileDirectory As String = Directory.GetCurrentDirectory.Remove(Directory.GetCurrentDirectory.IndexOf("\bin\Debug"), 10) + "\"
    Private imgEnemyOne As Image
    Private imgEnemyTwo As Image

    Private listener As New TcpListener(5019)
    Private client As New TcpClient

    Private lstComputers As New List(Of String)
    Public meObj As OverDropObject
    Public otherComObj As New List(Of OverDropObject)

    Public lstAnimationObjects As New List(Of AnimationObject)

    Public pnt1 As New Point(0, 800)
    Public pnt2 As New Point(245, 500)

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        imgEnemyOne = Image.FromFile(currentFileDirectory & "EnemyFOne.png")
        imgEnemyTwo = Image.FromFile(currentFileDirectory & "BugOne_MiniEnemy_SpriteSheet.png")

        meObj = New OverDropObject(New Point(0, 0), imgEnemyOne)
        lstAnimationObjects.Add(New AnimationObject(New Point(0, 0), imgEnemyTwo, 100))

        Me.KeyPreview = True

        Dim ListenerThread As New Thread(New ThreadStart(AddressOf Listening))
        tbxConnectionComputerName.Text = My.Computer.Name
        ListenerThread.Start()
    End Sub

    Private Sub PaintMain(ByVal o As Object, ByVal e As PaintEventArgs) Handles pbxPlayArea.Paint
        For Each obj As OverDropObject In otherComObj 'Draws other Computer controlled objects
            e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetDrawPoint().X, obj.GetDrawPoint().Y, obj.imgMainImage.Width / 2, obj.imgMainImage.Height / 2))
        Next

        For Each obj As AnimationObject In lstAnimationObjects 'Draws animations
            If obj.pntCurrentImgIndexes.X <> -1 Then
                e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetMainPoint().pnt.X, obj.GetMainPoint().pnt.Y, 256, 256),
                                     obj.lstAnimations(obj.pntCurrentImgIndexes.X)(obj.pntCurrentImgIndexes.Y), System.Drawing.GraphicsUnit.Pixel)
            Else
                e.Graphics.DrawImage(obj.imgMainImage, New Rectangle(obj.GetMainPoint().pnt.X, obj.GetMainPoint().pnt.Y, 256, 256),
                                     obj.lstAnimations(0)(0), System.Drawing.GraphicsUnit.Pixel)
            End If
        Next

        e.Graphics.DrawImage(imgEnemyTwo, New Rectangle(meObj.GetDrawPoint().X, meObj.GetDrawPoint().Y, meObj.imgMainImage.Width / 2, meObj.imgMainImage.Height / 2))
        Debug.Print(meObj.GetMainPointMiddle().pnt.ToString & ", hey this is pos")
    End Sub

    Private Sub ButtonPress(ByVal o As System.Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.W Then
            GivePositionChangeToFriends(My.Computer.Name, New Point(meObj.GetDrawPoint().X, meObj.GetDrawPoint().Y - 5)) 'send before do
            meObj.SetMainPoint(New Point(meObj.GetDrawPoint().X, meObj.GetDrawPoint().Y - 5))
        ElseIf e.KeyCode = Keys.S Then
            GivePositionChangeToFriends(My.Computer.Name, New Point(meObj.GetDrawPoint().X, meObj.GetDrawPoint().Y + 5)) 'send before do
            meObj.SetMainPoint(New Point(meObj.GetDrawPoint().X, meObj.GetDrawPoint().Y + 5))
        ElseIf e.KeyCode = Keys.A Then
            GivePositionChangeToFriends(My.Computer.Name, New Point(meObj.GetDrawPoint().X - 5, meObj.GetDrawPoint().Y)) 'send before do
            meObj.SetMainPoint(New Point(meObj.GetDrawPoint().X - 5, meObj.GetDrawPoint().Y))
        ElseIf e.KeyCode = Keys.D Then
            GivePositionChangeToFriends(My.Computer.Name, New Point(meObj.GetDrawPoint().X + 5, meObj.GetDrawPoint().Y)) 'send before do
            meObj.SetMainPoint(New Point(meObj.GetDrawPoint().X + 5, meObj.GetDrawPoint().Y))
        ElseIf e.KeyCode = Keys.E Then
            lstAnimationObjects(0).PlayAnimation(1)
        ElseIf e.KeyCode = Keys.Up Then
            lstAnimationObjects(0).PlayAnimation(0)
        ElseIf e.KeyCode = Keys.T Then
            lstAnimationObjects(0).StopAnimation()
        End If

    End Sub

    Private Sub GivePositionChangeToFriends(ByVal strName As String, ByVal pnt As Point)
        'send my computers the name
        For index As Short = 0 To lstComputers.Count - 1
            If lstComputers(index).ToString = My.Computer.Name Then
                Continue For  'Don't send it to my name
            End If

            client = New TcpClient(lstComputers(index).ToString, 5019)

            Dim Writer As New StreamWriter(client.GetStream())
            Writer.Write(chrStartProcessingText & strName & chrSendFrom &
                         chrStartProcessingText & pnt.X.ToString & chrMakeNum &
                         chrStartProcessingText & pnt.Y.ToString & chrMakeNum &
                         chrChangePosition)
            Writer.Flush()
        Next
    End Sub

    'Start Internet
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
    'End Internet

    'Start Internet2
    Function FindAngle(ByVal l11 As Point, ByVal l12 As Point, ByVal l21 As Point, ByVal l22 As Point) As Single
        Return (Math.Atan2(l12.Y - l11.Y, l12.X - l11.X) - Math.Atan2(l22.Y - l21.Y, l22.X - l21.X)) '* (180 / Math.PI)
    End Function

    Function FindDistance(ByVal pnt1 As Point, ByVal pnt2 As Point) As Single
        Return Math.Sqrt(sqr(pnt2.X - pnt1.X) + sqr(pnt2.Y - pnt1.Y))
    End Function
    'End Internet2

    Private Sub tmrGameUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrGameUpdate.Tick
        Dim int As Integer = DistanceToSegment(meObj.GetMainPointMiddle().pnt, pnt1, pnt2)
        Debug.Print(int.ToString & " is Distance")
        If DistanceToSegment(meObj.GetMainPointMiddle().pnt, pnt1, pnt2) < meObj.GetMainPointMiddle().sngRadius Then
            Dim xMove, yMove As Short
            'Works
            Dim bAngle As Single = 1.5708 - Math.Abs(FindAngle(pnt1, pnt2, meObj.GetMainPointMiddle().pnt, pnt2))
            'Works
            Dim nSide As Single = Math.Cos(bAngle) * FindDistance(meObj.GetMainPointMiddle().pnt, pnt2)
            'Works
            Dim lLength As Single = meObj.GetMainPointMiddle().sngRadius - nSide

            Dim pSide As Single = Math.Tan(bAngle) * nSide

            Dim riseAB As Single = pnt1.Y - pnt2.Y

            Dim runAB As Single = pnt1.X - pnt2.X

            Dim scaleFactor As Single = pSide / FindDistance(pnt1, pnt2)

            Dim scaledRiseAB As Single = riseAB * scaleFactor

            Dim scaledRunAB As Single = runAB * scaleFactor

            meObj.SetMainPoint(New Point(meObj.GetDrawPoint().X + xMove, meObj.GetDrawPoint().Y + yMove))
        End If

        Refresh()
    End Sub

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

            ElseIf chrCurrent = chrChangePosition Then
                currentProcess = Process.ChangePos

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

            ElseIf currentProcess = Process.ChangePos Then  '0 is X, 1 is Y
                otherComObj(lstComputers.IndexOf(strPersonFrom)).SetMainPoint(New Point(lstSht(0), lstSht(1)))
                Refresh()
                shtInfo = String.Empty

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
        otherComObj.Add(New OverDropObject(New Point(0, 0), imgEnemyOne))  'Adds a new character to the screen
        lstComputers.Add(strName)
        lbxComputersConnectedTo.Items.Add(strName)
    End Sub

End Class
