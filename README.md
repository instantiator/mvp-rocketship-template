# mvp-rocketship-template

A simple template to develop, publish, and test a simple .NET service backend on AWS, with serverless Lambda functions, supported by DynamoDb, behind an API Gateway, and protected by Cognito auth. 🚀

_Hopefully this will accelerate some early prototyping for some of my projects, and perhaps you'll find it useful as a reference too._

## Design

The rocket ship design...

![A diagram illustrating several services, each with access to a database; behind an API gateway, which has access to an auth service. A web app and mobile app both have access to the API Gateway and auth service, too.](documentation/images/rocket-mvp.png "A diagram illustrating several services, each with access to a database; behind an API gateway, which has access to an auth service. A web app and mobile app both have access to the API Gateway and auth service, too.")

Included in this template:

- [x] SAM template
- [x] Sample functions (.NET 6 Lambdas)
- [x] Endpoint tests (MSTest)
- [x] API Gateway configuration
- [x] CORS header support
- [x] Database (DynamoDb)
- [x] Auth (Cognito)

Not included in this template:

* Web and mobile client apps

## AWS services and costs

AWS should come with a warning - if you exceed the free tier, you'll be billed. There's no easy way to set a cap on consumption, so keep an eye on your usage.

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

* `deploy-stack-aws.sh -a <profile-name> -s <stack-name> -u <test-user-email>` - build and deploy the application stack to AWS
* `delete-stack-aws.sh -a <profile-name> -s <stack-name>` - delete a given stack on AWS
* `test-stack.sh -a <profile-name> -s <stack-name>` - run the endpoint tests against a given stack on AWS
* `test-transient-stack.sh -a <profile-name> -u <test-user-email>` - deploy a stack, run the endpoint tests against it, then delete it

In nearly all cases, you must provide an AWS profile available on your machine, and a stack name. The transient stack test script doesn't require a stack name - it makes one up for you. You must provide a test user email address.

## Auth

On first deployment, your new test user will receive a temporary password by email. This will then be immediately updated to `testpass1`. (This is hard coded in: `deploy-stack-aws.sh`.)

You should obtain and use the user's `AccessToken` as the `Authorization: Bearer` heading in calls to the API, eg.

```shell
curl -H "Authorization: Bearer <AccessToken>" https://endpoint.uri.etc.
```

For all subsequent calls where you need a token, you can use the output from `auth-scripts/get-user-access-token.sh`, eg.

```shell
export ACCESS_TOKEN=$(auth-scripts/get-user-access-token.sh -u <username> -p <password> -ci <cognito-client-id>)
curl -H "Authorization: Bearer $ACCESS_TOKEN" https://endpoint.uri.etc.
```

## Testing

The `test-stack.sh` and `test-transient-stack.sh` scripts will run the endpoint tests from: `src/SampleFunctions/EndpointTests`

These will exercise Cognito (retrieving user credentials through the `aws.cognito.signin.user.admin` OAuth scope), and then provide the _access_ token they received as in the `Authorization: Bearer <token>` header to interact with the APIs.

## CORS

To access the APIs from a browser, additional CORS headers are required.

* Cross-origin requests are rejected by browsers because CORS headers aren't available.
* CORS headers are also required for 4XX errors from the Cognito authorizer, and 5XX from lambda execution.

CORS headers can be generated by the lambda methods themselves, and by Gateway for the 4XX and 5XX errors.

Browsers make a pre-flight check using the `OPTIONS` method before calling the actual method (eg. `GET` / `POST` etc.) for an API call. An additional endpoint is required for the `OPTIONS` HTTP method which should also provide the CORS headers. This is illustrated in `template.yaml` and the function code provided.

The code in this template assume that the source of requests comes from `https://localhost:5001` (Coincidentally, `5001` is the port that Blazor web assembly apps are hosted on your local machine by default.)

If you use a browser-based client, pay careful attention to the CORS configuration. You'll need to alter the `Access-Control-Allow-Origin` header found in each function and `template.yaml` to match the URL of the page that makes the call.

## Exercising the functions

The template deploys two functions - one, called the root handler, is responsible for calls to `/FunctionOne`. The other, called the notes handler, is responsible for calls to `/FunctionOne/notes`.

