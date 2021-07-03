using System.Collections;
using System.Collections.Generic;

namespace Okane.Broker.Commands
{
    public class LoginCommand : Command
    {
        public override string command { get; set; } = "login";
        public IDictionary arguments { get; set; }

        public LoginCommand(string userId, string password, string appName)
        {
            arguments = new Dictionary<string, string>()
            {
                { "userId", userId },
                { "password", password },
                { "appName", appName }
            };
        }
    }
}
