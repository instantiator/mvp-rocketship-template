#!/bin/bash

set -e
set -o pipefail
set -o allexport

usage() {
  cat << EOF
Deploys a transient stack to AWS, and runs tests against it.

Assumes aws-vault installed and your profile is created.

Required options:
    -a <profile>   --aws-profile <profile> Sets the profile to use (see: ~/.aws/config and aws-vault list)
    -u <email>     --test-user <email>     Sets the test user email address

Optional:
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

if [ -z "$TEST_USER_EMAIL" ]; then
  echo "Please provide a test user email address for this stack."
  usage
  exit 1
fi

# name transient stack
STACK_SUFFIX=$(echo $RANDOM | md5 | head -c 10;)
STACK_NAME="rocket-test-${STACK_SUFFIX}"
echo "Stack: ${STACK_NAME}"

# deploy transient stack - with a trap to delete the stack on exit
function cleanup() {
  echo "Deleting stack: ${STACK_NAME}..."
  echo
  ./delete-stack-aws.sh -a $PROFILE -s $STACK_NAME
}
trap cleanup EXIT

# deploy stack, set test user password, etc.
echo "Deploying stack: ${STACK_NAME}..."
echo
./deploy-stack-aws.sh -a $PROFILE -u $TEST_USER_EMAIL -s $STACK_NAME

# get stack outputs
TEST_USER_PASSWORD=testpass1
eval $(util-scripts/export-stack-outputs.sh -a $PROFILE -s $STACK_NAME -x OUT_)

# launch tests
AWS_REGION=eu-west-2
dotnet test \
  src/SampleFunctions/EndpointTests/EndpointTests.csproj \
  -l:"console;verbosity=normal"
