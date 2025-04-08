Imports System.Windows.Forms
Imports System
Imports System.IO
Imports System.Data
Imports System.Data.OleDb
Imports System.Text.RegularExpressions
Imports System.Net
Imports System.CodeDom

Public Class Form1
    Dim originalFilePathExisting As String = String.Empty
    Dim originalFilePath As String = String.Empty
    Dim blFMSDirFound As Boolean = False
    Dim blPPDirFound As Boolean = False
    Dim NCXDir As String = "C:\Prima Power\ncexpress\"
    Dim PartDir As String
    Dim Machine As String
    Dim PartNCX As New PARANCXLib.PartNCX
    Dim NestNCX As New PARANCXLib.NestNCX
    Dim DbOrder As PARANCXLib.OrderCollection
    Dim Order As PARANCXLib.Order
    Dim ImportFile As String

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            blFMSDirFound = Directory.Exists("C:\fms\")
            blPPDirFound = Directory.Exists("C:\Prima Power\")

            If blFMSDirFound Then
                NCXDir = "C:\Prima Power\ncexpress\"
            End If

            Machine = "SGE8_NORTEK" 'Configure default machine here //Setup way to allow user to choose machine if necessary -AM


            Dim envFilePath As String = Path.Combine(NCXDir, Machine, "WORK", "environment.dat")


            If File.Exists(envFilePath) Then
                Dim lines As String() = File.ReadAllLines(envFilePath)

                'Gather PartDir information from environment.dat -AM
                If lines.Length >= 19 Then
                    PartDir = lines(18).Trim()
                Else
                    MessageBox.Show("Error Reading environment.dat file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    PartDir = Path.Combine(NCXDir, "parts\")
                End If
            Else
                MessageBox.Show($"environment.dat not found: {envFilePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                PartDir = Path.Combine(NCXDir, "parts\")
            End If

            ' Ensure PartDir exists -AM
            If Not Directory.Exists(PartDir) Then
                Directory.CreateDirectory(PartDir)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error during form initialization: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Public Sub GetEnv(ByVal EnvStr As String) 'maybe not necessary (need to test more... does not seem to hurt) -AM
        Try
            If File.Exists(EnvStr) Then
                Using rf As New StreamReader(EnvStr)
                    Dim linecount As Integer = 0
                    Do Until rf.EndOfStream
                        Dim str As String = rf.ReadLine
                        linecount += 1
                        If linecount = 19 Then PartDir = str
                    Loop
                End Using
            End If
        Catch ex As Exception
            MessageBox.Show($"Error getting Environment data: {ex.Message}", "Environment Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ToggleButtons(ByVal enable As Boolean) 'this enables me to control button availability -AM
        Button1.Enabled = enable
        Button2.Enabled = enable
        Button3.Enabled = enable
        Button4.Enabled = enable
        btnMakeCPs.Enabled = enable
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim openFileDialog As New OpenFileDialog() With {
    .Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
    .Title = "Select a File to Load"
    }

        If openFileDialog.ShowDialog = DialogResult.OK Then
            originalFilePath = openFileDialog.FileName
            Label2.Text = $"Selected File: {originalFilePath}"
            ToggleButtons(False)


            'Prompt for usign new PartDir folder
            Dim folderDialog As New FolderBrowserDialog() With {
                .Description = "Create a folder to store .cp files for this project",
                .SelectedPath = PartDir ' Sets the initial folder to PartDir
}

            If folderDialog.ShowDialog() = DialogResult.OK Then
                PartDir = folderDialog.SelectedPath
                MessageBox.Show($"New Part Directory Set: {PartDir}", "Folder Selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show("No folder selected. Using existing PartDir.", "Folder Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If


            ProgressBar1.Value = 0
            ProgressBar1.Maximum = 100
            ProgressBar1.Step = 1
            Label3.Text = ""
            Dim foundFiles As New List(Of String)

            Try
                Dim lines = File.ReadAllLines(originalFilePath)

                If lines.Length < 2 Then
                    MessageBox.Show("File does not contain enough data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    ToggleButtons(True)
                    Return
                End If

                Dim cpFiles = Directory.GetFiles(PartDir, "*.cp", SearchOption.AllDirectories)

                For i = 1 To lines.Length - 1
                    ' Skip blank lines
                    If String.IsNullOrWhiteSpace(lines(i)) Then Continue For

                    ' Split the line by tab characters and process
                    Dim fields = lines(i).Split(vbTab)

                    Try
                        If fields.Length >= 17 Then
                            Dim PartName = fields(1).Trim
                            Dim Material = fields(6).Trim
                            Dim Thickness = fields(7).Trim
                            Dim Machine = fields(3).Trim
                            Dim DrawingFile = fields(9).Trim
                            Dim Turret = fields(10).Trim

                            Dim partFilePath = Path.Combine(PartDir, PartName & ".cp")


                            Label3.Text = $"Processing: {PartName}.cp"
                            Label3.Refresh()

                            ' Check if part file exists
                            If cpFiles.Contains(partFilePath) Then
                                Dim fileName = Path.GetFileName(partFilePath)
                                foundFiles.Add(fileName)
                            Else
                                ' Process part if it doesn't exist
                                PartNCX.Init()
                                PartNCX.Machine = Machine
                                PartNCX.SetMaterial(Material, Thickness)
                                PartNCX.Turret = Turret
                                PartNCX.Import(DrawingFile)

                                Dim applyPerimeter = False

                                ' Rules for specific prefix (CFD, CBI, and CSV)
                                If PartName.StartsWith("CFD") OrElse PartName.StartsWith("CBI") Then
                                    ' Always apply perimeter for CFD and CBI parts
                                    applyPerimeter = True
                                    Debug.WriteLine($"Perimeter applied due to PartName prefix (CFD/CBI): {PartName}")

                                ElseIf PartName.StartsWith("CSV") Then
                                    ' Apply status 2 for CSV parts- customer wants to check these always
                                    PartNCX.Status = 2
                                    Debug.WriteLine($"Status set to 2 for CSV part: {PartName}")

                                ElseIf PartNCX.Width <= 3.151 Then
                                    ' Apply perimeter if width <= 3.151 for other parts
                                    applyPerimeter = True
                                    Debug.WriteLine($"Perimeter applied due to width <= 3.151: {PartName}, Width: {PartNCX.Width}")

                                Else
                                    ' Process normally if width > 3.151 without applying perimeter
                                    Debug.WriteLine($"Processing without perimeter due to width > 3.151: {PartName}, Width: {PartNCX.Width}")
                                    applyPerimeter = False ' No perimeter applied, just proceed to autotool
                                End If

                                ' Apply perimeter if needed
                                If applyPerimeter Then

                                    PartNCX.Autotool(True, False, False, True, 0)

                                    ' Save after first autotool pass
                                    PartNCX.Save(partFilePath)

                                    'Read again....
                                    PartNCX.Read(partFilePath)

                                    ' Define perimeter rectangle with a 1-inch offset
                                    Dim left = PartNCX.Left - 1
                                    Dim bottom = PartNCX.Bottom - 1
                                    Dim length = PartNCX.Length + 2
                                    Dim width = PartNCX.Width + 2

                                    ' Draw expanded rectangle
                                    PartNCX.Line(left, bottom, left + length, bottom) ' Bottom edge
                                    PartNCX.Line(left + length, bottom, left + length, bottom + width) ' Right edge
                                    PartNCX.Line(left + length, bottom + width, left, bottom + width) ' Top edge
                                    PartNCX.Line(left, bottom + width, left, bottom) ' Left edge



                                    ' Read again
                                    'PartNCX.Read(PartName)

                                    ' Second Autotool Pass
                                    Dim LabelX = PartNCX.Length / 2
                                    Dim LabelY = PartNCX.Width / 2
                                    PartNCX.PrintLabel(1, LabelX, LabelY,, "(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)")
                                    PartNCX.Shear(0, 0, 0, 0, 0, 201)

                                    ' Final Save
                                    PartNCX.Save(partFilePath)
                                Else
                                    ' Process normally if no perimeter is required
                                    Dim LabelX = PartNCX.Length / 2
                                    Dim LabelY = PartNCX.Width / 2
                                    PartNCX.PrintLabel(1, LabelX, LabelY,, "(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)")
                                    PartNCX.Autotool(True, False, True, True, 0)
                                    PartNCX.Save(partFilePath)
                                End If

                                Debug.WriteLine($"New part created: {partFilePath}")
                            End If

                            ' Update progress bar
                            ProgressBar1.Value = CInt(i / (lines.Length - 1) * 100)
                        Else
                            ' If line doesn't have enough fields, show an error for that line
                            MessageBox.Show($"Invalid data format on line {i + 1}.", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                    Catch ex As Exception
                        ' Log any exceptions encountered during processing
                        Debug.WriteLine($"Error processing line {i + 1}: {ex.Message}")
                        MessageBox.Show($"Error processing line {i + 1}: {ex.Message}", "Processing Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End Try
                Next

                ' Show message with list of found files if any
                If foundFiles.Count > 0 Then
                    Dim message = "The following .cp files were already found:" & Environment.NewLine & String.Join(Environment.NewLine, foundFiles)
                    MessageBox.Show(message, "Found .cp Files", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("CP Files Created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Catch ex As Exception
                ' Handle any errors that occurred while reading or processing the file
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                ' Reset the progress bar and enable buttons
                ProgressBar1.Value = 0
                Label3.Text = ""
                ToggleButtons(True)
            End Try
        Else
            MessageBox.Show("No file selected.", "Load Canceled", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub



    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If String.IsNullOrEmpty(originalFilePath) Then
            MessageBox.Show("Please load a file first.", "No File Loaded", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim convertedFilePath = Path.Combine(Path.GetDirectoryName(originalFilePath), $"Converted_{Path.GetFileName(originalFilePath)}")

        Try
            ' Read all lines from the original file
            Dim lines = File.ReadAllLines(originalFilePath)
            Dim modifiedLines As New List(Of String)

            If lines.Length > 0 Then
                ' Convert header to list and add "Status" field
                Dim headerFields = lines(0).Split(vbTab).ToList()
                headerFields.Add("Status")
                modifiedLines.Add(String.Join(vbTab, headerFields.ToArray())) ' Convert back to array before joining
            End If

            ' Loop through each line after the header
            For i = 1 To lines.Length - 1
                Dim fields = lines(i).Split(vbTab).ToList() ' Convert array to List for easier modification

                ' Check if the line contains enough columns
                If fields.Count >= 17 Then
                    Dim partName = fields(1)
                    Dim quantity = Integer.Parse(fields(2)) ' Quantity of parts to duplicate
                    Dim productionLabel = fields(16)

                    ' Loop to duplicate the part based on quantity
                    For j = 0 To quantity - 1
                        Dim newFields = New List(Of String)(fields) ' Clone the list properly
                        Dim newPartName = $"{partName}-{j}" ' Create unique PartName by appending index

                        newFields(1) = newPartName ' Update PartName
                        newFields(2) = "1" ' Ensure that the quantity is set to 1 for each new part

                        ' Update ProductionLabel with the new PartName (you can customize how this label is updated)
                        newFields(16) = UpdateProductionLabel(productionLabel, newPartName)

                        ' Only append -3 if newPartName starts with CSV
                        If newPartName.StartsWith("CSV") Then
                            newFields.Add("-3")
                        Else
                            newFields.Add("") ' Leave empty if no status is assigned
                        End If

                        ' If quantity is 1, we should not create more than 1 instance of that part
                        If quantity = 1 AndAlso j > 0 Then
                            Continue For ' Skip the extra iterations if quantity is 1
                        End If

                        ' Add the modified line to the list
                        modifiedLines.Add(String.Join(vbTab, newFields.ToArray())) ' Convert List back to Array before joining
                    Next
                End If
            Next

            ' Write the modified lines to the converted file
            File.WriteAllLines(convertedFilePath, modifiedLines)

            MessageBox.Show($"File converted successfully!{vbCrLf}Converted file saved at: {convertedFilePath}", "Conversion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles btnMakeCPs.Click
        If String.IsNullOrEmpty(originalFilePath) Then
            MessageBox.Show("Please load a file first.", "No File Loaded", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            GetEnv(Path.Combine(NCXDir, Machine, "WORK", "Environment.dat"))

            If Directory.Exists(PartDir) Then
                PartNCX.Machine = Machine

                Dim lines = File.ReadAllLines(originalFilePath)

                ' Gather all .cp files in PartDir and its subdirectories -AM
                Dim cpFiles = Directory.GetFiles(PartDir, "*.cp", SearchOption.AllDirectories).ToList()

                ' Log all cp files found for debugging
                For Each file In cpFiles
                    Debug.WriteLine($"Found file: {file}")
                Next

                For i = 1 To lines.Length - 1
                    Dim fields = lines(i).Split(vbTab)
                    If fields.Length >= 17 Then
                        Dim partName = fields(1).Trim()
                        Dim quantity = Integer.Parse(fields(2))

                        ' Log which part we are checking
                        Debug.WriteLine($"Checking part: {partName}")

                        ' Find the .cp file in any subdirectory
                        Dim existingFile = cpFiles.FirstOrDefault(Function(f) Path.GetFileName(f).Trim().Equals(partName & ".cp", StringComparison.OrdinalIgnoreCase))

                        If existingFile IsNot Nothing Then

                            PartNCX.Read(existingFile)
                            PartNCX.Replace("PRINT_LABEL", "")
                            PartNCX.PrintLabel(1,,,, "(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)(<Production_Label>)")
                            PartNCX.Save(existingFile)

                            For j = 0 To quantity - 0 ' Adjust to avoid saving an extra file
                                Dim newPartName = $"{partName}-{j}"
                                Dim saveFolder = Path.GetDirectoryName(existingFile)
                                Dim savePath = Path.Combine(saveFolder, newPartName & ".cp")

                                PartNCX.Save(savePath)
                            Next
                        Else
                            MessageBox.Show($"Part file {partName} does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Else
                        ' MessageBox.Show($"Invalid data format on line {i + 1}.", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Next

                MessageBox.Show("New .cp files processed successfully!", "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show($"Part directory does not exist: {PartDir}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Application.Exit()
    End Sub

    Private Function UpdateProductionLabel(productionLabel As String, newPartName As String) As String
        Dim matches = Regex.Matches(productionLabel, "\((.*?)\)")
        Dim sections As New List(Of String)()

        For Each match As Match In matches
            sections.Add(match.Groups(1).Value)
        Next

        If sections.Count > 0 Then
            sections(sections.Count - 1) = newPartName
        End If

        Return "(" & String.Join(")(", sections) & ")"
    End Function

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If String.IsNullOrEmpty(originalFilePath) AndAlso String.IsNullOrEmpty(originalFilePathExisting) Then
            MessageBox.Show("Please load a file first.", "No File Loaded", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            ' Handle converted file path
            Dim convertedFilePath As String = String.Empty
            If Not String.IsNullOrEmpty(originalFilePath) Then
                convertedFilePath = Path.Combine(Path.GetDirectoryName(originalFilePath), $"Converted_{Path.GetFileName(originalFilePath)}")
            End If

            ' Use the converted file if it exists
            If Not String.IsNullOrEmpty(convertedFilePath) AndAlso File.Exists(convertedFilePath) Then
                NestNCX.Machine = "SGE8_NORTEK"
                DbOrder = New PARANCXLib.OrderCollection
                DbOrder.Import(convertedFilePath, False)
                MessageBox.Show("Import from converted file successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                ' If converted file doesn't exist, use the file selected in Button5 (originalFilePathExisting)
            ElseIf Not String.IsNullOrEmpty(originalFilePathExisting) AndAlso File.Exists(originalFilePathExisting) Then
                NestNCX.Machine = "SGE8_NORTEK"
                DbOrder = New PARANCXLib.OrderCollection
                DbOrder.Import(originalFilePathExisting, False)
                MessageBox.Show("Import from selected file successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show("No valid file available for import.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            ' Exit after a successful import
            Application.Exit()

        Catch ex As Exception
            ' Show any error that occurs during the process
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button5_Click_1(sender As Object, e As EventArgs) Handles Button5.Click
        Dim openFileDialogExisting As New OpenFileDialog() With {
            .Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            .Title = "Select a File to Load"
        }

        If openFileDialogExisting.ShowDialog() = DialogResult.OK Then
            originalFilePathExisting = openFileDialogExisting.FileName
            Label2.Text = $"Selected File: {originalFilePathExisting}"

            Label3.Text = ""

            Dim foundFiles As New List(Of String)

            Try
                ' Read all lines from the file
                Dim lines = File.ReadAllLines(originalFilePathExisting)

                ' Check if the file has enough data
                If lines.Length < 2 Then
                    MessageBox.Show("File does not contain enough data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    ToggleButtons(True)
                    Return
                End If

                ' Example: Process the lines (you can customize this logic as needed)
                For Each line As String In lines
                    ' Simulate processing the line (your logic here)
                Next

                ' Process completed
                'MessageBox.Show("File imported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                ' Handle any errors during file processing
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

            Finally
                ' Ensure buttons are re-enabled after processing
                ToggleButtons(True)
            End Try
        End If
    End Sub

End Class