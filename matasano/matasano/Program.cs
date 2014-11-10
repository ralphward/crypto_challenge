﻿using System;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Security.Cryptography;
using System.IO;

namespace matasano
{
    class Program
    {

        static void Main(string[] args)
        {
            //FileStream fs = new FileStream("c:\\crypto_output.txt", FileMode.Create);
            // First, save the standard output.
            //TextWriter tmp = Console.Out;
            //StreamWriter sw = new StreamWriter(fs);
            //Console.SetOut(sw);

            //Question 1
            string q1_arg = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d";

            //Question 2
            string q2_arg1 = "1c0111001f010100061a024b53535009181c";
            string q2_arg2 = "686974207468652062756c6c277320657965";

            //Question 3
            string q3_arg = "1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736";

            //Question 5
            string q5_arg = "Burning 'em, if you ain't quick and nimble\r\nI go crazy when I hear a cymbal";

            Console.WriteLine("Question 1 - Convert hex to base 64");
            Console.WriteLine(Convert.ToBase64String(cvtHx_to_Binary(q1_arg)));
            Console.WriteLine("");

            Console.WriteLine("Question 2 - two equal-length buffers and produce XOR sum.");
            Console.WriteLine(q2(q2_arg1, q2_arg2));
            Console.WriteLine("");

            Console.WriteLine("Question 3 - Single-character XOR Cipher");
            Console.WriteLine(q3(q3_arg));
            Console.WriteLine("");

            Console.WriteLine("Question 4 - Detect single-character XOR");
            Console.WriteLine(q4());
            Console.WriteLine("");

            Console.WriteLine("Question 5 - Repeating-key XOR Cipher");
            Console.WriteLine(q5(q5_arg));
            Console.WriteLine("");

            Console.WriteLine("Question 6 - Break Repeating-key XOR ");
            Console.WriteLine(q6());
            Console.WriteLine("");

            Console.WriteLine("Question 7 - AES in ECB Mode");
            Console.WriteLine(q7());
            Console.WriteLine();

            Console.WriteLine("Question 8 - AES in ECB Mode");
            Console.WriteLine(q8());
            Console.WriteLine();   
            
            Console.WriteLine("Implement PKCS#7 padding");
            Console.WriteLine(q9("YELLOW SUBMARINE", 20));

            Console.WriteLine(q10());
            Console.ReadLine();



            //sw.Close();
        }

        private static byte[] q9(string s, int length)
        {
            // there has to be a cleaner way of implementing this padding
            int padding = (length % s.Length) + (((length / s.Length) * s.Length) - s.Length);
            byte[] pad = new byte[padding];
            byte[] orig = Encoding.ASCII.GetBytes(s);
            byte[] rtn = new byte[s.Length + padding];

            for (int i = 0; i < padding; i++)
            {
                if (padding > 255 && i < 255)
                    pad[i] = Convert.ToByte(255);
                else
                    pad[i] = Convert.ToByte(padding % 255);
            }

            orig.CopyTo(rtn, 0);
            pad.CopyTo(rtn, orig.Length);

            return rtn;
        }

        private static string q10()
        {
            /*
            string unEncrypted = q7();
            byte[] raw_binary = Encoding.ASCII.GetBytes(unEncrypted);
            byte[] btkey = Encoding.ASCII.GetBytes("YELLOW SUBMARINE");

            return ecb_encryption(raw_binary, btkey);
            */
            return "";
        }

        private static string ecb_encryption(byte[] raw_text, byte[] key)
        {
            RijndaelManaged aes128 = new RijndaelManaged();
            aes128.Mode = CipherMode.ECB;
            aes128.Padding = PaddingMode.Zeros;
            ICryptoTransform decryptor = aes128.CreateEncryptor(key, null);
            MemoryStream ms = new MemoryStream(raw_text);
            CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

            byte[] plain = new byte[raw_text.Length];
            int decryptcount = cs.Read(plain, 0, plain.Length);

            ms.Close();
            cs.Close();

            return Convert.ToBase64String(plain, 0, decryptcount);

        }

        private static string ecb_decryption(byte[] raw_text, byte[] key)
        {
            RijndaelManaged aes128 = new RijndaelManaged();
            aes128.Mode = CipherMode.ECB;
            aes128.Padding = PaddingMode.Zeros;
            ICryptoTransform decryptor = aes128.CreateDecryptor(key, null);
            MemoryStream ms = new MemoryStream(raw_text);
            CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

            byte[] plain = new byte[raw_text.Length];
            int decryptcount = cs.Read(plain, 0, plain.Length);

            ms.Close();
            cs.Close();

            return Encoding.ASCII.GetString(plain, 0, decryptcount);


        }


        private static byte[] cbc_encryption(byte[] raw_data, byte[] iv, byte[] key)
        {


            return new byte[10];
        }
        
        private static string q8()
        {
            string[] strCiphers = q8_string();
            string cipher = "";
            //int count = 1;
            for (int k = 0; k < strCiphers.Length; k++)
            {
                byte[] raw_binary = cvtHx_to_Binary(strCiphers[k]);
                //int tmp_count = 0;
                for (int i = 0; i < raw_binary.Length - 16; i += 16)
                {
                    for (int j = i + 16; j < raw_binary.Length; j += 16 )
                    {
                        bool match = true;
                        for (int l = 0; l < 16; l++)
                        {
                            if (raw_binary[i + l] != raw_binary[j + l])
                                match = false;
                        }

                        if (match)
                        {
                            cipher = strCiphers[k];
                        }                

                    }
                }

            }
            return cipher;

        }


        private static string q7()
        {
            string strEncrypted = q7_string();
            byte[] raw_binary = Convert.FromBase64String(strEncrypted);
            byte[] btkey = Encoding.ASCII.GetBytes("YELLOW SUBMARINE");

            return ecb_decryption(raw_binary, btkey);
        }

        private static string q6()
        {

            string base64_raw;
            base64_raw = q6String();
            int intKey = -1;
            int keysizeCandidate = 0;

            byte[] raw_binary = Convert.FromBase64String(base64_raw);

            for (int keysize = 2; keysize <= 40; keysize++)
            {
                byte[] b1 = new byte[keysize];
                byte[] b2 = new byte[keysize];
                byte[] b3 = new byte[keysize];
                byte[] b4 = new byte[keysize];

                for (int i = 0; i < b1.Length; i++)
                {
                    b1[i] = raw_binary[i];
                    b2[i] = raw_binary[i + keysize];
                    b3[i] = raw_binary[i + (keysize * 2)];
                    b4[i] = raw_binary[i + (keysize * 3)];
                }

                int sub_size = hamming_distance(b1, b2) / keysize;
                int sub_size1 = hamming_distance(b3, b4) / keysize;
                sub_size = (sub_size + sub_size1) / 2;
                if (intKey == -1 || sub_size < intKey)
                {
                    keysizeCandidate = keysize;
                    intKey = sub_size;
                }
                //Console.WriteLine(keysize = " + keysize + " - Hamming Number = " + sub_size);
            }

            // Magic Number here - 29 had a reasonable hamming number - but so did a lot of other numbers, might have been better to do an incident match to get the keysize
            keysizeCandidate = 29;

            // using a 2d array so I can pivot the data more easily
            byte[,] bin_2D = new byte[raw_binary.Length / keysizeCandidate, keysizeCandidate];

            int counter = 0;
            for (int i = 0; i < raw_binary.Length / keysizeCandidate; i++)
            {
                for (int j = 0; j < keysizeCandidate; j++)
                {
                    bin_2D[i, j] = raw_binary[counter];
                    counter++;
                }

            }

            // this will be used to find the key
            char[] key = new char[keysizeCandidate];

            for (int i = 0; i < keysizeCandidate; i++)
            {
                byte[] single_key_extract = new byte[raw_binary.Length / keysizeCandidate];
                for (int j = 0; j < raw_binary.Length / keysizeCandidate; j++)
                {
                    single_key_extract[j] = bin_2D[j, i];
                }

                //each time we get here we'll run single_key_extract through single character decryption
                byte[] rtnByte = new byte[single_key_extract.Length];
                int current_score = 0;
                for (int k = 0; k <= 255; k++)
                {
                    for (int j = 0; j < single_key_extract.Length; j++)
                    {
                        rtnByte[j] = Convert.ToByte(single_key_extract[j] ^ (byte)k);
                    }

                    string candidate = Encoding.ASCII.GetString(rtnByte);
                    int candidate_score = candidate.Length - candidate.Replace(" ", "").Length;
                    candidate_score += candidate.Length - candidate.Replace("e", "").Length;
                    candidate_score += candidate.Length - candidate.Replace("t", "").Length;
                    candidate_score += candidate.Length - candidate.Replace("a", "").Length;
                    candidate_score += candidate.Length - candidate.Replace("o", "").Length;
                    if (candidate_score > current_score)
                    {
                        current_score = candidate_score;
                        key[i] = (char)k;
                    }
                }

            }

            return new string(key);

        }

        private static int hamming_distance(byte[] byte_1, byte[] byte_2)
        {
            int dist = 0;

            BitArray bv_1 = new BitArray(byte_1);
            BitArray bv_2 = new BitArray(byte_2);
            for (int j = 0; j < bv_1.Count; j++)
            {
                if (bv_1.Get(j) != bv_2.Get(j))
                {
                    dist += 1;
                }
            }

            return dist;
        }


        private static string q5(string strPlain)
        {
            string hex = "";
            for (int i = 0; i < strPlain.Length; i++)
                hex += Convert.ToString(Convert.ToChar(strPlain.Substring(i, 1)), 16);

            byte[] cipher = new byte[3];
            cipher[0] = (byte)'I';
            cipher[1] = (byte)'C';
            cipher[2] = (byte)'E';

            byte[] arrPlain = cvtHx_to_Binary(hex);
            byte[] strEncoded = new byte[arrPlain.Length];
            for (int i = 0; i < arrPlain.Length; i++)
            {
                strEncoded[i] = Convert.ToByte(arrPlain[i] ^ cipher[i % 3]);
            }

            return BitConverter.ToString(strEncoded).Replace("-", string.Empty);
        }

        private static string q3(string arg)
        {
            byte[] par_1 = cvtHx_to_Binary(arg);
            byte[] rtnByte = new byte[par_1.Length];
            string rtnValue = "";
            int current_score = 0;
            for (int i = 0; i <= 255; i++)
            {
                for (int j = 0; j < par_1.Length; j++)
                {
                    rtnByte[j] = Convert.ToByte(par_1[j] ^ (byte)i);
                }

                string candidate = Encoding.ASCII.GetString(rtnByte);
                int candidate_score = candidate.Length - candidate.Replace(" ", "").Length;
                candidate_score += candidate.Length - candidate.Replace("a", "").Length;
                candidate_score += candidate.Length - candidate.Replace("e", "").Length;
                candidate_score += candidate.Length - candidate.Replace("i", "").Length;
                candidate_score += candidate.Length - candidate.Replace("o", "").Length;
                candidate_score += candidate.Length - candidate.Replace("u", "").Length;
                if (candidate_score > current_score)
                {
                    rtnValue = candidate;
                    current_score = candidate_score;
                }
            }
            return rtnValue;
        }

