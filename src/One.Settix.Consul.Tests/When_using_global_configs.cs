using System;
using System.Collections.Generic;
using Machine.Specifications;

namespace One.Settix.Consul.Tests
{
    public class When_global_key_value_is_stored
    {
        Establish context = async () =>
        {
            consul = ConsulForSettixFactory.CreateWithGlobalContext();
            key = "global-key-" + Guid.NewGuid().ToString("N");
            await consul.SetGlobalAsync(key, "value").ConfigureAwait(false);
        };

        Because of = async () => valueFromConsul = await consul.GetGlobalAsync(key).ConfigureAwait(false);

        It should_get_value = () => valueFromConsul.ShouldEqual("value");

        static string key;
        static string valueFromConsul;
        static ConsulForSettix consul;
    }

    public class When_global_key_value_exist
    {
        Establish context = async () =>
        {
            consul = ConsulForSettixFactory.CreateWithGlobalContext();
            existingKey = "global-key-exists-" + Guid.NewGuid().ToString("N");
            missingKey = "global-key-missing-" + Guid.NewGuid().ToString("N");
            await consul.SetGlobalAsync(existingKey, "value").ConfigureAwait(false);
        };

        Because of = async () =>
        {
            keyExist = await consul.ExistsGlobalAsync(existingKey).ConfigureAwait(false);
            keyNotExist = await consul.ExistsGlobalAsync(missingKey).ConfigureAwait(false);
        };

        It should_be_true_for_existing_key = () => keyExist.ShouldBeTrue();

        It should_be_false_for_missing_key = () => keyNotExist.ShouldBeFalse();

        static string existingKey;
        static string missingKey;
        static bool keyExist;
        static bool keyNotExist;
        static ConsulForSettix consul;
    }

    public class When_global_key_value_is_deleted
    {
        Establish context = async () =>
        {
            consul = ConsulForSettixFactory.CreateWithGlobalContext();
            key = "global-key-delete-" + Guid.NewGuid().ToString("N");
            await consul.SetGlobalAsync(key, "value").ConfigureAwait(false);
            await consul.DeleteGlobalAsync(key).ConfigureAwait(false);
        };

        Because of = async () => exception = await Catch.ExceptionAsync(async () => await consul.GetGlobalAsync(key).ConfigureAwait(false)).ConfigureAwait(false);

        It should_throw_key_not_found = () => exception.ShouldBeOfExactType<KeyNotFoundException>();

        static string key;
        static Exception exception;
        static ConsulForSettix consul;
    }
}
