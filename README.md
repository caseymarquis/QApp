# QApp

QApp is a repo which can be cloned to quickly create a dotnet core web application with a vuejs frontend.

The application can be run locally on .net core, and has the proper configuration to
be easily pushed to Heroku, running from within a docker container.

There's more to it than that, as the application uses a lot of my personal .net libraries for building web applications,
simplifying things like database connections and utilizing a DI/Actor framework I built for standalone .net services.
User login/creating/editing is available, Vuex and VueRouter are already set up and ready to go. Webpack is configured properly
for single file components, changing file names to invalidate caching, and async loading application chunks for speedy performance.
A decent base style is present using bootstrap 4 with the bootswatch 'flatly' theme. There's even a proper Dockerfile for building
both the backend and frontend and deploying them as a unit. Essentially, once you understand what all the pieces are doing,
you're ready to just make something and deploy it within minutes, saving the typical half day or more of setup usually spent
getting a new application ready.

This probably shouldn't be used for an important internet facing production application (the security isn't battle-tested by any means, and has known/accepted XSS vulnerabilities),
but it's perfect for trusted LAN deployments, prototypes, or hobby projects. If you have valuable information to store, guard it with something better
than this!

## Getting Started with dotnet core on your local PC.

0. Install the latest LTS nodejs and the current version of dotnet core if needed.
1. git clone https://github.com/caseymarquis/QApp.git YourProjectName
2. Rename QApp.sln to YourProjectName.sln. It's just neater that way.
3. Delete the .git folder, and then run 'git init'. You could alternately fork QApp and work from there.
3. Install PGSql from https://www.postgresql.org/download/ if you don't already have a local copy.
This will be used for debugging with dotnet core locally.
4. Create a user in PGSql named 'euler', give them enough rights to create a database, and set their password to '3.14159265358979323846264338327'.
Don't use these credentials for a real database! These are just the default connection settings in the file QApp creates. You can change them in that file
to use something else. You can also change the default pretty easily.
5. Run ./_just-cloned-repo.bat, or open it up and run the linux equivalent of what's inside it. It should just be running npm install in the ./App/wwwdev/ directory.
6. Run ./_RunAllTheThings, which will open up everything you need to start hacking away. If on linux, you can get a sense of what it's doing by taking a look inside. It's just for convenience.
7. Visual Studio should now be open. You should, at the very least change the static
AppName field in the AppConfig class found in ./App/App.cs. The database name and program data directory are based on this field, so changing it ensures you don't
end up with two databases called 'QAppExample'.
7. You can then select AppCore as the start up project, and then start running the application.
8. Poke around for a bit in ./App/App.cs. Create some controllers. Call them from the website. Etc.
9. If you don't want to use PGSql, you'll need to delete the migrations folder, and run 'add-migration init' from nuget after configuring Entity Framework for use with a different DB.

## Getting Started with Heroku.

0. Follow the dotnet core instructions above.
0. Install the Heroku CLI if needed.
1. Run 'heroku login' if you haven't already.
2. Navigate to the QApp solution folder in a terminal of some kind.
3. Run 'heroku create heroku-project-name'.
3. Run 'heroku container:login'
3. Run 'heroku addons:create heroku-postgresql:hobby-dev'. This creates a free (limited) database for your Heroku app.
4. Run 'heroku container:push web'. This could take a while the first time.
5. Run 'heroku container:release web'
6. Navigate to https://heroku-project-name.herokuapp.com/
7. Login as the admin user. Whatever you use as a password will be set as the admin password.
8. You're now deployed on Heroku.
9. If you make changes to the app, you can deploy them by running the push/release steps again.
10. If you need to run heroku commands on a new PC, you'll need to run 'git remote add heroku git@heroku.com:heroku-project-name.git' in the directory of the cloned repository.

## Getting Started with .NET Framework.

1. Follow the instructions for dotnet core, minus installing dotnet core.
2. Switch the startup app to AppFramework.
3. If you launch AppFramework.exe with argument "/i", it will install itself as a service. (You'll need an elevated command prompt.)

## Database Migrations

Migrations are a bit weird in the built in efcore database as they are contained in a dotnet standard library. To perform a migration, navigate to ./App, then run the command: `dotnet ef migrations add MigrationName -s ../AppCore`. This tells EfCore to build the dotnet core project in order create the migration files.
