# mvp-serverless-app-template

A simple template to develop, publish, and test a simple .NET service backend on AWS, with serverless Lambda functions, supported by DynamoDb, behind an API Gateway, and protected by Cognito auth.

![A diagram illustrating several services, each with access to a database; behind an API gateway, which has access to an auth service. A web app and mobile app both have access to the API Gateway and auth service, too.](documentation/images/rocket-mvp.png "A diagram illustrating several services, each with access to a database; behind an API gateway, which has access to an auth service. A web app and mobile app both have access to the API Gateway and auth service, too.")

- [x] SAM template
- [x] Functions (Lambda)
- [x] API Gateway
- [x] Database (DynamoDb)
- [x] Auth (Cognito)

## AWS services

This design shouldn't be too hard on the budget during development and prototyping. Here's a quick breakdown of each AWS service used, and their free tiers:

| Service | Purpose | Free tier | Free limit |
|-|-|-|-|
| CloudFormation | Deployment stacks | Always free | 1k handler operations pcm per account |
| DynamoDb | Data storage | Always free | 25Gb storage; 200m requests pcm |
| Lambda | Serverless functions | Always free | 1m requests pcm; 3.2m secs compute pcm |
| API Gateway | API endpoints | 12 months | 1m API calls pcm |
| Cognito | Authentication | Always free | 50k active users pcm |

* AWS free tiers and specifics for each service: https://aws.amazon.com/free/

## Prerequisites

This template assume you're working on a Mac with [Homebrew](https://brew.sh/) installed and `bash` available. If not, very little should be different - but you'll need alternative commands for your OS to install the tools.

```shell
brew install awscli
brew install aws-sam-cli
brew install --cask aws-vault
brew install --cask dotnet-sdk
```

### Optional tools

* VSCode AWS Toolkit extension

## Init

### Set up IAM in the AWS console

* Create an IAM user for the aws cli and SAM to use (eg. `administrator`)
  * Choose access keys, rather than username/password
* Get the credentials for the user (access key id, secret access key)
* Grant the new user administrative permissions, eg. the `AdministratorAccess` policy

### Set up your local machine

`cat ~/.aws/config` and `aws-vault list` to see your currently configured profiles.

* Add the credentials to `aws-vault`, ie.
  ```shell
  aws-vault add administrator
  ```
* Add the profile to `~/.aws/config`, eg.
  ```ini
  [profile administrator]
  region=eu-west-2
  output=json
  ```

## Deployment scripts

* `deploy-stack-aws.sh -a <profile-name> -s <stack-name>` - build and deploy to AWS
* `delete-stack-aws.sh -a <profile-name> -s <stack-name>` - delete a given stack on AWS

In each case, you must provide a profile and stack.

## Auth

On first deployment, you new test user will receive a temporary password by email. You'll need to set the password on first use with the `first-time-set-password.sh` script:

```shell
auth-scripts/first-time-set-password.sh \
  -a <aws-profile> \
  -u <user-email> \
  -t <temporary-password> \
  -p <new-password> \
  --client-id <cognito-client-id> \
  --user-pool-id <cognito-user-pool-id>
```

You may need to use the `--region` parameter if your default region is not set to `eu-west-2`.

`cognito-client-id` and `cognito-user-pool-id` can be obtained from the deployed stack outputs.

Use the `IdToken` output from this script as the `Authorization: Bearer` heading value in calls to the API Gateway. eg.

```shell
curl -H "Authorization: Bearer <IdToken>" https://gateway.endpoint.uri.etc.
```

For subsequent calls, you can use the output from `get-user-id-token.sh`, eg.

```shell
export ID_TOKEN=$(auth-scripts/get-user-id-token.sh -u <username> -p <password> -ci <cognito-client-id>)
```

## Misc notes

### OS X aws-vault keychain access

To manage the keychain for aws-vault, you'll want to import it into the Keychain Access application:

* File / Import Items...
* Keychains are located in: `~/Library/Keychains`
* Import this file: `aws-vault.keychain-db`

## Acknowledgements

With thanks to...

* [Nathaniel Beckstead](https://github.com/scriptingislife) for this guide: [How to add Cognito to your AWS SAM app](https://scriptingis.life/Cognito-AWS-SAM)

