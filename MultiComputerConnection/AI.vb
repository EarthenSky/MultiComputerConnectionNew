Public Class AI
    Inherits AnimationObject
    'The idle animation is bad cause time restraints.

    Private shtMoveSpeed As Short = 2

    Private shtHealth As Short = 1
    Public blnIsDead As Boolean = False
    Private WithEvents tmrPush As New Timer()

    Public shtLookAngle As Short

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal radius As Short, ByVal shtAnimationInterval As Short)
        MyBase.New(pnt, img, radius, shtAnimationInterval)
        PlayAnimation(1)  'AI starts with idle animation.
    End Sub

    Private pntMoveTo As Point = New Point(-1, -1) 'AI Position can never become negitave.  (-1, -1) is pretty much null
    'Very simple AI, runs into the walls.  This is a feature not a bug cause the AI are supposed to be stupid.  
    'When you get close to the AI it gets faster and more precise.
    Public Sub AIMove()
        If blnIsDead = False Then  'Makes sure the AI is alive
            Dim pntMyPos As Point = lstPointPosition(0).pnt
            Dim shtDistance As Short = Form1.FindDistance(Form1.meObj.GetDrawPoint(0), lstPointPosition(0).pnt)

            If shtDistance > 300 Then 'The AI goes to it's next position slowly then stops.
                If shtMoveSpeed <> 1 Then
                    shtMoveSpeed = 1
                End If

                If GeneralPointComparison(pntMyPos, pntMoveTo) Then  'Got to the next position.
                    pntMoveTo = New Point(-1, -1) 'Set next point to "null."

                    If pntCurrentImgIndexes.X <> 1 Then
                        PlayAnimation(1)
                    End If

                ElseIf pntMoveTo <> New Point(-1, -1) Then  'Not at the next position, move to it.
                    Dim pntMove As Point = GetMoveAmountScaled(pntMoveTo, pntMyPos)
                    lstPointPosition(0) = New CircleBox(New Point(pntMyPos.X + pntMove.X, pntMyPos.Y + pntMove.Y), lstPointPosition(0).sngRadius)

                    If pntCurrentImgIndexes.X <> 0 Then
                        PlayAnimation(0)
                    End If

                End If

            ElseIf shtDistance > 200 Then 'The AI goes to it's next position then sets a new next position, loop.
                If pntCurrentImgIndexes.X <> 0 Then
                    PlayAnimation(0)
                End If

                If shtMoveSpeed <> 2 Then
                    shtMoveSpeed = 2
                End If

                If GeneralPointComparison(pntMyPos, pntMoveTo) OrElse pntMoveTo = New Point(-1, -1) Then  'Next position needs to be set.
                    pntMoveTo = New Point(Form1.meObj.GetDrawPoint(0).X + 32, Form1.meObj.GetDrawPoint(0).Y + 32)  'Set next position.

                    'Move to next position.
                    Dim pntMove As Point = GetMoveAmountScaled(pntMoveTo, pntMyPos)
                    lstPointPosition(0) = New CircleBox(New Point(pntMyPos.X + pntMove.X, pntMyPos.Y + pntMove.Y), lstPointPosition(0).sngRadius)

                Else  'Move to next position.
                    Dim pntMove As Point = GetMoveAmountScaled(pntMoveTo, pntMyPos)
                    lstPointPosition(0) = New CircleBox(New Point(pntMyPos.X + pntMove.X, pntMyPos.Y + pntMove.Y), lstPointPosition(0).sngRadius)

                End If

            Else 'The AI quickly moves directly towards the player.
                If pntCurrentImgIndexes.X <> 0 Then
                    PlayAnimation(0)
                End If

                If shtMoveSpeed <> 6 Then
                    shtMoveSpeed = 6
                End If

                pntMoveTo = New Point(Form1.meObj.GetDrawPoint(0).X + 32, Form1.meObj.GetDrawPoint(0).Y + 32)  'Set next position. (The +32 is to make point the center.)

                'Move to next position.
                Dim pntMove As Point = GetMoveAmountScaled(pntMoveTo, pntMyPos)
                lstPointPosition(0) = New CircleBox(New Point(pntMyPos.X + pntMove.X, pntMyPos.Y + pntMove.Y), lstPointPosition(0).sngRadius)

            End If
        End If
    End Sub

    Public Sub SetLookAngle()
        Dim angle = Form1.FindAngle(pntMoveTo, New Point(lstPointPosition(0).pnt.X + 32, lstPointPosition(0).pnt.Y + 32), New Point(0, Short.MinValue), New Point(0, Short.MaxValue)) * 57.2958
        shtLookAngle = angle
    End Sub

    Private Const shtDiffNum As Short = 50
    Private Function GeneralPointComparison(ByVal pnt1 As Point, ByVal pnt2 As Point) As Boolean  'Checks if two points are roughly similar.
        Return IIf(Math.Abs(pnt1.X - pnt2.X) < shtDiffNum AndAlso Math.Abs(pnt1.Y - pnt2.Y) < shtDiffNum, True, False)
    End Function

    Private Function GetMoveAmountScaled(ByVal pntAway As Point, ByVal pntCentre As Point) As Point  'Finds how much the AI needs to move.
        Dim dblRise As Double = pntAway.Y - pntCentre.Y - 32
        Dim dblRun As Double = pntAway.X - pntCentre.X - 32

        Dim dblDistance As Double = Form1.FindDistance(pntAway, pntCentre)
        Dim dblScale As Double = shtMoveSpeed / dblDistance

        dblRise *= dblScale
        dblRun *= dblScale

        Dim dblNum As Double = Math.Abs(dblRise) + Math.Abs(dblRun)
        If dblNum <> 0 Then
            Dim dblscale2 As Double = shtMoveSpeed / dblNum

            dblRise *= dblscale2
            dblRun *= dblscale2
        End If

        Return New Point(dblRun, dblRise)
    End Function

    Public Sub HitPlayerSword()  'Called when this is hit by the player's sword.
        If shtHealth >= 0 Then
            ChangeHealth(-1)
        End If
    End Sub

    Public Sub ChangeHealth(ByVal shtChangeNum As Short)
        shtHealth += shtChangeNum
        If shtHealth <= 0 Then
            blnIsDead = True
            PlayAnimation(3)  'Dead animation.
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
    Public Sub tmrPushTick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrPush.Tick  'This pushes back the AI when hit.
        lstPointPosition(0) = New CircleBox(New Point(lstPointPosition(0).pnt.X + pntPushBack.X * -4, lstPointPosition(0).pnt.Y + pntPushBack.Y * -4), lstPointPosition(0).sngRadius)
        shtCurrentLoops += 1
        If shtCurrentLoops >= shtLoops Then
            shtCurrentLoops = 0
            tmrPush.Enabled = False
        End If
    End Sub

End Class
