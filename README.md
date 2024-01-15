## Welcome to uno game!

To play, choose to run Program.cs in Uno Items or WebApp project.
The game has common engine and ability to save game for web and so for console app.
Also added local save support.


## EF

~~~bash
dotnet tool update --global dotnet-ef
dotnet ef migrations add --project DAL --startup-project WebApp InitialCreate

dotnet ef database update --project DAL
dotnet ef database drop --project DAL
~~~
