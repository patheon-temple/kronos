dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate --project .\src\Kronos.WebAPI\ --context AthenaDbContext --output-dir Migrations/Athena
dotnet ef migrations add InitialCreate --project .\src\Kronos.WebAPI\ --context HermesDbContext --output-dir Migrations/Hermes

dotnet dev-certs https -ep ../.aspnet/https/aspnetapp.pfx --format PFX -p password -t