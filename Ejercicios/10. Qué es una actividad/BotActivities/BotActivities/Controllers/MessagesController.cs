﻿using System.Net;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading;

namespace BotActivities
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                IConversationUpdateActivity updatedStatus = message;
                ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                if (updatedStatus.MembersAdded != null && updatedStatus.MembersAdded.Any())
                {
                    foreach (var item in updatedStatus.MembersAdded)
                    {
                        if (item.Id != message.Recipient.Id)
                        {
                            Activity isTyping = message.CreateReply();
                            isTyping.Type = ActivityTypes.Typing;
                            connector.Conversations.ReplyToActivityAsync(isTyping);
                            Thread.Sleep(3000);
                            Activity reply = message.CreateReply("Hola nuevo usuario, bienvenido!");
                            connector.Conversations.ReplyToActivityAsync(reply);
                        }   
                    }  
                }
                
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {

            }

            return null;
        }
    }
}