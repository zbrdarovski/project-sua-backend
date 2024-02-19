FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine

# Set the working directory in the container
WORKDIR /app

# Copy the build output from your local machine to the container
COPY ./bin/Release/net6.0/ /app/

EXPOSE 11187
EXPOSE 1187
EXPOSE 80
EXPOSE 443

ENV RABBITMQ_HOSTNAME=amqp://studentdocker.informatika.uni-mb.si:5672
ENV RABBITMQ_PORT=5672
ENV RABBITMQ_USERNAME=student
ENV RABBITMQ_PASSWORD=student123
ENV MONGODB_CONNECTION_STRING=mongodb+srv://sua-user:30SD8YKo4tg7R7v5@cluster0.550s6o6.mongodb.net/?retryWrites=true&w=majority

ENV ASPNETCORE_URLS=http://+:1187

# Set the command to run your app using the dotnet command
ENTRYPOINT ["dotnet", "StatsBrdarovski.dll"]