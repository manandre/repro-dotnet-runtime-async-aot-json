FROM mcr.microsoft.com/devcontainers/base:ubuntu-24.04

# NativeAOT native dependencies
RUN apt-get update && apt-get install -y \
    clang \
    zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

# Install .NET 11 preview SDK
RUN curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh \
    && chmod +x /tmp/dotnet-install.sh \
    && /tmp/dotnet-install.sh \
        --channel 11.0 \
        --quality preview \
        --install-dir /usr/local/dotnet \
    && rm /tmp/dotnet-install.sh

ENV DOTNET_ROOT=/usr/local/dotnet
ENV PATH="$PATH:/usr/local/dotnet"
