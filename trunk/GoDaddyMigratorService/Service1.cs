using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Security.Permissions;
using EmailMigratorLib;
using ActiveUp.Net.Mail;

namespace GoDaddyMigratorService
{
    public partial class Service1 : ServiceBase
    {

        ManualResetEvent pause = new ManualResetEvent(false);
        public ManualResetEvent stop = new ManualResetEvent(false);

        SqlDependency sqldep;

        public Service1()
        {
            InitializeComponent();

            CanPauseAndContinue = true;
        }

        protected override void OnStart(string[] args)
        {
            Thread t = new Thread(new ThreadStart(DatabaseQueueChecker));
            t.Start();
        }

        protected override void OnStop()
        {
            stop.Set();
        }

        protected override void OnPause()
        {
            pause.Set();
        }

        protected override void OnContinue()
        {
            pause.Reset();
        }

        public void DatabaseQueueChecker()
        {

            while (!stop.WaitOne(0, false))
            {
                bool sleep = true;

                if (!pause.WaitOne(0, false))
                {
                    EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.QueueTableAdapter qta = new EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.QueueTableAdapter();
                    GoDaddyMigratorDataSet.QueueDataTable queue = qta.GetDataByQueued();

                    if (queue.Count > 0)
                    {
                        if (qta.OwnQueueItem(queue[0].queueid) == 1)
                        {
                            //processss
                            Migrate(queue[0].accountid);

                            // queue[0].accountid;

                            sleep = false;
                        }
                        
                    }                    

                }
                
                if(sleep)
                    Thread.Sleep(200);                
            }
        }

        public void Migrate(int accountid)
        {
            EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.AccountsTableAdapter ata = new EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.AccountsTableAdapter();
            EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.FoldersTableAdapter fta = new EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.FoldersTableAdapter();                               
            EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.MessagesTableAdapter mta = new EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters.MessagesTableAdapter();
            EmailMigratorLib.GoDaddyMigratorDataSet ds = new GoDaddyMigratorDataSet();

            ata.FillByAccountID(ds.Accounts, accountid);
            if(ds.Accounts.Count != 1)
                return;
            
            fta.FillByAccountID(ds.Folders, accountid);

            GoDaddy godaddy = new GoDaddy(ds.Accounts[0].username, ds.Accounts[0].domain, ds.Accounts[0].password);
            if (!godaddy.Login())
                return;

            List<string> folders = godaddy.GetFolders();

            Imap4Client imap4 = new Imap4Client();

            imap4.Connect("haruhi.roosp.com");
            imap4.Login(ds.Accounts[0].username + "@" + ds.Accounts[0].domain, ds.Accounts[0].password);

            foreach (string folder in folders)
            {
                string targetfolder = folder;
                switch (folder)
                {
                    case "INBOX.Trash":
                    case "INBOX.Bulk Mail":
                    case "INBOX.Drafts":
                    case "INBOX.Email_Templates":
                    case "INBOX.Send_Later":
                        continue;
                    case "INBOX.Sent_Items":
                        targetfolder = "INBOX.Sent Items";
                        break;
                }

                GoDaddyMigratorDataSet.FoldersRow f = ds.Folders.FindByName(folder);
                if(f == null)
                {
                    f = ds.Folders.AddFoldersRow(ds.Accounts[0], folder);
                    fta.Update(ds.Folders);
                }

                try
                {
                    imap4.CreateMailbox(targetfolder);
                }
                catch (ActiveUp.Net.Mail.Imap4Exception)
                {
                }

                Mailbox box = imap4.SelectMailbox(targetfolder);

                List<int> messages = godaddy.GetMessageUids(folder);
                mta.FillByFolderID(ds.Messages, f.folderid);

                foreach (int uid in messages)
                {
                    if (ds.Messages.FindByfolderiduid(f.folderid, uid) == null)
                    {
                        byte[] messagecontent = godaddy.GetMessage(folder, uid);

                        if (messagecontent.Length > 0)
                            box.Append(messagecontent);

                        mta.Update(ds.Messages.AddMessagesRow(f, uid));
                    }                    
                }
            }
            imap4.Disconnect();
        }
    }
}
