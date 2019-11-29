using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AlternativeProcesses.CompressComponents
{
    public class Huffman
    {
        private NodeComponents root;
        private Dictionary<byte, string> prefixDictionary = new Dictionary<byte, string>();
        public Dictionary<string, byte> decompressPrefixDictionary = new Dictionary<string, byte>();


        public Dictionary<byte, string> GetDictionary()
        {
            return prefixDictionary;
        }

        public void HuffmanAssembler(List<NodeComponents> charactersList)
        {

            while (charactersList.Count > 1)
            {
                var newNode = new NodeComponents();
                newNode = NodeAssembler(charactersList[0], charactersList[1]);
                charactersList.RemoveAt(0);
                charactersList.RemoveAt(0);

                var newPossitionHuffmanNode = charactersList.FindIndex(x => x.CharacterData.probabilidad >= newNode.CharacterData.probabilidad);
                if (newPossitionHuffmanNode == -1)
                {
                    charactersList.Add(newNode);
                }
                else
                {
                    charactersList.Insert(newPossitionHuffmanNode, newNode);
                }

            }
            root = charactersList[0];
            charactersList.Clear();
            charactersList = null;

        }

        NodeComponents NodeAssembler(NodeComponents lowNode, NodeComponents maxNode)
        {

            var father = new NodeComponents();
            var infoNewFather = new initialReadComponents();
            father.CharacterData = infoNewFather;
            father.LeftSon = lowNode;
            father.RigthSon = maxNode;
            father.CharacterData.probabilidad = lowNode.CharacterData.probabilidad + maxNode.CharacterData.probabilidad;
            return father;
        }

        public Dictionary<byte, string> fillPrefixDictionary()
        {
            PrefixCodeAssignment(root, "");
            return prefixDictionary;
        }

        void PrefixCodeAssignment(NodeComponents actualNode, string prefixCode)
        {
            if (actualNode.RigthSon != null && actualNode.LeftSon != null)
            {
                PrefixCodeAssignment(actualNode.LeftSon, prefixCode + "0");
                PrefixCodeAssignment(actualNode.RigthSon, prefixCode + "1");
            }
            else
            {
                actualNode.CharacterData.codigoPrefijo = prefixCode;
                prefixDictionary.Add(actualNode.CharacterData.caracter, actualNode.CharacterData.codigoPrefijo);
            }

        }

        public byte[] compressBuffer(byte[] decompressBuffer, int bufferLenght, ref int compressBufferPosition, ref int originalBitsAmount)
        {
            //juntar 8 bits
            originalBitsAmount = 0;
            var compressBuffer = new byte[bufferLenght];


            var _8BitsString = string.Empty;
            compressBufferPosition = 0;

            for (int i = 0; i < decompressBuffer.Length && i < bufferLenght; i++)
            {
                _8BitsString += prefixDictionary[decompressBuffer[i]];
                if (_8BitsString.Length == 8)//si se llenan 8 bytes, guarda
                {
                    compressBuffer[compressBufferPosition] = Convert.ToByte(binaryToDecimal(_8BitsString));
                    originalBitsAmount += 8;
                    compressBufferPosition++;
                    _8BitsString = string.Empty;
                }
                else if (_8BitsString.Length > 8)//si se pasó, guarda una cadena de 8 bytes y deja los restantes 
                {
                    var auxString = string.Empty;//almacena el prefijo a guardar y los sobrantes temporalmente
                    for (int j = 0; j < 8; j++)
                    {
                        auxString += _8BitsString[j];
                    }
                    compressBuffer[compressBufferPosition] = Convert.ToByte(binaryToDecimal(auxString));
                    originalBitsAmount += 8;
                    compressBufferPosition++;
                    auxString = string.Empty;
                    for (int j = 8; j < _8BitsString.Length; j++)
                    {
                        auxString += _8BitsString[j];
                    }
                    _8BitsString = auxString;
                    if (i == decompressBuffer.Length - 1)//si no se ha llegado a 8 y se está en la última iteración, añade ceros al principio
                    {
                        originalBitsAmount += auxString.Length;
                        auxString = string.Empty;
                        for (int j = 0; j < _8BitsString.Length; j++)
                        {
                            if (auxString.Length == 8)
                            {                                
                                 compressBuffer[compressBufferPosition] = Convert.ToByte(binaryToDecimal(auxString));
                                 auxString = string.Empty;
                                auxString += _8BitsString[j];
                            }
                            else
                            {
                                auxString += _8BitsString[j];
                            }
                        }
                        if (auxString.Length < 8)
                        {
                            compressBuffer[compressBufferPosition] = Convert.ToByte(binaryToDecimal(auxString.PadRight(8, '0')));
                        }
                        else if(auxString.Length == 8)
                        {
                            compressBuffer[compressBufferPosition] = Convert.ToByte(binaryToDecimal(auxString));
                        }

                    }

                }
                else if (i == decompressBuffer.Length - 1)//si no se ha llegado a 8 y se está en la última iteración, añade ceros al principio
                {
                    var auxString = _8BitsString;
                    _8BitsString = string.Empty;
                    originalBitsAmount += auxString.Length;
                    for (int j = 0; j < 8 - auxString.Length; j++)
                    {
                        _8BitsString += "0";
                    }
                    _8BitsString = auxString + _8BitsString;
                    compressBuffer[compressBufferPosition] = Convert.ToByte(binaryToDecimal(auxString));
                    compressBufferPosition++;
                }
            }

            var bufferToWrite = new byte[compressBufferPosition + 1];
            for (int i = 0; i < compressBufferPosition + 1; i++)
            {
                bufferToWrite[i] = compressBuffer[i];
            }
            return bufferToWrite;
        }
        int binaryToDecimal(string prefixCode)
        {
            var prefixCodeArray = prefixCode.ToCharArray();

            Array.Reverse(prefixCodeArray);
            var Decimal = 0;

            for (int i = 0; i < prefixCodeArray.Length; i++)
            {
                if (prefixCodeArray[i] == '1')
                {
                    Decimal += (int)Math.Pow(2, i);
                }
            }
            return Decimal;
        }


    }
}
