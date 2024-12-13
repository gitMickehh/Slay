using System;

namespace Techno
{
    public static class ServiceLocator<TService>
    {
        private static TService s_Instance;

        public static TService Service
        {
            get => s_Instance;
            set
            {
                if (HasService)
                {
                    throw new InvalidOperationException(
                        $"Cannot set new value for {nameof(Service)} when one is already set."
                    );
                }
                else
                {
                    s_Instance = value;
                }
            }
        }

        public static bool HasService => Service != null;

        public static bool TryGetService(out TService service)
        {
            service = Service;
            return service != null;
        }

        public static void Reset()
        {
            s_Instance = default;
        }
    }
}
