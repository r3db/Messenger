using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;

namespace Messenger.API.IO
{
    internal sealed class MessageBuffer
    {
        private readonly SortedDictionary<int, string> buffer = new SortedDictionary<int, string>();
        private readonly object locker = new object();

        // Done!
        public void Store(string message, int id)
        {
            // Todo: Remove
            message.NotEmpty();

            // Message carries enough information!
            if (message.Length > 4) return;

            // Message does not carry enough information, so we store it until it makes sense!
            // Ex: "VER ", "911 ", "imd>" 
            // We put it in a buffer for later processing!
            lock (this.locker)
            {
                if (this.buffer.ContainsKey(id) == false)
                {
                    this.buffer.Add(id, message);
                    Console.WriteLine("--> Add");
                }
                else
                {
                    throw new NotSupportedException("This should never happen");
                }
            }
        }

        public void Store(int id, ref string message)
        {
            // Todo: Remove
            message.NotEmpty();

            lock (this.locker)
            {
                if (this.buffer.ContainsKey(id) == false)
                {
                    this.buffer.Add(id, message);
                    this.DefragmentMessage(ref message);
                }
                else
                {
                    throw new NotSupportedException("This should never happen");
                }
            }
        }

        private void DefragmentMessage(ref string message)
        {
            if (this.buffer.Count <= 1)
            {
                return;
            }

            #region Check Fragmentation

            //Verificar se as temos numa sequencia que seja NÃO fragmentada!
            int[] keys = this.buffer.Keys.ToArray();
            for (int i = 0; i < keys.Length - 1; ++i)
            {
                if (keys[i + 1] - keys[i] > 1)
                {
                    return;
                }
            }

            #endregion

            //Se chegamos até este ponto, isso significa que conseguimos desframentar uma mensagem!
            //Embora não tenhamos garantias que a mensagem se encontra completa!
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, string> kvp in this.buffer)
            {
                sb.Append(kvp.Value);
            }
            this.buffer.Clear();
            message = sb.ToString();

        }

        public bool Defragmenting(int id, ref string message, ref MessageContainer result, int payload, Action<int, string> action)
        {
            int contentLength = Encoding.UTF8.GetBytes(message.Substring(message.IndexOf("\r\n") + 2)).Length;

            if (payload > contentLength)
            {
                //Message incomplete!
                //Vamos mandar para o buffer e efectuar o 'parse' mais tarde!
                int length = message.Length;
                this.Store(id, ref message);
                if (length != message.Length)
                {
                    action(id, message);
                    //this.ParseMessage(id, message);
                }
                return true;
            }
            return false;
        }

    }
}
