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


    Private listener As New TcpListener(5019)
    Private client As New TcpClient

    Private lstComputers As New List(Of String)

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim ListenerThread As New Thread(New ThreadStart(AddressOf Listening))
        tbxConnectionComputerName.Text = My.Computer.Name
        ListenerThread.Start()
    End Sub

    Private Sub Listening()  'starts listening for info from other com.
        listener.Start()
    End Sub

    'Get info from other computers here.
    Private Sub tmrListenerUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrListenerUpdate.Tick
        Try
            If listener.Pending = True Then  'If a Computer wants to send something.
                client = listener.AcceptTcpClient() 'Accepts the "message".

                ProcessInformation(client)

            End If
        Catch ex As Exception
            Console.WriteLine(ex)
            Dim Errorresult As String = ex.Message
            MessageBox.Show(Errorresult & vbCrLf & vbCrLf & "???", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ProcessInformation(ByRef client As System.Net.Sockets.TcpClient)
        Dim shtInfo As String = ""

        Dim Reader As New StreamReader(client.GetStream())  'Start getting the other com info.

        Dim currentProcess As Process = Process.None 'Current thing to do with the recieved information.

        While Reader.Peek > -1  'Changes each character sent into a string and adds it to the 
            Dim chrCurrent As Char = Convert.ToChar(Reader.Read())

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

            End If

            If currentProcess = Process.StringStart Then 'Adds chars to be proccessed
                shtInfo += chrCurrent

            ElseIf currentProcess = Process.AddComEnd Then  'this is inefficient cause all coms have to give anmes and it repeats.
                If lstComputers.Contains(shtInfo) = False Then
                    lstComputers.Add(shtInfo)
                    lbxComputersConnectedTo.Items.Add(shtInfo)

                    Dim Writer As New StreamWriter(client.GetStream())
                    GiveComNamesToFriends(Writer, shtInfo)

                End If
                shtInfo = String.Empty

            ElseIf currentProcess = Process.GiveComs Then 'Other computer tells it's name, this computer gives any other names to that com.
                If lstComputers.Count <> 0 Then 'If lstComputers has items in it, send them
                    client = New TcpClient(shtInfo, 5019)
                    Dim Writer As New StreamWriter(client.GetStream())

                    For index As Short = 0 To lstComputers.Count - 1
                        Writer.Write(chrStartProcessingText & lstComputers(index).ToString() & chrAddComToConnectListEnd)
                        Writer.Flush()
                    Next

                End If
                'then adds
                lstComputers.Add(shtInfo)
                lbxComputersConnectedTo.Items.Add(shtInfo)
                shtInfo = String.Empty

            ElseIf currentProcess = Process.MessageEnd Then
                lbxChatConsole.Items.Add(shtInfo.ToString())
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

            tbxMessageToSend.Text = "Sent!"
        Catch ex As Exception
            Console.WriteLine(ex)
            Dim Errorresult As String = ex.Message
            MessageBox.Show(Errorresult & vbCrLf & vbCrLf & "Please Review Client Address", "Error Sending Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

        client = New TcpClient(tbxConnectionComputerName.Text, 5019)

        'Send my name and ask for it's computers
        Dim Writer As New StreamWriter(client.GetStream())
        Writer.Write(chrStartProcessingText & My.Computer.Name & chrGiveComs)
        Writer.Flush()

        GiveComNamesToFriends(Writer, tbxConnectionComputerName.Text)

        'send it my computers.
        For index As Short = 0 To lstComputers.Count - 1
            If lstComputers(index).ToString = My.Computer.Name Then
                Continue For  'Don't include my name
            End If
            Writer.Write(chrStartProcessingText & lstComputers(index).ToString & chrAddComToConnectListEnd)
            Writer.Flush()
        Next

        lstComputers.Add(tbxConnectionComputerName.Text)
        lbxComputersConnectedTo.Items.Add(tbxConnectionComputerName.Text)
    End Sub

    Private Sub GiveComNamesToFriends(ByRef Writer As StreamWriter, ByVal name As String)
        'send my computers the name
        For index As Short = 0 To lstComputers.Count - 1
            If lstComputers(index).ToString = My.Computer.Name Then
                Continue For  'Don't send it to my name
            End If

            client = New TcpClient(lstComputers(index).ToString, 5019)
            Writer.Write(chrStartProcessingText & name & chrAddComToConnectListEnd)
            Writer.Flush()
        Next
    End Sub

End Class
