using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PP_lab2
{
    [Serializable]
    class Message
    {
        int sourceId;
        int messageType;

        public Task task;

        public Message(int sourceId, int messageType)
        {
            this.SourceId = sourceId;
            this.MessageType = messageType;
        }

        public int SourceId { get => sourceId; set => sourceId = value; }
        public int MessageType { get => messageType; set => messageType = value; }
    }
}
