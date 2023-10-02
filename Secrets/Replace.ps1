# Define the source code file path and the destination file path
$sourceFile = "Secrets.Template.cs"
$destinationFile = "Secrets2.cs"


# Read the contents of the original file
$content = Get-Content $sourceFile

# Find all placeholders and replace them with the corresponding environment variable value
$content | ForEach-Object {
    if ($_ -match '%%(.+?)%%') {
        $envVar = $Matches[1]
        $envVal = [Environment]::GetEnvironmentVariable($envVar)
        if ($null -eq $envVal) {
            Write-Warning "No environment variable found for placeholder: $envVar"
            $_
        } else {
            $_ -replace "%%$envVar%%", $envVal
        }
    } else {
        $_
    }
} | Set-Content $destinationFile
