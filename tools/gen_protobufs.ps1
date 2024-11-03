# Define SDK paths array
$sdkPaths = @(
    "./src/Athena.SDK",
    "./src/Hermes.SDK"
)

# Tool paths - using Windows path style
$protocPath = ".\tools\protoc\bin\protoc.exe"
$grpcPluginPath = ".\tools\protoc\bin\grpc_csharp_plugin.exe"

# Output directories
$csharpOutputDir = ".\src\Kronos.WebAPI\gen"
$tsOutputDir = ".\src\kronos-vue\src\gen"

# Verify tools exist
if (-not (Test-Path $protocPath)) {
    Write-Error "protoc.exe not found at: $protocPath"
    exit 1
}
if (-not (Test-Path $grpcPluginPath)) {
    Write-Error "grpc_csharp_plugin.exe not found at: $grpcPluginPath"
    exit 1
}

# Convert to absolute paths
$protocPath = (Get-Item $protocPath).FullName
$grpcPluginPath = (Get-Item $grpcPluginPath).FullName
$csharpOutputDir = (New-Item -ItemType Directory -Force -Path $csharpOutputDir).FullName
$tsOutputDir = (New-Item -ItemType Directory -Force -Path $tsOutputDir).FullName

# First ensure @protobuf-ts/plugin is installed
Write-Host "Installing @protobuf-ts/plugin..."
Push-Location ".\src\kronos-vue"
npm install --save-dev @protobuf-ts/plugin
$tsPluginPath = ".\node_modules\.bin\protoc-gen-ts.cmd"
if (-not (Test-Path $tsPluginPath)) {
    Write-Error "@protobuf-ts/plugin not found. Please ensure it's installed correctly."
    exit 1
}
$tsPluginPath = (Get-Item $tsPluginPath).FullName
Pop-Location

# Process each SDK path
foreach ($sdkPath in $sdkPaths) {
    # Skip if SDK path doesn't exist
    if (-not (Test-Path "$sdkPath\protos")) {
        Write-Warning "Protos directory not found at: $sdkPath\protos"
        continue
    }

    # Get all .proto files for current SDK
    $protoFiles = Get-ChildItem -Path "$sdkPath\protos" -Filter "*.proto" -Recurse | Select-Object -ExpandProperty Name
    
    # Generate code files for each .proto file
    foreach ($protoFile in $protoFiles) {
        Write-Host "Processing: $protoFile from $sdkPath"
        
        # Generate C# code
        Write-Host "Generating C# code..."
        & $protocPath --csharp_out=$csharpOutputDir `
            --plugin=protoc-gen-grpc=$grpcPluginPath `
            --grpc_out=$csharpOutputDir `
            --proto_path="$sdkPath\protos" $protoFile

        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to generate C# code for $protoFile"
            continue
        }

        # Generate TypeScript code
        Write-Host "Generating TypeScript code..."
        & $protocPath --ts_out=$tsOutputDir `
            --plugin=protoc-gen-ts=$tsPluginPath `
            --proto_path="$sdkPath\protos" $protoFile

        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to generate TypeScript code for $protoFile"
        }
    }
}

Write-Host "Code generation complete!"
Write-Host "C# output: $csharpOutputDir"
Write-Host "TypeScript output: $tsOutputDir"