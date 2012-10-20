using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using EmailMigratorLib;
using ActiveUp.Net.Mail;

namespace GoDaddyMigrator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            string [] usernameanddomain = email.Text.Split(new Char[] {'@'});
            if (usernameanddomain.Count() != 2)
            {
                MessageBox.Show("Invalid email");
                return;
            }

            GoDaddy t = new GoDaddy(usernameanddomain[0], usernameanddomain[1], password.Text);
            if(!t.Login())
                return;

            List<string> folders = t.GetFolders();
            
            Imap4Client imap4 = new Imap4Client();

            imap4.Connect(imapserver.Text);
            imap4.Login("eric@roosp2.com", password.Text);       
            
            foreach(string folder in folders)
            {
                switch (folder)
                {
                    case "Trash":
                    case "Bulk Mail":
                    case "Drafts":
                    case "Templates":
                    case "Send Later":
                        continue;
                }

                try
                {
                    imap4.CreateMailbox(folder);                    
                }
                catch (ActiveUp.Net.Mail.Imap4Exception a)
                {

                }

                Mailbox box = imap4.SelectMailbox(folder);

                List<int> messages = t.GetMessageUids(folder);

                foreach (int uid in messages)
                {
                    // if(!CopiedMessage(folder, uid))

                    byte[] messagecontent = t.GetMessage(folder, uid);

                    if (messagecontent.Length > 0)                    
                        box.Append(messagecontent);

                    // AddMessage(folder, uid);
                }
            }
            imap4.Disconnect();
        }
    }
}
