using System.Runtime.InteropServices;

namespace SharpLzo
{
    internal static partial class LzoNative
    {
        private const string Library = "liblzo2";

#if NET8_0_OR_GREATER
        [LibraryImport(Library)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        private static partial LzoResult __lzo_init_v2(
            uint v, int s1, int s2, int s3, int s4, int s5, int s6, int s7, int s8, int s9
        );
#else
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        private static extern LzoResult __lzo_init_v2(
            uint v, int s1, int s2, int s3, int s4, int s5, int s6, int s7, int s8, int s9
        );
#endif

#if NET8_0_OR_GREATER
        [LibraryImport(Library)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        private static partial uint lzo_version();
#else
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint lzo_version();
#endif

#if NET8_0_OR_GREATER
        [LibraryImport(Library)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        private static unsafe partial LzoResult lzo1x_1_compress(
            byte* inData, UIntPtr inLength,
            byte* outData, out UIntPtr outLength,
            byte* wrkmem
        );
#else
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe LzoResult lzo1x_1_compress(
            byte* inData, UIntPtr inLength,
            byte* outData, out UIntPtr outLength,
            byte* wrkmem
        );
#endif

#if NET8_0_OR_GREATER
        [LibraryImport(Library)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        private static unsafe partial LzoResult lzo1x_999_compress(
            byte* inData, UIntPtr inLength,
            byte* outData, out UIntPtr outLength,
            byte* wrkmem
        );
#else
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe LzoResult lzo1x_999_compress(
            byte* inData, UIntPtr inLength,
            byte* outData, out UIntPtr outLength,
            byte* wrkmem
        );
#endif

#if NET8_0_OR_GREATER
        [LibraryImport(Library)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        private static unsafe partial LzoResult lzo1x_decompress_safe(
            byte* inData, UIntPtr inLength,
            byte* outData, ref UIntPtr outLength,
            byte* wrkmem
        );
#else
        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe LzoResult lzo1x_decompress_safe(
            byte* inData, UIntPtr inLength,
            byte* outData, ref UIntPtr outLength,
            byte* wrkmem
        );
#endif

        public static LzoResult Init()
        {
            return __lzo_init_v2(1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
        }

        public static uint Version()
        {
            return lzo_version();
        }

        public static unsafe LzoResult Compress(
            CompressionMode mode,
            ReadOnlySpan<byte> inData, int inLength,
            Span<byte> outData, out int outLength,
            Span<byte> wrkmem
        )
        {
            LzoResult result;
            UIntPtr outLengthInternal;

            fixed (byte* inDataPtr = &MemoryMarshal.GetReference(inData))
            fixed (byte* outDataPtr = &MemoryMarshal.GetReference(outData))
            fixed (byte* wrkmemPtr = &MemoryMarshal.GetReference(wrkmem))
            {
                result = mode switch
                {
                    CompressionMode.Lzo1x_1 => lzo1x_1_compress(
                        inDataPtr, new UIntPtr((uint)inLength),
                        outDataPtr, out outLengthInternal,
                        wrkmemPtr
                    ),

                    CompressionMode.Lzo1x_999 => lzo1x_999_compress(
                        inDataPtr, new UIntPtr((uint)inLength),
                        outDataPtr, out outLengthInternal,
                        wrkmemPtr
                    ),

                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
            }

            outLength = (int)outLengthInternal.ToUInt32();
            return result;
        }

        public static unsafe LzoResult DecompressSafe(
            ReadOnlySpan<byte> inData, int inLength,
            Span<byte> outData, out int outLength
        )
        {
            LzoResult result;
            var outLengthInternal = new UIntPtr((uint)outData.Length);

            fixed (byte* inDataPtr = &MemoryMarshal.GetReference(inData))
            fixed (byte* outDataPtr = &MemoryMarshal.GetReference(outData))
            {
                result = lzo1x_decompress_safe(
                    inDataPtr, new UIntPtr((uint)inLength),
                    outDataPtr, ref outLengthInternal,
                    null
                );
            }

            outLength = (int)outLengthInternal.ToUInt32();
            return result;
        }
    }
}
