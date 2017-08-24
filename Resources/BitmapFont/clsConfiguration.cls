VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsConfiguration"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' ====================================================================================
'    INTRODUCTION
' ====================================================================================
'
'    Configuration Class Module by Pascal vd Heiden, www.codeimp.com
'
'    This Class module contains code of my design. You are free to use it,
'    as long as you do not remove my details up here in this comment. Thanks.
'
'    This Class module requires "Microsoft Scripting Runtime" reference (scrrun.dll)
'    for the Dictionary object. Select this in the Project -> References dialog.
'
'    Can safely be compiled with Fast Optimization and
'    all Advanced Optimization switches checked.
'
'
' ====================================================================================
'    CONFIGURATION FILE STRUCTURE SYNTAX
' ====================================================================================
'
'    Whitepace is always allowed. This includes spaces, tabs
'    linefeeds (10) and carriage returns (13)
'
'    Keys may not have spaces or assignment operator = in them.
'
'    Comments start with // (unless used within strings)
'    and count as comment for the rest of the line. Or use /* and */
'    to mark the beginning and end of a comment.
'
'    Simple setting:
'
'              key = value;
'
'    Example:  speed = 345;
'              cars = 8;
'
'    Strings must be in quotes.
'
'    Example:  nickname = "Gherkin";
'              altnick = "Gherk inn";
'
'    String Escape Sequences:
'        \n    New line (10)
'        \r    Carriage return (13)
'        \t    Tab (9)
'        \"    Double-quotation mark
'        \\    Backslash
'        \000  Any ASCII character (MUST be 3 digits! So for 13 you use \013)
'
'    Decimals ALWAYS use a dot, NEVER comma!
'
'    Example:  pressure = 15.29;
'              acceleration = 1.0023;
'
'    Structures must use brackets.
'
'    Structure Example:
'
'              key
'              {
'                   key = value;
'                   key = value;
'
'                   key
'                   {
'                        key = value;
'                        key = value;
'                        key = value;
'                   }
'
'                   key = value;
'                   key = value;
'                   key = value;
'                   key = value;
'                   key = value;
'              }
'
'    As you can see, structures inside structures are allowed
'    and you may go as deep as you want. Note that only the root structures
'    can be readed from config using ReadSetting. ReadSetting will return a
'    Dictionary object containing everything in that root structure.
'
'    Key names must be unique within their scope.
'
'    This is NOT allowed, it may not have 'father' more
'    than once in the same scope:
'
'              mother = 45;
'              father = 52;
'
'              father
'              {
'                   length = 1.87;
'              }
'
'    This however is allowed, because father
'    now exists in a different scope:
'
'              mother = 45;
'              father = 52;
'
'              parents
'              {
'                   father = 52;
'              }
'
'    This too is allowed, both 'age' are in a different scope:
'
'              mother
'              {
'                   age = 45;
'              }
'
'              father
'              {
'                   age = 52;
'              }
'
'
' ====================================================================================
'    FUNCTION CALL DESCRIPTIONS
' ====================================================================================
'
'    InputConfiguration
'
'    This loads a configuration from a string. The string must contain a
'    configuration using the rules described above.
'
' ------------------------------------------------------------------------------------
'
'    LoadConfiguration
'
'    Loads a configuration from a file. The file must contain a configuration
'    using the rules described above.
'
' ------------------------------------------------------------------------------------
'
'    NewConfiguration
'
'    Erases all loaded settings and starts with a new, clear configuration
'
' ------------------------------------------------------------------------------------
'
'    OutputConfiguration
'
'    Returns the configuration as a string following the rules described above.
'    You can optionally determine the newline character to use and/or omit
'    whitespace in the result.
'
' ------------------------------------------------------------------------------------
'
'    SaveConfiguration
'
'    Writes the configuration to file following the rules described above.
'    You can optionally specify the newline character to use and/or omit
'    whitespace in the result.
'
' ------------------------------------------------------------------------------------
'
'    ReadSetting
'
'    Reads a setting from the root level and returns it in its own variable type.
'    You can optionally specify a default to return if the specified setting
'    does not exist and/or specify if you would like to get a reference if the
'    setting is a Dictionary object (structure in configuration).
'    If you use a reference, you can change the configuration immediately
'    through that reference.
'
' ------------------------------------------------------------------------------------
'
'    WriteSetting
'
'    Writes a setting to the root level in the configuration.
'    You can choose to write the setting as reference if it is of Dictionary type.
'    Note that your configuration changes too when you change anything
'    in your Dictionary when it is written as reference!
'
' ------------------------------------------------------------------------------------
'
'    RemoveSetting
'
'    Removes a setting from the root level in the configuration.
'    Its as simple as that. What else is there to tell about it?!
'
' ------------------------------------------------------------------------------------
'
'    Root
'
'    Returns the entire configuration (root level) as a Dictionary object.
'    You can optionally specify to return a reference through which you can change
'    the configuration immediately.
'
' ====================================================================================
' ====================================================================================



