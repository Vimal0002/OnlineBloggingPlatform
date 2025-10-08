# Online E-Blogging Platform

A modern, feature-rich blogging platform built with ASP.NET Core 8.0, featuring beautiful animations, responsive design, and comprehensive blogging functionality.

## 🚀 Features

### Core Functionality
- **User Authentication & Authorization** - Secure registration, login, and user management
- **Blog Post Management** - Create, edit, delete, and publish blog posts
- **Rich Text Editor** - Built-in WYSIWYG editor for content creation
- **Categories & Tags** - Organize content with categories and tags
- **Comments System** - Readers can comment on posts with nested replies
- **Search & Filtering** - Advanced search functionality with category and tag filtering
- **Responsive Design** - Mobile-first design that works on all devices

### Advanced Features
- **Featured Posts** - Highlight important content on the homepage
- **SEO Optimization** - Meta descriptions, friendly URLs, and optimized content structure
- **Image Support** - Featured images for posts and user profiles
- **User Profiles** - Customizable user profiles with bio and profile pictures
- **Dashboard Analytics** - View counts, engagement metrics
- **Draft System** - Save posts as drafts before publishing

### Technical Features
- **Modern UI/UX** - Beautiful animations and smooth transitions
- **Performance Optimized** - Efficient database queries and caching
- **Security** - Built-in security features and data validation
- **Scalable Architecture** - Repository pattern and clean code structure
- **Admin Features** - Content moderation and user management

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, HTML5, CSS3, JavaScript
- **Icons**: Font Awesome 6
- **Fonts**: Google Fonts (Inter + Playfair Display)
- **Architecture Patterns**: Repository Pattern, Dependency Injection

## 📋 Requirements

- .NET 8.0 SDK
- SQL Server (LocalDB is sufficient for development)
- Visual Studio 2022 or VS Code
- Modern web browser

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd OnlineBloggingPlatform
```

### 2. Install .NET 8.0 SDK
Download and install the .NET 8.0 SDK from the official Microsoft website.

### 3. Restore NuGet Packages
```bash
dotnet restore
```

### 4. Update Database Connection String
Update the connection string in `appsettings.json` if needed:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OnlineBloggingPlatform;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 5. Create and Seed Database
The application will automatically create the database and seed initial data on first run.

### 6. Run the Application
```bash
dotnet run
```

### 7. Access the Application
Open your browser and navigate to:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

## 👤 Default Admin Account

A default admin account is created automatically:
- **Email**: admin@blogplatform.com
- **Password**: Admin123!

## 📁 Project Structure

```
OnlineBloggingPlatform/
├── Controllers/           # MVC Controllers
├── Models/               # Data models and entities
├── Views/                # Razor views and layouts
├── ViewModels/           # View models for data binding
├── Data/                 # Database context and configurations
├── Services/             # Business logic services
├── Repositories/         # Data access layer
├── wwwroot/             # Static files (CSS, JS, images)
│   ├── css/             # Custom stylesheets
│   ├── js/              # Custom JavaScript
│   └── images/          # Static images
├── Program.cs           # Application entry point and configuration
├── appsettings.json     # Application configuration
└── README.md            # This file
```

## 🎨 Key Features Walkthrough

### Home Page
- Hero section with call-to-action
- Featured posts carousel
- Category showcase
- Latest posts grid
- Newsletter signup

### User Authentication
- Modern login/register forms
- Password requirements and validation
- Remember me functionality
- Profile management

### Blog Post Creation
- Rich text editor with formatting tools
- Image upload support
- SEO metadata fields
- Draft/publish options
- Category and tag assignment

### Content Management
- Post listing with search and filters
- Category-based browsing
- Tag-based filtering
- User-specific content management

### Responsive Design
- Mobile-first approach
- Smooth animations and transitions
- Interactive UI elements
- Optimized for all screen sizes

## 🔧 Configuration Options

### Application Settings
Configure various aspects of the application in `appsettings.json`:

```json
{
  "ApplicationSettings": {
    "ApplicationName": "Online Blogging Platform",
    "PostsPerPage": 10,
    "MaxTagsPerPost": 10,
    "EnableComments": true,
    "RequireEmailConfirmation": false,
    "MaxImageUploadSize": 5242880,
    "AllowedImageFormats": ["jpg", "jpeg", "png", "gif", "webp"]
  }
}
```

### Email Configuration
Set up email services for notifications:
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "FromEmail": "noreply@blogplatform.com",
    "FromName": "Blog Platform"
  }
}
```

## 🚀 Deployment

### Development
```bash
dotnet run --environment Development
```

### Production
1. Update connection strings for production database
2. Configure email settings
3. Set environment to Production
4. Deploy to your preferred hosting platform (IIS, Azure, AWS, etc.)

```bash
dotnet publish -c Release -o ./publish
```

## 📱 Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🐛 Known Issues

- Rich text editor might need additional styling for complex content
- Image upload currently supports URL input only
- Email functionality requires SMTP configuration

## 🔮 Future Enhancements

- [ ] File upload for images
- [ ] Social media integration
- [ ] Advanced analytics dashboard
- [ ] Multi-language support
- [ ] Theme customization
- [ ] API endpoints for mobile app
- [ ] Advanced user roles and permissions
- [ ] Content scheduling
- [ ] Email notifications
- [ ] Social sharing buttons

## 📞 Support

For support and questions:
- Create an issue in the repository
- Email: support@blogplatform.com

## 🙏 Acknowledgments

- Bootstrap team for the UI framework
- Font Awesome for the icons
- Microsoft for ASP.NET Core
- Google Fonts for typography

---

Built with ❤️ using ASP.NET Core