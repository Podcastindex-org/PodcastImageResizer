Imports System.Drawing
Imports System.Drawing.Imaging

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim strURL As String = ""
            Dim intSize As Integer = 0
            Dim strCRC32FileName As String
            strURL = Request.RawUrl.Replace("/", "").Trim

            'CHECK TO SEE IF JPG IS FOUND AT THE END OF THE URL

            If strURL.Contains(".jpg") = True Then

                'THE REQUEST IS ASKING FOR A JPG IMAGE, STRIP OUT THE JPG EXTENTION OF THE URL

                strURL = strURL.Replace(".jpg", "")

                'CHECK TO SEE IF THE DIMENSIONS ARE SPECIFIED

                If strURL.Contains("_") = True Then

                    'THE REQUEST DID CONTAIN DIMENSIONS, GET THE POSITION AND SET THE VALUE

                    Dim intPosition As Integer
                    intPosition = InStrRev(strURL, "_")
                    Dim strDimension As String
                    strDimension = Mid(strURL, intPosition + 1)
                    intSize = CInt(strDimension)

                    'CHECK FOR OUT OF BOUND DIMENSIONS AND SET THE MAX AND MIN

                    If intSize = 0 Then
                        intSize = 15
                    End If
                    If intSize > 3000 Then
                        intSize = 3000
                    End If

                    'REMOVE THE DIMENSIONS FROM THE URL

                    strCRC32FileName = Mid(strURL, 1, intPosition - 1)

                    'SET THE HTTP REPONSE SETTINGS

                    Response.ContentType = "image/jpeg"
                    Response.Clear()
                    Response.BufferOutput = True

                    'LOAD THE IMAGE FROM THE HARD DRIVE AND RESIZE

                    Dim strFileName As String
                    strFileName = "D:\images\" + strCRC32FileName + ".image"

                    'CHECK TO MAKE SURE THE IMAGE FILE EXISTS, IF NOT THEN REVERT TO THE DEFAULT IMAGE

                    If System.IO.File.Exists(strFileName) = False Then
                        strFileName = "D:\images\default.jpg"
                    End If

                    'LOAD THE IMAGE INTO A BITMAP DATA TYPE

                    Dim objOriginalArtwork As New Bitmap(strFileName)

                    'RESIZE THE IMAGE

                    Dim objResized = New Bitmap(intSize, intSize)
                    Dim objGraphics = Graphics.FromImage(objResized)
                    objGraphics.DrawImage(objOriginalArtwork, 0, 0, intSize, intSize)
                    Dim myEncoder = System.Drawing.Imaging.Encoder.Quality
                    Dim myEncoderParameters As New EncoderParameters(1)
                    Dim myEncoderParameter = New EncoderParameter(myEncoder, 95L)
                    myEncoderParameters.Param(0) = myEncoderParameter
                    Dim jpgEncoder As ImageCodecInfo
                    jpgEncoder = GetEncoder(ImageFormat.Jpeg)

                    'SAVE THE RESIZED IMAGE STREAM TO THE HTTP DATA STREAM

                    objResized.Save(Response.OutputStream, ImageFormat.Jpeg)

                    'DISPOSE VARABLES

                    objOriginalArtwork.Dispose()
                    objResized.Dispose()
                    objGraphics.Dispose()
                    myEncoder = Nothing
                    myEncoderParameter.Dispose()
                    myEncoderParameters.Dispose()
                    jpgEncoder = Nothing
                    objResized.Dispose()

                    'FLUSH OUT THE HTTP DATA STREAM TO THE REQUEST

                    Response.Flush()
                Else

                    'ERROR RESPONSE IF THEY DIDN'T SPECIFY A DIMENSION SIZE

                    Response.Write("Looks like you didn't specify a dimension size.")
                End If
            Else

                'ERROR RESPONSE IF THEY REQUESTED AN IMAGE OTHER THAN JPG

                Response.Write("Looks like you didn't specify a jpg image.")
            End If
        Catch ex As Exception

            'CATCH ALL ERROR RESPONSE

            Response.Write("Looks like the image request is not properly formated.")
        End Try

    End Sub

    Private Function GetEncoder(imgFormat As ImageFormat) As ImageCodecInfo
        Dim objCodecs As ImageCodecInfo()
        objCodecs = ImageCodecInfo.GetImageDecoders()
        For Each objcodec As ImageCodecInfo In objCodecs
            If objcodec.FormatID = imgFormat.Guid Then
                Return objcodec
            End If
        Next
    End Function
End Class