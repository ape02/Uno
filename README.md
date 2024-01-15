## EF

~~~bash
dotnet tool update --global dotnet-ef
dotnet ef migrations add --project DAL --startup-project WebApp InitialCreate

dotnet ef database update --project DAL
dotnet ef database drop --project DAL
~~~
