Imports System.Net.Http
Imports System.Text.RegularExpressions

Public Class Form1
    ' دالة لجلب محتوى الصفحة من الإنترنت
    Public Async Function GetPageContentAsync(ByVal url As String) As Task(Of String)
        Dim client As New HttpClient()
        Dim response As HttpResponseMessage = Await client.GetAsync(url)
        If response.IsSuccessStatusCode Then
            Dim pageContent As String = Await response.Content.ReadAsStringAsync()
            Return pageContent
        Else
            Return Nothing
        End If
    End Function

    ' دالة لاستخراج جميع العناوين باستخدام تعبير منتظم
    Public Function ExtractLines(ByVal pageContent As String, ByVal pattern As String) As List(Of String)
        Dim matches As MatchCollection = Regex.Matches(pageContent, pattern)
        Dim extractedLines As New List(Of String)
        For Each match As Match In matches
            extractedLines.Add(match.Value.Trim())
        Next
        Return extractedLines
    End Function

    ' دالة لاستخراج رابط الصورة بعد كلمة "Password:"
    Public Function ExtractImageUrl(ByVal pageContent As String) As String
        ' نمط التعبير المنتظم للبحث عن رابط الصورة بعد كلمة "Password:"
        Dim pattern As String = "Password:\s*<img\s+src\s*=\s*[""']([^""']+)[""']"
        Dim match As Match = Regex.Match(pageContent, pattern, RegexOptions.IgnoreCase)
        If match.Success Then
            ' استرجاع الرابط من المجموعة الأولى في النمط
            Return match.Groups(1).Value.Trim()
        End If
        Return String.Empty
    End Function

    ' دالة لجلب النصوص وعرضها في ComboBox، بالإضافة لجلب رابط الصورة
    Public Async Sub FetchAndDisplayLines()
        Dim url As String = "https://www.vpnbook.com/freevpn"
        Dim pageContent As String = Await GetPageContentAsync(url)

        If Not String.IsNullOrEmpty(pageContent) Then
            ' نمط التعبير المنتظم للبحث عن العناوين
            Dim pattern As String = "\b[A-Z]{2}\d{1,3}\.vpnbook\.com\b"

            ' استخراج العناوين المطابقة للنمط
            Dim extractedLines As List(Of String) = ExtractLines(pageContent, pattern)

            If extractedLines.Count > 0 Then
                ' عرض العناوين المستخرجة في ComboBox
                ComboBox1.Items.Clear() ' إفراغ العناصر القديمة
                For Each line As String In extractedLines
                    ComboBox1.Items.Add(line)
                Next

                ' اختيار أول عنصر في ComboBox بشكل افتراضي
                If ComboBox1.Items.Count > 0 Then
                    ComboBox1.SelectedIndex = 0
                End If
            Else
                MessageBox.Show("لم يتم العثور على أي عناوين.")
            End If

            ' استخراج رابط الصورة
            Dim imageUrl As String = ExtractImageUrl(pageContent)
            If Not String.IsNullOrEmpty(imageUrl) Then
                ' إضافة النطاق الثابت إلى الرابط إذا كان الرابط نسبي
                If Not imageUrl.StartsWith("http") Then
                    imageUrl = "https://www.vpnbook.com/" & imageUrl
                End If
                ' عرض رابط الصورة في TextBox2
                TextBox3.Text = imageUrl
            Else
                TextBox3.Text = "لم يتم العثور على رابط الصورة."
            End If

        Else
            MessageBox.Show("فشل في جلب محتوى الصفحة.")
        End If
    End Sub

    ' حدث لتحديث TextBox عند تغيير العنصر المختار في ComboBox
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedItem IsNot Nothing Then
            TextBox1.Text = ComboBox1.SelectedItem.ToString()
        End If
    End Sub

    ' استدعاء الدالة عند الضغط على زر معين
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        FetchAndDisplayLines()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form2.Show()
    End Sub
End Class
