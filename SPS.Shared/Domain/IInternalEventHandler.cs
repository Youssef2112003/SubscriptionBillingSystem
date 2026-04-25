namespace SPS.Shared.Domain
{
    public interface IInternalEventHandler
    {
        void Handle(object @event);
    }
}
