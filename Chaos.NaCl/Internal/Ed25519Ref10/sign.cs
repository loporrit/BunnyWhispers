﻿using System;
using System.Security.Cryptography;

namespace Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class Ed25519Operations
	{
		/*public static void crypto_sign(
		  byte[] sm, out int smlen,
		   byte[] m, int mlen,
		   byte[] sk
		)
		{
			byte[] az = new byte[64];
			byte[] r = new byte[64];
			byte[] hram = new byte[64];
			GroupElementP3 R;
			int i;

			Helpers.crypto_hash_sha512(az, sk, 0, 32);
			az[0] &= 248;
			az[31] &= 63;
			az[31] |= 64;

			smlen = mlen + 64;
			for (i = 0; i < mlen; ++i) sm[64 + i] = m[i];
			for (i = 0; i < 32; ++i) sm[32 + i] = az[32 + i];
			Helpers.crypto_hash_sha512(r, sm, 32, mlen + 32);
			for (i = 0; i < 32; ++i) sm[32 + i] = sk[32 + i];

			ScalarOperations.sc_reduce(r);
			GroupOperations.ge_scalarmult_base(out R, r, 0);
			GroupOperations.ge_p3_tobytes(sm, 0, ref R);

			Helpers.crypto_hash_sha512(hram, sm, 0, mlen + 64);
			ScalarOperations.sc_reduce(hram);
			var sm32 = new byte[32];
			Array.Copy(sm, 32, sm32, 0, 32);
			ScalarOperations.sc_muladd(sm32, hram, az, r);
			Array.Copy(sm32, 0, sm, 32, 32);
		}*/

		public static void crypto_sign2(
			byte[] sig, int sigoffset,
			byte[] m, int moffset, int mlen,
			byte[] sk, int skoffset)
		{
			byte[] az;
			byte[] r;
			byte[] hram;
			GroupElementP3 R;
		    using SHA512 sha512 = SHA512.Create();
			{
                sha512.TransformFinalBlock(sk, skoffset, 32);
				az = sha512.Hash;
			    ScalarOperations.sc_clamp(az, 0);

			    sha512.Initialize();
				if (az.Length < 64) throw new Exception($"az.Length: {az.Length} < 64");
				sha512.TransformBlock(az, 32, 32, null, 0);
				sha512.TransformFinalBlock(m, moffset, mlen);
				r = sha512.Hash;

				ScalarOperations.sc_reduce(r);
				GroupOperations.ge_scalarmult_base(out R, r, 0);
				GroupOperations.ge_p3_tobytes(sig, sigoffset, ref R);

				sha512.Initialize();
				sha512.TransformBlock(sig, sigoffset, 32, null, 0);
				sha512.TransformBlock(sk, skoffset + 32, 32, null, 0);
				sha512.TransformFinalBlock(m, moffset, mlen);
				hram = sha512.Hash;

				ScalarOperations.sc_reduce(hram);
				var s = new byte[32];//todo: remove allocation
				Array.Copy(sig, sigoffset + 32, s, 0, 32);
				ScalarOperations.sc_muladd(s, hram, az, r);
				Array.Copy(s, 0, sig, sigoffset + 32, 32);
			}
		}
	}
}