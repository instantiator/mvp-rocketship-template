#!/bin/bash

set -e
set -o pipefail
set -o allexport

usage() {
  cat << EOF
Runs the endpoint tests against a given stack.

Assumes aws-vault installed and your profile is created.

Options:
    -a <profile>   --aws-profile <profile> Sets the profile to use (see: ~/.aws/config and aws-vault list)
    -s <stack>     --stack <stack>         Sets the stack name
    -h             --help                  Prints this help message and exits

EOF
}

# parameters
while [ -n "$1" ]; do
  case $1 in
  -a | --aws-profile)
    shift
    PROFILE=$1
    ;;
  -s | --stack)
    shift
    STACK_NAME=$1
    ;;
  -h | --help)
    usage
    exit 0
    ;;
  *)
    echo -e "Unknown option $1...\n"
    usage
    exit 1
    ;;
  esac
  shift
done

if [ -z "$PROFILE" ]; then
  echo "Please provide a profile."
  usage
  exit 1
fi

if [ -z "$STACK_NAME" ]; then
  echo "Please set the stack name."
  usage
  exit 1
fi

# 1. fetch outputs from stack
echo "Obtaining outputs from stack: $STACK_NAME"
eval $(util-scripts/export-stack-outputs.sh -a $PROFILE -s $STACK_NAME -x OUT_)
TEST_USER_EMAIL=$OUT_TestUserEmail
TEST_USER_PASSWORD=testpass1

# 3. launch tests
AWS_REGION=eu-west-2
dotnet build src/SampleFunctions/EndpointTests/EndpointTests.csproj
dotnet test \
  src/SampleFunctions/EndpointTests/EndpointTests.csproj \
  -l:"console;verbosity=normal"
