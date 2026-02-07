#!/bin/bash
set -e

function fail() {
    echo $@ >&2
    exit 1
}

# Configuration
ZISK_TAG="${ZISK_TAG}"

if [ -z "$ZISK_TAG" ]; then
    fail "Error: ZISK_TAG is not set"
fi

TMP_DIR="tmp"
ZISK_REPO="https://github.com/0xPolygonHermez/zisk.git"
OUTPUT_DIR="output"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "Building libziskos"
echo "Tag: ${ZISK_TAG}"
echo "Target: riscv64imad-zisk-zkvm-elf"
echo ""

. "${SCRIPT_DIR}/build.lib.sh"

cleanup
prepare_repo
build_docker_image
build_in_docker
build_syscalls
build_dotnet
copy_manifest

echo "Build completed"
echo "Output: ${OUTPUT_DIR}/libziskos.a"
echo "Size: $(du -h ${OUTPUT_DIR}/libziskos.a | cut -f1)"
if [ -f "${OUTPUT_DIR}/lib.dll" ]; then
    echo "Output: ${OUTPUT_DIR}/lib.dll"
    echo "Size: $(du -h ${OUTPUT_DIR}/lib.dll | cut -f1)"
fi
if [ -f "${OUTPUT_DIR}/bflat-manifest.json" ]; then
    echo "Output: ${OUTPUT_DIR}/bflat-manifest.json"
fi
