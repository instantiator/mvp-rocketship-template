#!/bin/bash

set -e
set -o pipefail

usage() {
  cat << EOF
Generates EXPORT commands to set the outputs from a stack as local variables.

Run direcly to see the export commands.

Use the output from this script with eval to set environment variables.

Options:
    -a <profile> --aws-profile <profile> Sets the profile to use (see: ~/.aws/config and aws-vault list)
    -s <name>    --stack <name>          Sets the name of the stack (required)
    -x <prefix>  --prefix <prefix>       Short prefix to prepend to all stack output values.
    -h           --help                  Prints this help message and exits
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
  -x | --prefix)
    shift
    PREFIX=$1
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

# describe the stack
DESCRIPTION=$(aws-vault exec $PROFILE -- aws cloudformation describe-stacks --stack-name $STACK_NAME)

# use this to print the outputs as JSON
# echo $DESCRIPTION | jq ".Stacks[0].Outputs"

echo $DESCRIPTION | jq -c ".Stacks[0].Outputs[]" | while read -r OUTPUT; do 
  KEY=${PREFIX}$(echo $OUTPUT | jq -r ".OutputKey")
  VALUE=$(echo $OUTPUT | jq -r ".OutputValue")
  echo "export ${KEY}='${VALUE}'"
  echo
done
