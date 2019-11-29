using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using MensajesRelevantesSA.Models;
using AlternativeProcesses;
using System.Numerics;
using AlternativeProcesses.CompressComponents;
namespace MensajesRelevantesSA.Repository
{
    public class MessagesLogic
    {
        private int p = 11;
        private int g = 3;

        public string Create(string senderReceptor, string textMessage, HttpPostedFileBase file)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:53273/api/Texts");
                if (textMessage.Equals(string.Empty) && file == null)
                {
                    return "No puede enviar mensajes vacios";
                }
                if (textMessage != string.Empty)
                {
                    var cipher = new SDES();
                    var getUser = new UsersLogic();

                    var receptor = senderReceptor.Split('|');
                   
                    var a = getUser.getUserByUsername(receptor[1]).PrivateKey;
                    var receptorKey = (Math.Pow(g,a)%p) ;
                    
                    var key = BigInteger.ModPow((int)receptorKey,SessionUserNode.getInstance.PrivateKey,p);
                    var cipherKey = Convert.ToString(((int)key),2);

                     textMessage =  cipher.CipherText(textMessage, cipherKey);
                }
                else
                {
                    textMessage = string.Empty;
                }
                var compressFile = new CompresssDecompressActions();
                var message = new MessageModel()
                {
                    SenderReceptor = senderReceptor,
                    Message = textMessage,
                    UploadedFile = file!=null ? compressFile.generateCharactersList(file):"",
                    PublicKey = (int)(Math.Pow(g, SessionUserNode.getInstance.PrivateKey)%p)
                };

                var postTask = client.PostAsJsonAsync("Messages", message);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return "200";
                }
                else
                {
                    if ((int)result.StatusCode >= 400 && (int)result.StatusCode < 500)
                    {
                        return result.StatusCode.ToString() + ". Revise los datos ingresados :D";
                    }
                    else
                    {
                        return result.StatusCode.ToString() + ". Contacte a un desarrollador del sistema D:";
                    }
                }
            }
        }
        public List<MessageModel> getMessages(string SenderReceptor)
        {
            using (var client = new HttpClient())
            {
                var searchedMessages = getAllUserMessages(SenderReceptor);

                if (searchedMessages.GetType().ToString() == "System.String")
                {
                    return null;
                }

                var decipheredMessages = new List<MessageModel>();
                    var decipherMessage = new SDES();
                    var getUser = new UsersLogic();
                                    var receptor = SenderReceptor.Split('|');
                if (receptor[1] == string.Empty)
                {
                    return searchedMessages;
                }
                if (receptor[0] == SessionUserNode.getInstance.Username)
                {//es un mensaje que yo envié
                    var a = getUser.getUserByUsername(receptor[1]).PrivateKey;
                    foreach (var message in searchedMessages)
                    {                    
                        var key = BigInteger.ModPow((int)message.PublicKey,a,p);
                        var commonKey = Convert.ToString((int)key,2);
                        message.Message = message.Message != string.Empty ? decipherMessage.DecipherText(message.Message, commonKey) : string.Empty;
                        decipheredMessages.Add(message);                    
                    }           
                }
                else
                {//es un mensaje recibido
                    foreach (var message in searchedMessages)
                    {                    
                        var key = BigInteger.ModPow((int)message.PublicKey,SessionUserNode.getInstance.PrivateKey,p);
                        var commonKey = Convert.ToString((int)key,2);
                        message.Message = message.Message != string.Empty ? decipherMessage.DecipherText(message.Message, commonKey) : string.Empty;
                        decipheredMessages.Add(message);                    
                    }  
                }
                         
                     

                return searchedMessages;
            }
        }
        dynamic getAllUserMessages(string SenderReceptor)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:53273/api/Texts");
                List<MessageModel> searchedMessages = null;
                var responseTask = client.GetAsync("Messages/" + SenderReceptor);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<MessageModel>>();
                    readTask.Wait();
                    searchedMessages = readTask.Result;
                    return searchedMessages;
                }
                else
                {
                    searchedMessages = null;
                    if ((int)result.StatusCode == 404)
                    {
                        return "404 no encontrado. No tiene ningún mensaje con este usuario";
                    }
                    else
                    {
                        return result.StatusCode.ToString() + ". Contacte a un desarrollador del sistema D:";
                    }
                }
            }
        }

        public List<string> getAllContacts(string emitter)
        {
            using (var client = new HttpClient())
            {
                var searchedMessages = getAllUserMessages(emitter);

                if (searchedMessages.GetType().ToString() == "System.String")
                {
                    return null;
                }

                var contactFriends = new List<string>();
                var IHaveAllFriends = false;
                while (!IHaveAllFriends)
                {
                    if (searchedMessages.Count == 0)
                    {
                        IHaveAllFriends = true;
                    }
                    else
                    {
                        var actualMessage = searchedMessages[searchedMessages.Count - 1];
                        var emitterReceptor = actualMessage.SenderReceptor.Split('|');
                        var newFriend = string.Empty;
                        for (int i = 0; i < 2; i++)
                        {
                            if (emitterReceptor[i] != emitter)
                            {
                                newFriend = emitterReceptor[i];
                                break;
                            }
                        }
                        searchedMessages.Remove(actualMessage);
                        if (!contactFriends.Contains(newFriend))
                        {
                            contactFriends.Add(newFriend);
                        }
                    }
                }
                return contactFriends;
            }
        }

        public List<MessageModel> loadMessages(string receptor)
        {
            var loggedUser = SessionUserNode.getInstance.Username;
            
            var loggedUserSentMessages = getMessages(loggedUser + '|' +  receptor);
            var loggedUserReceivedMessages = getMessages(receptor + '|' +  loggedUser);
            if (loggedUserSentMessages != null && loggedUserReceivedMessages != null)
            {
                var conversation = new List<MessageModel>();
                conversation = loggedUserSentMessages.Union(loggedUserReceivedMessages).ToList();
                conversation = conversation.OrderBy(message=> message.SentDate).ToList();
                return conversation;
            }
            else if (loggedUserSentMessages != null)
            {
                loggedUserSentMessages = loggedUserSentMessages.OrderBy(message=> message.SentDate).ToList();
                return loggedUserSentMessages;
            }
            else if (loggedUserReceivedMessages != null)
            {
                loggedUserReceivedMessages = loggedUserReceivedMessages.OrderBy(message=> message.SentDate).ToList();
                return loggedUserReceivedMessages;
            }
            else
            {
                return null;
            }
        }
        public string DecompressSelectedFile(string fileData)
        {
            var decompression = new CompresssDecompressActions();
            decompression.DescomprimirArchivo(fileData);
            return "";
        }
    }
}