$packageVersion = "2.2.1";
$sharedDescription = "Codon is a zero-dependency cross-platform MVVM framework for creating UWP, WPF, and Xamarin applications. It provides much of what you need to rapidly create sophisticated yet maintainable applications.";
$iconUrl = "https://codonframework.github.io/External/Images/CodonLogo_128x128.png";
$licenseUrl = "https://codonframework.github.io/External/License.txt";
$nugetLocalFeedDirectory = "C:\Dev\NugetLocal";

# These are set later in the script.
#$nugetExePath = "C:\Dev\Tools\Nuget.exe";

$sourceRoot = "../../Source/Framework/";
$nuspecFiles = @(cls

	"Codon.Essentials.nuspec",
	"../../Source/Framework/Codon/Codon.nuspec",
	"../../Source/Framework/Platforms/Android/Codon.Platform/Codon.Platform.Android.nuspec",	
	"../../Source/Framework/Platforms/Ios/Codon.Platform/Codon.Platform.Ios.nuspec",	
	"../../Source/Framework/Platforms/Uwp/Codon.Platform/Codon.Platform.Uwp.nuspec",
	"../../Source/Framework/Platforms/Wpf/Codon.Platform/Codon.Platform.Wpf.nuspec",
	"Codon.Extras.nuspec",
	"../../Source/Framework/Codon.Extras/Codon.Extras.Core.nuspec",
	"../../Source/Framework/Platforms/Android/Codon.Extras.Platform/Codon.Extras.Platform.Android.nuspec",	
	"../../Source/Framework/Platforms/Ios/Codon.Extras.Platform/Codon.Extras.Platform.Ios.nuspec",	
	"../../Source/Framework/Platforms/Uwp/Codon.Extras.Platform/Codon.Extras.Platform.Uwp.nuspec",
	"../../Source/Framework/Platforms/Wpf/Codon.Extras.Platform/Codon.Extras.Platform.Wpf.nuspec",
	"../../Source/Framework/Codon.UI.Data/Codon.UI.Data.nuspec",
	"../../Source/Framework/Codon.UndoModel/Codon.UndoModel.nuspec")

$assemblyToEnsureJustBuilt = @(
	"../../Source/Framework/Platforms/Android/Codon.Platform/bin/Release/Codon.Platform.dll",
	"../../Source/Framework/Codon/bin/Release/netstandard1.4/Codon.dll",
	"../../Source/Framework/Platforms/Ios/Codon.Platform/bin/Release/Codon.Platform.dll",
	"../../Source/Framework/Platforms/Uwp/Codon.Platform/bin/Release/Codon.Platform.dll",
	"../../Source/Framework/Platforms/Wpf/Codon.Platform/bin/Release/Codon.Platform.dll",
	"../../Source/Framework/Platforms/Android/Codon.Extras.Platform/bin/Release/Codon.Extras.Platform.dll",
	"../../Source/Framework/Codon.Extras/bin/Release/netstandard1.4/Codon.Extras.dll",
	"../../Source/Framework/Platforms/Ios/Codon.Extras.Platform/bin/Release/Codon.Extras.Platform.dll",
	"../../Source/Framework/Platforms/Uwp/Codon.Extras.Platform/bin/Release/Codon.Extras.Platform.dll",
	"../../Source/Framework/Platforms/Wpf/Codon.Extras.Platform/bin/Release/Codon.Extras.Platform.dll",
	"../../Source/Framework/Codon.UI.Data/bin/Release/netstandard1.4/Codon.UI.Data.dll",
	"../../Source/Framework/Codon.UndoModel/bin/Release/netstandard1.4/Codon.UndoModel.dll")

$mustBeBuiltAfter = [DateTime]::Now.AddMinutes(-15)

foreach ($assembly in $assemblyToEnsureJustBuilt)
{
	$filePath = $PSScriptRoot + "/" + $assembly
	Write-Host "Resolving assembly at $assembly "
	$file = Get-ChildItem -Path $filePath

	if ($file.LastWriteTime -lt $mustBeBuiltAfter)
	{
		throw "Assembly was not built recently enough. " + $file.FullName
		Exit
	}
}

