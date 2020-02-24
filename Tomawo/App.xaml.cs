using System;
using System.Collections.Generic;
using Microsoft.Shell;

namespace Tomawo
{
    public partial class App : ISingleInstanceApp
    {
        private const string Unique = "UniqueTomawoApp";

        [STAThread]
        public static void Main()
        {
            if (!SingleInstance<App>.InitializeAsFirstInstance(Unique)) return;

            App application = new App();
            application.InitializeComponent();
            application.Run();
                
            SingleInstance<App>.Cleanup();
        }
        
        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }

        #endregion
    }
}
