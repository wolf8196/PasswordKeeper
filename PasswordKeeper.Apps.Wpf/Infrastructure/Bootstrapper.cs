using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Abstractions.Adapters;
using PasswordKeeper.Apps.Wpf.Abstractions.Services;
using PasswordKeeper.Apps.Wpf.Adapters;
using PasswordKeeper.Apps.Wpf.Features;
using PasswordKeeper.Apps.Wpf.Services;
using PasswordKeeper.Security;
using PasswordKeeper.Services;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.CryptoProviders;
using PasswordKeeper.Services.Entities;
using PasswordKeeper.Services.Models;
using PasswordKeeper.Utils;
using SimpleInjector;

namespace PasswordKeeper.Apps.Wpf.Infrastructure
{
    public static class Bootstrapper
    {
        private static readonly Container Container;

        static Bootstrapper()
        {
            Container = new Container();
        }

        public static Container RegisterBindings()
        {
            if (Properties.Settings.Default.IsDesktop)
            {
                Container.Register<ISettings, DesktopSettings>(Lifestyle.Singleton);
            }
            else
            {
                Container.Register<ISettings, PortableSettings>(Lifestyle.Singleton);
            }

            Container.Register<IRandomNumberGenerator, RandomNumberGeneratorAdapter>(Lifestyle.Singleton);

            Container.Register<ISerializer<User>, UserSerializer>(Lifestyle.Singleton);
            Container.Register<IDatabaseProvider, DatabaseProvider>(Lifestyle.Singleton);
            Container.Register<IUserManager, UserManager>(Lifestyle.Singleton);
            Container.Register<ILocker, FileLocker>(Lifestyle.Singleton);

            Container.Register<IDatabaseService, DatabaseService>(Lifestyle.Singleton);
            Container.Register<IUserService, UserService>(Lifestyle.Singleton);
            Container.Register<IAttachmentService, AttachmentService>(Lifestyle.Singleton);
            Container.Register<IGenerator<string, PasswordGenerationParameters>, PasswordGenerationService>(Lifestyle.Singleton);

            Container.Register<IMessenger, Messenger>(Lifestyle.Singleton);

            Container.Register<ICommandFactory, DelegateCommandFactory>(Lifestyle.Singleton);
            Container.Register<IViewModelFactory>(() => new SimpleInjectorViewModelFactory(Container), Lifestyle.Singleton);
            Container.Register<IPropertyHolder, PropertyHolder>(Lifestyle.Singleton);

            Container.Register<IFolderBrowserDialogAdapter, FolderBrowserDialogAdapter>(Lifestyle.Singleton);
            Container.Register<IWindowStorage<ViewModelBase>, WindowStorage<ViewModelBase>>(Lifestyle.Singleton);

            Container.Register<IFileProcessor, FileProcessor>(Lifestyle.Transient);
            Container.Register<IStartupService, WindowsStartupService>(Lifestyle.Singleton);

            return Container;
        }
    }
}