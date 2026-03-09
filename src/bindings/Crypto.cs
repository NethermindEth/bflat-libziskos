// SPDX-FileCopyrightText: 2026 Demerzel Solutions Limited
// SPDX-License-Identifier: LGPL-3.0-only

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Nethermind.ZiskBindings;

public static unsafe partial class Crypto
{
    private const string NativeLibraryName = "__Internal";

    [LibraryImport(NativeLibraryName, EntryPoint = "blake2b_compress_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial void blake2b_compress_c(
        uint rounds,
        Span<ulong> state,
        ReadOnlySpan<ulong> message,
        ReadOnlySpan<ulong> offset,
        byte final_block
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_fp_to_g1_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_fp_to_g1_c(
        Span<byte> ret,
        ReadOnlySpan<byte> fp
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_fp2_to_g2_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_fp2_to_g2_c(
        Span<byte> ret,
        ReadOnlySpan<byte> fp2
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_g1_add_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_g1_add_c(
        Span<byte> ret,
        ReadOnlySpan<byte> a,
        ReadOnlySpan<byte> b
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_g1_msm_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_g1_msm_c(
        Span<byte> ret,
        ReadOnlySpan<byte> pairs,
        nuint num_pairs
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_g2_add_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_g2_add_c(
        Span<byte> ret,
        ReadOnlySpan<byte> a,
        ReadOnlySpan<byte> b
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_g2_msm_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_g2_msm_c(
        Span<byte> ret,
        ReadOnlySpan<byte> pairs,
        nuint num_pairs
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_pairing_check_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_pairing_check_c(
        ReadOnlySpan<byte> pairs,
        nuint num_pairs
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "bn254_g1_add_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bn254_g1_add_c(
        ReadOnlySpan<byte> p1,
        ReadOnlySpan<byte> p2,
        Span<byte> ret
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "bn254_g1_mul_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bn254_g1_mul_c(
        ReadOnlySpan<byte> point,
        ReadOnlySpan<byte> scalar,
        Span<byte> ret
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "bn254_pairing_check_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bn254_pairing_check_c(
        ReadOnlySpan<byte> pairs,
        nuint num_pairs
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "keccak256_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial void keccak256_c(
        ReadOnlySpan<byte> input,
        nuint input_len,
        Span<byte> output
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "modexp_bytes_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial nuint modexp_bytes_c(
        ReadOnlySpan<byte> base_ptr,
        nuint base_len,
        ReadOnlySpan<byte> exp_ptr,
        nuint exp_len,
        ReadOnlySpan<byte> modulus_ptr,
        nuint modulus_len,
        Span<byte> result_ptr
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "secp256k1_ecdsa_address_recover_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte secp256k1_ecdsa_address_recover_c(
        ReadOnlySpan<byte> sig,
        byte recid,
        ReadOnlySpan<byte> msg,
        Span<byte> output
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "secp256k1_ecdsa_verify_and_address_recover_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte secp256k1_ecdsa_verify_and_address_recover_c(
        ReadOnlySpan<byte> sig,
        ReadOnlySpan<byte> msg,
        ReadOnlySpan<byte> pk,
        Span<byte> output
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "secp256r1_ecdsa_verify_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool secp256r1_ecdsa_verify_c(
        ReadOnlySpan<byte> msg,
        ReadOnlySpan<byte> sig,
        ReadOnlySpan<byte> pk
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "sha256_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial void sha256_c(ReadOnlySpan<byte> input, nuint input_len, Span<byte> output);

    [LibraryImport(NativeLibraryName, EntryPoint = "verify_kzg_proof_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool verify_kzg_proof_c(
        ReadOnlySpan<byte> z,
        ReadOnlySpan<byte> y,
        ReadOnlySpan<byte> commitment,
        ReadOnlySpan<byte> proof
    );
}