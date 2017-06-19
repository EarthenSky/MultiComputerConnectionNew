Public Class MainCharacter
    Inherits AnimationObject

    Private Const shtImageSize As Short = 64
    Private Const shtHealthOffset As Short = 1
    Private Const shtMaxHealth As Short = 3
    Public shtHealth As Short = shtMaxHealth

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal radius As Short, ByVal shtAnimationInterval As Short)
        MyBase.New(pnt, img, radius, shtAnimationInterval)
    End Sub

    Public Sub Update()  'TODO: this is debug.

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

    Public Sub HitAI() 'You hit the ai with body
        ChangeHealth(-1)
        'TODO: Push both back.
    End Sub
End Class
