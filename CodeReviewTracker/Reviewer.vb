<Serializable>
Public Class Reviewer
    Public ReadOnly Property UserName As String
    Public ReadOnly Property ReviewCount As Integer
    Public ReadOnly Property LastReview As Date
    Public ReadOnly Property DeActivated As Boolean

    <Newtonsoft.Json.JsonConstructor>
    Public Sub New(UserName As String, ReviewCount As Integer, LastReview As Date, DeActivated As Boolean)
        _UserName = UserName
        _ReviewCount = ReviewCount
        _LastReview = LastReview
        _DeActivated = DeActivated
    End Sub

    Public Sub New(un As String)
        UserName = un
        ReviewCount = 0
        LastReview = Date.MinValue
        DeActivated = False
    End Sub

    Public Sub RecordReview()
        _ReviewCount += 1
        _LastReview = Now.Date
    End Sub

    Friend Sub DeActivate()
        _DeActivated = True
    End Sub
End Class
