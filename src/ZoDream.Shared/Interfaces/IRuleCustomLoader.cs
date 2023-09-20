namespace ZoDream.Shared.Interfaces
{
    public interface IRuleCustomLoader<T>
    {
        public void Ready(T loader);

        public void Destroy(T loader);
    }
}
