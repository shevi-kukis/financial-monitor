
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src



COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore



FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app


RUN mkdir -p /data


RUN addgroup -S appgroup && adduser -S appuser -G appgroup

RUN chown -R appuser:appgroup /data

COPY --from=build /app/publish .

USER appuser


EXPOSE 8080

ENTRYPOINT ["dotnet", "FinancialMonitor.dll"]