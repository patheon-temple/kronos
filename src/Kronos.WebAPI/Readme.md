dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate --project .\src\Kronos.WebAPI\
dotnet ef database update --project .\src\Kronos.WebAPI\ --startup-project .\src\Kronos.WebAPI\
