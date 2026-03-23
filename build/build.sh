#!/bin/bash
set -e

function fail() {
    echo $@ >&2
    exit 1
}

# Configuration
ZISK_REF="${ZISK_REF}"

if [ -z "$ZISK_REF" ]; then
    fail "Error: ZISK_REF is not set"
fi

ZISK_REPO="https://github.com/0xPolygonHermez/zisk.git"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
TMP_DIR="${ROOT_DIR}/tmp"
OUTPUT_DIR="${ROOT_DIR}/output"

echo "Building libziskos"
echo "Tag: ${ZISK_REF}"
echo "Target: riscv64imad-zisk-zkvm-elf"
echo ""

. "${SCRIPT_DIR}/build.lib.sh"

cleanup
prepare_repo
build_docker_image
build_in_docker
build_syscalls
copy_manifest

echo "Build completed"
echo "Output: ${OUTPUT_DIR}/libziskos.a"
echo "Size: $(du -h "${OUTPUT_DIR}/libziskos.a" | cut -f1)"
if [ -f "${OUTPUT_DIR}/libziskos.bflat.manifest" ]; then
    echo "Output: ${OUTPUT_DIR}/libziskos.bflat.manifest"
fi
