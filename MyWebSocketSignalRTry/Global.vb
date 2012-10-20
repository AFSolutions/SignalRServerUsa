Imports SignalR.Hubs
Imports AFSSignalRServer.Global_asax
Imports System.Runtime.Serialization
REM ready version v2.8
<HubName("wsHub")>
Public Class wsHub
    Inherits Hub
    Implements IDisconnect


    Shared _clients As InMemoryRepository

    'Private Shared _connectionsList As New Concurrent.ConcurrentDictionary(Of String, Users)

    'Public Shared Property ConnectionsList As Concurrent.ConcurrentDictionary(Of String, Users)
    '    Get
    '        Return _connectionsList
    '    End Get
    '    Set(value As Concurrent.ConcurrentDictionary(Of String, Users))
    '        _connectionsList = value
    '    End Set
    'End Property


    Sub New()
        _clients = InMemoryRepository.GetInstance
    End Sub
  

    Private Sub MembersChangeing()
        'Dim fffg = MembersLoginLogoutClass.Instance
        'RemoveHandler fffg.MembersListChanged, AddressOf MembersChangeing
        Me.GetUsersHelper()
    End Sub


    Public Sub Login(ByVal Conid As String, ByVal name As String)
        'Try
        '    Dim ffg As MembersLoginLogoutClass = MembersLoginLogoutClass.Instance

        '    Dim nUser As New Users With {.ConnectionId = Conid, .Name = name}

        '    If ffg.AddUser(nUser) Then
        '        Clients.clientUserLoggedIn(nUser)
        '        Clients.clientGetUsers(ffg.GetAllUsers)
        '    Else
        '        Caller.clientIsConnected(False)
        '    End If
        'Catch ex As Exception
        '    Caller.clientOnErrorOccured(ex.Message.ToString)
        '    Caller.clientIsConnected(False)
        '    'Debug.WriteLine(ex.Message.ToString & vbCrLf & ex.Source.ToString & vbCrLf & ex.StackTrace.ToString)
        'End Try

        Dim NewUser As New Users With {.ConnectionId = Conid, .Name = name}

        'Dim nwUser = ConnectionsList.AddOrUpdate(Conid, NewUser, Function(key, oldvalue)
        '                                                             If key = oldvalue.ConnectionId Then
        '                                                                 Return oldvalue
        '                                                             Else
        '                                                                 Return NewUser
        '                                                             End If
        '                                                         End Function)
        'Dim bb = ConnectionsList.TryAdd(Conid, NewUser)
        'If bb Then
        '    Clients.clientUserLoggedIn(NewUser)
        '    Me.GetUsersHelper()
        'Else
        '    Caller.clientOnErrorOccured("not added to list...")
        'End If

        _clients.Add(NewUser)
        Clients.clientUserLoggedIn(NewUser)
        'Me.GetUsersHelper()
    End Sub

    Private Sub GetUsersHelper(Optional ByVal ee As Users = Nothing)
        'Dim ffg = MembersLoginLogoutClass.Instance
        'Debug.WriteLine("alluserslist: " & ffg.GetAllUsers.Count)
        Dim nlist As New List(Of Users)
        For Each a In _clients.Users
            nlist.Add(a)
        Next

        Clients.clientGetUsers(nlist)
    End Sub

    Public Sub GetUsers()
        Try
            Me.GetUsersHelper()
        Catch ex As Exception
            Caller.clientOnErrorOccured(ex.Message.ToString)
        End Try
    End Sub

    Public Sub ConnectedUsersCount()
        Try

            'Caller.clientConnectedUsersCount(ConnectionsList.Count)
        Catch ex As Exception
            Caller.clientOnErrorOccured(ex.Message.ToString)
        End Try
    End Sub

    Public Sub LogOut(ByVal name As String, ByVal connid As String)
        'Try

        '    Dim ffg As MembersLoginLogoutClass = MembersLoginLogoutClass.Instance
        '    Dim user = (New Users With {.Name = name, .ConnectionId = connid})
        '    ffg.removeUser(user)

        '    'For Each a In ffg.GetAllUsers
        '    '    If a.ConnectionId <> connid Then
        '    '        Clients(a.ConnectionId).clientUserLoggedOut(user)
        '    '    End If
        '    'Next
        '    Clients.clientUserLoggedOut(user)

        '    Clients.clientGetUsers(ffg.GetAllUsers)
        'Catch ex As Exception
        '    Caller.clientOnErrorOccured(ex.Message.ToString)
        'End Try


        Dim user = New Users With {.ConnectionId = connid, .Name = name}
        _clients.Remove(user)
        Clients.clientUserLoggedOut(user)
        Me.GetUsersHelper()
    End Sub


    Public Sub BroadCastMessage(ByVal conid As String, ByVal message As String)
        Clients.clientGotMessage(conid, message)
    End Sub

    Public Sub SendMessage(ByVal ToId As String, ByVal mesage As String)

        Clients(ToId).clientGotSeperateMessage((From a In _clients.Users
                                               Where a.ConnectionId = Context.ConnectionId
                                               Select a).FirstOrDefault, mesage)
    End Sub


    Public Sub UserWritesMessage(ByVal conid As String)
        Clients(conid).clientPartnerWritesMessage(Context.ConnectionId)
    End Sub




    'Public Sub StreamData(ByVal toid As String, ByVal filename As String, ByVal arrb As Byte())
    '    Clients(toid).clientGetStream(filename, arrb)
    'End Sub

    REM for filelenght < 2,5 megabytes
