using Machine.Specifications;

namespace One.Settix.Consul.Tests
{
    public class When_key_value_exist
    {
        Establish context = async () =>
        {
            consul = ConsulForSettixFactory.Create();

            //fist delete keys to be sure that didn't exists
            await consul.DeleteAsync("key1".CreateSettixRawKey()).ConfigureAwait(false);
            await consul.DeleteAsync("key2".CreateSettixRawKey()).ConfigureAwait(false);

            await consul.SetAsync("key1".CreateSettixRawKey(), "value1").ConfigureAwait(false);
        };

        Because of = async () =>
        {
            key1Exist = await consul.ExistsAsync("key1".CreateSettixRawKey()).ConfigureAwait(false);
            key2Exist = await consul.ExistsAsync("key2".CreateSettixRawKey()).ConfigureAwait(false);
        };

        It should_be_true = () => key1Exist.ShouldBeTrue();

        It should_be_false = () => key2Exist.ShouldBeFalse();

        static bool key1Exist;
        static bool key2Exist;
        static ConsulForSettix consul;
    }
}
