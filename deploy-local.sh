#!/bin/bash

samlocal deploy \
  --stack-name mvp-functions \
  --region eu-west-2 \
  --capabilities CAPABILITY_IAM \
  --resolve-s3
