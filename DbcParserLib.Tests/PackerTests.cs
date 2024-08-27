using NUnit.Framework;
using DbcParserLib.Model;
using System;

namespace DbcParserLib.Tests
{
    public class PackerTests
    {
        [Test]
        public void SimplePackingTestSigned()
        {
            var sig = new Signal
            {
                Length = 14,
                StartBit = 2,
                ValueType = DbcValueType.Signed,
                ByteOrder = 1, // 0 = Big Endian (Motorola), 1 = Little Endian (Intel)
                Factor = 0.01,
                Offset = 20
            };

            var txMsg = Packer.TxSignalPack(-34.3, sig);
            Assert.AreEqual(43816, txMsg);

            var val = Packer.RxSignalUnpack(txMsg, sig);
            Assert.AreEqual(-34.3, val, 1e-2);


            var byteMsg = new byte[8];
            Packer.TxSignalPack(byteMsg, - 34.3, sig);
            Assert.AreEqual(43816, BitConverter.ToUInt64(byteMsg));

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(-34.3, valbyte, 1e-2);
        }

        [TestCase((ushort)0, 3255382835ul)]
        [TestCase((ushort)2, 13021531340ul)]
        [TestCase((ushort)5, 104172250720ul)]
        [TestCase((ushort)12, 13334048092160ul)]
        public void FloatLittleEndianValuePackingTest(ushort start, ulong packet)
        {
            var sig = new Signal
            {
                Length = 32,
                StartBit = start,
                ValueType = DbcValueType.IEEEFloat,
                ByteOrder = 1, // 0 = Big Endian (Motorola), 1 = Little Endian (Intel)
                Factor = 1,
                Offset = 0
            };

            var expected = -34.3f;
            var txMsg = Packer.TxSignalPack(expected, sig);
            Assert.AreEqual(packet, txMsg);

            var val = Packer.RxSignalUnpack(txMsg, sig);
            Assert.AreEqual(expected, val, 1e-2);


            var byteMsg = new byte[8];
            Packer.TxSignalPack(byteMsg, expected, sig);
            Assert.AreEqual(packet, BitConverter.ToUInt64(byteMsg));

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(expected, valbyte, 1e-2);
        }

        [TestCase((ushort)0, 439799153665ul)]
        [TestCase((ushort)2, 655406731270ul)]
        [TestCase((ushort)5, 828061286960ul)]
        [TestCase((ushort)12, 105991844730880ul)]
        public void FloatBigEndianValuePackingTest(ushort start, ulong packet)
        {
            var sig = new Signal
            {
                Length = 32,
                StartBit = start,
                ValueType = DbcValueType.IEEEFloat,
                ByteOrder = 0, // 0 = Big Endian (Motorola), 1 = Little Endian (Intel)
                Factor = 1,
                Offset = 0
            };

            var value = -34.3f;
            var txMsg = Packer.TxSignalPack(value, sig);
            Assert.AreEqual(packet, txMsg);

            var val = Packer.RxSignalUnpack(txMsg, sig);
            Assert.AreEqual(value, val, 1e-2);


            var byteMsg = new byte[8];
            Packer.TxSignalPack(byteMsg, value, sig);
            Assert.AreEqual(packet, BitConverter.ToUInt64(byteMsg));

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(value, valbyte, 1e-2);
        }

        [Test]
        public void DoubleLittleEndianValuePackingTest()
        {
            var sig = new Signal
            {
                Length = 64,
                StartBit = 0,
                ValueType = DbcValueType.IEEEDouble,
                ByteOrder = 1, // 0 = Big Endian (Motorola), 1 = Little Endian (Intel)
                Factor = 1,
                Offset = 0
            };

            var expected = -34.3567;
            var txMsg = Packer.TxSignalPack(expected, sig);
            Assert.AreEqual(13853404129830452697, txMsg);

            var val = Packer.RxSignalUnpack(txMsg, sig);
            Assert.AreEqual(expected, val, 1e-2);


            var byteMsg = new byte[8];
            Packer.TxSignalPack(byteMsg, expected, sig);
            Assert.AreEqual(13853404129830452697, BitConverter.ToUInt64(byteMsg));

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(expected, valbyte, 1e-2);
        }

        [Test]
        public void DoubleBigEndianValuePackingTest()
        {
            var sig = new Signal
            {
                Length = 64,
                StartBit = 7,
                ValueType = DbcValueType.IEEEDouble,
                ByteOrder = 0, // 0 = Big Endian (Motorola), 1 = Little Endian (Intel)
                Factor = 1,
                Offset = 0
            };

            var expected = -34.35564;
            var txMsg = Packer.TxSignalPack(expected, sig);
            Assert.AreEqual(2419432028705210816, txMsg);

            var val = Packer.RxSignalUnpack(txMsg, sig);
            Assert.AreEqual(expected, val, 1e-2);


            var byteMsg = new byte[8];
            Packer.TxSignalPack(byteMsg, expected, sig);
            Assert.AreEqual(2419432028705210816, BitConverter.ToUInt64(byteMsg));

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(expected, valbyte, 1e-2);
        }

