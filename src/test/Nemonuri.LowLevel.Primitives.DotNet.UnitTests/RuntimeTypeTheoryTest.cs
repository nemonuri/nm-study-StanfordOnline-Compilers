using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit.Sdk;
using Xunit.v3;
using static Nemonuri.LowLevel.Primitives.DotNet.UnitTests.TestTypeInfo;

namespace Nemonuri.LowLevel.Primitives.DotNet.UnitTests;

public class RuntimeTypeTheoryTest
{
    private readonly ITestOutputHelper _out;

    public RuntimeTypeTheoryTest(ITestOutputHelper @out)
    {
        _out = @out;
    }


    [Theory]
    [MemberData(nameof(Datas1))]
    public void TestSizeOf(string typeName, int expectedSize)
    {
        // Arrange
        Type? foundType = null;
        Assembly foundAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .First(assem => { foundType = assem.GetType(typeName, false); return foundType is not null; });

        _out.WriteLine(foundAssembly.GetName().ToString());
        
        // Act
        int actualSize = RuntimeTypeTheory.SizeOf(foundType!.TypeHandle);
        
        // Assert
        Assert.Equal(expectedSize, actualSize);
    }

    public static TheoryData<string, int> Datas1 => new 
    (
        new TestTypeInfo[]
        {
            Create<int>(), Create<nint>(), Create<string>(),
            Create<System.Index>(), Create<System.Range>(), Create<CancellationToken>(),
            Create<int?>(), Create<char?>(),
            Create<NoMember>(), Create<DesiredSize64>(), Create<ByteEnum>(),
            Create<DesiredSizeTooSmall>(), Create<WithPointer>(), Create<WithObject>()
        }
        .Select(static a => (a.TypeObject.FullName!, a.Size))
    );


#if false
    [Theory]
    [InlineType<int>] [InlineType<System.IntPtr>] [InlineType<StrongBox<string>>]
    [InlineType<System.Index>] [InlineType<System.Range>] [InlineType<CancellationToken>]
    [InlineType<int?>] [InlineType<char?>]
    [InlineType<NoMember>] [InlineType<DesiredSize64>] [InlineType<ByteEnum>]
    [InlineType<DesiredSizeTooSmall>] [InlineType<WithPointer>] [InlineType<WithObject>]
    public unsafe void TestSize_Generic<T>(T _)
    {
        // Arrange
        Type typ = typeof(T);
        _out.WriteLine($"Type: {typ.AssemblyQualifiedName}");
        int expectedSize = Unsafe.SizeOf<T>();
        _out.WriteLine($"Unsafe.SizeOf: {expectedSize}");
        
        // Act
        int actualSize = RuntimeTypeTheory.SizeOf(typ.TypeHandle);
        
        // Assert
        Assert.Equal(expectedSize, actualSize);
    }
#endif


    private struct NoMember {}

    [StructLayout(LayoutKind.Sequential, Size = 64)]
    private struct DesiredSize64 {}

    private enum ByteEnum : byte { None = 0 }

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    private struct DesiredSizeTooSmall
    {
        public char c1, c2, c3;
        public int i1, i2, i3;
        public long l1, l2, l3;
    }

    private unsafe struct WithPointer
    {
        public void* p1;
    }

    private struct WithObject
    {
        public short s1;
        public object o1;
        public byte b1;
        public CancellationToken ct1;
        public byte b2;
    }
    
    

}
