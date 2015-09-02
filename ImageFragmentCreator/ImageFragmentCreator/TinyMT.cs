// TinyMT.cs

using System;
using System.Runtime.InteropServices;

namespace ToyBox {

    public static class MyRandom
    {
        private static TinyMT rand;

        static MyRandom()
        {
            rand = new TinyMT();
            rand.Jump32((ulong)new Random().Next());
        }

        public static uint Next(uint max)
        {
            return (rand.GenerateUInt() / 100) % max;
        }
    }

	public class TinyMT {
		public struct tinymt32_t {
			uint status1,status2,status3,status4;	// C だと uint status[4]
			uint mat1;
			uint mat2;
			uint tmat;

			public tinymt32_t(uint inMat1,uint inMat2,uint inTMat) {
				status1=status2=status3=status4=4;
				mat1=inMat1;
				mat2=inMat2;
				tmat=inTMat;
			}
		};

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public void tinymt32_init(ref tinymt32_t outRandStatus,int inSeed);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public unsafe void tinymt32_init_by_array(ref tinymt32_t outRandStatus,
														 UInt32* inInitKey,int inKeyLength);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public uint tinymt32_generate_uint32(ref tinymt32_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public float tinymt32_generate_float(ref tinymt32_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public float tinymt32_generate_floatOO(ref tinymt32_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public float tinymt32_generate_floatOC(ref tinymt32_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public float tinymt32_generate_float01(ref tinymt32_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public float tinymt32_generate_float12(ref tinymt32_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public double tinymt32_generate_32double(ref tinymt32_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public void tinymt32_jump(ref tinymt32_t inRandStatus,
												UInt64 inLowerStep,UInt64 inUpperStep,
												string inPolyStr);

		public struct tinymt64_t {
			ulong status1,status2;	// uint64_t status[2];
			uint mat1;
			uint mat2;
			ulong tmat;

			public tinymt64_t(uint inMat1,uint inMat2,ulong inTMat) {
				status1=status2=0;
				mat1=inMat1;
				mat2=inMat2;
				tmat=inTMat;
			}
		};

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public void tinymt64_init(ref tinymt64_t outRandStatus,int inSeed);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public unsafe void tinymt64_init_by_array(ref tinymt64_t outRandStatus,
														 UInt64* inInitKey,int inKeyLength);
	
		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public UInt64 tinymt64_generate_uint64(ref tinymt64_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public double tinymt64_generate_double(ref tinymt64_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public double tinymt64_generate_doubleOO(ref tinymt64_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public double tinymt64_generate_doubleOC(ref tinymt64_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public double tinymt64_generate_double01(ref tinymt64_t inRandStatus);

		[DllImport("TinyMT.dll",CallingConvention=CallingConvention.Cdecl)]
		extern static public double tinymt64_generate_double12(ref tinymt64_t inRandStatus);

		tinymt32_t mRand32Status=new tinymt32_t(0x8f7011ee,0xfc78ff1f,0x3793fdff);
		tinymt64_t mRand64Status=new tinymt64_t(0xfa051f40,0xffd0fff4,0x58d02ffeffbfffbc);
		string kJumpPolyStr="d8524022ed8dff4a8dcc50c798faba43";

		public TinyMT() {
			tinymt32_init(ref mRand32Status, 1);
			tinymt64_init(ref mRand64Status, 1);
		}

		public uint GenerateUInt() {
			return tinymt32_generate_uint32(ref mRand32Status);
		}
	
		public float GenerateFloat() {
			return tinymt32_generate_float(ref mRand32Status);
		}

		public float GenerateFloatOO() {
			return tinymt32_generate_floatOO(ref mRand32Status);
		}

		public float GenerateFloatOC() {
			return tinymt32_generate_floatOC(ref mRand32Status);
		}

		public float GenerateFloat01() {
			return tinymt32_generate_float01(ref mRand32Status);
		}

		public float GenerateFloat12() {
			return tinymt32_generate_float12(ref mRand32Status);
		}

		public double Generate32Double() {
			return tinymt32_generate_32double(ref mRand32Status);
		}

		public void Jump32(ulong inStep) {
			tinymt32_jump(ref mRand32Status, inStep,0,kJumpPolyStr);
		}

		public void Jump32(ulong inHighStep,ulong inLowStep) {
			tinymt32_jump(ref mRand32Status,inLowStep,inHighStep,kJumpPolyStr);
		}

		// [0,2^64)
		public ulong GenerateUInt64() {
			return tinymt64_generate_uint64(ref mRand64Status);
		}

		// [0,1)
		public double GenerateDouble() {
			return tinymt64_generate_double(ref mRand64Status);
		}

		// (0,1)
		public double GenerateDoubleOO() {
			return tinymt64_generate_doubleOO(ref mRand64Status);
		}

		// (0,1]
		public double GenerateDoubleOC() {
			return tinymt64_generate_doubleOC(ref mRand64Status);
		}

		// [0,1)
		public double GenerateDouble01() {
			return tinymt64_generate_double01(ref mRand64Status);
		}

		// [1,2)
		public double GenerateDouble12() {
			return tinymt64_generate_double12(ref mRand64Status);
		}
	}
}
