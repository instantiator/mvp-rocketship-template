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

## Scripts

* `run-localstack.sh` - update and start localstack
* `deploy-local.sh` - build and deploy to localstack
* `stop-localstack.sh` - stop localstack
