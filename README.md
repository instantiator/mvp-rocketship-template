# mvp-framework

A simple framework to develop, publish, and test simple services.

## In development

- [x] Skeleton function, with template, local build and deployment support
- [ ] Support full basic MVP artefacts
  - [x] Functions
  - [ ] Gateway
  - [ ] Cognito
  - [ ] Database

## Prerequisites

* docker CLI
* localstack CLI
* samlocal CLI
* dotnet CLI

## Optional developer tools

* VSCode AWS Toolkit extension

## Build

```
sam build
```

or

```
cd src/SampleFunctions
dotnet build
```

## Deploy to localstack

Start localstack:

```
localstack update all
localstack start
```

Deploy to local:

```
samlocal deploy \
  --stack-name mvp-functions \
  --region eu-west-2 \
  --capabilities CAPABILITY_IAM \
  --resolve-s3
```
