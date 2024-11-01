# First get all .proto files
$protoFiles = Get-ChildItem -Path "./src/Athena.SDK/protos" -Filter "*.proto" -Recurse | Select-Object -ExpandProperty Name

# Create output directory if it doesn't exist
$outputDir = "./src/Kronos.WebAPI/gen"
New-Item -ItemType Directory -Force -Path $outputDir

# Generate C# files for each .proto file
foreach ($protoFile in $protoFiles) {
    Write-Host "Generating C# code for: $protoFile"
    
    ./tools/protoc/bin/protoc --csharp_out=$outputDir `
	   --plugin=protoc-gen-grpc=./tools/protoc/bin/grpc_csharp_plugin.exe `
 	   --grpc_out=$outputDir `
           --proto_path=./src/Athena.SDK/protos $protoFile
}