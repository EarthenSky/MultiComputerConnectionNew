Public Class MainCharacter
    Inherits AnimationObject

    Private Const shtImageSize As Short = 64
    Private Const shtHealthOffset As Short = 1
    Private Const shtMaxHealth As Short = 3
    Private Const shtSwordDistanceForward As Short = 16
    Public shtHealth As Short = shtMaxHealth

    Public WithEvents tmrPush As New Timer()
    Public WithEvents tmrInvulnerability As New Timer()

    Public blnInvulnerable As Boolean = False

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal radius As Short, ByVal shtAnimationInterval As Short)
        MyBase.New(pnt, img, radius, shtAnimationInterval)
        tmrInvulnerability.Interval = 150
        tmrPush.Interval = 5
    End Sub

    Public Sub SetSecondPointRotation(ByVal shtRotation As Double)
        Dim pntTemp As Point = GetRotatedPoint(New Point(lstPointPosition(0).pnt.X + shtSwordDistanceForward + 32, lstPointPosition(0).pnt.Y + 32), New Point(lstPointPosition(0).pnt.X + 32, lstPointPosition(0).pnt.Y + 32), shtRotation)
        'Debug.Print("pntrot, " & pntTemp.ToString())
        'Debug.Print("pntstart, " & lstPointPosition(0).pnt.ToString())
        lstPointPosition(1) = New CircleBox(New Point(pntTemp.X - 32, pntTemp.Y - 32), lstPointPosition(1).sngRadius)
    End Sub

    'Start Converted Internet
    Private Function GetRotatedPoint(ByVal pnt As Point, ByVal pntRotation As Point, ByVal shtRotationAngle As Double) As Point 'Gets a point rotated around another point.
        Dim x1 As Double = pnt.X - pntRotation.X
        Dim y1 As Double = pnt.Y - pntRotation.Y

        Dim x2 As Double = x1 * Math.Cos(shtRotationAngle) - y1 * Math.Sin(shtRotationAngle)
        Dim y2 As Double = x1 * Math.Sin(shtRotationAngle) + y1 * Math.Cos(shtRotationAngle)

        Dim x3 As Double = x2 + pntRotation.X
        Dim y3 As Double = y2 + pntRotation.Y

        Return New Point(x3, y3)
    End Function
    'End Converted Internet

    Public Sub DrawHealth(ByVal e As PaintEventArgs, ByVal imgGoodHealth As Image, ByVal imgBadHealth As Image)
        Dim rectTempDrawPoint As Rectangle = New Rectangle(shtHealthOffset * 4, shtHealthOffset * 4, shtImageSize, shtImageSize)
        For index As Short = 0 To shtHealth - 1
            e.Graphics.DrawImage(imgGoodHealth, rectTempDrawPoint)
            rectTempDrawPoint = New Rectangle(rectTempDrawPoint.X + shtHealthOffset + shtImageSize, rectTempDrawPoint.Y, shtImageSize, shtImageSize)
        Next
        For index As Short = shtHealth To shtMaxHealth - 1
            e.Graphics.DrawImage(imgBadHealth, rectTempDrawPoint)
            rectTempDrawPoint = New Rectangle(rectTempDrawPoint.X + shtHealthOffset + shtImageSize, rectTempDrawPoint.Y, shtImageSize, shtImageSize)
        Next
    End Sub

    Public Sub ChangeHealth(ByVal shtChangeNum As Short)
        shtHealth += shtChangeNum
        If shtHealth <= 0 Then
            EndGaem()
        End If
    End Sub

    Public Sub EndGaem()
        Form1.tmrGameUpdate.Enabled = False
    End Sub

    Public Sub HitAI(ByVal pntPushBack As Point) 'You hit the ai with body
        If blnInvulnerable = False Then 'Cannot take damage if invulnerable
            ChangeHealth(-1)
        End If

        tmrPush.Enabled = True
        Me.pntPushBack = pntPushBack
    End Sub

    Private Const shtLoops As Short = 2
    Private shtCurrentLoops As Short = 0
    Private pntPushBack As Point
    Public Sub tmrPushTick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrPush.Tick
        lstPointPosition(0) = New CircleBox(New Point(lstPointPosition(0).pnt.X + pntPushBack.X * 4, lstPointPosition(0).pnt.Y + pntPushBack.Y * 4), lstPointPosition(0).sngRadius)
        shtCurrentLoops += 1
        If shtCurrentLoops >= shtLoops Then
            shtCurrentLoops = 0
            tmrPush.Enabled = False
        End If
    End Sub 'I like how it pushes back more if you are going faster.

    Public Sub InvulnerablityActivate()
        tmrInvulnerability.Enabled = True
        blnInvulnerable = True
    End Sub

    Public Sub tmrInvulnerabilityTick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrInvulnerability.Tick
        blnInvulnerable = False
        tmrInvulnerability.Enabled = False
    End Sub
End Class
