using Machine.Specifications;
using System.Collections.Generic;
using System.Linq;

namespace One.Settix.Consul.Tests
{
    public class When_get_all_values
    {
        Establish context = async () =>
        {
            consul = ConsulForSettixFactory.Create();
            await consul.SetAsync("key".CreateSettixRawKey(), "value").ConfigureAwait(false);
            await consul.SetAsync("key1".CreateSettixRawKey(), "value1").ConfigureAwait(false);
            await consul.SetAsync("Key2".CreateSettixRawKey(), "value2").ConfigureAwait(false);

            settixContext = new ApplicationContext("one.settix.consul.tests/test/*");
        };

        Because of = () => result = consul.GetAll(settixContext);

        It should_have_count_equal_to_3 = () => result.Count().ShouldEqual(3);

        static ISettixContext settixContext;
        static IEnumerable<DeployedSetting> result;
        static ConsulForSettix consul;
    }
}
