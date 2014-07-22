Imports System.Net.Sockets
Public Class mainPanel
    Dim msg As String
    Dim lockObject As New Object()
    Dim socketThread As Threading.Thread
    Dim socket As Socket

    Private Sub mainPanel_load(sender As Object, e As EventArgs) Handles MyBase.Load
        tableUpdate(New String() {"1", "1", "0", "0", "0", "set name User 1"})
        tableUpdate(New String() {"2", "1", "1", "0", "0", "set name User 2"})
    End Sub

    Private Function Connect(ByVal host As String) As Socket
        Dim dataReceived(1024) As [Byte]
        Dim randVal As Integer
        Dim octString As String = ""
        Dim request As String = ""

        Dim socket As Socket = New Socket(SocketType.Stream, ProtocolType.Tcp)
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
            Console.WriteLine("'" & reader.Peek() & "'")
            Console.WriteLine(reader.ReadLine())
        End While

        connectionStatus.Text = "Connected"

        msg = "set name monitor"
        SendMessage2(socket, msg, msg.Length)

        msg = "updateTable 1 1 4 1 1 set name monitor"
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
                Console.WriteLine("Close byte was received. Closing socket.")
                socket.Disconnect(False)
                Exit While
            End If
            message = Nothing
            message = decodeMessage(dataReceived, bytesReceived)

            'For i As Integer = 0 To bytesReceived
            'Console.WriteLine(dataReceived(i))
            'Next
            'Console.WriteLine("Bytes Received: " & bytesReceived)
            'Console.WriteLine("Message: " & message)
            Console.WriteLine("We got here")
            messageSplit = Split(message, " ", 2)

            If (messageSplit(0).ToLower() = "updatetable") Then
                Console.WriteLine("We got here")
                ' = New String() {messageSplit(0), messageSplit(1), messageSplit(2), messageSplit(3), messageSplit(4), messageSplit(5)}
                Dim tableCells As String() = Split(messageSplit(1), " ", 6)
                'messageSplit = Split(messageSplit(1), " ", 6)
                If Me.clientTable.InvokeRequired Then
                    Me.clientTable.Invoke(New Action(Sub() tableUpdate(tableCells)))
                Else
                    tableUpdate(tableCells)
                End If
                'SyncLock (lockObject)

                'End SyncLock
            End If
            Console.WriteLine("Message: '" & message & "'")

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
        Console.WriteLine((data(1) And CByte(&H7F)))
        If ((data(1) And CByte(&H7F)) = 126) Then
            maskIndex = 4
        ElseIf ((data(1) And CByte(&H7F)) = 127) Then
            maskIndex = 10
            'Else
            '   maskIndex = 2
        End If

        ReDim decodedBytes(len - maskIndex - 4)

        For i = 0 To 3
            mask(i) = data(maskIndex + i)
        Next

        j = maskIndex + 4

        For i = 0 To len - maskIndex - 4
            'Console.WriteLine(Convert.ToByte(data(j)) Xor mask(i Mod 4))
            decodedBytes(i) = data(j) Xor mask(i Mod 4)
            j += 1
        Next

        If (data(1) = len - 4) Then
            'Console.WriteLine("Data(1) = len: '" & System.Text.Encoding.UTF8.GetString(data, maskIndex, len - 4) & "'")
            msg = System.Text.Encoding.UTF8.GetString(data, maskIndex, len - 4)
        ElseIf (data(1) = len - 2) Then
            msg = System.Text.Encoding.UTF8.GetString(data, maskIndex, len - 2)
        Else
            'Console.WriteLine(System.Text.Encoding.UTF8.GetString(decodedBytes))
            msg = System.Text.Encoding.UTF8.GetString(decodedBytes)
        End If

        Return msg
    End Function

    Private Sub tableUpdate(ByRef rowData As String())

        If rowData.Count > 0 Then

            If clientTable.RowCount > 1 Then

                For Each row In clientTable.Rows

                    If row.Index = clientTable.Rows.Count - 1 Then
                        Continue For
                    End If
                    For Each cell In row.Cells
                        Console.Write(clientTable.Rows(row.Index).Cells(cell.ColumnIndex).Value)
                        Console.Write(" ")
                    Next
                    Console.Write(vbCrLf)
                    'Console.WriteLine(If(row.Cells(0).Value = 0, "0", row.Cells(0).Value()))
                Next

            End If

            clientTable.Rows.Add(rowData)
        End If

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
            maskingBytes(i) = CByte(Int((128 * Rnd()) + 1) Mod 56 * 2)
            reply(frameCount + i) = maskingBytes(i)
        Next

        For i As Integer = 0 To len
            If (Not i = len) Then
                reply(frameCount + i + 4) = Convert.ToByte(message.Chars(i)) Xor maskingBytes(i Mod 4)
            Else
                reply(frameCount + i + 4) = CByte(0) Xor maskingBytes(i Mod 4)
            End If
        Next

        SyncLock (lockObject)
            If (socket.Send(reply) <= 0) Then
                Console.WriteLine("WE ARE NOT WRITING!!")
            End If
        End SyncLock

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

    Private Sub Connect_Click(sender As Object, e As EventArgs) Handles menuItem_Connect.Click
        Dim msg As String
        If (Not (socket Is Nothing OrElse socket.Connected = False)) Then
            Exit Sub
        End If
        socket = Connect("127.0.0.1")
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
        socket.Disconnect(False)
        socket = Nothing
    End Sub
End Class
