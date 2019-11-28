using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AlternativeProcesses.CompressComponents
{
    class CompresssDecompressActions
    {
        private Huffman HuffmanAlgorithm = new Huffman();
        public List<initialReadComponents> generateCharactersList(HttpPostedFileBase file)
        {
            var bufferLength = file.ContentLength;

            var amountOfCharacters = 0;
            List<initialReadComponents> charactersList = new List<initialReadComponents>();
            Stream stream = file.InputStream;

            //se hace una copia del archivo
            var ms = new MemoryStream();
            file.InputStream.CopyTo(ms);
            file.InputStream.Position = ms.Position = 0;


            using (var reader = new BinaryReader(stream))
            {
                amountOfCharacters = Convert.ToInt32(reader.BaseStream.Length);
                var byteBurffer = new byte[bufferLength];
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    byteBurffer = reader.ReadBytes(bufferLength);
                    for (int i = 0; i < byteBurffer.Length; i++)
                    {

                        var characterForList = new initialReadComponents();
                        characterForList = charactersList.Find(x => x.caracter == byteBurffer[i]);
                        if (characterForList == null)
                        {
                            characterForList = new initialReadComponents();
                            characterForList.caracter = byteBurffer[i];
                            characterForList.frecuencia = 1;
                            charactersList.Add(characterForList);
                        }
                        else
                        {
                            characterForList.frecuencia++;
                        }
                    }

                }
            }



            for (int i = 0; i < charactersList.Count; i++)
            {
                charactersList[i].probabilidad = Convert.ToDouble(charactersList[i].frecuencia) / Convert.ToDouble(amountOfCharacters);
            }
            ComprimirArchivo(charactersList, ms, Path.GetFileNameWithoutExtension(file.FileName), bufferLength);
            return charactersList;
        }

        public void ComprimirArchivo(List<initialReadComponents> listaDeCaracteres, Stream archivo, string nombreArchivo, int buffLength)
        {
            listaDeCaracteres.Sort((comp1, comp2) => comp1.probabilidad.CompareTo(comp2.probabilidad));
            var nodosParaHuffman = new List<NodeComponents>();

            for (int i = 0; i < listaDeCaracteres.Count; i++)
            {
                var nodoTransicion = new NodeComponents();
                nodoTransicion.CharacterData = listaDeCaracteres[i];
                nodosParaHuffman.Add(nodoTransicion);
            }
            
            HuffmanAlgorithm.HuffmanAssembler(nodosParaHuffman);
            HuffmanAlgorithm.fillPrefixDictionary();
            var bufferLength = Convert.ToInt64(archivo.Length);


            var arch = nombreArchivo + ".huff";
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedFiles"), arch);

            using (var reader = new BinaryReader(archivo))
            {
                using (var writeStream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(writeStream))
                    {
                        var byteBuffer = new byte[bufferLength];
                        //meto el diccionario de prefijos al buffer y lo escribo 
                        foreach (var item in HuffmanAlgorithm.GetDictionary())
                        {
                            writer.Write($"{item.Key}|{item.Value}|");
                        }

                        writer.Write($"|");
                        int cantBitsOriginales = 0;
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            var myIntValue = unchecked((int)bufferLength);
                            int tamañoBufferNuevo = 0;
                            byteBuffer = HuffmanAlgorithm.compressBuffer(reader.ReadBytes(myIntValue), myIntValue, ref tamañoBufferNuevo, ref cantBitsOriginales);
                            tamañoBufferNuevo++;
                            writer.Write($"|{cantBitsOriginales}|");
                            for (int i = 0; i < byteBuffer.Length; i++)
                            {
                                writer.Write(byteBuffer[i]);

                            }
                        }
                    }
                }
            }

        }


        public void DescomprimirArchivo(string nombre)
        {
            var nombre2 = string.Empty;
            for (int i = 0; i < nombre.Length; i++)
            {
                if (nombre[i] == '.')
                {
                    break;
                }
                else
                {
                    nombre2 += nombre[i];
                }

            }
            var arch = nombre2 + ".huff";
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedFiles"), arch);
            var path2 = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedFiles"), nombre);
            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var writeStream = new FileStream(path2, FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(writeStream))
                        {
                            var esDiccionario = false;
                            var bufferLength = Convert.ToInt64(stream.Length);
                            var diccionarioDePrefijosDescomp = new Dictionary<string, byte>();
                            var cantBitsOriginales = 0;
                            while (esDiccionario == false)
                            {
                                var lectorPosActual = string.Empty;
                                lectorPosActual = reader.ReadString();
                                if (lectorPosActual[0] != '|')
                                {

                                    var ContenidoDiccionario = lectorPosActual.Split('|');
                                    diccionarioDePrefijosDescomp.Add(ContenidoDiccionario[1], Convert.ToByte((ContenidoDiccionario[0])));

                                }
                                else
                                {
                                    lectorPosActual = reader.ReadString();
                                    var ContenidoDiccionario = lectorPosActual.Split('|');
                                    cantBitsOriginales = Convert.ToInt32(ContenidoDiccionario[1]);
                                    esDiccionario = true;
                                }
                            }
                            var byteBuffer = new byte[bufferLength];
                            //Aca comienza el texto comprimido que debe descomprimirse
                            int cantLeidos = 0;
                            var cadenaIteracionAnterior = string.Empty;
                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                var cadenaAux = string.Empty;
                                var myIntValue = unchecked((int)bufferLength);
                                byteBuffer = reader.ReadBytes(myIntValue);
                                for (int i = 0; i < byteBuffer.Length; i++)
                                {//Recorrer posicion por posicion viendo que tiene la cadena
                                    cadenaAux = string.Empty;

                                    cadenaAux = decimalABinario(byteBuffer[i].ToString());
                                    if (cadenaIteracionAnterior != "")
                                    {

                                        if (cadenaAux.Length < 8)
                                        {
                                            cadenaAux = cadenaAux.PadLeft(8, '0');
                                            cadenaAux = cadenaIteracionAnterior + cadenaAux;
                                            cadenaIteracionAnterior = string.Empty;
                                            /*ya la tengo de 8 bits entonces tengo que recorrerla para ver si mi diccionario
                                            la contiene*/
                                            var contadorPosicionActualAux = 1;
                                            while (contadorPosicionActualAux <= cadenaAux.Length)
                                            {
                                                var contenidoAux = string.Empty;
                                                contenidoAux = cadenaAux.Substring(0, contadorPosicionActualAux);
                                                if (diccionarioDePrefijosDescomp.ContainsKey(contenidoAux))
                                                {
                                                    cantLeidos += contenidoAux.Length;
                                                    writer.Write(diccionarioDePrefijosDescomp[contenidoAux]);
                                                    if (cantLeidos == cantBitsOriginales)
                                                    {
                                                        break;
                                                    }
                                                    cadenaAux = cadenaAux.Substring(contadorPosicionActualAux, cadenaAux.Length - contadorPosicionActualAux);
                                                    contadorPosicionActualAux = 1;
                                                }
                                                else
                                                {

                                                    contadorPosicionActualAux++;
                                                }
                                            }
                                            if (cadenaAux != "")
                                            {
                                                /*quiere decir que de lo restante no hay ningun valor en el diccionario
                                                 que lo contenga, por lo que debo buscar en mi siguiente caracter*/
                                                cadenaIteracionAnterior = cadenaAux;

                                            }
                                        }
                                        else
                                        {
                                            var contadorPosicionActualAux = 1;

                                            cadenaAux = cadenaIteracionAnterior + cadenaAux;
                                            cadenaIteracionAnterior = "";
                                            while (contadorPosicionActualAux <= cadenaAux.Length)
                                            {
                                                var contenidoAux = "";
                                                contenidoAux = cadenaAux.Substring(0, contadorPosicionActualAux);
                                                if (diccionarioDePrefijosDescomp.ContainsKey(contenidoAux))
                                                {
                                                    cantLeidos += contenidoAux.Length;
                                                    writer.Write(diccionarioDePrefijosDescomp[contenidoAux]);
                                                    if (cantLeidos == cantBitsOriginales)
                                                    {
                                                        break;
                                                    }
                                                    cadenaAux = cadenaAux.Substring(contadorPosicionActualAux, cadenaAux.Length - contadorPosicionActualAux);
                                                    contadorPosicionActualAux = 1;
                                                }
                                                else
                                                {

                                                    contadorPosicionActualAux++;
                                                }
                                            }
                                            if (cadenaAux != "")
                                            {
                                                /*quiere decir que de lo restante no hay ningun valor en el diccionario
                                                 que lo contenga, por lo que debo buscar en mi siguiente caracter*/
                                                cadenaIteracionAnterior = cadenaAux;

                                            }
                                        }

                                    }
                                    else
                                    {//no quedo nada de la iteracion anterior
                                        if (cadenaAux.Length < 8)
                                        {
                                            cadenaAux = cadenaAux.PadLeft(8, '0');
                                            /*ya la tengo de 8 bits entonces tengo que recorrerla para ver si mi diccionario
                                            la contiene*/
                                            var contadorPosicionActualAux = 1;
                                            while (contadorPosicionActualAux <= cadenaAux.Length)
                                            {
                                                var contenidoAux = string.Empty;
                                                contenidoAux = cadenaAux.Substring(0, contadorPosicionActualAux);
                                                if (diccionarioDePrefijosDescomp.ContainsKey(contenidoAux))
                                                {
                                                    cantLeidos += contenidoAux.Length;
                                                    writer.Write(diccionarioDePrefijosDescomp[contenidoAux]);
                                                    if (cantLeidos == cantBitsOriginales)
                                                    {
                                                        break;
                                                    }
                                                    cadenaAux = cadenaAux.Substring(contadorPosicionActualAux, cadenaAux.Length - contadorPosicionActualAux);
                                                    contadorPosicionActualAux = 1;
                                                }
                                                else
                                                {

                                                    contadorPosicionActualAux++;
                                                }
                                            }
                                            if (cadenaAux != "")
                                            {
                                                /*quiere decir que de lo restante no hay ningun valor en el diccionario
                                                 que lo contenga, por lo que debo buscar en mi siguiente caracter*/
                                                cadenaIteracionAnterior = cadenaAux;

                                            }
                                        }
                                        else
                                        {
                                            var contadorPosicionActualAux = 1;
                                            while (contadorPosicionActualAux <= cadenaAux.Length)
                                            {
                                                var contenidoAux = string.Empty;
                                                contenidoAux = cadenaAux.Substring(0, contadorPosicionActualAux);
                                                if (diccionarioDePrefijosDescomp.ContainsKey(contenidoAux))
                                                {
                                                    cantLeidos += contenidoAux.Length;
                                                    writer.Write(diccionarioDePrefijosDescomp[contenidoAux]);
                                                    if (cantLeidos == cantBitsOriginales)
                                                    {
                                                        break;
                                                    }
                                                    cadenaAux = cadenaAux.Substring(contadorPosicionActualAux, cadenaAux.Length - contadorPosicionActualAux);
                                                    contadorPosicionActualAux = 1;
                                                }
                                                else
                                                {

                                                    contadorPosicionActualAux++;
                                                }
                                            }
                                            if (cadenaAux != "")
                                            {
                                                /*quiere decir que de lo restante no hay ningun valor en el diccionario
                                                 que lo contenga, por lo que debo buscar en mi siguiente caracter*/
                                                cadenaIteracionAnterior = cadenaAux;

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            string decimalABinario(string Decimal)
            {
                var codPrefijo = "";
                var numero = Convert.ToInt32(Decimal);
                while (numero > 0)
                {
                    if (numero % 2 == 0)
                    {
                        codPrefijo = "0" + codPrefijo;
                    }
                    else
                    {
                        codPrefijo = "1" + codPrefijo;
                    }
                    numero = (numero / 2);
                }

                return codPrefijo;
            }
        }

    }
}
