using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeProcesses
{
    public class CesarCipherAlgorithm
    {
        private Dictionary<char, char> LowercaseDictionary = new Dictionary<char, char>();
        private Dictionary<char, char> UppercaseDictionary = new Dictionary<char, char>();
        string UppercaseAlphabet = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ0123456789";
        string LowercaseAlphabet = "abcdefghijklmnñopqrstuvwxyz";

        public void DictionaryAsembler(string keyWord)
        {
            keyWord = keyWord.ToUpper();
            var alphabetCurrentPosition = 0;
            for (int i = 0; i < UppercaseAlphabet.Length; i++)
            {
                if (i < keyWord.Length)
                {
                    UppercaseDictionary.Add(UppercaseAlphabet[i], keyWord[i]);
                }
                else
                {
                    var IFoundIt = false;
                    while (!IFoundIt)
                    {
                        if (!UppercaseDictionary.ContainsValue(UppercaseAlphabet[alphabetCurrentPosition]))
                        {
                            UppercaseDictionary.Add(UppercaseAlphabet[i], UppercaseAlphabet[alphabetCurrentPosition]);
                            alphabetCurrentPosition++;
                            IFoundIt = true;
                        }
                        else
                        {
                            alphabetCurrentPosition++;
                        }
                    }

                }
            }

            keyWord = keyWord.ToLower();

            alphabetCurrentPosition = 0;
            for (int i = 0; i < LowercaseAlphabet.Length; i++)
            {
                if (i < keyWord.Length)
                {
                    LowercaseDictionary.Add(LowercaseAlphabet[i], keyWord[i]);
                }
                else
                {
                    var IFoundIt = false;
                    while (!IFoundIt)
                    {
                        if (!LowercaseDictionary.ContainsValue(LowercaseAlphabet[alphabetCurrentPosition]))
                        {
                            LowercaseDictionary.Add(LowercaseAlphabet[i], LowercaseAlphabet[alphabetCurrentPosition]);
                            alphabetCurrentPosition++;
                            IFoundIt = true;
                        }
                        else
                        {
                            alphabetCurrentPosition++;
                        }
                    }

                }
            }
        }

        public string CipherCesar(string keyData, string keyToCipher)
        {
            var cipheredKey = string.Empty;
            foreach (var item in keyData)
            {
                var IsInTheDictionary = UppercaseDictionary.TryGetValue(item, out char llega);
                if (IsInTheDictionary)
                {
                    cipheredKey += UppercaseDictionary[item];
                }
                else
                {
                    IsInTheDictionary = LowercaseDictionary.TryGetValue(item, out char llegas);
                    if (IsInTheDictionary)
                    {
                        cipheredKey += LowercaseDictionary[item];
                    }
                    else
                    {
                        cipheredKey += item;
                    }
                }
            }
            return cipheredKey;
        }

        public string DecipherCesar(string cipheredKey, string keyToDecipher)
        {
            var originalKey = string.Empty;

            UppercaseDictionary.Clear();
            LowercaseDictionary.Clear();
            DictionaryAsembler(keyToDecipher);

            foreach (var item in cipheredKey)
            {
                var IsInTheDictionary = UppercaseDictionary.ContainsValue(item);
                if (IsInTheDictionary)
                {
                    var valorTextoDecifrado = UppercaseDictionary.FirstOrDefault(x => x.Value == item);
                    originalKey += valorTextoDecifrado.Key;
                }
                else
                {
                    IsInTheDictionary = LowercaseDictionary.ContainsValue(item);
                    if (IsInTheDictionary)
                    {
                        var valorTextoDecifrado = LowercaseDictionary.FirstOrDefault(x => x.Value == item);
                        originalKey += valorTextoDecifrado.Key;
                    }
                    else
                    {
                        originalKey += item;
                    }
                }
            }
            return originalKey;
        }
    }
}
