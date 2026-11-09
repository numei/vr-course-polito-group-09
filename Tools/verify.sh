#!/usr/bin/env bash
set -euo pipefail
project_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
unity_editor="${UNITY_EDITOR:-${1:-Unity}}"
results_dir="${RESULTS_DIR:-$project_root/TestResults}"
mkdir -p "$results_dir"
if ! command -v "$unity_editor" >/dev/null 2>&1; then
  echo "Set UNITY_EDITOR to the Unity executable, or pass it as the first argument." >&2
  exit 1
fi
for platform in EditMode PlayMode; do
  echo "Running $platform tests..."
  "$unity_editor" -batchmode -nographics -projectPath "$project_root" \
    -runTests -testPlatform "$platform" -testResults "$results_dir/$platform.xml" \
    -logFile "$results_dir/$platform.log"
  python3 - "$results_dir/$platform.xml" <<'PY'
import sys
import xml.etree.ElementTree as E
r = E.parse(sys.argv[1]).getroot()
values = {key: int(r.get(key, '0')) for key in ['total', 'passed', 'failed', 'skipped', 'inconclusive']}
print(values)
if values['total'] == 0 or values['passed'] != values['total']:
    raise SystemExit('Incomplete or failed test suite. Read the XML/log.')
PY
done
