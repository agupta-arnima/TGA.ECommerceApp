setting powershell:

Run the powershell command to set enviornment variable for debug 
# Set environment variables
[System.Environment]::SetEnvironmentVariable("CERT_PATH", "C:\Users\sackumar6\source\repos\TGA.ECommerceApp\Capstone.ECommerceApp.Auth.API\Certs\server.pfx", "User")
[System.Environment]::SetEnvironmentVariable("CERT_PASSWORD", "1234", "User")

# Verify environment variables
Write-Output "CERT_PATH: $env:CERT_PATH"
Write-Output "CERT_PASSWORD: $env:CERT_PASSWORD"