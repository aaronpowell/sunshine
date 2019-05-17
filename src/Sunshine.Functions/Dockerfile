FROM mcr.microsoft.com/azure-functions/dotnet
RUN apt update && \
    apt install unzip procps curl -y && \
    curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

WORKDIR /home/site/wwwroot
COPY ./bin/Debug/netcoreapp2.1/publish .
