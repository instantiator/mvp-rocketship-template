#!/bin/bash

set -e

localstack update all
localstack start -d
