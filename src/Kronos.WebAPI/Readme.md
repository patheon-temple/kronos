dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate --project .\src\Kronos.WebAPI\

dotnet dev-certs https -ep ../.aspnet/https/aspnetapp.pfx --format PFX -p password -t