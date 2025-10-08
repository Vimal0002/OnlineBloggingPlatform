# Azure App Service Deployment Script
# Prerequisites: Install Azure CLI (https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)

param(
    [string]$ResourceGroup = "OnlineBlogPlatform-rg",
    [string]$AppName = "onlineblogplatform-$(Get-Random -Minimum 1000 -Maximum 9999)",
    [string]$Location = "East US",
    [string]$PlanName = "OnlineBlogPlatform-plan"
)

Write-Host "🚀 Deploying Online Blogging Platform to Azure..." -ForegroundColor Green

# Login to Azure (uncomment if not logged in)
# az login

# Create resource group
Write-Host "📦 Creating resource group: $ResourceGroup" -ForegroundColor Yellow
az group create --name $ResourceGroup --location $Location

# Create App Service Plan (Free tier for testing)
Write-Host "🏗️ Creating App Service Plan: $PlanName" -ForegroundColor Yellow
az appservice plan create --name $PlanName --resource-group $ResourceGroup --sku F1 --is-linux

# Create Web App
Write-Host "🌐 Creating Web App: $AppName" -ForegroundColor Yellow
az webapp create --resource-group $ResourceGroup --plan $PlanName --name $AppName --runtime "DOTNETCORE:8.0"

# Configure app settings
Write-Host "⚙️ Configuring app settings..." -ForegroundColor Yellow
az webapp config appsettings set --resource-group $ResourceGroup --name $AppName --settings `
    ASPNETCORE_ENVIRONMENT=Production `
    WEBSITE_RUN_FROM_PACKAGE=1

# Deploy the application
Write-Host "📤 Publishing and deploying application..." -ForegroundColor Yellow
dotnet publish --configuration Release --output ./publish
Compress-Archive -Path ./publish/* -DestinationPath ./app.zip -Force

# Deploy zip package
az webapp deployment source config-zip --resource-group $ResourceGroup --name $AppName --src ./app.zip

Write-Host "✅ Deployment completed!" -ForegroundColor Green
Write-Host "🌍 Your blog platform is available at: https://$AppName.azurewebsites.net" -ForegroundColor Cyan
Write-Host "🔐 Admin credentials: admin@blogplatform.com / Admin123!" -ForegroundColor Cyan

# Clean up deployment files
Remove-Item ./publish -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item ./app.zip -Force -ErrorAction SilentlyContinue