#-------------------------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See https://go.microsoft.com/fwlink/?linkid=2090316 for license information.
#-------------------------------------------------------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/core/sdk:latest

# Avoid warnings by switching to noninteractive
ENV DEBIAN_FRONTEND=noninteractive

# Configure apt and install packages
RUN apt-get update \
    && apt-get -y install --no-install-recommends apt-utils 2>&1 \
    #
    # Do the installs
    && apt-get -y install \
        fonts-powerline \
        git \
        gnupg-agent \
        procps \
        locales \
        lsb-release \
        neovim \
        sudo \
        ttf-ancient-fonts \
        whiptail \
        zsh \
        zsh-antigen \
    #
    # Clean up
    && apt-get autoremove -y \
    && apt-get clean -y \
    && rm -rf /var/lib/apt/lists/*

# Configure non-root user
ARG CONTAINER_USERNAME
ARG CONTAINER_UID
ARG CONTAINER_GID
RUN groupadd --gid ${CONTAINER_GID} ${CONTAINER_USERNAME} \
    && useradd --uid ${CONTAINER_UID} --gid ${CONTAINER_GID} -m ${CONTAINER_USERNAME} \
    && echo ${CONTAINER_USERNAME} ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/${CONTAINER_USERNAME} \
    && chmod 0440 /etc/sudoers.d/${CONTAINER_USERNAME}
ENV USER=${CONTAINER_USERNAME}
ENV HOME /home/${CONTAINER_USERNAME}

# Configure locale
RUN sed -i '/en_US.UTF-8/s/^#*//' /etc/locale.gen \
    && locale-gen \
    && echo 'LANG=en_US.UTF-8' > /etc/locale.conf

# Configure shell
ENV SHELL /bin/zsh
COPY .zshrc /home/${CONTAINER_USERNAME}/.zshrc
RUN chown ${CONTAINER_UID}:${CONTAINER_GID} /home/${CONTAINER_USERNAME}/.zshrc

# Set the default user
USER ${CONTAINER_USERNAME}

# Use a volume for the NuGet configuration
RUN mkdir -p /home/${CONTAINER_USERNAME}/.nuget/NuGet \
    && chown ${CONTAINER_UID}:${CONTAINER_GID} /home/${CONTAINER_USERNAME}/.nuget/NuGet \
    && chmod g+s /home/${CONTAINER_USERNAME}/.nuget/NuGet
VOLUME /home/${CONTAINER_USERNAME}/.nuget/NuGet

# Switch back to dialog for any ad-hoc use of apt-get
ENV DEBIAN_FRONTEND=dialog
