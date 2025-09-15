# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy files of project
COPY ["finance-dashboard-api.csproj", "./"]
RUN dotnet restore "./finance-dashboard-api.csproj"

# Copy code and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Lambda base image
FROM public.ecr.aws/lambda/dotnet:8 AS runtime
WORKDIR /var/task

# Copy pusblished artifactories
COPY --from=build /app/publish .

# Define handler
CMD ["finance-dashboard-api::finance_dashboard_api.Lambda.LambdaEntryPoint"]

