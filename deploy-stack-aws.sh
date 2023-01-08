#!/bin/bash

set -e
set -o pipefail

usage() {
  cat << EOF
Deploys a stack to AWS with everything defined here.

Assumes aws-vault installed and your profile is created.

Options:
    -a <profile>   --aws-profile <profile> Sets the profile to use (see: ~/.aws/config and aws-vault list)
    -u <email>     --test-user <email>     Sets the test user email address
    -s <name>      --stack <name>          Sets the name of the stack (required)
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
  -u | --test-user)
    shift
    TEST_USER_EMAIL=$1
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

if [ -z "$TEST_USER_EMAIL" ]; then
  echo "Please set a test user email address."
  usage
  exit 1
fi

# build
aws-vault exec $PROFILE --no-session -- \
  sam build

# deploy
aws-vault exec $PROFILE --no-session -- \
  sam deploy \
    --stack-name $STACK_NAME \
    --region eu-west-2 \
    --capabilities CAPABILITY_IAM \
    --resolve-s3 \
    --parameter-overrides CognitoTestUserEmail=$TEST_USER_EMAIL
