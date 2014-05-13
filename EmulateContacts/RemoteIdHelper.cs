using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.PersonalInformation;

namespace EmulateContacts
{
    class RemoteIdHelper
    {
        // Key used to store the app's GUID
        private const string ContactStoreLocalInstanceIdKey = "LocalInstanceId";

        // Create a GUID and store it in the ContactStore's extended properties collection. If the GUID
        // has already been set, it is not overwritten, so there is no danger in calling this method
        // multiple times.
        public async Task SetRemoteIdGuid(ContactStore store)
        {
            IDictionary<string, object> properties;
            properties = await store.LoadExtendedPropertiesAsync().AsTask<IDictionary<string, object>>();
            if (!properties.ContainsKey(ContactStoreLocalInstanceIdKey))
            {
                // The given store does not have a local instance id so set one against store extended properties
                Guid guid = Guid.NewGuid();
                properties.Add(ContactStoreLocalInstanceIdKey, guid.ToString());
                System.Collections.ObjectModel.ReadOnlyDictionary<string, object> readonlyProperties = new System.Collections.ObjectModel.ReadOnlyDictionary<string, object>(properties);
                await store.SaveExtendedPropertiesAsync(readonlyProperties).AsTask();
            }
        }

        /// <summary>
        /// Prepends the supplied remote ID with the GUID for the application in order to make sure
        /// that the remote ID is unique across all applications.
        /// </summary>
        /// <param name="store">The app's contact store. The remote ID Guid is stored in the store's
        /// extended properties collection.</param>
        /// <param name="remoteId">The remote ID to be prepended with the app's GUID.</param>
        /// <returns></returns>
        public async Task<string> GetTaggedRemoteId(ContactStore store, string remoteId)
        {
            string taggedRemoteId = string.Empty;

            System.Collections.Generic.IDictionary<string, object> properties;
            properties = await store.LoadExtendedPropertiesAsync().AsTask<System.Collections.Generic.IDictionary<string, object>>();
            if (properties.ContainsKey(ContactStoreLocalInstanceIdKey))
            {
                taggedRemoteId = string.Format("{0}_{1}", properties[ContactStoreLocalInstanceIdKey], remoteId);
            }
            else
            {
                // handle error condition
            }

            return taggedRemoteId;
        }

        /// <summary>
        /// Removes the prepended GUID from the supplied remote ID.
        /// </summary>
        /// <param name="store">The app's contact store. The remote ID Guid is stored in the store's
        /// extended properties collection.</param>
        /// <param name="taggedRemoteId">The remote ID from which the app's GUID should be removed.</param>
        /// <returns></returns>
        public async Task<string> GetUntaggedRemoteId(ContactStore store, string taggedRemoteId)
        {
            string remoteId = string.Empty;

            System.Collections.Generic.IDictionary<string, object> properties;
            properties = await store.LoadExtendedPropertiesAsync().AsTask<System.Collections.Generic.IDictionary<string, object>>();
            if (properties.ContainsKey(ContactStoreLocalInstanceIdKey))
            {
                string localInstanceId = properties[ContactStoreLocalInstanceIdKey] as string;
                if (taggedRemoteId.Length > localInstanceId.Length + 1)
                {
                    remoteId = taggedRemoteId.Substring(localInstanceId.Length + 1);
                }
            }
            else
            {
                // handle error condition
            }

            return remoteId;
        }

    }
}