        [Test]
        public void SimplePackingTestNonSigned()
        {
            var sig = new Signal
            {
                Length = 16,
                StartBit = 24,
                ValueType = DbcValueType.Signed,
                ByteOrder = 1, // 0 = Big Endian (Motorola), 1 = Little Endian (Intel)
                Factor = 0.125,
                Offset = 0
            };

            var txMsg = Packer.TxSignalPack(800, sig);
            Assert.AreEqual(107374182400, txMsg);

            var val = Packer.RxSignalUnpack(txMsg, sig);
            Assert.AreEqual(800, val);

            val = Packer.RxSignalUnpack(9655716608953581040, sig);
            Assert.AreEqual(800, val);
        }

        [Test]
        public void SimplePackingMultiPackBigEndian()
        {
            var sig1 = new Signal
            {
                StartBit = 7,
                Length = 10,
                ValueType = DbcValueType.Unsigned,
                ByteOrder = 0,
                Factor = 1,
                Offset = 0
            };

            var sig2 = new Signal
            {
                StartBit = 13,
                Length = 6,
                ValueType = DbcValueType.Unsigned,
                ByteOrder = 0,
                Factor = 1,
                Offset = 0
            };

            var sig3 = new Signal
            {
                StartBit = 23,
                Length = 32,
                ValueType = DbcValueType.Unsigned,
                ByteOrder = 0,
                Factor = 1,
                Offset = 0
            };

            var sig4 = new Signal
            {
                StartBit = 55,
                Length = 16,
                ValueType = DbcValueType.Unsigned,
                ByteOrder = 0,
                Factor = 1,
                Offset = 0
            };

            ulong TxMsg = 0;
            TxMsg |= Packer.TxSignalPack(0, sig1);
            TxMsg |= Packer.TxSignalPack(63, sig2);
            TxMsg |= Packer.TxSignalPack(0, sig3);
            TxMsg |= Packer.TxSignalPack(ushort.MaxValue, sig4);

            Assert.AreEqual(18446462598732857088, TxMsg);


            byte[] txMsg = new byte[8];
            Packer.TxSignalPack(txMsg, 0, sig1);
            Packer.TxSignalPack(txMsg, 63, sig2);
            Packer.TxSignalPack(txMsg, 0, sig3);
            Packer.TxSignalPack(txMsg, ushort.MaxValue, sig4);

            Assert.AreEqual(18446462598732857088, BitConverter.ToUInt64(txMsg));
        }

        [Test]
        public void PackingTest64Bit()
        {
            var sig = new Signal
            {
                Length = 64,
                StartBit = 0,
                ValueType = DbcValueType.Signed,
                ByteOrder = 1, // 0 = Big Endian (Motorola), 1 = Little Endian (Intel)
                Factor = 1e-16,
                Offset = 0
            };

            var txMsg = Packer.TxSignalPack(396.31676720860366, sig);
            Assert.AreEqual(3963167672086036480, txMsg);

            var val = Packer.RxSignalUnpack(txMsg, sig);
            Assert.AreEqual(396.31676720860366, val);


            var byteMsg = new byte[8];
            Packer.TxSignalPack(byteMsg, 396.31676720860366, sig);
            Assert.AreEqual(3963167672086036480, BitConverter.ToUInt64(byteMsg));

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(396.31676720860366, valbyte);
        }

        //Although Pack has one output per value/signal. Unpack can produce the same result for two different RxMsg64 inputs
        [Test]
        public void UnPackingTestMultipleUnpacks()
        {
            var sig = new Signal
            {
                Length = 8,
                StartBit = 56,
                ValueType = DbcValueType.Unsigned,
                ByteOrder = 1, // 0 = Big Endian (Motorola), 1 = Little Endian (Intel)
                Factor = 1,
                Offset = -125
            };

            var txMsg = Packer.TxSignalPack(8, sig);
            Assert.AreEqual(9583660007044415488, txMsg);

            var val = Packer.RxSignalUnpack(txMsg, sig);
            Assert.AreEqual(8, val);

            val = Packer.RxSignalUnpack(9655716608953581040, sig);
            Assert.AreEqual(8, val);


            var byteMsg = new byte[8];
            Packer.TxSignalPack(byteMsg, 8, sig);
            Assert.AreEqual(9583660007044415488, BitConverter.ToUInt64(byteMsg));

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(8, valbyte);

            valbyte = Packer.RxSignalUnpack(BitConverter.GetBytes(9655716608953581040), sig);
            Assert.AreEqual(8, valbyte);
        }

