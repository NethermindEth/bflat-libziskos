FROM rustlang/rust@sha256:3bd35cf08d640be132d2d7cff92f5fc25414e90d71d60c7c2471792f22c5aadd

# Install build dependencies including RISC-V toolchain
RUN apt-get update && apt-get install -y \
  nasm \
  make \
  gcc \
  g++ \
  git \
  gcc-riscv64-linux-gnu \
  g++-riscv64-linux-gnu \
  binutils-riscv64-linux-gnu \
  libclang-dev \
  clang \
  && rm -rf /var/lib/apt/lists/*

# Add rust-src component for build-std
RUN rustup component add rust-src

# Set environment variables for cross-compilation
ENV CC_riscv64imad_unknown_none_elf=riscv64-linux-gnu-gcc
ENV CXX_riscv64imad_unknown_none_elf=riscv64-linux-gnu-g++
ENV AR_riscv64imad_unknown_none_elf=riscv64-linux-gnu-ar
ENV CARGO_TARGET_RISCV64IMAD_UNKNOWN_NONE_ELF_LINKER=riscv64-linux-gnu-gcc

WORKDIR /workspace
