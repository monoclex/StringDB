(
	jekyll build
	del /f /q ".\_site\build.bat"
	echo all | xcopy /E ".\_site" ".\"
)