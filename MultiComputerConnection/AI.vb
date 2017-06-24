Public Class AI
    Inherits AnimationObject

    Public shtHealth As Short = 1
    Public blnIsDead As Boolean = False
    Public WithEvents tmrPush As New Timer()

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal radius As Short, ByVal shtAnimationInterval As Short)
        MyBase.New(pnt, img, radius, shtAnimationInterval)
    End Sub

    Private pntMoveTo As Point
    Public Sub AIMove() 'TODO: AI code
        Dim shtDistance As Short = FindDistance(Form1.meObj.GetDrawPoint(0), lstPointPosition(0).pnt)

        If shtDistance > 700 Then 'Nest movement

        ElseIf shtDistance > 350 Then 'aimless movement

        Else 'targeted movement

        End If
    End Sub

    'Start Converted Internet
    Function sqr(ByVal x) As Double
        Return x * x
    End Function

    Function FindDistance(ByVal pnt1 As Point, ByVal pnt2 As Point) As Single
        Return Math.Sqrt(sqr(pnt2.X - pnt1.X) + sqr(pnt2.Y - pnt1.Y))
    End Function
    'End Converted Internet

    Public Sub HitPlayerSword()
        If shtHealth >= 0 Then
            ChangeHealth(-1)
        End If
    End Sub

    Public Sub ChangeHealth(ByVal shtChangeNum As Short)
        shtHealth += shtChangeNum
        If shtHealth <= 0 Then
            blnIsDead = True
            PlayAnimation(3)
        End If
    End Sub

    Public Sub PushBack(ByVal pntPushBack As Point)
        If blnIsDead = False Then
            tmrPush.Interval = 5
            tmrPush.Enabled = True

            Me.pntPushBack = pntPushBack
        End If
    End Sub

    Private Const shtLoops As Short = 2
    Private shtCurrentLoops As Short = 0
    Private pntPushBack As Point
    Public Sub tmrPushTick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrPush.Tick
        lstPointPosition(0) = New CircleBox(New Point(lstPointPosition(0).pnt.X + pntPushBack.X * -4, lstPointPosition(0).pnt.Y + pntPushBack.Y * -4), lstPointPosition(0).sngRadius)
        shtCurrentLoops += 1
        If shtCurrentLoops >= shtLoops Then
            shtCurrentLoops = 0
            tmrPush.Enabled = False
        End If
    End Sub 'I like how it pushes back more if you are going faster.

End Class
