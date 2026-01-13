using Xunit;

namespace SharpLzo.Tests
{
    public class DecompressionTests
    {
        [Fact]
        public void CanDecompress()
        {
            var data = GetData();
            var compressed = Lzo.Compress(data);
            var decompressed = Lzo.Decompress(compressed);

            Assert.NotNull(compressed);
            Assert.NotEqual(data, compressed);
            Assert.NotNull(decompressed);
            Assert.Equal(data, decompressed);
        }

        [Theory]
        [InlineData(CompressionMode.Lzo1x_1)]
        [InlineData(CompressionMode.Lzo1x_999)]
        public void CanDecompressWithMode(CompressionMode compressionMode)
        {
            var data = GetData();
            var compressed = Lzo.Compress(compressionMode, data);
            var decompressed = Lzo.Decompress(compressed);

            Assert.NotNull(compressed);
            Assert.NotEqual(data, compressed);
            Assert.NotNull(decompressed);
            Assert.Equal(data, decompressed);
        }

        [Fact]
        public void CanTryDecompress()
        {
            var data = GetData();
            var compressResult = Lzo.TryCompress(data, out var compressed);
            var decompressResult = Lzo.TryDecompress(compressed, out var decompressed);

            Assert.Equal(LzoResult.OK, compressResult);
            Assert.NotNull(compressed);
            Assert.NotEqual(data, compressed);

            Assert.Equal(LzoResult.OK, decompressResult);
            Assert.NotNull(decompressed);
            Assert.Equal(data, decompressed);
        }

        [Theory]
        [InlineData(CompressionMode.Lzo1x_1)]
        [InlineData(CompressionMode.Lzo1x_999)]
        public void CanTryDecompressWithMode(CompressionMode compressionMode)
        {
            var data = GetData();
            var compressResult = Lzo.TryCompress(compressionMode, data, out var compressed);
            var decompressResult = Lzo.TryDecompress(compressed, out var decompressed);

            Assert.Equal(LzoResult.OK, compressResult);
            Assert.NotNull(compressed);
            Assert.NotEqual(data, compressed);

            Assert.Equal(LzoResult.OK, decompressResult);
            Assert.NotNull(decompressed);
            Assert.Equal(data, decompressed);
        }

        [Theory]
        [InlineData(CompressionMode.Lzo1x_1)]
        [InlineData(CompressionMode.Lzo1x_999)]
        public void CanTryDecompressWithSpan(CompressionMode compressionMode)
        {
            var data = GetData();
            var compressed = new byte[data.Length + data.Length / 16 + 64 + 3];
            var decompressed = new byte[data.Length];
            var compressResult = Lzo.TryCompress(compressionMode, data, data.Length, compressed, out var compressedLength);
            var decompressResult = Lzo.TryDecompress(compressed, compressedLength, decompressed, out var decompressedLength);

            Assert.Equal(LzoResult.OK, compressResult);
            Assert.True(compressedLength > 0);
            Assert.NotNull(compressed);
            Assert.NotEqual(data, compressed);

            Assert.Equal(LzoResult.OK, decompressResult);
            Assert.Equal(data.Length, decompressedLength);
            Assert.NotNull(decompressed);
            Assert.Equal(data, decompressed);
        }

        [Theory]
        [InlineData(CompressionMode.Lzo1x_1)]
        [InlineData(CompressionMode.Lzo1x_999)]
        public void CanTryDecompressWithSpanAndWorkMemory(CompressionMode compressionMode)
        {
            var data = GetData();
            var workMemory = new byte[Lzo.WorkMemorySize];
            var compressed = new byte[data.Length + data.Length / 16 + 64 + 3];
            var decompressed = new byte[data.Length];
            var compressResult = Lzo.TryCompress(compressionMode, data, data.Length, compressed, out var compressedLength, workMemory);
            var decompressResult = Lzo.TryDecompress(compressed, compressedLength, decompressed, out var decompressedLength);

            Assert.Equal(LzoResult.OK, compressResult);
            Assert.True(compressedLength > 0);
            Assert.NotNull(compressed);
            Assert.NotEqual(data, compressed);

            Assert.Equal(LzoResult.OK, decompressResult);
            Assert.Equal(data.Length, decompressedLength);
            Assert.NotNull(decompressed);
            Assert.Equal(data, decompressed);
        }

        private static byte[] GetData()
        {
            var rng = new Random();
            var data = new byte[10 * 1024];
            rng.NextBytes(data);
            return data;
        }
    }
}
