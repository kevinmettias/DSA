#!/usr/bin/env bash
# A/B a pilot step: before = HEAD, after = HEAD plus this step's working-tree files (ab-prep.sh).
# Usage: ab.sh <pass-name> "<file list>" <BenchmarkClass>...
set -eu
AB=/f/repos/DSA-ab-2f
pass=$1; files=$2; shift 2
bash /f/repos/DSA/.scratch/dsa-2f-pilot/ab-prep.sh "$files"
~/.claude/tools/bdn-ab/bdn-ab.exe run -before "$AB/before" -after "$AB/after" -out "$AB/$pass" \
  -lock /f/repos/DSA-ab/bdn-ab.lock -project DSAExperimentation.Benchmarks -filter '*.{class}Benchmarks.*' -build "$@"
~/.claude/tools/bdn-ab/bdn-ab.exe report "$AB/$pass"
