# mvp-serverless-app-template

A simple template to develop, publish, and test a simple .NET service behind an API gateway on AWS.

![A diagram illustrating several services, each with access to a database; behind an API gateway, which has access to an auth service. A web app and mobile app both have access to the API Gateway and auth service, too.](documentation/images/rocket-mvp.png "A diagram illustrating several services, each with access to a database; behind an API gateway, which has access to an auth service. A web app and mobile app both have access to the API Gateway and auth service, too.")

- [x] SAM template
- [x] Functions (Lambda)
- [x] API Gateway
- [x] Database (DynamoDb)
- [ ] Auth (Cognito)

## Progress

- [x] Skeleton function, with template, build and deployment support
- [ ] Support full basic MVP design
- [ ] Support testing

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

```shell
brew install awscli
brew install aws-sam-cli
brew install --cask aws-vault
brew install --cask dotnet-sdk
```

### Optional tools

* VSCode AWS Toolkit extension

## Init

### Create a user for your deployment

* Create an IAM user for the aws cli and SAM to use (eg. `administrator`)
  * Choose access keys, rather than username/password
* Get the credentials for the user (access key id, secret access key)
* Grant the new user administrative permissions, eg. the `AdministratorAccess` policy

### Set up your local machine

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

## Scripts

* `deploy-stack-aws.sh -p <profile-name> -s <stack-name>` - build and deploy to AWS
* `delete-stack-aws.sh -p <profile-name> -s <stack-name>` - delete a given stack on AWS

In each case, you must provide a profile and stack.

### Profiles

* `cat ~/.aws/config` and `aws-vault list` to see your currently configured profiles.
* Use `aws-vault add <profile>` to store the credentials (access key id, and secret access key) for a profile.
* Use the `config` file to store other values (eg. region).