'Do not allow any undeclared variables
Option Explicit

'Case sensitive comparisions
Option Compare Binary

Private Const ASSIGNOP As Long = 61          ' =
Private Const BEGINOP As Long = 123          ' {
Private Const ENDOP As Long = 125            ' }
Private Const TERMINATEOP As Long = 59       ' ;
Private Const STRINGOP As Long = 34          ' "
Private Const COMMENT As String = "//"       '
Private Const BEGINCOMMENT As String = "/*"  '
Private Const ENDCOMMENT As String = "*/"    '

Private Const WS_SPACE As Long = 32          ' space
Private Const WS_LINEFEED As Long = 10       ' linefeed
Private Const WS_TAB As String = vbTab       '
Private Const WS_RETURN As String = vbCr     '

Private Const ESCAPESEQ As Long = 92         ' \
Private Const ES_NEWLINE As Long = 110       ' n
Private Const ES_RETURN As Long = 114        ' r
Private Const ES_TAB As Long = 116           ' t
Private Const ES_QUOTE As Long = 34          ' "
Private Const ES_BACKSLASH As Long = 92      ' \

'This will hold the object orientated configuration
Private Config As Dictionary

'Last line that was read where error occurred
Private cLastReadLine As Long

Public Property Get CurrentScanLine() As Long
     CurrentScanLine = cLastReadLine
End Property

Private Sub Class_Initialize()
     
     'New database
     Set Config = New Dictionary
End Sub

Private Sub Class_Terminate()
     
     'Clean up
     Set Config = Nothing
End Sub

Private Function DeepCopy(ByRef Dict As Dictionary) As Dictionary
     Dim NewConfig As Dictionary
     Dim CopyObject As Dictionary
     Dim ConfigKeys As Variant
     Dim ConfigValue As Variant
     Dim i As Long
     
     'Create new config
     Set NewConfig = New Dictionary
     
     'Add all items from Config
     If Dict.Count Then
          ConfigKeys = Dict.Keys
          For i = LBound(ConfigKeys) To UBound(ConfigKeys)
               
               'Check if the value is a dictionary
               If VarType(Dict(ConfigKeys(i))) = vbObject Then
                    
                    'Get the object
                    Set CopyObject = Dict(ConfigKeys(i))
                    
                    'Deepcopy this too
                    NewConfig.Add ConfigKeys(i), DeepCopy(CopyObject)
                    
                    'Clean up
                    Set CopyObject = Nothing
               Else
                    
                    'Normal copy
                    ConfigValue = Dict(ConfigKeys(i))
                    NewConfig.Add ConfigKeys(i), ConfigValue
               End If
          Next i
     End If
     
     'Return the result
     Set DeepCopy = NewConfig
     
     'Clean up
     Set NewConfig = Nothing
