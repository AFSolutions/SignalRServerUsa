Public Delegate Sub MemebersListChangedEventHandler()

Public Class MembersLoginLogoutClass
    Private Shared FInstance As MembersLoginLogoutClass = Nothing




    Public Shared Event MembersListChanged As MemebersListChangedEventHandler
    Public Shared Event ErrorOccured(ByVal errormess As String)

    Private Shared _connectionsList As New Concurrent.ConcurrentDictionary(Of String, Users)

    Public Shared Property ConnectionsList As Concurrent.ConcurrentDictionary(Of String, Users)
        Get
            Return _connectionsList
        End Get
        Set(value As Concurrent.ConcurrentDictionary(Of String, Users))
            _connectionsList = value
        End Set
    End Property


    Protected Sub New()


    End Sub

    Public Shared Function Instance() As MembersLoginLogoutClass

        If (FInstance Is Nothing) Then
            FInstance = New MembersLoginLogoutClass
        End If

        Return FInstance
    End Function

    Public Function AddUser(ByVal nuser As Users) As Boolean
        Try
            'ConnectionsList.AddOrUpdate(nuser.ConnectionId, nuser.Name, Function(key, oldvalue)
            '                                                                If nuser.ConnectionId = key Then
            '                                                                    Return oldvalue
            '                                                                Else
            '                                                                    Return Nothing
            '                                                                End If
            '                                                            End Function)

            Dim nwUser = ConnectionsList.AddOrUpdate(nuser.ConnectionId, nuser, Function(key, oldvalue)
                                                                                    If key = oldvalue.ConnectionId Then
                                                                                        Return oldvalue
                                                                                    Else
                                                                                        Return nuser
                                                                                    End If
                                                                                End Function)

            Return True
        Catch ex As Exception
            RaiseEvent ErrorOccured(ex.Message.ToString & vbCrLf & ex.Source.ToString)
            Return False
        End Try

    End Function
    Public Function removeUser(ByVal nuser As Users) As Boolean
        If ConnectionsList.TryRemove(nuser.ConnectionId, nuser) Then
            RaiseEvent MembersListChanged()
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetAllUsers() As List(Of Users)
        Dim retlist As New List(Of Users)
        For Each a In ConnectionsList
            retlist.Add(New Users With {.ConnectionId = a.Key, .Name = CType(a.Value, Users).Name})
        Next
        Return retlist
    End Function

    Public Function RemoveUsersOut(ByVal conid As String) As Boolean
        Dim Name As New Users
        If ConnectionsList.TryRemove(conid, Name) Then
            RaiseEvent MembersListChanged()
            Return True
        Else
            Return False
        End If
    End Function

End Class
