Imports System.Web.SessionState

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Wird beim Start der Anwendung ausgelöst

        
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Wird beim Start der Sitzung ausgelöst
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Wird zu Beginn jeder Anforderung ausgelöst
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Wird beim Versuch ausgelöst, den Benutzer zu authentifizieren
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Wird bei einem Fehler ausgelöst
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Wird am Ende der Sitzung ausgelöst
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Wird am Ende der Anwendung ausgelöst
    End Sub

End Class