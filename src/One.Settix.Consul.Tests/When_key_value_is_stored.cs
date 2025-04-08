using Machine.Specifications;

namespace One.Settix.Consul.Tests
{
    public class When_key_value_is_stored
    {
        Establish context = async () =>
        {
            consul = ConsulForSettixFactory.Create();
            await consul.SetAsync("key".CreateSettixRawKey(), "value").ConfigureAwait(false);
        };

        Because of = async () => valueFromConsul = await consul.GetAsync("key".CreateSettixRawKey()).ConfigureAwait(false);

        It should_get_value = () => valueFromConsul.ShouldEqual("value");

        static string valueFromConsul;
        static ConsulForSettix consul;
    }
}
