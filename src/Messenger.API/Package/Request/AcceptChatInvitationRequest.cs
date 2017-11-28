using System;
using Contracts;

namespace Messenger.API.Package.Request
{
    public sealed class AcceptChatInvitationRequest : PackageRequest
    {
        public string Account { get; private set; }
        public string Ticket { get; private set; }
        public int SessionID { get; private set; }

        public AcceptChatInvitationRequest(string account, string ticket, int sessionID)
        {
            account.NotEmpty();
            ticket.NotEmpty();

            this.Account = account;
            this.Ticket = ticket;
            this.SessionID = sessionID;

        }

        public override string ToString()
        {
            return string.Format("ANS {0} {1} {2} {3}", base.TransactionID, this.Account, this.Ticket, this.SessionID);
        }

    }
}