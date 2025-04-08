using System;
using System.Collections.Generic;
using Machine.Specifications;

namespace One.Settix.Consul.Tests
{
    public class When_key_value_is_deleted
    {
        Establish context = async () =>
        {
            consul = ConsulForSettixFactory.Create();
            await consul.SetAsync("key".CreateSettixRawKey(), "value").ConfigureAwait(false);
            await consul.GetAsync("key".CreateSettixRawKey()).ConfigureAwait(false);
            await consul.DeleteAsync("key".CreateSettixRawKey()).ConfigureAwait(false);
        };

        Because of = async () => exception = await Catch.ExceptionAsync(async () => await consul.GetAsync("key".CreateSettixRawKey()).ConfigureAwait(false)).ConfigureAwait(false);

        It should_be_able_to_find_key_value = () => exception.ShouldBeOfExactType<KeyNotFoundException>();

        static Exception exception;
        static ConsulForSettix consul;
    }
}
