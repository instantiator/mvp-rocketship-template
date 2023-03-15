#!/bin/bash

set -e
set -o pipefail

usage() {
  cat << EOF
Gets a user's access token.

Options:
    -u <email>     --test-user <email>             The test user email address
    -p <password>  --password <password>           The password for the user
    -ci <id>       --client-id <id>                The cognito client id (an output from the original stack)
    -r <region>    --region <region>               The AWS region to use (default is eu-west-2)
    -h             --help                          Prints this help message and exits

EOF
}

REGION=eu-west-2

# parameters
while [ -n "$1" ]; do
  case $1 in
  -r | --region)
    shift
    REGION=$1
    ;;
  -u | --test-user)
    shift
    TEST_USER_EMAIL=$1
    ;;
  -p | --password)
    shift
    PASSWORD=$1
    ;;
  -ci | --client-id)
    shift
    CLIENT_ID=$1
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

if [ -z "$TEST_USER_EMAIL" ]; then
  echo "Please provide the test user email."
  usage
  exit 1
fi

if [ -z "$PASSWORD" ]; then
  echo "Please provide the test user's password."
  usage
  exit 1
fi

if [ -z "$CLIENT_ID" ]; then
  echo "Please provide the Cognito client id."
  usage
  exit 1
fi

USER_TOKENS_JSON=$(aws cognito-idp initiate-auth \
    --region $REGION \
    --auth-flow USER_PASSWORD_AUTH \
    --auth-parameters "USERNAME=${TEST_USER_EMAIL},PASSWORD=${PASSWORD}" \
    --client-id ${CLIENT_ID} \
    --output json)

echo $USER_TOKENS_JSON | jq -r ".AuthenticationResult.AccessToken"

# Use the Access Token as the auth bearer heading in calls to the API Gateway
# eg. curl -H "Authorization: Bearer <IdToken>" https://gateway.endpoint.uri.etc.