Imports System.IO
Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json.Linq
'موقع https : //ocr.space/
Public Class Form2
    Private ReadOnly apiKey As String = "K82213120188957" 'APi

    ' دالة لتحميل الصورة من الإنترنت
    Private Async Function DownloadImageAsync(ByVal imageUrl As String) As Task(Of Byte())
        Using client As New HttpClient()
            Return Await client.GetByteArrayAsync(imageUrl)
        End Using
    End Function

    ' دالة لاستخراج النص من الصورة عبر OCR.space API
    Private Async Function ExtractTextFromImageUrlAsync(ByVal imageUrl As String) As Task(Of String)
        Dim extractedText As String = String.Empty

        Try
            ' تحميل الصورة من الإنترنت
            Dim imageData As Byte() = Await DownloadImageAsync(imageUrl)

            Using client As New HttpClient()
                Dim requestContent As New MultipartFormDataContent()
                requestContent.Add(New ByteArrayContent(imageData), "file", "image.png")
                requestContent.Add(New StringContent(apiKey), "apikey")
                requestContent.Add(New StringContent("eng"), "language")

                Dim response As HttpResponseMessage = Await client.PostAsync("https://api.ocr.space/parse/image", requestContent)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                ' تحليل JSON للاستجابة
                Dim resultJson As JObject = JObject.Parse(result)
                extractedText = resultJson("ParsedResults")(0)("ParsedText").ToString()
            End Using
        Catch ex As Exception
            MessageBox.Show("حدث خطأ: " & ex.Message)
        End Try

        Return extractedText
    End Function

    ' حدث لزر لاستخراج النص عند الضغط عليه
    Private Async Sub ButtonExtractText_Click(sender As Object, e As EventArgs) Handles ButtonExtractText.Click
        Dim imageUrl As String = TextBox2.Text ' ضع رابط الصورة هنا
        Dim extractedText As String = Await ExtractTextFromImageUrlAsync(imageUrl)
        TextBox1.Text = (extractedText)
    End Sub
End Class
