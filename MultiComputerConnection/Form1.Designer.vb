<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
        Me.tmrListenerUpdate = New System.Windows.Forms.Timer(Me.components)
        Me.pbxPlayArea = New System.Windows.Forms.PictureBox()
        Me.tmrGameUpdate = New System.Windows.Forms.Timer(Me.components)
        Me.lblInfo = New System.Windows.Forms.Label()
        CType(Me.pbxPlayArea, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tmrListenerUpdate
        '
        Me.tmrListenerUpdate.Enabled = True
        Me.tmrListenerUpdate.Interval = 16
        '
        'pbxPlayArea
        '
        Me.pbxPlayArea.BackColor = System.Drawing.Color.Black
        Me.pbxPlayArea.Location = New System.Drawing.Point(12, 12)
        Me.pbxPlayArea.Name = "pbxPlayArea"
        Me.pbxPlayArea.Size = New System.Drawing.Size(1071, 964)
        Me.pbxPlayArea.TabIndex = 8
        Me.pbxPlayArea.TabStop = False
        '
        'tmrGameUpdate
        '
        Me.tmrGameUpdate.Enabled = True
        Me.tmrGameUpdate.Interval = 8
        '
        'lblInfo
        '
        Me.lblInfo.AutoSize = True
        Me.lblInfo.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblInfo.Location = New System.Drawing.Point(864, 12)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(39, 13)
        Me.lblInfo.TabIndex = 9
        Me.lblInfo.Text = "Label1"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(1094, 993)
        Me.Controls.Add(Me.lblInfo)
        Me.Controls.Add(Me.pbxPlayArea)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "Form1"
        Me.Text = "MultiComputerConnection"
        CType(Me.pbxPlayArea, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tmrListenerUpdate As System.Windows.Forms.Timer
    Friend WithEvents pbxPlayArea As System.Windows.Forms.PictureBox
    Friend WithEvents tmrGameUpdate As System.Windows.Forms.Timer
    Friend WithEvents lblInfo As Label
End Class
