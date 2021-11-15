FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY ./app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "HtmlPdfApi.dll"]
