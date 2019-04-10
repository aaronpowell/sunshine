FROM mcr.microsoft.com/dotnet/core/sdk:2.2
RUN apt update && \
    apt install unzip && \
    curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg
WORKDIR /app
COPY ./bin/Debug/netcoreapp2.2/publish .
ENTRYPOINT [ "dotnet", "./Sunshine.MockApi.dll" ]