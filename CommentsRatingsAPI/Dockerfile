#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

# Use the official .NET 6 runtime as a parent image
FROM mcr.microsoft.com/dotnet/sdk:6.0

# Set the working directory in the container
WORKDIR /app

# Copy the build output from your local machine to the container
COPY ./bin/Release/net6.0/ /app/

EXPOSE 5000

# Set the command to run your app using the dotnet command
ENTRYPOINT ["dotnet", "CommentsRatingsAPI.dll"]