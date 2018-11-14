using Autofac;
using EasyNote.Integration.EasyNoteAPI.Gateway;

namespace EasyNote.Integration.EasyNoteAPI.IoC
{
    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<APIGateway>().As<IAPIGateway>();
            builder.RegisterType<EasyNoteService>().As<IEasyNoteService>().SingleInstance();
        }
    }
}
