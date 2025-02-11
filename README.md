The 'TENNIS COACH' is the main file name


Overview


The Tennis Club Application is a web-based platform designed to streamline the management of tennis club activities, including member registration, scheduling, coaching, and enrollment. This application aims to enhance the experience for both club members and coaches, making it easier to organize events and track participation.

Features
User Registration and Login: Members and coaches can create accounts and log in securely.
Schedule Management: Members can view upcoming tennis schedules and enroll in events.
Coach Profiles: Access detailed profiles of coaches, including their qualifications and schedules.
Admin Panel: Admins can manage schedules, coaches, and member accounts through a dedicated interface.

Technologies Used
Frontend: HTML, CSS, Bootstrap
Backend: ASP.NET Core
Database: SQL Server
Authentication: ASP.NET Identity for secure login and user management
Getting Started


To set up the project locally, follow these steps:

Clone the repository:

Terminal
git clone https://github.com/at-matt/Tennisclubapp_App.git
Navigate to the project directory:

Terminal
cd Tennisclubapp_App
Restore the NuGet packages:

Terminal
dotnet restore

Add migration using "Add-Migration yourmessage" command 
Update the database: Ensure your database connection string is configured in appsettings.json, then run the following command to apply migrations:

Terminal
Add-Migration yourmessage
Update-Database


Run the application:
Terminal
dotnet run
 
This is a Teamwork, Thanks to all the team members for their dedicated efforts.
 