When developing your application, give some though to which endpoints need authentication and which don't. `template.yaml` applies an authorizer to the notes endpoint, but not to the root endpoint or to the `OPTIONS` events.

| Handler | Endpoint | Method | Auth | Notes |
|-|-|-|-|-|
| `FunctionOne.HandlerRoot:Handle` | `/FunctionOne` | `GET` | Not required. | Returns "ok". | 
| `FunctionOne.HandlerRoot:Handle` | `/FunctionOne` | `OPTIONS` | Not required. | Supports a CORS pre-flight request for headers. | 
| `FunctionOne.HandlerNotes:Handle` | `/FunctionOne/notes` | `GET` | Access token required. | Returns a count of the number of notes in the database table. |
| `FunctionOne.HandlerNotes:Handle` | `/FunctionOne/notes` | `POST` | Access token required. | Adds a new note to the database table. |
| `FunctionOne.HandlerRoot:Handle` | `/FunctionOne/notes` | `OPTIONS` | Not required. | Supports a CORS pre-flight request for headers. | 

### Manual test

Retrieve an access token for this user:

```shell
export ACCESS_TOKEN=$(auth-scripts/get-user-access-token.sh -u <test-user-email> -p <password> -ci <client-id>)
```

POST to the notes endpoint, to add a new note to the table:

```shell
curl -H "Authorization: Bearer $ACCESS_TOKEN" -X POST https://endpoint.uri.etc/staging/FunctionOne/notes
```

GET from the notes endpoint, to see how many notes there are:

```shell
curl -H "Authorization: Bearer $ACCESS_TOKEN" https://endpoint.uri.etc/staging/FunctionOne/notes
```

## Tidying up

Finished? Tidy up so that resources don't accidentally incur costs. Particularly as, by default, some of your endpoints aren't protected by Cognito.

```shell
./delete-stack-aws.sh -a <aws-profile> -s <stack-name>
```

**NB. There is a known issue that prevents stack deletion from completing. You can complete deletion in the AWS console.**

## Misc notes

### API gateway / Cognito user pool authorizers vs. tokens

If scopes are not applied to an API Gateway function's event source, the authorizer will accept the user's _id token_ by default.

If scopes are applied, then the authorizer will accept the user's _access token_ instead.

NB. scopes _are_ applied in `template.yaml` - which is why these functions require the _access token_.

### OS X aws-vault keychain access

To manage the keychain for aws-vault, you'll want to import it into the Keychain Access application:

* File / Import Items...
* Keychains are located in: `~/Library/Keychains`
* Import this file: `aws-vault.keychain-db`

### Client apps

Client apps are assumed to be simple (perhaps a [SPA](https://en.wikipedia.org/wiki/Single-page_application)), and can be hosted anywhere.

I've used Blazor webasm in the past to build a simple responsive app quickly, and can recommend it - but you may already have a solution (perhaps a trendy javascript or TS framework) in mind.

Where using a browser-based client application, pay careful attention to the CORS configuration. You'll need to alter the `Access-Control-Allow-Origin` header found in each function and `template.yaml` to match the URL of the page that makes the call.

### LocalStack

This template was originally proposed with [LocalStack](https://localstack.cloud/) in mind. It's a nice implementation of the AWS APIs that allows local deployment.

However, the Community tier for LocalStack does not support API Gateway, and the Pro tier is slightly too costly to develop the framework against. I've abandoned it for now.

## Acknowledgements

With thanks to...

* [Nathaniel Beckstead](https://github.com/scriptingislife), for: [How to add Cognito to your AWS SAM app](https://scriptingis.life/Cognito-AWS-SAM)
* Jeff Adams, [ten mile square](https://tenmilesquare.com/), for: [AWS SAM API with Cognito](https://tenmilesquare.com/resources/cloud-infrastructure/aws-sam-api-with-cognito/)

## Changelog

### 2023-03-16

Add testing support, CORS, and simplify the provided lambda functions:

- [x] Update deployment scripts to set the test user password directly
- [x] Add tests, and testing scripts for named stacks and transient stacks
- [x] Apply OAuth scopes to API endpoints in `template.yaml`
- [x] Add CORS headers to 4XX and 5XX responses in `template.yaml`
- [x] Create an `OPTIONS` lambda handler to provide CORS headers
- [x] Provide CORS headers in all other handler responses, too
