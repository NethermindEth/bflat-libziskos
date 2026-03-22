function cleanup() {
    # Clean up previous builds
    echo "Cleaning up previous builds..."
    rm -rf "${TMP_DIR}" "${OUTPUT_DIR}"
    mkdir -p "${TMP_DIR}" "${OUTPUT_DIR}"
}

function prepare_repo() {
    # Clone the repository
    echo "Cloning zisk repository (ref: ${ZISK_REF})..."
    ZISK_DIR="${TMP_DIR}/zisk"

    git init "${ZISK_DIR}"
    cd "${ZISK_DIR}"
    git remote add origin "${ZISK_REPO}"
    git fetch --depth=1 origin "${ZISK_REF}"
    git checkout --detach FETCH_HEAD

    ZISK_COMMIT=$(git -C "${ZISK_DIR}" rev-parse HEAD)
    echo "Zisk commit: ${ZISK_COMMIT}"

    # Apply patch
    echo "Applying crate type patch..."
    pushd "${ZISK_DIR}"

    git apply "${SCRIPT_DIR}/cargo.toml.patch" || fail "Failed to apply crate type patch"

    # Copy custom target spec
    echo "Copying custom target specification..."
    cp "${SCRIPT_DIR}/riscv64imad-zisk-zkvm-elf.json" . || fail "Failed to copy custom target specification"

    # Copy patch file
    echo "Copying entrypoint patch..."
    cp "${SCRIPT_DIR}/entrypoint.patch" . || fail "Failed to copy patch file"

    popd
}

function build_docker_image() {
    # Build Docker image
    echo "Building Docker image..."
    docker build -t ziskos-builder "${SCRIPT_DIR}" || fail "Failed to build Docker image"
}

function build_in_docker() {
    # Build in Docker
    pushd "${ZISK_DIR}"
        echo "Building with Docker..."
        docker run --rm \
        -v "$(pwd):/workspace" \
        -w /workspace \
        ziskos-builder \
        bash -c "
            set -e
            echo 'Patching lib-c/build.rs for zkvm target...'
            sed -i '3a\\use std::env;' lib-c/build.rs
            sed -i '9a\\    if env::var(\"CARGO_CFG_TARGET_OS\").unwrap_or_default() == \"zkvm\" {\\n        println!(\"cargo:rustc-cfg=feature=\\\\\"no_lib_link\\\\\"\");\\n        return;\\n    }' lib-c/build.rs

            echo 'Building ziskos entrypoint for riscv64imad-zisk-zkvm-elf...'

            echo 'Applying patches for no_entrypoint feature...'
            # Apply patch to wrap _start, _zisk_main, memcpy, memmove and replace sys_alloc_aligned
            patch -p1 -l < /workspace/entrypoint.patch || exit 1

            cd ziskos/entrypoint

            # Add no_entrypoint to existing [features] section
            sed -i '/^\[features\]/a no_entrypoint = []' Cargo.toml

            cargo +nightly build --release --target /workspace/riscv64imad-zisk-zkvm-elf.json -Z build-std=std,panic_abort -Z json-target-spec --features no_entrypoint

            echo 'Build completed!'
        " || fail "Failed to build ziskos entrypoint"

        # Copy the built library
        echo "Copying built library..."
        BUILT_LIB="target/riscv64imad-zisk-zkvm-elf/release/libziskos.a"
        if [ -f "${BUILT_LIB}" ]; then
            cp "${BUILT_LIB}" "${OUTPUT_DIR}/libziskos.a" || fail "Failed to copy built library"
            echo "Library copied to ${OUTPUT_DIR}/libziskos.a"
        else
            echo "Error: Built library not found at ${BUILT_LIB}"
            echo "Searching for available libraries..."
            find target -name "*.a" -type f
            exit 1
        fi
    popd
}

function build_syscalls() {
    # Build zisk_syscalls.S and add to lib.a
    if [ -f "${ROOT_DIR}/src/zisk_syscalls/zisk_syscalls.S" ]; then
        echo "Building zisk_syscalls.S..."
        docker run --rm \
        -v "${ROOT_DIR}/src/zisk_syscalls:/syscalls" \
        -v "${OUTPUT_DIR}:/output" \
        -w /syscalls \
        ziskos-builder \
        bash -c "
            set -e
            echo 'Assembling zisk_syscalls.S...'
            riscv64-linux-gnu-as --march=rv64ima --mabi=lp64 zisk_syscalls.S -o zisk_syscalls.o || exit 1

            echo 'Adding zisk_syscalls.o to lib.a...'
            cd /output
            riscv64-linux-gnu-ar r libziskos.a /syscalls/zisk_syscalls.o || exit 1
            riscv64-linux-gnu-ranlib libziskos.a || exit 1

            echo 'zisk_syscalls added to lib.a'
        " || fail "Failed to build and add zisk_syscalls"
    else
        echo "Skipping zisk_syscalls - zisk_syscalls.S not found"
    fi
}

function copy_manifest() {
    # Copy manifest
    echo "Copying bflat-manifest.json..."
    if [ -f "${ROOT_DIR}/bflat-manifest.json" ]; then
        cp "${ROOT_DIR}/bflat-manifest.json" "${OUTPUT_DIR}/libziskos.bflat.manifest" || fail "Failed to copy bflat-manifest.json"

        if [ -n "${ZISK_COMMIT}" ] && command -v jq &>/dev/null; then
            jq --arg hash "${ZISK_COMMIT}" '. + {zisk_ref_hash: $hash}' \
                "${OUTPUT_DIR}/libziskos.bflat.manifest" > "${OUTPUT_DIR}/libziskos.bflat.manifest.tmp" \
                && mv "${OUTPUT_DIR}/libziskos.bflat.manifest.tmp" "${OUTPUT_DIR}/libziskos.bflat.manifest"
            echo "Injected zisk_ref_hash: ${ZISK_COMMIT}"
        fi

        echo "Manifest at ${OUTPUT_DIR}/libziskos.bflat.manifest"
    fi
}
