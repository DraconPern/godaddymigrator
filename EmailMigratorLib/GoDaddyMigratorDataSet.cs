namespace EmailMigratorLib {
    
   
    public partial class GoDaddyMigratorDataSet {
        public static string GetConnectionString()
        {            
            return EmailMigratorLib.Properties.Settings.Default.GodaddyMigratorConnectionString;
        }


        public partial class FoldersDataTable
        {
            public GoDaddyMigratorDataSet.FoldersRow FindByName(string name)
            {
                foreach (FoldersRow f in this.Rows)
                {
                    if(f.name == name)
                        return f;
                }

                return null;                
            }

        }

    }


    

}

namespace EmailMigratorLib.GoDaddyMigratorDataSetTableAdapters
{
    
    
    public partial class AccountsTableAdapter {
    }
}
