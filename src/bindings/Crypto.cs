// SPDX-FileCopyrightText: 2026 Demerzel Solutions Limited
// SPDX-License-Identifier: LGPL-3.0-only

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
        ulong* state,
        ulong* message,
        ulong* offset,
        byte final_block
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_fp_to_g1_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_fp_to_g1_c(byte* ret, byte* fp);

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_fp2_to_g2_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_fp2_to_g2_c(byte* ret, byte* fp2);

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_g1_add_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_g1_add_c(byte* ret, byte* a, byte* b);

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_g1_msm_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_g1_msm_c(byte* ret, byte* pairs, nuint num_pairs);

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_g2_add_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_g2_add_c(byte* ret, byte* a, byte* b);

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_g2_msm_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_g2_msm_c(byte* ret, byte* pairs, nuint num_pairs);

    [LibraryImport(NativeLibraryName, EntryPoint = "bls12_381_pairing_check_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bls12_381_pairing_check_c(byte* pairs, nuint num_pairs);

    [LibraryImport(NativeLibraryName, EntryPoint = "bn254_g1_add_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bn254_g1_add_c(byte* p1, byte* p2, byte* ret);

    [LibraryImport(NativeLibraryName, EntryPoint = "bn254_g1_mul_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bn254_g1_mul_c(byte* point, byte* scalar, byte* ret);

    [LibraryImport(NativeLibraryName, EntryPoint = "bn254_pairing_check_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte bn254_pairing_check_c(byte* pairs, nuint num_pairs);

    [LibraryImport(NativeLibraryName, EntryPoint = "keccak256_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial void keccak256_c(byte* input, nuint input_len, byte* output);

    [LibraryImport(NativeLibraryName, EntryPoint = "modexp_bytes_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial nuint modexp_bytes_c(
        byte* base_ptr,
        nuint base_len,
        byte* exp_ptr,
        nuint exp_len,
        byte* modulus_ptr,
        nuint modulus_len,
        byte* result_ptr
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "secp256k1_ecdsa_address_recover_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte secp256k1_ecdsa_address_recover_c(
        byte* sig,
        byte recid,
        byte* msg,
        byte* output
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "secp256k1_ecdsa_verify_and_address_recover_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial byte secp256k1_ecdsa_verify_and_address_recover_c(
        byte* sig,
        byte* msg,
        byte* pk,
        byte* output
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "secp256r1_ecdsa_verify_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool secp256r1_ecdsa_verify_c(byte* msg, byte* sig, byte* pk);

    [LibraryImport(NativeLibraryName, EntryPoint = "sha256_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial void sha256_c(byte* input, nuint input_len, byte* output);

    [LibraryImport(NativeLibraryName, EntryPoint = "verify_kzg_proof_c")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U1)]
    public static partial bool verify_kzg_proof_c(byte* z, byte* y, byte* commitment, byte* proof);
}