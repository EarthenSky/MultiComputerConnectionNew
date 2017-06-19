Public Class AI
    Inherits AnimationObject

    Public shtHealth As Short = 3
    Public blnIsDead As Boolean = False

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal radius As Short, ByVal shtAnimationInterval As Short)
        MyBase.New(pnt, img, radius, shtAnimationInterval)

    End Sub

    Sub Move()

    End Sub

    Public Sub HitPlayerSword()
        ChangeHealth(-1)
    End Sub

    Public Sub ChangeHealth(ByVal shtChangeNum As Short)
        shtHealth += shtChangeNum
        If shtHealth <= 0 Then
            blnIsDead = True
        End If
    End Sub
End Class
