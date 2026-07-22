# Official rust image (rustlang/rust only publishes nightly tags).
FROM rust:1.97.1

# The build uses nightly-gated flags (-Z build-std, -Z json-target-spec);
# RUSTC_BOOTSTRAP lets the pinned stable toolchain accept them, so the build
# is reproducible instead of floating with rustlang/rust:nightly.
ENV RUSTC_BOOTSTRAP=1

# Install build dependencies including RISC-V toolchain
RUN apt-get update && apt-get install -y \
  nasm \
  make \
  gcc \
  g++ \
  git \
  libclang-dev \
  gcc-riscv64-linux-gnu \
  g++-riscv64-linux-gnu \
  binutils-riscv64-linux-gnu \
  && rm -rf /var/lib/apt/lists/*

# Add rust-src component for build-std
RUN rustup component add rust-src

# Set environment variables for cross-compilation
ENV CC_riscv64imad_unknown_none_elf=riscv64-linux-gnu-gcc
ENV CXX_riscv64imad_unknown_none_elf=riscv64-linux-gnu-g++
ENV AR_riscv64imad_unknown_none_elf=riscv64-linux-gnu-ar
ENV CARGO_TARGET_RISCV64IMAD_UNKNOWN_NONE_ELF_LINKER=riscv64-linux-gnu-gcc

WORKDIR /workspace
