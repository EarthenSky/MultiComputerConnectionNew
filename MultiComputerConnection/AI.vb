Public Class AI
    Inherits AnimationObject

    Private shtMoveSpeed As Short = 2

    Public shtHealth As Short = 1
    Public blnIsDead As Boolean = False
    Public WithEvents tmrPush As New Timer()

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal radius As Short, ByVal shtAnimationInterval As Short)
        MyBase.New(pnt, img, radius, shtAnimationInterval)
    End Sub

    Private pntMoveTo As Point = New Point(-1, -1) 'AI Position can never become negitave.  (-1, -1) is pretty much null
    'Very simple AI, runs into the walls, feature not bug cause the AI are supposed to be easy.  
    'When you get close to the AI it gets faster and more precise.
    Public Sub AIMove()
        If blnIsDead = False Then
            Dim pntMyPos As Point = lstPointPosition(0).pnt
            Dim shtDistance As Short = FindDistance(Form1.meObj.GetDrawPoint(0), lstPointPosition(0).pnt)

            If shtDistance > 300 Then 'No next movement pos
                If shtMoveSpeed <> 2 Then
                    shtMoveSpeed = 2
                End If

                If GeneralPointComparison(pntMyPos, pntMoveTo) Then 'Point needs to be set
                    pntMoveTo = New Point(-1, -1) 'Set point to 'null'

                ElseIf pntMoveTo <> New Point(-1, -1) Then 'Move to point
                    Dim pntMove As Point = GetMoveAmountScaled(pntMoveTo, pntMyPos)
                    lstPointPosition(0) = New CircleBox(New Point(pntMyPos.X + pntMove.X, pntMyPos.Y + pntMove.Y), lstPointPosition(0).sngRadius)

                End If

                'Debug.Print("1" & pntMoveTo.ToString())
            ElseIf shtDistance > 200 Then 'Aimless movement
                If shtMoveSpeed <> 2 Then
                    shtMoveSpeed = 2
                End If

                If GeneralPointComparison(pntMyPos, pntMoveTo) OrElse pntMoveTo = New Point(-1, -1) Then 'Point needs to be set
                    pntMoveTo = New Point(Form1.meObj.GetDrawPoint(0).X + 32, Form1.meObj.GetDrawPoint(0).Y + 32)  'Set point
                    Dim pntMove As Point = GetMoveAmountScaled(pntMoveTo, pntMyPos)
                    lstPointPosition(0) = New CircleBox(New Point(pntMyPos.X + pntMove.X, pntMyPos.Y + pntMove.Y), lstPointPosition(0).sngRadius)

                Else 'Move to point
                    Dim pntMove As Point = GetMoveAmountScaled(pntMoveTo, pntMyPos)
                    lstPointPosition(0) = New CircleBox(New Point(pntMyPos.X + pntMove.X, pntMyPos.Y + pntMove.Y), lstPointPosition(0).sngRadius)

                End If

                'Debug.Print("2" & pntMoveTo.ToString())
            Else 'targeted movement
                If shtMoveSpeed <> 4 Then
                    shtMoveSpeed = 4
                End If

                pntMoveTo = New Point(Form1.meObj.GetDrawPoint(0).X + 32, Form1.meObj.GetDrawPoint(0).Y + 32) 'Set point (the +32 is for the centre)

                Dim pntMove As Point = GetMoveAmountScaled(pntMoveTo, pntMyPos)
                lstPointPosition(0) = New CircleBox(New Point(pntMyPos.X + pntMove.X, pntMyPos.Y + pntMove.Y), lstPointPosition(0).sngRadius)

                'Debug.Print("3" & pntMoveTo.ToString())
            End If
        End If
    End Sub

    Private Function GeneralPointComparison(ByVal pnt1 As Point, ByVal pnt2 As Point) As Boolean
        If Math.Round(pnt1.X / 10) = Math.Round(pnt2.X / 10) And Math.Round(pnt1.Y / 10) = Math.Round(pnt2.Y / 10) Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetMoveAmountScaled(ByVal pntAway As Point, ByVal pntCentre As Point) As Point
        Dim rise As Double = pntAway.Y - pntCentre.Y - 32
        Dim run As Double = pntAway.X - pntCentre.X - 32

        Dim dis As Double = FindDistance(pntAway, pntCentre)
        Dim scale As Double = shtMoveSpeed / dis

        rise *= scale
        run *= scale

        Dim num = Math.Abs(rise) + Math.Abs(run)
        If num <> 0 Then
            Dim scale2 As Double = shtMoveSpeed / num
            rise *= scale2
            run *= scale2
        End If

        Return New Point(run, rise)
    End Function

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
