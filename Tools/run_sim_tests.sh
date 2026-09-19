#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
export PATH="${HOME}/.dotnet:${PATH}"
cd "$ROOT/Tools/Heartline.Sim"
dotnet run -c Release
