#!/bin/bash

set -e
set -o pipefail

usage() {
  cat << EOF
Sets the test user password. Assumes you know the temporary password, sent by email.

Options:
    -a <profile>   --aws-profile <profile>         The profile to use (see: ~/.aws/config and aws-vault list)
    -u <email>     --test-user <email>             The test user email address
    -t <password>  --temporary-password <password> The temporary password, sent by email to the user
    -p <password>  --new-password <password>       The new password for the user
    -ci <id>       --client-id <id>                The cognito client id (an output from the original stack)
    -up <id>       --user-pool-id <id>             The cognito user pool id (an output from the original stack)
    -r <region>    --region <region>               The AWS region to use (default is eu-west-2)
    -h             --help                          Prints this help message and exits

EOF
}

REGION=eu-west-2

# parameters
while [ -n "$1" ]; do
  case $1 in
  -a | --aws-profile)
    shift
    PROFILE=$1
    ;;
  -r | --region)
    shift
    REGION=$1
    ;;
  -u | --test-user)
    shift
    TEST_USER_EMAIL=$1
    ;;
  -t | --temporary-password)
    shift
    TEMP_PASSWORD=$1
    ;;
  -p | --new-password)
    shift
    NEW_PASSWORD=$1
    ;;
  -ci | --client-id)
    shift
    CLIENT_ID=$1
    ;;
  -up | --user-pool-id)
    shift
    POOL_ID=$1
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

if [ -z "$TEST_USER_EMAIL" ]; then
  echo "Please provide the test user email."
  usage
  exit 1
fi

if [ -z "$TEMP_PASSWORD" ]; then
  echo "Please provide the test user's temporary password."
  usage
  exit 1
fi
if [ -z "$NEW_PASSWORD" ]; then
  echo "Please provide the test user's new password."
  usage
  exit 1
fi

if [ -z "$CLIENT_ID" ]; then
  echo "Please provide the Cognito client id."
  usage
  exit 1
fi

if [ -z "$POOL_ID" ]; then
  echo "Provide the Cognito user pool id."
  usage
  exit 1
fi

echo Initiating USER_PASSWORD_AUTH flow...
echo

INITIAL_SESSION=$(aws cognito-idp initiate-auth \
    --region $REGION \
    --auth-flow USER_PASSWORD_AUTH \
    --auth-parameters "USERNAME=${TEST_USER_EMAIL},PASSWORD=${TEMP_PASSWORD}" \
    --client-id ${CLIENT_ID} \
    --query "Session" \
    --output text)

# echo "Session token:"
# echo $INITIAL_SESSION
# echo

echo Initiating admin-respond-to-auth-challenge...
echo

USER_TOKENS_JSON=$(aws-vault exec $PROFILE --no-session -- \
  aws cognito-idp admin-respond-to-auth-challenge \
    --region $REGION \
    --user-pool-id $POOL_ID \
    --client-id $CLIENT_ID \
    --challenge-name NEW_PASSWORD_REQUIRED \
    --challenge-responses "USERNAME=${TEST_USER_EMAIL},NEW_PASSWORD=${NEW_PASSWORD}" \
    --session $INITIAL_SESSION)

# echo "AccessToken:"
# echo $USER_TOKENS_JSON | jq -r ".AuthenticationResult.AccessToken"
# echo

# echo "RefreshToken:"
# echo $USER_TOKENS_JSON | jq -r ".AuthenticationResult.RefreshToken"
# echo

echo "IdToken:"
echo $USER_TOKENS_JSON | jq -r ".AuthenticationResult.IdToken"
echo

echo Use the Id Token as the auth bearer heading in calls to the API Gateway.
echo eg. curl -H "Authorization: Bearer <IdToken>" https://gateway.endpoint.uri.etc.
echo
echo For subsequent calls, you can use the output from get-user-id-token.sh
echo eg. export ID_TOKEN=$(auth-scripts/get-user-id-token.sh -u <username> -p <password> -ci <client-id>)
echo
