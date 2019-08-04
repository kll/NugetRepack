#!/usr/bin/env bash

# sane settings for the script interpreter
set -o errexit  # exit when a command fails
set -o pipefail # pipes have exit status of the last command that had a non-zero exit code
set -o nounset  # exit on attempts to use undeclared variables
[[ "${TRACE:-}" == 'true' ]] && set -o xtrace   # prints every expression before executing it when debugging

# ensure we clean up after ourselves
trap 'build_exitcode=$?; echo "Cleaning up"; rm -f nuget.config; exit $build_exitcode' INT TERM EXIT

# configuration
readonly pack_projects='NugetRepack.Tool'
readonly tools_path='.tools'

# Set magic variables for current file & dir
__dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
__file="${__dir}/$(basename "${BASH_SOURCE[0]}")"

main() {
  verify_prerequisites
  run_cake "$@"
}

verify_prerequisites() {
  verify_dotnet
  dotnet tool restore
}

verify_dotnet() {
  # Ensure dotnet is available.
  if ! [ -x "$(command -v dotnet)" ]; then
    echo 'Error: dotnet is not installed.' >&2
    exit 1
  fi

  # Check the version
  local found_dotnet_sdk_version="$(dotnet --version)"
  echo "Using .NET core SDK version: ${found_dotnet_sdk_version}"
}

run_cake() {
  dotnet cake --paths_tools="${tools_path}" --bootstrap
  dotnet cake --paths_tools="${tools_path}" --projects="${pack_projects}" "$@"
}

main "$@"
