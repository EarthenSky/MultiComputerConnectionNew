Public Class AnimationObject
    Public lstPointPosition As New List(Of CircleBox) 'Holds all of the circles.

    Public lstAnimations As New List(Of Rectangle())
    Private WithEvents tmrAnimation As Timer

    Public imgMainImage As Image

    Public pntCurrentImgIndexes As Point ' Y is animation number, X is animation pane.

    Sub New(ByVal pnt As Point, ByVal img As Image, ByVal shtAnimationInterval As Short)
        lstPointPosition.Add(New CircleBox(pnt, 5)) 'TODO: this presets radius to 5

        imgMainImage = img

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

    Public Function GetMainPoint() As CircleBox
        If lstPointPosition.Count = 1 Then
            Return lstPointPosition(0)
        ElseIf lstPointPosition.Count > 1 Then
            Return lstPointPosition(0) 'TODO: NOT DONE YET, THIS SHOULD OUTPUT THE MAIN POINT (THAT THE OBJECT TURNS AROUND?)
        End If
    End Function

    Public Sub SetMainPoint(ByVal pnt As Point)
        lstPointPosition(0) = New CircleBox(pnt, lstPointPosition(0).sngRadius)
    End Sub
End Class
