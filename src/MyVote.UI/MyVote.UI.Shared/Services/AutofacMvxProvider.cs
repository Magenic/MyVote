using System;
using System.Linq;

using Autofac;
using Autofac.Core;

using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.IoC;

using MyVote.UI;

public class AutofacMvxProvider : MvxSingleton<IMvxIoCProvider>, IMvxIoCProvider
{
    public AutofacMvxProvider()
    {
        AutofacInject.ResolveAutofacDependencies(this);
    }

    public void CallbackWhenRegistered(Type type, Action action)
    {
        AutofacInject.Container.ComponentRegistry.Registered += (sender, args) =>
        {
            if (args.ComponentRegistration.Services.OfType<TypedService>().Any(x => x.ServiceType == type))
            {
                action();
            }
        };
    }

    public void CallbackWhenRegistered<T>(Action action)
    {
        CallbackWhenRegistered(typeof(T), action);
    }

    public bool CanResolve(Type type)
    {
        return AutofacInject.Container.IsRegistered(type);
    }

    public bool CanResolve<T>() where T : class
    {
        return AutofacInject.Container.IsRegistered<T>();
    }

    public object Create(Type type)
    {
        return Resolve(type);
    }

    public T Create<T>() where T : class
    {
        return Resolve<T>();
    }

    public object GetSingleton(Type type)
    {
        return Resolve(type);
    }

    public T GetSingleton<T>() where T : class
    {
        return Resolve<T>();
    }

    public object IoCConstruct(Type type)
    {
        return Resolve(type);
    }

    public T IoCConstruct<T>() where T : class
    {
        return Resolve<T>();
    }

    public void RegisterSingleton(Type tInterface, Func<object> theConstructor)
    {
        var builder = new ContainerBuilder();
        builder.Register(x => theConstructor()).As(tInterface).AsSelf().SingleInstance();
        builder.Update(AutofacInject.Container);
    }

    public void RegisterSingleton<TInterface>(Func<TInterface> theConstructor) where TInterface : class
    {
        var builder = new ContainerBuilder();
        builder.Register(x => theConstructor()).As<TInterface>().AsSelf().SingleInstance();
        builder.Update(AutofacInject.Container);
    }

    public void RegisterSingleton(Type tInterface, object theObject)
    {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(theObject).As(tInterface).AsSelf().SingleInstance();
        builder.Update(AutofacInject.Container);
    }

    public void RegisterSingleton<TInterface>(TInterface theObject) where TInterface : class
    {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(theObject).As<TInterface>().AsSelf().SingleInstance();
        builder.Update(AutofacInject.Container);
    }

    public void RegisterType(Type tFrom, Type tTo)
    {
        var builder = new ContainerBuilder();
        builder.RegisterType(tTo).As(tFrom).AsSelf();
        builder.Update(AutofacInject.Container);

    }

    public void RegisterType(Type t, Func<object> constructor)
    {
        var builder = new ContainerBuilder();
        builder.Register(x => constructor()).As(t).AsSelf();
        builder.Update(AutofacInject.Container);
    }

    public void RegisterType<TInterface>(Func<TInterface> constructor) where TInterface : class
    {
        var builder = new ContainerBuilder();
        builder.Register(x => constructor()).As<TInterface>().AsSelf().SingleInstance();
        builder.Update(AutofacInject.Container);
    }

    public void RegisterType<TFrom, TTo>()
        where TFrom : class
        where TTo : class, TFrom
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<TTo>().As<TFrom>().AsSelf();
        builder.Update(AutofacInject.Container);
    }

    public object Resolve(Type type)
    {
            return AutofacInject.Container.Resolve(type);
    }

    public T Resolve<T>() where T : class
    {
        return AutofacInject.Container.Resolve<T>();
    }

    public bool TryResolve(Type type, out object resolved)
    {
        return AutofacInject.Container.TryResolve(type, out resolved);
    }

    public bool TryResolve<T>(out T resolved) where T : class
    {
        return AutofacInject.Container.TryResolve<T>(out resolved);
    }
}