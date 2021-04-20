FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

ARG environment='Development'
ARG connectionstring='Endpoint=https://dt-ac-weu-dev-sbus.azconfig.io;Id=iabO-l9-s0:4bgePqmt8LQwewj80hzW;Secret=I5BzPzS25vvwZWvv/7Or81zrqZi8fNI3NzReoYaKL/A='

ENV ASPNETCORE_ENVIRONMENT      ${environment}
ENV ASPNETCORE_URLS             http://+:5000
ENV APPCONFIG_SETTINGSPREFIX    DT.SBUS.beapipayat:
ENV APPCONFIG_CONNECTIONSTRING  ${connectionstring}
ENV APPCONFIG_SITEPREFIX        "Primary:"

COPY app/ app/

EXPOSE 5000

WORKDIR /app

ENTRYPOINT ["dotnet", "DirectTransact.WebApi.PayAT.dll"]

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
#
#COPY app/ app/
#
#EXPOSE 80
#
#WORKDIR /app
#
#ENTRYPOINT ["dotnet", "DT-PH-ZAN-AUTH-API-OpenAPI.dll"]
#