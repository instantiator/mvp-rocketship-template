# mvp-framework

A simple framework to develop, publish, and test simple services.

## Prerequisites

* docker
* localstack
* samlocal

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
