# libziskos for RISC-V 64-bit

[![Build and Release](https://github.com/nethermindeth/bflat-libziskos/actions/workflows/release.yml/badge.svg)](https://github.com/nethermindeth/bflat-libziskos/actions/workflows/release.yml)
[![GitHub release](https://img.shields.io/github/v/release/nethermindeth/bflat-libziskos)](https://github.com/nethermindeth/bflat-libziskos/releases)
[![GitHub downloads](https://img.shields.io/github/downloads/nethermindeth/bflat-libziskos/total)](https://github.com/nethermindeth/bflat-libziskos/releases)

Static library builds of [ziskos](https://github.com/0xPolygonHermez/zisk) for RISC-V 64-bit architecture with IMA extensions, suitable for [Nethermind's bflat](https://github.com/nethermindeth/bflat-riscv64) compiler. Additionally it includes a thin layer with C# bindings.

## About

This project provides pre-built static libraries for the zisk zkVM runtime, compiled for RISC-V 64-bit targets. The builds are specifically configured for:

- **ISA**: RV64IMAD (Integer, Multiplication, Atomic, Double-precision FP)
- **ABI**: LP64D
- **Target OS**: zkvm
- **Vendor**: zisk

The reason for choosing this target over rv64im is pretty simple. It is much more compatible with [dotnet-riscv](https://github.com/nethermindeth/dotnet-riscv) images already used in bflat. Still, these binaries are normally guaranteed to have no C, A, D instructions.

## Artifacts

Each release includes:

- `lib.a` - static library with ziskos functionality (precompiles).
- `lib.dll` - .NET managed library.

## Usage

Use bflat's ability to link to external libraries and provide the link to this repository.

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
export ZISK_TAG=v0.15.0
```

3. Run the build script:
```bash
./build.sh
```

4. Find the built libraries in the `output/` directory:
```bash
ls -lh output/
```

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.