REM  *****  BASIC  *****

Sub Main

End Sub

Sub Html2Trac
	Dim oDoc
	Dim n
	oDoc = ThisComponent
	n = InputBox("Enter a pagename")
	shell "/home/ben/Projects/Html2Trac/html2trac.sh " & convertFromUrl(oDoc.Location) & " " & n

End Sub
