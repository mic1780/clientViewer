Imports System.Net.Sockets
Public Class mainPanel
    Dim msg As String
    Dim lockObject As New Object()
    Dim statusLock As New Object()
    Dim socketThread As Threading.Thread
    Dim socket As Socket

    Private Sub mainPanel_load(sender As Object, e As EventArgs) Handles MyBase.Load
    End Sub

    Private Function Connect(ByVal host As String) As Socket
        Dim dataReceived(1024) As [Byte]
        Dim randVal As Integer
        Dim octString As String = ""
        Dim request As String = ""

        socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        socket.SendTimeout = 1000

        Try
            socket.Connect(host, 1780)
        Catch
            SyncLock (lockObject)
                bottomStatusText.Text = "Failed to connect to host at: " & host
            End SyncLock
            Console.WriteLine("ERROR: Failed to connect to host. Exiting Connect function...")
            Return Nothing
            Exit Function
        End Try

        If socket.Connected = False Then
            Return Nothing
        End If
        Dim stream As New NetworkStream(socket)
        Dim reader As New IO.StreamReader(stream)
        Dim writer As New IO.StreamWriter(stream)

        For i As Int16 = 1 To 16 Step 1
            Randomize()
            randVal = CInt(Math.Floor((256 * Rnd()) + 1))
            octString &= Oct(randVal)
        Next

        request &= "GET ws://" & host & ":1780/" & vbCrLf & _
                   "Connection: Upgrade" & vbCrLf & _
                   "Upgrade: websocket" & vbCrLf & _
                   "Sec-WebSocket-Key: " & getSHA1Hash(octString) & vbCrLf & _
                   "Sec-WebSocket-Version: 13" & vbCrLf & vbCrLf

        writer.Write(request)
        writer.Flush()
        While (Not (reader.Peek() = Nothing Or reader.Peek() = -1))
            'Console.WriteLine("'" & reader.Peek() & "'")
            Console.WriteLine(reader.ReadLine())
        End While
        writer.Close()
        reader.Close()
        stream.Close()

        connectionStatus.Text = "Connected"

        msg = "set name monitor"
        SendMessage2(socket, msg, msg.Length)

        msg = "set monitor 1"
        SendMessage2(socket, msg, msg.Length)

        Return socket

    End Function

    Private Sub socketActive(socket As Socket)

        'Set up variables for our streams
        Dim dataReceived(1024) As Byte
        Dim bytesReceived As Integer

        Dim message As String
        Dim messageSplit As String()

        While (socket.Connected)
            bytesReceived = socket.Receive(dataReceived, dataReceived.Length, 0)

            'If we received -1 bytes or a closing byte, close the socket
            If (bytesReceived <= 0 OrElse dataReceived(0) = &H88) Then
                bottomStatusText.Text = "Close byte was received. Closing socket."
                socket.Disconnect(False)
                Exit While
            End If
            message = Nothing
            message = decodeMessage(dataReceived, bytesReceived)

            updateStatusText("Bytes Received: " & bytesReceived)

            messageSplit = Split(message, " ", 2)

            If (messageSplit(0).ToLower() = "update") Then

                messageSplit = Split(messageSplit(1), " ", 2)

                Dim socketInt As Integer = Convert.ChangeType(messageSplit(0), GetType(Integer))

                If Me.clientTable.InvokeRequired Then
                    Me.clientTable.Invoke(New Action(Sub() queryTable(socketInt, messageSplit(1))))
                Else
                    queryTable(socketInt, messageSplit(1))
                End If

            End If

        End While

        connectionStatus.Text = "Disconnected"

    End Sub

    Private Function decodeMessage(ByVal data() As Byte, ByVal len As Integer) As String
        'Initialize counter variables
        Dim i, j As Integer

        'Initialize some variable to help decode the message
        Dim maskIndex As Integer
        Dim mask(4) As Byte

        Dim decodedBytes() As Byte

        'Get our mask index position
        maskIndex = 2
        If ((data(1) And CByte(&H7F)) = 126) Then
            maskIndex = 4
        ElseIf ((data(1) And CByte(&H7F)) = 127) Then
            maskIndex = 10
        End If

        ReDim decodedBytes(len - maskIndex - 4)

        For i = 0 To 3
            mask(i) = data(maskIndex + i)
        Next

        j = maskIndex + 4

        For i = 0 To len - maskIndex - 4
            decodedBytes(i) = data(j) Xor mask(i Mod 4)
            j += 1
        Next

        msg = System.Text.Encoding.UTF8.GetString(data, maskIndex, data(1))

        Return msg
    End Function

    Private Sub queryTable(ByVal socket As Integer, ByVal message As String)

        Dim foundRow As DataGridViewRow = Nothing
        Dim messageSplit As String()

        If message.Length > 0 Then

            If clientTable.RowCount > 1 Then

                For Each row In clientTable.Rows

                    If row.Index = clientTable.Rows.Count - 1 Then
                        Continue For
                    End If

                    If (row.Cells(0).Value = socket.ToString()) Then
                        foundRow = row
                        Exit For
                    End If
                Next

            End If

            If (foundRow Is Nothing) Then
                addRow(clientTable, socket, message)
            Else
                foundRow.Cells("lastCommand").Value = message

                messageSplit = Split(message, " ", 2)
                If (messageSplit(0).ToLower() = "set") Then
                    alterRow(foundRow, messageSplit(1))
                End If

            End If

        End If

    End Sub

    Private Sub addRow(ByRef table As DataGridView, ByVal socketNumber As Integer, ByVal lastCommand As String)
        table.Rows.Add(New String() {socketNumber.ToString, "1", "0", "0", lastCommand})
    End Sub

    Private Sub alterRow(ByRef row As DataGridViewRow, ByVal command As String)
        Dim commandArray As String() = Split(command, " ", 2)

        Select Case commandArray(0)
            Case "active"
                row.Cells("isActive").Value = commandArray(1)
            Case "admin"
                row.Cells("isAdmin").Value = commandArray(1)
            Case "monitor"
                row.Cells("isMonitor").Value = commandArray(1)
            Case Else
                bottomStatusText.Text = "Invalid set command from socket #" & row.Cells("socketNumber").Value
        End Select

    End Sub

    Function getSHA1Hash(ByVal strToHash As String) As String
        Dim sha1Obj As New Security.Cryptography.SHA1CryptoServiceProvider
        Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(strToHash)
        Dim result As String

        bytesToHash = sha1Obj.ComputeHash(bytesToHash)
        result = Convert.ToBase64String(bytesToHash)

        Return result
    End Function

    Sub SendMessage2(ByVal socket As Socket, ByVal message As String, ByVal len As Integer)
        Dim sentBytes As Integer
        Dim frameCount As Integer
        Dim len16 As UInt16
        Dim reply(len + 8) As [Byte]
        Dim frame(10) As [Byte]
        Dim maskingBytes(4) As [Byte]

        frame(0) = CByte(&H81)

        If (len <= 125) Then
            frame(1) = CByte(len + 128)
            frameCount = 2
        ElseIf (len >= 126 AndAlso len <= 65535) Then
            frame(1) = CByte(126 + 128)
            len16 = Convert.ToUInt16(len)
            frame(2) = CByte(BitConverter.GetBytes(len16).GetValue(0))
            frame(3) = CByte(BitConverter.GetBytes(len16).GetValue(1))
            frameCount = 4
        Else
            frame(1) = CByte(127 + 128)
            'Odds are we are not ever going to get here. but here is some code
            'frame(2) = CByte(((len >> 56) And CByte(255)))
            'frame(3) = CByte(((len >> 48) And CByte(255)))
            'frame(4) = CByte(((len >> 40) And CByte(255)))
            'frame(5) = CByte(((len >> 32) And CByte(255)))
            'frame(6) = CByte(((len >> 24) And CByte(255)))
            'frame(7) = CByte(((len >> 16) And CByte(255)))
            'frame(8) = CByte(((len >> 8) And CByte(255)))
            'frame(9) = CByte((len) And CByte(255))
            frameCount = 10
        End If

        For i As Integer = 0 To frameCount - 1
            reply(i) = Convert.ToByte(frame(i))
        Next

        For i As Integer = 0 To 3
            Randomize()
            maskingBytes(i) = CByte(Int((127 * Rnd()) + 1) Mod 56 * 2) 'For security, I recommend changing this if you got this from GitHub
            reply(frameCount + i) = maskingBytes(i)
        Next

        For i As Integer = 0 To len
            If (Not i = len) Then
                reply(frameCount + i + 4) = Convert.ToByte(message.Chars(i)) Xor maskingBytes(i Mod 4)
            Else
                reply(frameCount + i + 4) = CByte(0) Xor maskingBytes(i Mod 4)
            End If
        Next

        sentBytes = socket.Send(reply, reply.Length, 0)

        If (sentBytes <= 0) Then
            Console.WriteLine("WE ARE NOT WRITING!!")
        End If

        For i As Integer = 0 To reply.Length - 1
            reply(i) = Nothing
        Next

    End Sub

    Sub SendMessage(sck As Socket, message As String)
        Dim rawData = System.Text.Encoding.UTF8.GetBytes(message)
        Dim decoded = System.Text.Encoding.UTF8.GetChars(rawData)
        Console.WriteLine(decoded)
        Dim frameCount = 0
        Dim frame(10) As Byte
        frame(0) = CByte(129)

        If rawData.Length <= 125 Then
            frame(1) = CByte(rawData.Length + 1)
            frameCount = 2
        ElseIf rawData.Length >= 126 AndAlso rawData.Length <= 65535 Then
            frame(1) = CByte(126)
            Dim len = CByte(rawData.Length)
            frame(2) = CByte(((len >> 8) And CByte(255)))
            frame(3) = CByte((len) And CByte(255))
            frameCount = 4
        Else
            frame(1) = CByte(127)
            Dim len = CByte(rawData.Length + 1)
            frame(2) = CByte(((len >> 56) And CByte(255)))
            frame(3) = CByte(((len >> 48) And CByte(255)))
            frame(4) = CByte(((len >> 40) And CByte(255)))
            frame(5) = CByte(((len >> 32) And CByte(255)))
            frame(6) = CByte(((len >> 24) And CByte(255)))
            frame(7) = CByte(((len >> 16) And CByte(255)))
            frame(8) = CByte(((len >> 8) And CByte(255)))
            frame(9) = CByte((len) And CByte(255))
            frameCount = 10
        End If
        Dim bLength = frameCount + rawData.Length
        Console.WriteLine(frameCount)
        Console.WriteLine(rawData.Length)
        Dim reply(bLength) As Byte

        Dim bLim = 0
        For i = 0 To frameCount - 1
            reply(bLim) = frame(i)
            bLim += 1
        Next

        For i = 0 To rawData.Length - 1
            reply(bLim) = rawData(i)
            bLim += 1
        Next

        sck.Send(reply)
    End Sub

    Private Sub updateStatusText(ByVal text As String)
        SyncLock statusLock
            bottomStatusText.Text = text
        End SyncLock
    End Sub

    Private Sub Connect_Click(sender As Object, e As EventArgs) Handles menuItem_Connect.Click
        'Dim msg As String
        If (Not (socket Is Nothing OrElse socket.Connected = False)) Then
            Exit Sub
        End If

        Connect("127.0.0.1")

        If (socket IsNot Nothing AndAlso socket.Connected) Then
            socketThread = New Threading.Thread(AddressOf Me.socketActive)

            socketThread.Name = "socketThread"
            socketThread.Start(socket)
        Else
            socket = Nothing
        End If
    End Sub

    Private Sub connectionStatusChange(sender As Object, e As EventArgs) Handles connectionStatus.TextChanged
        Dim text As String = connectionStatus.Text.ToLower()
        If (text = "connected") Then
            connectionStatus.Image = My.Resources.plug_connect
        ElseIf (text = "disconnected") Then
            connectionStatus.Image = My.Resources.plug_disconnect_slash
        End If
    End Sub

    Private Sub Disconnect_Click(sender As Object, e As EventArgs) Handles menuItem_Disconnect.Click
        Try
            socket.Disconnect(False)
        Catch
            updateStatusText("Failed to disconnect")
        End Try
        socket = Nothing
    End Sub
End Class
