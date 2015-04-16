Imports System.IO

Public Class Worker

    Inherits System.ComponentModel.Component

    ' Declares the variables you will use to hold your thread objects.

    Public WorkerThread As System.Threading.Thread


    Public filefolder As String = ""
    Public prefix As String = ""
    Public suffix As String = ""

    
    Public result As String = ""

    Public Event WorkerComplete(ByVal Result As String)
    Public Event WorkerProgress(ByVal filesrenamed As Long, ByVal currentfilename As String, ByVal totalfiles As Long)



#Region " Component Designer generated code "

    Public Sub New(ByVal Container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        Container.Add(Me)
    End Sub

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Component overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container
    End Sub

#End Region

    Private Sub Error_Handler(ByVal exc As Exception)
        Try
            If exc.Message.IndexOf("Thread was being aborted") < 0 Then
                Dim Display_Message1 As New Display_Message("The Worker Thread encountered the following problem: " & vbCrLf & exc.ToString)
                Display_Message1.ShowDialog()
                Dim dir As DirectoryInfo = New DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
                If dir.Exists = False Then
                    dir.Create()
                End If
                Dim filewriter As StreamWriter = New StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
                filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & exc.ToString)
                filewriter.Flush()
                filewriter.Close()
            End If
        Catch ex As Exception
            MsgBox("An error occurred in Prefix Suffix Mass File Renamer's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

   

    Public Sub ChooseThreads(ByVal threadNumber As Integer)
        Try
            ' Determines which thread to start based on the value it receives.
            Select Case threadNumber
                Case 1
                    ' Sets the thread using the AddressOf the subroutine where
                    ' the thread will start.
                    WorkerThread = New System.Threading.Thread(AddressOf WorkerExecute)
                    ' Starts the thread.
                    WorkerThread.Start()

            End Select
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Public Sub WorkerExecute()
        Try
            Dim dinfo As DirectoryInfo = New DirectoryInfo(filefolder)
            
            Dim check As FileInfo
            Dim finfo As FileInfo
            Dim runner As Long = 0
            Dim oldname As String = ""
            Dim newname As String = ""
            For Each finfo In dinfo.GetFiles
                Try
                    runner = runner + 1

                    newname = prefix & finfo.Name.Replace(finfo.Extension, "") & suffix
                    
                    newname = newname & finfo.Extension
                    oldname = finfo.Name
                    check = New FileInfo((dinfo.FullName & "\" & newname).Replace("\\", "\"))
                    If check.Exists = False Then
                        finfo.MoveTo(check.FullName)
                    End If
                Catch ex As Exception
                    Error_Handler(ex)
                Finally
                    RaiseEvent WorkerProgress(runner, oldname & " -> " & finfo.Name, dinfo.GetFiles.Length)
                End Try
            Next
            result = "Success"
            RaiseEvent WorkerComplete(result)
        Catch ex As Exception
            result = "Failure"
            RaiseEvent WorkerComplete(result)
        End Try

        WorkerThread.Abort()
    End Sub







End Class
