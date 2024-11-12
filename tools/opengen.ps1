npx @openapitools/openapi-generator-cli generate `
  -i http://localhost:5108/swagger/v1/swagger.json `
  -g typescript-fetch `
  -o ./lib/kronos-ts-client `
  --additional-properties=supportsES6=true,npmName=@pantheon/kronos-client,npmVersion=1.0.0,typescriptThreePlus=true,withInterfaces=true