using Machine.Specifications;

namespace One.Settix.Consul.Tests
{
    public class When_using_case_sensitive_keys
    {
        Establish context = async () =>
        {
            consul = ConsulForSettixFactory.Create();
            await consul.SetAsync("key1".CreateSettixRawKey(), "value1").ConfigureAwait(false);
            await consul.SetAsync("Key2".CreateSettixRawKey(), "value2").ConfigureAwait(false);
        };

        Because of = async () =>
        {
            valueFromConsul1 = await consul.GetAsync("Key1".CreateSettixRawKey()).ConfigureAwait(false);
            valueFromConsul2 = await consul.GetAsync("key2".CreateSettixRawKey()).ConfigureAwait(false);
        };

        It should_get_value1 = () => valueFromConsul1.ShouldEqual("value1");

        It should_get_value2 = () => valueFromConsul2.ShouldEqual("value2");

        static string valueFromConsul1;
        static string valueFromConsul2;
        static ConsulForSettix consul;
    }
}