#Region "simple files getting"
    Public Sub SendFiletoUser(ByVal FileGetter As String, ByVal ServerFrom As String, ByVal filename As String, ByVal bytearr As Byte())
        Clients(FileGetter).clientGetStream(ServerFrom, filename, bytearr)
    End Sub


    'Public Sub SendFiletoUser(ByVal file As SimpleFileGetterType)
    '    Clients(file.FileGetter).clientGetStream(file.ServerFrom, file.Filename, file.ByteArr)
    'End Sub
#End Region

#Region "recieve getting files okay..."
    Public Sub AskForFileRecieve(ByVal server As String, ByVal SendTo As String, ByVal filename As String)
        'Clients(client).clientAskForFileRecieve(Context.ConnectionId, filename)
        Clients(SendTo).clientAskForFileRecieve(server, filename)
    End Sub

    Public Sub AskForMultipleFileRecieve(ByVal server As String, ByVal SendTo As String, ByVal listoffiles As List(Of String))
        Clients(SendTo).clientAskForMultipleFileRecieve(server, listoffiles)
    End Sub

    Public Sub YesNoFileRecieve(ByVal Answerer As String, ByVal ServerSendTo As String, ByVal filename As String, ByVal yesno As Boolean)
        Clients(ServerSendTo).clientYesNoFileRecieve(Answerer, filename, yesno)
    End Sub
#End Region

#Region "multiparted sending files"

    'Public Sub InitMultiPartedFileSending(ByVal toid As String, ByVal filename As String, ByVal numberofParts As String, ByVal FileFullLength As String)
    '    Clients(toid).clientInitMultiPartedFileSend(Context.ConnectionId, filename, numberofParts, FileFullLength)
    'End Sub

    'Public Sub SendMultiPartedSegment(ByVal toid As String, ByVal filename As String, ByVal partnb As String, ByVal segment As Byte())
    '    Clients(toid).clientMultiPartSegmentSent(Context.ConnectionId, filename, partnb, segment)
    'End Sub

    'Public Sub MultiPartedFileSegmentSending(ByVal toid As String, ByVal filename As String, ByVal partnumber As String)
    '    Clients(toid).clientSendSegments(Context.ConnectionId, filename, partnumber)
    'End Sub

    'Public Sub MultiPartedFileSegmentGotten(ByVal toid As String, ByVal filename As String, ByVal partnumber As String)
    '    Clients(toid).clientSendNextSegment(Context.ConnectionId, filename, partnumber)
    'End Sub

    'Public Sub MultiPartedFileSenT(ByVal toid As String, ByVal filename As String)
    '    Clients(toid).MultiPartedFileSentReady(filename)
    'End Sub

    Public Sub InitMultiPartedFileSending(ByVal Getter As String, ByVal Sender As String, ByVal filename As String, ByVal numberofParts As String, ByVal FileFullLength As String)
        Clients(Getter).clientInitMultiPartedFileSend(Sender, filename, numberofParts, FileFullLength)
    End Sub

    Public Sub SendMultiPartedSegment(ByVal Reciever As String, ByVal server As String, ByVal filename As String, ByVal partnb As String, ByVal segment As Byte())
        Clients(Reciever).clientMultiPartSegmentSent(server, Reciever, filename, partnb, segment)
    End Sub

    Public Sub MultiPartedFileSegmentSending(ByVal Server As String, ByVal Reciever As String, ByVal filename As String, ByVal partnumber As String)
        Clients(Server).clientSendSegments(Reciever, filename, partnumber)
    End Sub

    Public Sub MultiPartedFileSegmentGotten(ByVal toid As String, ByVal filename As String, ByVal partnumber As String)
        Clients(toid).clientSendNextSegment(Context.ConnectionId, filename, partnumber)
    End Sub

    Public Sub MultiPartedFileSenT(ByVal server As String, ByVal reciever As String, ByVal filename As String)
        Clients(reciever).MultiPartedFileSentReady(filename)
    End Sub
#End Region

    Public Function Disconnect() As Threading.Tasks.Task Implements IDisconnect.Disconnect
        'Dim fffg = MembersLoginLogoutClass.Instance
        'Dim x = From a In fffg.GetAllUsers
        '        Where a.ConnectionId = Context.ConnectionId
        '        Select a

        'If x.Count > 0 Then
        '    Dim gg = fffg.RemoveUsersOut(Context.ConnectionId)
        '    Return Clients.clientUserLoggedOut(x(0))
        'Else
        'End If
        Dim removedUser = (From b In _clients.Users
                          Where b.ConnectionId = Context.ConnectionId
                          Select b).FirstOrDefault

        If removedUser IsNot Nothing Then
            _clients.Remove(removedUser)
            Return Clients.clientUserLoggedOut(removedUser)
        End If
    End Function

    Private Sub MemErrOcc(errormess As String)
        Caller.clientOnErrorOccured(errormess)
    End Sub

End Class


<DataContract>
Public Class Users
    <DataMember>
    Public Property Name As String
    <DataMember>
    Public Property ConnectionId As String
End Class