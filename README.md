# mvp-framework

A simple framework to develop, publish, and test a simple service behind an API gateway.

## In development

- [x] Skeleton function, with template, build and deployment support
- [ ] Support full basic MVP arrangement
  - [x] Functions
  - [x] Gateway
  - [x] Database
  - [ ] Cognito

## Prerequisites

```
brew install awscli
brew install aws-sam-cli
brew install --cask aws-vault
brew install --cask dotnet-sdk
```

## Optional developer tools

* VSCode AWS Toolkit extension

## Scripts

* `deploy-stack-aws.sh -p <profile-name> -s <stack-name>` - build and deploy to AWS
* `delete-stack-aws.sh -p <profile-name> -s <stack-name>` - delete a given stack on AWS

In each case, you must provide a profile and stack.

### Profiles

* `cat ~/.aws/config` and `aws-vault list` to see your currently configured profiles.
* Use `aws-vault add <profile>` to store the credentials (access key id, and secret access key) for a profile.
* Use the `config` file to store other values (eg. region).
