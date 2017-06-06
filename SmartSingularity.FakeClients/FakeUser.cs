using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSingularity.FakeClients
{
    public static class FakeUser
    {
        private static string[] _users = new string[] {"David","Jérémy","Sylvain","Ghislaine","Sylvie","Hélène","Thomas","Steven","Isabelle","Fabienne","Chistophe","Monique","Yohann","Patrick",
        "Karine","Maria","Alexandra","Alexandre","Fabrice","Éric","Laurent","Florence","Vincent","Antoine","Michel","François","Thierry","Lorita","Alan","Céline","Henri","Bruno","Julien",
        "Johnny","Adeline","Agnès","Luc","Frédéric","Patricia","Anne","Angeline"};

        public static string GetRandomUserName()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);

            return _users[rnd.Next(0, _users.Length - 1)];
        }
    }
}
