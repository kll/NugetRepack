#!/usr/bin/env bash

# sane settings for the script interpreter
set -o errexit  # exit when a command fails
set -o pipefail # pipes have exit status of the last command that had a non-zero exit code
set -o nounset  # exit on attempts to use undeclared variables
[[ "${TRACE:-}" == 'true' ]] && set -o xtrace   # prints every expression before executing it when debugging

# configuration
readonly build_image='mcr.microsoft.com/dotnet/core/sdk:latest'

# Set magic variables for current file & dir
__dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
__file="${__dir}/$(basename "${BASH_SOURCE[0]}")"

# Prevent MSYS from trying to translate paths when running on Windows
export MSYS_NO_PATHCONV=1

main() {
  # Ensure docker is available.
  if ! [ -x "$(command -v docker)" ]; then
    echo 'Error: docker is not installed.' >&2
    exit 1
  fi

  docker run  --mount type=bind,source="${__dir}",target=/src  \
              --workdir /src \
              --rm \
              -e PUSH_SOURCE \
              -e PUSH_APIKEY \
              -e TRACE \
              "${build_image}" ./build "$@"
}

main "$@"
