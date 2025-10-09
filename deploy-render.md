# Deploy to Render.com

## Quick Deployment Steps:

1. **Visit Render.com** and sign up/login
2. **Connect GitHub Repository:**
   - Go to https://dashboard.render.com
   - Click "New +" → "Web Service"
   - Connect your GitHub account
   - Select repository: `Vimal0002/OnlineBloggingPlatform`

3. **Configure Deployment:**
   - **Name:** `onlinebloggingplatform`
   - **Environment:** `Docker`
   - **Plan:** `Free`
   - **Region:** `Oregon (US West)`

4. **Environment Variables:**
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `ASPNETCORE_URLS` = `http://+:10000`

5. **Deploy:**
   - Click "Create Web Service"
   - Render will automatically build and deploy from your GitHub repo

## Alternative: One-Click Deploy

[![Deploy to Render](https://render.com/images/deploy-to-render-button.svg)](https://render.com/deploy?repo=https://github.com/Vimal0002/OnlineBloggingPlatform)

## Your live URL will be:
`https://onlinebloggingplatform.onrender.com`

## Features:
- ✅ Automatic deployments from GitHub
- ✅ Free HTTPS certificates
- ✅ Custom domains supported
- ✅ Auto-scaling
- ✅ Health checks