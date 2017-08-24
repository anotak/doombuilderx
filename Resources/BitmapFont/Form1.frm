VERSION 5.00
Begin VB.Form frmChars 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Bitmap Font Generator"
   ClientHeight    =   8910
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   8715
   ClipControls    =   0   'False
   BeginProperty Font 
      Name            =   "Arial"
      Size            =   8.25
      Charset         =   0
      Weight          =   400
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   Icon            =   "Form1.frx":0000
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   594
   ScaleMode       =   3  'Pixel
   ScaleWidth      =   581
   StartUpPosition =   3  'Windows Default
   Begin VB.PictureBox picChar 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00000000&
      BorderStyle     =   0  'None
      CausesValidation=   0   'False
      ClipControls    =   0   'False
      FillColor       =   &H00FFFFFF&
      BeginProperty Font 
         Name            =   "Arial"
         Size            =   24
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00FFFFFF&
      Height          =   510
      Left            =   7890
      ScaleHeight     =   34
      ScaleMode       =   3  'Pixel
      ScaleWidth      =   34
      TabIndex        =   0
      Top             =   60
      Visible         =   0   'False
      Width           =   510
   End
   Begin VB.PictureBox picBitmap 
      AutoRedraw      =   -1  'True
      BackColor       =   &H00000000&
      BorderStyle     =   0  'None
      CausesValidation=   0   'False
      ClipControls    =   0   'False
      FillColor       =   &H00FFFFFF&
      ForeColor       =   &H00FFFFFF&
      Height          =   3840
      Left            =   45
      ScaleHeight     =   256
      ScaleMode       =   3  'Pixel
      ScaleWidth      =   512
      TabIndex        =   2
      Top             =   45
      Width           =   7680
   End
   Begin VB.Label lblChars 
      Caption         =   "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 !@#$%^&*()_+=-[]:;'"",.<>/?\"
      BeginProperty Font 
         Name            =   "Arial Black"
         Size            =   24
         Charset         =   0
         Weight          =   700
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   1095
      Left            =   60
      TabIndex        =   1
      Top             =   7785
      UseMnemonic     =   0   'False
      Visible         =   0   'False
      Width           =   8475
   End
End
Attribute VB_Name = "frmChars"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Long)
Sub DrawChar(Char As String)
     
     picChar.Width = picChar.TextWidth(Char)
     
     DoEvents
     
     picChar.Height = picChar.TextHeight(Char)
     
     DoEvents
          
     Set picChar.Picture = Nothing
     picChar.Cls
     
     picChar.Print Char
     
     DoEvents
End Sub

Private Sub Form_Load()
     Dim i
     Dim CharX As Integer
     Dim CharY As Integer
     Dim LargestY As Integer
     Dim cfg As New clsConfiguration
     Dim settings As Dictionary
     Dim chars As New Dictionary
     
     Show
     Refresh
     
     picChar.Font = lblChars.Font
     picChar.FontSize = lblChars.FontSize
     
     ''Adjustment (in pixels) per character
     Const u1 As Single = -4 '-2
     Const v1 As Single = -4 '-2
     Const u2 As Single = 4 '2
     Const v2 As Single = 5 '5
     
     ''Spacing
     Const sx As Long = 10
     Const sy As Long = 6
     
     ''Offset
     Const ox As Long = 10
     Const oy As Long = 3
     
     ''Start values
     CharY = oy
     CharX = ox
     LargestY = 0
     
     cfg.NewConfiguration
     
     'Render all chars
     For i = 1 To Len(lblChars)
          
          'Get the bitmap char
          DrawChar Mid$(lblChars, i, 1)
          
          If (picChar.Height - 3) > LargestY Then LargestY = (picChar.Height - 3)
          If CharX + picChar.Width >= (picBitmap.Width - sx / 2) Then
               'Go to next character position
               CharY = CharY + LargestY + sy
               LargestY = 0
               CharX = ox
          End If
          
          'Draw on bitmap
          picBitmap.PaintPicture picChar.Image, CharX, CharY - 2
          
          'Make settings
          Set settings = New Dictionary
          settings.Add "width", CLng(picChar.Width)
          settings.Add "height", CLng((picChar.Height - 3))
          settings.Add "u1", (CSng(CharX) + u1) / CSng(picBitmap.Width)
          settings.Add "v1", (CSng(CharY) + v1) / CSng(picBitmap.Height)
          settings.Add "u2", (CSng(CharX + picChar.Width) + u2) / CSng(picBitmap.Width)
          settings.Add "v2", (CSng(CharY + (picChar.Height - 3)) + v2) / CSng(picBitmap.Height)
          chars.Add Asc(Mid$(lblChars, i, 1)), settings
          Set settings = Nothing
          
          'Go to next character position
          CharX = CharX + picChar.Width + sx
     Next i
     
     'save bitmap
     SavePicture picBitmap.Image, "font.bmp"
     
     'save config
     cfg.WriteSetting "count", CLng(Len(lblChars))
     cfg.WriteSetting "chars", chars, True
     cfg.SaveConfiguration "font.cfg"
     
End Sub

