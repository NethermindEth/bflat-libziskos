function cleanup() {
    # Clean up previous builds
    echo "Cleaning up previous builds..."
    rm -rf "${TMP_DIR}" "${OUTPUT_DIR}"
    mkdir -p "${TMP_DIR}" "${OUTPUT_DIR}"
}

function prepare_repo() {
    # Clone the repository
    echo "Cloning zisk repository (tag: ${ZISK_TAG})..."
    git clone --depth 1 --branch "${ZISK_TAG}" "${ZISK_REPO}" "${TMP_DIR}/zisk" || fail "Failed to clone zisk repository"

    # Apply patch
    echo "Applying crate type patch..."
    pushd "${TMP_DIR}/zisk"

    cat > /tmp/ziskos.patch << 'EOF'
diff --git a/ziskos/entrypoint/Cargo.toml b/ziskos/entrypoint/Cargo.toml
index 5974a596..e6668a6f 100644
--- a/ziskos/entrypoint/Cargo.toml
+++ b/ziskos/entrypoint/Cargo.toml
@@ -7,6 +7,9 @@ keywords = { workspace = true }
 repository = { workspace = true }
 categories = { workspace = true }

+[lib]
+crate-type = ["staticlib", "rlib"]
+
 [dependencies]
 lib-c = { workspace = true }

EOF

    git apply /tmp/ziskos.patch

    # Copy custom target spec
    echo "Copying custom target specification..."
    cp "${SCRIPT_DIR}/riscv64imad-zisk-zkvm-elf.json" . || fail "Failed to copy custom target specification"

    popd
}

function build_docker_image() {
    # Build Docker image
    echo "Building Docker image..."
    docker build -t ziskos-builder "${SCRIPT_DIR}" || fail "Failed to build Docker image"
}

function build_in_docker() {
    # Build in Docker
    pushd "${TMP_DIR}/zisk"
        echo "Building with Docker..."
        docker run --rm \
        -v "$(pwd):/workspace" \
        -w /workspace \
        ziskos-builder \
        bash -c "
            set -e
            echo 'Patching lib-c/build.rs for zkvm target...'
            sed -i '7a\\    if env::var(\"CARGO_CFG_TARGET_OS\").unwrap_or_default() == \"zkvm\" {\\n        println!(\"cargo:rustc-cfg=feature=\\\\\"no_lib_link\\\\\"\");\\n        return;\\n    }\\n' lib-c/build.rs

            echo 'Building ziskos entrypoint for riscv64imad-zisk-zkvm-elf...'
            cd ziskos/entrypoint
            cargo +nightly build --release --target /workspace/riscv64imad-zisk-zkvm-elf.json -Z build-std=std,panic_abort -Z json-target-spec

            echo 'Build completed!'
        " || fail "Failed to build ziskos entrypoint"

        # Copy the built library
        echo "Copying built library..."
        BUILT_LIB="target/riscv64imad-zisk-zkvm-elf/release/libziskos.a"
        if [ -f "${BUILT_LIB}" ]; then
            cp "${BUILT_LIB}" "../../${OUTPUT_DIR}/libziskos.a" || fail "Failed to copy built library"
            echo "Library copied to ${OUTPUT_DIR}/libziskos.a"
        else
            echo "Error: Built library not found at ${BUILT_LIB}"
            echo "Searching for available libraries..."
            find target -name "*.a" -type f
            exit 1
        fi
    popd
}

function build_dotnet() {
    # Build .NET library if the project exists
    if [ -f "dotnet/zisklib.riscv64.csproj" ] ; then
        echo "Building .NET library..."
        docker run --rm \
        -v "$(pwd):/workspace" \
        -w /workspace \
        mcr.microsoft.com/dotnet/sdk:10.0 \
        bash -c "
            set -e
            echo 'Building zisklib.riscv64.csproj...'
            dotnet build dotnet/zisklib.riscv64.csproj -c:Release || exit 1
            echo '.NET build completed!'
        " || fail "Failed to build .NET library"

        # Copy the built DLL
        echo "Copying built .NET library..."
        BUILT_DLL="dotnet/bin/Release/net10.0/linux-riscv64/zisklib.dll"
        if [ -f "${BUILT_DLL}" ]; then
            cp "${BUILT_DLL}" "${OUTPUT_DIR}/lib.dll" || fail "Failed to copy built .NET library"
            echo ".NET library copied to ${OUTPUT_DIR}/lib.dll"
        else
            echo "Warning: .NET library not found at ${BUILT_DLL}"
            echo "Searching for available DLLs..."
            find dotnet/bin -name "*.dll" -type f || true
        fi
    else
        fail "Failed to find zisklib project"
    fi
}
