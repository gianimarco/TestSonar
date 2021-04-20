$location = Get-Location
if (!($location -match "docker")) {
    Set-Location -Path ./docker/ 
}

Copy-Item -Recurse -Path "../src/." -Destination "." -Force -ErrorAction Continue

dotnet build "./src/DirectTransact.WebApi.PayAT/DirectTransact.WebApi.PayAT.csproj" -c Release -o ./bld

dotnet publish "./src/DirectTransact.WebApi.PayAT/DirectTransact.WebApi.PayAT.csproj" -c Release -o ./app

#docker build --build-arg environment='Development' --build-arg connectionstring='Endpoint=https://bob-ac-weu-dev-sbus.azconfig.io;Id=jnRr-l9-s0:zHqED/TBnsEek97Nue/g;Secret=At+zMRVbNPgARJrCrNQkrzSSHqd/zNPM+GyacAAjFTw=' -t webapi-apiauth:latest .
docker build -t webapi-beapipayat:latest .

Remove-Item -Recurse -Force src/
Remove-Item -Recurse -Force bld/
Remove-Item -Recurse -Force app/
Set-Location ..