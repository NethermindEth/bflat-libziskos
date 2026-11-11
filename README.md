# libziskos for RISC-V 64-bit

[![Build and Release](https://github.com/nethermindeth/bflat-libziskos/actions/workflows/build-release.yml/badge.svg)](https://github.com/nethermindeth/bflat-libziskos/actions/workflows/build-release.yml)
[![GitHub release](https://img.shields.io/github/v/release/nethermindeth/bflat-libziskos)](https://github.com/nethermindeth/bflat-libziskos/releases)
[![GitHub downloads](https://img.shields.io/github/downloads/nethermindeth/bflat-libziskos/total)](https://github.com/nethermindeth/bflat-libziskos/releases)

Static library builds of [ziskos](https://github.com/0xPolygonHermez/zisk) for RISC-V 64-bit architecture with IMA extensions, suitable for [Nethermind's bflat](https://github.com/nethermindeth/bflat-riscv64) compiler. The repository also ships the [`Nethermind.ZiskOS.Runtime`](https://www.nuget.org/packages/Nethermind.ZiskOS.Runtime) NuGet package, which embeds the static library for use from .NET projects targeting the Zisk zkVM.

## About

This project provides pre-built static libraries for the zisk zkVM runtime, compiled for RISC-V 64-bit targets. The builds are specifically configured for:

- **ISA**: RV64IMAD (Integer, Multiplication, Atomic, Double-precision FP)
- **ABI**: LP64D
- **Target OS**: zkvm
- **Vendor**: zisk

The reason for choosing this target over rv64im is pretty simple. It is much more compatible with [dotnet-riscv](https://github.com/nethermindeth/dotnet-riscv) images already used in bflat. Still, these binaries are normally guaranteed to have no C, A, D instructions.

## Artifacts

Each release includes:

- `libziskos.a` - static library with ziskos functionality (precompiles) and direct system call wrappers (`zisk_syscalls.S` assembled for rv64ima).
- `libziskos.bflat.manifest` - bflat manifest describing the library, its target triple, and the resolved zisk commit hash. It also instructs bflat to wrap `memcpy`, `memset`, `memmove`, and `memcmp`.
- `zisk_entry.o`, `zisk.ld`, `zisk_sim.ld`, `zisk.bflat.manifest`, `zisk_sim.bflat.manifest` - the ZisK and ZisK simulator platforms for bflat (see [Platform](#platform)).
- `Nethermind.ZiskOS.Runtime` NuGet package - .NET runtime package that bundles `libziskos.a`, the platform files and the manifests for consumption from managed projects.

## Platform

This package also defines the ZisK target for bflat: `--libc zisk` selects
the platform that `platform/` describes, so bflat itself knows nothing about
ZisK. `platform/build.sh` builds it into `output/` (`zisk_entry.o`, `zisk.ld`, `zisk_sim.ld`, `zisk.bflat.manifest` and `zisk_sim.bflat.manifest`):

- `entry.S` is the guest's `_start` and the ZisK halt protocol, plus the initial argv/envp/auxv vector that musl's start-up reads.
- `entry.S` also implements the platform interface that bflat's modules call
  (bflat itself has no code for any one zkVM): `zkvm_platform_exit` (the `a7 = 93` ecall), `zkvm_platform_syscall` (on to musl: ZisK emulates Linux syscalls) and the bridge of the ziskos Rust allocator onto bflat's heap.
- the linker script is the ZisK memory map;
- the platform manifest gives the instruction set, the ABI, the RyuJIT
  options, the defines and the post-link steps, and lists the bflat modules
  that the guest needs (format: `docs/manifest.md` in bflat-riscv64).

The package also defines the `zisk_sim` platform: the same entry point with
the debugging memory map of the ZisK simulator. It links no libziskos.

`--extlib` with a `.bflat.manifest` path reads every `*.bflat.manifest` in that
directory, so the platform and the bindings come together. A guest that does
not use the accelerators can be built with the platform files alone.
`tests/platform/run.sh` runs the unit tests of the entry point under qemu-user.

## Usage

Use bflat's ability to link to external libraries and provide the link to this repository, or reference the `Nethermind.ZiskOS.Runtime` NuGet package from a .NET project.

## Signature

The release workflow signs the `.nupkg` before it publishes it. The build
job lists the package entries (`scripts/sign_package.py list-nupkg`). The
sign job, the reusable workflow `sign.yml` of bflat-riscv64, signs the lists
with the GitHub Actions secret `BFLAT_SIGNING_KEY`; it is the only job that
has the key, and it runs no build. The publish job adds the signatures
(`scripts/sign_package.py embed`) and checks them with the variable
`BFLAT_SIGNING_PUBLIC_KEY`. bflat refuses a package with a bad signature, and an
unsigned package unless `--allow-unsigned` is given. A local
`.bflat.manifest` is not checked. See `docs/signing.md` in bflat-riscv64.
`scripts/sign_package.py` is a copy of the script in bflat-riscv64; keep it
the same.

## Building Locally

### Prerequisites

- Docker
- Git
- Bash

### Build Instructions

1. Clone this repository:

```bash
git clone https://github.com/nethermindeth/bflat-libziskos.git
cd bflat-libziskos
```

2. Set the zisk version tag you want to build:

```bash
export ZISK_REF=v1.4.0-alpha
```

3. Run the build script:

```bash
./build/build.sh
```

4. Find the built artifacts in the `output/` directory:

```bash
ls -lh output/
```

## Build Details

The build process:

1. **Clones the zisk repository** at the specified tag.
2. **Builds a Docker image** based on `rustlang/rust:nightly` with the RISC-V cross-compilation toolchain (`gcc-riscv64-linux-gnu`, `binutils-riscv64-linux-gnu`) and the `rust-src` component for `-Z build-std`.
3. **Applies `entrypoint.patch`** to the cloned `ziskos/entrypoint` crate. The patch wraps `_start`, `_zisk_main`, `memcpy`, `memmove`, and `memcmp` for bflat-side wrap linkage, builds the unmangled `_start`/`_zisk_main` entrypoints, replaces `sys_alloc_aligned`, and exposes `inline_bump_alloc_aligned`. It also patches `lib-c/build.rs` to skip the C++ library compilation for the `zkvm` target.
   `build/secp256k1/secp256k1.patch` then routes zisklib's `k1·G + k2·P` (used by ECDSA recovery) to `straus.rs`: a windowed Straus-Shamir multiplication using signed radix-2^12 digits against positive-only tables of G and 2^128·G (`g_tables.rs`, generated by `gen_tables.py`). The accumulator handles equal points and infinity in place without replaying the walk. The patch also reads and writes the 32-byte big-endian values of `zkvm_secp256k1_ecrecover` with `ld`/`sd` and `rev8` instead of byte by byte.
   `tests/secp256k1/run.sh` checks it on the host against a plain Python recovery.
4. **Adds the `no_entrypoint` feature** to the entrypoint crate so the static library can be linked into a host-provided entrypoint.
5. **Builds the Rust library** with Rust nightly using a custom RISC-V target specification (`riscv64imad-zisk-zkvm-elf`) and `-Z build-std=std,panic_abort`.
6. **Assembles system call wrappers** from `src/zisk_syscalls/zisk_syscalls.S` with `riscv64-linux-gnu-as --march=rv64ima --mabi=lp64`.
7. **Merges syscalls into `libziskos.a`** using `riscv64-linux-gnu-ar` and `riscv64-linux-gnu-ranlib`.
8. **Emits the bflat manifest** (`libziskos.bflat.manifest`), injecting the resolved zisk commit hash as `metadata.zisk_ref_hash` when `jq` is available.

The final `libziskos.a` contains:

- ziskos core functionality (precompiles, runtime)
- Direct system call wrappers for zkVM syscalls

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.
