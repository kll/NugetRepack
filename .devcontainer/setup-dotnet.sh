#!/usr/bin/env bash

# sane settings for the script interpreter
set -o errexit  # exit when a command fails
set -o pipefail # pipes have exit status of the last command that had a non-zero exit code
set -o nounset  # exit on attempts to use undeclared variables
[[ "${TRACE:-}" == 'true' ]] && set -o xtrace   # prints every expression before executing it when debugging

main() {
  # silently return successfully if no arguments were passed
  # this is so the docker build will not fail if no additional versions are requested
  if [ "$#" -eq 0 ]; then
    exit 0
  fi

  # check for correct number arguments
  if [ "$#" -ne 2 ]; then
    echo "Usage: $0 VERSION_ARRAY CHECKSUM_ARRAY" >&2
    exit 1
  fi

  # parse the arguments into arrays
  read -r -a versions <<< "$1"
  read -r -a checksums <<< "$2"

  # ensure the length of each argument array is the same
  if [ "${#versions[@]}" -ne "${#checksums[@]}" ]; then
    echo 'The VERSION_ARRAY parameter and the CHECKSUM_ARRAY parameter must have the same number of elements.' >&2
    echo "Usage: $0 VERSION_ARRAY CHECKSUM_ARRAY" >&2
    exit 1
  fi

  # ensure the target directory exists
  mkdir -p /usr/share/dotnet

  # loop through the arrays installing each version
  for index in "${!versions[@]}"
  do
    install_dotnet "${versions[index]}" "${checksums[index]}"
  done

  # create link to the executable
  ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet --force

  # trigger first run experience by running arbitrary command to populate local package cache
  dotnet help
}

install_dotnet() {
  local version="${1}"
  local checksum="${2}"

  echo "Installing .NET SDK version ${version} with checksum ${checksum}."
  curl -SL --output dotnet.tar.gz "https://dotnetcli.blob.core.windows.net/dotnet/Sdk/${version}/dotnet-sdk-${version}-linux-x64.tar.gz"
  echo "${checksum} dotnet.tar.gz" | sha512sum -c -
  tar --overwrite -zxf dotnet.tar.gz -C /usr/share/dotnet
  rm dotnet.tar.gz
}

main "$@"