function Pack-NuGetFiles
{
  Param
  (
	[string]$PackageVersion = "1.0.J.B",
	[string]$NugetPath = "C:\Dev\Tools\Nuget.exe",
	[string]$localNugetDirectory = "C:\Dev\NugetLocal"
  )
	 
	$buildNumber = $env:TF_BUILD_BUILDNUMBER
	if ($buildNumber -eq $null)
	{
		$buildIncrementalNumber = 0
	}
	else
	{
		$splitted = $buildNumber.Split('.')
		$buildIncrementalNumber = $splitted[$splitted.Length - 1]
	}
	 
	Write-Host "Executing Publish-NugetPackage in path $SrcPath, PackageVersion is $PackageVersion"

	$jan1 = Get-Date 1/1
	$today = Get-Date
	$jdate = ($today - $jan1).days
	$buildIncrementalNumber = $today.Hour * 60 + $today.Minute
	#$julian.ToString("000")
	 
	#$jdate = Get-JulianDate
	$PackageVersion = $PackageVersion.Replace("J", $jdate).Replace("B", $buildIncrementalNumber)
	 
	Write-Host "Transformed PackageVersion is $PackageVersion "
  
	$AllNuspecFiles = $nuspecFiles 
			# Get-ChildItem $SrcPath*.nuspec
   
	#Remove all previous packed packages in the directory
	  
	$AllNugetPackageFiles = Get-ChildItem $SrcPath*.nupkg
   
	foreach ($file in $AllNugetPackageFiles)
	{ 
		Remove-Item $file
	}

	#$scriptPath = $PSScriptRoot
	#$scriptDir = $PSScriptRoot
 
	foreach ($fileName in $AllNuspecFiles)
	{ 
		#Write-Host "Modifying file " + $file.FullName
		#save the file for restore
		#$backFile = $file.FullName + "._ORI"
		#$tempFile = $file.FullName + ".tmp"
		#Copy-Item $file.FullName $backFile -Force
		#now load all content of the original file and rewrite modified to the same file
		#Get-Content $file.FullName |
		#%{$_ -replace '<version>[0-9]+(.([0-9]+|*)){1,3}</version>', "<version>$PackageVersion</version>" } > $tempFile
		#Move-Item $tempFile $file.FullName -force

		$filePath = $PSScriptRoot + "/" + $fileName
		$file = Get-ChildItem -Path $filePath
 
		
		#Write-Host "Packing: `"$file`"";

		#Create the .nupkg from the nuspec file
		$ps = new-object System.Diagnostics.Process
		$ps.StartInfo.Filename = "$NugetPath"
		$ps.StartInfo.Arguments = "pack `"$file`" -version $PackageVersion -Properties Configuration=release;IconUrl=`"$iconUrl`";Desc=`"$sharedDescription`";Authors=`"Daniel Vaughan`";LicenseUrl=`"$licenseUrl`";Owners=`"Daniel Vaughan`""
		$ps.StartInfo.WorkingDirectory = $file.Directory.FullName
		$ps.StartInfo.RedirectStandardOutput = $True
		$ps.StartInfo.RedirectStandardError = $True
		$ps.StartInfo.UseShellExecute = $false
		$ps.start()
		if(!$ps.WaitForExit(30000)) 
		{
			$ps.Kill()
		}
		[string] $Out = $ps.StandardOutput.ReadToEnd();
		[string] $ErrOut = $ps.StandardError.ReadToEnd();
		#Write-Host "Nuget pack Output of commandline " + $ps.StartInfo.Filename + " " + $ps.StartInfo.Arguments
		Write-Host $Out
		if ($ErrOut -ne "") 
		{
			Write-Error "Nuget pack Errors $filePath"
			Write-Error $ErrOut
		}
		else
		{
			$pathQuery = $file.Directory.ToString() + "/*" + $PackageVersion + ".nupkg"
			$packageFile = Get-ChildItem -Path $pathQuery
			$newPath = $localNugetDirectory + "/" + $packageFile.Name

			Move-Item $packageFile $newPath -force
		}
		#Restore original file
		#Move-Item $backFile $file -Force

	}
	 
	$AllNugetPackageFiles = Get-ChildItem $SrcPath*.nupkg
}

Invoke-Item $nugetLocalFeedDirectory
Pack-NuGetFiles $packageVersion

