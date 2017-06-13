Public Class AnimationObject
    Inherits OverDropObject

    Public lstAnimations As New List(Of Rectangle())
    Private WithEvents tmrAnimation As Timer

    Public pntCurrentImgIndexes As Point ' Y is animation number, X is animation pane.

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal radius As Short, ByVal shtAnimationInterval As Short)
        MyBase.New(pnt, img, radius)

        tmrAnimation = New Timer()
        tmrAnimation.Interval = shtAnimationInterval
        tmrAnimation.Enabled = True

        CutAnimationsFromSpriteSheet(4, 4, img.Width, img.Height, img)
    End Sub

    Private Sub CutAnimationsFromSpriteSheet(ByVal shtColumns As Short, ByVal shtRows As Short, ByVal shtWidth As Short, ByVal shtHeight As Short, ByVal mainImg As Image)
        For index As Short = 0 To shtRows - 1  'Amount of animations to create
            Dim aryRect(3) As Rectangle
            For jIndex As Short = 0 To shtColumns - 1 'Amount of panes in each animation
                aryRect(jIndex) = New Rectangle(jIndex * 256, index * 256, 256, 256)
            Next
            lstAnimations.Add(aryRect)
        Next
    End Sub

    Public Sub PlayAnimation(ByVal shtIndex As Short)
        pntCurrentImgIndexes = New Point(shtIndex, 0)
        'tmrAnimation.Enabled = True
    End Sub

    Public Sub StopAnimation()
        pntCurrentImgIndexes = New Point(-1, 0)
        'tmrAnimation.Enabled = False
    End Sub

    Private Sub tmrGameUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrAnimation.Tick ' Loops current animation
        If pntCurrentImgIndexes.Y >= 3 Then
            pntCurrentImgIndexes = New Point(pntCurrentImgIndexes.X, 0)
        Else
            pntCurrentImgIndexes = New Point(pntCurrentImgIndexes.X, pntCurrentImgIndexes.Y + 1)
        End If
    End Sub
End Class
