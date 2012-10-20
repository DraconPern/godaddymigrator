using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using EmailMigratorLib;

namespace GoDaddyMigratorWebService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://godaddymigrator.frontmotion.com/webservice/2008-02-21/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class GoDaddyMigrator : System.Web.Services.WebService
    {
        [WebMethod]
        public string Ping(string key)
        {
            return "Pong(key=" + key + ")";
        }

        [WebMethod]
        public void Migrate(string username, string domain, string password)
        {
            // add the migration to the queue
            EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.AccountsTableAdapter ata = new EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.AccountsTableAdapter();
            EmailMigratorLib.GoDaddyMigratorDataSet ds = new GoDaddyMigratorDataSet();
            ata.FillByLoginInfo(ds.Accounts, username, domain);             

            GoDaddyMigratorDataSet.AccountsRow account = null;
            if (ds.Accounts.Count >= 1)
            {
                account = ds.Accounts[0];
            }
            else
            {                
                account = ds.Accounts.AddAccountsRow(username, domain, password);
                ata.Update(ds.Accounts);
            }
            
            EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.QueueTableAdapter qta = new EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.QueueTableAdapter();
            var queue = ds.Queue.AddQueueRow(account, true);
            qta.Update(ds.Queue);
        }

        [WebMethod]
        public bool ValidateGoDaddyAccount(string user, string domain, string password)
        {
            // do a quick validation test.
            GoDaddy g = new GoDaddy(user, domain, password);
            return g.Login();
        }


    }
}
