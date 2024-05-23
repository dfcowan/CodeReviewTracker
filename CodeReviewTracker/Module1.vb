Imports CodeReviewTracker

Module Module1

    Private ReadOnly Property FileName As String
        Get
            Return "G:\My Drive\Dropbox\codereview.json"
        End Get
    End Property

    Sub Main()
        Dim reviewers As List(Of Reviewer)

        Try
            Dim json As String = IO.File.ReadAllText(FileName)
            reviewers = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Reviewer))(json)
        Catch ex As IO.FileNotFoundException
            reviewers = New List(Of Reviewer)
        End Try

        If My.Application.CommandLineArgs.ToList.Count = 0 Then
            SuggestReviewers(reviewers)
        ElseIf My.Application.CommandLineArgs.ToList.Count = 1 AndAlso My.Application.CommandLineArgs(0).ToLower = "dump" Then
            DumpReviewers(reviewers)
        ElseIf My.Application.CommandLineArgs.ToList.Count = 2 Then
            If (My.Application.CommandLineArgs(0).ToLower = "-r") Then
                RemoveReviewer(reviewers, My.Application.CommandLineArgs(1))
            Else
                RecordReview(reviewers, My.Application.CommandLineArgs.ToList)
            End If
        ElseIf My.Application.CommandLineArgs.ToList.Count = 3 Then
            RecordReview(reviewers, My.Application.CommandLineArgs.ToList)
        Else
            Console.WriteLine("For code review suggestions, please supply no arguments.")
            Console.WriteLine("To record code review, please supply two or three reviewer usernames")
            Return
        End If
    End Sub

    Private Sub DumpReviewers(reviewers As List(Of Reviewer))
        Dim sorted As List(Of Reviewer) = reviewers.OrderBy(Function(r) r.ReviewCount).OrderBy(Function(r) r.LastReview).ToList

        Dim i As Integer = 0
        While i < sorted.Count
            PrintReviewer(sorted(i))

            i += 1
        End While

        If sorted.Count = 0 Then
            Console.WriteLine("no reviewers found")
        End If
    End Sub

    Private Sub PrintReviewer(reviewer As Reviewer)
        Dim tabsCoveredByUserName As Integer = reviewer.UserName.Length \ 8
        Dim tabsNeeded As Integer = 3 - tabsCoveredByUserName

        Console.Write($"{reviewer.UserName}")
        For i As Integer = 1 To tabsNeeded
            Console.Write(vbTab)
        Next
        Console.WriteLine($"{reviewer.ReviewCount}{vbTab}{reviewer.LastReview.ToShortDateString}{vbTab}{Not reviewer.DeActivated}")
    End Sub

    Private Sub RecordReview(reviewers As List(Of Reviewer), reviewerUserNames As List(Of String))
        For Each userName In reviewerUserNames
            If Not VerifyUserName(reviewers, userName) Then
                Return
            End If
        Next

        For Each reviewer In reviewers
            If reviewerUserNames.Contains(reviewer.UserName) Then
                reviewer.RecordReview()
            End If
        Next

        SaveReviewers(reviewers)
    End Sub

    Private Sub RemoveReviewer(reviewers As List(Of Reviewer), toRemove As String)
        For Each reviewer In reviewers
            If reviewer.UserName = toRemove Then
                reviewer.DeActivate()
                SaveReviewers(reviewers)
                Return
            End If
        Next
    End Sub

    Private Sub SaveReviewers(reviewers As List(Of Reviewer))
        Dim json As String = Newtonsoft.Json.JsonConvert.SerializeObject(reviewers, Newtonsoft.Json.Formatting.Indented)
        IO.File.WriteAllText(FileName, json)
    End Sub

    Private Sub SuggestReviewers(reviewers As List(Of Reviewer))
        Dim sorted As List(Of Reviewer) = reviewers.Where(Function(r) Not r.DeActivated).OrderBy(Function(r) r.ReviewCount).OrderBy(Function(r) r.LastReview).ToList

        Dim i As Integer = 0
        While i < 5 AndAlso i < sorted.Count
            PrintReviewer(sorted(i))

            i += 1
        End While

        If sorted.Count = 0 Then
            Console.WriteLine("no reviewers found")
        End If
    End Sub

    Private Function VerifyUserName(reviewers As List(Of Reviewer), userName As String) As Boolean
        If reviewers.Select(Function(r) r.UserName).Contains(userName) Then
            Return True
        End If

        Console.WriteLine($"{userName} not found, would you like to add them? Y/N")
        Dim add As String = Console.ReadLine()

        If add.ToUpper = "Y" Then
            reviewers.Add(New Reviewer(userName))
            Return True
        Else
            Return False
        End If
    End Function

End Module