End Function

Private Function Escaped(ByRef Value As String) As String
     
     '    \n New line
     '    \r Carriage return
     '    \t Tab
     '    \" Double quotation mark
     '    \\ Backslash
     
     'Copy string
     Escaped = Value
     
     'Replace characters with escape sequences
     Escaped = Replace$(Escaped, "\", "\\")       'Note the \ must be the first to replace!
     Escaped = Replace$(Escaped, vbLf, "\n")
     Escaped = Replace$(Escaped, vbCr, "\r")
     Escaped = Replace$(Escaped, vbTab, "\t")
     Escaped = Replace$(Escaped, """", "\""")
End Function

Public Sub InputConfiguration(ByVal Configuration As String)
     
     
     'This reads the data and builds a new dictionary object
     
     
     'Remove Returns and Tabs so we only have Linefeed as newline
     Configuration = Replace$(Configuration, WS_RETURN, "")
     Configuration = Replace$(Configuration, WS_TAB, "")
     
     
     'First line
     cLastReadLine = 1
     
     'Load main structure
     Set Config = LoadStructure(0, Configuration)
End Sub

Public Sub LoadConfiguration(ByRef Filename As String)
     Dim FB As Integer        'File buffer
     Dim Data As String       'Data
     
     
     'This reads a file and builds a new dictionary object
     
     
     'Open the file to read
     FB = FreeFile
     Open Filename For Binary As #FB
     
     'Read all data
     Data = Space$(LOF(FB))
     Get #FB, 1, Data
     
     'Close file
     Close #FB
     
     
     'Remove Returns and Tabs so we only have Linefeed as newline
     Data = Replace$(Data, WS_RETURN, "")
     Data = Replace$(Data, WS_TAB, "")
     
     
     'First line
     cLastReadLine = 1
     
     'Load main structure
     Set Config = LoadStructure(0, Data)
End Sub

Private Function LoadStructure(ByRef p As Long, ByRef Data As String) As Dictionary
     Dim c As String * 1           'Character at p
     Dim ca As Long                'ASCII value of c
     Dim np As Long                'Next position
     Dim StringData As Boolean     'True if in a string
     Dim NumberData As Boolean     'True if in a number
     Dim Sequence As Boolean       'True if getting a sequence character
     Dim Assigning As Boolean      'True when assigning
     Dim NewKey As String
     Dim NewValue As String
     
     'Create dictionary
     Set LoadStructure = New Dictionary
     
     'Continue until end of data
     Do While p < Len(Data)
          
          'Next char
          p = p + 1
          
          'Get char
          c = Mid$(Data, p, 1)
          ca = AscW(c)
          
          'Check if we are processing number data
          If NumberData Then
               
               'Check if assignment ends
               If ca = TERMINATEOP Then
                    
                    'Check number type
                    If InStr(NewValue, ".") <> 0 Then
                         
                         'Add the number to dictionary as single
                         LoadStructure.Add Trim$(NewKey), CSng(Val(NewValue))
                    Else
                         
                         'Add the number to dictionary as long
                         LoadStructure.Add Trim$(NewKey), CLng(NewValue)
                    End If
                    
                    'Reset
                    NewKey = ""
                    NewValue = ""
                    
                    'End of assign
                    NumberData = False
                    Assigning = False
               
               'Check if newline
               ElseIf (ca = WS_LINEFEED) Then
                    
                    'Count the new line
                    cLastReadLine = cLastReadLine + 1
               Else
                    
                    'Add to value
                    NewValue = NewValue & c
               End If
               
          'Check if we are processing string data
          ElseIf StringData Then
               
               'Check if previous char was a slash
               If Sequence Then
                    
                    'Check the char
                    Select Case ca
                         Case ES_BACKSLASH: NewValue = NewValue & "\"
                         Case ES_NEWLINE: NewValue = NewValue & vbLf
                         Case ES_QUOTE: NewValue = NewValue & """"
                         Case ES_RETURN: NewValue = NewValue & vbCr
                         Case ES_TAB: NewValue = NewValue & vbTab
                         Case Else
                              
                              'Check if its a number
                              If IsNumeric(c) Then
                                   
                                   'Always 3 chars
                                   np = CLng(Mid$(Data, p, 3))
                                   NewValue = NewValue & ChrW$(np)
                                   p = p + 2
                              Else
                                   
                                   'Add character
                                   NewValue = NewValue & c
                              End If
                    End Select
                    
                    'End of sequence
                    Sequence = False
               Else
                    
                    'Check if sequence start
                    If ca = ESCAPESEQ Then
                         
                         'Start escape sequence
                         Sequence = True
                         
                    'Check if string ends
                    ElseIf ca = STRINGOP Then
                         
                         'Add the string to dictionary
                         LoadStructure.Add Trim$(NewKey), NewValue
                         
                         'End of string
                         StringData = False
                         
                         'Reset
                         NewKey = ""
                         NewValue = ""
                    
                    'Check if newline
                    ElseIf (ca = WS_LINEFEED) Then
                         
                         'Count the new line
                         cLastReadLine = cLastReadLine + 1
                         
                    Else
                         
                         'Add to string
                         NewValue = NewValue & c
                    End If
               End If
               
          'Check if assigning
          ElseIf Assigning Then
               
               'Check for STRINGOP or Numeric character
               If (ca = STRINGOP) Then
                    
                    'Begin string data here
                    StringData = True
                    
               ElseIf (IsNumeric(c) = True) Or (c = "-") Or (c = ".") Or (LCase$(c) = "e") Or (c = "&") Then
                    
                    'Begin numeric data here
                    NumberData = True
                    
                    'Note that this byte is part of the value
                    p = p - 1
               
               'Check if newline
               ElseIf (ca = WS_LINEFEED) Then
                    
                    'Count the new line
                    cLastReadLine = cLastReadLine + 1
                    
               'Check if assignment ends
               ElseIf (ca = TERMINATEOP) Then
                    
                    'End of assign
                    Assigning = False
                    
               'Everything else but spaces are not allowed
               ElseIf (ca <> WS_SPACE) Then
                    
                    'Invalid assignment
                    Err.Raise vbObjectError, , "Invalid assignment. Forgot an assignment terminator?"
               End If
               
          'Anything else
          Else 'If (ca <> WS_SPACE) Then
               
               'Check for a Key, BEGINOP, ENDOP or ASSIGNOP, COMMENT or whitespace
               Select Case ca
                    Case BEGINOP
                         
                         'Check for spaces in key name
                         If (InStr(Trim$(NewKey), " ") > 0) Then
                              
                              'Spaces not allowed in key names
                              Err.Raise vbObjectError, , "Spaces not allowed in key names."
                         Else
                              
                              'Add structure
                              LoadStructure.Add Trim$(NewKey), LoadStructure(p, Data)
                              
                              'Reset
                              NewKey = ""
                         End If
                         
                    Case ENDOP
                         
                         'Leave here
                         Exit Do
                         
                    Case ASSIGNOP
                         
                         'Check for spaces in key name
                         If (InStr(Trim$(NewKey), Chr$(WS_SPACE)) > 0) Then
                              
                              'Spaces not allowed in key names
                              Err.Raise vbObjectError, , "Spaces not allowed in key names."
                         Else
                              
                              'Now assigning
                              Assigning = True
                         End If
                    
                    Case TERMINATEOP
                         
                         'Add the key to dictionary with 0 value
                         LoadStructure.Add Trim$(NewKey), CLng(0)
                         
                         'Reset
                         NewKey = ""
                         NewValue = ""
                         
                    Case WS_LINEFEED
                         
                         'Count the new line
                         cLastReadLine = cLastReadLine + 1
                         
                         'Add as space
                         NewKey = NewKey & Chr$(WS_SPACE)
                         
                    'Check for possible comment
                    Case AscW(COMMENT), AscW(BEGINCOMMENT)
                         
                         'Check for 2 bytes line comment
                         If Mid$(Data, p, 2) = COMMENT Then
                              
                              'Find the next linefeed
                              np = InStr(p, Data, vbLf)
                              
                              'Check if linefeed was found
                              If np > 0 Then
                                   
                                   'Count linefeed
                                   cLastReadLine = cLastReadLine + 1
                                   
                                   'Skip to next
                                   p = np
                              Else
                                   
                                   'No linefeed can be found, end of file!
                                   p = Len(Data)
                                   Exit Do
                              End If
                              
                         'Check for 2 bytes block comment
                         ElseIf Mid$(Data, p, 2) = BEGINCOMMENT Then
                              
                              'Find the next endcomment
                              np = InStr(p, Data, ENDCOMMENT)
                              
                              'Check if endcomment was found
                              If np > 0 Then
                                   
                                   'Count the number of linefeeds in comment block
                                   cLastReadLine = cLastReadLine + UBound(Split(Mid$(Data, p, np - p), Chr$(WS_LINEFEED)))
                                   
                                   'Skip to next
                                   p = np + 1
                              Else
                                   
                                   'No endcomment can be found, end of file!
                                   p = Len(Data)
                                   Exit Do
                              End If
                         End If
                         
                    'Add to key name
                    Case Else: NewKey = NewKey & c
                    
               End Select
          End If
     Loop
End Function

Public Sub NewConfiguration()
     
     'First line
     cLastReadLine = 1
     
     'Create new, empty dictionary
     Set Config = New Dictionary
End Sub

Public Function OutputConfiguration(Optional ByVal NewLine As String = vbCrLf, Optional ByVal Whitespace As Boolean = True) As String
     
     'Create configuration as string
     OutputConfiguration = OutputDictionary(Config, 0, NewLine, Whitespace)
End Function

Private Function OutputDictionary(ByRef Dict As Dictionary, ByVal Level As Long, Optional ByVal NewLine = vbCrLf, Optional ByVal Whitespace As Boolean = True) As String
     Dim LevelTabs As String
     Dim sp As String
     Dim Keys As Variant
     Dim Data As String
     Dim i As Long
     
     'Check if this dictionary is not empty
     If Dict.Count > 0 Then
          
          'Create whitespace
          If Whitespace Then
               LevelTabs = String$(Level, vbTab)
               sp = " "
          End If
          
          'Get the keys
          Keys = Dict.Keys
          
          'Go for all keys in dictionary
          For i = LBound(Keys) To UBound(Keys)
               
               'Check type of value
               Select Case VarType(Dict(Keys(i)))
                    
                    'Dictionary Object
                    Case vbObject
                         
                         'Output empty line
                         If Whitespace Then Data = Data & LevelTabs & NewLine
                         
                         'Output the key
                         Data = Data & LevelTabs & Keys(i) & NewLine
                         
                         'Ouput the BEGINOP
                         Data = Data & LevelTabs & ChrW$(BEGINOP) & NewLine
                         
                         'Output Dictionary
                         Data = Data & OutputDictionary(Dict(Keys(i)), Level + 1, NewLine, Whitespace)
                         
                         'Output the ENDOP
                         Data = Data & LevelTabs & ChrW$(ENDOP) & NewLine
                         
                         'Output empty line
                         If Whitespace Then Data = Data & LevelTabs & NewLine
                         
                    'Integral Number
                    Case vbInteger, vbLong, vbByte
                         
                         'Output the key = value;
                         Data = Data & LevelTabs & Keys(i) & sp & ChrW$(ASSIGNOP) & sp & Dict(Keys(i)) & ChrW$(TERMINATEOP) & NewLine
                         
                    'Floating point Number
                    Case vbSingle, vbDouble, vbCurrency, vbDecimal
                         
                         'Output the key = value;
                         Data = Data & LevelTabs & Keys(i) & sp & ChrW$(ASSIGNOP) & sp & Format(Dict(Keys(i)), "###############################0.0#####") & "f" & ChrW$(TERMINATEOP) & NewLine
                         
                    'Boolean as Number
                    Case vbBoolean
                         
                         'Output the key = value;
                         Data = Data & LevelTabs & Keys(i) & sp & ChrW$(ASSIGNOP) & sp & CLng(Dict(Keys(i))) & ChrW$(TERMINATEOP) & NewLine
                         
                    'Other (String)
                    Case Else
                         
                         'Output the key = "value";
                         Data = Data & LevelTabs & Keys(i) & sp & ChrW$(ASSIGNOP) & sp & ChrW$(STRINGOP) & Escaped(Dict(Keys(i))) & ChrW$(STRINGOP) & ChrW$(TERMINATEOP) & NewLine
                         
               End Select
          Next i
     End If
     
     'Return data
     OutputDictionary = Data
End Function

Public Function ReadSetting(ByRef Setting As String, Optional ByRef Default As Variant, Optional ByVal Reference As Boolean)
     
     'Check if setting exists
     If Config.Exists(Setting) Then
          
          'Check setting type
          If VarType(Config(Setting)) = vbObject Then
               
               'Check if we should return a reference
               If Reference Then
                    
                    'Return a reference
                    Set ReadSetting = Config(Setting)
               Else
                    
                    'Return the setting
                    Set ReadSetting = DeepCopy(Config(Setting))
               End If
          Else
               
               'Return the setting
               ReadSetting = Config(Setting)
          End If
     Else
          
          'Return the default
          If VarType(Default) = vbObject Then Set ReadSetting = Default Else ReadSetting = Default
     End If
End Function

Public Sub RemoveSetting(ByRef Setting As String)
     
     'Remove setting if exists
     If Config.Exists(Setting) Then Config.Remove Setting
End Sub

Public Function Root(Optional ByVal Reference As Boolean) As Dictionary
     
     'Check if we should return a reference
     If Reference Then
          
          'Return a reference
          Set Root = Config
     Else
          
          'Return the setting
          Set Root = DeepCopy(Config)
     End If
End Function

Public Sub SaveConfiguration(ByRef Filename As String, Optional ByVal NewLine As String = vbCrLf, Optional ByVal Whitespace As Boolean = True)
     Dim FB As Integer        'File buffer
     Dim Data As String       'Data
     
     
     'This reads a file and builds a new dictionary object
     
     
     'Create data
     Data = OutputDictionary(Config, 0, NewLine, Whitespace)
     
     
     'Kill the file if exists
     If Dir(Filename) <> "" Then Kill Filename
     
     'Open the file to write
     FB = FreeFile
     Open Filename For Binary As #FB
     
     'Write configuration data
     Put #FB, 1, Data
     
     'Close file
     Close #FB
End Sub

Public Sub WriteSetting(ByRef Setting As String, ByRef Value As Variant, Optional ByVal Reference As Boolean)
     Dim DictValue As Dictionary
     
     'Check if the setting exists
     If Config.Exists(Setting) Then
          
          'Check type of value
          If VarType(Value) = vbObject Then
               
               'Check if we should apply referenced
               If Reference Then
                    
                    'Apply setting as reference
                    Set Config(Setting) = Value
               Else
                    
                    'Apply setting
                    Set DictValue = Value
                    Set Config(Setting) = DeepCopy(DictValue)
                    
                    'Clean up
                    Set DictValue = Nothing
               End If
          Else
               
               'Apply setting
               Config(Setting) = Value
          End If
     Else
          
          'Add setting
          Config.Add Setting, Value
     End If
End Sub