        public static byte[] cvtHx_to_Binary(string hex)
        {
            byte[] rtnValue = new byte[hex.Length / 2];

            for (int i = 0; i < rtnValue.Length; i++)
            {
                string byteValue = hex.Substring(i * 2, 2);
                rtnValue[i] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return rtnValue;
        }

        public static string q2(string arg1, string arg2)
        {
            byte[] par_1 = cvtHx_to_Binary(arg1);
            byte[] par_2 = cvtHx_to_Binary(arg2);

            byte[] rtnValue = new byte[par_1.Length];

            for (int i = 0; i < par_1.Length; i++)
                rtnValue[i] = Convert.ToByte(par_1[i] ^ par_2[i]);

            return BitConverter.ToString(rtnValue).Replace("-", string.Empty);
        }

        private static string q4()
        {

            string[] q4_args = q4_string();
            int current_score = 0;
            string rtnValue = "";
            for (int i = 0; i < q4_args.Length; i++)
            {

                string candidate = q3(q4_args[i]);
                int candidate_score = candidate.Length - candidate.Replace(" ", "").Length;
                candidate_score += candidate.Length - candidate.Replace("a", "").Length;
                candidate_score += candidate.Length - candidate.Replace("e", "").Length;
                candidate_score += candidate.Length - candidate.Replace("i", "").Length;
                candidate_score += candidate.Length - candidate.Replace("o", "").Length;
                candidate_score += candidate.Length - candidate.Replace("u", "").Length;
                if (candidate_score > current_score)
                {
                    rtnValue = q4_args[i] + "\n" + candidate;
                    current_score = candidate_score;
                }
            }
            return rtnValue;
        }

        private static string[] q4_string()
        {
            string[] q4_args = new string[327];
            q4_args[0] = "0e3647e8592d35514a081243582536ed3de6734059001e3f535ce6271032";
            q4_args[1] = "334b041de124f73c18011a50e608097ac308ecee501337ec3e100854201d";
            q4_args[2] = "40e127f51c10031d0133590b1e490f3514e05a54143d08222c2a4071e351";
            q4_args[3] = "45440b171d5c1b21342e021c3a0eee7373215c4024f0eb733cf006e2040c";
            q4_args[4] = "22015e420b07ef21164d5935e82338452f42282c1836e42536284c450de3";
            q4_args[5] = "043b452e0268e7eb005a080b360f0642e6e342005217ef04a42f3e43113d";
            q4_args[6] = "581e0829214202063d70030845e5301f5a5212ed0818e22f120b211b171b";
            q4_args[7] = "ea0b342957394717132307133f143a1357e9ed1f5023034147465c052616";
            q4_args[8] = "0c300b355c2051373a051851ee154a023723414c023a08171e1b4f17595e";
            q4_args[9] = "550c3e13e80246320b0bec09362542243be42d1d5d060e203e1a0c66ef48";
            q4_args[10] = "e159464a582a6a0c50471310084f6b1703221d2e7a54502b2b205c433afa";
            q4_args[11] = "ec58ea200e3005090e1725005739eda7342aed311001383fff7c58ef1f11";
            q4_args[12] = "01305424231c0d2c41f105057f74510d335440332f1038ec17275f5814e1";
            q4_args[13] = "05f12f380720ea2b19e24a07e53c142128354e2827f25a08fb401c3126a6";
            q4_args[14] = "0d17272f53063954163d050a541b1f1144305ae37d4932431b1f33140b1b";
            q4_args[15] = "0b4f070f071fe92c200e1fa05e4b272e50201b5d493110e429482c100730";
            q4_args[16] = "100a3148080f227fe60a132f0c10174fe3f63d1a5d38eb414ca8e82f2b05";
            q4_args[17] = "0a19e83c58400a023b13234572e6e4272bf67434331631e63b5e0f00175c";
            q4_args[18] = "54520c2ceb45530e0f78111d0b0707e01e4bf43b0606073854324421e6f9";
            q4_args[19] = "09e7585353ee4a34190de1354e481c373a1b2b0a136127383e271212191f";
            q4_args[20] = "0f060d09fb4f2d5024022c5ff6463c390c2b5f1a5532071a31f33503fcea";
            q4_args[21] = "371d39121605584f48217235ee1e0602445c162e4942254c071954321d29";
            q4_args[22] = "4a0900e63e5f161e15554045f3594c2a6a77e4e52711602beaf53ae53bed";
            q4_args[23] = "29011616565d2a372a605bee39eced31183fe068185c3b445b391fe53232";
            q4_args[24] = "e4102337000303452a1e2f2b29493f54ed5a037b3e08311b625cfd005009";
            q4_args[25] = "2d560d4b0618203249312a310d5f541f295c3f0f25235c2b20037d1600f3";
            q4_args[26] = "2c245155e8253708391a7ceb0d05005c3e080f3f0f0e5a16583b111f4448";
            q4_args[27] = "493804044d262eec3759594f212d562420105d6a39e70a0f3957f347070c";
            q4_args[28] = "e72d1d1f103807590f4339575e00381074485d2d580249f744052605e11d";
            q4_args[29] = "e131570ae95307143a71131729552d001057a4540a1f425b190b572dee34";
            q4_args[30] = "2c1655342f02581c202b0a5c17a358291e1506f325550f05365e165c1c5f";
            q4_args[31] = "e318164df80b043e5406296e5359271d152f552e155a43eda81f23231d1c";
            q4_args[32] = "001de0413e174e18192c061e4b3d1b5626f90e3e1429544a20ee150d0c20";
            q4_args[33] = "32e902193219033c58191302441a5c1b584825ea140c290927aaea53e23c";
            q4_args[34] = "3a36363a732e32ea3f0e430508204b332c382a19292d5b291122e123446a";
            q4_args[35] = "1804115614031f5f571f2b143c5d3c1b257a4b37350f18445a3e08341c3d";
            q4_args[36] = "21f2fb250b2e55151e77253a3f0e5f4b2030370a4155e720e73914e35a4a";
            q4_args[37] = "510a55583a3c491221397c123a2b14a8305b3b09e71b241d0e51202e1a32";
            q4_args[38] = "1b51202f4917232b512a141d6812f03c455df05e5a1c2cee14390b3b593a";
            q4_args[39] = "5f5731e5203116ee131a4a4b24112cef5d0822f035e6547d3a0014462f26";
            q4_args[40] = "0028fb522104f771501a555d3f581e30e9ec3e49e3e63123432f07794145";
            q4_args[41] = "1459f6312f000e5a1373e346e40f211e1b0b0e17000f391f170552150500";
            q4_args[42] = "7e301e18325717e3412e022f087be30e5641080151357714e0e0eee15e11";
            q4_args[43] = "533258e9360f513b083aa51d2824222f40200a470537ecec392d31070b38";
            q4_args[44] = "07e32c180dfa56496a461627542115132a4c284050495b23e2245b093159";
            q4_args[45] = "2d3c230a1e5a300f6c3e26ed0d1709434950fd6f1e121335054129e4e4ec";
            q4_args[46] = "ef22fa2112311b11584ce43434f46f521a215433f9514fe33d313a3e0838";
            q4_args[47] = "34e7f336270c08010f2f544f0f1c1e235c0222644c2632efec061de2115f";
            q4_args[48] = "121a42395d4c560d213b0c0a26a7e4f4382718153d5e511158a10b2c021e";
            q4_args[49] = "e05d414dfa40222f0c382a03235f4d0d04372d4b7855105e26e44f2e0555";
            q4_args[50] = "7f3a4f1351f85b0344223e1177e14707190c0e311f4ca633f5f3e9352372";
            q4_args[51] = "01424d5d1a322a0d381717130e181d07240c2c19ecee750b1a37085d014c";
            q4_args[52] = "16012c5de55a0314a8260e2759e439123ca0c81c321d454e4e0ee14f4c1d";
            q4_args[53] = "0b1415512f38580e4e2a227def242643183c224f0ea146443403022fe9fd";
            q4_args[54] = "43eb2b1078322a02192d5b5e0c360d584d0b5e2c13072912ee32f03f4155";
            q4_args[55] = "002a52553e08361b0be0074b573e201c164c093a5c0f0159333b59770d5b";
            q4_args[56] = "38e63c1c5244301a5a01f26930321256143e1ae05e1120a9eaf20a192d58";
            q4_args[57] = "7d54140a152ef4035f09083ded531ee04df55848020656a1342e502649eb";
            q4_args[58] = "0c211dfe101702015516341136252f3f06f73247133113f5642d083a3417";
            q4_args[59] = "015e3d51433f3c003e5e28030b1d413eee186824504b241e0f0d32373e2b";
            q4_args[60] = "2d465040ec130c5c0e2704aa17010c40095207223669110f22f45ea155f7";
            q4_args[61] = "14552e2b341e5ce0195351066a23e3283e0ee935444b255a1c5c3cef7614";
            q4_args[62] = "372b453d5a357c05142be65b3c17f92d2b134853390a312bf92a531b513d";
            q4_args[63] = "5658265f4c0ce4440a20322f591a413034292b312206a01be6453a512d21";
            q4_args[64] = "1c585c19f31f785324f8583d1ee02620342b10a236263f105011ee5b0e14";
            q4_args[65] = "0f522b550818591a752e5fea0e033322ee5e280a4a1b244f5a2b35341255";
            q4_args[66] = "39093c1ced331b264127173f1312e2455fa33b31012c1f4d073c553f5d5e";
            q4_args[67] = "18f82d5d07e2430b3b3c1b5b49effb0313173f5d4a2e5c134555ff6b1d1a";
            q4_args[68] = "550a20234202726341190311295254f4064205aa515ae0145a23071c4e18";
            q4_args[69] = "3f2047024e3ce4555a1b39fa145455012c3afb0f2d11134846182e3c575b";
            q4_args[70] = "e3e456571937762828065443153b51152e262f09c937024405284f236432";
            q4_args[71] = "012f580c3536ec5c021574541d5c41123a4e661d5f0f5f344a083e3a5e4c";
            q4_args[72] = "4216252d01eb0a2a4623621b48360d312c29f33e380650447617124b3e71";
            q4_args[73] = "54141e59323606390204e95f1206520e5c084510034d30171c5e744f335d";
            q4_args[74] = "1e30061401600b342e171059526d1949431a3f412f56594c183711ea4837";
            q4_args[75] = "3131254f11e76f550e1e4d26f1391f44363b151c31281ff45259351da0e6";
            q4_args[76] = "5def250d0f3505385f22e9f4112633005d272d092e0138275851f943e90e";
            q4_args[77] = "0939165718303b445210095c16390cf04f19450e06f4545c0a0c320e3e23";
            q4_args[78] = "1e0b0b1f573f3d0fe05d43090fa8482242300819313142325b1f4b19365b";
            q4_args[79] = "0d3b2a5d271e463d2203765245065d5d684a051e5815265b52f3171d3004";
            q4_args[80] = "6af423303817a43324394af15a5c482e3b16f5a46f1e0b5c1201214b5fe4";
            q4_args[81] = "4030544f3f51151e436e04203a5e3b287ee303490a43fb3b28042f36504e";
            q4_args[82] = "1a2d5a03fc0e2c04384046242e2b5e1548101825eb2f285f1a210f022141";
            q4_args[83] = "122355e90122281deeed3ba05636003826525d5551572d07030d4935201f";
            q4_args[84] = "2a3c484a15410d3b16375d4665271b5c4ce7ee37083d3e512b45204f17f6";
            q4_args[85] = "03222801255c2c211a7aeb1e042b4e38e8f1293143203139fb202c325f2b";
            q4_args[86] = "06542a28041956350e292bf3fe5c32133a2a171b3a3e4e4e3101381529e3";
            q4_args[87] = "4a5209ef24e5f3225e503b143d0e5747323fe7ee3d5b1b5110395619e65a";
            q4_args[88] = "1fee0a3945563d2b5703701817584b5f5b54702522f5031b561929ea2d1e";
            q4_args[89] = "e7271935100e3c31211b23113a3a5524e02241181a251d521ff52f3c5a76";
            q4_args[90] = "144a0efee02f0f5f1d353a1c112e1909234f032953ec591e0a58e55d2cf4";
            q4_args[91] = "efee0cf00d0955500210015311467543544708eb590d113d30443d080c1e";
            q4_args[92] = "1a562c1f7e2b0030094f051c03e30f4d501a0fe22a2817edfc5e470c3843";
            q4_args[93] = "1c3df1135321a8e9241a5607f8305d571aa546001e3254555a11511924";
            q4_args[94] = "eb1d3f54ec0fea341a097c502ff1111524e24f5b553e49e8576b5b0e1e33";
            q4_args[95] = "72413e2f5329e332ec563b5e65185efefd2c3b4e5f0b5133246d214a401d";
            q4_args[96] = "352a0ae632183d200a162e5346110552131514e0553e51003e220d47424b";
            q4_args[97] = "1d005c58135f3c1b53300c3b49263928f55625454f3be259361ded1f0834";
            q4_args[98] = "2d2457524a1e1204255934174d442a1a7d130f350a123c4a075f5be73e30";
            q4_args[99] = "0c0518582d131f39575925e0231833370c482b270e183810415d5aec1900";
            q4_args[100] = "453b181df1572735380b0446097f00111f1425070b2e1958102ceb592928";
            q4_args[101] = "010a4a2d0b0926082d2f1525562d1d070a7a08152f5b4438a4150b132e20";
            q4_args[102] = "2b395d0d5d015d41335d21250de33e3d42152d3f557d1e44e4ee22255d2d";
            q4_args[103] = "4a1b5c272d0d1c45072639362e402dee2853e51311262b17aa72eb390410";
            q4_args[104] = "e7015f0215352030574b4108e44d0e1a204418e62325ff7f34052f234b2d";
            q4_args[105] = "1d563c13202346071d39e34055402b0b392c27f552222d3deb3843ee2c16";
            q4_args[106] = "29332a521f3c1b0811e33e1a25520e323e75e01c17473f55071226120d3d";
            q4_args[107] = "210b35ee1a0a5335222e35033905170c4f3104eb032d425058367d5a2bf2";
            q4_args[108] = "1e553809415efb1c460f2f0ffafaec491e4d4e49510452e8245a366a4106";
            q4_args[109] = "e1f92cee0e10142514e7ec13155c412fe901092f1f0fa738280c5eee5e04";
            q4_args[110] = "3526291e0b2a5f486a3051041f4c16372f5402e6f70b31a03525190b161a";
            q4_args[111] = "260e5e1f0c2e4d7528ef11552fefe247201e4752085c1da903563c162a4b";
            q4_args[112] = "2a14ff2e3265e604075e523b24455c364a7f284f3a43051d52152f1119e8";
            q4_args[113] = "5f02e55a4b1300063640ef10151002565f0b0c010033a1cbef5d3634484a";
            q4_args[114] = "1b121c585b495a5e033a09037f2d1754072c2d49084055172a3c220bed4f";
            q4_args[115] = "1613400e1632435c0018482aa55b363d26290ae4405ded280f2b0c271536";
            q4_args[116] = "4011250ce02119464a1de43113170356342c272d1d3355555e5706245e0a";
            q4_args[117] = "16272d5e545953002e10020875e223010719555410f91ce518420e382456";
            q4_args[118] = "0d4037320345f945241a1d090a545a310142442131464f4d10562ae4f05a";
            q4_args[119] = "07ee4d4ae12e571e313c1636313134233e495459e548317708563c2c1b2f";
            q4_args[120] = "e75803294b36565225552c3406304f0201e43323291b5e0e2159025c2f25";
            q4_args[121] = "5e63194411490c44494232237e1b323108573d3f391d1f3537e4165a2b35";
            q4_args[122] = "51000a3a264c503b5852072a5636f04f5cea58a42838f5fca876415c3521";
            q4_args[123] = "3c14130be511275932055a30aa2d03470c51060009f210543002585f5713";
            q4_args[124] = "10f0370c5823115200e5015d083e2f1a5df91d68065c1b03f0080855e529";
            q4_args[125] = "02ec00f1462d034123151ba6fc07eb3d5e54e85a3f3ee532fb41791a060b";
            q4_args[126] = "0c29274232f93efb3d465544e45e491b042ced245100e3f05c14134c254b";
            q4_args[127] = "5741235f051e080401a8013c065627e8ee5432205114243d54320e133f2d";
            q4_args[128] = "4a4d181635411f5d084e31ed230c16506d5125415e060e4dcd0e5f3708e3";
            q4_args[129] = "2d531c3e22065a5eee07310c145305131800063e4a20094b2006ea131240";
            q4_args[130] = "e7335c1c4308160be6aa551a0f5a58243e0b10ee470047683c345e1c5b0c";
            q4_args[131] = "5434505ee22a18110d20342e4b53062c4d79042a0a02422e225b2523e95a";
            q4_args[132] = "3252212407115c07e15eee06391d0519e9271b641330011f383410281f0e";
            q4_args[133] = "2cee2b355233292b595d1c69592f483b54584f7154fd4928560752e333a1";
            q4_args[134] = "17272b272f110df5e91c560a39104510240b5c4b0c1c570871e422351927";
            q4_args[135] = "c32550ec3f132c0c2458503ae5241d3c0d7911480a073826315620403615";
            q4_args[136] = "16e11c270d2b010650145de2290b0beb1e120a3a354b2104064f3b533c4e";
            q4_args[137] = "505746313d4d2e3455290a281ee81d50007e1148252528025237715a342a";
            q4_args[138] = "1c0a13163e404e40242142061d34185421160220fa031f7a423a08f2e01a";
            q4_args[139] = "101d303802f51b0c08ef461259315b553823e622a12d565509e23c624139";
            q4_args[140] = "0a3d1309e4384c0eed383846545a035a41ee1771513b090a031e15f45159";
            q4_args[141] = "2d4944092a1965542507003b23195758403e175a0a450c5c38114de21141";
            q4_args[142] = "eb100fe63a031c4b35eb591845e428441c0d5b0037131f5c160a31243619";
            q4_args[143] = "c155ef0d19143e24392507a202581a25491b135c27571d5c5b35250f0bef";
            q4_args[144] = "0e1d510556485e39557e044e2cf10457523016473f500b1e36370c17591c";
            q4_args[145] = "7e5a19250a5e152b46f5130a094cef08e84704ef10197324464b0114017a";
            q4_args[146] = "3b56f126390008343d3c400232ed201667211f0b1a1413080202530b08e2";
            q4_args[147] = "4912321b61c90a0cf6ef0a0a0c0f17fa62eb385e2616194526701aff5fe6";
            q4_args[148] = "2c57114b0400152d4f2aeb18ed41386c2e3a023a281d1a311eefe750ebab";
            q4_args[149] = "3a4353282114593b3e36446d2c5e1e582e335337022930331f211604576a";
            q4_args[150] = "295f3bfae9271ae8065a3b4417545c3e5b0df11a53351c78530915392d2e";
            q4_args[151] = "074a122ee01b17131e4e124e2322a9560ce4120e37582b24e1036fe93f30";
            q4_args[152] = "3c08290121090ef72f25e4f220323444532d3fe71f34553c7b2726131009";
            q4_args[153] = "12e84a3308590357a719e74c4f2133690a20031a0b045af63551325b1219";
            q4_args[154] = "0e3d4fe03f56523cf40f29e4353455120e3a4f2f26f6a30a2b3e0c5b085a";
            q4_args[155] = "57f3315c33e41c0f523426232d0651395c1525274e314d0219163b5f181f";
            q4_args[156] = "53471622182739e9e25b473d74e1e7023d095a3134e62d1366563004120e";
            q4_args[157] = "230a06431935391d5e0b5543223a3bed2b4358f555401e1b3b5c36470d11";
            q4_args[158] = "22100330e03b4812e6120f163b1ef6abebe6f602545ef9a459e33d334c2a";
            q4_args[159] = "463405faa655563a43532cfe154bec32fe3345eb2c2700340811213e5006";
            q4_args[160] = "14241340112b2916017c270a0652732ee8121132385a6c020c040e2be15b";
            q4_args[161] = "251119225c573b105d5c0a371c3d421ef23e22377fee334e0228561b2d15";
            q4_args[162] = "2e4c2e373b434b0d0b1b340c300e4b195614130ea03c234c292e14530c46";
            q4_args[163] = "0d2c3f08560ee32e5a5b6413355215384442563e69ec294a0eef561e3053";
            q4_args[164] = "193c100c0b24231c012273e10d2e12552723586120020b02e45632265e5f";
            q4_args[165] = "2c175a11553d4b0b16025e2534180964245b125e5d6e595d1d2a0710580b";
            q4_args[166] = "213a175ff30855e4001b305000263f5a5c3c5100163cee00114e3518f33a";
            q4_args[167] = "10ed33e65b003012e7131e161d5e2e270b4645f358394118330f5a5b241b";
            q4_args[168] = "33e80130f45708395457573406422a3b0d03e6e5053d0d2d151c083337a2";
            q4_args[169] = "551be2082b1563c4ec2247140400124d4b6508041b5a472256093aea1847";
            q4_args[170] = "7b5a4215415d544115415d5015455447414c155c46155f4058455c5b523f";
            q4_args[171] = "0864eb4935144c501103a71851370719301bec57093a0929ea3f18060e55";
            q4_args[172] = "2d395e57143359e80efffb13330633ea19e323077b4814571e5a3de73a1f";
            q4_args[173] = "52e73c1d53330846243c422d3e1b374b5209543903e3195c041c251b7c04";
            q4_args[174] = "2f3c2c28273a12520b482f18340d565d1fe84735474f4a012e1a13502523";
            q4_args[175] = "23340f39064e306a08194d544647522e1443041d5ee81f5a18415e34a45f";
            q4_args[176] = "475a392637565757730a0c4a517b2821040e1709e028071558021f164c54";
            q4_args[177] = "100b2135190505264254005618f51152136125370eef27383e45350118ed";
            q4_args[178] = "3947452914e0223f1d040943313c193f295b221e573e1b5723391d090d1f";
            q4_args[179] = "2c33141859392b04155e3d4e393b322526ee3e581d1b3d6817374d0c085b";
            q4_args[180] = "c2ea5821200f1b755b2d13130f04e26625ea3a5b1e37144d3e473c24030d";
            q4_args[181] = "ee15025d2019f757305e3f010e2a453a205f1919391e1a04e86d1a350119";
            q4_args[182] = "1a5beb4946180fe0002a031a050b41e5164c58795021e1e45c59e2495c20";
            q4_args[183] = "1121394f1e381c3647005b7326250514272b55250a49183be5454ba518eb";
            q4_args[184] = "1ee55936102a465d5004371f2e382f1d03144f170d2b0eed042ee341eb19";
            q4_args[185] = "ec1014ef3ff1272c3408220a41163708140b2e340e505c560c1e4cf82704";
            q4_args[186] = "274b341a454a27a0263408292e362c201c0401462049523b2d55e5132d54";
            q4_args[187] = "e259032c444b091e2e4920023f1a7ce40908255228e36f0f2424394b3c48";
            q4_args[188] = "34130cf8223f23084813e745e006531a1e464b005e0e1ee405413fe22b4e";
            q4_args[189] = "4af201080c0928420c2d491f6e5121e451223b070dee54244b3efc470a0e";
            q4_args[190] = "771c161f795df81c22101408465ae7ef0c0604733ee03a20560c1512f217";
            q4_args[191] = "2f3a142c4155073a200f04166c565634020a59ea04244ff7413c4bc10858";
            q4_args[192] = "240d4752e5fa5a4e1ce255505602e55d4c575e2b59f52b4e0c0a0b464019";
            q4_args[193] = "21341927f3380232396707232ae424ea123f5b371d4f65e2471dfbede611";
            q4_args[194] = "e10e1c3b1d4d28085c091f135b585709332c56134e4844552f45eb41172a";
            q4_args[195] = "3f1b5a343f034832193b153c482f1705392f021f5f0953290c4c43312b36";
            q4_args[196] = "3810161aea7001fb5d502b285945255d4ef80131572d2c2e59730e2c3035";
            q4_args[197] = "4d59052e1f2242403d440a13263e1d2dea0612125e16033b180834030829";
            q4_args[198] = "022917180d07474c295f793e42274b0e1e16581036225c1211e41e04042f";
            q4_args[199] = "ec2b41054f2a5f56065e5e0e1f56e13e0a702e1b2f2137020e363a2ae2a4";
            q4_args[200] = "53085a3b34e75a1caa2e5d031f261f5f044350312f37455d493f131f3746";
            q4_args[201] = "0c295f1724e90b001a4e015d27091a0b3256302c303d51a05956e6331531";
            q4_args[202] = "e42b315ce21f0def38144d20242845fa3f3b3b0ce8f4fb2d31ed1d54134b";
            q4_args[203] = "2957023141335d35372813263b46581af6535a16404d0b4ff12a207648ec";
            q4_args[204] = "e4421e301de25c43010c504e0f562f2018421ce137443b41134b5f542047";
            q4_args[205] = "0c5600294e085c1d3622292c480d261213e05c1334385108c145f3090612";
            q4_args[206] = "062d2e02267404241f4966e6e010052d3224e72856100b1d22f65a30e863";
            q4_args[207] = "324950394700e11a01201a0564525706f1013f353319076b4c0d015a2e24";
            q4_args[208] = "2a1be80e2013571522483b1e20321a4e03285d211a444d113924e8f41a1f";
            q4_args[209] = "27193ae2302208e73010eaa1292001045737013e10e4745aed2c105b25fb";
            q4_args[210] = "1b135d46eaef103e1d330a14337a2a4302441c1631ed07e7100c743a0e35";
            q4_args[211] = "1a0957115c293b1c0de853245b5b18e2e12d28421b3230245d7b4a55f355";
            q4_args[212] = "e7360e2b3846202a2926fa495e3302ed064d127a17343a1f11032b40e8f5";
            q4_args[213] = "06e8f90a3118381c5414157d1434050210363e30500511a00a3d56e10438";
            q4_args[214] = "30021931f7193e25a0540ef52658350929380974fb035b1a5d2c042959c7";
            q4_args[215] = "151b0c24052d0e56025404390e5a3909edec0d03070f040cff710825363e";
            q4_args[216] = "2a2328120b2203320810134a0c0a0ef30b25460bec011c1e26e913575a51";
            q4_args[217] = "e12d0948ed3c511416151d1c54082b3e385d14f838510bec4e4b5f585321";
            q4_args[218] = "1559305c3a49192a010f04ec11001a3d5a5621e5535358353206521f013f";
            q4_args[219] = "172c2c155a3a322009505c290516a2c4e4405a1e0a1e353b6e1a5a4e2f09";
            q4_args[220] = "552c34e2432b0df1132b130841000d4007232339a2092a593f142b0a0117";
            q4_args[221] = "0931432e452d3aea1d02587d3a3e56ed2a3050e2f9363df366331e421947";
            q4_args[222] = "0250094823545b20163f1d0a36a92228ed25564d1a304deae8035c32370d";
            q4_args[223] = "4314380e264e2359e6a412504a424328e84434ff30236649353315344a00";
            q4_args[224] = "25e33540550d3c15135b0eed451cfd1812eaf2063f085d6e214d121c342f";
            q4_args[225] = "37513b2d0a4e3e5211372a3a01334c5d51030c46463e3756290c0d0e1222";
            q4_args[226] = "132f175e4c4af1120138e1f2085a3804471f5824555d083de6123f533123";
            q4_args[227] = "0de11936062d3d2f12193e135f38ff5e1a531d1426523746004e2c063a27";
            q4_args[228] = "49241aee1802311611a50de9592009e936270108214a0c4213a01f09545f";
            q4_args[229] = "02e14d2babee204a5c4337135821360d021b7831305963ee0737072f0deb";
            q4_args[230] = "1512371119050c0c1142245a004f033650481830230a1925085c1a172726";
            q4_args[231] = "3be62f230a4b50526ec9345100252aa729eafa59221b3fa517304e500a15";
            q4_args[232] = "5e57f231333c3d0c470a47551733511031362a3bed0f334a3f3136104230";
            q4_args[233] = "eb24015d051a151f245905061a37ea273d2239fe02463a5e314d565f0457";
            q4_args[234] = "23025f415d290a594e3b5940313347a11c5e41531ff15a385a183829780a";
            q4_args[235] = "51e0035f2deb3b163eabe8550e2e0414491f573b5419234a28183044e112";
            q4_args[236] = "1d54e8390b26585f3aef5f14206672240c4a5e5d31e01b4d406e351401fa";
            q4_args[237] = "e555173e242c753b275d4ee50b2f26501402a71b1b5733ec19ee34284aed";
            q4_args[238] = "2ee8f023401c09383b084d623ef324ee5a33065a6d5e365b092c5d0d4501";
            q4_args[239] = "3f4e024d4b161e144d5e3b140d1e2944465b491d265603a705373c231240";
            q4_args[240] = "544f0d4ea6091e00e62d3e130d4f005139f339001a3b480c221b730be75e";
            q4_args[241] = "5f1f4f3e0a0dec3b5128e32960e42d0fee02275528154b10e65c36555a2e";
            q4_args[242] = "ea3e311b5b0f5f220b1f1b2914f12111f41213e06232224df5ec0114470d";
            q4_args[243] = "51203f1e01e5563851284013514a565e53125223052f47100e5011100201";
            q4_args[244] = "3f5bee2305217838582be55958a00245265b0308ec56525b5c114c2d5407";
            q4_args[245] = "e6e74818e53602160e45372029eb4de72754ec3f49290d2f5901014c0e7f";
            q4_args[246] = "08e715e612380a5c1908285a1222073a023c562907384e4f470444483f34";
            q4_args[247] = "1110382b5225343ba6092133483e2d683e1e280227084a1e405e3a341513";
            q4_args[248] = "415f240f0c53e3f7196e2252fb0105347f345e531f535a344bf439220916";
            q4_args[249] = "5722e7f7fa2f4c2e057e2a025e2dec31413439aa12265f5a3458f81a4b15";
            q4_args[250] = "135839401856f337a72fec475a060de239a650163a55392a5b303f051415";
            q4_args[251] = "56090f18023a2b16e2364407050d48e1541408281d3aa3e84c5b264c1f33";
            q4_args[252] = "1725f9540aec5e10ed293e4e5a5a2d2125f053251a55395d1c2044022231";
            q4_args[253] = "292d523ff86a180620075f325e02566659f30423525a053a01f0087f4b3b";
            q4_args[254] = "17fe493808f25309251e1325596ce32b42311e5d0c2f58652640582a4b17";
            q4_args[255] = "67381a5afb7128150a0043e45b173d2111155c49092d2635370a3a201826";
            q4_args[256] = "e62d021d36e03b205d5f1f295c094608342a412122583f3bfc34190be62c";
            q4_args[257] = "393a055f59060d454a235326e844243a30285c14e316272524f4f0444f51";
            q4_args[258] = "352c3c5b2b5845244f55494940194721f80b120f07392b7c2c5a0508111e";
            q4_args[259] = "2f1219430151e60f11150b101e295736361b1e053e4d08f83f230e2c383a";
            q4_args[260] = "ef5b1d492610e834330f5cf3a2485d324f2822084f41111f582957191b19";
            q4_args[261] = "1e3e223704fe1d2e1f592753e5550f15170b231b4234e945301f5605a670";
            q4_args[262] = "300d322759ea0337015c662a0e073809543f2741104835512d0624551751";
            q4_args[263] = "373727ef1f41084d0b5c0c0137283b1337026aea1c5ae115064ffa183402";
            q4_args[264] = "09152b11e1233e5a0e302a521c5a33181e180026463744a82c024b4bf04e";
            q4_args[265] = "1df61df1263fee59135c13400950153d3c5c59183b020b1d2d2c492f4968";
            q4_args[266] = "e2000c405a01ede30c4c082e2537443c120f38fc57c43651423e5c3beb1d";
            q4_args[267] = "1922182420191b293e163d58020b005f454a0621051a38e80b090a463ee9";
            q4_args[268] = "39513f2d47042c0fe5134419ec48490f150f323a5ee7a7e0201e193a5e1b";
            q4_args[269] = "2037200a2b1013567b35fb4a0f322c2f49435d091920521c302b413f5f35";
            q4_args[270] = "775d1a345b483b35a02a4c3e17ee3a3d5a5b57153613264f23041922432f";
            q4_args[271] = "35125b3e0a1d2257eb002a26455e1a2f042e1545e92f0b3408032c4f3551";
            q4_args[272] = "2d4c392321300a18ed4f3e2c314d20500052aa3917e55d0d29500754282e";
            q4_args[273] = "381b2e263758f63c474a1c23110c2d5f1c220412e91043580656080c0427";
            q4_args[274] = "081ce1e5350b6a3535f0e6592e5b543432340e38f008e0324102e45a3f25";
            q4_args[275] = "30040c181615362e4d1016160a4a5c006eeb1d2422355a3f1028ff192a07";
            q4_args[276] = "53f6354d4b5d121974245c14f0225713331f2e381810101428571725e432";
            q4_args[277] = "1a2c06372d5b1419742150042d25003c2650512834ef16e51d183f0f0508";
            q4_args[278] = "3d191107251100ee2e4125405a44174f061e0e1e5959e606530e06ed245e";
            q4_args[279] = "3f592d47512dec5922500e460e1de7183b4c3c2e583942255a0c5d4d2305";
            q4_args[280] = "3438001e482a002d56113a1fe13bed542d3508e22f4e22221431121c1539";
            q4_args[281] = "ed445a5d28415073eb18022ef836274d573a48090f2a663058194901405d";
            q4_args[282] = "215b143954fc313c1e28584b51e729ef31013b232bfb4c52e2322a2d4557";
            q4_args[283] = "5244102e1c3d304450ee01761924e62ff2173305e15809102b2125284dfc";
            q4_args[284] = "171a3f010f3639056f2be71c2047581de32e05a20833e1221b0e25362459";
            q4_args[285] = "2958280de238084f5a1c292e005be71f3b311e1f415809383d3862260238";
            q4_args[286] = "361f56ecee120156375862eb3627185c2519545149e2e50b1f3b0c4e3352";
            q4_args[287] = "e6115f440634e4005d273611e41c5d383c3814537b3d23362b084024345b";
            q4_args[288] = "10370656372e0236eb4f3303e216505f0e465228383729394faa2f205f34";
            q4_args[289] = "2e125b2f2c1d0f1f170e0c51331f0c06291610345c0603791f33253f0e0c";
            q4_args[290] = "1c2b080526133aeb3e23571d4cfa1e48057a2a010a490a50391b09514f2e";
            q4_args[291] = "59383ae11237e5450029162d2e1d3e09221a160e42ea06ea0ca7c7ecf4ea";
            q4_args[292] = "3d3024f34d5c07464bea3b185e110d3a10395d3b2632343cf30ca2e6065a";
            q4_args[293] = "262f111c0e15441a4825111b185f1e5756243206125f4603e97e79582d27";
            q4_args[294] = "2d5801ee2654113e2da00b58e9260d643c10423e1d1f42093b0d0f7d5102";
            q4_args[295] = "3649211f210456051e290f1b4c584d0749220c280b2a50531f262901503e";
            q4_args[296] = "52053e3e152b5b2b4415580fec57ef5c08e5ed43cc2d2e5b40355d0d2017";
            q4_args[297] = "6d3917263f030c4b55f0025d501e57504a122729293c4c5819680d3001ed";
            q4_args[298] = "1e313323324e5e177b171cf70c371541395c0e2b7726e42505483014362e";
            q4_args[299] = "1910e4f7253f0a012057e03b1e3b4201362b224ff60e0b3a1d115b043957";
            q4_args[300] = "200c1e0b242e5e3b4755f61e3be05c040908f1234358e55562711d2efa0f";
            q4_args[301] = "0737e0160b1d13132044080d2325f1f0ee2f00354f2106471131020a5d0b";
            q4_args[302] = "3f21060de62c052a17576e2ce729242b3e3621300627f01e52580a480050";
            q4_args[303] = "1b381a11351f4f5d22040c3c4b3e7d263714e8e61a571d107a34260a4a51";
            q4_args[304] = "edf52314e111207c0b23eb482f441d211f306137152407040e08530a783e";
            q4_args[305] = "3c054e2d4e2905275e640220f74f1a193f54e1ed5b4e2a290eab27a55147";
            q4_args[306] = "33522817335316ea2f3df957e25e02030601514f09f74c2fedee102d3114";
            q4_args[307] = "5d05231d03313826164156110c44e4111f4658005e115e300f413b430300";
            q4_args[308] = "380bf53a4331f74627492c133fe8eb3141ee39040def040c1a0ae914e3ed";
            q4_args[309] = "5b00f0211f0a091e05582e22f05a5d262e0ce352251d25100b102b11e339";
            q4_args[310] = "36053935f051f959093252411e2d5af81f360c0fa15d0b373b1d26323b77";
            q4_args[311] = "501424184202206215e05944505c4817514540445b0207025de05b050932";
            q4_args[312] = "0a5a114515536f553a352c513f0b12f700345fa51d5efb28222676e559ea";
            q4_args[313] = "561b0557403f5f534a574638411e2d3b3c133f79555c333215e6f5f9e7ec";
            q4_args[314] = "6658f7210218110f00062752e305f21601442c5310162445ed4d175630f3";
            q4_args[315] = "0e2154253c4a22f02e1b0933351314071b521513235031250c18120024a1";
            q4_args[316] = "e03555453d1e31775f37331823164c341c09e310463438481019fb0b12fa";
            q4_args[317] = "37eee654410e4007501f2c0e42faf50125075b2b46164f165a1003097f08";
            q4_args[318] = "2a5332145851553926523965582e5b2f530d5d1e292046344feaed461517";
            q4_args[319] = "583d2b06251f551d2f5451110911e6034147481a05166e1f241a5817015b";
            q4_args[320] = "1f2d3f5c310c315402200010e24135592435f71b4640540a041012ee1b3f";
            q4_args[321] = "5b2010060e2f5a4d045e0b36192f79181b0732183b4a261038340032f434";
            q4_args[322] = "3a5557340be6f5315c35112912393503320f54065f0e275a3b5853352008";
            q4_args[323] = "1c595d183539220eec123478535337110424f90a355af44c267be848173f";
            q4_args[324] = "41053f5cef5f6f56e4f5410a5407281600200b2649460a2e3a3c38492a0c";
            q4_args[325] = "4c071a57e9356ee415103c5c53e254063f2019340969e30a2e381d5b2555";
            q4_args[326] = "32042f46431d2c44607934ed180c1028136a5f2b26092e3b2c4e2930585a";

            return q4_args;
        }

        public static string q6String()
        {
            string s;
            s = "HUIfTQsPAh9PE048GmllH0kcDk4TAQsHThsBFkU2AB4BSWQgVB0dQzNTTmVS";
            s += "BgBHVBwNRU0HBAxTEjwMHghJGgkRTxRMIRpHKwAFHUdZEQQJAGQmB1MANxYG";
            s += "DBoXQR0BUlQwXwAgEwoFR08SSAhFTmU+Fgk4RQYFCBpGB08fWXh+amI2DB0P";
            s += "QQ1IBlUaGwAdQnQEHgFJGgkRAlJ6f0kASDoAGhNJGk9FSA8dDVMEOgFSGQEL";
            s += "QRMGAEwxX1NiFQYHCQdUCxdBFBZJeTM1CxsBBQ9GB08dTnhOSCdSBAcMRVhI";
            s += "CEEATyBUCHQLHRlJAgAOFlwAUjBpZR9JAgJUAAELB04CEFMBJhAVTQIHAh9P";
            s += "G054MGk2UgoBCVQGBwlTTgIQUwg7EAYFSQ8PEE87ADpfRyscSWQzT1QCEFMa";
            s += "TwUWEXQMBk0PAg4DQ1JMPU4ALwtJDQhOFw0VVB1PDhxFXigLTRkBEgcKVVN4";
            s += "Tk9iBgELR1MdDAAAFwoFHww6Ql5NLgFBIg4cSTRWQWI1Bk9HKn47CE8BGwFT";
            s += "QjcEBx4MThUcDgYHKxpUKhdJGQZZVCFFVwcDBVMHMUV4LAcKQR0JUlk3TwAm";
            s += "HQdJEwATARNFTg5JFwQ5C15NHQYEGk94dzBDADsdHE4UVBUaDE5JTwgHRTkA";
            s += "Umc6AUETCgYAN1xGYlUKDxJTEUgsAA0ABwcXOwlSGQELQQcbE0c9GioWGgwc";
            s += "AgcHSAtPTgsAABY9C1VNCAINGxgXRHgwaWUfSQcJABkRRU8ZAUkDDTUWF01j";
            s += "OgkRTxVJKlZJJwFJHQYADUgRSAsWSR8KIgBSAAxOABoLUlQwW1RiGxpOCEtU";
            s += "YiROCk8gUwY1C1IJCAACEU8QRSxORTBSHQYGTlQJC1lOBAAXRTpCUh0FDxhU";
            s += "ZXhzLFtHJ1JbTkoNVDEAQU4bARZFOwsXTRAPRlQYE042WwAuGxoaAk5UHAoA";
            s += "ZCYdVBZ0ChQLSQMYVAcXQTwaUy1SBQsTAAAAAAAMCggHRSQJExRJGgkGAAdH";
            s += "MBoqER1JJ0dDFQZFRhsBAlMMIEUHHUkPDxBPH0EzXwArBkkdCFUaDEVHAQAN";
            s += "U29lSEBAWk44G09fDXhxTi0RAk4ITlQbCk0LTx4cCjBFeCsGHEETAB1EeFZV";
            s += "IRlFTi4AGAEORU4CEFMXPBwfCBpOAAAdHUMxVVUxUmM9ElARGgZBAg4PAQQz";
            s += "DB4EGhoIFwoKUDFbTCsWBg0OTwEbRSonSARTBDpFFwsPCwIATxNOPBpUKhMd";
            s += "Th5PAUgGQQBPCxYRdG87TQoPD1QbE0s9GkFiFAUXR0cdGgkADwENUwg1DhdN";
            s += "AQsTVBgXVHYaKkg7TgNHTB0DAAA9DgQACjpFX0BJPQAZHB1OeE5PYjYMAg5M";
            s += "FQBFKjoHDAEAcxZSAwZOBREBC0k2HQxiKwYbR0MVBkVUHBZJBwp0DRMDDk5r";
            s += "NhoGACFVVWUeBU4MRREYRVQcFgAdQnQRHU0OCxVUAgsAK05ZLhdJZChWERpF";
            s += "QQALSRwTMRdeTRkcABcbG0M9Gk0jGQwdR1ARGgNFDRtJeSchEVIDBhpBHQlS";
            s += "WTdPBzAXSQ9HTBsJA0UcQUl5bw0KB0oFAkETCgYANlVXKhcbC0sAGgdFUAIO";
            s += "ChZJdAsdTR0HDBFDUk43GkcrAAUdRyonBwpOTkJEUyo8RR8USSkOEENSSDdX";
            s += "RSAdDRdLAA0HEAAeHQYRBDYJC00MDxVUZSFQOV1IJwYdB0dXHRwNAA9PGgMK";
            s += "OwtTTSoBDBFPHU54W04mUhoPHgAdHEQAZGU/OjV6RSQMBwcNGA5SaTtfADsX";
            s += "GUJHWREYSQAnSARTBjsIGwNOTgkVHRYANFNLJ1IIThVIHQYKAGQmBwcKLAwR";
            s += "DB0HDxNPAU94Q083UhoaBkcTDRcAAgYCFkU1RQUEBwFBfjwdAChPTikBSR0T";
            s += "TwRIEVIXBgcURTULFk0OBxMYTwFUN0oAIQAQBwkHVGIzQQAGBR8EdCwRCEkH";
            s += "ElQcF0w0U05lUggAAwANBxAAHgoGAwkxRRMfDE4DARYbTn8aKmUxCBsURVQf";
            s += "DVlOGwEWRTIXFwwCHUEVHRcAMlVDKRsHSUdMHQMAAC0dCAkcdCIeGAxOazkA";
            s += "BEk2HQAjHA1OAFIbBxNJAEhJBxctDBwKSRoOVBwbTj8aQS4dBwlHKjUECQAa";
            s += "BxscEDMNUhkBC0ETBxdULFUAJQAGARFJGk9FVAYGGlMNMRcXTRoBDxNPeG43";
            s += "TQA7HRxJFUVUCQhBFAoNUwctRQYFDE43PT9SUDdJUydcSWRtcwANFVAHAU5T";
            s += "FjtFGgwbCkEYBhlFeFsABRcbAwZOVCYEWgdPYyARNRcGAQwKQRYWUlQwXwAg";
            s += "ExoLFAAcARFUBwFOUwImCgcDDU5rIAcXUj0dU2IcBk4TUh0YFUkASEkcC3QI";
            s += "GwMMQkE9SB8AMk9TNlIOCxNUHQZCAAoAHh1FXjYCDBsFABkOBkk7FgALVQRO";
            s += "D0EaDwxOSU8dGgI8EVIBAAUEVA5SRjlUQTYbCk5teRsdRVQcDhkDADBFHwhJ";
            s += "AQ8XClJBNl4AC1IdBghVEwARABoHCAdFXjwdGEkDCBMHBgAwW1YnUgAaRyon";
            s += "B0VTGgoZUwE7EhxNCAAFVAMXTjwaTSdSEAESUlQNBFJOZU5LXHQMHE0EF0EA";
            s += "Bh9FeRp5LQdFTkAZREgMU04CEFMcMQQAQ0lkay0ABwcqXwA1FwgFAk4dBkIA";
            s += "CA4aB0l0PD1MSQ8PEE87ADtbTmIGDAILAB0cRSo3ABwBRTYKFhROHUETCgZU";
            s += "MVQHYhoGGksABwdJAB0ASTpFNwQcTRoDBBgDUkksGioRHUkKCE5THEVCC08E";
            s += "EgF0BBwJSQoOGkgGADpfADETDU5tBzcJEFMLTx0bAHQJCx8ADRJUDRdMN1RH";
            s += "YgYGTi5jMURFeQEaSRAEOkURDAUCQRkKUmQ5XgBIKwYbQFIRSBVJGgwBGgtz";
            s += "RRNNDwcVWE8BT3hJVCcCSQwGQx9IBE4KTwwdASEXF01jIgQATwZIPRpXKwYK";
            s += "BkdEGwsRTxxDSToGMUlSCQZOFRwKUkQ5VEMnUh0BR0MBGgAAZDwGUwY7CBdN";
            s += "HB5BFwMdUz0aQSwWSQoITlMcRUILTxoCEDUXF01jNw4BTwVBNlRBYhAIGhNM";
            s += "EUgIRU5CRFMkOhwGBAQLTVQOHFkvUkUwF0lkbXkbHUVUBgAcFA0gRQYFCBpB";
            s += "PU8FQSsaVycTAkJHYhsRSQAXABxUFzFFFggICkEDHR1OPxoqER1JDQhNEUgK";
            s += "TkJPDAUAJhwQAg0XQRUBFgArU04lUh0GDlNUGwpOCU9jeTY1HFJARE4xGA4L";
            s += "ACxSQTZSDxsJSw1ICFUdBgpTNjUcXk0OAUEDBxtUPRpCLQtFTgBPVB8NSRoK";
            s += "SREKLUUVAklkERgOCwAsUkE2Ug8bCUsNSAhVHQYKUyI7RQUFABoEVA0dWXQa";
            s += "Ry1SHgYOVBFIB08XQ0kUCnRvPgwQTgUbGBwAOVREYhAGAQBJEUgETgpPGR8E";
            s += "LUUGBQgaQRIaHEshGk03AQANR1QdBAkAFwAcUwE9AFxNY2QxGA4LACxSQTZS";
            s += "DxsJSw1ICFUdBgpTJjsIF00GAE1ULB1NPRpPLF5JAgJUVAUAAAYKCAFFXjUe";
            s += "DBBOFRwOBgA+T04pC0kDElMdC0VXBgYdFkU2CgtNEAEUVBwTWXhTVG5SGg8e";
            s += "AB0cRSo+AwgKRSANExlJCBQaBAsANU9TKxFJL0dMHRwRTAtPBRwQMAAATQcB";
            s += "FlRlIkw5QwA2GggaR0YBBg5ZTgIcAAw3SVIaAQcVEU8QTyEaYy0fDE4ITlhI";
            s += "Jk8DCkkcC3hFMQIEC0EbAVIqCFZBO1IdBgZUVA4QTgUWSR4QJwwRTWM=";

            return s;

        }

        private static string q7_string()
        {
            string s;

            s = "CRIwqt4+szDbqkNY+I0qbDe3LQz0wiw0SuxBQtAM5TDdMbjCMD/venUDW9BL";
            s += "PEXODbk6a48oMbAY6DDZsuLbc0uR9cp9hQ0QQGATyyCESq2NSsvhx5zKlLtz";
            s += "dsnfK5ED5srKjK7Fz4Q38/ttd+stL/9WnDzlJvAo7WBsjI5YJc2gmAYayNfm";
            s += "CW2lhZE/ZLG0CBD2aPw0W417QYb4cAIOW92jYRiJ4PTsBBHDe8o4JwqaUac6";
            s += "rqdi833kbyAOV/Y2RMbN0oDb9Rq8uRHvbrqQJaJieaswEtMkgUt3P5Ttgeh7";
            s += "J+hE6TR0uHot8WzHyAKNbUWHoi/5zcRCUipvVOYLoBZXlNu4qnwoCZRSBgvC";
            s += "wTdz3Cbsp/P2wXB8tiz6l9rL2bLhBt13Qxyhhu0H0+JKj6soSeX5ZD1Rpilp";
            s += "9ncR1tHW8+uurQKyXN4xKeGjaKLOejr2xDIw+aWF7GszU4qJhXBnXTIUUNUf";
            s += "RlwEpS6FZcsMzemQF30ezSJHfpW7DVHzwiLyeiTJRKoVUwo43PXupnJXDmUy";
            s += "sCa2nQz/iEwyor6kPekLv1csm1Pa2LZmbA9Ujzz8zb/gFXtQqBAN4zA8/wt0";
            s += "VfoOsEZwcsaLOWUPtF/Ry3VhlKwXE7gGH/bbShAIKQqMqqUkEucZ3HPHAVp7";
            s += "ZCn3Ox6+c5QJ3Uv8V7L7SprofPFN6F+kfDM4zAc59do5twgDoClCbxxG0L19";
            s += "TBGHiYP3CygeY1HLMrX6KqypJfFJW5O9wNIF0qfOC2lWFgwayOwq41xdFSCW";
            s += "0/EBSc7cJw3N06WThrW5LimAOt5L9c7Ik4YIxu0K9JZwAxfcU4ShYu6euYmW";
            s += "LP98+qvRnIrXkePugS9TSOJOHzKUoOcb1/KYd9NZFHEcp58Df6rXFiz9DSq8";
            s += "0rR5Kfs+M+Vuq5Z6zY98/SP0A6URIr9NFu+Cs9/gf+q4TRwsOzRMjMQzJL8f";
            s += "7TXPEHH2+qEcpDKz/5pE0cvrgHr63XKu4XbzLCOBz0DoFAw3vkuxGwJq4Cpx";
            s += "kt+eCtxSKUzNtXMn/mbPqPl4NZNJ8yzMqTFSODS4bYTBaN/uQYcOAF3NBYFd";
            s += "5x9TzIAoW6ai13a8h/s9i5FlVRJDe2cetQhArrIVBquF0L0mUXMWNPFKkaQE";
            s += "BsxpMCYh7pp7YlyCNode12k5jY1/lc8jQLQJ+EJHdCdM5t3emRzkPgND4a7O";
            s += "NhoIkUUS2R1oEV1toDj9iDzGVFwOvWyt4GzA9XdxT333JU/n8m+N6hs23MBc";
            s += "Z086kp9rJGVxZ5f80jRz3ZcjU6zWjR9ucRyjbsuVn1t4EJEm6A7KaHm13m0v";
            s += "wN/O4KYTiiY3aO3siayjNrrNBpn1OeLv9UUneLSCdxcUqjRvOrdA5NYv25Hb";
            s += "4wkFCIhC/Y2ze/kNyis6FrXtStcjKC1w9Kg8O25VXB1Fmpu+4nzpbNdJ9LXa";
            s += "hF7wjOPXN6dixVKpzwTYjEFDSMaMhaTOTCaqJig97624wv79URbCgsyzwaC7";
            s += "YXRtbTstbFuEFBee3uW7B3xXw72mymM2BS2uPQ5NIwmacbhta8aCRQEGqIZ0";
            s += "78YrrOlZIjar3lbTCo5o6nbbDq9bvilirWG/SgWINuc3pWl5CscRcgQQNp7o";
            s += "LBgrSkQkv9AjZYcvisnr89TxjoxBO0Y93jgp4T14LnVwWQVx3l3d6S1wlsci";
            s += "dVeaM24E/JtS8k9XAvgSoKCjyiqsawBMzScXCIRCk6nqX8ZaJU3rZ0LeOMTU";
            s += "w6MC4dC+aY9SrCvNQub19mBdtJUwOBOqGdfd5IoqQkaL6DfOkmpnsCs5PuLb";
            s += "GZBVhah5L87IY7r6TB1V7KboXH8PZIYc1zlemMZGU0o7+etxZWHgpdeX6JbJ";
            s += "Is3ilAzYqw/Hz65no7eUxcDg1aOaxemuPqnYRGhW6PvjZbwAtfQPlofhB0jT";
            s += "Ht5bRlzF17rn9q/6wzlc1ssp2xmeFzXoxffpELABV6+yj3gfQ/bxIB9NWjdZ";
            s += "K08RX9rjm9CcBlRQeTZrD67SYQWqRpT5t7zcVDnx1s7ZffLBWm/vXLfPzMaQ";
            s += "YEJ4EfoduSutjshXvR+VQRPs2TWcF7OsaE4csedKUGFuo9DYfFIHFDNg+1Py";
            s += "rlWJ0J/X0PduAuCZ+uQSsM/ex/vfXp6Z39ngq4exUXoPtAIqafrDMd8SuAty";
            s += "EZhyY9V9Lp2qNQDbl6JI39bDz+6pDmjJ2jlnpMCezRK89cG11IqiUWvIPxHj";
            s += "oiT1guH1uk4sQ2Pc1J4zjJNsZgoJDcPBbfss4kAqUJvQyFbzWshhtVeAv3dm";
            s += "gwUENIhNK/erjpgw2BIRayzYw001jAIF5c7rYg38o6x3YdAtU3d3QpuwG5xD";
            s += "fODxzfL3yEKQr48C/KqxI87uGwyg6H5gc2AcLU9JYt5QoDFoC7PFxcE3RVqc";
            s += "7/Um9Js9X9UyriEjftWt86/tEyG7F9tWGxGNEZo3MOydwX/7jtwoxQE5ybFj";
            s += "WndqLp8DV3naLQsh/Fz8JnTYHvOR72vuiw/x5D5PFuXV0aSVvmw5Wnb09q/B";
            s += "owS14WzoHH6ekaWbh78xlypn/L/M+nIIEX1Ol3TaVOqIxvXZ2sjm86xRz0Ed";
            s += "oHFfupSekdBULCqptxpFpBshZFvauUH8Ez7wA7wjL65GVlZ0f74U7MJVu9Sw";
            s += "sZdgsLmnsQvr5n2ojNNBEv+qKG2wpUYTmWRaRc5EClUNfhzh8iDdHIsl6edO";
            s += "ewORRrNiBay1NCzlfz1cj6VlYYQUM9bDEyqrwO400XQNpoFOxo4fxUdd+AHm";
            s += "CBhHbyCR81/C6LQTG2JQBvjykG4pmoqnYPxDyeiCEG+JFHmP1IL+jggdjWhL";
            s += "WQatslrWxuESEl3PEsrAkMF7gt0dBLgnWsc1cmzntG1rlXVi/Hs2TAU3RxEm";
            s += "MSWDFubSivLWSqZj/XfGWwVpP6fsnsfxpY3d3h/fTxDu7U8GddaFRQhJ+0ZO";
            s += "dx6nRJUW3u6xnhH3mYVRk88EMtpEpKrSIWfXphgDUPZ0f4agRzehkn9vtzCm";
            s += "NjFnQb0/shnqTh4Mo/8oommbsBTUKPYS7/1oQCi12QABjJDt+LyUan+4iwvC";
            s += "i0k0IUIHvk21381vC0ixYDZxzY64+xx/RNID+iplgzq9PDZgjc8L7jMg+2+m";
            s += "rxPS56e71m5E2zufZ4d+nFjIg+dHD/ShNPzVpXizRVUERztLuak8Asah3/yv";
            s += "wOrH1mKEMMGC1/6qfvZUgFLJH5V0Ep0n2K/Fbs0VljENIN8cjkCKdG8aBnef";
            s += "EhITdV7CVjXcivQ6efkbOQCfkfcwWpaBFC8tD/zebXFE+JshW16D4EWXMnSm";
            s += "/9HcGwHvtlAj04rwrZ5tRvAgf1IR83kqqiTvqfENcj7ddCFwtNZrQK7EJhgB";
            s += "5Tr1tBFcb9InPRtS3KYteYHl3HWR9t8E2YGE8IGrS1sQibxaK/C0kKbqIrKp";
            s += "npwtoOLsZPNbPw6K2jpko9NeZAx7PYFmamR4D50KtzgELQcaEsi5aCztMg7f";
            s += "p1mK6ijyMKIRKwNKIYHagRRVLNgQLg/WTKzGVbWwq6kQaQyArwQCUXo4uRty";
            s += "zGMaKbTG4dns1OFB1g7NCiPb6s1lv0/lHFAF6HwoYV/FPSL/pirxyDSBb/FR";
            s += "RA3PIfmvGfMUGFVWlyS7+O73l5oIJHxuaJrR4EenzAu4Avpa5d+VuiYbM10a";
            s += "LaVegVPvFn4pCP4U/Nbbw4OTCFX2HKmWEiVBB0O3J9xwXWpxN1Vr5CDi75Fq";
            s += "NhxYCjgSJzWOUD34Y1dAfcj57VINmQVEWyc8Tch8vg9MnHGCOfOjRqp0VGyA";
            s += "S15AVD2QS1V6fhRimJSVyT6QuGb8tKRsl2N+a2Xze36vgMhw7XK7zh//jC2H";

            return s;
        }

        private static string[] q8_string()
        {
            string[] s = new string[204];

            s[0] = "8a10247f90d0a05538888ad6205882196f5f6d05c21ec8dca0cb0be02c3f8b09e382963f443aa514daa501257b09a36bf8c4c392d8ca1bf4395f0d5f2542148c7e5ff22237969874bf66cb85357ef99956accf13ba1af36ca7a91a50533c4d89b7353f908c5a166774293b0bf6247391df69c87dacc4125a99ec417221b58170e633381e3847c6b1c28dda2913c011e13fc4406f8fe73bbf78e803e1d995ce4d";
            s[1] = "bd20aad820c9e387ea57408566e5844c1e470e9d6fbbdba3a6b4df1dd85bce2208f1944f1827d015df9c46c22803f41d1052acb721977f0ccc13db95c970252091ea5e36e423ee6a2f2d12ef909fcadd42529885d221af1225e32161b85e6dc03465cf398c937846b18bac05e88820a567caac113762753dffbe6ece09823bab5aee947a682bb3156f42df1d8dc320a897ee79981cf937390b4ae93eb8657f6c";
            s[2] = "ed9eccbe79394ca0575a81d1fa51443aa3e83e5e3cdb7a4c5897faa6b4ac123c1dde2dff4d7c5b77d2aa3c090cebce70340e7f0e0b754ca26b9c108ca0dbfdcd8aa230eb9420654b252ffc118e830179cc12b64b2819f81edcc2543d759c422c3850051d543c9bc1dcda7c2a6c9896e1161d61c3c13c80ee79c08379abf3e189f2ecf5f997db17db69467bf6dfd485522d084d6e00a329526848bb85414a7e6a";
            s[3] = "4c0a7faa755a8b877860d60f62e4a85e7319b39628d509211a00026d4d278dd7746c6feb6bf232d38298a321e48f8172eadb7782e188e1342bf088814329608ae756700d7786ab99602e83ab084407e05c349a0d72ee7662005784694d98fdf1d2e08efce71d14d940e7c4105d3fa5095454fe478401ba38f98a4eebd38477c613972c86f08e69f9e82e1ac09e67d81238271bb492da526bb1897bd330fe7b75";
            s[4] = "dd6a92b0659968f01d7b638960d747f7f0a0b20460de239b8f16d5a95936d1a4d9f4d3c77f5b14942d91304ce572dab54c8e4c01cab0df8d7653f0ef9fd486191a0ead2f1cfa71b30d7653322fde828b83f4ffafd2060727acf2c0d4062ed24fc9608bae7ab851cce4fde56d4ad593ed775ce856d7299e17d5f88325dddf7e268534710d3510ed24093d217f199afdb650581ac7962d0e281469e040beae01e1";
            s[5] = "b1f6508b1321e924066febfe8030a908e8086eb5ac423895b74ae91b9cea65e9d4249057b23b970e23f0b87b641b98cbc5fb7438a2844fd949a937f05f7670462266c3927177a2bf3c5873695ba9334c0d57e749e2132df586899c88827cad98efc7c9d74f001f57b31d3826e4448067ff43b2ab045a712372ce8f8a229e845289ecaa2e038eefc9ef4a710509f4ea14e695cf44977cbfbb1a9d806e43fe1af7";
            s[6] = "cc230392928a4146fe2dfea83e2567923bb2676de8b879e12cfaa09379bef6e599aec3187be62a7335b90c269247724880835747e302068a3325ed3a02feedd5c129aa8d2846db88e29ae896a8d9355483a54bbd70e1ed628c78a8eae97a3518f33b6d9c4d04ef3bf79df7cd1042eaf209d6d52a9f1f293ae3a699a3a3dbad77a9cc0d4b0b47db49397949b61ba6be52a140a3dc7dee41d106609df433d587c3";
            s[7] = "da56b86bb13657ddf180fec84bc8070fac27fded7d3d659d7951d1d7bd2b3c015463c2118fc4be14cb3c21f1249a6509d5b409d3181c447f0146ea80efc324fa5484d4fc6be037993d75d4f5deeb33ca0c71fca4fcc19ba6b6d8900ef0432e15d82a1a9e494f2ef12d4d3e3f1ec40f5ce75a6b0b2fd21656aae26e47af8ae60d5fba3dd86c9fe2dd116f8f443408c004ee826168dd888b3508b4eba633eddb60";
            s[8] = "855ad6a9215ba9db1eb2fc024fcafd9af2d31b81a60341b6024231cb1a6a1e291598cc886f042851c8f218e01a57141932b6cdbf3f19fe705a7fb7db0c9833f49a30906fe7d5e2a178fe4b92e089829e7c14da0659ce76441e0c17a54acd7b3840769496868907e83e9d2fb9fd2dbf1a230a0ee15eb978f2d6e12f9d63a686bf0361503a4e4234e8984c68b0e0882f3a0261bbe1d248f4c107ce2453ae63cbe6";
            s[9] = "abfb8e76094d4c27a44f3170271c9c0e312c192f42bc9597c928d8c9e461f5e2ffb555283e4c07914f82dc6dae5e8f3b3d4dd5cb3ab1ce8f0d8cde59ad92d5db56e2c6f584c1ede4a31403141ba42b18528d501f62c5cad6950c8d8f14c1c4439dcc27f4f20cecb1d7559758d5080d88fc7ec9045a201442027baad56ae952c5be100b55291d843720c92d10547022b12505e2000084ec72b069afa60f15258c";
            s[10] = "0471984bac512f7b486ddd641cc31823e66c7050ccec2ac71cf85f3493b32d8776e9d486ec29f8ab568451c8b60527d52b15d152db1b072acce870e3ff3541d814c52bc1393e416797a423c88d1563704ec8c16d820320063fddb89592d5c2a24e978fd5fa44fd0f8f76821b359caf041edc75f06c235ddc2561198553f1cdbeed11ec62cc9af29235ff619caede84679bb25f520313543becbb79becbfdd509";
            s[11] = "629f595ea8e740e8a3505770ef70d3a6ab74b17c6879a1983430e607b04c232657b8664557455b0432bc8c5913d520c3e82e29aede851d30bb2796216edf3addebe34afdf478d76077429006fdfcdc26dc054c5829c08cccdc330522a59316837b53cf5c38bf84d5ab1a536371e50d187f2aecb260ce79caffc3ec77a4bf7698174e1fcd4635648144e2d0d983715656f6efda616271f6baaf9d79ff8ee7c017";
            s[12] = "d72574d26e0273ac584fdc8e149196b528b3c8fbbe053616b5017e5638cd6ec86119712e32ecff6124ed3ef268d6a80f3e7e9fe9f7e891acf29392fd0391b77c0da13559c9aa963ec91882ba1e53d81d07056eef4535776e894154e07edfd0efd53dbff0a1ceaca47aeca4bdfd733d7a262a67101b691862d773c15e90254786e28ac66e559eaf756ca2ba342fcf07ee14c011998683e335c4f3ec60c9c15f15";
            s[13] = "6037304b0a7baad814a3b2ef8fa0be71beef741ab72785537e82b844609b36fc07822abc20bc28afeb8fc92fe47aa5164d1b7f89039094ee894d24f0fae9b32b14008033361f90c0b1b748bd555a0026d2203f1949b082421e2c7039d450890715dc7b5c7538da3607b7fe232b19fcb9b7dac6e8ca55fca01e12c868edd170f17567cca171e1c731d5c1573d1635ca8dd60eae9089b762d677aa0aab2ef95097";
            s[14] = "251c7b45c01ad596fcf1732283be13cf9d2c0ff8f2a5b93fbedb6a79d28acdeb54f88e24ae1c9a02cfb0856187217287df5ff63cb43595822853f609a789b3d78fffadbd891a90867b1568e2cc551d362ac257a94d91a7eb0028e6e9bb958ff7fd203b84ed085760d4c349268b635f0759f356b7efcfbbe3d9b54583c43a38590b171e3d554b67201b7f7353ceaeb1fb098046a173c105b07031e94185d0bce0";
            s[15] = "1b7ad8547ce04b015719fddaab0464f3a510905ea630a8feb7c9097486c52d7b4d41ef61351ea4086ccea0bcee8275cafdd00730d00b126d8dc2b393cbd99f6ca9fdc43e595aa1a5e664d6a49577ef73e1242ff47529cb7c106c772991da48bfd0c190eced3845546a021781ae0ff8acd94e44f67dda15637c5a46072d3543fa251d36398786303e7f2192c71c60b48f3f0a59e21014bb566ca76237022c041b";
            s[16] = "fe2977f56f9f414ddf898d21b3e13a48116dbf3ed13253ee69c2005ba1afd30d0665d03e76820e172af73ee8bb3ac387170f81530d73debf97d9dc73b6f4073089ef48ef43f24b0a0e69e2ed1f469482f9c74ae3618399bf6bf99bbf9a6476ed1d3398bc9a59f90feb131b2147795c39a38ec4ab883a3732f9a4c01fd3600047973b361d60f8457e63abbd29d7d73415d89c852305649125533ad73828e34d2e";
            s[17] = "e437a598aa79834390aa01e784b78bd452286d6258dc6ecce45f8bbd9f372a74c775c19eb37ea5308205ef05971cd6c7ee041e5504abf8f756f2c215563b12cc234ab1d9d709fe31a77707d2c7dbd5a88c75b01746675e3f242a56e222fb60206c88472deb198e3cc1a22ffd4005e2da131ef092cc78ea954d85453a29faa8185abdc35fe1ce1484e6f2f537fbbd84c08af0070ecfabcc3678b083e48a2abb87";
            s[18] = "972d40a3fe523a117967480b2e3ae1b7e513be738d708ff579e856b0bc19e2da4d4c1d2f3143720bbf3d58f1a067799e8209cfb3f3a4f75d920ef269e839ad3733e99f08b1785dc2f053b37c04a4f7051aef25f27c335bf3ae4873cd740e088002f7ac499b78c17345426e159fbada0af882f47b78c0dc520b901573530082f0c370bc8af1ccb4ed58b08806ebcda8b10ecd3d1d4b917eeb2cfbaaf3ee967eac";
            s[19] = "05ab18cccf11529d76c7fb4c4abe28681241d2cbf92db2cfc8739f7fc1b7a4dad4804a8ca0e8e33bf1e9737211a168f457176fc40be85796fba14e2945476e05231b58a4c41d250cce3eb19b507db40dee7c75d80721f32e15f02cb2ad21ad0ac0624b97ebf50500b613ea4fb875a1aa99ef6ffb1143e5d87b3111563d4a1a6bad39c1173448b7b1874a4e990fd13c4c93234e1ad3a7612b0454f83c0aabd22d";
            s[20] = "c95794a41d00f818a2d9a9c3d8367829b5c47be92826245228750113158a654316d860deef07d9b26c50a04fa5b0073ce1b8cb46177b0b0c02423807bc7979076bf5d1f17e32b06a6d5ff8eb906585f570eec97d1223c516faf7fca2ed26e3d86716fe238f44cdbe0290c7d87a36bc368dd09198b1b7abd2745c0f06042f4bd5dc49697a0a8d2fd88f164b57f67615f401172d3eab3694980849edc60703754d";
            s[21] = "96b0db74959d0c3ec0819f3d7239d8f4e74c0fab50f8a99130e5d2c80e4917f7632679ef0bfd303ad61fe005fed62afd574a75a014042a4b75eac3948886b2c99cd9bece7cef4fbc86c816b9cf6441040593759adef2ce62fd3bd97dc4d496b02c3a883172610af18ed57322d98b0b7f51776229969e27a06b6445c22651b6aea4dda50761d04ccb3bf1d99e353d11b967f24c08c1eba16beff4a2898ff1e641";
            s[22] = "9eed35024a40add6409a9690e570ef357dfc0b38491706783dbb6043bd4fcdaf01986fcccbf89f15bc53fe4aff70821b309aa5cec59ef3c588c1042593f9994644bca862152a20bf94dc0d288176eb9f49b7f814bf35050e83b139d2dbd5f08d3cef35e271ccc6d8074fc5fe1570886a0746ce19be8cea27c4382bd04d8d45c7b7fd9e3e89ad38eb37656577395fa0062e5f8e15be2c9a4833bb1f2fce90bb86";
            s[23] = "b5eea554b30b184185899a3c7c312cb1ba0c3fcffc406412e0b3aed64f3125cd227ec3c338af76fa5559f70f7a71b85b362b97c72aa402864f5fe572cc749d00a9513080fb0484224f039ac899d22934d2b24e3a3d96982dad4e04eb20974a2bb3b082cce404fb81739e49225fd3440e2c8ea6ee34e85706597a2edab42383adb134ca0a6461af0a530dbf35add348a0b879ef28a66a940ab13f113572e18e2c";
            s[24] = "9c87acba9cb25609ff339e3ebc5cecbda640690223d05f67c3a394f451b869c5822d6120976381eaf127624a055493b2dac4376c8bb87c0a0372456e17ec09318537b4670061a8c69da4d10bd6a24ae0036eae1fd7e486d0d184241cf22b9318158534439be1a9b983e7f8d5292b423db57144cd68e756380debd54eb6589322b7793289d08cd5b3149780b791bffd25b9318ea1e30b031171cbb2d09feb9715";
            s[25] = "6714d76c6b6779e80bc285a60e8cc599e15d68dc86b48ec6f236bbb4e35d6275380514d9febee8bcebac6f125affe10ea2767c31bf0a52bb1652116fbed7d348bc0e7163b6a2d3ad2d3871f972e9539e8e37bb90a7955a44f865de35467d7da098991760cc56d23690c12ad7049b0dc05c21d31ccbfc08375c1f171efaf223d1401ac4f1aa59573c7c429ac8b5fccdbfcdc6bfb78a378334339eadcb46ce23eb";
            s[26] = "a5954615b2f4fb346d49e867dc890c4aa0a29c53f80e2c23c73a6610aa64095df8817d34189b79b68984c9f9e2f92c52066a059dfed97e610cfd0f23cde1f4ebede8a049300d683eaf8f38bf36c627a3e8f3a68e134f162d021711f79d65570835a33f207028f5330c03a5cd3467b563bf53d30f0bc6ccec32c3883992314f6ff55d176814a0b213b124b69c0323ede75c4ea53021561d38ece0c822609f6a30";
            s[27] = "162189160d7682e4a71ab44ce4bf5c74fd15e01d2ccec755a0b1a9da4b07515d79c9e889bb372bc553d2f1f2f3cf8acef44f29dfe771dd06e18ba68b6ed5cf59b923d6182f4e4e5f5a043fb4be4fa98515367f3c1db2567f7bd9fbce48782578800cee1fd570fa839a5953c1962c2871eae88f78c4830a58cf5311f3945ffbde897d1305076e72b34ed4a00ac3c59aa4e70325a5e40cd332a663835e81042283";
            s[28] = "85d5934dc56a636fb3bf963e9b26dcbc69e3227461861368c23fa43f989d85ee1ef7e661e193abbf7926ee82878a2cdfec132e75660b32684df6cd35965115710be138877dde090c4bc01cdf43a82476b330ff1d1f2989e88e0d7eb4108163f99748289fb5264f44935b4f8428b2cdc49e95dbd8cc6dc4a2957f23ec7bc549ebb09d025d8e9d0bba765daf955e469f43f706d901a323c63ba51fc55261d9ad9a";
            s[29] = "72d8a640970bf9ce9f9f3a87fb908eebf7e603fb0589a00893381cc6662726130bc564c42ccef93447bb74065650138396d1452408641b05990cb6c0e73b1a93bed3e428c70a0a073777089d555b9195fb371bcaff16a24487918737f2fe4d43597664daf610196b3dbc25ade7e68cbe3be39712a1508e3a85531bd175c6484a48058104d6d6f18fd71db2bfd61e72987b092edced52e3f848e7a39e90ed07f5";
            s[30] = "18a498742ea54c0f2cb6548b014bcca1cae40ecac2c92a36fc359b95814471ad4f4d22782aeb8d6f66136c4cc17cb38a339a825e44cd6e41e75c578f61271e0ead4db3f46fbf55596e3c187a0a47645da7e671d1d7390892c0e8cd7e939de470cc21a1d427db9214c6e92f49ae747b00bd646bae4be4feaa0f7f14feafeb5de3bd3dfbb6c4d2320fe0598f99006c4e26d41b9245c6dbea2b7d70bc8176ef7e91";
            s[31] = "7ac2e99dd57d33ec02f7f970ea2a2a30b4521b53b18768cf885577c2a0ddebc837444c363ffeeedf87ddc938114c0a7e75e53798938a87f6c9f8c45f23f016b47af4475a5182b08c1f4c3ddc0537906a7f0b6574583ede7c1e71e0ab2bc0b89a1f2d08aea2e2df1447e283d7513022500363ea1a94d2bb224c5483fd7e921c0d86470a02df90065c3d2a68c80526c27b7b1a4e8109acf29dc15eb3f0ebee54a2";
            s[32] = "9b6211054cf023051dc6de2f154d048b28760d017439e4acf15ef9399c44ff6f792698b9f68b8670ce4ae92acb7f3014db8d76602d0e105e5c25a8a89cdfd67a0ad97742ed351921417ba5a3dd6093b65adfc81866540d2717d6055d11a901064edf74decf459f91c5b01877cb849603c00e992a3fd2dffa0e4275ca8fcdf1f00d97cb9018cea728f84a4a037276737bfe22e913fe4a019d56b4f4b810d1578d";
            s[33] = "73bfef877342cda6e7656622003ca23cf207d0d87cdf7b710d6f5a203d9d0a1873f7b85541b75bc29e8d0fbb1a2af867f796715e7aaf0325b54a0b0eac532267c1aa4d92204538fd71ebe107881504053ca9b8f60664c6303256af6b388f41ae196c9f0dd566937ac72ae61100bd37404c4a3703bccf6d1c570768f8011771c40f9d2d7e0803e67fea300c9f175f3c81ea98e766ea6f962ca0f999e0dd59aa88";
            s[34] = "e18d180805692f072599de2ce7d7bb126f59aa34cb09a51c9c4c4e8cb939211040421ea2973f1b968ebc1255ba836bde06ec9c81cacc7827af41c4e1d1c3334a9365c6d20c248214c30fdb59955d27625f02a93ecfac6392fcd8ac871ccb45efd710b93517922e82171a3ab1c700912014905ebc444699b5fdc2256c84ed3cdcc2993d54c9b971c7200cff0d3776be30eeeb3e7e3533bba3c9242b056f1c3db1";
            s[35] = "9912d81db2a9f7fd7aa71ee6d6bb3d4a2171891764b8c2713ffdc385fc5e477be9815ed3ac2cb4d22459ec7bc1899145e6010ac52a43f83c9a9bede255d9f9d56ff129178311cd369f65e3e652324d0cfabbee46ae060bb0b3028f7ccf392f94fbba7541bfc60b40d046efe29d3f973eb73ea064eef35a9d6d1b9e874ad5d0242dbc49e05c89f614af30b68c3d9ce86a2a105da1603e0bbc5ae25bb97eef20ab";
            s[36] = "ad273780a54c9b86c78d0edde05767983a2850a4dc517017d72f99fabb86387f5ac51885620376419a2835b59c5713dada58caac9970d67262902046ec09d8eb1f315e4d3e345ada768ea5ea795673f7243df722b0ad97de29481a351d9234246c628d5947e1c84d68145e4f37dd229bd35059713d7ef7ab426a6f4f2d4467ba9edc7d580f4d4dc451e05235eb78e806fa8ee260cfeb99ffb0affc5f073d47aa";
            s[37] = "5f1938686b21ca887be812db8a167d3c02ae4fa5b2dc3fff62855f1fd4fd223a33cd79f49296259801ca9199e4906d5dd59346e91426e2edc6a4867657b38634025fa48bae33b11e340c2dd179a933f4a8d246ef9081a3d5bbea945bd2847aec688a7cd791451a2cc5d49273a8341fbfe7d4879211e17e2483585aa845b30c6630183eaec7c56ab83a97e6ad6deffefd4afabf7216f0907747f73cc3c3cc3a76";
            s[38] = "5c221b27b55eb9a86e9c33ad4821a0ed8231d498a35a76a6c9d090f923f507eff17cfdb871becd01adc48b2b595d815beccd8331ccb811b830459d9ceb4c80caa3435c8a240d37b29896d0c1168bcdfc026c39a40c7fa25e543ec6996ceac3450d512c5d469a2716bdb6cb05de0eedf84ed1a65261f3c120ce6073e74ee61a593f43d11357f86fb5b067e49e646f6120e2d1a566af2555c914a9053659e4644f";
            s[39] = "1c7c75d5dc7788fd522aa5b9802829db5905b1473a6763d5b8506780620d24ad33e750770845fb1db3ca2a6ec80b3166d77d3af62ffd55e5b5f1e691da70baca5c2e09dfc9756656660d3fdacb54a899b17c1acd833e914c84827a7bde8edd9b2334ea6f97e8ae8463dfd55190ee40265ac501fd3d4ccc3d4902e9d3cadd0bc0b3bf7368bd2448f0e0e041e3dd6252f1f2cd80cfa9e57e9cd2ac757d43751dee";
            s[40] = "edf041362a040258e823cb5b43362cfc999a6c7a24e5ad328e920f2bee58c68cf915e91b242100c7dfc09d548ab2149513badc28eecd58af740eef50c549bc817e229df73abfc183cdbf6f17720c934a6d911d40c3e57f9edcd3404a0fb9ae3c7b018dce904e1a9d6fa4583d23dac50fa24b2d8390eb0b013c86a8c4567a1faa13614917685abf7d32f049d70f5e598d79f9af8d57cce9722ae27a3d770f4b87";
            s[41] = "5891e30532337de940d24eecd41a66b6d67fe95468d87b5a435c7ccf878b84f129a5da489fba7a6b1703a0d25ff3cf5821ad2ec5740323074f39684a5ee5f77b0cf38c7a26e5ffc0260ca7ce107a43986d74cfc884fc96cd5e547534722d6f8fb8bce5bde6b613edf0c58a885cc24464c32756b7508831658792a2e53c27c4bd5705df47221d0df2aebcaf96f62c71b9f295dea67d781b1f8e4e4c388c5da113";
            s[42] = "b66e364a5dde79e0931fb1c7011cacdd4200f5c2d3ec1890a8328cb76d4f6188125c395e252ba5e230740fb639b8fcf3cc4376df816e336337f381f5d20332c55af9147e2f284000ccdef57b71f57f86b6580d3fa07df96312c18839267f613436f3d08bca2d91c1fb30354780fd00ebbfdf364cd4d95642afe0dc4c802399627f3035a80aef062c2cc32e4d10c315f9bc2cd6c8027adc0e84c6e7175e41920d";
            s[43] = "e6fe6ba39e62b5d74a2a5626edf7af4361dce0c68375a41da8d440542f2d7153a5c17b92a878759611b04352882bba761eab7c83ce083937cf70f7ad847cd831bd7bc78f2176f6cf7e6970b8a8d31b950ef381a3d758df9baa3327d8341a131e3974db3ec9aec1025833df854bbd9b8ee9040f7aec638b3204e2615f84f06de88350eb8239586f8d7f21a16065af4e63d0a1d357c9b97ddb5a9a9d0e0a7a07b0";
            s[44] = "5a398d3939915edefe5a9a868d0007ea4290e9ad06e9cae3e3f5545146e1097441906cd746e742137d72b57db769599072c78745be3a61e75cdf39b36ed16b981020fa80683e7acca0a336f5d3e1114adf71b87b42dea425772feb5e11230f617f32bf4cb79f08059025ea758b51925181b06e7069b6e8c4417a41ff1bb356199201fcf2be77b8245bbd0c7bd5c4112d71e02ef3388cefc65c501387202a5778";
            s[45] = "a6f59254597a80789198257b1c4afb3e3e1115b6abd757e4bfbe9a37b8e75a9fcf169909f75e9c431567f89599cc3f3def026657fc611336f691525a3f6f574f7c2ea81cf5b91fffc4407b99506f06daae682253e7f53448e17582a2a0f4e0446ba399f33e154dd83a2f50c90be6a194c6dae51d52a9f357d9307af734a9a517f9cbc37b20311cafec6786008ef81b5b5d7203c3507a12758dad1da672dcd089";
            s[46] = "9fe0c137a408737bf0edda26dfed799300636d1dae89c806e91f0a7744a1e3a0c5817ccce30e4d0df932dc54271ab7ca6ce58e070e28ce5a7c08897dfbb261e54ec9781dc748ca1ae95874f14fae8fd98aafa2f85c66f3c4a09f0cc2e8add7b2e02777ff943645802fda6ae863b250f4d44e481996eb3d833c91e73a054c3787619a3e483ba88c78e3fc44cfb83223654dbf83f81364e80e20b667e926793575";
            s[47] = "a7768cd20873b4bde23aebe2d75fdf7e89aebf00b0e29af26788cc8a39a20529ac1d56eaa8afd7e441a1194545ffbd5a253df1a45b7852cf4dd48934d8a6fddf83c2c2d08f8cef7272b72d8dfaf3353e230ed7f35a3c0b0c3815d59e52a0643df16cb294f36a98ceeeecf07ac03971b90791c23c3c44a6a9d8d03d11172aa79c399cb42f11c228bb96b8a73eff4de46a0748bd9cc2ce210423e9b36f357c671c";
            s[48] = "9c1511ec979946309b7ebe7cd852e70373b9055d1385b26042fca1f5a281b27094469783d25fcb79ff237a4193976d1d2e99cbe62729335d492af4df8cd374eb93299ba478732a0a35866d154ab247993f5a6d39bf88aee31d59e6f85c7ac540f26cb8e8f445176ea9d9901d4c006b8abd3549d0204c086bc37587f7260ac298bc91b2291241daa3c7cedf1dcfc4a07f5334561e562188a852d37961d77139de";
            s[49] = "dcc71ff06ae66dc397898a78042c5dbc7ba6e26239e3768e73dad3d50985eafc608bf11aa65b2dd87a2fb02106a0e2bdefdd2642eed4332601f5e5c1de1820391380cc20a965c7abd6e29334ad360719c32ec8141c5be5d5f20666f33a9888882ef713126f1b5e8ea670c4bbc89d659906a0426df9c6a8b508583299b5a23b187a1471db4233518bb73360a9b4da1ad89ab7f6a0801a3b469119dfbc423e2460";
            s[50] = "fc98eae247986086a16a4c8f1c03fcde1c3c166c6f0fab5d69247679b8df05c7870b57ac0663598c1467b2831344ef18b2821e18513230c85cb66a738e41f7f44922efc06e7516c24a2e24718cccd1fb10fa1ce8415d96675f26137368d157a0199f4c083977a86f50579d786662ad0735f9f2c6757e7a1e9b21f0b0538e8118e4ba3824dd4362d4de815018b286863cc98bc0301093e29b6828796b07ad0f03";
            s[51] = "72096e97889f59b90143012cad09621824645bb73933e7fc1678f7fc0e481160ae6f582a6571aaea67f17961207718056e22d4eb802c75093d8b9eead2a572307feabbbb50a724314535c2d7f8f4dfe447e48546a74109c3d2757b6b1d13401a8918ff3efa95301a0f1b9383a4848cd981fe7f51bc97123aa4178c69171fcd7e402a4f8d2aa9fa6b37b961375c1c23c9776549192fbd014b723c3b18e94980e4";
            s[52] = "294d751a11c97374016a320cb0bf8055aa626efd89b001a2395015e685be976e3609769d4aef65a7a52d05306d072d00d96bbd97c7303f72a8636be03121b3dc038ed9a83a09b12a9441bf6894528bf05aef24910b70287c36bbfb008971971528da3985a795b53278b7895c9f21c09727e3503c5ca45726ad1dd3f9ef4b98b2f7148045406d6239a764196885718a142a7b1556ba17065195845af76fc114b9";
            s[53] = "2614c5dc2763b73eb35a47a25c9a345bbbcb63aa0deeea220dc53212b484bb979cc7140c6fb8ce85925f3de771b96d3fc1876aceef6cc5163fd9234a17b84b539e2c1d8e25e1dd630a5de4a09b0189ac067164d57d9ae96f4026acdf30ceb0c1587a35873b560f81eb456fbc923d062b12d14823e1e8f979375c125a8750a0eba3a60572545ad6878da4f8dc4ff6287b3db60f79f62e71cc4208357d77d63f30";
            s[54] = "e6d0c1937e2c536c51bcf19327a29e42521626310ba1311ec07176ccf1843d3b20c380781d6f9f7102bf612e2c3da981124dd6b98c1592ae151ca74916502f69567a47a4d19d3aa5df761e36ba63f29893b870a058cb28a57ebc1996156c610d7d8398b2dffdccabbb88bb2ed3a4d31659eb5f4d8d9ecdc24b93b67fadc1afda19d671fc8776bfad98715181a03b995042195d04362e6e67bf178b6c30e35504";
            s[55] = "8120ba7045f7a9406aba4dfc0b38113652f0974a193e6d7c89cfc90af4274ad5ba6885c67afed567ba46c7b0a49ca1c3e511cf09c39ef1d316a40bae8d623fed41b0264c823107c4bff4cbbe0d8474eaa28e3fa0697db02afe74f73dc3a93bad499200183d91ef8cf2daaa964f0495cdbb0edbac1e1c3ca11f95045ff21a82c3c48bcaed0bf0318fb5f71083b721939541b284d89c529b51c64419a9a382a469";
            s[56] = "635596989e52fa53e7d9e7e64e44ed7939d61927a1d703e2528d999eb59354a6153094ba23ec33685da97dc1cad44a794d87ce4223017e38964c9a6541da2a7ba15042ad615ecc3f9e705d63219dcdfc2abfcb1ddb13a59d305263a9e8cb349154cf6d0ef595397e8ae5ce469db0a19ef94bb0d29d639d8de08922a19fe70cc013dc35d8703e81faf31a4582df916d57fdcfb46cb013fce5eed4a92bc266993a";
            s[57] = "0ebe80e9fcd2182a5b78afa930cad0b636b2f0bff5006e8f9fa1e61f38a60cc89e69fb209017e0740a476638b76022d473a2ae0c9be53dc4af61046a286f4f756540479f3d7ce2a8bc376174845770d294a0918755e94a583f641ffeed2b0da563a2a196d3f149a0b3a2464f6965932722292cede87c509c3f2dcdd13c28f4363c857eb5385fd9847c766a4508dde8db43faa64135b951674f7ca0097cae40e2";
            s[58] = "8bec4c9cdf0d91fa4fe2282b3491dbf21e375f75abc4a56c407068b1daa9c066c7d84d6b207c3d37eb326ed858f83fb26e3d36ff8624377e244a3d98f21d7d518d6a54f56ce0d3d75bc8919d8cd0cc05b3c5dcb02e8cb1f4a762e8fc76666b968a2e5a07fc37d59767febdb40579fea58adb3c2a86012c05842329eeee21d4d2ae21a424f3c656b37262b1fb49db8caa2f51e01e0212a112b763086022cb84d5";
            s[59] = "eb5deba76131a74bc74af0167c011f74973281bc2be283d2fae7a2d37b675d0cbff05ace01f7ad510ad4d7e668337b566e44c2e017da24f2625a7360bacecf6d8e06ce1a2a16d1c36bcf6e7f6a8261e30119bfe42e0c9719bdcf6bab85d81888b0ba95524a7491f795d7d0408a3eed101d349bd9e7527900fce1e7c3048955319a8712c96a0222b4b1ef397467fc48733148e01076b507687b1dd44606365fd4";
            s[60] = "e50069b11598be706fa281e34685a742ebeb874694a704a83e0fdad38009a56f91aadd07778eea5c472c8ebf31b885fd9fbbcf5c67e69b8f8d66d478d8faab05b0ff4fcb23a57ae92b7c643fa2975383ae410a254732327ea71e43963104ada6d8b1226ae12dfec697eac6269c2d4982cf3482300291fba53bfab6bc35eae2e23dec2776bd5c62d08d6696e688216294b04cc27c36e6195d0d238524eaff76e0";
            s[61] = "6f8a4b9afe48394b1498da14381b74c1fe78202740befc9638063f02558db37d07865c65667a5fb74c402d884752cf5767b2fe72bc2f494d9c5fcbbf8a5c27428ecdb0460b45f3f81c169b200ae3df2d031a6e57aee64df77005a12c251248e8815f4306732ffc25e07028064c8be6ff567b750a1302cba12badd6c58401c767c4acbaad3066de04978e1fe4ecb1883d27e14a61027b633b5c0aa4d9d5c049e6";
            s[62] = "df8fabd0276c0671dfe70110c367c3916096e9fa2c11fd68c9e5254dd19155e704a535de2cfdf2e49a498026c44f69e747a3e169b5333209516f46b8947bebd2e045f18b43ca71c7ba22d5f314db2e834b8455095cd4f16c1e7d611e32bd2d2e60ca8b532dd8b81978aff08f91a76f74dfdb7b0e80632837d4eb0850e0183b9ef26ceefc7b79ae332701228dc8f03eafc3ed238ad90f84c409b84dba0852998b";
            s[63] = "c504bf5d2d4c60e435f13d2790b4ccddadd011a83fe46b1044aa3d731eaf270a32a6223c4d4d590c2ab739da7c8c3f40f9c444c7fb72fa0674ca2c424e85d5ed0865e865d356230c78e0eb138a25fb601226a352817165b3f9d74104c826de733fdc5d182ad72b5fae99caf78d50a83ed4c29d52c49b3ee4c6c2234e133a5855992cf70622e4b109fae5aefbb9c35eb2e70c170ba0c73024b2e234eee274c023";
            s[64] = "9cc79a16010c54042fd3417ababb2c35885efa6f6d6eb038a73ebb78779e0562acd4c041f602f70ef4a003d2773f91684955c56b9cba9e4d8afd477c48a170901e10b0a46afd6bafc222ffcbdf484d6bec242314f7a1dba70c71515bd89b646b0c115f83b9974c06ddbc3237ddc41e0a3e39f20494a81fc6bf4ea39bfca127ffc0472d47d18af7e5958061691b6fb946b2301ad3af7d83d899b772d1dc99290d";
            s[65] = "0ae1a45422f235d1384fb78de2223956a94fa4ca95da18a4718a9bd9a8aa481c1761449178c8f5c6f989d67ef822e3f04ba837a8c00fc65f256d6040a7757588e2bbc0e49632befe723190e64463fb65933ab9e483c8993030b7752c87c5e2a86be543774cf321f078a79099f49a4f47398409a7e7c63cbc45cce30cebe7625e4ac7d1a99693ebad56018205d58d3d847d9c26734dfaf61d0dea4fe97d7b696c";
            s[66] = "871c43d879e1c26c5a4fe347363301f309079bfd12498ce1c0f4fdb25f2bd16d32951d3afe7c86ca4dcf498a1d751f4e810b5db09c5967ac1f4698e25ff8431719e64a65d5dcd5203b4589d1bb1395b3ad91dfd98a0c3812054a892304bf2c839e87198c97ec1add230e27ae7a77672ecf4dd7437322f6dcffff61f72e0928da18086cb4911c9253c1728132379e7f2da963596fdab76afe4864cb0da5ec82ef";
            s[67] = "9636aac7d58076270654761611b445f26981dca15eb722d647ea4187bddff3993c18f86da35bf3b65b033ac201cf36e2caa8b0395a798685360f32c16d026ae2dcb1ff5830feadaf3a4f3c7111f6a8ccf4a82232af38fbac70f42e44a54c966a0c1efd43ca679e67276d7f331bd2a2c6493367e815ff5a01c55bea9a3ab72e6255a8643eb9361f59a18878e19f803f35ad8653426dcdffc0aabd22c2f83ba485";
            s[68] = "1cfeb40cd6dd4d38215706a8c79bb5ab61e7e64a46616d9d24973aa400f4d64f32f89ddef2eb652c97e3a633b771150bed0cf22abea2d77a445ef894eda35c49a12ae645fb5f2e207ef8531dcec96e79f62cdd68a28ab062b47e21978de285c439d5fcc5c8c8f1ae1b92c681f39e1c62013f4f9b3723a3167ed77976a399574e58c82d9498072e52fa67dde0b315c31f986b389f090d34f4915b4c1b545edeed";
            s[69] = "bf8e8ef17ddebe4332b9225c59ef3c66bea13a4876b288575eb669a6d292a92524fe395468b0831a8e3711c449248bc00dd7525c3d4ee2e51ec7b166ab1743b930b417be1ea95910eab11c3d537ee6b1d96defd5ead7ad7b91ebdcc87d55dd92c8cd8bf48f865bf8c50e4d645c4cbc0fe639733e7f22e69f37a8082b89ced3f2ce8ec3af3025359165d2b1cf5b32d3e153b48a03ffb408c07e15089373b6ede3";
            s[70] = "563517a9319b861e7de18e3b4eb5798b79d7a766145aa727b5bef9cb00a0fda2fec7cb9a60020ce11e2c59323752c5420fc16bf51db63997d9a981b6be1e8e067d588ccf1e1e483addab6126de539bb11da8dc0b0452198ad80b07c8fa004e4bee61582573eb151752626a7ad276ef72b2683a96ac16bd737e76ba5b0ef92cec2ad699ce173c1100188c7bfe974579de69add387ff5aec0eda11423058e7184a";
            s[71] = "ba88956c4b2f1d102ed6aedc976d401c882e548322d11260a4913cd6a692f45ad686fe71945866e59ea1ef4247411df8f4daab1b2c04571a802bcdcc33c82982f5a8d6f640eb8ffef80d35fc3defae0412d34eef6d547797dbf8a5f2ad41cb672378b887c509855b1bc48bf2dc65d82eb13b68032c73b51b510f1487865613a3035f6bf2d28c485378b7e3ec16de75d1599e18a494003049864c16ff3df396f7";
            s[72] = "0da7aa0791c748ade375cb27a06e7678936edee2d7613d925d56fcd7d080c9368abc9dd9e096557d95b1dd0b30cf1379f539241046b60a594eaf70a0dc70a2f5ca3e5129be8b025890fd60a9f60fb3f369b4e7b01f71a1b4d53251631f1c95bac15ad46d944cb980f8aeff3e7c817a83a719aeac0b253963220aedb5afd20875cecffb24237e62ea90d3f89cbbe08e8a7ea1442ce08bdb253a7d5d825e520b41";
            s[73] = "7c42c2bd4e3dcf1604998f68c97b972dce2d7d2e4aa91337c497e8167df04c652a80a947ef92caf164f13574037ca56709abbe5bc4ed5d0a2b6fac4ba577fe21d3d57fd689c5308422ac1147e9859fd100e184b4a759498680daed20d6c38b36873b324248edc8df3d97b75250a8bdc2a6250aa6100b039e2d89ccfbac8ce580cc60d774d9dca7560e9f054378cd321a9fb1ab3d0cdda40950ed97d083d41672";
            s[74] = "e1f7b433ea6829da76f20f6406c2d106ae9af4603d0b2f4c3a5e910f554fd4fd37282128c90fcd9d09c39e1577b584515598ae7147b9eb9f0a8118c39d20684da9d75f41f2b995967bc14bc6f0c0fc4de4e36b9a9f5f28a7965f9ed09c806a0e7e122753d1ae5340f9f3022081c3ba46eb584cf3a20488025ea738b92f074c08eb5dbc18dc9b7b73aef4d9cddb1dba466a2cafffbecf72eacf5316bd6911063d";
            s[75] = "2e616a4afac8ff14997875e6af38c2afe9d6d072950c8352a992a799c2ea7e67fcba8ce9f814850a79b7697896d93be69b8e61bcc38f93482de016d7c423f56aa552af97da41a0c766c562f7818fb85f8f6d4286812c7aa9c813a5337bfaefb07eb26039e558abbe50aaab557283727e9abd0d7248f4c9051d26d216b2e0ce48633186c14cb1779f76890ecf8ace3e46b46dbf61e97b0b592184d1728f7a0b2f";
            s[76] = "c65208e98fcdd6aa99ec2386a4c9fe171ab4e7fd2785e561d5431390687e54a451dd64e11db47ae68b955bbf9812ae1511241919115a1773e91f2f10b050d3bdd3e3de85484507685cc19208430a73bfeeef9ad5703d122f41267a3201235f6327e566490ab79413a640244a30a4f39ac16cb632988ea6a113a23b737b5b9caaccbce581c23ce1a2e2d10cb55f8f0246c3b6c833e51d979dc55c58ad619b573d";
            s[77] = "57a7ea611bb1fd027168546218b6d693dd1c7f36e17e22ef18bb0893295e39fe560a50e87e37a796af6b837611390d77cc4506ac270bcdb24440aa7166e8c7b73735f00ec7e57048c6ed686ac3ca582ef5d5e4d315193b64f1b343422baa83f8f0185730fcbfa086a5ba17660ebf11e2c30e1ace11820ac8f6ec5831b9d1da8e1f8eb4ebb63e6580e52d365726dda5c8c3b3e3e7ece55a90cd44af91e29e2daa";
            s[78] = "7c2baf2d810982cfb42d88578ba534beb667fd3fabc08187f8cd081b286ab01a3c86312579484ecb4dbfb34cdc999ea0a7eb306309a25a2ee2821a4e1ccb9e73f173f8ffba4952ab1ffc8dc56ccfff3684458f2187b16bb3d7ca8b04d79ae55187951ffa02c6082be9f991cdf1a8da01a6e1cc1939dc5ea62fdfa16235ebc492e9f373dcced886f6ccab84aa8cd1a65a3bbff4fca574beb78c2c859fdf2e5341";
            s[79] = "cf908d17119961c0b68e32761a47e65de9a0c19139c8d730c2e286f1726c777db4a7eda6638b2f4b14e4d2143294ef803c0d6a8096dca846b800f1b60589e38bdd0211378f9f84984f2224ae96ded22ad00c3565174d5bccbfda546344b2e12aae9fc421deee7691d8a076b67c4fa29aba650e3a9a25913dbcbbdbeec0441b6683395e00ac49e0de56dc2d7251f1809c6e1463e2a25b9452a18097063cfe170c";
            s[80] = "ee715dc3ae77c9825286c1f85d07eecb285e931153fdb104ffef7b207a244edd71200d7f536636344da91a093f5f9167862cc879073fcf31a10192190381ac054c4f1034011f8379b5e9202ec911f8a13bb16254924a3a90dea47718ebbdf7ba54516994dcb5c377e522631da0b22bc6953d63f06808d62943ab6d4a1ecfb6def827ed9787ad92645f82a73f9ac8426d8535282e977a968a0068c06d10248138";
            s[81] = "33e3eefbe3683fcbceade4017b01f4825ad12754ecd46bd9ada3fd864e2a362d5239ffa96b2310b10848fa72312207db91a7385160909a20256ac8d345efbc11c4ea653ed156d9a79ca03b9cc3c6cc4c418fe1188ef9cb293da50e35efa3c6941accc18a275a36b9d5b825e216109dff61aba2ef63ecd1ce19e6131a233990c97b403c8f53691bf7ad711d42dcdbd28d0e9265ba1396a961edb3f7fd0b65225c";
            s[82] = "c33ee6bde2ae0fcc7e45c080516704533aaa1bd93198ec29bece1daf59dce7cb2bdd03e136a49412b2efa5eaafd40ccfb4db03eda231caa3ca4f27a7fd45719851014edd840f94515355006f10209abbd3ba778fe743c64bde25a8fa166ec2be579e314c204196603194bb498264ed1bd0532be423e6129564c83bc1b5e75b57167d9a48445c2e1240ad0fc05615f9a7a5640f9813117a94087712fa2b9a9907";
            s[83] = "0402e3dfb95a6f9d5656eb216048d399566cf168610652e0d4c318eac6d6930da9d8ebc46646841bc9db67ff08530977e48c9a9ec715d2b14d6db4d9ce7c4c1e3e8f205b8924b0a6dff21608ad015b46599284f5d71daa26e6fbed67b10c62cb62b9de163b9b2c38efa926de47ba9523d5bd9835a7470d2c02732d6549f082f38a39e4b17d9ddccdc1c3f56a25d713f83f82fd52816a74c1620bd6aa5f0461e9";
            s[84] = "f69c9b51ac03c43c9fbbf45979b2ed25695d9511e91c307f6a8af95b208f3ac91c64066f0744f956625e83eda34ddde6f41ccaa984704c4652d3c5ab6116eee088f96db303b5b62347a5954df990f11ee52c7b93197af1189861f0dcf70d4cb6d1a417b76ce3f69e76e312f00a9b413a3bf6281b7299ee1920c0e12a908ce3a6dc9f260856fb1207ee03e227f7e52405c0d30600f079fdaf8ccb1c1984806f63";
            s[85] = "e7e8a2ccea1420a7ef916f4ff11a7d7c9ce59671a8e319a28394924f4d9513d355dad9a4cd4ab4ae142c29096a8a06cd65bbaf8cadfb288384a14829781ffd1d0028babedc9e01396f70b9f55ede0136b13acb63e273508ff29f2be6c96ab06939a6aff015853e40732282e559eec31a205ff220fc81d4adf2d85679756da7e12238d5e2bb3b2585440cc33bf51a652c4bfd13d6e23306c555c595a70daef040";
            s[86] = "a6110e5a6f1954f221fd1fad1d46f873ebcc12b113b2f5db602b61844abb88b388a4ad8e7da9920bcada4ad6694a916302bef75f3d67e30d36f01a0f2b19b54fe87fd5542e9beb2ecae3ddf58c5fd0c53c713b35e1fa5d138ed0d1c2fb23d80e097ef8651fedd04704b3ce9c5dec41582a75716f7f6ab6a84b2bd58b85e670a96bf28676532a7e667752545d1dabfae827a60c17e7d50a9b055fbd46300fed23";
            s[87] = "e2b52b85ad60e58b71d8770a34d1292940a4d1f237aba02295166ca2781e7648d2d392ebfd1e1fe4fd4c17f1e9a6753923d71cb0885622b446307751f7859beb944c214e92973060a6837489228633b314ba352096a12ad7a96bb74841764180b3d194154f30a3a55ee91d99cd915f369e773c70af0bdbb657af8d141eb869c99ffabf0aa238ac959dd7486e8a9f34b6eb9acadadaaad4ec271663c5b49b154c";
            s[88] = "238587d193f3f483befa803e462049976b2a46f9709819dd40dfc460609dd6f3e080c17d438295b547e9758762fc8d7a54d259d59ce42a8cb237c0fba449f80ab0dc572afa6cb42e0bfef3527f491c1c0074a753afdd6fbade2a9b05211f9913f6a764ae231949d32c257b7d708379a4ce7ba934a041b96716c5f2f4ea9d7ef0fb4f45b80cd164bd08e17a07a2d59918da5e1e99ae86fd3d37ba048588c0ff60";
            s[89] = "7c7117ac1637be1806ff4bad67cc6cefd37a6105f719a2ca78777f9b0ff0beb95b799dd02d9b5e75805edc6b8904201d146ed3d30c6f5b7cca111f492f385829eb783d6c1b6f2cfb42389721e657497722d0034601e175f255df58193a8c9aa85a255a58a27d0bda12a3472cf2ecc92d1b01adf7527d59f28f197ad9cf8149e6c2edbbb63bf62ad4d8ffa6af063df3bd2531e30b8b2f0eedc4655a85a0b1507e";
            s[90] = "6b455a025a012f52d23328bde5ce12cf34c857dfe3a3cf0103623f2e23cdbf7fcc8ea4dbae50270a17a42bdd40e7bc70a914b38736ab8add806b76c729de80e4ba32577bebd3399cfc7fdfe5a6fb26be3ecd6bc10f58fa9af8196236d260ba651c87f09cb5074651899d2da94fd68ec04d9595bdfcac13a384567da78a4e1f4aafc7a6d96bd06e56fcd37474ce43a94d642f6c372e81a1e7ec9e918d8a9b7547";
            s[91] = "7dcba26ee9faf206a569a2a0d0ac5d18329eab019b2d63810666d8ee3499cd89b76ee1abbcbef4685d66f59d075dc14ebe0bca909d0ddbb49b804638f45fc680cab7ca5fd6a71173fc86415c154ddae49c770e715e5e664818f33813e7fb87f66f124dd777a7374c44dedc1dffa2b6bbb6f1d78ce79c2bd75ec39a553a54e1a59358a818d8573e063b043f65efe6c0165935eeac7fee7539efff7fc9e990b8f8";
            s[92] = "15d2ad5936987551d8e52429eabdde68a7bd063e67e01606876ca555a2eeac073b4ebd5934b64e211f38fc958888dc986c8c329fd225337c01e1ca992c6a67e71be713200548a3643407f2882699ce67cf441f47a94107bba9229b2723dd6ed64825419b1161a34705e75213eee5887acfa99498810e8cf65ba69bd4362908ae98a09ca1364a73ab43fb948e2d628c4b48cc9f5926911813397e02a34d3c7887";
            s[93] = "c02fef796de9393640c495e177c8366f08b2c7e2714d8347e7e0172733062d5fa2f605ac626dc1d2620262e758127f647a7bf86f42484b20eacfec3ae680154b20041419fa10b206e3956de8e3e528626a984dc5a0c1d25f6353b1bac4ea132b7d4f5bb24e35061d0800d3f2d9b4a3db208aa118064d055dba3954908dbd1a9b5811145c2fe2afd2567fe69bb12381eef2d8aa54e2bcbcf06bdb5d981112ab4a";
            s[94] = "61d18c04b2da973cb1b92831e03f2a719342bb1db4a0be31090c037f3b921cbb514958bba9154b33c9bd90d18072ad405c45acd11ffb2cfaddca7b4f379a05f6f406ba92dc573829ec01a45a5dd0a4b11a37447379de1e88fe4aec80412bf880680930641604529657dd3911a9fb183938e58f027737c8084cc49b48e6f608852fa47f7894f9f221dfea77ccd266c5ab72e6b474047482a5a484b99302360d04";
            s[95] = "42981c52740395bd1fe931ac4d69aa079ff3c20c8f41e0573799e4ed0a131d18edc87d5fb5ecb1f83fab3992f65b1a21fe3b3852e21e3a7c14c23b9e925fbc6fe1fc54c1de075366f2a3a98bfb1d2f205271d9cbecd96524b025c43ca146e9edc7ddda466955936bab80844b2216c733264814d15291504bfa86fc2e2731eb4d60a968db11d300312918ef521df4e271a7eb816c6f82365a32d2879b1076f9a7";
            s[96] = "a54b59ccf99a9bd1d3123b3dcbfed82e03e32cd8913238cbf15fed0554d27ce353a0e559321b7041ad315bfa1eaef5e381f8e944a277e869a4c141552b2b900338443f6ea1f5c4598978e91d40fb05f70e9f4e7ff097dbcfb7fc738d726d4ac3ba445f8f8092dc02efe1d522f9b0686f4106d8fd1aab9573b7301596a895e0dd5f1e5d9c190724f815b9e3af96312977606dd95e7d12bad197aed3d4e8990183";
            s[97] = "79913e2226cb3942e2035a2fce0425cd76bb9a419834a87db2616d540272462e2b9641d70665cb5ed77169fd4f91997fc353b1d2c67173d678458f669b4dae1b2022dbb982a47e393053f9a4f6251dbcbee3f956e5c94e691b164e952e5b6faffa13fe54292a3a4bc0a24380b0aa37f21b46adb17459fc050ba0aa06e2617a0f08b1996f5fabeb738c166c8ad64b46aa669709651f8603acfcfda921691c9bc0";
            s[98] = "cd0ec41b770b73b83fba99249ac7d4c9ae390467f939acc45da75f80e3c2197a14e388109a48c41b59a5876faa6a487585fb6f443ce81c0b6fa2d3029e4892b21e15080500eab6692a207c31f9b95a854bbe7bc5de2f21466b31acd8c33c316e5929c4f60652ba999cd1953ea322a70a856ac421c5d3679760dbfbaa3774faf277466e3992496e1e7f5171a1d38a90385ffb5014fb5cdbabe6a498c2a1dd0eef";
            s[99] = "9ea41993cd1bc03f9b930559d80f522ca9bea117815a76b6ffc016cc5c6dd219725bd27d77eb2d86043df3012672f07f104d99b213febb67eb7afbd029d711dd767fe82c8f6c353113b98581fc77d239deee6da238b03ef4d9fe1f94583f4ca46d03e9ef6b561482b5b2e94490e9147439be0614cb76a94ccb0f157a7d5f9fa9bb88746d1404204a455c486839ac9bdd9f8b7b9e87e2c446928ea7582a70369f";
            s[100] = "f3ad1dd610276dd58969641170f06a02f9ae902ff236bdd5f225d1aabdd33d18a9cad219f661ff9e4ee980ad2c9a7d675d29653baa66b8a800d0e5a3e3043ccc179576b1738ca66574d8357ee51a419091d1b81204956947d7ed886605983a67db21f128e12178a25146cb31b56481a516a618158f06b693fd12e95a2e719b8a2c59d1317d042b61dbac1162036d20b1f42e470e70beea90eb8ac7d3dd3153f2";
            s[101] = "a0eae030b02cb9427fc5b3331f87ee11413673029e7b704c12fb6ff793990000b07e51c6247c2821b9a7fea5519cb59d205010a0a8d61f14c3a6e6d4a0a56e074b47be6962bc50efbb87d551f7f2edb050a16dea2ae242b545dec9991c7c0657ef9c8b3eda246bc83daba3d0b4d736e9076ea00880d8cd584c82b57f394090f673c630102b40cfccf88b89cde18dac32b701fafaef40b6b4da92610d9de35506";
            s[102] = "89337575556106dbc8d63e5398c3f80907d2e2dc5173f9b25bfb189afdbe93e91ee9e6381b37b79d03298af9ec4347f5c2a6ee7c1db060784793fd841582d756e9ec899d066afa7a87e480255fc2bead29446e121690f7db994904bdeb37b52baa9378948da479c1ccc1a2acabee961338ad89384383a5bc2800a1212be1609b0a93fb957f773e8c706b0aba324941263c744cf82e6fcdcad3f61fc4d9079c4a";
            s[103] = "87c8bdf4644c06b3d0392b7eee0bf9b112bc90c505824ce485607a039ae032b3a1b08d04340cc03af12ef6c1a409af124a49cf8e109f48bbcba4a5eb555681376aa65ad72205bcc53b5fa98a1005cbe0c34fc9e245971ae6743a968094ae0dfde27bf0454eb6af260150391efa1c853800f958e7aadaf2c93fdd9b892d666bbaaa7e23e303ad7932d2904f416594a58123208bb1dd84b161c3dc522fab3ea5dd";
            s[104] = "5a010dd5af854f928deaa20794d9cbf1fd8fc4c5cd06d311e09cab2bb1b8e36e45bacabf9b2e246cc8e6b2e1728893304b8c476a1dced4561404d53f3816f7af63ffa5576589ee33cfeaa43a9e5861e21b45a81bbb8300c4bbac95b9f9dbb9c50503ff9f6e4cd9cb9106cc8022454bec97a386705163904597fdb86c582301345a624e69cee2b2f18850b6e0f05fae3dee26628eb85dc9929df0782008397745";
            s[105] = "1387f82d0416cb810910caad7458eb33e5f215d25b6bda3c74797ae0f3c6011b8dd73776dca3b0e3172e1ba6d73ae5c4ceae3d9a569f4e4db4be567a9497b4fc933c3ddc9c95893f7ca979a31921e8e71913395ab2eb62abd43050528d93ce77addb618c6185ae2dd06879cfda10964e1b08068d2ffe18f69bc6f6d35fba952c4a7f6e51fc1a98168dd42444f6958cedec4b4ce7146c64da1833e376859ff5dc";
            s[106] = "a7c53662c30007687a7d69b24b98063308c923761636e43f7cb14b4b865e6f73df2f0cabbbe7d31e81083e91df1eec9602a369d5b0c54c32107f55a4866cd92743ec804256d9c67a7c2be4de7ca39eea31332d73acf470561886571c3635cca62bfa3c96aacbc72a458f1173d77306e52a1fc651d5be9cf6fa163a147028366412144f6f69de1bf87aeb476084e8146a4ae0f156aa8292287580432f82ce84c2";
            s[107] = "d229258baa0f320da070138c1c542bf342de984bcb1914e1ec00c79ff257f89168783ab7502745fb18482cbb73bbe25eeb887d361bfb5eb4045268714d9704a8ec12eef23e301517fa5608fdebf41fc2cbfb2259847ad673b2052160ad1cb9c4f39dc8e2f83dbeca3e63cede73c72c437be5b202f9a0ec62bc8fb64da39abec120f559ca5b864c82a7d0996fbf7e79e280036129610b9fb875a8d415e76036b1";
            s[108] = "b148a13d9a04ba6ef17afb0e25a6c91a454ec0eded513a567a9824dd3cd16770f4c1dae48854c2cf557139640c1cd121cac974f74f7001aa4927f6bdb4e0fa73676855df520e2af6ac785a420e43e829fa4e77e5de386d58404d42aa57bf56467f98322275df9f1a72fbb03fa8ea8b84356bbcd7159c59ef283a1aec240ef5d25df6e2aaaea36826beb03b0826d4abc8f22837812dafe6c9623517471fc653b9";
            s[109] = "10673c1edaab3f72c43bbaccca6d7e57c431173d959194936c77abe1c9de51121a96c3259def6e13ae0e4b5273851211147f5bb68eb2d6b8afef610d1d441dd62c1d4b58cb07ae0fb1459fb4adca6bc4ceb668ff89078a5f7b551ff4938ca67b868375b561b02a4ae0d9deee674f7c463da3c6e3f906691620e8bcd771e2e6dc021f96de49dfa2814c675128c3dbced0c180b9e9a151c4deb1657c8eeda9691d";
            s[110] = "90f51ef3c5ec5c487661f814db06fc348a865813d34c5c07d1146922131766c363d264cd54f6cd7a673c9195807e0c6d1bcc717e0def10c8993d646a6805513cf6164d17f4b4ea631903f69d0729669b71e595bc6d9a6c90d6e7ea07c5ef5b3475b02e9c0499143574e3c3a5e7113ee95291f8a4afa383b427d520025e4b84843e923cbe64ab00a2b66215e298d5dadcdfdd95c202936353121b25cc57461099";
            s[111] = "83922bda8f9e49a140380ee6707373caa0809408561947696d097bfd375764761d35641adf23b73b28c03d337e24c2c2bbb677510416bad360f6daa719aba4cd33df035e3a5d3f92c6e1105dda2179ee66707074c4ff424a614cf0f65fb6397c0342817a0900ea1622189d6e1865f42897c14e1cfb781078d934bb3e9b82c56379f8cf3a5414db99b26ea0f76db908112258b60062ebbb7ed0f70df3bbe98c27";
            s[112] = "a85778013115e2e1438383ea9cde9af751416d76c312b21eaf077510b0d0701b13af17f464332837ac3f433d500f13097c3128f63bc9f170f64167b40cddb61c2246376fb21fc108f7db686c5fc58a3339616bcd45e12b67444020bafe2720108ca1de21475d0d95b87488339329367ca9dd59fb795215eda44f374385aa5e79b52a70c6989f16dc39bc0f350eff9679d00b995d4076df12dc72796b8a8f50b1";
            s[113] = "86ca1968815ac1b731eb97200a3709c9b4bbe6dd5acac1374445d30f38344f071c0c2ab1f1848ef3c4c8c3c50a698cf81ca782287452365b3c71d8942e4c434be00fbae1620e6ef353eca4a567360c4215b88af7e4e880a064a9beafcf7b7f66b788eec1d9df52a071dbe360ed995816d8a1d18f685f677f9b3c85d68e967d5d50e7cface661062ae15524784812a2e37a4188659c411f3a9f3237076298aded";
            s[114] = "4b0528929f64f67fb8b3aaccf224ec75b206f0162b45a7c43c51623691745b6f5e550c640465d72243f9a8431fc8d3d7ce5a4bbd7368e68f73542fe801e2e0a7dd3561168eb1ac6f2ef03f68ad364bbf0be5c7717c8e6089ff2e1c9da9370aa3354928eb0d8e0981bc18b7652aedc48a767639a2e597c3cfb1273479ff84a93985377e5b66ddcfef2932d16ceead9ef45de94f8c240c84ebe83c264da19ed41a";
            s[115] = "2a3e7d226848d48144a7f14097861e791c5d8f89b7118966806fec7c92a6a078a65f8d40469d37bd60da1a44da63a9ca1680ce16d16b1eacf3be569bba99eab9a522894ddc1fd21c2ce5c5211ced70b638cf5ae3675054c1c25b9c0c83af2bee3733c602c7c57a518e6fd16eaf8f2e65966017e6920a3c45d182587aabbfe63da3cfbe3aa1baf3814c0db0a54d0ad24dfc98f010628c0e772382a6e64e848e39";
            s[116] = "142576258685aea788c4bbae38cc0e56d161cb4c61e964f8593a0863d1187d7065ba7fba1d5635d08a9b817d307fc1aa50e3d2ccab9a3282b1af89c5923d46bbabf7009f874387084d97fed40c5f0beeccad4cf41b191f8eaf47c59afb577e6786877e07d7e9c4df5974e9b11212f2dc7947a3d95283622adfb5cc10ea8fb22981e1a423d01870fc97e3e6d64bb7523690a44a1434cb26dfba27efa2e99ff8b5";
            s[117] = "453448b64fc193c1b14f30a38e8dd341abdba8f2a441f83b699de49a9ecaf89e0ddb7aa55ec8031fd78a106d859aa716cfbc7eff87c2e615894bc067382f1a75029251b152026f7e3d2e6067a560853d152a7986a24d68cccc17ac69d43952987a37380d10392f9b004f1f56a141cab65adf9f3ecba1468be89506c0941b5b59330325a6a8c7578b47ed80b8859544046b709b06fc73d682e420e74a458a2d56";
            s[118] = "31213438c9334719556b0da94761a36f6619df09d11dce7324555ddcc5dea353ce5ff3a613e11e2363fcf80288b5cd4dcc6fbbb0e798e1fdd81fb0d0c8c046cc45c9f1d4565c7a348eeb4024a69fae2a83786c13942ced7e5612c097f9d0aa221a6c4a43cae656d99de0b9d82b55d1bfe59b9b150ffc2474a7fe43b4ad85ca9cb7bf0ec7fe30d1ce55915d289c51f467167febf98196283f57e6f93aa180770f";
            s[119] = "8f59fabb1f74e632870a2450cc065e7b7cefb160b94a83681ffc09d13130df3f0de8afb16eadc592f5db7a27ee5654eaa7d106b176c792405295dcd1c8b186e6c38f4ad0b352768eaf68e3cc71b0decbf056c52dc77759b7d6360efeb4a314f63fd5c1d48a519fa4a3a76dd920d7e66b65b98e653230b1c08efe0a60dee8584215f59827c5d7cdfc01e5ab541a685ecdaa3561d8abd4b313197102bc544e31ea";
            s[120] = "9bcf858f8da8817c14d138d086e10ad9a7f6c888c525268005f10900ecd89fe557c2369e26b74657d6a6a8e361998885562ad1853b88da9ed07ff9ed93431f7f8f71f56a301a48b8c297c15259113ea6441b655e3de65e614fa7e62b70113eba7546fd45f7047aeb873675e22016a34c4c45a43d5f0caf47e8b0c0862dfa9fc6fd2d47dba41ea0255f5dbe87ef16b01161954ef4fb1e07de2a88e5af40d7cf34";
            s[121] = "2fc2d979753d8ad7be05e124d5221d1b0987f0c0a0c3f7a45eb20dc78ae781296ae97fb03953d5c166506df0eba71ed1d255f24f4927eb2ca3ad779dcdfe6b4bd966d4e004797230e9d638632c4757d75ca8ea71d2ae788cf1f652ecc3e5e2f4618f08c34b314fb2dc8812a38a68f6d68c3c6cdd443b83b0db40d633e5c6987fea55ea81e33f71dc7cfd89177a8d6250812ec1254f8172f0703aac81afc284c6";
            s[122] = "50b09e60a3f73eb762e044c0ec9d2e3f69a3a1d2938c810024e27bf918c36fb124ff343dc650d66dfa930511973b0e9d9f38d7d29f9def91e0a709b8acf229042fcba6eb12646fa1fd15e7bbafece40078d16af566cb5c638c5982c056ce5ee618943f8ec23ed12d837ce26c86f6af16bd095e84df2b2e41a3f1793fac41f3e198830931258f22b6e488f1267ed6c859ed9acddd2de63bb1536524ab60ae1518";
            s[123] = "7f7b9bf30bdbd3ff69056cd772cd7c7d36f0ba3d6faec40bef857382433ad28faa7c95b4e5fb0b4df3352858baeed36cb2197c34f1d587f40885a72d9c06914b7eaef9cb4ef32abd6724d62a625fe4ed94ffc8a8d651a3fcb420e15c93bfd034c6fe564c61855c97745d8f2b0ea616720597a07046bac8b12381811679eccf38ec1f80683eced2002d0cd2887024a12dcb377b52c26a819642568c5a7bd47943";
            s[124] = "b6fa8ec4e0614ae8d1cd80d53ea6b9ef4897daaae766ecaf63b89e3b2c7aefe41538e6ada1a9f2f127a369d137ff9705e128f7bf929a77016306e524e0f482fb822a513dd6c819834c1240ad6e05ab328fdf1cb0d6b0b84a1060334c3e0c7879769067a012734adf4e5205e920f6ab17e93e194d2fcb57bf7d0fbcfb8986945eef133dd2f01c1155a64aa8d3ef1591fa77cfcb1a2d0d91f5694b1c059ad348d8";
            s[125] = "e914f7f3b3e74ac771edf57ed0962f94191fdd13144801978356fb76f04035e8394621a31306f157039345d828c96bbb42a107eca547c2f2d7148fc53f1af4107c5251bdf432d0fc66f79b6d6d0bd4f1d85f15541f306ba3a8ebebd17b7f7476922455324cfa0ca47fcbbb258baf0b2df7d09dc41f52284cd7dbf7d665786c86a4bc5940db36b2a1a80d0069921b50264fe6c1af073c18b86e2ea4c6ec512eaf";
            s[126] = "526c57ef7d5f7b1de12585a25fd7dd4046d75a7eafce16228d45ebf421e37bc6493c77627e36e431eacb67970a3cd2df757202dace88ae53690e8ac3f566dd615bab61a24a5ee530a26b9dd919d1a0c520a675394db6c94e461a3c0006cd7c422994b714dc57c0f026eeee5396e3e188ee80485d091927f8dfbbf3b2009323d2099c58dbb9b256afad2d24d1c43b4d15934d6616446d309f45723aa1de923e48";
            s[127] = "6e79bd2186a5c84578f4a895f43c9f7336a6c7751262f4ab2db09a3af67f14b8dcc15a79992986f63db39f9ee04da469502ad90cd55f556a67c524b9f610d05eaba8347fb86d7bd1367115a22c5138d60086b90fba66d74bb5901b7cb7386006b77bd196892846e55ad1642d9841ed43c59391e96a02c9c58ec094f211356640efd40e50f3d5e4a07fd9cc8e0784ab6e4e38007280849f27acf065835ce6b037";
            s[128] = "c48aa6bd467d3e031e1afa40c93bd789fae12de227a2d989db062f69147bd4dfbe364897e0861f14c2a8c86716930f3bf217cf918303ab96e5c76f6b222e74eea2142a4ccc516d25104a43c883fc593568e68f6c25ff40c5cbb566f425e7de43f12b8d69f51861f4ac1ec3076d9e593b1a8c67a5af7a7e4a7345710e1e716e223808a24ff25946494db8bd5e47b05c1be530c685cea68a2494275c330beb3bad";
            s[129] = "143c6e7b667e6c8872108da70e4896accabea7b76163b3c4ee3bf0f3551a24d86aa4ae029579e21e23ca5504658721a0198d79b64270e9f248d588fb6c5a7f6f4f618fba10b03b4d687aba7d4464526d27abc7ae7d3f3669340f1af280ab5efdef1364450cc076e3e046c2fb532edcde528a4b7d393ff33b2bc530066d208d8d7a0229012a1f0f310f3f2345ff84fc68ea8734d26718626a06b029b5b2762c09";
            s[130] = "df37782dc76e95d8c84cb883c5517025b42a04de301aef0656e1bf09fa152ea9382d8218ab4dc7d23f50542559b8d2242fed01bc44e560a75f6d2bd3783d87e3230206d4c5f0e046d4da193807f4a53eb0f7234ad4e6c3246ae8f848d7af73a6976f2fb9dbf3d4f190c2e94bb8d98127df13ec6b30a6781bd36a9a90c0e1f9a09231f309ad2a89f94cbbf45f9e0fe34191faf95dd1482a6fea01fb1ed677bc4c";
            s[131] = "5c4ca78f8de3527788e7d1efcd6aad0adc3878ea70993ae20937ef0a601730494946f078de2099c62de9af1c47ee4f18216ed5a7268464f210374dbf421d55449c8f399d8824c5a0ff8526a940223ee999a6f945f0ba3eaa672c434ad867ac7adaa46bd3289729c6c7d920dd0d8237bf678d88bde91e0683e72e88fef50bdb23cceb6270acba5aeebd0a834ccf99cd3e6bad8c158f5819f1f1c785fdaa3df505";
            s[132] = "d880619740a8a19b7840a8a31c810a3d08649af70dc06f4fd5d2d69c744cd283e2dd052f6b641dbf9d11b0348542bb5708649af70dc06f4fd5d2d69c744cd2839475c9dfdbc1d46597949d9c7e82bf5a08649af70dc06f4fd5d2d69c744cd28397a93eab8d6aecd566489154789a6b0308649af70dc06f4fd5d2d69c744cd283d403180c98c8f6db1f2a3f9c4040deb0ab51b29933f2c123c58386b06fba186a";
            s[133] = "b563aac8275730bd4cf89ab32bb4b152be8fae16afab58ab3ea0e825c8ce28ddbe26c8cafef763f1d9c3f30d60335cd0b765b98a11d5cfbe7a2d75e8f8a5e851ee6a17de174d8bea5c1e089beffc99709d6dcc03e578220eccdfa99d3fa0a3d2f6736de041cd783ad7f866df5dcd2a752cfbfc380cf84da5c5dd3fc486cf1adc14d29d9e91737514e8c67d5c5aece4a19216e2b069f53b8ab4acaef17f815004";
            s[134] = "61750df604b4ddc333b9e669b218e46bbe50be4b2d3a76e8043e79e98eac204797138e0eae7f39ce256a1a315476039a8f0776a724270f32b32c973d6316f281081970dccb8c13b9f8df927b1dd29eec37f4b68a3e3179a62329bcee1407d00cb1ff2a636a42ca8e939e6a9c56a382289ae5cbb7eb07df1182a7ef10882f28a7fe5d8582a2fb93b4fdf02ccd5b9050b60665771a21c0f35cb29eea40710dec75";
            s[135] = "32d13a6e522e33e556b5b084b1e6811157e88ef0a897b6e62d31c984a51957eebf6b1e46ea80b1def31d34a99bb5a630d9127537537dda3f88eaf2bee4c37d859cf91387963e67748e7c09b3e0c1c8ce56937bded53e2b353dcb39e8aafe8ccdf71e82d88df62184750b125bd09064e296c5caca1122ec13aa030cf79ab9394746a377ae2e93a78986fa3531098c3e904a6b0cddaa4c8dea9b807cf63f464dce";
            s[136] = "06b741133a836fc85541f7cd567567fb972db462ecbaee424d5b6739d1610b49c782f090ebef367313424bde55f1a0778136b12736f85792d242f6528631887dca216e4b9c68be2d7c070c3a552e97659d27bc3783e0039aa305f8c468fad6b4611c80c8325639a8fcee045877c80d5ef7852775fbe2984cbb0b7373957d888607141129bfbab0439a13c0ff96a1867006e80f3169b5463f5578ae0663fba8fc";
            s[137] = "1a8ee9810be01477ac27603d611511406ddc13dafb632055244d1aa9c48bd25220cdb6d866dde2a038d6970831059e4cb9f6c99437f5b18d97701d15c353ef1cc3a102a2aa224b28d8aa3a1b5be2c3cb843b0c358db57058862df57bb72d618bc11bdf853ee3f254a90ae6bf281bf71689d30e46d09cae6f312bda779a09927c6bf5a78e0e3bd633788b84af7bdf832a5130c503c6f14c207d9b37d7000860ba";
            s[138] = "dbdb1be943bdd334d497b2667ff2360ed4d679742a349a85720b118c5a6d72d82c7bdd8b3fd4b6ff2ac48b501848114a3392381cc1bbe96cfe561c09381af8a156729fe9ff32ab9bb5af8381e8bfee5971be6ce4f93640c53adf6cd7545ed7d67963207ff164a8bab4f237509b4a1138378b25fc13dc18674cc8dd38b56e1a5ba56fb91206cf8cdf916bd761a38b1a69d9e19269351dbc149c6fd936441d4feb";
            s[139] = "d4fd8dec0a4e36e1ee573e4a9229bf826140d8693004efbeee7879b4d4396d647710584c7abd1322a72ec272e77d0840780e03a3cbeb6f1bc034d3b43029d159cb305962fd00525a2ee4c08c8def3300be80acdbc9b72a71992f29dc9e7a2e0e849acbabd18ac462410223565dee57b6d32b462e2c72988252596a0f54c450274678ade9ebc1cc9ba13099b0243619f25cc3e3092b1188e4ab81813df028b61e";
            s[140] = "f870496fd68a5cfcab9893c5919bddc1a3700a2734a461a349c2c63cf84c11b0c6746e3d749aac5353f65033646ddf08c017acfeb1971fde9eaa28d52a0940bd547a009a48e2989835f8465dd9398e8842483e5cec0134b98186dd5afc2cc67b5ca3018d291e651ac0f76311777a88265ad7a8c7a5113823adaf09c215abb421971461502cd3db04631ca690e8a47f2fa26645b70553b8920c991d14029a1f15";
            s[141] = "df0ce1bd0178f16591520f2151a53cf1a7d093dc2fa78802b0e24f53227a41d43a027eab6356ad4fb00ec1f0b25f5ab4c6cde3b21dd369048b7b03c48dc8d50ba9c74f0861c0daefd2eb3ee175a70624b10e347c3cbdb3366f82f4cd7cc547fdb8aa61fd8d625f46755e9f4de0c26342b18bf9dfbd1291972adc88b71f268b550715e379ae229c6193ab8a9ee63efb654d0c02558540dd41ce8ded2861a942bf";
            s[142] = "0584e1b50d800f10f6ed71c24bbdf57e846549f5dbd4c19a7bfa6eb5f3eae1c9300cf1a05f79a6b6505c22d7d51423bcf90c804b78422eb106b6f6f1824a8ff4e636d7fd56a487b9f84d506e1d31cfc81083070096319497a3026a72544296da46c55595812d5f291c40fc5c79361584c222d72100d11bb7f9b3a7d0a7e9319d021f320865f9e78668ff2b53b1bedf8cc1749df956c62c663dc2849eaa6f89bd";
            s[143] = "bb9f7d72594a7784b9542c489c9968720a7b63909c4b71549b48f53f760ab672eb0148cf2d551fcd75c92748f4d83e237d4183ade38df1fc83dfecd1873c3105b62f673c936466c2b54af3efddb00818ab838e28580ce2dea3e7a7975f8a4782780b2aee0da2ddeddf1468ea68ed08bb37ec9b6093848189530d79c6b83f93cf44bfa4815987d0b5cec886dab7ce4d7ca69505a312ee9dd5ec14e373f3b785ea";
            s[144] = "78f49a8bf13c09d1456dba5adf8ed318d4929a1202d2ee4d9598ad5f82d0c065886d90de5b0db5aaa360730c555bea866c2eac517ac780e9327778ad14dda9feb1eef63e65e5b91e594fd369add2113ee17792797622cda7849fd0ba6d1d25ae824321251aa739efb3d3d10671ab0fcde2712d206a46a7ad18affcc2322f596870e83a9870264d2c93decba66fb61e1e5cb9fe1d6437ac794f1fede9e9ea7190";
            s[145] = "0ec873128086df42cd3c655e4cd07de6fd0130442d65481574198ca51b73bb44675fac69f109164a484362ac834a46c952b29f13a19629c47e4dd45262b24fc51ee787daab371a210ccf04744b0a4ce0e8d020ab6d89aac2b4d938b223af2b8e52b86318abeb5c9b8c549a0eb737f2c70d7c50d6d6acd000cc263d673dd9e8e51de2ee9d9a4a4c01750f3810030dafb6adae804a8d4937ddcf6a8b154c5fbcc8";
            s[146] = "6de3e2bcc8f15fbbe9d4f5b8ede81c7b1a22ccebfbf57af59bce75463ee333aeda67fd01148cada144b6c87d77aa073ba97e71439732348b447c848497cb1aa3b17b4ebe5547d24cb8b4bb0f600f1b2e17f44674569beb563df1d8d65b88ca151dad2ea419f15d1ecbddf4842abb8fe9155c7d55e8331f87b945ab58e9cff8783620ecb734ef09237980ecadf42349c29e48bf69821be8fea65f0028514d1218";
            s[147] = "68519e1c9cb71716eaca86409cb3fcebd0824f9a7644fd5ff7b0cd8d119cfb338faae18866a79c8531b119a3790847636c0cfcc36a5e067d254b73152f451d26be586e5a5af4baaa949d25f93127a21fe5d8d78c91e6c501e31ae790ff1538fb2af33d21c3098cbe2c31949732fb7ddf4a2bc66a2f2a6443c1151bd0e4ed376ebc0806ae7e6f040728e5ff5b8a2cc5aa05a6fa5d6ecfcee62e1a769a08553df7";
            s[148] = "e9192de6e18c2212a9dd0a15c6d44ff8436c0c55b601e489a04e8a5dca6d1c27d89cfcd33058732cd62a6e2fa3c2c68b7160dc742636c3510257be5b411c825ed415f0499c162b0d6c7d4e4681576484e9a83e42f37ef1621914c0b1c1aef1d13552e790d303feb0a9d37da4dfb1d8e13bb168f569b4d2de8d771a9fc26aad060ad9d2859b47fbe516f182cae3f1dc8b2e10193a73fd85ab18d861034996363e";
            s[149] = "81b452905a7ce49cd1970c2528a81bff37438b0d5fa46be3972a8af9ea42de08b1323016acdc098076a1d96b23f5756aaf17f21a7466e8f9e35a57a1e590531e00e3e06e9139de0d689e7f4dbe4c51e24e27f1cb449904245172a1233414274fa8610296e6ff32efa95fc109a430134f72d9fb7fe3a8f757f247fa479a6be9b709a5755881989137f344cd2863e6662f8920c4d4f9d56518c29bed26f7cdbbe3";
            s[150] = "76b4bd5e74a04c232114f7bc2dde69ad03016a4d9caa6221e5af5af6a21d5a6cc6467f7bc474ac39a9e63fb29139fd5568f17dd4fde624602ab51fbb7c342efb32e74ef6beb3f0f7f5d8f1dd182d8508ae76426f709ce483c59ea4fb5d333f436246dd55e2262d5ea838808065a4b675ebdf4096eeac9c2ada7a4b5ae996497349eef97f297c819b4c3ef3f7ea77aa5040f993846d7282e4eade08ef7b69e280";
            s[151] = "03c8fe0d084bf989374efb55a1d23d205c771d95028fd2df5d9894e553ae3ae8e495ea9339dde7cd03cc8964a7b09d48836845f4d5e772ac57ceb6ed8132a091db8d580692b5255e5d6304a42dda211b202e95f4a1a1c75b27bf9fb16a376b17325657005cbc1030e13689552163a51e3e0c1cad3f5b9668106b263474221d3615fa81cd5188cda9b3a936cd5353ac22874ec3c0809d05d8e54027b0d86935c0";
            s[152] = "fbc9664d90ce4e0714e0f7218ce2a4fbb94f0c11fbf801b593862f268c10dad88807296c34ff60064eabbbc3148fe24645ceadc927d5c29ba0bae863f36baba73af13818e18aff57e7c2a07d42fb9d4e377e0b7eb5cdf89688255d7d3d1b17adfb5339c1eaa893fe50acb8ca84cb6f41ba7273ce67270a0fc575248374e70f7816e1153e0ba2a2b67ae774a6189be6adca806a2e737f5fcb90119110e85c2c08";
            s[153] = "e15363715632ef6a4842fa179d62ab8c00048a1163deb50189c0f39138d2893e43272142693d461abcc0af99b766e1c57605220835d15086ad0c05bb0ee42eafb41f70cbbe6bb43b416e2e096444a4958a31c65813192aa6c1e0234e14805ab77d7bb1cf043e0167b415b1fc8343a8d2621d3468c0b18f6b67a84175f19557c16b2eb7d505fc3c5d2a2bc0dc72a54a4ade03a1d9ad4cbc8446ed62139d27598d";
            s[154] = "a89be467d7742e8b406b26d1eca536631d095a40fbc71c16f81a88596c6f4dba12656b3b0be04e584d27f015f7ec6175497de0154814317e1545920094a00637caf5504fb3880d848aa4b4ad9dd31066f2862be860698158a46606a527e96d817670bb05692e6bf4dcdf35db9300096b6215bab8716c5a6c6f3253b2c498a2049ef1a17581fe495c58fd801ef0d01c6cb3aea3acf263822df61b8346167bbda4";
            s[155] = "8a16f31ba90de9c00b91a469e02536743080ca9665520e469e46770d5824b61e1954d4170284470a40867497cc44274ffe0937c9d2c2c1c573082617c3ab0b525054da5f14fb0b996c6682a4298b04dd071d9d069ce4b7f45fc8b4184c5d508568314d852280955e3bd0278b72bb39e80d57eab841fadb5d56a382a71329c7666a09e78b907cc90ebf3e442567a9cf9d5adf50b7132cace6a7c682327c86ca06";
            s[156] = "0acb1c12e8b403dd557fcfe6e0fe39a0abcce80b16c7df02046d4803246fbdf32ea630fdc1260c1ada965fa49eeeca41667bb14005f1dc41b73f6a4091e202f4b69958cc2018ccc4ec596c4d8ebbda4f33a7f669c6f6e2a473cb02e14f3681adca3438489a339cdc104334e4299d368d95c36cf3a59abbda58cdb0b063157258e47175a867f84efa0c840891c2704c4ad8dcc2d3261eb6dff8aa9d954f362dce";
            s[157] = "0aa7e6dc522878e4b4285f6fb4c6d910dbf6dfa9cff497e4e5e9acf94d9f875b6a44e104c75da1c18cc8c38fc5dd6a996c65c20ccbfe59f235fae69b335b7bbf1b7383620aac0e3263f2670219225a0af5d7705432bdbed76428da335ae0d1458c31aab01d201cb391fa400c60935dfb71c0cb1c518f16d24defc2c6cc08f760f57d065d6a130dcfab41e1982d111fa5f1a6cbd559919b0a6f176795a490143e";
            s[158] = "82c2881ffb531d9aeb8ed181e45c840274e915fe7c68fe739b389c47d7ec37bb0b755682602b410701935fe06f3913b5c8c2673005c9be92ce2eca5254db1780131f88a9ffb089aff222a31f5bd5a1369967d346e51a2f27d2c1c3c1366a294fda19fdbcda143d82258b5b530a8cdec3f9c45894be4854f2ebab12b4a39652a6f0137c3cd4ad999d6ec99e2e2edafeb2380f4baca1d58226938f11cfae22e1f5";
            s[159] = "fbab9368d87575fc20764e143a03c57a9adffbce7c0af1763fa46b72b85371c774be61389effc8c6448d85ed3e70241140c242207d6d710823247fe03c84e0f4495aa61ca533f4dd76a84ab01fc147e28ec3fd06ac8dbe5599c7111616feea1b6d5670b216bb9a00a36a7060f83968154ba95c87da09f7de7b8de2a64c9561cdd77c707a7bd457eb864ca32276976822c9738d66825e4b2c933ebe5b19293a94";
            s[160] = "62d68f62c6c31a6b4823d70e14b6df69d85e932562469a57f81016d714572aeb33722860d9472d95bcf11037029ae844f435af4f80d8d3e32daeddad9c8c1706a11362e1b297f12e5729817fdfe2c56e14ebcfeff64f288701919a996278337eaa020ca4f0499693e3d5ed635a884d16b308f91e43a1d35a65a77731b6aba15c53d9c7cb27e934d50f618bff9ec6c5009df25ff2d63d4a977a7fa08fa9de8dec";
            s[161] = "52aed070d9c9df7f80a864f11359a784e8e69f3be8243c8870cc55b8a3028084e7c6fbeba45e753180902cf79961b8842df8c3397f4998240586b26d3f32d6f407b844f8c876783b36f13d3f426eb7f5634ed36cd9e6858d157f129a93c9b9173972542225ec11d3efda6aa4df27608154b946ee76df14fa38dd8b0ff19e57f24fe406723a67f2bb117f54ffd75513f4d8de3acda796be3226347b062a7e93e8";
            s[162] = "41521f410309aa525e0d24a8fd69d0ed6fd71a85211276ce60f20dda50efe01f042a1e742155cd1e177cdd91036c51e289f051cd9fb3e20dbc3514bff025acb220fdede9cb397c9872e506b54942c440eab63e137b17cbff7b9809a58f2911558a334d1b2c8516cbe70f9c31d521d2f68f7599a936832a4a907a2bf6b3ce956a1d9c21e274b31100837e4bb822c186edc4c5704ac586311b115201afb34ad1a3";
            s[163] = "3ec0710262ae38c3c3c2a05341dff0fe57b28bc9631d59b03307c0eae22754bc4b2fa823fa648ca6dec00839df02f5946323a12f90041b927fe0b68302ed84a4fc7d8032360ab9617c9f802ffbfe2f76ec35446202241d441f333ea97b3a16458547861075db10245eb723d96df6dfb6824b27203c06d7f34bbcc80cdad693c5f10bd825aa55b9026b93c4a849d19e685a65e56c7cd69e6a62efcedc22f29032";
            s[164] = "2698bebac58f462c4e4220862777c895237de5093deb066e722825cf0fc948cb0726be48c01d2023a420e372662585f0e3b0d85794a57f3359c1d99df277498ccdbdb710bf1e9dd4d1cf67ec0120c00751282f3788887e9dfbc0052f97831852f464e487a01e402396c9db16eb4e6e3e5b3fe0bd5ed35885007370c019fc8ff1b4626373e59e39e40e1a826f753d8512dd0ea1b006982412a58abcface5cdd70";
            s[165] = "afa29dd110fd6c1140461eada1f28fef7dc356074b1779fd967e702964b49a8bf76d01b69e0226c4965f8a579a54ce37cae60455512f5c3a67162ad4dff46c5070f8b4fec0f3df2d3701768f12470f1bb7391b75bb30713c553daa1b590739848d1fe725de478b93984fb0902c05e8025cdc27b4ed12fd3a35e5a0943070dac5f070d5c1930a84d24883f2f866b7c10a5eaf760ea8b4b536011ef1f6aa3e1442";
            s[166] = "4b8cdafe7aaf62ac32eccfd683e33b97a1cfd49d3403d05e45f88a6221ba91f4e19420fd6b54afdafcf45c7fe4fdfcbba32a505275b708acc5d314bb6f1dde73504c16494be36bc04170b200cb310ab02af315885fe4fdc7863802702a7bfb2299c1d29c58c9eb081327e7d5213caaf3c95c8b8f3fdb348420e14563eee44a2c42fe5362f7000765ee762f634c00cc5245ad883c8ec86d940961bc799d0b34a7";
            s[167] = "69a64a43f08a01fb5778150f25e401a56a5b7761016fe389bb0836cb58beda6daea8c351ebf995abaabb9647038c526f265958cf7744b2f2034a6e57584cdb4a390097822eac39635fef8eecc037eec892e03584f4ba48446bd3abf4da7d2e2fba5eac5a54185b746b4dc5b22fbdf15e0b1e212730c9205459e77c747d37269c3b6ff4eb4a6547670018227fd669b9d7f2b71fa11e6611b96b89bfa617d834cc";
            s[168] = "8216793155a04b38a6a759447b149f1f70badc37fd0be10c91904c98ff54b7e846f16bb18ddcb94da5ea275f0270d50fec803ba0209ba89748992bdd440f010d5925d74c75fdb5c08d6d5017b27036a15f6c09e6a644734a54abde1fc9e2fdc27d768786e47afa785068ad7ec89cde11bd4e0936784f92ad95bcb2618992425262f1e24aed99dbfc2e7e3fc2409d9107f35fbe0a2734c74b011ffa2def2bbabc";
            s[169] = "ccd01f5c033f0672397c858c9cce261bd38c3d0639cd6c224b87404405deb1d6d37cb48334bdfc6d3337e969e24afb9fdd51eab9cf237ba617d90aeabc7a235f55fbaee1a0e0438f0dc452741283f14857ae33e12842092757b7c9b78a4d17d6ebd53ede39c15a10908f81c0f54430a12da0edc260cb7eece8f1de17280ac4825bee176dcc17342409a86febce108309d1453be483d2a93d41cdd4cb25a74076";
            s[170] = "1cf44e4b017034c6a48b7eb00c1d29ff9175b1848e7b28c92999553ba4047f134023a59fcfb8ec67ec68f8f550f88929e8f1ec08d132662f8cff6bb26c73e91f6137b13341ca46fbc78bb1adfdc146c0ef97e268db9678f99103292587a019eb96032b209f971902295cee4394d0f9ed3dbbbbd31819ce89dca4c79df06b18890dc06b7a18e555b594ca49ac2567dd3effb8cb0e1b4c4b00e8bb3652fb80bd6b";
            s[171] = "34a65910de02e60fd24e7e9d79124d47a22795aa88b237606ede9794d4e202a88f7a30a8364f8cecc26623a1fee2c7b635717d4a1adc4c708c0cf75b862789bdde08a5e83cff3598d3f350e8da8a4524597cd8443a11730f9b25914aa4baf76602f7728d4706f2d27f2188d6148c2ae86eb478bf13d6e88116f6f83d464ba94bfde3ff1a959bf017bf520829f87e70232dd90ec9887d7449eb8c4ac00fe0024d";
            s[172] = "06b11e5c1616cfcf0b12b37260b1c692a925ca660bb201055faef857ea5af91b710b262dafec77bbf56292e4c17cd41998427b9451a83123463669fce5d5bbc7b91682d04af65fbb6dde85e68f6498160f8fbacdceb9efd141de27f17f505695d9a153472524ec9f5ae888da1921c74d620632bd1a46335339c76914283a44842e92e88f9542afc72b105df70775b95a046fdd067c416a6f4e6fb9008e439ce2";
            s[173] = "d40836565695f9c60cb8c14e2f5029d0b424786ddea0db1a4f1257d3b49bdae5e5211f4e366032967735651604913b4a1202387b22f7936373bf8f275d93885c4a5823b11c25ab423252b210d409512eee359c443db4e9ea409559f3d8b4d8f83f4250304407bfa73af3c64c5c2928f7a5d4b72c4c3bc654e54c32ec2c21a4422fe8b16f365278ce0953f3531b01b42ee86c508370a5fc7c30caa70c35209f5b";
            s[174] = "89242509fe2a81d33361f6a5bce205f6a003d5c9cf062b94139a2242842df6adc6227f1ada0d2108b7fc3bb67f7f56481da6db804f17888aac79cc9cc96f4a46299d0e031c14ad3b03938b374d351222fe6913515a1e27978827dfd2b44b08d0c144126b1054cfcd6f9f6f23dd604e588ae5d59d7d34fa196c3631ec51d67db55333e436058300a86da2b41340f06063f970115442c0a92959930d1d678998b6";
            s[175] = "a6105724daff1aee3f1cef70545a6598af5edae58e9442e46115fd9c40ad9665be161868d3cb927fae6821753898cbff472c63034a87d603ccbce573dd60612aab7594ca72eae7e6b9280c157f8e09d421a5e79fa30225556b4694164d372f9ecb929c66e372bc90cb0a0b5ac5e00442aca84575723fb2cb8de354568a11bdffde58df4c631795358fab233cd152f05c533051cd782ed04a6acaecbb112e5df1";
            s[176] = "83081eb3c0ffa6a31464bef8f2162800db954b7c591e6877fd1239e11d34b4cb880f5d6a16f656c7ddac424bb7d070e0d7c06ec7a876e62b1985ff19273e39087455bd45e9f9670db0c53f613bbae387369374e85f9dd3aa59feff07adeeeb48b8ba804e52edb86a1dc775f26f44bf6d98a6c545e06294bb91c3aad7dc9c74bc9fd3d50587c47f655552d8f334f7433b447fcd93346b00cf94d409149179abaa";
            s[177] = "094e8f596d63b23f2e84cc1f497b317b0eb7d16c1b4a84516a7303695fdc7621897582f430a93a2d71ad81c7384cb7048ed2603197322d1db1756d9510ce04d85cabc976ae23c0f9f0c21397eeb15cf1e7ee6cf4a2f0af36eab3de56c2d9ffb82a25e363f7e78636834beee3ae8c31672e014395513b2cacf95ee06abaf8efc0abe7d71f830460ba98d9453f6a19769a3c3a5f7787c63de02734b306b7618c5e";
            s[178] = "fdc6f257d53d89239637b754a7daa4123f8117ca6de1c12711a62b5d84f79a202b5398045bf870bb17a6de293e22ad501ce513978ae6dbd7d815bea46e88b97db2d060867294331e83e609329bf7ec5218e35ff966512fbb0f589c3c99189c1ace8eece35c9e4e71c636169265dea4b8b9dd77a68bb89f3996de8ca7b883c82e412d4c6838b5fbab99c54551be6cd2c9c2bf56230ee7df31473eef108798a186";
            s[179] = "7483922168980b08e93ceca676e7dbcd1fba48dc57feed07e007ba960d355e6ace0e355febf3d6c4bd84743264efbfa6538deb9426f6105fcdae23b97f2d9b3cc04f67dfb0013a609f48876979d7ee1f992d48b491778deda0b9d534c725a24c2da5b9031b210f5d5e0910e655b88cf8320ffd0899cf8837ff8f37812c322204ea6dc6af21bf7b6b8c7c2863e1586c3e9978a632c5dc7e2f9ede4876cd6581fa";
            s[180] = "7087ece83be7f847d6f24aec67a80b21a57760d7cf90ab520a32f8f4cd17b6efdcf3a54da2c7da763e63c616ab2f67cab97d64d68e768e7cfb5f04c76e279a7fbebe87b339579fb0cbf4692672d14de1cf806a6bec7b8c403d77b623b341eb50944784e92cb5d0342f6b693acc79a6568b3c7fc2ee93ea47ab6b30436c843f5462d1e7d155b7f0a30f6cfbe2b7b0036f95e17953a32189bc70fae9b667ebefae";
            s[181] = "b49c94b4ef277fa560681ee9d6592cae31240588f2d72a86ff6a2b334367c9e1a7f8c390cf41f756bd7c90c194e2025d0dc37cc5d6ac46a066590cdead1ade5a950554c0e8367b474bd0d5866e60618927e4d6981c218e7eff8a4a20a71ea331a3ff5cf223af046eb7ae3a6183d0b2345714e41a42c02987444c88a18f04120aec1cc88f1fe0855fcc6497c1fd2cc503efe81064f4b6270aef56dfdea8316099";
            s[182] = "e05aaad657e76e27b295aa656e22f43b47a4db338e95a39825762f39e6e29da46556cb46a76c45d6ca5e61c8d8ff2294fc257aa5f6d44d682c7f51b1bc32d2f6bbfbd6d5ceda5670055e92edd99017d4593e5fc49d1378c774dd4fe272bea67c0dfef85737d507369ad8b609c4ca3e5ba52d1ee5217e4457bfcabd06d8d4b5501e8cb95244a5eafad84fd7b2f87c54fa26e1365f1ffcaaa50fa159fab1e7bfeb";
            s[183] = "b3895f21b709d6618fb326a474c929e111bf0ed3a44e6f35808022c2da657c0bff05860332fece4a9fe1b49188e7c82f05e99f28b16ecce9568b1f746380fcf00aae1f19837753d246a5ac5c5a08d8b4ef85aa4ec8c3b17e024126eb50e81c840a0f93450ca01e1ed5117d817f135f7512ecdf1fe3bc465c51763fc967788b343db8ab16f9927326513a92e096272f86e515a19338e33bf0e010bfe98af332de";
            s[184] = "e7a7defa6acc34edaf6867ee76a1917eade7428120991a3ab66e033fa0f39ef94ca30d8b7a3596d7679ae0bf6249cf31fe661ebc0e00a03055f461b518074dd7b302752152e777e28fe1512ba81a60a7fe8566cac240c3bf6628c758feeb458b73dc7fb08fc6dcf8e3fd036828605a4380bcca584588498b141c327635c10ae8a84a345dc4b92fb1c711ec91205cb259bffd6e5509647717180c8fb8cc518182";
            s[185] = "72bf73883eede4c7afa2db08d5d19c084aff5d215889d78467296d55f46b35955e676a3cdf41862dfebe021c091a48f42c9f4735bc3e2c22ea6f481646195c0f0b98338004ec69420d29d4dc9cf3462e6fa7d22a4461588a6ae3d4003fd5b468dfee34766dc881b88d109bcc038bf86a1f442bd92effe7ae5c3b56de08ea502f93f9d5aba2126690147b77b8f7aa921bdee57835d6a7d55fc875e839e88d2e33";
            s[186] = "7a4252f313e9357485d1a2e0e1417989cb6f2c80c19e2aeee71477f5e0748c3d264362e1ba6bb05d556236879bcbd5da52af725ad1aea29e955217dc8a5b13397647a6d57e273afd3a884175703c59a61956bda54c6bf38f327c29b6359c7a1bd3a5f1bcf3b355ec3a07e29010cb742a462edb9ad2142920f5a8e1e1fe87c533389bb99cd57fd5a73b812bc24034eb62c2e3c17c2b52d0f8d65033c8b0b92e6e";
            s[187] = "3bd94f9b4f7ec90219acb46c1ac6a8b597db73f8945f3f3d88a5b99c72f7d112e39bbe961feb0b635d4e5709c6270ba848a0233cf216ab252dbf129efc1b0af8c8c3cf54bebaa72d5252848155b84d73486bbe6f49d4393a6c22ccf3c103666ebd738f24dc573a69f6f58672d100c5aa57a893eabfb34e4270027dbd9df9b7dd94bbe5ada3f5045776133b9d0cf826dcd1b458ab635cb005493167560cc23a4d";
            s[188] = "c12d6deacc0cb3256e8bb49888cfe3580bb7c397270ff4dec562b6c2a81468c831c303982112e4fab409b3fb51af69e4ef6abbbd1e52f12de44693fcead0b601da48dfa4781839f7c2b43fa11f86f727def889835e10ce34afbc985cc978dbe3eef04d3533b49d2fcbb4d9ad61311a4859cd802f5e6502ec87fe987f13ea54082bb6e5f6d6abadf273be65ac58221d9da10e8c928c8a90627858fb222a672161";
            s[189] = "ac204457694bdbddc21a634ab13743a37f67360df1adb7bb090b5649ea63136dfc5408c3feecfd3b556a428bdafb58cfccc58ab4c96b409708a58e5a8edf944f95a710bcbebf004ef37654d125314b4f447d34ede6936bb1ec51643aa668456d7a440e947deb1eed3355d23678c5feebe0c9bd24514d07d03a9212c33f5aa0c8d8b75e6388204caf4d7264ddda2e062f2ef24dc441fbf9e928559daad5922057";
            s[190] = "6303b8bae1b14c332df030685bc0a0adf32bd8b6f1431fca361cc555ba91b013cd3d07c525bc110760ab36cb47897e777c5c83c6896a3b70d0a4ca8f405ba22554079ef658f976351e334ece622acfa29c03842761c7dad92b03da3e80be471388d955f6cc73261e889086be607567a1ddf9bf4c6813e194157cb928bc597ffcbad2e851eb11d0bda4e8b3806d37d7e5eca24546e2ccf68542d9dad23b9335e2";
            s[191] = "2b05d320464f3fd68e9c8461689a47dad22ca4877d10539062ffb1c22b599586daa9233e66fe12c3bf08527e1ca5ed250a6544c492bee68284559aa29516e83e6849e0dfc569301e77d3beee392ce5a134367cc78ea5283be538e88152be0230de9e7dc3ce05737a7645788cda9202362c5b4c5d792fc20f24f5a232fec877f659c558e2d4639c7630064f8ab91d585170387740910f96fc92a2a188d98d31f6";
            s[192] = "e8ed25e6f2b5cd72277cff0831b34d9980566aa183ec0e171ee3539bcdd70c831154e2c415c0a4dbba166cedb8ff10e677546845f85c081aed78a280fde896eab49ee37f88afb3e85676bcaf4cdb8acdf528ee703dd5ec05d7290ec200bbda77df57251e31ea70c6785c72c9a4b19439fa6f45f1713ba7890d983dedd45e2cf05f07606db3000dd60b69c3363efa71f9a6ee1dd8fdbb15b15257ef8b8754e349";
            s[193] = "e59604ffc914374ae1ef25c09e3eae08bcec726f68b58b1d8b514e26a0f32e091329dd591da4c203d6cdb2b47ed82e608965ccee9f4ae34374bcf6a6f6d982004382287087c322ba96e1a2e178f142a302725a434d03a6e9ae4f9241bc252ee92e7752be3ce483f5b89ce8e455958ed4d1f60eb23724136c04f54f03434925636d6fcf2ca884cb0b91031cf905b9386b3bb623e4b25dc59770b7bff8bc772068";
            s[194] = "c4f4ea4fdd7233ec7d425387cfb2ed5ff71cf31624bac99b18e80d1976968ac7eb62c1fbe008951b781f4ace195573b3ecc4cdcd8f4cb7241d963cab67c030d609cb23f00c71e8f7c885ce8e948d61cba17657e9875e15f80c06168f798646b1e618801123929851a4c7838f97efd8b2418a894d3e7438c3331cc63cd87d1057e6b3f29f40c7a8da7a5a2501718e991c30f4f91fe32a19e6e313603424a56f89";
            s[195] = "f1798850ee36bc07a1c9a95352c7147b9a083c24a0b3c655e419e02358f17a08235d4478815b065565d3c366044d19629d3420074961ee72583908d8d0de84949ee3a7889cbbbee77df74273b97ba5d96710df88816a3a6e180610d2c7c43305939cfcb36486cb994f843fc4acaa467ea34eae07d3170f0d40eb1e5a86ff9d17650844b33086f5e36b70c1383ea59d0a442b6b7bcf7b52f2a2fa91a2a2b0e6b6";
            s[196] = "4cbd2002032ec927bdf62aaef9c2f8490484db405c68f6b65ce06f27fe1253bfec1e66a9aa34e0ff290ced66e0efd026f69082c91e78cb8b5006972d77ea501cc2e969f52069bb97b8fd3ad9cb70dd832c39904837de32195ee6642ac274117eab545e58caacd00b31f5d4603fda91fc249eb5d2268d95e1a323da630be718e5cd7c82722ea347322befcd8cd7a85e180a34487319ae14c105e8db5754c347b3";
            s[197] = "acceaf70e88969fcc1ec9286388b5fb2d086d0d22617186b509b0675193070b773eb057772cd1d573fc29dfa756372c2b3c0e7610f479bf871fbc8f7fbeca8bd31c480abcf18b94ba2c9b2af81687330bda4aa170341c41c6282f58bb420120a6d3ac54e7eefa12cdba25724753f840f4a886817a566ca62e7d195a5766b6ecfc2df80f0401af48020da44c0eb7b387a502f075946707492f125452aab1849b0";
            s[198] = "7a415b762daa914e1e9e29e466d4e780cc67fc0f3e662b2652c0864bb83eb7cbdac4ac6e56c249b2e38228790ae1628d95f16a04832d70941d44a3d39409a942a64e66dc1995605dff569a140724a6ac7faa5247f27d66e026accf9dc6fdc2db8040d2519c6838601a3cb8b10afb4db257dfd9743f53ca09d940b18b9989cd64f174dd8f74a19006ac446b12c03c1df38446b08d05a701f19e1c4169698df731";
            s[199] = "4c251a5a914cab979d07f19c3da38e3ad678c87d17822b1109cb718fc909fb9ce285ffcbf05facd6eb1f348ccfbcc5b3c634946a49213564aaa34408e8d8f1b26dc531d3f56deec0ec5223589964b296a62b0c7eb3785c559447b73a098859d8dfb6a762c808d190f1439048a9993aef5cd637eadf28499239abbe7d9dec6e2399581c427ffe2321e8288ae147174bde7cd811bb3c78751dab36fc0f87092a7e";
            s[200] = "27a9a955ad770f38912dade0a36ff8530fded4bf57aceb950a10afe66bc88b2c5efc7a563ef269d00d90d5b169bf2c6791eff0cf9507160b653d0e19eb050ef90b04c8e21597d44705a6267f05977cd3a0cf2b7d6ed8130e7e80bdf668bb8ed75df11c4ea7def35cfbbedb043dd449c900f3dd213eb5d52ebeece6071589016a56ef0d4bfd8fe621b58fe1dbfafe0658a96cecce18eea088e4606346dce5e7c2";
            s[201] = "95b3a1adbcf8d2987c2f2cba58d5f0a4ef6e0301e186b8d62a59acb6eb4be54867136f319f97470fedae743acd6bb0231ab7d60450e9856989e92c06ec014341af3e69f5e54da348868528df895b56458661d8fdebb3eb4d9aba2b854e897b7ae43cbc1c0f53c959e9ee3d2c3c0e87bed291053da3aaf181a40cc6f25c067be5bb329fdc22068ef11566a08c7918125c6eaffee31147b8004fcba000ceb9802e";
            s[202] = "a6cadd53a2621482b7d66ecc82dc4ea6431bc0191c3801ac6b705df38c7fffe469043e5002096aca4aaca5ef033cc2c5f884d5c339d9a648ffa9400098c7851b9f5990b3771fcf55b196b7c2723085ad11268daadd411967a6a545986c93b86b7f72387bf68dc6ae8dcfd21c7f57ba70b15f3f5517c5585f345ff751c7bf21dc1a33d396ba180a2cb22998fc05ca47b5525a2c150effb15ffd681006c479f0c5";
            s[203] = "06df04188832b10afff94209d2aa1c8a123702de28234dcd3e0a7d36c1aa8449e6fa55e3e1e3d77d8424e87a45e38697755f84c49a99473797268113eb69098888947526035b246d00a630f6201ecc4075d8aa6604de73e2119e264e4c96751f2a67a2e46cf467a0df8f0520bcf4762b2715aba266d9b3f5e8fa67d12f9caac928b07ac3be99f41120655aa77f6433fc264673a92929e792187f87b5fda50cf2";

            return s;
        }

        private static string q10_string()
        {
            string s = "CRIwqt4+szDbqkNY+I0qbNXPg1XLaCM5etQ5Bt9DRFV/xIN2k8Go7jtArLIy";
            s += "P605b071DL8C+FPYSHOXPkMMMFPAKm+Nsu0nCBMQVt9mlluHbVE/yl6VaBCj";
            s += "NuOGvHZ9WYvt51uR/lklZZ0ObqD5UaC1rupZwCEK4pIWf6JQ4pTyPjyiPtKX";
            s += "g54FNQvbVIHeotUG2kHEvHGS/w2Tt4E42xEwVfi29J3yp0O/TcL7aoRZIcJj";
            s += "MV4qxY/uvZLGsjo1/IyhtQp3vY0nSzJjGgaLYXpvRn8TaAcEtH3cqZenBoox";
            s += "BH3MxNjD/TVf3NastEWGnqeGp+0D9bQx/3L0+xTf+k2VjBDrV9HPXNELRgPN";
            s += "0MlNo79p2gEwWjfTbx2KbF6htgsbGgCMZ6/iCshy3R8/abxkl8eK/VfCGfA6";
            s += "bQQkqs91bgsT0RgxXSWzjjvh4eXTSl8xYoMDCGa2opN/b6Q2MdfvW7rEvp5m";
            s += "wJOfQFDtkv4M5cFEO3sjmU9MReRnCpvalG3ark0XC589rm+42jC4/oFWUdwv";
            s += "kzGkSeoabAJdEJCifhvtGosYgvQDARUoNTQAO1+CbnwdKnA/WbQ59S9MU61Q";
            s += "KcYSuk+jK5nAMDot2dPmvxZIeqbB6ax1IH0cdVx7qB/Z2FlJ/U927xGmC/RU";
            s += "FwoXQDRqL05L22wEiF85HKx2XRVB0F7keglwX/kl4gga5rk3YrZ7VbInPpxU";
            s += "zgEaE4+BDoEqbv/rYMuaeOuBIkVchmzXwlpPORwbN0/RUL89xwOJKCQQZM8B";
            s += "1YsYOqeL3HGxKfpFo7kmArXSRKRHToXuBgDq07KS/jxaS1a1Paz/tvYHjLxw";
            s += "Y0Ot3kS+cnBeq/FGSNL/fFV3J2a8eVvydsKat3XZS3WKcNNjY2ZEY1rHgcGL";
            s += "5bhVHs67bxb/IGQleyY+EwLuv5eUwS3wljJkGcWeFhlqxNXQ6NDTzRNlBS0W";
            s += "4CkNiDBMegCcOlPKC2ZLGw2ejgr2utoNfmRtehr+3LAhLMVjLyPSRQ/zDhHj";
            s += "Xu+Kmt4elmTmqLgAUskiOiLYpr0zI7Pb4xsEkcxRFX9rKy5WV7NhJ1lR7BKy";
            s += "alO94jWIL4kJmh4GoUEhO+vDCNtW49PEgQkundV8vmzxKarUHZ0xr4feL1ZJ";
            s += "THinyUs/KUAJAZSAQ1Zx/S4dNj1HuchZzDDm/nE/Y3DeDhhNUwpggmesLDxF";
            s += "tqJJ/BRn8cgwM6/SMFDWUnhkX/t8qJrHphcxBjAmIdIWxDi2d78LA6xhEPUw";
            s += "NdPPhUrJcu5hvhDVXcceZLa+rJEmn4aftHm6/Q06WH7dq4RaaJePP6WHvQDp";
            s += "zZJOIMSEisApfh3QvHqdbiybZdyErz+yXjPXlKWG90kOz6fx+GbvGcHqibb/";
            s += "HUfcDosYA7lY4xY17llY5sibvWM91ohFN5jyDlHtngi7nWQgFcDNfSh77TDT";
            s += "zltUp9NnSJSgNOOwoSSNWadm6+AgbXfQNX6oJFaU4LQiAsRNa7vX/9jRfi65";
            s += "5uvujM4ob199CZVxEls10UI9pIemAQQ8z/3rgQ3eyL+fViyztUPg/2IvxOHv";
            s += "eexE4owH4Fo/bRlhZK0mYIamVxsRADBuBlGqx1b0OuF4AoZZgUM4d8v3iyUu";
            s += "feh0QQqOkvJK/svkYHn3mf4JlUb2MTgtRQNYdZKDRgF3Q0IJaZuMyPWFsSNT";
            s += "YauWjMVqnj0AEDHh6QUMF8bXLM0jGwANP+r4yPdKJNsoZMpuVoUBJYWnDTV+";
            s += "8Ive6ZgBi4EEbPbMLXuqDMpDi4XcLE0UUPJ8VnmO5fAHMQkA64esY2QqldZ+";
            s += "5gEhjigueZjEf0917/X53ZYWJIRiICnmYPoM0GSYJRE0k3ycdlzZzljIGk+P";
            s += "Q7WgeJhthisEBDbgTuppqKNXLbNZZG/VaTdbpW1ylBv0eqamFOmyrTyh1APS";
            s += "Gn37comTI3fmN6/wmVnmV4/FblvVwLuDvGgSCGPOF8i6FVfKvdESs+yr+1AE";
            s += "DJXfp6h0eNEUsM3gXaJCknGhnt3awtg1fSUiwpYfDKZxwpPOYUuer8Wi+VCD";
            s += "sWsUpkMxhhRqOBKaQaBDQG+kVJu6aPFlnSPQQTi1hxLwi0l0Rr38xkr+lHU7";
            s += "ix8LeJVgNsQdtxbovE3i7z3ZcTFY7uJkI9j9E0muDN9x8y/YN25rm6zULYaO";
            s += "jUoP/7FQZsSgxPIUvUiXkEq+FU2h0FqAC7H18cr3Za5x5dpw5nwawMArKoqG";
            s += "9qlhqc34lXV0ZYwULu58EImFIS8+kITFuu7jOeSXbBgbhx8zGPqavRXeiu0t";
            s += "bJd0gWs+YgMLzXtQIbQuVZENMxJSZB4aw5lPA4vr1fFBsiU4unjOEo/XAgwr";
            s += "Tc0w0UndJFPvXRr3Ir5rFoIEOdRo+6os5DSlk82SBnUjwbje7BWsxWMkVhYO";
            s += "6bOGUm4VxcKWXu2jU66TxQVIHy7WHktMjioVlWJdZC5Hq0g1LHg1nWSmjPY2";
            s += "c/odZqN+dBBC51dCt4oi5UKmKtU5gjZsRSTcTlfhGUd6DY4Tp3CZhHjQRH4l";
            s += "Zhg0bF/ooPTxIjLKK4r0+yR0lyRjqIYEY27HJMhZDXFDxBQQ1UkUIhAvXacD";
            s += "WB2pb3YyeSQjt8j/WSbQY6TzdLq8SreZiuMWcXmQk4EH3xu8bPsHlcvRI+B3";
            s += "gxKeLnwrVJqVLkf3m2cSGnWQhSLGbnAtgQPA6z7u3gGbBmRtP0KnAHWSK7q6";
            s += "onMoYTH+b5iFjCiVRqzUBVzRRKjAL4rcL2nYeV6Ec3PlnboRzJwZIjD6i7WC";
            s += "dcxERr4WVOjOBX4fhhKUiVvlmlcu8CkIiSnZENHZCpI41ypoVqVarHpqh2aP";
            s += "/PS624yfxx2N3C2ci7VIuH3DcSYcaTXEKhz/PRLJXkRgVlWxn7QuaJJzDvpB";
            s += "oFndoRu1+XCsup/AtkLidsSXMFTo/2Ka739+BgYDuRt1mE9EyuYyCMoxO/27";
            s += "sn1QWMMd1jtcv8Ze42MaM4y/PhAMp2RfCoVZALUS2K7XrOLl3s9LDFOdSrfD";
            s += "8GeMciBbfLGoXDvv5Oqq0S/OvjdID94UMcadpnSNsist/kcJJV0wtRGfALG2";
            s += "+UKYzEj/2TOiN75UlRvA5XgwfqajOvmIIXybbdhxpjnSB04X3iY82TNSYTmL";
            s += "LAzZlX2vmV9IKRRimZ2SpzNpvLKeB8lDhIyGzGXdiynQjFMNcVjZlmWHsH7e";
            s += "ItAKWmCwNkeuAfFwir4TTGrgG1pMje7XA7kMT821cYbLSiPAwtlC0wm77F0T";
            s += "a7jdMrLjMO29+1958CEzWPdzdfqKzlfBzsba0+dS6mcW/YTHaB4bDyXechZB";
            s += "k/35fUg+4geMj6PBTqLNNWXBX93dFC7fNyda+Lt9cVJnlhIi/61fr0KzxOeX";
            s += "NKgePKOC3Rz+fWw7Bm58FlYTgRgN63yFWSKl4sMfzihaQq0R8NMQIOjzuMl3";
            s += "Ie5ozSa+y9g4z52RRc69l4n4qzf0aErV/BEe7FrzRyWh4PkDj5wy5ECaRbfO";
            s += "7rbs1EHlshFvXfGlLdEfP2kKpT9U32NKZ4h+Gr9ymqZ6isb1KfNov1rw0KSq";
            s += "YNP+EyWCyLRJ3EcOYdvVwVb+vIiyzxnRdugB3vNzaNljHG5ypEJQaTLphIQn";
            s += "lP02xcBpMNJN69bijVtnASN/TLV5ocYvtnWPTBKu3OyOkcflMaHCEUgHPW0f";
            s += "mGfld4i9Tu35zrKvTDzfxkJX7+KJ72d/V+ksNKWvwn/wvMOZsa2EEOfdCidm";
            s += "oql027IS5XvSHynQtvFmw0HTk9UXt8HdVNTqcdy/jUFmXpXNP2Wvn8PrU2Dh";
            s += "kkIzWhQ5Rxd/vnM2QQr9Cxa2J9GXEV3kGDiZV90+PCDSVGY4VgF8y7GedI1h";
            return s;
        }

    }
}