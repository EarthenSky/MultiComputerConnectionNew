Public Class MainCharacter
    Inherits AnimationObject

    Private Const shtImageSize As Short = 64
    Private Const shtHealthOffset As Short = 1
    Private Const shtMaxHealth As Short = 3
    Public shtHealth As Short = shtMaxHealth

    Public WithEvents tmrPush As New Timer()

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal radius As Short, ByVal shtAnimationInterval As Short)
        MyBase.New(pnt, img, radius, shtAnimationInterval)
    End Sub

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
            Application.Exit() 'Game dies if you die.
        End If
    End Sub

    Public Sub HitAI(ByVal pntPushBack As Point) 'You hit the ai with body
        If shtCurrentLoops = 0 Then
            ChangeHealth(-1)
            tmrPush.Interval = 5
            tmrPush.Enabled = True

            Me.pntPushBack = pntPushBack
        End If
    End Sub

    Private Const shtLoops As Short = 2
    Private shtCurrentLoops As Short = 0
    Private pntPushBack As Point
    Public Sub tmrPushTick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrPush.Tick
        lstPointPosition(0) = New CircleBox(New Point(lstPointPosition(0).pnt.X + pntPushBack.X * 8, lstPointPosition(0).pnt.Y + pntPushBack.Y * 8), lstPointPosition(0).sngRadius)
        shtCurrentLoops += 1
        If shtCurrentLoops >= shtLoops Then
            shtCurrentLoops = 0
            tmrPush.Enabled = False
        End If
    End Sub 'I like how it pushes back more if you are going faster.
End Class
