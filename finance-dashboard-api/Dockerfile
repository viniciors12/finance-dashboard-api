# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy files of project
COPY ["MyApp.csproj", "./"]
RUN dotnet restore "./MyApp.csproj"

# Copy code and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Lambda base image
FROM public.ecr.aws/lambda/dotnet:8 AS runtime
WORKDIR /var/task

# Copy pusblished artifactories
COPY --from=build /app/publish .

# Define handler
CMD ["MyApp::MyApp.LambdaEntryPoint::FunctionHandler"]
