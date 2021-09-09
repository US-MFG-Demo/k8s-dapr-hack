FROM mcr.microsoft.com/dotnet/sdk:5.0

ARG USERNAME=vscode

RUN apt-get update && apt-get install -y \
    ca-certificates \
    curl \
    apt-transport-https \
    lsb-release \
    gnupg \
    docker \
    && rm -rf /var/lib/apt/lists/*

RUN curl -sL https://aka.ms/InstallAzureCLIDeb | /bin/bash

RUN az bicep install

RUN curl -s https://raw.githubusercontent.com/rancher/k3d/main/install.sh | bash

RUN wget -q https://raw.githubusercontent.com/dapr/cli/master/install/install.sh -O - | /bin/bash
