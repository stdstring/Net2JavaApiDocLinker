using System;
using System.Drawing;
using System.Windows.Forms;
using UserDocsApiResolver.Library.ApiMemberStorage;
using log4net.Config;

namespace UserDocsApiResolver
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            ApiMemberStorageConfig config = new ApiMemberStorageConfigReader().Read();
            ApiMemberStorageManager storageManager = ApiMemberStorageManager.Create(config);
            using (KeyboardHook keyboardHook = new KeyboardHook(new UserRequestProcessor(storageManager)))
            {
                NotifyIcon trayIcon = new NotifyIcon();
                trayIcon.Icon = new Icon("app.ico");
                trayIcon.Text = "UserDocsApiResolver";
                trayIcon.Visible = true;
                MenuItem setHookMenuItem = new MenuItem("Set hook");
                setHookMenuItem.Enabled = true;
                MenuItem unsetHookMenuItem = new MenuItem("Unset hook");
                unsetHookMenuItem.Enabled = false;
                setHookMenuItem.Click += (sender, e) =>
                                             {
                                                 setHookMenuItem.Enabled = false;
                                                 unsetHookMenuItem.Enabled = true;
                                                 keyboardHook.SetHook();
                                             };
                unsetHookMenuItem.Click += (sender, e) =>
                                               {
                                                   setHookMenuItem.Enabled = true;
                                                   unsetHookMenuItem.Enabled = false;
                                                   keyboardHook.UnsetHook();
                                               };
                MenuItem separatorMenuItem = new MenuItem("-");
                MenuItem exitMenuItem = new MenuItem("Exit", (sender, e) => Application.Exit());
                ContextMenu contextMenu = new ContextMenu(new[]
                                                              {
                                                                  setHookMenuItem,
                                                                  unsetHookMenuItem,
                                                                  separatorMenuItem,
                                                                  exitMenuItem
                                                              });
                trayIcon.ContextMenu = contextMenu;
                Application.Run();
            }
        }
    }
}
