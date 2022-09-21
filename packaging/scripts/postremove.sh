#!/bin/bash

msg() {
    printf "\033[32m $1\033[0m\n"
}

msg "Running post-remove actions..."