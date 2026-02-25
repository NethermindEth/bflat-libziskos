using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public static unsafe partial class ZiskOsPrecompile
    {
        private const string NativeLibraryName = "__Internal";

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
    }
}