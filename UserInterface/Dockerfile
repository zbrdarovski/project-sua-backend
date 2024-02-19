#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

# Use the official .NET 6 runtime as a parent image
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine

# Set the working directory in the container
WORKDIR /app

# Copy the build output from your local machine to the container
COPY ./bin/Release/net6.0/ /app/

EXPOSE 11180
EXPOSE 1189
EXPOSE 80
EXPOSE 443

ENV RABBITMQ_HOSTNAME=rabbit
ENV RABBITMQ_PORT=5672
ENV RABBITMQ_USERNAME=student
ENV RABBITMQ_PASSWORD=student123
ENV MONGODB_CONNECTION_STRING=mongodb+srv://sua-user:30SD8YKo4tg7R7v5@cluster0.550s6o6.mongodb.net/?retryWrites=true&w=majority

ENV ASPNETCORE_URLS=http://+:1180

# Set the command to run your app using the dotnet command
# Replace 'MyApp.dll' with the name of your application's DLL
ENTRYPOINT ["dotnet", "UserInterface.dll"]


##See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
##Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
##For more information, please see https://aka.ms/containercompat
#
#FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["UserInterface/UserInterface.csproj", "UserInterface/"]
#RUN dotnet restore "UserInterface/UserInterface.csproj"
#COPY . .
#WORKDIR "/src/UserInterface"
#RUN dotnet build "UserInterface.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "UserInterface.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "UserInterface.dll"]