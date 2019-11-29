using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeProcesses
{
    public class SDES
    {
        private string P10, P8, IP, EP, P4, IPInv;
        
        public void LoadPermutation()
        {
            P10 = "2416390875";
            P8 = "52637498";
            IP = "15203746";
            EP = "30121230";
            P4 = "1320";
            IPInv = string.Empty;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var valIP = Convert.ToInt32(Convert.ToString(IP[j]));
                    if (valIP == i)
                    {
                        IPInv += j;
                        break;
                    }
                }
            }
        }

        public string CipherText(string inputString, string key)
        {
            key = fillKey(key);
            LoadPermutation();
            key = fillKey(key);
            var valoresDeK = GenerateK1AndK2(key);
            var byteBuffer = new byte[Encoding.ASCII.GetBytes(inputString).Length];
            byteBuffer = Encoding.Default.GetBytes(inputString);
            var outputText = string.Empty;
            for (int i = 0; i < byteBuffer.Length; i++)
            {
                var decipheredByte = Convert.ToString(byteBuffer[i], 2).PadLeft(8, '0');
                var cipheredByte = CipherByte(valoresDeK[0], valoresDeK[1], decipheredByte, string.Empty, true);
                var asciiCipheredByte = Convert.ToInt16(cipheredByte, 2);
                outputText += Convert.ToString(Convert.ToByte(asciiCipheredByte)) + "|";
            }
            
            return outputText;
        }
        public string DecipherText(string inputString, string key)
        {
            key = fillKey(key);
            LoadPermutation();
            key = fillKey(key);
            var valoresDeK = GenerateK1AndK2(key);
            inputString = inputString.Substring(0, inputString.Length - 1);
            var byteBuffer = inputString.Split('|');
            var outputByteBuffer = new byte[byteBuffer.Length];
            for (int i = 0; i < byteBuffer.Length; i++)
            {
                var decipheredByte = Convert.ToString(Convert.ToInt32(byteBuffer[i]), 2).PadLeft(8, '0');
                var cipheredByte = CipherByte(valoresDeK[1], valoresDeK[0], decipheredByte, string.Empty, true);
                var asciiCipheredByte = Convert.ToInt16(cipheredByte, 2);
                outputByteBuffer[i] = Convert.ToByte(asciiCipheredByte);
            }

            return Encoding.Default.GetString(outputByteBuffer);
        }

        public string fillKey(string binaryKey)
        {
            return binaryKey.PadLeft(10, '0');
        }

        public string[] GenerateK1AndK2(string binaryKey)
        {
            var kValues = new string[2];
            binaryKey = Permutate10(binaryKey);//P10
            binaryKey = LeftShift1(binaryKey.Substring(0, 5)) + LeftShift1(binaryKey.Substring(5, 5));//LS1
            kValues[0] = Permutate8(binaryKey);//K1
            binaryKey = LeftShift2(binaryKey.Substring(0, 5)) + LeftShift2(binaryKey.Substring(5, 5));//LS2
            kValues[1] = Permutate8(binaryKey);//K1
            return kValues;
        }

        string CipherByte(string K1, string K2, string plainText, string iniPer, bool firstEntrance)
        {

            if (firstEntrance)
            {
                iniPer = InititalPermutation(plainText);
                plainText = iniPer;
            }
            else iniPer = plainText;

            plainText = ExpandAndPermutate(plainText.Substring(4, 4));

            plainText = !firstEntrance ? XOr(plainText, K2) : XOr(plainText, K1);

            plainText = SBoxes(plainText, string.Empty);

            plainText = Permutate4(plainText);

            plainText = XOr(plainText, iniPer.Substring(0, 4));

            plainText += iniPer.Substring(4, 4);

            if (!firstEntrance) return InverseInitialPermutation(plainText);
            else return CipherByte(K1, K2, Swap(plainText), iniPer, false);

        }

        string Permutate10(string binaryKey)
        {
            var permutatedKey = new char[10];

            for (int i = 0; i < 10; i++)
            {
                permutatedKey[i] = binaryKey[Convert.ToInt16(Convert.ToString(P10[i]))];
            }

            binaryKey = new string(permutatedKey);

            return binaryKey;
        }

        string Permutate8(string LS1)
        {
            var LS1Permutated = new char[8];

            for (int i = 0; i < 8; i++)
            {
                LS1Permutated[i] = LS1[Convert.ToInt16(Convert.ToString(P8[i]))];
            }

            LS1 = new string(LS1Permutated);

            return LS1;
        }

        string Permutate4(string outputSBox)
        {
            var outputSBoxPermutated = new char[4];

            for (int i = 0; i < 4; i++)
            {
                outputSBoxPermutated[i] = outputSBox[Convert.ToInt16(Convert.ToString(P4[i]))];
            }

            outputSBox = new string(outputSBoxPermutated);

            return outputSBox;
        }

        string InititalPermutation(string initialByte)
        {
            var initialBytePermutated = new char[8];

            for (int i = 0; i < 8; i++)
            {
                initialBytePermutated[i] = initialByte[Convert.ToInt16(Convert.ToString(IP[i]))];
            }

            initialByte = new string(initialBytePermutated);

            return initialByte;
        }

        string ExpandAndPermutate(string initialBytePermutated)
        {
            var ExpPerInitialByte = new char[8];

            for (int i = 0; i < 8; i++)
            {
                ExpPerInitialByte[i] = initialBytePermutated[Convert.ToInt16(Convert.ToString(EP[i]))];
            }

            initialBytePermutated = new string(ExpPerInitialByte);

            return initialBytePermutated;
        }

        string InverseInitialPermutation(string cipheredByte)
        {
            var sortedCipheredByte = new char[8];

            for (int i = 0; i < 8; i++)
            {
                sortedCipheredByte[i] = cipheredByte[Convert.ToInt16(Convert.ToString(IPInv[i]))];
            }

            cipheredByte = new string(sortedCipheredByte);

            return cipheredByte;
        }

        string LeftShift1(string p10Key)
        {
            var LS1 = string.Empty;
            LS1 += p10Key.Substring(1, p10Key.Length - 1);
            LS1 += p10Key.Substring(0, 1);
            return Convert.ToString(LS1);
        }

        string LeftShift2(string LS1)
        {
            var LS2 = string.Empty;
            LS2 += LS1.Substring(2, LS1.Length - 2);
            LS2 += LS1.Substring(0, 2);
            return Convert.ToString(LS2);
        }

        string SBoxes(string inputString, string output)
        {
            var row = inputString.Substring(0, 1) + inputString.Substring(3, 1);
            var column = inputString.Substring(1, 1) + inputString.Substring(2, 1);

            if (output.Equals(string.Empty))
            {
                var SBox0 = new string[,]{
                                    { "01", "00", "11", "10" },
                                    { "11", "10", "01", "00" },
                                    { "00", "10", "01", "11" },
                                    { "11", "01", "11", "10" },
                                  };
                output += SBox0[Convert.ToInt32(row, 2), Convert.ToInt32(column, 2)];
                return SBoxes(inputString.Substring(4, 4), output);//se manda el segundo bloque y la salida
            }
            else
            {
                var SBox1 = new string[,]{
                                    { "00", "01", "10", "11" },
                                    { "10", "00", "01", "11" },
                                    { "11", "00", "01", "00" },
                                    { "10", "01", "00", "11" },
                                  };
                output += SBox1[Convert.ToInt32(row, 2), Convert.ToInt32(column, 2)];
                return output;
            }
        }

        string XOr(string inputString1, string inputString2)
        {
            var ComparedInputString = string.Empty;
            for (int i = 0; i < inputString1.Length; i++)
            {
                ComparedInputString += inputString1[i].Equals(inputString2[i]) ? "0" : "1";
            }
            return ComparedInputString;
        }

        string Swap(string inputString)
        {
            return (inputString.Substring(inputString.Length / 2, inputString.Length / 2) + inputString.Substring(0, inputString.Length / 2));
        }
    }
}
