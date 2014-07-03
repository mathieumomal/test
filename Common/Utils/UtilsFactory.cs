using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Utils.WcfMailService;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Utils
{
    public class UtilsFactory
    {
        private static UnityContainer _container;

        /// <summary>
        /// Public reference to the unity container which will 
        /// allow the ability to register instrances or take 
        /// other actions on the container.
        /// </summary>
        public static UnityContainer Container
        {
            get
            {
                if (_container == null)
                    _container = new UnityContainer();
                return _container;
            }
            private set
            {
                _container = value;
            }
        }

        /// <summary>
        /// Static constructor for DependencyFactory which will 
        /// initialize the unity container.
        /// </summary>
        static UtilsFactory()
        {
            SetDefaultDependencies();

        }

        /// <summary>
        /// Resolves the type parameter T to an instance of the appropriate type.
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        public static T Resolve<T>()
        {
            T ret = default(T);

            if (Container.IsRegistered(typeof(T)))
            {
                ret = Container.Resolve<T>();
            }

            return ret;
        }

        public static void SetDefaultDependencies()
        {
            //Map of relation between Interfaces and classes
            Container.RegisterType<IMailManager, MailManager>(new TransientLifetimeManager());
            Container.RegisterType<ISendMail, WcfMailService.SendMailClient>(new TransientLifetimeManager());
            Container.RegisterType<WcfMailService.SendMailClient>(new InjectionConstructor());
        }

        public static bool NullBooleanCheck(bool? boo, bool defaultValue)
        {
            if (!boo.HasValue)
            {
                boo = defaultValue;
            }
            return (bool)boo;
        }
    }
}
