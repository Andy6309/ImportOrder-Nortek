<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        GroupBox1 = New GroupBox()
        Button5 = New Button()
        Label3 = New Label()
        ProgressBar1 = New ProgressBar()
        btnMakeCPs = New Button()
        Button4 = New Button()
        Label2 = New Label()
        Button2 = New Button()
        Button1 = New Button()
        Label1 = New Label()
        Button3 = New Button()
        GroupBox1.SuspendLayout()
        SuspendLayout()
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(Button5)
        GroupBox1.Controls.Add(Label3)
        GroupBox1.Controls.Add(ProgressBar1)
        GroupBox1.Controls.Add(btnMakeCPs)
        GroupBox1.Controls.Add(Button4)
        GroupBox1.Controls.Add(Label2)
        GroupBox1.Controls.Add(Button2)
        GroupBox1.Controls.Add(Button1)
        GroupBox1.Controls.Add(Label1)
        GroupBox1.Location = New Point(12, 12)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(532, 239)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Modify Order"
        ' 
        ' Button5
        ' 
        Button5.Location = New Point(321, 36)
        Button5.Name = "Button5"
        Button5.Size = New Size(191, 34)
        Button5.TabIndex = 8
        Button5.Text = "Open Existing Order"
        Button5.UseVisualStyleBackColor = True
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(6, 153)
        Label3.Name = "Label3"
        Label3.Size = New Size(0, 20)
        Label3.TabIndex = 7
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Location = New Point(6, 111)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(520, 29)
        ProgressBar1.TabIndex = 6
        ' 
        ' btnMakeCPs
        ' 
        btnMakeCPs.Location = New Point(195, 185)
        btnMakeCPs.Name = "btnMakeCPs"
        btnMakeCPs.Size = New Size(144, 48)
        btnMakeCPs.TabIndex = 5
        btnMakeCPs.Text = "Make New CP Files"
        btnMakeCPs.UseVisualStyleBackColor = True
        ' 
        ' Button4
        ' 
        Button4.Location = New Point(382, 185)
        Button4.Name = "Button4"
        Button4.Size = New Size(144, 48)
        Button4.TabIndex = 4
        Button4.Text = "Import Order"
        Button4.UseVisualStyleBackColor = True
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(15, 78)
        Label2.MaximumSize = New Size(600, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(0, 20)
        Label2.TabIndex = 3
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(6, 185)
        Button2.Name = "Button2"
        Button2.Size = New Size(144, 48)
        Button2.TabIndex = 1
        Button2.Text = "Convert File"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(106, 36)
        Button1.Name = "Button1"
        Button1.Size = New Size(209, 34)
        Button1.TabIndex = 1
        Button1.Text = "Order and AutoTool"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(6, 43)
        Label1.Name = "Label1"
        Label1.Size = New Size(94, 20)
        Label1.TabIndex = 0
        Label1.Text = "Select Order:"
        ' 
        ' Button3
        ' 
        Button3.Location = New Point(450, 257)
        Button3.Name = "Button3"
        Button3.Size = New Size(94, 29)
        Button3.TabIndex = 1
        Button3.Text = "Exit"
        Button3.UseVisualStyleBackColor = True
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(562, 298)
        Controls.Add(Button3)
        Controls.Add(GroupBox1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Form1"
        Text = "Import Order - Nortek"
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Button2 As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents Button4 As Button
    Friend WithEvents btnMakeCPs As Button
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents Label3 As Label
    Friend WithEvents Button5 As Button

End Class
