﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.lbxChatConsole = New System.Windows.Forms.ListBox()
        Me.btnUpdateChatConsole = New System.Windows.Forms.Button()
        Me.tbxMessageToSend = New System.Windows.Forms.TextBox()
        Me.tbxConnectionComputerName = New System.Windows.Forms.TextBox()
        Me.lblInfo = New System.Windows.Forms.Label()
        Me.lbxComputersConnectedTo = New System.Windows.Forms.ListBox()
        Me.lblInfo2 = New System.Windows.Forms.Label()
        Me.tmrListenerUpdate = New System.Windows.Forms.Timer(Me.components)
        Me.btnConnect = New System.Windows.Forms.Button()
        Me.pbxPlayArea = New System.Windows.Forms.PictureBox()
        Me.tmrGameUpdate = New System.Windows.Forms.Timer(Me.components)
        CType(Me.pbxPlayArea, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lbxChatConsole
        '
        Me.lbxChatConsole.FormattingEnabled = True
        Me.lbxChatConsole.Location = New System.Drawing.Point(1171, 557)
        Me.lbxChatConsole.Name = "lbxChatConsole"
        Me.lbxChatConsole.Size = New System.Drawing.Size(176, 420)
        Me.lbxChatConsole.TabIndex = 0
        '
        'btnUpdateChatConsole
        '
        Me.btnUpdateChatConsole.BackColor = System.Drawing.Color.Bisque
        Me.btnUpdateChatConsole.FlatAppearance.BorderSize = 2
        Me.btnUpdateChatConsole.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnUpdateChatConsole.Location = New System.Drawing.Point(1171, 13)
        Me.btnUpdateChatConsole.Name = "btnUpdateChatConsole"
        Me.btnUpdateChatConsole.Size = New System.Drawing.Size(176, 40)
        Me.btnUpdateChatConsole.TabIndex = 1
        Me.btnUpdateChatConsole.Text = "Send Message"
        Me.btnUpdateChatConsole.UseVisualStyleBackColor = False
        '
        'tbxMessageToSend
        '
        Me.tbxMessageToSend.Location = New System.Drawing.Point(1171, 59)
        Me.tbxMessageToSend.Multiline = True
        Me.tbxMessageToSend.Name = "tbxMessageToSend"
        Me.tbxMessageToSend.Size = New System.Drawing.Size(176, 137)
        Me.tbxMessageToSend.TabIndex = 2
        '
        'tbxConnectionComputerName
        '
        Me.tbxConnectionComputerName.Location = New System.Drawing.Point(1171, 485)
        Me.tbxConnectionComputerName.Name = "tbxConnectionComputerName"
        Me.tbxConnectionComputerName.Size = New System.Drawing.Size(176, 20)
        Me.tbxConnectionComputerName.TabIndex = 3
        Me.tbxConnectionComputerName.Text = "127-R300-C002"
        '
        'lblInfo
        '
        Me.lblInfo.AutoSize = True
        Me.lblInfo.Location = New System.Drawing.Point(1168, 469)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(179, 13)
        Me.lblInfo.TabIndex = 4
        Me.lblInfo.Text = "Input computer name to conenct to: "
        '
        'lbxComputersConnectedTo
        '
        Me.lbxComputersConnectedTo.FormattingEnabled = True
        Me.lbxComputersConnectedTo.Location = New System.Drawing.Point(1171, 215)
        Me.lbxComputersConnectedTo.Name = "lbxComputersConnectedTo"
        Me.lbxComputersConnectedTo.Size = New System.Drawing.Size(176, 251)
        Me.lbxComputersConnectedTo.TabIndex = 5
        '
        'lblInfo2
        '
        Me.lblInfo2.AutoSize = True
        Me.lblInfo2.Location = New System.Drawing.Point(1171, 199)
        Me.lblInfo2.Name = "lblInfo2"
        Me.lblInfo2.Size = New System.Drawing.Size(159, 13)
        Me.lblInfo2.TabIndex = 6
        Me.lblInfo2.Text = "List of computers connected to: "
        '
        'tmrListenerUpdate
        '
        Me.tmrListenerUpdate.Enabled = True
        Me.tmrListenerUpdate.Interval = 16
        '
        'btnConnect
        '
        Me.btnConnect.BackColor = System.Drawing.Color.Bisque
        Me.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btnConnect.Location = New System.Drawing.Point(1171, 511)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(176, 40)
        Me.btnConnect.TabIndex = 7
        Me.btnConnect.Text = "Connect To Computer"
        Me.btnConnect.UseVisualStyleBackColor = False
        '
        'pbxPlayArea
        '
        Me.pbxPlayArea.BackColor = System.Drawing.Color.Black
        Me.pbxPlayArea.Location = New System.Drawing.Point(13, 13)
        Me.pbxPlayArea.Name = "pbxPlayArea"
        Me.pbxPlayArea.Size = New System.Drawing.Size(1149, 964)
        Me.pbxPlayArea.TabIndex = 8
        Me.pbxPlayArea.TabStop = False
        '
        'tmrGameUpdate
        '
        Me.tmrGameUpdate.Enabled = True
        Me.tmrGameUpdate.Interval = 4
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Linen
        Me.ClientSize = New System.Drawing.Size(1359, 993)
        Me.Controls.Add(Me.pbxPlayArea)
        Me.Controls.Add(Me.btnConnect)
        Me.Controls.Add(Me.lblInfo2)
        Me.Controls.Add(Me.lbxComputersConnectedTo)
        Me.Controls.Add(Me.lblInfo)
        Me.Controls.Add(Me.tbxConnectionComputerName)
        Me.Controls.Add(Me.tbxMessageToSend)
        Me.Controls.Add(Me.btnUpdateChatConsole)
        Me.Controls.Add(Me.lbxChatConsole)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "Form1"
        Me.Text = "MultiComputerConnection"
        CType(Me.pbxPlayArea, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lbxChatConsole As System.Windows.Forms.ListBox
    Friend WithEvents btnUpdateChatConsole As System.Windows.Forms.Button
    Friend WithEvents tbxMessageToSend As System.Windows.Forms.TextBox
    Friend WithEvents tbxConnectionComputerName As System.Windows.Forms.TextBox
    Friend WithEvents lblInfo As System.Windows.Forms.Label
    Friend WithEvents lbxComputersConnectedTo As System.Windows.Forms.ListBox
    Friend WithEvents lblInfo2 As System.Windows.Forms.Label
    Friend WithEvents tmrListenerUpdate As System.Windows.Forms.Timer
    Friend WithEvents btnConnect As System.Windows.Forms.Button
    Friend WithEvents pbxPlayArea As System.Windows.Forms.PictureBox
    Friend WithEvents tmrGameUpdate As System.Windows.Forms.Timer

End Class