        //A bit packing test with a length of 1 (to test signals with < 8 bits)
        [Test]
        public void BitPackingTest1()
        {
            var sig = new Signal
            {
                Length = 1,
                StartBit = 18,
                ValueType = DbcValueType.Unsigned,
                ByteOrder = 1, // 0 = Big Endian (Motorola), 1 = Little Endian (Intel)
                Factor = 1,
                Offset = 0
            };

            var txMsg = Packer.TxSignalPack(1, sig);
            Assert.AreEqual(262144, txMsg);

            var val = Packer.RxSignalUnpack(txMsg, sig);
            Assert.AreEqual(1, val);

            val = Packer.RxSignalUnpack(140737488617472, sig);
            Assert.AreEqual(1, val);


            var byteMsg = new byte[8];
            Packer.TxSignalPack(byteMsg, 1, sig);
            Assert.AreEqual(262144, BitConverter.ToUInt64(byteMsg));

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(1, valbyte);

            valbyte = Packer.RxSignalUnpack(BitConverter.GetBytes(140737488617472), sig);
            Assert.AreEqual(1, valbyte);
        }

        [Test]
        public void UnsignedPackingTest()
        {
            var sig = new Signal
            {
                Length = 3,
                StartBit = 6,
                ValueType = DbcValueType.Unsigned,
                ByteOrder = 1, // 0 = Big Endian (Motorola), 1 = Little Endian (Intel)
                Factor = 1,
                Offset = 0
            };

            var txMsg = Packer.TxSignalPack(6, sig);
            Assert.AreEqual(384, txMsg);

            var val = Packer.RxSignalUnpack(txMsg, sig);
            Assert.AreEqual(6, val);

            val = Packer.RxSignalUnpack(498806260540323729, sig);
            Assert.AreEqual(6, val);


            var byteMsg = new byte[8];
            Packer.TxSignalPack(byteMsg, 6, sig);
            Assert.AreEqual(384, BitConverter.ToUInt64(byteMsg));

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(6, valbyte);

            valbyte = Packer.RxSignalUnpack(BitConverter.GetBytes(498806260540323729), sig);
            Assert.AreEqual(6, valbyte);
        }

        [Test]
        public void BytePackingBigEndianSmaller8Byte()
        {
            var messageLength = 5;

            var sig = new Signal
            {
                Length = 12,
                StartBit = 13,
                ValueType = DbcValueType.Unsigned,
                ByteOrder = 0,
                Factor = 1,
                Offset = 0
            };

            var byteMsg = new byte[messageLength];

            Packer.TxSignalPack(byteMsg, 4095, sig);
            Assert.AreEqual(byteMsg, new byte[] { 0, 63, 252, 0, 0 });

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(4095, valbyte);
        }

        [Test]
        public void BytePackingLittleEndianSmaller8Byte()
        {
            var messageLength = 5;

            var sig = new Signal
            {
                Length = 12,
                StartBit = 26,
                ValueType = DbcValueType.Unsigned,
                ByteOrder = 1,
                Factor = 1,
                Offset = 0
            };

            var byteMsg = new byte[messageLength];

            Packer.TxSignalPack(byteMsg, 4095, sig);
            Assert.AreEqual(byteMsg, new byte[] {0, 0, 0, 252, 63});

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(4095, valbyte);
        }

        [Test]
        public void BytePackingBigEndianBigger8Byte()
        {
            var messageLength = 24;

            var sig = new Signal
            {
                Length = 12,
                StartBit = 138,
                ValueType = DbcValueType.Unsigned,
                ByteOrder = 0,
                Factor = 1,
                Offset = 0
            };

            var byteMsg = new byte[messageLength];

            Packer.TxSignalPack(byteMsg, 4095, sig);
            Assert.AreEqual(byteMsg, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 255, 128, 0, 0, 0, 0 });

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(4095, valbyte);
        }

        [Test]
        public void BytePackingLittleEndianBigger8Byte()
        {
            var messageLength = 24;

            var sig = new Signal
            {
                Length = 12,
                StartBit = 138,
                ValueType = DbcValueType.Unsigned,
                ByteOrder = 1,
                Factor = 1,
                Offset = 0
            };

            var byteMsg = new byte[messageLength];

            Packer.TxSignalPack(byteMsg, 4095, sig);
            Assert.AreEqual(byteMsg, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 252, 63, 0, 0, 0, 0, 0 });

            var valbyte = Packer.RxSignalUnpack(byteMsg, sig);
            Assert.AreEqual(4095, valbyte);
        }
    }
}