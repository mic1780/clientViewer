<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class mainPanel
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.clientTable = New System.Windows.Forms.DataGridView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.actionList = New System.Windows.Forms.ListBox()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.menuItem_File = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItem_Connect = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItem_Disconnect = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItem_Help = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.bottomStatusText = New System.Windows.Forms.ToolStripStatusLabel()
        Me.bottomSeperater = New System.Windows.Forms.ToolStripStatusLabel()
        Me.connectionStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.socketNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.isActive = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.isAdmin = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.isMonitor = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.lastCommand = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.clientTable, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'clientTable
        '
        Me.clientTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.clientTable.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.socketNumber, Me.isActive, Me.isAdmin, Me.isMonitor, Me.lastCommand})
        Me.clientTable.Location = New System.Drawing.Point(12, 57)
        Me.clientTable.Name = "clientTable"
        Me.clientTable.RowTemplate.Height = 24
        Me.clientTable.Size = New System.Drawing.Size(606, 325)
        Me.clientTable.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 37)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(117, 17)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Client Information"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(621, 37)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(115, 17)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Available Actions"
        '
        'actionList
        '
        Me.actionList.FormattingEnabled = True
        Me.actionList.ItemHeight = 16
        Me.actionList.Items.AddRange(New Object() {"Set Name", "Make Admin", "Make Monitor", "Disconnect", ""})
        Me.actionList.Location = New System.Drawing.Point(624, 57)
        Me.actionList.Name = "actionList"
        Me.actionList.Size = New System.Drawing.Size(209, 84)
        Me.actionList.TabIndex = 4
        '
        'MenuStrip1
        '
        Me.MenuStrip1.BackColor = System.Drawing.SystemColors.MenuBar
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuItem_File, Me.menuItem_Connect, Me.menuItem_Disconnect, Me.menuItem_Help})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(846, 28)
        Me.MenuStrip1.TabIndex = 5
        Me.MenuStrip1.Text = "menu"
        '
        'menuItem_File
        '
        Me.menuItem_File.Name = "menuItem_File"
        Me.menuItem_File.Size = New System.Drawing.Size(44, 24)
        Me.menuItem_File.Text = "File"
        '
        'menuItem_Connect
        '
        Me.menuItem_Connect.Name = "menuItem_Connect"
        Me.menuItem_Connect.Size = New System.Drawing.Size(75, 24)
        Me.menuItem_Connect.Text = "Connect"
        '
        'menuItem_Disconnect
        '
        Me.menuItem_Disconnect.Name = "menuItem_Disconnect"
        Me.menuItem_Disconnect.Size = New System.Drawing.Size(94, 24)
        Me.menuItem_Disconnect.Text = "Disconnect"
        '
        'menuItem_Help
        '
        Me.menuItem_Help.Name = "menuItem_Help"
        Me.menuItem_Help.Size = New System.Drawing.Size(53, 24)
        Me.menuItem_Help.Text = "Help"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.bottomStatusText, Me.bottomSeperater, Me.connectionStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 418)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(846, 25)
        Me.StatusStrip1.SizingGrip = False
        Me.StatusStrip1.TabIndex = 6
        Me.StatusStrip1.Text = "bottomStatusBar"
        '
        'bottomStatusText
        '
        Me.bottomStatusText.Name = "bottomStatusText"
        Me.bottomStatusText.Size = New System.Drawing.Size(195, 20)
        Me.bottomStatusText.Text = "Click Connect to get started."
        Me.bottomStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'bottomSeperater
        '
        Me.bottomSeperater.Name = "bottomSeperater"
        Me.bottomSeperater.Size = New System.Drawing.Size(521, 20)
        Me.bottomSeperater.Spring = True
        '
        'connectionStatus
        '
        Me.connectionStatus.Image = Global.clientViewer.My.Resources.Resources.plug_disconnect_slash
        Me.connectionStatus.Name = "connectionStatus"
        Me.connectionStatus.Size = New System.Drawing.Size(115, 20)
        Me.connectionStatus.Text = "Disconnected"
        Me.connectionStatus.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        '
        'socketNumber
        '
        Me.socketNumber.HeaderText = "Socket #"
        Me.socketNumber.Name = "socketNumber"
        Me.socketNumber.ReadOnly = True
        Me.socketNumber.Width = 70
        '
        'isActive
        '
        Me.isActive.HeaderText = "Is Active"
        Me.isActive.Name = "isActive"
        Me.isActive.ReadOnly = True
        Me.isActive.Width = 70
        '
        'isAdmin
        '
        Me.isAdmin.HeaderText = "Is Admin"
        Me.isAdmin.Name = "isAdmin"
        Me.isAdmin.ReadOnly = True
        Me.isAdmin.Width = 70
        '
        'isMonitor
        '
        Me.isMonitor.HeaderText = "Is Monitor"
        Me.isMonitor.Name = "isMonitor"
        Me.isMonitor.ReadOnly = True
        Me.isMonitor.Width = 75
        '
        'lastCommand
        '
        Me.lastCommand.HeaderText = "Last Command"
        Me.lastCommand.Name = "lastCommand"
        Me.lastCommand.ReadOnly = True
        Me.lastCommand.Width = 238
        '
        'mainPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(846, 443)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.actionList)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.clientTable)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(864, 490)
        Me.Name = "mainPanel"
        Me.ShowIcon = False
        Me.Text = "Activity Monitor"
        CType(Me.clientTable, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents clientTable As System.Windows.Forms.DataGridView
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents actionList As System.Windows.Forms.ListBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents menuItem_File As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuItem_Connect As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuItem_Disconnect As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuItem_Help As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents bottomStatusText As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents connectionStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents bottomSeperater As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents socketNumber As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents isActive As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents isAdmin As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents isMonitor As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents lastCommand As System.Windows.Forms.DataGridViewTextBoxColumn

End Class
