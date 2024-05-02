using Climatics.Weather;
using Worlding;

namespace Stories
{
    public class EnvPredefinedPreconditions
    {
        public World World { get; }

        public EnvPredefinedPreconditions(World world)
        {
            World = world;
        }

        public bool IsState(string state) =>
            World.State.CurrentState == state;

        public bool IsHour(int hour) =>
            World.Time.Hour == hour;

        public bool IsWeather(WeatherKind kind) =>
            World.Time.Weather == kind;

        public bool IsDaylight() =>
            World.Time.IsLight;
    }
}
