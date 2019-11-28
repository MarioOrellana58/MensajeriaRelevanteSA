using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using MensajesRelevantesSA.Models;
namespace MensajesRelevantesSA.Repository
{
    public class MessagesLogic
    {
        public string Create(string senderReceptor, string textMessage, HttpPostedFileBase file)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:53273/api/Texts");
                
                var message = new MessageModel()
                {
                    SenderReceptor = senderReceptor,
                    Message = textMessage.Length!=0 ? textMessage:string.Empty,
                    UploadedFile = file                     
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
            else
            {
                loggedUserReceivedMessages = loggedUserReceivedMessages.OrderBy(message=> message.SentDate).ToList();
                return loggedUserReceivedMessages;
            }
        }
    }
}