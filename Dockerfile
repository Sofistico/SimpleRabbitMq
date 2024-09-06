
# syntax=docker/dockerfile:1

################################################################################

# Create a stage for building the application.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG TARGETARCH

# Copy the source code into the container
COPY . /source

# Set the working directory to the project folder
WORKDIR /source/SimpleRabbitMq.Api

# Build the application.
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish SimpleRabbitMq.Api.csproj -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

################################################################################

# Create a new stage for running the application.
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY --from=build /app .

# Create a non-privileged user that the app will run under.
ARG UID=10001
RUN adduser \
    --disabled-password \
    --gecos "" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --uid "${UID}" \
    appuser
USER appuser

ENTRYPOINT ["dotnet", "SimpleRabbitMq.Api.dll"]

