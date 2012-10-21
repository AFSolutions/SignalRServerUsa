Imports SignalR.Hubs
Imports AFSSignalRServer.Global_asax
Imports System.Runtime.Serialization
REM ready version v2.8
<HubName("wsHub")>
Public Class wsHub
    Inherits Hub
    Implements IDisconnect
#Region "internal"
    Shared _clients As InMemoryRepository

    Private Sub GetOnlineUsers(Optional ByVal ee As Users = Nothing)
        'Dim ffg = MembersLoginLogoutClass.Instance
        'Debug.WriteLine("alluserslist: " & ffg.GetAllUsers.Count)
        Dim nlist As New List(Of Users)
        For Each a In _clients.Users
            nlist.Add(a)
        Next

        Clients.clientGetUsers(nlist)
    End Sub

    Sub New()
        MyBase.New()
        _clients = InMemoryRepository.GetInstance
    End Sub

    Private Sub MemErrOcc(errormess As String)
        Caller.clientOnErrorOccured(errormess)
    End Sub

#End Region


#Region "external"


    Public Sub Login(ByVal Conid As String, ByVal name As String)
        Dim NewUser As New Users With {.ConnectionId = Conid, .Name = name}
        _clients.Add(NewUser)
        Clients.clientUserLoggedIn(NewUser)
        Me.GetOnlineUsers()
    End Sub

    Public Sub LogOut(ByVal name As String, ByVal connid As String)
        My.Computer.FileSystem.WriteAllText("debug.txt", "name: " & name & " / " & connid & " loggsout", True)

        Dim user = New Users With {.ConnectionId = connid, .Name = name}
        _clients.Remove(user)
        For Each a In _clients.Users
            If a.ConnectionId <> Context.ConnectionId Then
                Clients(a.ConnectionId).clientUserLoggedOut(user)
            End If
        Next
    End Sub

    Public Sub GetUsers()
        Try
            Me.GetOnlineUsers()
        Catch ex As Exception
            Caller.clientOnErrorOccured(ex.Message.ToString)
        End Try
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




    
#Region "simple files getting"
    Public Sub SendFiletoUser(ByVal FileGetter As String, ByVal ServerFrom As String, ByVal filename As String, ByVal bytearr As Byte())
        Clients(FileGetter).clientGetStream(ServerFrom, filename, bytearr)
    End Sub


    
#End Region

#Region "recieve getting files okay..."
    Public Sub AskForFileRecieve(ByVal server As String, ByVal SendTo As String, ByVal filename As String)
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
        Dim removedUser = (From b In _clients.Users
                          Where b.ConnectionId = Context.ConnectionId
                          Select b).FirstOrDefault



        If removedUser IsNot Nothing Then
            _clients.Remove(removedUser)
            For Each a In _clients.Users
                If a.ConnectionId <> Context.ConnectionId Then
                    Debug.WriteLine("IDisconnected raise for: " & a.Name)
                    Return Clients.clientUserLoggedOut(removedUser)
                End If
            Next
        Else
            Debug.WriteLine("error in disconnecting interface: " & Context.ConnectionId & " not in userlist")
            Return Nothing
        End If
    End Function

#End Region

End Class


<DataContract>
Public Class Users
    <DataMember>
    Public Property Name As String
    <DataMember>
    Public Property ConnectionId As String
End Class